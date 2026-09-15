// Partial split:
//   PlayerMovement.cs          fields + Ready + Source physics step
//   PlayerMovement.Camera.cs   look / fov / bob / labels
//   PlayerMovement.Stance.cs   crouch / stand
//   PlayerMovement.Probes.cs   vault / ladder / wall / rays
//   PlayerMovement.Impulse.cs  jump / lurch / recoil / shockwave
// Same type, no extra using needed. Sibling motors: SourceMove, HypersteelPhysSauce.

public enum CameraState
{
	Normal,
	Freelooking,
	Wallrunning,
	Ladder,
}

public partial class PlayerMovement : CharacterBody3D
{
	public GodotParadiseFiniteStateMachine FSM;
	private Node3D _head;
	private Node3D _neck;
	private Node3D _eyes;
	private Node3D _mesh;

	private Vector3 _lastPhysicsPos;
	[Export] private Node3D _resetPosition;
	[Export] private CollisionShape3D _standingCollider;
	[Export] private AnimationTree _animator;
	[Export] private bool _physicsInterpolate = true;

	public CameraState camState;

	[ExportCategory("Movement")]
	[Export] public float walkingSpeed {private set; get;} = 5.0f;
	[Export] public float sprintingSpeed {private set; get;} = 8.0f;
	[Export] public float maxSpeed {private set; get;} = 12f;
	[Export] public float accelerationRate {private set; get;} = 1.0f;
	[Export] public float lerpSpeed {private set; get;} = 10.0f; // Gradually changes a value. (Adding smoothing to values)
	[Export] private float _airLerpSpeed = 3.0f;

	[ExportCategory("Source Movement")]
	[Export] public bool sourceMovement {private set; get;} = true;
	[Export] public float sourceGroundAccelerate {private set; get;} = 10f;
	[Export] public float sourceAirAccelerate {private set; get;} = 100f;
	[Export] public float sourceAirSpeedCap {private set; get;} = 1.0f;
	[Export] public float sourceFriction {private set; get;} = 4f;
	[Export] public float sourceStopSpeed {private set; get;} = 1.5f;
	[Export] public float sourceMaxVelocity {private set; get;} = 350f;
	[Export] public float sourceFloorMaxAngle {private set; get;} = Mathf.DegToRad(45f);
	public bool sourceOnFloor = false;
	public bool ActuallyGrounded { get; private set; }
	public bool InCoyote { get; private set; }
	public bool OnGround => sourceMovement ? (sourceOnFloor || InCoyote) : IsOnFloor();

	[ExportGroup("Hypersteel Sauce")]
	[Export] public HypersteelPhysSauce sauce;

	[ExportCategory("Jumping")]
	[Export] private float _jumpVelocity = 4.5f;
	[Export] private float _coyoteTime = 0.5f;
	[Export] private int _jumps = 1;
	private int _jumpsDone = 0;

	public float currentSpeed = 5.0f;
	public float momentum {set; get;} = 0.0f;
	public float airTime = 0.0f;
	public Vector3 direction = Vector3.Zero;
	public Vector2 inputDirection = Vector2.Zero;
	public Vector3 lastVelocity = Vector3.Zero;
	public Vector3 playerVelocity = Vector3.Zero;

	// private rotations
	private float _rotationX = 0f;
	private float _rotationZ = 0f;

	[ExportSubgroup("Z Tilt")]
	[Export] private float _zRotationLerp = 7f;
	[Export] private float _zClamp = 5f;

	// Events
	public static event Action<Vector3> VelocityChange; // Event that constantly fires when velocity changes


	[ExportSubgroup("Crouching")]
	[Export] private CollisionShape3D _crouchingCollider;
	[Export] public RayCast3D ceilingRay {private set; get;}
	[Export] public float crouchingSpeed {private set; get;} = 3.0f;
	[Export(PropertyHint.Range, "0.25f, 0.75f")] private float _crouchingDepth = 0.5f;
	private float _initialDepth;


	[ExportSubgroup("Sliding")]
	[Export] public float slideTimerMax {private set; get;} = 1.0f;
	[Export] public float slideSpeed  {private set; get;}= 10.0f;
	public float slideTimer = 0.0f;
	public Vector2 slideVector = Vector2.Zero;
	public Basis slideBasis;
	public float initialRotationY;

	[ExportSubgroup("Vaulting")]
	[Export] private RayCast3D _vaultRay;
	[Export] private RayCast3D _vaultCheck;
	[Export] private ShapeCast3D _vaultCast;
	[Export] public ShapeCast3D stepCast;
    [Export] public float vaultMomentum {private set; get;}
	[Export] public float vaultJumpVelocity {private set; get;}
	private Vector3 _vaultProjection = Vector3.Zero;
	private Vector3 _vaultPoint = Vector3.Zero;

	[ExportSubgroup("Wall running")]
	[Export] public float wallRunTime {private set; get;} = 3f;
	[Export] public float wallRunSpeed {private set; get;} = 15f;
	[Export] public float wallFrictionCoefficient {private set; get;} = 0.5f;
	[Export] public float wallJumpSpeed {private set; get;} = 4.5f;
	
	public float wallRunTimer = 0.0f;
	private PlayerWallrun _wallRunStateNode;


	[ExportSubgroup("Vertical wallrun")]
	[Export] public float verticalRunHeight {private set; get;} = 4f; // in metres

	[ExportSubgroup("Ladder")]
	[Export] public float ladderSpeed;
	public bool isLadder;
	public Ladder currentLadder;

	[ExportSubgroup("Head Bobbing")]
	[Export] private float _headBobSprintSpeed = 22.0f;
	[Export] private float _headBobWalkingSpeed = 14.0f;
	[Export] private float _headBobCrouchSpeed = 10.0f;
	[Export] private float _headBobWallrunSpeed = 48.0f;
	

	[Export] private float _headBobSprintIntensity = 0.2f; //in centimetres
	[Export] private float _headBobWalkIntensity = 0.1f;
	[Export] private float _headBobCrouchIntensity = 0.05f;
	[Export] private float _headBobWallrunIntensity = 0.4f;
	

	private Vector2 _headBobVector = Vector2.Zero; // Keep track of side to side and up and down of bob
	private float _headBobIndex = 0.0f; // Keep track of our head bob index along the sin wave
	private float _headBobCurrentIntensity = 0.0f;
	

	[ExportSubgroup("Sensitivity")]
	[Export(PropertyHint.Range, "0, 1,")] public float mouseSensitivityX = 0.4f;
	[Export(PropertyHint.Range, "0, 1,")] public float mouseSensitivityY = 0.4f;


	[ExportSubgroup("Free Looking")]
	[Export] private float _freeLookTilt = 0.3f;

	[ExportSubgroup("Field of View")]
	[Export] private Camera3D _camera;
	[Export] private float _maxFov = 98f;
	private float _minFov = 0f;
	[Export] private float _velocityExponent = 2.0f;
	[Export] private float _maxPlayerVelocity = 15f;
	[Export] private float _fovLerpSpeed = 2.0f;


	[ExportSubgroup("Interface")]
	[Export] private Label _speedLabel;
	[Export] private Label _momentumLabel;
	[Export] private Label _stateLabel;
	[Export] private Label _animationLabel;
	[Export] private Label _desiredSpeedLabel;
	[Export] private Label _previousStateLabel;

	// inputs
	public bool sprintAction {private set; get;} = false;
	private bool _previousSprintAction;
	private float _yawDelta;
	private float _timeOnGround;
	private float _bhopIgnoreLeft;
	private bool _wasGrounded;


	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    public override void _Ready()
    {
		FSM = GetNode<GodotParadiseFiniteStateMachine>("FSM");
		_wallRunStateNode = (PlayerWallrun)FSM.GetStateByName("PlayerWallrun");

		_head = GetNode<Node3D>("Mesh/Neck/Head");
		_eyes = GetNode<Node3D>("Mesh/Neck/Head/Eyes");
		_neck = GetNode<Node3D>("Mesh/Neck");
		_mesh = GetNode<Node3D>("Mesh");

		_lastPhysicsPos = GlobalTransform.Origin;

		_initialDepth = _head.Position.Y;

		_minFov = _camera.Fov;
		sauce ??= new HypersteelPhysSauce();
        Input.MouseMode = Input.MouseModeEnum.Captured;

		// Events
		PlayerAir.PlayerLanded += ResetJumps;
    }

    public override void _ExitTree()
    {
        PlayerAir.PlayerLanded -= ResetJumps;
    }

	public override void _PhysicsProcess(double delta)
	{
		Vector2 inputDir = Input.GetVector("left", "right", "forward", "backward");
		inputDirection = inputDir;

		VelocityChange?.Invoke(Velocity); // Invoke change in velocity event

		HandleAnimation();

		sprintAction = _previousSprintAction;

		if (OnGround)
			sprintAction = !Input.IsActionPressed("sprint");

		// Handle free looking
		if (Input.IsActionPressed("free_look") || FSM.CurrentState is PlayerSlide)
		{
			camState = CameraState.Freelooking;

			// Slide Tilt
			if (FSM.CurrentState is PlayerSlide)
			{
				Vector3 eyeRotation = _eyes.Rotation;
				eyeRotation.Z = -Mathf.DegToRad(7f);
				_eyes.Rotation = _eyes.Rotation.Lerp(eyeRotation, 1.0f - Mathf.Pow(0.5f, (float)delta * lerpSpeed));
			}
			else
			{
				Vector3 eyeRotation = _eyes.Rotation;
				eyeRotation.Z = -Mathf.DegToRad(_neck.Rotation.Y * _freeLookTilt);
				_eyes.Rotation = eyeRotation;
			}
		}
		// If not free looking return to normal camera state
		else
		{
			if (FSM.CurrentState is not PlayerWallrun && FSM.CurrentState is not PlayerLadder)
			{
				camState = CameraState.Normal;

				Vector3 neckRot = new Vector3(0f, 0f, _neck.Rotation.Z);
				_neck.Rotation = _neck.Rotation.Lerp(neckRot, 1.0f - Mathf.Pow(0.5f, (float)delta * lerpSpeed));

				Vector3 eyeRot = new Vector3(_eyes.Rotation.X, _eyes.Rotation.Y, 0f);
				_eyes.Rotation = _eyes.Rotation.Lerp(eyeRot, 1.0f - Mathf.Pow(0.5f, (float)delta * lerpSpeed));
			}
		}

		// Handle head bob
		switch (FSM.CurrentState)
		{
			case PlayerWalk:
				_headBobCurrentIntensity = _headBobWalkIntensity;
				_headBobIndex += _headBobWalkingSpeed * (float)delta;
				break;
			
			case PlayerSprint:
				_headBobCurrentIntensity = _headBobSprintIntensity;
				_headBobIndex += _headBobSprintSpeed * (float)delta;
				break;
			
			case PlayerCrouch:
				_headBobCurrentIntensity = _headBobCrouchIntensity;
				_headBobIndex += _headBobCrouchSpeed* (float)delta;
				break;

			case PlayerWallrun:
				_headBobCurrentIntensity = _headBobWallrunIntensity;
				_headBobIndex += _headBobWallrunSpeed * (float)delta;
				break;

			default:
				break;
		}

		if ((OnGround && FSM.CurrentState is not PlayerSlide && inputDir != Vector2.Zero))
		{
			Vector2 headBob;

			headBob.Y = Mathf.Sin(_headBobIndex);
			headBob.X = Mathf.Sin(_headBobIndex / 2) + 0.5f;

			_headBobVector = headBob;
			
			Vector3 eyes = _eyes.Position;
			
			eyes.Y = Mathf.Lerp(eyes.Y, _headBobVector.Y * (_headBobCurrentIntensity / 2.0f), 1.0f - Mathf.Pow(0.5f, (float)delta * lerpSpeed));
			eyes.X = Mathf.Lerp(eyes.X, _headBobVector.X * _headBobCurrentIntensity, 1.0f - Mathf.Pow(0.5f, (float)delta * lerpSpeed));

			_eyes.Position = eyes;
		}
		else
		{
			Vector3 eyes = _eyes.Position;
			eyes.Y = Mathf.Lerp(eyes.Y, 0.0f, 1.0f - Mathf.Pow(0.5f, (float)delta * lerpSpeed));
			eyes.X = Mathf.Lerp(eyes.X, 0.0f, 1.0f - Mathf.Pow(0.5f, (float)delta * lerpSpeed));

			_eyes.Position = eyes;
		}

		playerVelocity = Velocity;

		bool specialMove = FSM.CurrentState is PlayerVault
			|| FSM.CurrentState is PlayerWallrun
			|| FSM.CurrentState is PlayerLadder
			|| FSM.CurrentState is PlayerVerticalWallrun;

		Vector3 wishDir = SourceMove.WishDirection(Transform.Basis, inputDir);
		direction = wishDir.LengthSquared() > 0.0001f ? wishDir : direction;

		if (FSM.CurrentState is not PlayerVault || FSM.CurrentState is not PlayerWallrun
			|| FSM.CurrentState is not PlayerLadder || FSM.CurrentState is not PlayerVerticalWallrun)
			HandleJump(_jumpVelocity);

		if (sourceMovement && !specialMove)
		{
			float dt = (float)delta;
			float wishSpeed = Mathf.Max(currentSpeed, 0.01f);
			bool grounded = sourceOnFloor;
			ActuallyGrounded = grounded;

			float coyoteDur = sauce != null ? sauce.coyoteTime : _coyoteTime;
			if (grounded)
			{
				airTime = 0f;
				_timeOnGround += dt;
				InCoyote = false;
				if (!_wasGrounded)
				{
					float hs = new Vector3(playerVelocity.X, 0f, playerVelocity.Z).Length();
					if (sauce != null && sauce.bhopEnabled && hs < sauce.highVelocity)
					{
						_bhopIgnoreLeft = sauce.bhopFrictionIgnore;
						if (sauce.lurchEnabled)
							SourceMove.LurchIntoWish(ref playerVelocity, wishDir, sauce.bhopLurch);
					}
				}
			}
			else
			{
				airTime += dt;
				_timeOnGround = 0f;
				InCoyote = airTime < coyoteDur;
			}

			if (sauce != null && sauce.pivotEnabled)
			{
				float yawRate = Mathf.Abs(_yawDelta * mouseSensitivityX) / Mathf.Max(dt, 0.0001f);
				SourceMove.PivotTowardWish(ref playerVelocity, wishDir, yawRate, sauce.pivotScalar, sauce.pivotMax, dt);
			}

			if (grounded || (InCoyote && sauce != null && sauce.coyoteAffectsFriction))
			{
				float friction = sourceFriction;
				if (FSM.CurrentState is PlayerSlide)
					friction *= 0.15f;
				if (sauce != null && sauce.highSpeedFrictionCut)
				{
					float hs = new Vector3(playerVelocity.X, 0f, playerVelocity.Z).Length();
					if (hs >= sauce.highVelocity)
						friction *= sauce.highVelFrictionMul;
					else
					{
						float ramp = Mathf.Clamp(_timeOnGround / Mathf.Max(sauce.frictionRampTime, 0.001f), 0f, 1f);
						friction *= Mathf.Lerp(sauce.highVelFrictionMul, 1f, ramp);
					}
				}
				if (_bhopIgnoreLeft > 0f)
				{
					friction = 0f;
					_bhopIgnoreLeft -= dt;
				}
				SourceMove.Friction(ref playerVelocity, dt, friction, sourceStopSpeed);
				SourceMove.Accelerate(ref playerVelocity, wishDir, wishSpeed, sourceGroundAccelerate, dt);
			}
			else
			{
				playerVelocity.Y -= gravity * dt;
				if (sauce != null && sauce.airDragEnabled)
				{
					float keep = FSM.CurrentState is PlayerSlide ? sauce.airFrictionSlide : sauce.airFrictionSprint;
					SourceMove.AirDrag(ref playerVelocity, keep, dt);
					SourceMove.AirAccelerate(ref playerVelocity, wishDir, wishSpeed, sourceAirAccelerate, sourceAirSpeedCap, dt);
				}
				else
				{
					SourceMove.AirAccelerate(ref playerVelocity, wishDir, wishSpeed, sourceAirAccelerate, sourceAirSpeedCap, dt);
				}
			}

			SourceMove.ClampVelocity(ref playerVelocity, sourceMaxVelocity);
			Velocity = playerVelocity;
			var hit = SourceMove.MoveAndSlideOwn(this, ref playerVelocity, UpDirection, sourceFloorMaxAngle, gravity, sauce);
			sourceOnFloor = hit.OnFloor;
			ActuallyGrounded = hit.OnFloor;
			Velocity = playerVelocity;
			_wasGrounded = sourceOnFloor;
		}
		else
		{
			// Wallrun / vault / ladder keep the old Hypersteel write so their impulses stick.
			if (IsOnFloor())
			{
				direction = direction.Lerp((Transform.Basis * new Vector3(inputDir.X, 0f, inputDir.Y)).Normalized(),
					1.0f - Mathf.Pow(0.5f, (float)delta * lerpSpeed));
			}
			else
			{
				if (!FSM.CurrentStateNameIs("PlayerWallrun") && !FSM.CurrentStateNameIs("PlayerLadder"))
				{
					playerVelocity.Y -= gravity * (float)delta;

					if (inputDir != Vector2.Zero)
					{
						direction = direction.Lerp((Transform.Basis * new Vector3(inputDir.X, 0f, inputDir.Y)).Normalized(),
							1.0f - Mathf.Pow(0.5f, (float)delta * _airLerpSpeed));
					}
				}
			}

			if (direction != Vector3.Zero)
			{
				if (FSM.CurrentState is not PlayerLadder)
				{
					playerVelocity.X = direction.X * currentSpeed;
					playerVelocity.Z = direction.Z * currentSpeed;
				}
			}
			else
			{
				playerVelocity.X = Mathf.MoveToward(Velocity.X, 0, currentSpeed);
				playerVelocity.Z = Mathf.MoveToward(Velocity.Z, 0, currentSpeed);
			}

			Velocity = playerVelocity;
			MoveAndSlide();
			sourceOnFloor = IsOnFloor();
		}

		_lastPhysicsPos = GlobalTransform.Origin;
		lastVelocity = playerVelocity;
		
		_previousSprintAction = sprintAction;
		_yawDelta = 0f;
	}

	private void OnCollisionCheckerAreaEntered(Area3D area)
	{
		string group = CollisionChecker.GetGroupFromBody(area);

		// Check if current group is ladder
		if (group == "Ladder")
		{
			isLadder = true;
			currentLadder = area.GetParent<Ladder>();
		}
	}

	private void OnCollisionCheckerAreaExited(Area3D area)
	{
		string group = CollisionChecker.GetGroupFromBody(area);

		if (group == "Ladder")
		{
			isLadder = false;
		}
	}
}

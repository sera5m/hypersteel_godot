using Godot;

namespace Hypersteel.Damage;

/// <summary>Event-updated Δhp samples. No _Process. DoT sums until Conclude.</summary>
public sealed class DamageWatch
{
	public bool Enabled;
	public float WindowSec = 1f;
	public float BurstThreshold = 40f;
	public float DripThreshold = 2f;

	float _dotAcc;
	bool _dotOpen;
	ulong _dotStartMsec;
	ulong _lastMsec;
	float _windowTaken;
	ulong _windowStart;

	public float LastBurstPerSec { get; private set; }
	public bool WasBurst { get; private set; }
	public bool WasDrip { get; private set; }

	public void Sample(float taken, ulong nowMsec)
	{
		if (!Enabled) return;
		if (_windowStart == 0 || (nowMsec - _windowStart) > (ulong)(WindowSec * 1000f))
		{
			_windowStart = nowMsec;
			_windowTaken = 0f;
		}
		_windowTaken += taken;
		_lastMsec = nowMsec;
		float perSec = _windowTaken / WindowSec;
		LastBurstPerSec = perSec;
		WasBurst = perSec >= BurstThreshold;
		WasDrip = taken > 0f && taken <= DripThreshold && !WasBurst;
	}

	public void DotAdd(float amount, ulong nowMsec)
	{
		if (!Enabled) return;
		if (!_dotOpen)
		{
			_dotOpen = true;
			_dotStartMsec = nowMsec;
			_dotAcc = 0f;
		}
		_dotAcc += amount;
		_lastMsec = nowMsec;
	}

	public float ConcludeDot(ulong nowMsec)
	{
		if (!_dotOpen) return 0f;
		float total = _dotAcc;
		Sample(total, nowMsec);
		_dotOpen = false;
		_dotAcc = 0f;
		return total;
	}
}

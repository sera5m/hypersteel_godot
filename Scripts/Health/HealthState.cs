using System;
using System.Collections.Generic;
using Godot;

namespace Hypersteel.Health;

public sealed class HealthState
{
	public readonly HealthRuleConfs Rules;
	public float Shield { get; private set; }
	public bool Dead { get; private set; }

	readonly Dictionary<BodySegment, float> _hp = new();
	readonly Dictionary<string, BodySegment> _bones = new(StringComparer.OrdinalIgnoreCase);
	float _iFrameLeft;

	public HealthState(HealthRuleConfs rules)
	{
		Rules = rules ?? new HealthRuleConfs();
		Reset();
	}

	public void Reset()
	{
		Dead = false;
		Shield = Rules.maxShield;
		_iFrameLeft = 0f;
		_hp.Clear();
		foreach (BodySegment s in Enum.GetValues(typeof(BodySegment)))
			_hp[s] = Rules.MaxFor(s);
	}

	public float HpOf(BodySegment s) => _hp.TryGetValue(s, out float v) ? v : 0f;

	public float TotalHp
	{
		get
		{
			float sum = 0f;
			foreach (var kv in _hp) sum += Math.Max(0f, kv.Value);
			return sum;
		}
	}

	public void MapSkeleton(Skeleton3D skeleton)
	{
		_bones.Clear();
		if (Rules.boneMap != null)
		{
			foreach (BoneSegmentBind bind in Rules.boneMap)
			{
				if (bind == null || string.IsNullOrEmpty(bind.bone)) continue;
				_bones[bind.bone] = bind.segment;
			}
		}

		if (skeleton == null) return;

		for (int i = 0; i < skeleton.GetBoneCount(); i++)
		{
			string name = skeleton.GetBoneName(i);
			if (_bones.ContainsKey(name)) continue;
			_bones[name] = Rules.SegmentForBone(name);
		}
	}

	public BodySegment SegmentFromBone(string boneName)
	{
		if (string.IsNullOrEmpty(boneName)) return Rules.defaultSegment;
		if (_bones.TryGetValue(boneName, out BodySegment s)) return s;
		return Rules.SegmentForBone(boneName);
	}

	public HitCalculations.Result Apply(in Hit hit, float dt)
	{
		if (Dead)
			return new HitCalculations.Result(0, 0, 0, 0, true);

		if (_iFrameLeft > 0f)
		{
			_iFrameLeft -= dt;
			return new HitCalculations.Result(0, 0, 0, 0, false);
		}

		BodySegment seg = hit.Bone.IsEmpty ? hit.Segment : SegmentFromBone(hit.Bone);
		float hp = HpOf(seg);
		var result = HitCalculations.Resolve(hit, Rules, seg, hp, Shield);

		Shield = Math.Max(0f, Shield - result.IntoShield);
		_hp[seg] = Math.Max(0f, hp - result.IntoHp);

		float torso = HpOf(BodySegment.Torso);
		float head = HpOf(BodySegment.Head);
		if (torso <= 0f || head <= 0f || result.Lethal)
			Dead = true;

		if (result.IntoHp > 0f || result.IntoShield > 0f)
			_iFrameLeft = Rules.iFrame;

		return result;
	}
}

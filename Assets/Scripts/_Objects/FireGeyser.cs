using UnityEditor;
using UnityEngine;
using UnityEngine.VFX;

public class FireGeyser : Entity
{
	[Space]
	[Header("Components")]
	[SerializeField] VisualEffect _FireVFX;

	[Header("Settings")]
	[SerializeField] float _TimePerAttack = 1.5f;
	[SerializeField] float _ActiveAttackTime = 3.0f;
	[SerializeField] LayerMask _InteractionLayers;

	[Header("Debugging")]
	public float _Timer;

	BasicFSM<Entity> _FSM;

	public override void Awake()
	{
		base.Awake();

		FlagsHelper.Unset(ref _Flags, EntityFlags.ToDestroyOnDeath);

		_FSM = new();
		_FSM.AddState(new WaitingState((int)FSMStates.Waiting));
		_FSM.AddState(new AttackState((int)FSMStates.Attacking));
		_FSM.AddState(new DeathState((int)FSMStates.Death));

		_FSM.SetState((int)FSMStates.Waiting, this);
	}

	public override void Update()
	{
		base.Update();

		_FSM.ExecuteState(this);
	}

#if UNITY_EDITOR
	public override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();

		if (_Transform != null)
		{
			if (_FSM != null && _FSM.IsCurrentStateValid())
			{
				Handles.Label(_Transform.position + Vector3.up * 5.0f, _FSM.GetCurrentState()._Name);
			}

			Handles.Label(_Transform.position + Vector3.up * 4.0f, _Flags.ToString());
		}
	}
#endif

	public class AttackState : BasicFSMState<Entity>
	{
		public AttackState(int stateIndex) : base(stateIndex, "Attack State")
		{
		}

		public override void Cleanup(Entity ent)
		{
			FireGeyser obj = (FireGeyser)ent;
			obj.FireFX_Stop();
		}

		public override void Execute(Entity ent)
		{
			FireGeyser obj = (FireGeyser)ent;

			obj._Timer += Time.deltaTime;

			if (obj.GetCurrentHealth() > 0.0f && obj._Timer <= obj.GetActiveAttackTime())
			{
				obj.FireFX_CheckCollision();
				return;
			}

			obj.FireFX_Stop();

			if (obj.GetCurrentHealth() > 0.0f)
			{
				obj._FSM.SetState((int)FSMStates.Waiting, ent);
			}
			else
			{
				obj._FSM.SetState((int)FSMStates.Death, ent);
			}
		}

		public override void Start(Entity ent, StateArg arg)
		{
			FireGeyser obj = (FireGeyser)ent;

			obj._Timer = 0.0f;
			obj.FireFX_Start();
		}
	}

	public class DeathState : BasicFSMState<Entity>
	{
		public DeathState(int stateIndex) : base(stateIndex, "Dead State")
		{
		}

		public override void Cleanup(Entity ent)
		{
		}

		public override void Execute(Entity ent)
		{
		}

		public override void Start(Entity ent, StateArg arg)
		{
			FireGeyser obj = (FireGeyser)ent;

			FlagsHelper.Unset(ref obj._Flags, EntityFlags.IsAttackAvailable);

			obj.SetHealth(0.0f);
			obj.FireFX_Stop();

			// TODO: Create bomb effect
		}
	}

	enum FSMStates
	{
		Waiting = 0,
		Attacking,
		Death,
	}

	public class WaitingState : BasicFSMState<Entity>
	{
		public WaitingState(int stateIndex) : base(stateIndex, "Waiting State")
		{
		}

		public override void Cleanup(Entity ent)
		{
		}

		public override void Execute(Entity ent)
		{
			FireGeyser obj = (FireGeyser)ent;

			if (obj.GetCurrentHealth() <= 0.0f)
			{
				obj._FSM.SetState((int)FSMStates.Death, ent);
			}

			obj._Timer += Time.deltaTime;

			if (obj._Timer > obj.GetTimePerAttack())
			{
				obj._FSM.SetState((int)FSMStates.Attacking, ent);
			}
		}

		public override void Start(Entity ent, StateArg arg)
		{
			FireGeyser obj = (FireGeyser)ent;
			WaitingStateArg waitArg = (WaitingStateArg)arg;

			obj._Timer = arg != null ? waitArg._WaitTimer : 0.0f;
			obj.FireFX_Stop();
		}
	}

	public class WaitingStateArg : StateArg
	{
		public float _WaitTimer;
	}

	#region Public Functions

	public float GetTimePerAttack()
	{
		return _TimePerAttack + Random.Range(0.5f, 2.5f);
	}

	public float GetActiveAttackTime()
	{
		return _ActiveAttackTime + Random.Range(0.5f, 1.5f);
	}

	public void FireFX_CheckCollision()
	{
		Collider[] colls = Physics.OverlapCapsule(
			_Transform.position,
			_Transform.position + Vector3.up * 2.5f,
			1.0f,
			_InteractionLayers,
			QueryTriggerInteraction.Ignore
		);

		foreach (Collider c in colls)
		{
			if (c.TryGetComponent(out IInteraction i))
			{
				i.ActFire();
			}
		}

		// Sometimes the collection may get modified
		for (int i = 0; i < _AttachedPikmin.Count; i++)
		{
			if (i <= _AttachedPikmin.Count)
			{
				_AttachedPikmin[i].ActFire();
			}
		}
	}

	public void FireFX_Start()
	{
		_FireVFX.Play();
	}

	public void FireFX_Stop()
	{
		_FireVFX.Stop();
	}

	#endregion
}

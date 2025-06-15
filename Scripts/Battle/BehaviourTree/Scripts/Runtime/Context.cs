using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TheKiwiCoder {

    // The context is a shared object every node has access to.
    // Commonly used components and subsytems should be stored here
    // It will be somewhat specfic to your game exactly what to add here.
    // Feel free to extend this class 
    public class Context {
        public GameObject gameObject;
        public Transform transform;
        public Animator animator;
        public Rigidbody physics;
        public NavMeshAgent agent;
        public SphereCollider sphereCollider;
        public BoxCollider boxCollider;
        public CapsuleCollider capsuleCollider;
        public CharacterController characterController;
        public BattleUnit battleUnit;
        public PlayerBattle PlayerBattle;
        public EnemyBattle EnemyBattle;
        public UI_BattlePopUp uI_BattlePopUp;
        public UI_EnemySlot uI_EnemySlot;
        // Add other game specific systems here

        public static Context CreateFromGameObject(GameObject gameObject) {
            // Fetch all commonly used components
            Context context = new Context();
            context.gameObject = gameObject;
            context.transform = gameObject.transform;
            context.animator = gameObject.GetComponent<Animator>();
            context.physics = gameObject.GetComponent<Rigidbody>();
            context.agent = gameObject.GetComponent<NavMeshAgent>();
            context.sphereCollider = gameObject.GetComponent<SphereCollider>();
            context.boxCollider = gameObject.GetComponent<BoxCollider>();
            context.capsuleCollider = gameObject.GetComponent<CapsuleCollider>();
            context.characterController = gameObject.GetComponent<CharacterController>();
            context.battleUnit = gameObject.GetComponent<BattleUnit>();
            context.PlayerBattle = gameObject.GetComponent<PlayerBattle>();
            context.EnemyBattle = gameObject.GetComponent<EnemyBattle>();
            UI_BattleTimeLineGrid uiBattle = Managers.UIManager.UIBattleGrid;
            context.uI_BattlePopUp = uiBattle.UI_battlepopup;
            context.uI_EnemySlot = uiBattle.UI_enemyslot;
            // Add whatever else you need here...

            return context;
        }
    }
}
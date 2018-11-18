using System;
using UnityEngine;


namespace UnityStandardAssets.CharacterController.ThirdPerson
{

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(Animator))]
    public class ThirdPersonCharacter : MonoBehaviour
    {
        //propriedades do editor (Public Serializable)
        [SerializeField] float m_MovingTurnSpeed = 360f;
        [SerializeField] float m_StationaryTurnSpeed = 180f;
        [SerializeField] float m_JumpPower = 12f;
        [Range(1f, 4f)] [SerializeField] float m_GravityMultiplier = 2f;
        [SerializeField] float m_RunCycleOffset = 0.2f;
        [SerializeField] float m_MoveSpeedMultplier = 1f;
        [SerializeField] float m_AnimSpeedMultiplier = 1f;
        [SerializeField] float m_GroudCheckDistance = 0.1f;
        public bool isStrafe;
        public bool isWeapon;

        //Componentes privados
        private Rigidbody m_Rigidbody;
        private Animator m_Animator;
        private CapsuleCollider m_Capsule;

        //Variaveis Float
        private float m_OrigGroundCHeckDistance;
        private float m_TurnAmout;
        private float m_CapsuleHeight;
        private float m_ForwardAmout;
        private const float k_Half = 0.5f;

        //Variaveis Vector3
        private Vector3 m_GroundNormal;
        private Vector3 m_CapsuleCenter;

        //Variaveis booleanas
        private bool m_IsGrounded;
        private bool m_Crouching;



        // Use this for initialization
        void Start()
        {
            //carrega as variaveis com os coomponentes do personagem
            m_Animator = gameObject.GetComponent<Animator>();
            m_Rigidbody = gameObject.GetComponent<Rigidbody>();
            m_Capsule = gameObject.GetComponent<CapsuleCollider>();
            
            //congela a rotação do personagem
            m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

            //Carrega variaveis
            m_CapsuleHeight = m_Capsule.height;
            m_CapsuleCenter = m_Capsule.center;
            m_OrigGroundCHeckDistance = m_GroudCheckDistance;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Move(Vector3 move, bool crouch, bool jump)
        {
            //verifica a direção de move e transforma de acordo com a posição do personagem
            move = transform.InverseTransformDirection(move);
            //verifica se o personagem está no chão
            CheckGroundStatus();
            //inclina a difeção baseando-se na inclinação do terreno
            move = Vector3.ProjectOnPlane(move, m_GroundNormal);

            //Verifica se o personagem está desarmado
            if (!isWeapon)
            {
                //recalcula rotação extra
                m_TurnAmout = Mathf.Atan2(move.x, move.z);
            }
            m_ForwardAmout = move.z;
            //aplica a rotação extra
            ApplyExtraTurnRotation();

            // se o personagem está tocando o chão podemos gerenciar o agachamento e o pulo
            if (m_IsGrounded){
                HandleGroundedMovement(crouch,jump);
            }
            //caso contrário somente podemos nos movimentar
            else
            {
                HandleAirbourneMovement();
            }

            //redimencionar a altura do collider quando o personagem estiver agachado
            ScaleCapsuleForCrounching(crouch);

            //
            PreventStandingLowHeadroom();

            //Atualizar a animação do personagem
            UpdateAnimator(move);



        }

        //Função para detectar agachamento
        private void ScaleCapsuleForCrounching(bool crouch)
        {
            //Se estiver no chão e tentar agachar
            if(m_IsGrounded && crouch)
            {
                //caso já estiver agachando apenas retorne
                if (m_Crouching) return;

                //diminui o tananho e o raio do capsule collider e atualiza a variavel de agachar como verdadeiro
                m_Capsule.height = m_Capsule.height / 2f;
                m_Capsule.center = m_Capsule.center / 2f;
                m_Crouching = true;
            }
            else
            {
                //Verifica se é possivel agachar ou continuar agachado.
                //Cria um raio usando como base a posiçãodo personagem subindo o equivalente a metade do raio do colidder.
                Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
                //Seta a variavel com a altura inicial da capsula - metade do raio da capsula
                float crouchRayLenght = m_CapsuleHeight - m_Capsule.radius * k_Half;

                //usa a fisica para criar uma esfera de detecção para ver se o personagem pode ficar agachado                
                if (Physics.SphereCast(crouchRay, m_Capsule.radius*k_Half, crouchRayLenght,Physics.AllLayers,QueryTriggerInteraction.Ignore))
                {
                    //seta a variavel agachado verdadeiro e retorna
                    m_Crouching = true;
                    return;
                }
                //Coloca os valores iniciais para resetar o tamanho do collider e retorna falseo para a variavel agachado
                m_Capsule.height = m_CapsuleHeight;
                m_Capsule.center = m_CapsuleCenter;
                m_Crouching = false;
            }
        }

        //previne de levantar em lugares que não cabem o personagem em pé
        private void PreventStandingLowHeadroom()
        {
            if (!m_Crouching)
            {
                //Verifica se é possivel agachar ou continuar agachado.
                //Cria um raio usando como base a posiçãodo personagem subindo o equivalente a metade do raio do colidder.
                Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
                //Seta a variavel com a altura inicial da capsula - metade do raio da capsula
                float crouchRayLenght = m_CapsuleHeight - m_Capsule.radius * k_Half;

                //usa a fisica para criar uma esfera de detecção para ver se o personagem pode ficar agachado                
                if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLenght, Physics.AllLayers, QueryTriggerInteraction.Ignore))
                {
                    //seta a variavel agachado verdadeiro e retorna
                    m_Crouching = true;
                }

            }
        }

        //função para passar os parametros para o Animator
        private void UpdateAnimator(Vector3 move)
        {
            //parametros de movimentação
            if (!isStrafe)
            {
                m_Animator.SetFloat(StaticAnimation.Forward,m_ForwardAmout,0.1f,Time.deltaTime);
                m_Animator.SetFloat(StaticAnimation.Turn, m_TurnAmout, 0.1f, Time.deltaTime);

            }
            else
            {
                //.. furura espansão "Armado";
            }

            //parametros de verificaçãao de chão e agachamento
            m_Animator.SetBool(StaticAnimation.Crouch, m_Crouching);
            m_Animator.SetBool(StaticAnimation.Onground, m_IsGrounded);
            if (!m_IsGrounded)
            {
                m_Animator.SetFloat(StaticAnimation.Jump, m_Rigidbody.velocity.y);
            }
            else
            {
                m_Animator.SetFloat(StaticAnimation.Jump, 0);
            }

            //Verifica qual perna está na frente para passar ao animator
            float runCycle = Mathf.Repeat(
                m_Animator.GetNextAnimatorStateInfo(0).normalizedTime + m_RunCycleOffset, 1);
            float jumpLeg = (runCycle < k_Half ? 1 : 1) * m_ForwardAmout;
            //Verifica se está no chão
            if (m_IsGrounded)
            {
                //atualiza o valor do pé durante o pulo
                m_Animator.SetFloat(StaticAnimation.JumpLeg, jumpLeg);
            }

            if(m_IsGrounded && move.magnitude > 0)
            {
                m_Animator.speed = m_AnimSpeedMultiplier;
            }
            else
            {
                m_Animator.speed = 1;
            }

        }

        //Gerencia quando o personagem estiver no ar
        private void HandleAirbourneMovement()
        {
            //Seta a gravidade com base na variavel definida pelo usuário
            Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;  
            m_Rigidbody.AddForce(extraGravityForce);

            //seta a variavel de distancia do chão caso o personagem estiver caindo
            m_GroudCheckDistance = m_Rigidbody.velocity.y < 0 ? m_OrigGroundCHeckDistance : 0.01f;
        }

        //Gerencia o movimento quando o personagem estiver no chão
        private void HandleGroundedMovement(bool crouch, bool jump)
        {
            //Gerencia quando o personagem precisa pular, mas não está agachado e está executando animações do estado "Grounded"
            //if(jump && !crouch && m_Animator.GetCurrentAnimatorStateInfo(0).IsName(StaticAnimation.StateGrounded))
            if (jump && !crouch)
            {
                //Pula - sobreescrevendo a força no exo vertical no Rigidbody
                m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_JumpPower, m_Rigidbody.velocity.z);
                //seta as variaveis relacionadas a detecção do chão.
                m_IsGrounded = false;
                m_Animator.applyRootMotion = false;
                m_GroudCheckDistance = 0.1f;
            }
            
        }

        //Adiciona rotação ao personagem
        private void ApplyExtraTurnRotation()
        {
            // se o personagem não estiver armado
            if (!isWeapon)
            {
                //rotaciona o personagem
                float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmout);
                transform.Rotate(0, m_TurnAmout * turnSpeed * Time.deltaTime, 0);
            }
        }

        //função executa sempre que há movimento no animator
        void OnAnimatorMove()
        {
            // se o personagem estiver no chão o rigidbody recebe as variaveis de movimento da animação
            if (m_IsGrounded && Time.deltaTime > 0)
            {
                Vector3 v = (m_Animator.deltaPosition * m_MoveSpeedMultplier) / Time.deltaTime;

                v.y = m_Rigidbody.velocity.y;
                m_Rigidbody.velocity = v;
            }
        }

        //função para verificar se o personagem está no chão
        private void CheckGroundStatus()
        {
            RaycastHit hitInfo;

#if UNITY_EDITOR

            Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * m_OrigGroundCHeckDistance));
#endif
            if(Physics.Raycast(transform.position + (Vector3.up * 0.1f),Vector3.down,out hitInfo, m_GroudCheckDistance))
            {
                m_GroundNormal = hitInfo.normal;
                m_IsGrounded = true;
                m_Animator.applyRootMotion = true;
            }
            else
            {
                m_IsGrounded = false;
                m_Animator.applyRootMotion = false;
            }
        }
    }

}
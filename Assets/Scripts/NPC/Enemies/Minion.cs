﻿using UnityEngine;
using System.Collections;
using CrowShadowManager;
using CrowShadowPlayer;

namespace CrowShadowNPCs
{
    public class Minion : Follower
    {
        public int healthLight = 300; //decrementa 1 por colisão
        public int healthMelee = 200;
        public int decrementFaca = 50, decrementBastao = 35, decrementPedra = 25;
        public float addPath = 0.5f; // quanto vai ser adicionado ao somatório das escolhas
        public float timeMaxPower = 3f; // tempo máximo que pode ficar colidindo com o minion para não ativar próximo poder
        public float timeMaxChangeVelocity = 6f, factorDivideSpeed = 1.2f; // tempo máximo com velocidade menor e fator para dividi-la
        public float timeInvertControls = 6f; // tempo adicional para ficar com o controle invertido

        MinionEmmitter emmitter;
        Player playerScript;
        Rigidbody2D playerRB;
        Renderer playerRenderer;
        AttackObject faca, bastao;
        ProtectionObject tampa, escudo;
        FarAttackObject pedra;
        FarAttackMiniGameObject papel;

        float timeLeftAttack = 0, timePower = 0, timeChangeVelocity = 0;
        int power = 0; // 1 - diminui velocidade, 2 - inverte controles, 3 - morre
        bool onCollision = false, changeVelocity = false;
        bool attackFlashlight = false, attackFaca = false, attackBastao = false, attackPedra = false, attackPapel = false;

        protected new void Start()
        {
            base.Start();
            distFollow = 0.1f;
            moveTowards = true;
            emmitter = GetComponentInParent<MinionEmmitter>();
            playerScript = player.GetComponent<Player>();
            playerRB = player.GetComponent<Rigidbody2D>();
            playerRenderer = player.GetComponent<Renderer>();
            faca = GameManager.instance.gameObject.transform.Find("Faca").gameObject.GetComponent<AttackObject>();
            bastao = GameManager.instance.gameObject.transform.Find("Bastao").gameObject.GetComponent<AttackObject>();
            tampa = GameManager.instance.gameObject.transform.Find("Tampa").gameObject.GetComponent<ProtectionObject>();
            escudo = GameManager.instance.gameObject.transform.Find("Escudo").gameObject.GetComponent<ProtectionObject>();
            pedra = GameManager.instance.gameObject.transform.Find("Pedra").gameObject.GetComponent<FarAttackObject>();
            papel = GameManager.instance.gameObject.transform.Find("Papel").gameObject.GetComponent<FarAttackMiniGameObject>();
        }

        protected new void Update()
        {
            // Ao patrulhar
            if (!followingPlayer)
            {
                // Se estiver correndo, aumenta a ára de busca
                if (playerScript.isRunning)
                {
                    circleCollider.radius = 0.8f;
                }
                else
                {
                    circleCollider.radius = 0.6f;
                }
            }
            else
            {
                // Condição quando está escondido
                if (!playerRenderer.enabled && playerRB.bodyType == RigidbodyType2D.Kinematic)
                {
                    FollowPlayer(false);
                }
            }

            if (attackFlashlight)
            {
                healthLight--;
            }
            else if (attackFaca && faca.active && timeLeftAttack <= 0)
            {
                timeLeftAttack = AttackObject.timeAttack;
                healthMelee -= decrementFaca;
            }
            else if (attackBastao && bastao.active && timeLeftAttack <= 0)
            {
                timeLeftAttack = AttackObject.timeAttack;
                healthMelee -= decrementBastao;
            }

            // Ao colidir
            if (onCollision && !GameManager.instance.blocked)
            {
                if (timePower > 0)
                {
                    timePower -= Time.deltaTime;
                }
                else
                {
                    // animaçãozinha de poderzinho atacando (pode ser uma luz)
                    if (GameManager.instance.playerProtected)
                    {
                        print("PROT");
                        if (Inventory.GetCurrentItemType() == Inventory.InventoryItems.TAMPA)
                        {
                            tampa.DecreaseLife();
                        }
                        else if (Inventory.GetCurrentItemType() == Inventory.InventoryItems.ESCUDO)
                        {
                            escudo.DecreaseLife();
                        }
                    }
                    else
                    {
                        timePower = timeMaxPower;
                        ActivatePower();
                    }
                }
            }

            // Mudança de velocidade do player
            if (changeVelocity)
            {
                if (timeChangeVelocity > 0)
                {
                    timeChangeVelocity -= Time.deltaTime;
                }
                else
                {
                    playerScript.movespeed = playerScript.movespeed * factorDivideSpeed;
                    changeVelocity = false;
                }
            }

            if (timeLeftAttack > 0)
            {
                timeLeftAttack -= Time.deltaTime;
            }

            if (healthLight <= 0)
            {
                GameManager.instance.pathCat += addPath;
                if (emmitter) emmitter.currentMinions--;
                Destroy(gameObject);
                // animação + som
            }
            else if (healthMelee <= 0)
            {
                GameManager.instance.pathBird += addPath;
                if (emmitter) emmitter.currentMinions--;
                Destroy(gameObject);
                // animação + som
            }

            base.Update();
        }

        private void ActivatePower()
        {
            print("ACTIVATE " + power);
            PlayAnimation("minionAttack");
            switch (power)
            {
                case 0:
                    timeChangeVelocity = timeMaxChangeVelocity;
                    changeVelocity = true;
                    playerScript.movespeed = playerScript.movespeed / factorDivideSpeed;
                    power++;
                    break;
                case 1:
                    playerScript.invertControlsTime += timeInvertControls;
                    power++;
                    break;
                case 2:
                    power = 0;
                    GameManager.instance.GameOver();
                    break;
                default:
                    break;
            }
        }

        protected new void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag.Equals("Player") && followingPlayer)
            {
                onCollision = true;
            }
            else
            {
                OnTriggerCalled(collision);
            }

            if ((collision.tag.Equals("Flashlight") && Inventory.IsCurrentItemType(Inventory.InventoryItems.FLASHLIGHT, true)) || collision.tag.Equals("Lamp"))
            {
                attackFlashlight = true;
            }
            else if (collision.tag.Equals("Faca"))
            {
                attackFaca = true;
            }
            else if (collision.tag.Equals("Bastao"))
            {
                attackBastao = true;
            }
            else if (collision.tag.Equals("Pedra") && pedra.active)
            {
                pedra.hitSuccess = true;
                healthMelee -= decrementPedra;
            }
            else if (collision.tag.Equals("Papel") && papel.active)
            {
                papel.hitSuccess = true;
                if (papel.achievedGoal)
                {
                    if (emmitter) emmitter.currentMinions--;
                    Destroy(gameObject);
                }
            }
        }

        protected new void OnTriggerStay2D(Collider2D collision)
        {
            
        }

        protected void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.tag.Equals("Player"))
            {
                onCollision = false;
            }

            if (collision.tag.Equals("Flashlight") || collision.tag.Equals("Lamp"))
            {
                attackFlashlight = false;
            }
            else if (collision.tag.Equals("Faca"))
            {
                attackFaca = true;
            }
            else if (collision.tag.Equals("Bastao"))
            {
                attackBastao = true;
            }
        }

        protected new void OnTriggerCalled(Collider2D collision)
        {
            if (hasActionPatroller)
            {
                print("ActionFollower: " + collision.tag);
                if (collision.gameObject.tag.Equals("Player"))
                {
                    if (followWhenClose && !followingPlayer)
                    {
                        circleCollider.radius = 0.2f;
                        FollowPlayer();
                    }
                }
            }
        }

        public void PlayAnimation(string anim, float time = 1f)
        {
            //print("ANIM:" + anim + time);
            animator.Play(anim);
            StartCoroutine(WaitCoroutineAnim(time));
        }

        IEnumerator WaitCoroutineAnim(float time)
        {
            Debug.Log("about to yield return WaitForSeconds(" + time + ")");
            yield return new WaitForSeconds(time);
            Debug.Log("Animation ended");
            animator.SetTrigger("changeDirection");
            yield break;
            //Debug.Log("You'll never see this"); // produces a dead code warning
        }
    }
}
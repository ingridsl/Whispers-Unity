﻿using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using CrowShadowManager;

namespace CrowShadowPlayer
{
    public class Flashlight : InventoryObject
    {
        public Inventory.InventoryItems item;

        private Player player;
        private Light lightComponent;
        private Collider2D colliderComponent;
        private CircleCollider2D circleCollider;
        
        private float rotationSpeed = 1f, timePressed = 0f, timeHoldButton = 0f;
        private bool changeDirectionTime = false, holdButton = false;

        new void Start()
        {
            player = GetComponentInParent<Player>();
            lightComponent = GetComponent<Light>();
            colliderComponent = GetComponent<Collider2D>();
            circleCollider = GetComponent<CircleCollider2D>();
            active = colliderComponent.enabled = lightComponent.enabled;
        }

        new void Update()
        {
            //0 = east, 1 = west, 2 = north, 3 = south
            if (Inventory.GetCurrentItemType() == item && !GameManager.instance.paused &&
                !GameManager.instance.blocked && !GameManager.instance.pausedObject)
            {
                if (CrossPlatformInputManager.GetButtonDown("keyUseObject") && !active)
                {
                    EnableFlashlight(true);
                    if (active)
                    {
                        transform.rotation = Quaternion.Euler((float)0.0, (float)0.0, (float)0.0);
                    }
                    timePressed = 0f;
                    holdButton = true;
                }
                else if (CrossPlatformInputManager.GetButtonDown("keyUseObject") && active)
                {
                    holdButton = false;
                    timeHoldButton = Time.time;
                }
                else if (CrossPlatformInputManager.GetButton("keyUseObject"))
                {
                    if ((Time.time - timeHoldButton) > 0.4 && !holdButton)
                    {
                        holdButton = true;
                    }

                    if (changeDirectionTime)
                    {
                        timePressed -= 4 * Time.deltaTime;
                    }
                    else
                    {
                        timePressed += 4 * Time.deltaTime;
                    }

                    if (timePressed >= 20f)
                    {
                        changeDirectionTime = true;
                    }
                    else if (timePressed <= 0f)
                    {
                        changeDirectionTime = false;
                    }
                }
                else if (CrossPlatformInputManager.GetButtonUp("keyUseObject") && active && !holdButton)
                {
                    EnableFlashlight(false);
                    timePressed = 0f;
                }
            }

            if (active)
            {
                if (Inventory.GetCurrentItemType() != item)
                {
                    EnableFlashlight(false);
                }

                lightComponent.spotAngle = 60 - timePressed;
                circleCollider.radius = 1f - (timePressed / 100f);

                switch (player.direction)
                {
                    case Player.Directions.EAST:
                        transform.localPosition = Vector3.Slerp(transform.localPosition, new Vector3(0f, -0.6f, 2f), rotationSpeed * Time.deltaTime);
                        Quaternion targetRotationE = Quaternion.Euler((float)180.0, (float)230.0 - timePressed, (float)0.0);
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotationE, rotationSpeed * Time.deltaTime);
                        colliderComponent.offset = new Vector2(-2f + (timePressed / 25f), 0);
                        break;
                    case Player.Directions.WEST:
                        transform.localPosition = Vector3.Slerp(transform.localPosition, new Vector3(0f, -0.6f, 2f), rotationSpeed * Time.deltaTime);
                        Quaternion targetRotationW = Quaternion.Euler((float)180.0, (float)130.0 + timePressed, (float)0.0);
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotationW, rotationSpeed * Time.deltaTime);
                        colliderComponent.offset = new Vector2(2f - (timePressed / 25f), 0);
                        break;
                    case Player.Directions.NORTH:
                        transform.localPosition = Vector3.Slerp(transform.localPosition, new Vector3(-0.45f, 0f, 2f), rotationSpeed * Time.deltaTime);
                        Quaternion targetRotationN = Quaternion.Euler((float)-45.0 + (timePressed / 2f), (float)0.0, (float)0.0);
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotationN, rotationSpeed * Time.deltaTime);
                        colliderComponent.offset = new Vector2(0, 2f - (timePressed / 25f));
                        break;
                    case Player.Directions.SOUTH:
                        transform.localPosition = Vector3.Slerp(transform.localPosition, new Vector3(-0.45f, 0f, 2f), rotationSpeed * Time.deltaTime);
                        Quaternion targetRotationS = Quaternion.Euler((float)45.0 - (timePressed / 2f), (float)0.0, (float)0.0);
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotationS, rotationSpeed * Time.deltaTime);
                        colliderComponent.offset = new Vector2(0, -2f + (timePressed / 25f));
                        break;
                    default:
                        break;
                }
            }

        }

        public void EnableFlashlight(bool e)
        {
            lightComponent.enabled = e;
            colliderComponent.enabled = e;
            active = e;
            if (active)
            {
                player.ChangeState((int)Player.States.FLASHLIGHT);
            }
            else
            {
                player.ChangeState((int)Player.States.DEFAULT);
            }
        }
    }
}
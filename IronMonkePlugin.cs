using BepInEx;
using System;
using UnityEngine;
using UnityEngine.XR;
using GorillaLocomotion;
using Utilla;

namespace IronMonke
{
    [ModdedGamemode]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class IronMonkePlugin : BaseUnityPlugin
    {
        bool inRoom;

        private XRNode rNode = XRNode.RightHand;
        private XRNode lNode = XRNode.LeftHand;

        Rigidbody RB;
        Transform rHandT;
        Transform lHandT;

        private float thrust;

        GameObject quitBox;
        Transform quitBoxT;

        void OnEnable()
        {
            HarmonyPatches.ApplyHarmonyPatches();
            Utilla.Events.GameInitialized += OnGameInitialized;

            RB.useGravity = true;
        }

        void OnDisable()
        {
            HarmonyPatches.RemoveHarmonyPatches();
            Utilla.Events.GameInitialized -= OnGameInitialized;

            RB.useGravity = true;
        }

        void OnGameInitialized(object sender, EventArgs e)
        {
            RB = Player.Instance.bodyCollider.attachedRigidbody;

            rHandT = Player.Instance.rightHandTransform;
            lHandT = Player.Instance.leftHandTransform;

            quitBox = GameObject.Find("QuitBox");
            quitBoxT = quitBox.transform;
            quitBoxT.position = new Vector3 (999999999, 999999999, 999999999);

        }

        void Update()
        {
            if(inRoom)
            {
                bool primaryRight;
                InputDevices.GetDeviceAtXRNode(rNode).TryGetFeatureValue(CommonUsages.primaryButton, out primaryRight);
                bool B = primaryRight;

                bool primaryLeft;
                InputDevices.GetDeviceAtXRNode(lNode).TryGetFeatureValue(CommonUsages.primaryButton, out primaryLeft);
                bool Y = primaryLeft;

                if(B)
                {
                    RB.AddForce(thrust * rHandT.right, ForceMode.Acceleration);
                }
                if(Y)
                {
                    RB.AddForce(-thrust * lHandT.right, ForceMode.Acceleration);
                }

                if(Y&&B)
                {
                    thrust = 4.1f;
                } else
                {
                    thrust = 7.3f;
                }

                bool wasInput = Y | B;
                if (wasInput) RB.useGravity = false;
                if (!wasInput) RB.useGravity = true;
            }
            
        }

        [ModdedGamemodeJoin]
        public void OnJoin(string gamemode)
        {
            inRoom = true;

            RB.useGravity = true;
        }

        [ModdedGamemodeLeave]
        public void OnLeave(string gamemode)
        {
            inRoom = false;

            RB.useGravity = true;
        }
    }
}

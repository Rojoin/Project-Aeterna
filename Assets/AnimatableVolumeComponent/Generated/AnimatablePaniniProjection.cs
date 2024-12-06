// <auto-generated />

using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace TsukimiNeko.AnimatableVolumeComponent
{
    [AnimatableOf(typeof(UnityEngine.Rendering.Universal.PaniniProjection))]
    [DisallowMultipleComponent]
    public class AnimatablePaniniProjection : AnimatableVolumeComponentBase
    {
        public override Type TargetType { get; } = typeof(UnityEngine.Rendering.Universal.PaniniProjection);

        public bool override_distance;
        public System.Single distance;
        public bool override_cropToFit;
        public System.Single cropToFit;


        private void WriteToVolumeComponent(UnityEngine.Rendering.Universal.PaniniProjection volumeComponent)
        {
            if (!volumeComponent) return;

            volumeComponent.active = active;
            volumeComponent.distance.overrideState = override_distance;
            volumeComponent.distance.value = distance;
            volumeComponent.cropToFit.overrideState = override_cropToFit;
            volumeComponent.cropToFit.value = cropToFit;

        }

        private void ReadFromVolumeComponent(UnityEngine.Rendering.Universal.PaniniProjection volumeComponent)
        {
            if (!volumeComponent) return;

            active = volumeComponent.active;
            override_distance = volumeComponent.distance.overrideState;
            distance = volumeComponent.distance.value;
            override_cropToFit = volumeComponent.cropToFit.overrideState;
            cropToFit = volumeComponent.cropToFit.value;

        }

        private void Reset()
        {
            var volume = GetComponent<Volume>();
            if (!volume || !volume.sharedProfile || !volume.sharedProfile.TryGet<UnityEngine.Rendering.Universal.PaniniProjection>(out var volumeComponent)) return;

            ReadFromVolumeComponent(volumeComponent);
        }

        public override void WriteToVolumeComponent()
        {
            if (!volumeHelper.TryGet<UnityEngine.Rendering.Universal.PaniniProjection>(out var volumeComponent)) return;

            WriteToVolumeComponent(volumeComponent);
        }

        public override void ReadFromVolumeComponent()
        {
            if (!volumeHelper.TryGet<UnityEngine.Rendering.Universal.PaniniProjection>(out var volumeComponent)) return;

            ReadFromVolumeComponent(volumeComponent);
        }
    }
}

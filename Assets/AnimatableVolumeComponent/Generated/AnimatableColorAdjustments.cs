// <auto-generated />

using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace TsukimiNeko.AnimatableVolumeComponent
{
    [AnimatableOf(typeof(UnityEngine.Rendering.Universal.ColorAdjustments))]
    [DisallowMultipleComponent]
    public class AnimatableColorAdjustments : AnimatableVolumeComponentBase
    {
        public override Type TargetType { get; } = typeof(UnityEngine.Rendering.Universal.ColorAdjustments);

        public bool override_postExposure;
        public System.Single postExposure;
        public bool override_contrast;
        public System.Single contrast;
        public bool override_colorFilter;
        public UnityEngine.Color colorFilter;
        public bool override_hueShift;
        public System.Single hueShift;
        public bool override_saturation;
        public System.Single saturation;


        private void WriteToVolumeComponent(UnityEngine.Rendering.Universal.ColorAdjustments volumeComponent)
        {
            if (!volumeComponent) return;

            volumeComponent.active = active;
            volumeComponent.postExposure.overrideState = override_postExposure;
            volumeComponent.postExposure.value = postExposure;
            volumeComponent.contrast.overrideState = override_contrast;
            volumeComponent.contrast.value = contrast;
            volumeComponent.colorFilter.overrideState = override_colorFilter;
            volumeComponent.colorFilter.value = colorFilter;
            volumeComponent.hueShift.overrideState = override_hueShift;
            volumeComponent.hueShift.value = hueShift;
            volumeComponent.saturation.overrideState = override_saturation;
            volumeComponent.saturation.value = saturation;

        }

        private void ReadFromVolumeComponent(UnityEngine.Rendering.Universal.ColorAdjustments volumeComponent)
        {
            if (!volumeComponent) return;

            active = volumeComponent.active;
            override_postExposure = volumeComponent.postExposure.overrideState;
            postExposure = volumeComponent.postExposure.value;
            override_contrast = volumeComponent.contrast.overrideState;
            contrast = volumeComponent.contrast.value;
            override_colorFilter = volumeComponent.colorFilter.overrideState;
            colorFilter = volumeComponent.colorFilter.value;
            override_hueShift = volumeComponent.hueShift.overrideState;
            hueShift = volumeComponent.hueShift.value;
            override_saturation = volumeComponent.saturation.overrideState;
            saturation = volumeComponent.saturation.value;

        }

        private void Reset()
        {
            var volume = GetComponent<Volume>();
            if (!volume || !volume.sharedProfile || !volume.sharedProfile.TryGet<UnityEngine.Rendering.Universal.ColorAdjustments>(out var volumeComponent)) return;

            ReadFromVolumeComponent(volumeComponent);
        }

        public override void WriteToVolumeComponent()
        {
            if (!volumeHelper.TryGet<UnityEngine.Rendering.Universal.ColorAdjustments>(out var volumeComponent)) return;

            WriteToVolumeComponent(volumeComponent);
        }

        public override void ReadFromVolumeComponent()
        {
            if (!volumeHelper.TryGet<UnityEngine.Rendering.Universal.ColorAdjustments>(out var volumeComponent)) return;

            ReadFromVolumeComponent(volumeComponent);
        }
    }
}

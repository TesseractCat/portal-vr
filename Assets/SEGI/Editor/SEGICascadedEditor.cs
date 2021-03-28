﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.Rendering.PostProcessing;
using UnityEditor.Rendering.PostProcessing;
using System;
using System.Linq.Expressions;

[Serializable]
[PostProcessEditor(typeof(SEGI_NKLI))]
public class SEGICascadedEditor : PostProcessEffectEditor<SEGI_NKLI>
{
    //Allow callbacks to main script
    //SEGICascaded _target;

    //SerializedParameterOverride serObj;

    SerializedParameterOverride VoxelResolution;
    SerializedParameterOverride TraceCacheResolution;
    SerializedParameterOverride visualizeSunDepthTexture;
    SerializedParameterOverride visualizeGI;
    SerializedParameterOverride visualizeGIPathCache;
    //SerializedParameterOverride Sun;
    SerializedParameterOverride giCullingMask;
    SerializedParameterOverride shadowVolumeMask;
    SerializedParameterOverride showVolumeObjects;
    SerializedParameterOverride shadowSpaceSize;
    SerializedParameterOverride temporalBlendWeight;
    SerializedParameterOverride visualizeVoxels;
    SerializedParameterOverride updateGI;
    SerializedParameterOverride MatchAmbientColor;
    SerializedParameterOverride skyColor;
    SerializedParameterOverride voxelSpaceSize;
    SerializedParameterOverride useBilateralFiltering;
    SerializedParameterOverride GIResolution;
    SerializedParameterOverride stochasticSampling;
    SerializedParameterOverride infiniteBounces;
    //SerializedParameterOverride followTransform;
    SerializedParameterOverride cones;
    SerializedParameterOverride coneTraceSteps;
    SerializedParameterOverride coneLength;
    SerializedParameterOverride coneWidth;
    SerializedParameterOverride occlusionStrength;
    SerializedParameterOverride nearOcclusionStrength;
    SerializedParameterOverride occlusionPower;
    SerializedParameterOverride coneTraceBias;
    SerializedParameterOverride nearLightGain;
    SerializedParameterOverride giGain;
    SerializedParameterOverride secondaryBounceGain;
    SerializedParameterOverride softSunlight;
    SerializedParameterOverride doReflections;

    SerializedParameterOverride voxelAA;
    SerializedParameterOverride updateVoxelsAfterX;
    SerializedParameterOverride updateVoxelsAfterXInterval;
    SerializedParameterOverride reflectionSteps;
    SerializedParameterOverride skyReflectionIntensity;
    SerializedParameterOverride gaussianMipFilter;
    SerializedParameterOverride reflectionOcclusionPower;
    SerializedParameterOverride farOcclusionStrength;
    SerializedParameterOverride farthestOcclusionStrength;
    SerializedParameterOverride secondaryCones;
    SerializedParameterOverride secondaryOcclusionStrength;
    SerializedParameterOverride skyIntensity;
    SerializedParameterOverride sphericalSkylight;
    SerializedParameterOverride innerOcclusionLayers;
    SerializedParameterOverride sunDepthTextureDepth;
    SerializedParameterOverride useReflectionProbes;
    SerializedParameterOverride reflectionProbeIntensity;
    //SerializedParameterOverride reflectionProbeAttribution;
    SerializedParameterOverride reflectionProbeLayerMask;
    SerializedParameterOverride useFXAA;

    SerializedParameterOverride NVIDIAVRWorksEnable;

    UnityEngine.Object SunProp;

    SEGI_NKLI instance;

    string presetPath = "Assets/SEGI/Resources/Cascaded Presets";

    GUIStyle headerStyle;
    GUIStyle vramLabelStyle
    {
        get
        {
            GUIStyle s = new GUIStyle(EditorStyles.boldLabel);
            s.fontStyle = FontStyle.Italic;
            return s;
        }
    }

    static bool showMainConfig = true;
    static bool showForwardConfig = true;
//    static bool showVolumeConfig = false;
    static bool showDebugTools = true;
    static bool showTracingProperties = true;
    static bool showEnvironmentProperties = true;
    static bool showPresets = true;
    static bool showReflectionProperties = true;
    static bool showPostEffect = true;
    static bool showVR = true;

    string presetToSaveName;

    int presetPopupIndex;

    public override void OnEnable()
    {
        VoxelResolution = FindParameterOverride(x => x.voxelResolution);
        TraceCacheResolution = FindParameterOverride(x => x.traceCacheResolution);
        visualizeGI = FindParameterOverride(x => x.visualizeGI);
        visualizeGIPathCache = FindParameterOverride(x => x.visualizeGIPathCache);
        visualizeSunDepthTexture = FindParameterOverride(x => x.visualizeSunDepthTexture);
        //Sun = FindParameterOverride(x <= x.Sun);
        giCullingMask = FindParameterOverride(x => x.giCullingMask);
        //shadowVolumeMask = FindParameterOverride(x => x.sha);
        //showVolumeObjects = serObj.FindProperty("showVolumeObjects");
        //shadowSpaceSize = FindParameterOverride(x => x.shado);
        temporalBlendWeight = FindParameterOverride(x => x.temporalBlendWeight);
        visualizeVoxels = FindParameterOverride(x => x.visualizeVoxels);
        updateGI = FindParameterOverride(x => x.updateGI);
        MatchAmbientColor = FindParameterOverride(x => x.MatchAmbiantColor);
        skyColor = FindParameterOverride(x => x.skyColor);
        voxelSpaceSize = FindParameterOverride(x => x.voxelSpaceSize);
        useBilateralFiltering = FindParameterOverride(x => x.useBilateralFiltering);
        GIResolution = FindParameterOverride(x => x.GIResolution);
        stochasticSampling = FindParameterOverride(x => x.stochasticSampling);
        infiniteBounces = FindParameterOverride(x => x.infiniteBounces);
        //followTransform = FindParameterOverride(x => x.followTransform);
        cones = FindParameterOverride(x => x.cones);
        coneTraceSteps = FindParameterOverride(x => x.coneTraceSteps);
        coneLength = FindParameterOverride(x => x.coneLength);
        coneWidth = FindParameterOverride(x => x.coneWidth);
        occlusionStrength = FindParameterOverride(x => x.occlusionStrength);
        nearOcclusionStrength = FindParameterOverride(x => x.nearOcclusionStrength);
        occlusionPower = FindParameterOverride(x => x.occlusionPower);
        coneTraceBias = FindParameterOverride(x => x.coneTraceBias);
        nearLightGain = FindParameterOverride(x => x.nearLightGain);
        giGain = FindParameterOverride(x => x.giGain);
        secondaryBounceGain = FindParameterOverride(x => x.secondaryBounceGain);
        softSunlight = FindParameterOverride(x => x.softSunlight);
        doReflections = FindParameterOverride(x => x.doReflections);
        voxelAA = FindParameterOverride(x => x.voxelAA);
        reflectionSteps = FindParameterOverride(x => x.reflectionSteps);
        skyReflectionIntensity = FindParameterOverride(x => x.skyReflectionIntensity);
        gaussianMipFilter = FindParameterOverride(x => x.gaussianMipFilter);
        reflectionOcclusionPower = FindParameterOverride(x => x.reflectionOcclusionPower);
        farOcclusionStrength = FindParameterOverride(x => x.farOcclusionStrength);
        farthestOcclusionStrength = FindParameterOverride(x => x.farthestOcclusionStrength);
        secondaryCones = FindParameterOverride(x => x.secondaryCones);
        secondaryOcclusionStrength = FindParameterOverride(x => x.secondaryOcclusionStrength);
        skyIntensity = FindParameterOverride(x => x.skyIntensity);
        sphericalSkylight = FindParameterOverride(x => x.sphericalSkylight);
        innerOcclusionLayers = FindParameterOverride(x => x.innerOcclusionLayers);
        //sunDepthTextureDepth = FindParameterOverride(x => x.sun);
        useReflectionProbes = FindParameterOverride(x => x.useReflectionProbes);
        reflectionProbeIntensity = FindParameterOverride(x => x.reflectionProbeIntensity);
        //reflectionProbeAttribution = FindParameterOverride(x => x.reflectionProbeAttribution);
        reflectionProbeLayerMask = FindParameterOverride(x => x.reflectionProbeLayerMask);
        useFXAA = FindParameterOverride(x => x.useFXAA);
        updateVoxelsAfterX = FindParameterOverride(x => x.updateVoxelsAfterX);
        updateVoxelsAfterXInterval = FindParameterOverride(x => x.updateVoxelsAfterXInterval);
        NVIDIAVRWorksEnable = FindParameterOverride(x => x.NVIDIAVRWorksEnable);


        //instance = target as SEGICascaded;
    }

    public override void OnInspectorGUI()
    {
        //serObj.Update();

        EditorGUILayout.HelpBox("This is a preview of the work-in-progress version of SEGI with cascaded GI volumes. Behavior is not final and is subject to change.", MessageType.Info);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        /*if (GUILayout.Button("Click to apply Changes made at runtime!"))
        {
            if (!_target.enabled) _target.enabled = true;
            else
            {
                //_target.SEGIBufferInit();
            }
        }*/
        EditorGUILayout.Space();
        EditorGUILayout.EndHorizontal();
        /*
        //Presets
        showPresets = EditorGUILayout.Foldout(showPresets, new GUIContent("Presets"));
        if (showPresets)
        {
            string path = "Assets/SEGI";
            //#if UNITY_EDITOR
            //MonoScript ms = MonoScript.FromScriptableObject(new SEGICascadedPreset());
            //path = System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(ms));
            //#endif
            presetPath = path + "/Resources/Cascaded Presets";
            EditorGUI.indentLevel++;
            string[] presetGUIDs = AssetDatabase.FindAssets("t:SEGICascadedPreset", new string[1] { presetPath });
            string[] presetNames = new string[presetGUIDs.Length];
            string[] presetPaths = new string[presetGUIDs.Length];

            for (int i = 0; i < presetGUIDs.Length; i++)
            {
                presetPaths[i] = AssetDatabase.GUIDToAssetPath(presetGUIDs[i]);
                presetNames[i] = System.IO.Path.GetFileNameWithoutExtension(presetPaths[i]);
            }

            EditorGUILayout.BeginHorizontal();
            presetPopupIndex = EditorGUILayout.Popup("", presetPopupIndex, presetNames);

            if (GUILayout.Button("Load"))
            {
                if (presetPaths.Length > 0)
                {
                    SEGICascadedPreset preset = AssetDatabase.LoadAssetAtPath<SEGICascadedPreset>(presetPaths[presetPopupIndex]);
                    //instance.ApplyPreset(preset);
                    //EditorUtility.SetDirty(target);
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            presetToSaveName = EditorGUILayout.TextField(presetToSaveName);

            if (GUILayout.Button("Save"))
            {
                //SavePreset(presetToSaveName);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        */
        //Main Configuration
        showMainConfig = EditorGUILayout.Foldout(showMainConfig, new GUIContent("Main Configuration"));
        if (showMainConfig)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginVertical();
            PropertyField(VoxelResolution, new GUIContent("Voxel Resolution", "The resolution of the voxel texture used to calculate GI."));
            PropertyField(TraceCacheResolution, new GUIContent("Trace Cache Resolution", "The resolution of the path tracer cache volume."));
            PropertyField(voxelAA, new GUIContent("Voxel AA", "Enables anti-aliasing during voxelization for higher precision voxels."));
            PropertyField(innerOcclusionLayers, new GUIContent("Inner Occlusion Layers", "Enables the writing of additional black occlusion voxel layers on the back face of geometry. Can help with light leaking but may cause artifacts with small objects."));
            PropertyField(gaussianMipFilter, new GUIContent("Gaussian Mip Filter", "Enables gaussian filtering during mipmap generation. This can improve visual smoothness and consistency, particularly with large moving objects."));
            PropertyField(voxelSpaceSize, new GUIContent("Voxel Space Size", "The size of the voxel volume in world units. Everything inside the voxel volume will contribute to GI."));
            //PropertyField(shadowSpaceSize, new GUIContent("Shadow Space Size", "The size of the sun shadow texture used to inject sunlight with shadows into the voxels in world units. It is recommended to set this value similar to Voxel Space Size."));
            PropertyField(giCullingMask, new GUIContent("GI Culling Mask", "Which layers should be voxelized and contribute to GI."));
            PropertyField(updateGI, new GUIContent("Update GI", "Whether voxelization and multi-bounce rendering should update every frame. When disabled, GI tracing will use cached data from the last time this was enabled."));
            PropertyField(updateVoxelsAfterX, new GUIContent("Update GI After X Moved", "Weather Voxel should be updated after a specified distance is moved rather than every frame"));
            PropertyField(updateVoxelsAfterXInterval, new GUIContent("Update GI after distance", "Update the voxels after moving x distance"));
            PropertyField(infiniteBounces, new GUIContent("Infinite Bounces", "Enables infinite bounces. This is expensive for complex scenes and is still experimental."));
            //PropertyField(followTransform, new GUIContent("Follow Transform", "If provided, the voxel volume will follow and be centered on this object instead of the camera. Useful for top-down scenes."));
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
            //LabelField("VRAM Usage: " + instance.vramUsage.ToString("F2") + " MB", vramLabelStyle);
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        //Forward
        showForwardConfig = EditorGUILayout.Foldout(showForwardConfig, new GUIContent("Forward Rendering Configuration"));
        if (showForwardConfig)
        {
            EditorGUI.indentLevel++;
            //PropertyField(useReflectionProbes, new GUIContent("Update Reflection Probe", "Approximates path traced Specular values using a Reflection Probe."));
            //PropertyField(reflectionProbeIntensity, new GUIContent("Reflection Probe Intensity", "Intensity of Reflection Probe influence."));
            //PropertyField(reflectionProbeAttribution, new GUIContent("Reflection Probe Attribution", "How much Reflection Probes contribute to GI"));
            PropertyField(reflectionProbeLayerMask, new GUIContent("fakeBuffer Layer Mask", "Used in forward mode to emulate deferred gbuffers."));
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();


        //Environment
        showEnvironmentProperties = EditorGUILayout.Foldout(showEnvironmentProperties, new GUIContent("Environment Properties"));
        /*if (Sun == null)
        {
            showEnvironmentProperties = true;
        }*/
        if (showEnvironmentProperties)
        {
            EditorGUI.indentLevel++;
            //PropertyField(Sun, new GUIContent("Sun", "The main directional light that will cast indirect light into the scene (sunlight or moonlight)."));
            PropertyField(softSunlight, new GUIContent("Soft Sunlight", "The amount of soft diffuse sunlight that will be added to the scene. Use this to simulate the effect of clouds/haze scattering soft sunlight onto the scene."));
            PropertyField(MatchAmbientColor, new GUIContent("Match Scene Lighting", "Sync Sky Color and intensity to scene lighting"));
            PropertyField(skyColor, new GUIContent("Sky Color", "The color of the light scattered onto the scene coming from the sky."));
            PropertyField(skyIntensity, new GUIContent("Sky Intensity", "The brightness of the sky light."));
            PropertyField(sphericalSkylight, new GUIContent("Spherical Skylight", "If enabled, light from the sky will come from all directions. If disabled, light from the sky will only come from the top hemisphere."));
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();


        //Tracing properties
        showTracingProperties = EditorGUILayout.Foldout(showTracingProperties, new GUIContent("Tracing Properties"));
        if (showTracingProperties)
        {
            EditorGUI.indentLevel++;
            PropertyField(temporalBlendWeight, new GUIContent("Temporal Blend Weight", "The lower the value, the more previous frames will be blended with the current frame. Lower values result in smoother GI that updates less quickly."));
            //PropertyField(useBilateralFiltering, new GUIContent("Bilateral Filtering", "Enables filtering of the GI result to reduce noise."));
            PropertyField(stochasticSampling, new GUIContent("Stochastic Sampling", "If enabled, uses random jitter to reduce banding and discontinuities during GI tracing."));
            PropertyField(GIResolution, new GUIContent("Subsampling Resolution", "GI tracing resolution will be subsampled at this screen resolution. Improves speed of GI tracing."));

            //PropertyField(cones, new GUIContent("Cones", "The number of cones that will be traced in different directions for diffuse GI tracing. More cones result in a smoother result at the cost of performance."));
            PropertyField(coneTraceSteps, new GUIContent("Cone Trace Steps", "The number of tracing steps for each cone. Too few results in skipping thin features. Higher values result in more accuracy at the cost of performance."));
            PropertyField(coneLength, new GUIContent("Cone length", "The number of cones that will be traced in different directions for diffuse GI tracing. More cones result in a smoother result at the cost of performance."));
            PropertyField(coneWidth, new GUIContent("Cone Width", "The width of each cone. Wider cones cause a softer and smoother result but affect accuracy and incrase over-occlusion. Thinner cones result in more accurate tracing with less coherent (more noisy) results and a higher tracing cost."));
            PropertyField(coneTraceBias, new GUIContent("Cone Trace Bias", "The amount of offset above a surface that cone tracing begins. Higher values reduce \"voxel acne\" (similar to \"shadow acne\"). Values that are too high result in light-leaking."));
            PropertyField(occlusionStrength, new GUIContent("Occlusion Strength", "The strength of shadowing solid objects will cause. Affects the strength of all indirect shadows."));
            PropertyField(nearOcclusionStrength, new GUIContent("Near Occlusion Strength", "The strength of shadowing nearby solid objects will cause. Only affects the strength of very close blockers."));
            PropertyField(farOcclusionStrength, new GUIContent("Far Occlusion Strength", "How much light far occluders block. This value gives additional light blocking proportional to the width of the cone at each trace step."));
            PropertyField(farthestOcclusionStrength, new GUIContent("Farthest Occlusion Strength", "How much light the farthest occluders block. This value gives additional light blocking proportional to (cone width)^2 at each trace step."));
            PropertyField(occlusionPower, new GUIContent("Occlusion Power", "The strength of shadowing far solid objects will cause. Only affects the strength of far blockers. Decrease this value if wide cones are causing over-occlusion."));
            PropertyField(nearLightGain, new GUIContent("Near Light Gain", "Affects the attenuation of indirect light. Higher values allow for more close-proximity indirect light. Lower values reduce close-proximity indirect light, sometimes resulting in a cleaner result."));
            PropertyField(giGain, new GUIContent("GI Gain", "The overall brightness of indirect light. For Near Light Gain values around 1, a value of 1 for this property is recommended for a physically-accurate result."));
            EditorGUILayout.Space();
            PropertyField(secondaryBounceGain, new GUIContent("Secondary Bounce Gain", "Affects the strength of secondary/infinite bounces. Be careful, values above 1 can cause runaway light bouncing and flood areas with extremely bright light!"));
            PropertyField(secondaryCones, new GUIContent("Secondary Cones", "The number of secondary cones that will be traced for calculating infinte bounces. Increasing this value improves the accuracy of secondary bounces at the cost of performance. Note: the performance cost of this scales with voxelized scene complexity."));
            PropertyField(secondaryOcclusionStrength, new GUIContent("Secondary Occlusion Strength", "The strength of light blocking during secondary bounce tracing. Be careful, a value too low can cause runaway light bouncing and flood areas with extremely bright light!"));
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();


        showReflectionProperties = EditorGUILayout.Foldout(showReflectionProperties, new GUIContent("Reflection Properties"));
        if (showReflectionProperties)
        {
            EditorGUI.indentLevel++;
            PropertyField(doReflections, new GUIContent("Do Reflections", "Enable this for cone-traced reflections."));
            PropertyField(reflectionSteps, new GUIContent("Reflection Steps", "Number of reflection trace steps."));
            PropertyField(reflectionOcclusionPower, new GUIContent("Reflection Occlusion Power", "Strength of light blocking during reflection tracing."));
            PropertyField(skyReflectionIntensity, new GUIContent("Sky Reflection Intensity", "Intensity of sky reflections."));
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        showPostEffect = EditorGUILayout.Foldout(showPostEffect, new GUIContent("Post Processing"));
        if (showReflectionProperties)
        {
            EditorGUI.indentLevel++;
            PropertyField(useFXAA, new GUIContent("FXAA", "Apply FXAA Anti-aliasing to the final image."));
            EditorGUI.indentLevel--;
        }


        EditorGUILayout.Space();
        EditorGUILayout.Space();

        showVR = EditorGUILayout.Foldout(showVR, new GUIContent("Stereo Rendering"));
        if (showReflectionProperties)
        {
            EditorGUI.indentLevel++;
            #if VRWORKS
                PropertyField(NVIDIAVRWorksEnable, new GUIContent("NVIDIA VRWorks", "Enables VRWorks if present and running on a Pascall class GPU."));
            #endif
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        //Debug tools
        showDebugTools = EditorGUILayout.Foldout(showDebugTools, new GUIContent("Debug Tools"));
        if (showDebugTools)
        {
            EditorGUI.indentLevel++;
            PropertyField(visualizeSunDepthTexture, new GUIContent("Visualize Sun Depth Texture", "Visualize the depth texture used to render proper shadows while injecting sunlight into voxel data."));
            PropertyField(visualizeGI, new GUIContent("Visualize GI", "Visualize GI result only (no textures)."));
            PropertyField(visualizeVoxels, new GUIContent("Visualize Voxels", "Directly view the voxels in the scene."));
            EditorGUILayout.Space();
            PropertyField(visualizeGIPathCache, new GUIContent("Trace ONLY Path Cache", "Visualize GI path tracer cache result only. (You must also select 'Visualize GI')"));
            EditorGUI.indentLevel--;
        }


        //serObj.ApplyModifiedProperties();
    }
}


using ICities;
using System;

namespace UltimateEyecandy
{
    public class LoadingExtension : LoadingExtensionBase
    {
        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);
            try
            {
                // Create backup:
                UltimateEyecandyTool.SaveBackup();
            }
            catch (Exception e)
            {
                DebugUtils.LogException(e);
            }
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            // Check if in-game or in Asset Editor:
            if (mode != LoadMode.LoadGame && mode != LoadMode.NewGame && mode != LoadMode.LoadAsset && mode != LoadMode.NewAsset)
            {
                return;
            }
            //  
            UltimateEyecandyTool.Initialize(mode);
            UltimateEyecandyTool.SaveInitialValues();
            UltimateEyecandyTool.LoadConfig();
        }

        public override void OnLevelUnloading()
        {
            UltimateEyecandyTool.isGameLoaded = false;
            UltimateEyecandyTool.Reset();
            //  
            base.OnLevelUnloading();
        }
    }
}

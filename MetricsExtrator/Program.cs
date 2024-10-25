using Microsoft.Build.Evaluation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MetricsExtrator
{

    class Program
    {
        private static CodeMetricsAnalyzer codeMetricsAnalyzer;
        private static CsvExporter exporter;

        static async Task Main(string[] args)
        {
            string codeSmell = "Long Method";
            string[] projectPaths;

            switch (codeSmell)
            {
                case "Long Method":
                    projectPaths = LargeClass_LongMethodProjects();
                    break;
                case "Large Class":
                    projectPaths = LargeClass_LongMethodProjects();
                    break;
                case "Data Class":
                    projectPaths = DataClassProjects();
                    break;
                case "Feature Envy":
                    projectPaths = FeatureEnvyProjects();
                    break;
                default :
                    projectPaths = LargeClass_LongMethodProjects();
                    break;
            }


            foreach (var projectPath in projectPaths)
            {

                try
                {
                    Console.WriteLine($"Analysing {projectPath}");
                    string generatedDatasetPath = @"C:\Users\XXXXXXX\source\repos\MetricsAnalyzer\GeneratedDatasets\";

                    var progress = new Progress<double>(percent => {
                        Console.WriteLine($"Progresso: {percent:P2}");
                    });

                    codeMetricsAnalyzer = new CodeMetricsAnalyzer();    
                    
                    var labeledCodeSmellMetrics = await codeMetricsAnalyzer.CalculateMetrics(projectPath, codeSmell, progress);

                    exporter = new CsvExporter();
                    exporter.ExportToCsv(labeledCodeSmellMetrics, $"{generatedDatasetPath}\\{codeSmell}\\GeneratedDataset_{codeSmell}");

                    Console.WriteLine($"Analysis completed. Results exported to metrics.csv");
                }
                catch { Console.WriteLine($"ERRO no {projectPath}"); }

            }
        }

        private static string[] LargeClass_LongMethodProjects()
        {
            return new string[] {

                @"C:\ProjetosCSharp\LargeClass_LongMethodProjects\ShopifySharp-a95f4e3b20dd5d14a1225a48aa2b7e8b3cb15547\ShopifySharp\ShopifySharp.csproj",

                @"C:\ProjetosCSharp\LargeClass_LongMethodProjects\BurningKnight-a55594c11ab681087356af2c129c2d493eba4bd2\Aseprite\Aseprite.csproj",
                @"C:\ProjetosCSharp\LargeClass_LongMethodProjects\BurningKnight-a55594c11ab681087356af2c129c2d493eba4bd2\BurningKnight\BurningKnight.csproj",
                @"C:\ProjetosCSharp\LargeClass_LongMethodProjects\BurningKnight-a55594c11ab681087356af2c129c2d493eba4bd2\Desktop\Desktop.csproj",
                @"C:\ProjetosCSharp\LargeClass_LongMethodProjects\BurningKnight-a55594c11ab681087356af2c129c2d493eba4bd2\Lens\Lens.csproj",
                @"C:\ProjetosCSharp\LargeClass_LongMethodProjects\BurningKnight-a55594c11ab681087356af2c129c2d493eba4bd2\VelcroPhysics\VelcroPhysics.csproj",

                @"C:\ProjetosCSharp\LargeClass_LongMethodProjects\Core2D-ca290ba82ac571439640af4cb248b2c1e42091d0\src\Core2D\Core2D.csproj",

                @"C:\ProjetosCSharp\LargeClass_LongMethodProjects\jellyfin-6c2eb5fc7e872a29b4a0951849681ae0764dbb8e\DvdLib\DvdLib.csproj",
                @"C:\ProjetosCSharp\LargeClass_LongMethodProjects\jellyfin-6c2eb5fc7e872a29b4a0951849681ae0764dbb8e\Emby.Dlna\Emby.Dlna.csproj",
                @"C:\ProjetosCSharp\LargeClass_LongMethodProjects\jellyfin-6c2eb5fc7e872a29b4a0951849681ae0764dbb8e\Emby.Drawing\Emby.Drawing.csproj",
                @"C:\ProjetosCSharp\LargeClass_LongMethodProjects\jellyfin-6c2eb5fc7e872a29b4a0951849681ae0764dbb8e\Emby.Naming\Emby.Naming.csproj",
                @"C:\ProjetosCSharp\LargeClass_LongMethodProjects\jellyfin-6c2eb5fc7e872a29b4a0951849681ae0764dbb8e\Emby.Notifications\Emby.Notifications.csproj",
                @"C:\ProjetosCSharp\LargeClass_LongMethodProjects\jellyfin-6c2eb5fc7e872a29b4a0951849681ae0764dbb8e\Emby.Server.Implementations\Emby.Server.Implementations.csproj",
                @"C:\ProjetosCSharp\LargeClass_LongMethodProjects\jellyfin-6c2eb5fc7e872a29b4a0951849681ae0764dbb8e\Jellyfin.Api\Jellyfin.Api.csproj",
                @"C:\ProjetosCSharp\LargeClass_LongMethodProjects\jellyfin-6c2eb5fc7e872a29b4a0951849681ae0764dbb8e\Jellyfin.Data\Jellyfin.Data.csproj",
                @"C:\ProjetosCSharp\LargeClass_LongMethodProjects\jellyfin-6c2eb5fc7e872a29b4a0951849681ae0764dbb8e\Jellyfin.Drawing.Skia\Jellyfin.Drawing.Skia.csproj",
                @"C:\ProjetosCSharp\LargeClass_LongMethodProjects\jellyfin-6c2eb5fc7e872a29b4a0951849681ae0764dbb8e\Jellyfin.Networking\Jellyfin.Networking.csproj",
                @"C:\ProjetosCSharp\LargeClass_LongMethodProjects\jellyfin-6c2eb5fc7e872a29b4a0951849681ae0764dbb8e\Jellyfin.Server\Jellyfin.Server.csproj",
                @"C:\ProjetosCSharp\LargeClass_LongMethodProjects\jellyfin-6c2eb5fc7e872a29b4a0951849681ae0764dbb8e\Jellyfin.Server.Implementations\Jellyfin.Server.Implementations.csproj",
                @"C:\ProjetosCSharp\LargeClass_LongMethodProjects\jellyfin-6c2eb5fc7e872a29b4a0951849681ae0764dbb8e\MediaBrowser.Common\MediaBrowser.Common.csproj",
                @"C:\ProjetosCSharp\LargeClass_LongMethodProjects\jellyfin-6c2eb5fc7e872a29b4a0951849681ae0764dbb8e\MediaBrowser.Controller\MediaBrowser.Controller.csproj",
                @"C:\ProjetosCSharp\LargeClass_LongMethodProjects\jellyfin-6c2eb5fc7e872a29b4a0951849681ae0764dbb8e\MediaBrowser.LocalMetadata\MediaBrowser.LocalMetadata.csproj",
                @"C:\ProjetosCSharp\LargeClass_LongMethodProjects\jellyfin-6c2eb5fc7e872a29b4a0951849681ae0764dbb8e\MediaBrowser.MediaEncoding\MediaBrowser.MediaEncoding.csproj",
                @"C:\ProjetosCSharp\LargeClass_LongMethodProjects\jellyfin-6c2eb5fc7e872a29b4a0951849681ae0764dbb8e\MediaBrowser.Model\MediaBrowser.Model.csproj",
                @"C:\ProjetosCSharp\LargeClass_LongMethodProjects\jellyfin-6c2eb5fc7e872a29b4a0951849681ae0764dbb8e\MediaBrowser.Providers\MediaBrowser.Providers.csproj",
                @"C:\ProjetosCSharp\LargeClass_LongMethodProjects\jellyfin-6c2eb5fc7e872a29b4a0951849681ae0764dbb8e\MediaBrowser.XbmcMetadata\MediaBrowser.XbmcMetadata.csproj",
                @"C:\ProjetosCSharp\LargeClass_LongMethodProjects\jellyfin-6c2eb5fc7e872a29b4a0951849681ae0764dbb8e\RSSDP\RSSDP.csproj",

                @"C:\ProjetosCSharp\LargeClass_LongMethodProjects\MonoGame-4802d00db04dc7aa5fe07cd2d908f9a4b090a4fd\MonoGame.Framework.Content.Pipeline\MonoGame.Framework.Content.Pipeline.csproj",
                @"C:\ProjetosCSharp\LargeClass_LongMethodProjects\MonoGame-4802d00db04dc7aa5fe07cd2d908f9a4b090a4fd\Tools\MonoGame.Content.Builder.Editor\MonoGame.Content.Builder.Editor.csproj",
                @"C:\ProjetosCSharp\LargeClass_LongMethodProjects\MonoGame-4802d00db04dc7aa5fe07cd2d908f9a4b090a4fd\Tools\MonoGame.Content.Builder\MonoGame.Content.Builder.csproj",
                @"C:\ProjetosCSharp\LargeClass_LongMethodProjects\MonoGame-4802d00db04dc7aa5fe07cd2d908f9a4b090a4fd\Tools\MonoGame.Effect.Compiler\MonoGame.Effect.Compiler.csproj",
                @"C:\ProjetosCSharp\LargeClass_LongMethodProjects\MonoGame-4802d00db04dc7aa5fe07cd2d908f9a4b090a4fd\Tools\MonoGame.Packaging.Flatpak\MonoGame.Packaging.Flatpak.csproj",
                @"C:\ProjetosCSharp\LargeClass_LongMethodProjects\MonoGame-4802d00db04dc7aa5fe07cd2d908f9a4b090a4fd\MonoGame.Framework\MonoGame.Framework.iOS.csproj",
                @"C:\ProjetosCSharp\LargeClass_LongMethodProjects\MonoGame-4802d00db04dc7aa5fe07cd2d908f9a4b090a4fd\Tests\Interactive\Linux\Lidgren.Network\Lidgren.Network.csproj",

                @"C:\ProjetosCSharp\LargeClass_LongMethodProjects\OpenRA-920d00bbae9fa8e62387bbff705ca4bea6a26677\OpenRA.Game\OpenRA.Game.csproj",
                @"C:\ProjetosCSharp\LargeClass_LongMethodProjects\OpenRA-920d00bbae9fa8e62387bbff705ca4bea6a26677\OpenRA.Mods.Cnc\OpenRA.Mods.Cnc.csproj",
                @"C:\ProjetosCSharp\LargeClass_LongMethodProjects\OpenRA-920d00bbae9fa8e62387bbff705ca4bea6a26677\OpenRA.Mods.Common\OpenRA.Mods.Common.csproj",
                @"C:\ProjetosCSharp\LargeClass_LongMethodProjects\OpenRA-920d00bbae9fa8e62387bbff705ca4bea6a26677\OpenRA.Mods.D2k\OpenRA.Mods.D2k.csproj",
                @"C:\ProjetosCSharp\LargeClass_LongMethodProjects\OpenRA-920d00bbae9fa8e62387bbff705ca4bea6a26677\OpenRA.Platforms.Default\OpenRA.Platforms.Default.csproj",
                @"C:\ProjetosCSharp\LargeClass_LongMethodProjects\OpenRA-920d00bbae9fa8e62387bbff705ca4bea6a26677\OpenRA.WindowsLauncher\OpenRA.WindowsLauncher.csproj",

                @"C:\ProjetosCSharp\LargeClass_LongMethodProjects\osu-2cac373365309a40474943f55c56159ed8f9433c\osu.Desktop\osu.Desktop.csproj",
                @"C:\ProjetosCSharp\LargeClass_LongMethodProjects\osu-2cac373365309a40474943f55c56159ed8f9433c\osu.Game\osu.Game.csproj",


                @"C:\ProjetosCSharp\LargeClass_LongMethodProjects\ShareX-c9a71ed00eda0e7c5a45237b9bcd3f8f614cda63\ShareX.HelpersLib\ShareX.HelpersLib.csproj",
                @"C:\ProjetosCSharp\LargeClass_LongMethodProjects\ShareX-c9a71ed00eda0e7c5a45237b9bcd3f8f614cda63\ShareX.ImageEffectsLib\ShareX.ImageEffectsLib.csproj",
                @"C:\ProjetosCSharp\LargeClass_LongMethodProjects\ShareX-c9a71ed00eda0e7c5a45237b9bcd3f8f614cda63\ShareX.IndexerLib\ShareX.IndexerLib.csproj",
                @"C:\ProjetosCSharp\LargeClass_LongMethodProjects\ShareX-c9a71ed00eda0e7c5a45237b9bcd3f8f614cda63\ShareX.MediaLib\ShareX.MediaLib.csproj",
                @"C:\ProjetosCSharp\LargeClass_LongMethodProjects\ShareX-c9a71ed00eda0e7c5a45237b9bcd3f8f614cda63\ShareX.ScreenCaptureLib\ShareX.ScreenCaptureLib.csproj",
                @"C:\ProjetosCSharp\LargeClass_LongMethodProjects\ShareX-c9a71ed00eda0e7c5a45237b9bcd3f8f614cda63\ShareX.UploadersLib\ShareX.UploadersLib.csproj",
                @"C:\ProjetosCSharp\LargeClass_LongMethodProjects\ShareX-c9a71ed00eda0e7c5a45237b9bcd3f8f614cda63\ShareX\ShareX.csproj",
                @"C:\ProjetosCSharp\LargeClass_LongMethodProjects\ShareX-c9a71ed00eda0e7c5a45237b9bcd3f8f614cda63\ShareX.HistoryLib\ShareX.HistoryLib.csproj",
                @"C:\ProjetosCSharp\LargeClass_LongMethodProjects\ShareX-c9a71ed00eda0e7c5a45237b9bcd3f8f614cda63\ShareX.NativeMessagingHost\ShareX.NativeMessagingHost.csproj"
            };
        }
        private static string[] DataClassProjects()
        {
            return new string[] {

                @"C:\ProjetosCSharp\DataClassProjects\duplicati-0a1b32e1887c98c6034c9fafdfddcb8f8f31e207\Duplicati\Library\Logging\Duplicati.Library.Logging.csproj",
                
                @"C:\ProjetosCSharp\DataClassProjects\BurningKnight-a55594c11ab681087356af2c129c2d493eba4bd2\Aseprite\Aseprite.csproj",
                @"C:\ProjetosCSharp\DataClassProjects\BurningKnight-a55594c11ab681087356af2c129c2d493eba4bd2\BurningKnight\BurningKnight.csproj",
                @"C:\ProjetosCSharp\DataClassProjects\BurningKnight-a55594c11ab681087356af2c129c2d493eba4bd2\Lens\Lens.csproj",
                @"C:\ProjetosCSharp\DataClassProjects\BurningKnight-a55594c11ab681087356af2c129c2d493eba4bd2\VelcroPhysics\VelcroPhysics.csproj",
                
                @"C:\ProjetosCSharp\DataClassProjects\Files-89c33841813a5590e6bf44fb02bb7d06348320c3\Files\Files.csproj",

                @"C:\ProjetosCSharp\DataClassProjects\Jackett-db695e5dc01755ff52b5cd7b4f0004ff1035649d\src\Jackett.Common\Jackett.Common.csproj",

                @"C:\ProjetosCSharp\DataClassProjects\jellyfin-6c2eb5fc7e872a29b4a0951849681ae0764dbb8e\DvdLib\DvdLib.csproj",
                @"C:\ProjetosCSharp\DataClassProjects\jellyfin-6c2eb5fc7e872a29b4a0951849681ae0764dbb8e\Jellyfin.Api\Jellyfin.Api.csproj",
                @"C:\ProjetosCSharp\DataClassProjects\jellyfin-6c2eb5fc7e872a29b4a0951849681ae0764dbb8e\MediaBrowser.Controller\MediaBrowser.Controller.csproj",

                @"C:\ProjetosCSharp\DataClassProjects\LiteDB-00d28bfafe3c685ae239f759f812def495278eaf\LiteDB\LiteDB.csproj",

                @"C:\ProjetosCSharp\DataClassProjects\mRemoteNG-e6d2c9791d7a5e55630c987a3c81fb287032752b\mRemoteNG\mRemoteNG.csproj",

                @"C:\ProjetosCSharp\DataClassProjects\ScreenToGif-2d318f837946f730e1b2e5c708ae9f776b9e360b\ScreenToGif\ScreenToGif.csproj",

                @"C:\ProjetosCSharp\DataClassProjects\OpenRA-920d00bbae9fa8e62387bbff705ca4bea6a26677\OpenRA.Game\OpenRA.Game.csproj",
                @"C:\ProjetosCSharp\DataClassProjects\OpenRA-920d00bbae9fa8e62387bbff705ca4bea6a26677\OpenRA.Mods.Cnc\OpenRA.Mods.Cnc.csproj",
                @"C:\ProjetosCSharp\DataClassProjects\OpenRA-920d00bbae9fa8e62387bbff705ca4bea6a26677\OpenRA.Mods.Common\OpenRA.Mods.Common.csproj",
                @"C:\ProjetosCSharp\DataClassProjects\OpenRA-920d00bbae9fa8e62387bbff705ca4bea6a26677\OpenRA.Mods.D2k\OpenRA.Mods.D2k.csproj",

                @"C:\ProjetosCSharp\DataClassProjects\osu-2cac373365309a40474943f55c56159ed8f9433c\osu.Game.Rulesets.Catch\osu.Game.Rulesets.Catch.csproj",
                @"C:\ProjetosCSharp\DataClassProjects\osu-2cac373365309a40474943f55c56159ed8f9433c\osu.Game.Rulesets.Osu\osu.Game.Rulesets.Osu.csproj",
                @"C:\ProjetosCSharp\DataClassProjects\osu-2cac373365309a40474943f55c56159ed8f9433c\osu.Game.Rulesets.Taiko\osu.Game.Rulesets.Taiko.csproj",
                @"C:\ProjetosCSharp\DataClassProjects\osu-2cac373365309a40474943f55c56159ed8f9433c\osu.Game.Tournament\osu.Game.Tournament.csproj",
                @"C:\ProjetosCSharp\DataClassProjects\osu-2cac373365309a40474943f55c56159ed8f9433c\osu.Game\osu.Game.csproj",

                @"C:\ProjetosCSharp\DataClassProjects\Radarr-5ce1829709e7e1de3953c04e5dab4f3a9d450b38\src\NzbDrone.Core\Radarr.Core.csproj",
                @"C:\ProjetosCSharp\DataClassProjects\Radarr-5ce1829709e7e1de3953c04e5dab4f3a9d450b38\src\Radarr.Http\Radarr.Http.csproj",

                @"C:\ProjetosCSharp\DataClassProjects\ShareX-c9a71ed00eda0e7c5a45237b9bcd3f8f614cda63\ShareX.IndexerLib\ShareX.IndexerLib.csproj",
                @"C:\ProjetosCSharp\DataClassProjects\ShareX-c9a71ed00eda0e7c5a45237b9bcd3f8f614cda63\ShareX.MediaLib\ShareX.MediaLib.csproj",
                @"C:\ProjetosCSharp\DataClassProjects\ShareX-c9a71ed00eda0e7c5a45237b9bcd3f8f614cda63\ShareX.ScreenCaptureLib\ShareX.ScreenCaptureLib.csproj",
                @"C:\ProjetosCSharp\DataClassProjects\ShareX-c9a71ed00eda0e7c5a45237b9bcd3f8f614cda63\ShareX.UploadersLib\ShareX.UploadersLib.csproj",
                @"C:\ProjetosCSharp\DataClassProjects\ShareX-c9a71ed00eda0e7c5a45237b9bcd3f8f614cda63\ShareX\ShareX.csproj",

                @"C:\ProjetosCSharp\DataClassProjects\Sonarr-6378e7afef6072eae20f6408818c6fb1c85661b7\src\NzbDrone.Core\Sonarr.Core.csproj",
            };
        }
        private static string[] RefusedBequestProjects()
        {
            return new string[] { 

            
            };
        }
        private static string[] FeatureEnvyProjects()
        {
            return new string[] {
                @"C:\ProjetosCSharp\FeatureEnvyProjects\duplicati-0a1b32e1887c98c6034c9fafdfddcb8f8f31e207\BuildTools\UpdateVersionStamp\UpdateVersionStamp.csproj",
                @"C:\ProjetosCSharp\FeatureEnvyProjects\duplicati-0a1b32e1887c98c6034c9fafdfddcb8f8f31e207\Duplicati\CommandLine\Duplicati.CommandLine.csproj",
                @"C:\ProjetosCSharp\FeatureEnvyProjects\duplicati-0a1b32e1887c98c6034c9fafdfddcb8f8f31e207\Duplicati\Library\AutoUpdater\Duplicati.Library.AutoUpdater.csproj",
                @"C:\ProjetosCSharp\FeatureEnvyProjects\duplicati-0a1b32e1887c98c6034c9fafdfddcb8f8f31e207\Duplicati\Library\Backend\GoogleServices\Duplicati.Library.Backend.GoogleServices.csproj",
                @"C:\ProjetosCSharp\FeatureEnvyProjects\duplicati-0a1b32e1887c98c6034c9fafdfddcb8f8f31e207\Duplicati\Library\Backend\S3\Duplicati.Library.Backend.S3.csproj",
                @"C:\ProjetosCSharp\FeatureEnvyProjects\duplicati-0a1b32e1887c98c6034c9fafdfddcb8f8f31e207\Duplicati\Library\Backend\SharePoint\Duplicati.Library.Backend.SharePoint.csproj",
                @"C:\ProjetosCSharp\FeatureEnvyProjects\duplicati-0a1b32e1887c98c6034c9fafdfddcb8f8f31e207\Duplicati\Library\Backend\TahoeLAFS\Duplicati.Library.Backend.TahoeLAFS.csproj",
                @"C:\ProjetosCSharp\FeatureEnvyProjects\duplicati-0a1b32e1887c98c6034c9fafdfddcb8f8f31e207\Duplicati\Library\Main\Duplicati.Library.Main.csproj",
                @"C:\ProjetosCSharp\FeatureEnvyProjects\duplicati-0a1b32e1887c98c6034c9fafdfddcb8f8f31e207\Duplicati\Server\Duplicati.Server.csproj",
				
				@"C:\ProjetosCSharp\FeatureEnvyProjects\Files-89c33841813a5590e6bf44fb02bb7d06348320c3\Files.Launcher\Files.Launcher.csproj",
				@"C:\ProjetosCSharp\FeatureEnvyProjects\Files-89c33841813a5590e6bf44fb02bb7d06348320c3\Files\Files.csproj",
				
				@"C:\ProjetosCSharp\FeatureEnvyProjects\GitExtensions\SpellChecker\SpellChecker.csproj",
                @"C:\ProjetosCSharp\FeatureEnvyProjects\GitExtensions\GitCommands\GitCommands.csproj",
				@"C:\ProjetosCSharp\FeatureEnvyProjects\GitExtensions\GitEntensions\GitEntensions.csproj",
				@"C:\ProjetosCSharp\FeatureEnvyProjects\GitExtensions\GitUI\GitUI.csproj",
				@"C:\ProjetosCSharp\FeatureEnvyProjects\GitExtensions\JiraCommitHintPlugin\JiraCommitHintPlugin.csproj",
                @"C:\ProjetosCSharp\FeatureEnvyProjects\GitExtensions\Gource\Gource.csproj",
                @"C:\ProjetosCSharp\FeatureEnvyProjects\GitExtensions\GitStatistics\GitStatistics.csproj",
                @"C:\ProjetosCSharp\FeatureEnvyProjects\GitExtensions\CreateLocalBranches\CreateLocalBranches.csproj",
                @"C:\ProjetosCSharp\FeatureEnvyProjects\GitExtensions\BackgroundFetch\BackgroundFetch.csproj",
                @"C:\ProjetosCSharp\FeatureEnvyProjects\GitExtensions\ResourceManager\ResourceManager.csproj",

				@"C:\ProjetosCSharp\FeatureEnvyProjects\Newtonsoft.Json\Newtonsoft.Json\Newtonsoft.Json.csproj",
				
				@"C:\ProjetosCSharp\FeatureEnvyProjects\jellyfin-6c2eb5fc7e872a29b4a0951849681ae0764dbb8e\Emby.Dlna\Emby.Dlna.csproj",
				@"C:\ProjetosCSharp\FeatureEnvyProjects\jellyfin-6c2eb5fc7e872a29b4a0951849681ae0764dbb8e\Emby.Photos\Emby.Photos.csproj",
				@"C:\ProjetosCSharp\FeatureEnvyProjects\jellyfin-6c2eb5fc7e872a29b4a0951849681ae0764dbb8e\Emby.Server.Implementations\Emby.Server.Implementations.csproj",
				@"C:\ProjetosCSharp\FeatureEnvyProjects\jellyfin-6c2eb5fc7e872a29b4a0951849681ae0764dbb8e\Jellyfin.Api\Jellyfin.Api.csproj",
				@"C:\ProjetosCSharp\FeatureEnvyProjects\jellyfin-6c2eb5fc7e872a29b4a0951849681ae0764dbb8e\Jellyfin.Server.Implementations\Jellyfin.Server.Implementations.csproj",
				@"C:\ProjetosCSharp\FeatureEnvyProjects\jellyfin-6c2eb5fc7e872a29b4a0951849681ae0764dbb8e\Jellyfin.Server\Jellyfin.Server.csproj",
				@"C:\ProjetosCSharp\FeatureEnvyProjects\jellyfin-6c2eb5fc7e872a29b4a0951849681ae0764dbb8e\MediaBrowser.Controller\MediaBrowser.Controller.csproj",
				@"C:\ProjetosCSharp\FeatureEnvyProjects\jellyfin-6c2eb5fc7e872a29b4a0951849681ae0764dbb8e\MediaBrowser.LocalMetadata\MediaBrowser.LocalMetadata.csproj",
				@"C:\ProjetosCSharp\FeatureEnvyProjects\jellyfin-6c2eb5fc7e872a29b4a0951849681ae0764dbb8e\MediaBrowser.Model\MediaBrowser.Model.csproj",
				@"C:\ProjetosCSharp\FeatureEnvyProjects\jellyfin-6c2eb5fc7e872a29b4a0951849681ae0764dbb8e\MediaBrowser.XbmcMetadata\MediaBrowser.XbmcMetadata.csproj",
				
				@"C:\ProjetosCSharp\FeatureEnvyProjects\ScreenToGif-2d318f837946f730e1b2e5c708ae9f776b9e360b\ScreenToGif\ScreenToGif.csproj",
				
				@"C:\ProjetosCSharp\FeatureEnvyProjects\osu-2cac373365309a40474943f55c56159ed8f9433c\osu.Game.Rulesets.Osu\osu.Game.Rulesets.Osu.csproj",
				@"C:\ProjetosCSharp\FeatureEnvyProjects\osu-2cac373365309a40474943f55c56159ed8f9433c\osu.Game.Rulesets.Taiko\osu.Game.Rulesets.Taiko.csproj",
				@"C:\ProjetosCSharp\FeatureEnvyProjects\osu-2cac373365309a40474943f55c56159ed8f9433c\osu.Game.Tournament\osu.Game.Tournament.csproj",
				@"C:\ProjetosCSharp\FeatureEnvyProjects\osu-2cac373365309a40474943f55c56159ed8f9433c\osu.Game\osu.Game.csproj",
				
				@"C:\ProjetosCSharp\FeatureEnvyProjects\Radarr-5ce1829709e7e1de3953c04e5dab4f3a9d450b38\src\NzbDrone.Core\Radarr.Core.csproj",
				@"C:\ProjetosCSharp\FeatureEnvyProjects\Radarr-5ce1829709e7e1de3953c04e5dab4f3a9d450b38\src\Radarr.Api.V3\Radarr.Api.V3.csproj",
				@"C:\ProjetosCSharp\FeatureEnvyProjects\Radarr-5ce1829709e7e1de3953c04e5dab4f3a9d450b38\src\Radarr.Http\Radarr.Http.csproj",
				
				@"C:\ProjetosCSharp\FeatureEnvyProjects\ShareX-c9a71ed00eda0e7c5a45237b9bcd3f8f614cda63\ShareX.HelpersLib\ShareX.HelpersLib.csproj",
				@"C:\ProjetosCSharp\FeatureEnvyProjects\ShareX-c9a71ed00eda0e7c5a45237b9bcd3f8f614cda63\ShareX.IndexerLib\ShareX.IndexerLib.csproj",
				@"C:\ProjetosCSharp\FeatureEnvyProjects\ShareX-c9a71ed00eda0e7c5a45237b9bcd3f8f614cda63\ShareX.ScreenCaptureLib\ShareX.ScreenCaptureLib.csproj",
				@"C:\ProjetosCSharp\FeatureEnvyProjects\ShareX-c9a71ed00eda0e7c5a45237b9bcd3f8f614cda63\ShareX.UploadersLib\ShareX.UploadersLib.csproj",
				@"C:\ProjetosCSharp\FeatureEnvyProjects\ShareX-c9a71ed00eda0e7c5a45237b9bcd3f8f614cda63\ShareX\ShareX.csproj",
				
				@"C:\ProjetosCSharp\FeatureEnvyProjects\Sonarr-6378e7afef6072eae20f6408818c6fb1c85661b7\src\NzbDrone.Api\Sonarr.Api.csproj",
				@"C:\ProjetosCSharp\FeatureEnvyProjects\Sonarr-6378e7afef6072eae20f6408818c6fb1c85661b7\src\NzbDrone.Core\Sonarr.Core.csproj",
				@"C:\ProjetosCSharp\FeatureEnvyProjects\Sonarr-6378e7afef6072eae20f6408818c6fb1c85661b7\src\NzbDrone.Host\Sonarr.Host.csproj",
				@"C:\ProjetosCSharp\FeatureEnvyProjects\Sonarr-6378e7afef6072eae20f6408818c6fb1c85661b7\src\Sonarr.Api.V3\Sonarr.Api.V3.csproj",
				@"C:\ProjetosCSharp\FeatureEnvyProjects\Sonarr-6378e7afef6072eae20f6408818c6fb1c85661b7\src\Sonarr.Http\Sonarr.Http.csproj",
				
				//faltou ml.agent
			
				


            };
        }
    }
}

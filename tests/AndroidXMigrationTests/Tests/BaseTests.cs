﻿using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Mono.Cecil;
using Xunit;

namespace Xamarin.AndroidX.Migration.Tests
{
	public class BaseTests
	{
		public const uint PortablePdbSignature = 0x424a5342;

		public const string MergedSupportDll = "AndroidSupport.Merged.dll";
		public const string MergedAndroidXDll = "AndroidX.Merged.dll";
		public const string ActiveDirectoryDll = "Microsoft.IdentityModel.Clients.ActiveDirectory.dll";
		public const string ActiveDirectoryPdb = "Microsoft.IdentityModel.Clients.ActiveDirectory.pdb";

		public const string ManagedSupportDll = "Aarxercise.Managed.Support.dll";
		public const string ManagedAndroidXDll = "Aarxercise.Managed.AndroidX.dll";
		public const string OldSupportDll = "Aarxercise.Old.Support.dll";
		public const string OldAndroidXDll = "Aarxercise.Old.AndroidX.dll";
		public const string BindingSupportDll = "Aarxercise.Binding.Support.dll";
		public const string BindingAndroidXDll = "Aarxercise.Binding.AndroidX.dll";
		public const string ReferenceSupportDll = "Aarxercise.Reference.Support.dll";
		public const string ReferenceAndroidXDll = "Aarxercise.Reference.AndroidX.dll";

		public const string SupportAar = "aarxersise.java.support.aar";
		public const string AndroidXAar = "aarxersise.java.androidx.aar";

		public const string FacebookSdkZip = "facebook-android-sdk.zip";

		public static Stream ReadAarEntry(string aarFilename, string entryFilename)
		{
			using (var file = File.OpenRead(aarFilename))
			{
				return ReadAarEntry(file, entryFilename);
			}
		}

		public static Stream ReadAarEntry(Stream aar, string entryFilename)
		{
			// convert to aar slashes
			entryFilename = entryFilename.Replace("\\", "/");

			using (var archive = new ZipArchive(aar, ZipArchiveMode.Read, true))
			{
				var entry = archive.Entries.FirstOrDefault(e => e.FullName == entryFilename);

				if (entry != null)
				{
					using (var stream = entry.Open())
					{
						var output = new MemoryStream();
						stream.CopyTo(output);
						output.Position = 0;

						return output;
					}
				}
			}

			return null;
		}

		public static string ExtractAarEntry(string aarFilename, string entryFilename)
		{
			using (var file = File.OpenRead(aarFilename))
			{
				return ExtractAarEntry(aarFilename, entryFilename);
			}
		}

		public static string ExtractAarEntry(Stream aar, string entryFilename)
		{
			var destFile = Utils.GetTempFilename(entryFilename);

			using (var file = File.OpenWrite(destFile))
			using (var stream = ReadAarEntry(aar, entryFilename))
			{
				stream.CopyTo(file);
			}

			return destFile;
		}

		public static string CreateFile(string filename, string contents)
		{
			var destFile = Utils.GetTempFilename(filename);

			File.WriteAllText(destFile, contents);

			return destFile;
		}

		public static string RunMigration(string supportDll, CecilMigrationResult expectedResult, string logPath = null)
		{
			var migratedDll = Utils.GetTempFilename() + ".dll";

			var migrator = new CecilMigrator();

			if (!string.IsNullOrWhiteSpace(logPath))
			{
				migrator.MessageLogged += (sender, e) =>
				{
					var log = string.Empty;
					if (e.IsError)
						log += "ERROR: ";
					if (e.IsVerbose)
						log += "VERBOSE: ";
					log += e.Message;
					File.AppendAllLines(logPath, new [] { log });
				};
			}

			var result = migrator.Migrate(supportDll, migratedDll);

			Assert.Equal(expectedResult, result);

			return migratedDll;
		}

		public static bool IsPortablePdb(string filename)
		{
			using var fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
			using var br = new BinaryReader(fs);
			
			return br.ReadUInt32() == PortablePdbSignature;
		}

		public static void AssertHasSupportTypes(IEnumerable<TypeReference> types)
		{
			Assert.NotEmpty(types.Where(t => t.FullName.StartsWith("Android.Support.") || t.FullName.StartsWith("Android.Arch.")));
		}

		public static void AssertNoSupportTypes(IEnumerable<TypeReference> types)
		{
			Assert.Empty(types.Where(t => 
				(t.FullName.StartsWith("Android.Support.") || t.FullName.StartsWith("Android.Arch.")) &&
				(t.FullName != "Android.Support.V4.Media.Session.MediaSessionCompat")));
		}
	}
}

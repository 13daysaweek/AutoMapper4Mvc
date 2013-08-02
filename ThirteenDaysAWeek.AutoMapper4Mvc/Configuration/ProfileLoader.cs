﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMapper;

namespace ThirteenDaysAWeek.AutoMapper4Mvc.Configuration
{
    /// <summary>
    /// Contains methods for initializing AutoMapper configuration via profiles contained in referenced
    /// assemblies
    /// </summary>
    public static class ProfileLoader
    {
        /// <summary>
        /// Scans all loaded assemblies and initializes AutoMapper with any classes that inherit from
        /// AutoMapper.Profile
        /// </summary>
        [Obsolete("LoadProfiles is obsolete, use LoadProfiles(IProfileStrategy loadStrategy) instead")]
        public static void LoadProfiles()
        {
            Assembly[] assembliesToScan = AppDomain.CurrentDomain.GetAssemblies();
            Type profileType = typeof (Profile);

            IEnumerable<Type> profiles = assembliesToScan.SelectMany(assembly => assembly.GetTypes())
                               .Where(type => profileType.IsAssignableFrom(type) && !type.IsAbstract && type != profileType);

            Mapper.Initialize(configuration =>
                {
                    foreach (Type profile in profiles)
                    {
                        configuration.AddProfile((Profile) Activator.CreateInstance(profile));
                    }
                });
        }

        /// <summary>
        /// Scans all assemblies returned by the provided IProfileLoadStrategy and configures
        /// AutoMapper with any profiles found in the specified assemblies
        /// </summary>
        /// <param name="loadStrategy">An IProfileLoadStrategy implementation that returns a list of assemblies to scan</param>
        public static void LoadProfiles(IProfileLoadStrategy loadStrategy)
        {
            IEnumerable<Assembly> assembliesToScan = loadStrategy.GetAssembliesToScan();
            IEnumerable<Type> profiles = GetProfileTypes(assembliesToScan);

            Mapper.Initialize(configuration => profiles.ForEach(type => configuration.AddProfile((Profile) Activator.CreateInstance(type))));
        }

        private static IEnumerable<Type> GetProfileTypes(IEnumerable<Assembly> assembliesToScan)
        {
            Type profileType = typeof (Profile);

            IEnumerable<Type> profiles = assembliesToScan.SelectMany(assembly => assembly.GetTypes())
                .Where(type => profileType.IsAssignableFrom(type) && !type.IsAbstract && type != profileType);

            return profiles;
        }
    }
}

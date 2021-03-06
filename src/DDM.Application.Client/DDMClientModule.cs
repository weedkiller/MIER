﻿using Abp.Modules;
using Abp.Reflection.Extensions;

namespace DDM
{
    public class DDMClientModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(DDMClientModule).GetAssembly());
        }
    }
}

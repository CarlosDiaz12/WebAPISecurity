﻿using AutoMapper;
using Microsoft.AspNetCore.DataProtection;
using WebAPISecurity.Models;

namespace WebAPISecurity
{
        public class IdProtectorConverter : ITypeConverter<BudgetCategory, BudgetCategoryDto>
        {
            private readonly IDataProtector protector;
            public IdProtectorConverter(IDataProtectionProvider dataProtectionProvider)
            {
                protector = dataProtectionProvider.CreateProtector(StringConstants.IdQryStr);
            }
            public BudgetCategoryDto Convert(BudgetCategory source, BudgetCategoryDto destination, ResolutionContext context)
            {
                return new BudgetCategoryDto
                {
                    Name = source.Name,
                    Amount = source.Amount,
                    EncryptId = protector.Protect(source.Id.ToString())
                };
            }
        }
    
}

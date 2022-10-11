using Microsoft.AspNetCore.Mvc;
using OnlineShopAdmin.DataAccess.Models;
using OnlineShopAdmin.DataAccess.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineShopAdmin.DataAccess
{
    public class Test
    {
        private readonly IRepository<Address> _addressRepository;

        public Test(IRepository<Address> addressRepository)
        {
            _addressRepository = addressRepository;
        }

        public async Task<IEnumerable<Address>> Index(CancellationToken cancellationToken = default)
        {
            var result = await _addressRepository.GetListAsync(cancellationToken:cancellationToken);
            return result ;
        }
    }
}

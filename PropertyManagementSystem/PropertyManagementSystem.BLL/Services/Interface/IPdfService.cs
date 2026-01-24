using PropertyManagementSystem.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystem.BLL.Services.Interface
{
    public interface IPdfService
    {
        Task<byte[]> GenerateLeasePdfAsync(Lease lease);
    }
}

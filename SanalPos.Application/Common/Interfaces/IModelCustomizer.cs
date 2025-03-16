using Microsoft.EntityFrameworkCore;

namespace SanalPos.Application.Common.Interfaces
{
    public interface IModelCustomizer
    {
        void Customize(ModelBuilder modelBuilder);
    }
} 
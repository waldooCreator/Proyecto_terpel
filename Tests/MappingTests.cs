using AutoMapper;
using Application.DTOs;
using Application.Mappings;
using Domain.Entities;
using Xunit;

namespace Tests
{
    public class MappingTests
    {
        [Fact]
        public void AutoMapper_Configuration_IsValid()
        {
            var cfg = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            cfg.AssertConfigurationIsValid();
        }

        [Fact]
        public void Map_RegistroVentaDto_To_RegistroVentaEDS()
        {
            var cfg = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            var mapper = cfg.CreateMapper();

            var dto = new RegistroVentaDto { CódigoEstación = "001", Estación = "X" };
            var entity = mapper.Map<RegistroVentaEDS>(dto);
            Assert.Equal("001", entity.CódigoEstación);
            Assert.Equal("X", entity.Estación);
        }
    }
}

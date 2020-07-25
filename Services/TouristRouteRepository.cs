using FakeXieCheng.API.Database;
using FakeXieCheng.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FakeXieCheng.API.Services
{
    public class TouristRouteRepository : ITouristRouteRepository
    {
        readonly AppDbContext _context;
        public TouristRouteRepository(AppDbContext context)
        {
            _context = context;
        }

        public TouristRoutePicture GetPicture(int pictureId)
        {
            return _context.TouristRoutePictures.Where(item=>item.Id==pictureId).FirstOrDefault();
        }

        public IEnumerable<TouristRoutePicture> GetPicturesByTouristRouteId(Guid touristRouteId)
        {
            return (_context.TouristRoutePictures.Where(p => p.TouristRouteId == touristRouteId)).ToList();
        }

        public TouristRoute GetTouristRoute(Guid touristRouteId)
        {
           return _context.TouristRoutes.Include(t=>t.TouristRoutePictures).FirstOrDefault(n=>n.Id==touristRouteId);
        }

        public IEnumerable<TouristRoute> GetTouristRoutes()
        {
            return _context.TouristRoutes.Include(t=>t.TouristRoutePictures);
        }

        public bool TouristRouteExists(Guid touristRouteId)
        {
            return _context.TouristRoutes.Any(t=>t.Id==touristRouteId);
        }
    }
}

using FakeXieCheng.API.Database;
using FakeXieCheng.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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

        public IEnumerable<TouristRoute> GetTouristRoutes(string keyword,string operatorType,int? rating)
        {
            IQueryable<TouristRoute> result = _context.TouristRoutes.Include(t => t.TouristRoutePictures);
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                result = result.Where(t=>t.Title.Contains(keyword));
            }
            if (rating>0)
            {
                result = operatorType switch
                {
                    "largerThan"=>result.Where(t=>t.Rating>=rating),
                    "lessThan"=>result.Where(t=>t.Rating<=rating),
                    _=>result.Where(t=>t.Rating==rating),
                };
            }
            return result.ToList();
        }

        public bool TouristRouteExists(Guid touristRouteId)
        {
            return _context.TouristRoutes.Any(t=>t.Id==touristRouteId);
        }
    }
}

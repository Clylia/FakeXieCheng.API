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
        public void AddTouristRoute(TouristRoute touristRoute)
        {
            if (touristRoute==null)
            {
                throw new ArgumentNullException(nameof(touristRoute));
            }
            _context.TouristRoutes.Add(touristRoute);
            //_context.SaveChanges();
            //Save();
        }
        public bool Save()
        {
            return (_context.SaveChanges()>=0 );
        }

        public void AddTouristRoutePicture(Guid touristRouteId, TouristRoutePicture touristRoutePicture)
        {
            if (touristRouteId==Guid.Empty)
            {
                throw new ArgumentNullException( nameof(touristRouteId));

            }
            if (touristRoutePicture==null)
            {

                throw new ArgumentNullException(nameof(touristRoutePicture));
            }
            touristRoutePicture.TouristRouteId = touristRouteId;
            _context.TouristRoutePictures.Add(touristRoutePicture);
        }
    }
}

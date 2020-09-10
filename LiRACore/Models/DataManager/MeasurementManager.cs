using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiRACore.Models.RawData;
using LiRACore.Models.RawData;

using LiRACore.Models.Repository;

using Microsoft.EntityFrameworkCore;

namespace LiRACore.Models.DataManager
{


    public class MeasurementManager : IDataRepository<Measurement>
    {
        readonly DeviceContext _rawDataContext;

        public MeasurementManager(DeviceContext context)
        {
            _rawDataContext = context;
        }


        //IEnumerable<Measurement> IDataRepository<Measurement>.GetAll()
        //{
        //    throw new NotImplementedException();
        //}

        //public Measurement Get(Guid id)
        //{
        //    throw new NotImplementedException();
        //}

        //public Measurement Get(int id)
        //{
        //    return _rawDataContext.Measurement.FirstOrDefault(e => e.MeasurementTypeId == id);
        //}

        //public MeasurementType GetLastItemId()
        //{
        //    return _rawDataContext.OrderByDescending(mt => mt.MeasurementTypeId)
        //               .FirstOrDefault();
        //}

        //MeasurementType IDataRepository<Measurement>.Get(Guid id1, Guid id2)
        //{
        //    throw new NotImplementedException();
        //}

        //public void Add(Measurement entity)
        //{
        //    _rawDataContext.Add(entity);
        //    _rawDataContext.SaveChanges();
        //}

        //public void Update(Measurement dbEntity, Measurement entity)
        //{
        //    throw new NotImplementedException();
        //}

        //public void Delete(Measurement entity)
        //{
        //    throw new NotImplementedException();
        //}

        //public Measurement GetByName(string name)
        //{
        //    return _rawDataContext.Measurement.FirstOrDefault(mt => mt.type == name);
        //}

        Measurement IDataRepository<Measurement>.Get(Guid id)
        {
            throw new NotImplementedException();
        }

        Measurement IDataRepository<Measurement>.Get(Guid id1, Guid id2)
        {
            throw new NotImplementedException();
        }

        Measurement IDataRepository<Measurement>.GetByName(string name)
        {
            throw new NotImplementedException();
        }

        Measurement IDataRepository<Measurement>.GetLastItemId()
        {
            throw new NotImplementedException();
        }

        public void AddAll(ICollection<Measurement> entities, Guid deviceId, Guid tripId)
        {


            Device device = _rawDataContext.Devices.Include(d => d.Trips).Where(d => d.DeviceId == deviceId).FirstOrDefault();
            Trip trip = device.Trips.Where(t => t.TripId == tripId).FirstOrDefault();

        

            foreach (Measurement entity in entities)
            {
                entity.Trip = trip;
                
                trip.Measurements.Add(entity);
            }


            _rawDataContext.SaveChanges();

        }

        void IDataRepository<Measurement>.Add(Measurement entity)
        {

            //Device device = _rawDataContext.Devices.Include(d => d.DeviceId).FirstOrDefault().Trips.Contains(d => d.TripId == );

            //Trip trip = device.Trips.Include(d => d.DeviceId).FirstOrDefault();

            //device.Trips.include


            //    _rawDataContext.Devices.Add(entity);
            //_rawDataContext.SaveChanges();

        }


        void IDataRepository<Measurement>.Update(Measurement dbEntity, Measurement entity)
        {
            throw new NotImplementedException();
        }

        void IDataRepository<Measurement>.Delete(Measurement entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Measurement> GetAll()
        {
            throw new NotImplementedException();
        }



        //public void Add(Trip entity)
        //{
        //    throw new NotImplementedException();
        //}

        //public void Update(Trip dbEntity, Trip entity)
        //{
        //    throw new NotImplementedException();
        //}

        //public void Delete(Trip entity)
        //{
        //    throw new NotImplementedException();
        //}

        //public SensorData Get(long id)
        //{
        //    throw new NotImplementedException();
        //}
    }



    // The RawDataManager handles all database operations related to the SensorData.The purpose of this class is to separate the actual data operations logic from our API Controller.
    //public class RawDataManager : IDataRepository<Measurement>
    //{
    //    readonly MeasurementContext _rawDataContext;

    //    public RawDataManager(MeasurementContext context)
    //    {
    //        _rawDataContext = context;
    //    }

    //    public IEnumerable<Measurement> GetAll()
    //    {
    //        return _rawDataContext.Measurements.ToList();
    //    }


    //    public Measurement Get(string id)
    //    //  public SensorData Get(long id)
    //    {
    //        return _rawDataContext.Measurements
    //              .FirstOrDefault(e => e.MeasurementId.ToString() == id);
    //    }

    //    public void Add(Measurement entity)
    //    {
    //        _rawDataContext.Measurements.Add(entity);
    //        _rawDataContext.SaveChanges();
    //    }

    //    public void Update(Measurement rawData, Measurement entity)
    //    {
    //        //rawData.FirstName = entity.FirstName;
    //        //rawData.LastName = entity.LastName;
    //        //rawData.Email = entity.Email;
    //        //rawData.DateOfBirth = entity.DateOfBirth;
    //        //rawData.PhoneNumber = entity.PhoneNumber;

    //        _rawDataContext.SaveChanges();
    //    }

    //    public void Delete(Measurement rawData)
    //    {
    //        _rawDataContext.Measurements.Remove(rawData);
    //        _rawDataContext.SaveChanges();
    //    }

    //    //public SensorData Get(long id)
    //    //{
    //    //    throw new NotImplementedException();
    //    //}
    //}
}

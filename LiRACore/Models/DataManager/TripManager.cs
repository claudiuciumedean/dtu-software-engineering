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


    public class TripManager : IDataRepository<Trip>
    {
        readonly DeviceContext _rawDataContext;

        public TripManager(DeviceContext context)
        {
            _rawDataContext = context;
        }

        public IEnumerable<Device> GetAll()
        {
            return _rawDataContext.Devices.ToList();
          // return null;
        }


      //  public Device Get(Guid id)
      //  //  public SensorData Get(long id)
      //  {
      //      // return _rawDataContext.Devices
      //      //       .FirstOrDefault(e => e.DeviceId == id);
      //     return _rawDataContext.Devices.Include(entity => entity.Trips).FirstOrDefault(e => e.DeviceId == id);

      ////      return _rawDataContext.Devices.Include(d => d.DeviceId).First();
      //  }

        public void Add(Device entity)
        {

               var device = _rawDataContext.Devices.Include(d => d.DeviceId);

                _rawDataContext.Devices.Add(entity);
                _rawDataContext.SaveChanges();
            
        }

        public void Update(Measurement rawData, Measurement entity)
        {
            //rawData.FirstName = entity.FirstName;
            //rawData.LastName = entity.LastName;
            //rawData.Email = entity.Email;
            //rawData.DateOfBirth = entity.DateOfBirth;
            //rawData.PhoneNumber = entity.PhoneNumber;

            _rawDataContext.SaveChanges();
        }

        public void Delete(Device rawData)
        {
            _rawDataContext.Devices.Remove(rawData);
            _rawDataContext.SaveChanges();
        }

        public void Update(Device dbEntity, Device entity)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Trip> IDataRepository<Trip>.GetAll()
        {
            throw new NotImplementedException();
        }

        Trip IDataRepository<Trip>.Get(Guid tripId)
        {
            // return _rawDataContext.Devices.Include(entity => entity.Trips).FirstOrDefault(e => e.DeviceId == deviceId).Trips.FirstOrDefault(t => t.TripId == tripId);

            return _rawDataContext.Trips.FirstOrDefault(e => e.TripId == tripId);
        }

        public Trip Get(Guid deviceId, Guid tripId)
        {
            //return _rawDataContext.Devices.Include(entity => entity.Trips).FirstOrDefault(e => e.DeviceId == deviceId).Trips.FirstOrDefault(t => t.TripId == tripId);
           // return _rawDataContext.Trips.FirstOrDefault(e => e.TripId == tripId && e.FK_Device == deviceId);
            return _rawDataContext.Devices.Include(entity => entity.Trips).FirstOrDefault(e => e.DeviceId == deviceId).Trips.FirstOrDefault(t => t.TripId == tripId);

        }

        public void Add(Trip entity)
        {
            _rawDataContext.Trips.Add(entity);
            _rawDataContext.SaveChanges();

        }

        public void Update(Trip dbEntity, Trip entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Trip entity)
        {
            throw new NotImplementedException();
        }

        public Trip GetByName(string name)
        {
            throw new NotImplementedException();
        }

        public Trip GetLastItemId()
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

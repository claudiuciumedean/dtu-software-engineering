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


    public class MeasurementTypeManager : IDataRepository<MeasurementType>
    {
        readonly DeviceContext _rawDataContext;

        public MeasurementTypeManager(DeviceContext context)
        {
            _rawDataContext = context;
        }


        IEnumerable<MeasurementType> IDataRepository<MeasurementType>.GetAll()
        {
            throw new NotImplementedException();
        }

        public MeasurementType Get(Guid id)
        {
            throw new NotImplementedException();
        }

        public MeasurementType Get(int id)
        {
            return _rawDataContext.MeasurementTypes.FirstOrDefault(e => e.MeasurementTypeId == id);
        }

        public MeasurementType GetLastItemId()
        {
            return _rawDataContext.MeasurementTypes.OrderByDescending(mt => mt.MeasurementTypeId)
                       .FirstOrDefault();
        }

        MeasurementType IDataRepository<MeasurementType>.Get(Guid id1, Guid id2)
        {
            throw new NotImplementedException();
        }

        public void Add(MeasurementType entity)
        {
            _rawDataContext.MeasurementTypes.Add(entity);
            _rawDataContext.SaveChanges();
        }

        public void Update(MeasurementType dbEntity, MeasurementType entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(MeasurementType entity)
        {
            throw new NotImplementedException();
        }

        public MeasurementType GetByName(string name)
        {
            return _rawDataContext.MeasurementTypes.FirstOrDefault(mt => mt.type == name);
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

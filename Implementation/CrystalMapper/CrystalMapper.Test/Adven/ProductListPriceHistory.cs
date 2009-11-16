/*
 * Author: CrystalMapper 
 * 
 * Date:  Wednesday, November 11, 2009 9:50 AM
 * 
 * Class: ProductListPriceHistory
 * 
 * Email: mk.faraz@gmail.com
 * 
 * Blogs: http://csharplive.wordpress.com, http://farazmasoodkhan.wordpress.com
 *
 * Website: http://www.linkedin.com/in/farazmasoodkhan
 *
 * Copyright: Faraz Masood Khan @ Copyright 2009
 *
/*/

using System;
using System.Data.Common;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;

using CoreSystem.Data;

using CrystalMapper;
using CrystalMapper.Data;
using CrystalMapper.Mapping;

namespace feedbook.Model
{
	[Table(TABLE_NAME)]
    public partial class ProductListPriceHistory : Entity< ProductListPriceHistory>  
    {		
		#region Table Schema
		
        public const string TABLE_NAME = "Production.ProductListPriceHistory";	
     
		public const string COL_PRODUCTID = "ProductID";
		public const string COL_STARTDATE = "StartDate";
		public const string COL_ENDDATE = "EndDate";
		public const string COL_LISTPRICE = "ListPrice";
		public const string COL_MODIFIEDDATE = "ModifiedDate";
		
        public const string PARAM_PRODUCTID = "@ProductID";	
        public const string PARAM_STARTDATE = "@StartDate";	
        public const string PARAM_ENDDATE = "@EndDate";	
        public const string PARAM_LISTPRICE = "@ListPrice";	
        public const string PARAM_MODIFIEDDATE = "@ModifiedDate";	
		
        #endregion
		
		#region Queries
		
		private const string SQL_INSERT_PRODUCTLISTPRICEHISTORY = "INSERT INTO Production.ProductListPriceHistory([ProductID],[StartDate],[EndDate],[ListPrice],[ModifiedDate]) VALUES (@ProductID,@StartDate,@EndDate,@ListPrice,@ModifiedDate);"  ;
		
		private const string SQL_UPDATE_PRODUCTLISTPRICEHISTORY = "UPDATE Production.ProductListPriceHistory SET  [EndDate] = @EndDate, [ListPrice] = @ListPrice, [ModifiedDate] = @ModifiedDate WHERE [ProductID] = @ProductID AND [StartDate] = @StartDate";
		
		private const string SQL_DELETE_PRODUCTLISTPRICEHISTORY = "DELETE FROM Production.ProductListPriceHistory WHERE  [ProductID] = @ProductID AND [StartDate] = @StartDate ";
		
        #endregion        
        
        #region Hash Code       
        
        private volatile int hashCode = 0;
        
        #endregion
        
        #region Declarations
        
		protected int productid = default(int);
	
		protected System.DateTime startdate = default(System.DateTime);
	
		protected System.DateTime? enddate = default(System.DateTime?);
	
		protected decimal listprice = default(decimal);
	
		protected System.DateTime modifieddate = default(System.DateTime);
	
		protected Product productEntity;
	
        #endregion

 		#region Properties	

        [Column( COL_STARTDATE, PARAM_STARTDATE, typeof(System.DateTime))]
                              public virtual System.DateTime StartDate 
        {
            get { return this.startdate; }
			set	{ 
                  if(this.startdate != value)
                    {
                        this.OnPropertyChanging(new PropertyChangingEventArgs("StartDate"));  
                        this.startdate = value; 
                        this.hashCode = 0;
                        this.OnPropertyChanged(new PropertyChangedEventArgs("StartDate"));
                    }   
                }
        }	
		
        [Column( COL_ENDDATE, PARAM_ENDDATE )]
                              public virtual System.DateTime? EndDate 
        {
            get { return this.enddate; }
			set	{ 
                  if(this.enddate != value)
                    {
                        this.OnPropertyChanging(new PropertyChangingEventArgs("EndDate"));  
                        this.enddate = value; 
                        this.OnPropertyChanged(new PropertyChangedEventArgs("EndDate"));
                    }   
                }
        }	
		
        [Column( COL_LISTPRICE, PARAM_LISTPRICE, typeof(decimal))]
                              public virtual decimal ListPrice 
        {
            get { return this.listprice; }
			set	{ 
                  if(this.listprice != value)
                    {
                        this.OnPropertyChanging(new PropertyChangingEventArgs("ListPrice"));  
                        this.listprice = value; 
                        this.OnPropertyChanged(new PropertyChangedEventArgs("ListPrice"));
                    }   
                }
        }	
		
        [Column( COL_MODIFIEDDATE, PARAM_MODIFIEDDATE, typeof(System.DateTime))]
                              public virtual System.DateTime ModifiedDate 
        {
            get { return this.modifieddate; }
			set	{ 
                  if(this.modifieddate != value)
                    {
                        this.OnPropertyChanging(new PropertyChangingEventArgs("ModifiedDate"));  
                        this.modifieddate = value; 
                        this.OnPropertyChanged(new PropertyChangedEventArgs("ModifiedDate"));
                    }   
                }
        }	
		
        [Column( COL_PRODUCTID, PARAM_PRODUCTID, default(int))]
                              public virtual int ProductID                
        {
            get
            {
                if(this.productEntity == null)
                    return this.productid ;
                
                return this.productEntity.ProductID;            
            }
            set
            {
                if(this.productid != value)
                {
                    this.OnPropertyChanging(new PropertyChangingEventArgs("ProductID"));                    
                    this.productid = value;                    
                    this.OnPropertyChanged(new PropertyChangedEventArgs("ProductID"));
                    
                    this.productEntity = null;
                }                
            }          
        }	
        
        public Product ProductEntity
        {
            get { 
                    if(this.productEntity == null
                       && this.productid != default(int)) 
                    {
                        Product productQuery = new Product {
                                                        ProductID = this.productid  
                                                        };
                        
                        Product[]  products = productQuery.ToList();                        
                        if(products.Length == 1)
                            this.productEntity = products[0];                        
                    }
                    
                    return this.productEntity; 
                }
			set	{ 
                  if(this.productEntity != value)
                    {
                        this.OnPropertyChanging(new PropertyChangingEventArgs("ProductEntity"));
                        if (this.productEntity != null)
                            this.Parents.Remove(this.productEntity);                            
                        
                        if((this.productEntity = value) != null) 
                            this.Parents.Add(this.productEntity); 
                        this.OnPropertyChanged(new PropertyChangedEventArgs("ProductEntity"));
                        
                        this.productid = this.ProductEntity.ProductID;
                    }   
                }
        }	
		
        
        #endregion        
        
        #region Methods     
		
        public ProductListPriceHistory()
        {
        }
        
        public override int GetHashCode()
        {      
            if(this.hashCode == 0)
            {
                int result = 7;            
                result = (11 * result) + this.productid.GetHashCode();
                result = (11 * result) + this.startdate.GetHashCode();
                this.hashCode = result;
             }           
            return this.hashCode;          
        }
        
        public override bool Equals(object obj)
        {
            ProductListPriceHistory entity = obj as ProductListPriceHistory;           
            
            return (
                    object.ReferenceEquals(this, entity)                    
                    || (
                        entity != null            
                        && this.ProductID == entity.ProductID
                        && this.StartDate == entity.StartDate
                        && this.ProductID != default(int)
                        && this.StartDate != default(System.DateTime)
                        )
                    );           
        }
        
		public override void Read(DbDataReader reader)
		{       
			this.productid = (int)reader[COL_PRODUCTID];
			this.startdate = (System.DateTime)reader[COL_STARTDATE];
			this.enddate = DbConvert.ToNullable< System.DateTime >(reader[COL_ENDDATE]);
			this.listprice = (decimal)reader[COL_LISTPRICE];
			this.modifieddate = (System.DateTime)reader[COL_MODIFIEDDATE];
            this.hashCode = 0;
            
            base.Read(reader);
		}
		
		public override bool Create(DataContext dataContext)
        {
            using(DbCommand command  = dataContext.CreateCommand(SQL_INSERT_PRODUCTLISTPRICEHISTORY))
            {	
				command.Parameters.Add(dataContext.CreateParameter(this.ProductID, PARAM_PRODUCTID));
				command.Parameters.Add(dataContext.CreateParameter(this.StartDate, PARAM_STARTDATE));
				command.Parameters.Add(dataContext.CreateParameter(DbConvert.DbValue(this.EndDate), PARAM_ENDDATE));
				command.Parameters.Add(dataContext.CreateParameter(this.ListPrice, PARAM_LISTPRICE));
				command.Parameters.Add(dataContext.CreateParameter(this.ModifiedDate, PARAM_MODIFIEDDATE));
                return (command.ExecuteNonQuery() == 1);
            }
        }

		public override bool Update(DataContext dataContext)
        {
            using(DbCommand command  = dataContext.CreateCommand(SQL_UPDATE_PRODUCTLISTPRICEHISTORY))
            {							
				command.Parameters.Add(dataContext.CreateParameter(this.ProductID, PARAM_PRODUCTID));
				command.Parameters.Add(dataContext.CreateParameter(this.StartDate, PARAM_STARTDATE));
				command.Parameters.Add(dataContext.CreateParameter(DbConvert.DbValue(this.EndDate), PARAM_ENDDATE));
				command.Parameters.Add(dataContext.CreateParameter(this.ListPrice, PARAM_LISTPRICE));
				command.Parameters.Add(dataContext.CreateParameter(this.ModifiedDate, PARAM_MODIFIEDDATE));
			
                return (command.ExecuteNonQuery() == 1);
            }
        }

		public override bool Delete(DataContext dataContext)
        {
            using(DbCommand command  = dataContext.CreateCommand(SQL_DELETE_PRODUCTLISTPRICEHISTORY))
            {							
				command.Parameters.Add(dataContext.CreateParameter(this.ProductID, PARAM_PRODUCTID));				
				command.Parameters.Add(dataContext.CreateParameter(this.StartDate, PARAM_STARTDATE));				
                return (command.ExecuteNonQuery() == 1);
            }
        }

        #endregion
        
        #region Entity Relationship Functions
        
        #endregion
    }
}
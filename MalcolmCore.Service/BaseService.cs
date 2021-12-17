using MalcolmCore.Data;
using MalcolmCore.IService;
using MalcolmCore.Utils.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MalcolmCore.Service
{
    /// <summary>
    /// 泛型方法，直接注入EF上下文
    /// </summary>
    public class BaseService : IBaseService
    {
        public DbContext db;

        /// <summary>
        /// 在使用的时候，自动注入db上下文
        /// </summary>
        /// <param name="db"></param>
        public BaseService(CoreFrameDBContext db)
        {
            this.db = db;

            //关闭全局追踪的代码
            //db.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }


        public void Text() 
        {
            //var data = from c in db.useinfo
            //           join p in db.useDetails on c.id equals p.id into r
            //           select r;

            //var data1 = db.useinfo.GroupJoin(db.useDetails, c => c.id, p => p.id, (c, p) => new { c,p});

            //var data2 = from c in db.useinfo
            //           join p in db.useDetails on c.id equals p.id into r
            //           from pc in r.DefaultIfEmpty()
            //           select pc;
        }

        /****************************************下面EFCore基础方法的封装（同步）***********************************************/
        //1. 直接提交数据库

        #region 01-数据源
        public IQueryable<T> Entities<T>() where T : class
        {
            return db.Set<T>();
        }

        public IQueryable<T> EntitiesNoTrack<T>() where T : class
        {
            return db.Set<T>().AsNoTracking();
        }

        #endregion

        #region 02-新增
        public int Add<T>(T model) where T : class
        {
            db.Entry(model).State = EntityState.Added;
            return db.SaveChanges();

        }
        #endregion

        #region 03-删除
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="model">需要删除的实体</param>
        /// <returns></returns>
        public int Del<T>(T model) where T : class
        {
            db.Entry(model).State = EntityState.Deleted;
            return db.SaveChanges();
        }
        #endregion

        #region 04-根据条件删除(支持批量删除)
        /// <summary>
        /// 根据条件删除(支持批量删除)
        /// </summary>
        /// <param name="delWhere">传入Lambda表达式(生成表达式目录树)</param>
        /// <returns></returns>
        public int DelBy<T>(Expression<Func<T, bool>> delWhere) where T : class
        {
            List<T> listDels = db.Set<T>().Where(delWhere).ToList();
            listDels.ForEach(model =>
            {
                db.Entry(model).State = EntityState.Deleted;
            });
            return db.SaveChanges();
        }
        #endregion

        #region 05-单实体修改
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model">修改后的实体</param>
        /// <returns></returns>
        public int Modify<T>(T model) where T : class
        {
            db.Entry(model).State = EntityState.Modified;
            return db.SaveChanges();
        }
        #endregion

        #region 06-批量修改（非lambda）
        /// <summary>
        /// 批量修改（非lambda）
        /// </summary>
        /// <param name="model">要修改实体中 修改后的属性 </param>
        /// <param name="whereLambda">查询实体的条件</param>
        /// <param name="proNames">lambda的形式表示要修改的实体属性名</param>
        /// <returns></returns>
        public int ModifyBy<T>(T model, Expression<Func<T, bool>> whereLambda, params string[] proNames) where T : class
        {
            List<T> listModifes = db.Set<T>().Where(whereLambda).ToList();
            Type t = typeof(T);
            List<PropertyInfo> proInfos = t.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
            Dictionary<string, PropertyInfo> dicPros = new Dictionary<string, PropertyInfo>();
            proInfos.ForEach(p =>
            {
                if (proNames.Contains(p.Name))
                {
                    dicPros.Add(p.Name, p);
                }
            });
            foreach (string proName in proNames)
            {
                if (dicPros.ContainsKey(proName))
                {
                    PropertyInfo proInfo = dicPros[proName];
                    object newValue = proInfo.GetValue(model, null);
                    foreach (T m in listModifes)
                    {
                        proInfo.SetValue(m, newValue, null);
                    }
                }
            }
            return db.SaveChanges();
        }
        #endregion

        #region 07-根据条件查询
        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="whereLambda">查询条件(lambda表达式的形式生成表达式目录树)</param>
        ///  <param name="isTrack">是否跟踪状态，默认是跟踪的</param>
        /// <returns></returns>
        public List<T> GetListBy<T>(Expression<Func<T, bool>> whereLambda, bool isTrack = true) where T : class
        {
            if (isTrack)
            {
                return db.Set<T>().Where(whereLambda).ToList();
            }
            else
            {
                return db.Set<T>().Where(whereLambda).AsNoTracking().ToList();
            }

        }
        #endregion

        #region 08-根据条件排序和查询
        /// <summary>
        /// 根据条件排序和查询
        /// </summary>
        /// <typeparam name="Tkey">排序字段类型</typeparam>
        /// <param name="whereLambda">查询条件</param>
        /// <param name="orderLambda">排序条件</param>
        /// <param name="isAsc">升序or降序</param>
        ///  <param name="isTrack">是否跟踪状态，默认是跟踪的</param>
        /// <returns></returns>
        public List<T> GetListBy<T, Tkey>(Expression<Func<T, bool>> whereLambda, Expression<Func<T, Tkey>> orderLambda, bool isAsc = true, bool isTrack = true) where T : class
        {
            IQueryable<T> data = null;
            if (isTrack)
            {
                data = db.Set<T>().Where(whereLambda);
            }
            else
            {
                data = db.Set<T>().Where(whereLambda).AsNoTracking();
            }
            if (isAsc)
            {
                data = data.OrderBy(orderLambda);
            }
            else
            {
                data = data.OrderByDescending(orderLambda);
            }
            return data.ToList();
        }
        #endregion

        #region 09-分页查询(根据Lambda排序)
        /// <summary>
        /// 根据条件排序和查询
        /// </summary>
        /// <typeparam name="Tkey">排序字段类型</typeparam>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="whereLambda">查询条件</param>
        /// <param name="orderLambda">排序条件</param>
        /// <param name="isAsc">升序or降序</param>
        ///  <param name="isTrack">是否跟踪状态，默认是跟踪的</param>
        /// <returns></returns>
        public List<T> GetPageList<T, Tkey>(int pageIndex, int pageSize, Expression<Func<T, bool>> whereLambda, Expression<Func<T, Tkey>> orderLambda, bool isAsc = true, bool isTrack = true) where T : class
        {

            IQueryable<T> data = null;
            if (isTrack)
            {
                data = db.Set<T>().Where(whereLambda);
            }
            else
            {
                data = db.Set<T>().Where(whereLambda).AsNoTracking();
            }
            if (isAsc)
            {
                data = data.OrderBy(orderLambda).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            }
            else
            {
                data = data.OrderByDescending(orderLambda).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            }
            return data.ToList();
        }
        #endregion

        #region 10-分页查询(根据名称排序)
        /// <summary>
        /// 分页查询输出总行数（根据名称排序）
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="rowCount">输出的总数量</param>
        /// <param name="whereLambda">查询条件</param>
        /// <param name="sortName">排序名称</param>
        /// <param name="sortDirection">asc 或 desc</param>
        ///  <param name="isTrack">是否跟踪状态，默认是跟踪的</param>
        /// <returns></returns>
        public List<T> GetPageListByName<T>(int pageIndex, int pageSize, Expression<Func<T, bool>> whereLambda, string sortName, SortEnum sortDirection, bool isTrack = true) where T : class
        {
            List<T> list = null;
            if (isTrack)
            {
                if (SortEnum.ASC == sortDirection) 
                {
                    list = db.Set<T>().Where(whereLambda).OrderBy(l => sortName)
                 .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                } 
                else 
                {
                    list = db.Set<T>().Where(whereLambda).OrderByDescending(l => sortName)
                 .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                }
                
            }
            else
            {
                if (SortEnum.ASC == sortDirection) 
                {
                    list = db.Set<T>().Where(whereLambda).AsNoTracking().OrderBy(l => sortName)
                 .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                } 
                else 
                {
                    list = db.Set<T>().Where(whereLambda).AsNoTracking().OrderByDescending(l => sortName)
                 .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                }
            }
            return list;
        }
        #endregion

        #region 11-分页查询输出总行数（根据Lambda排序）
        /// <summary>
        /// 根据条件排序和查询
        /// </summary>
        /// <typeparam name="Tkey">排序字段类型</typeparam>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="whereLambda">查询条件</param>
        /// <param name="orderLambda">排序条件</param>
        /// <param name="isAsc">升序or降序</param>
        ///  <param name="isTrack">是否跟踪状态，默认是跟踪的</param>
        /// <returns></returns>
        public List<T> GetPageList<T, Tkey>(int pageIndex, int pageSize, out int rowCount, Expression<Func<T, bool>> whereLambda, Expression<Func<T, Tkey>> orderLambda, bool isAsc = true, bool isTrack = true) where T : class
        {
            int count = db.Set<T>().Where(whereLambda).Count();
            IQueryable<T> data = null;
            if (isTrack)
            {
                data = db.Set<T>().Where(whereLambda);
            }
            else
            {
                data = db.Set<T>().Where(whereLambda).AsNoTracking();
            }
            if (isAsc)
            {
                data = data.OrderBy(orderLambda).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            }
            else
            {
                data = data.OrderByDescending(orderLambda).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            }
            rowCount = count;
            return data.ToList();
        }
        #endregion

        #region 12-分页查询输出总行数（根据名称排序）
        /// <summary>
        /// 分页查询输出总行数（根据名称排序）
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="rowCount">输出的总数量</param>
        /// <param name="whereLambda">查询条件</param>
        /// <param name="sortName">排序名称</param>
        /// <param name="sortDirection">asc 或 desc</param>
        ///  <param name="isTrack">是否跟踪状态，默认是跟踪的</param>
        /// <returns></returns>
        public List<T> GetPageListByName<T>(int pageIndex, int pageSize, out int rowCount, Expression<Func<T, bool>> whereLambda, string sortName, SortEnum sortDirection, bool isTrack = true) where T : class
        {
            int count = 0;
            count = db.Set<T>().Where(whereLambda).Count();
            List<T> list = null;
            if (isTrack)
            {
                if (SortEnum.ASC == sortDirection)
                {
                    list = db.Set<T>().Where(whereLambda).OrderBy(l => sortName)
                 .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                }
                else
                {
                    list = db.Set<T>().Where(whereLambda).OrderByDescending(l => sortName)
                 .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                }

            }
            else
            {
                if (SortEnum.ASC == sortDirection)
                {
                    list = db.Set<T>().Where(whereLambda).AsNoTracking().OrderBy(l => sortName)
                 .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                }
                else
                {
                    list = db.Set<T>().Where(whereLambda).AsNoTracking().OrderByDescending(l => sortName)
                 .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                }
            }
            rowCount = count;
            return list;
        }
        #endregion


        //2. SaveChange剥离出来，处理事务

        #region 01-批量处理SaveChange()
        /// <summary>
        /// 事务批量处理
        /// </summary>
        /// <returns></returns>
        public int SaveChange()
        {
            return db.SaveChanges();
        }
        #endregion

        #region 02-新增
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model">需要新增的实体</param>
        public void AddNo<T>(T model) where T : class
        {
            db.Entry(model).State = EntityState.Added;
        }
        #endregion

        #region 03-删除
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="model">需要删除的实体</param>
        public void DelNo<T>(T model) where T : class
        {
            db.Entry(model).State = EntityState.Deleted;
        }
        #endregion

        #region 04-根据条件删除
        /// <summary>
        /// 条件删除
        /// </summary>
        /// <param name="delWhere">需要删除的条件</param>
        public void DelByNo<T>(Expression<Func<T, bool>> delWhere) where T : class
        {
            List<T> listDels = db.Set<T>().Where(delWhere).ToList();
            listDels.ForEach(model =>
            {
                db.Entry(model).State = EntityState.Deleted;
            });
        }
        #endregion

        #region 05-修改
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model">修改后的实体</param>
        public void ModifyNo<T>(T model) where T : class
        {
            db.Entry(model).State = EntityState.Modified;
        }
        #endregion


        //3. EF调用sql语句

        #region 01-执行增加,删除,修改操作(或调用相关存储过程)
        /// <summary>
        /// 执行增加,删除,修改操作(或调用存储过程)
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public int ExecuteSql(string sql, params SqlParameter[] pars)
        {
            return db.Database.ExecuteSqlRaw(sql, pars);
        }

        #endregion

        #region 02-执行查询操作（调用查询类的存储过程）
        /// <summary>
        /// 执行查询操作
        /// 注：查询必须返回实体的所有属性字段；结果集中列名必须与属性映射的项目匹配；查询中不能包含关联数据
        /// 除Select以外其他的SQL语句无法执行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="isTrack">是否跟踪状态，默认是跟踪的</param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public List<T> ExecuteQuery<T>(string sql, bool isTrack = true, params SqlParameter[] pars) where T : class
        {
            if (isTrack)
            {
                //表示跟踪状态（默认是跟踪的）
                return db.Set<T>().FromSqlRaw(sql, pars).ToList();
            }
            else
            {
                //表示不跟踪状态
                return db.Set<T>().FromSqlRaw(sql, pars).AsNoTracking().ToList();
            }
        }
        #endregion

        #region 03-执行查询操作（与Linq相结合）
        /// <summary>
        /// 执行查询操作
        /// 注：查询必须返回实体的所有属性字段；结果集中列名必须与属性映射的项目匹配；查询中不能包含关联数据
        /// 除Select以外其他的SQL语句无法执行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        ///  <param name="whereLambda">查询条件</param>
        /// <param name="isTrack">是否跟踪状态，默认是跟踪的</param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public List<T> ExecuteQueryWhere<T>(string sql, Expression<Func<T, bool>> whereLambda, bool isTrack = true, params SqlParameter[] pars) where T : class
        {
            if (isTrack)
            {
                //表示跟踪状态（默认是跟踪的）
                return db.Set<T>().FromSqlRaw(sql, pars).Where(whereLambda).ToList();
            }
            else
            {
                //表示不跟踪状态
                return db.Set<T>().FromSqlRaw(sql, pars).Where(whereLambda).AsNoTracking().ToList();
            }
        }
        #endregion



        /****************************************下面EFCore基础方法的封装（异步）***********************************************/

        #region 01-新增
        public async Task<int> AddAsync<T>(T model) where T : class
        {
            await db.AddAsync(model);
            return await db.SaveChangesAsync();
        }
        #endregion

        #region 02-删除
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="model">需要删除的实体</param>
        /// <returns></returns>
        public async Task<int> DelAsync<T>(T model) where T : class
        {
            db.Entry(model).State = EntityState.Deleted;
            return await db.SaveChangesAsync();
        }
        #endregion

        #region 03-根据条件删除(支持批量删除)
        /// <summary>
        /// 根据条件删除(支持批量删除)
        /// </summary>
        /// <param name="delWhere">传入Lambda表达式(生成表达式目录树)</param>
        /// <returns></returns>
        public async Task<int> DelByAsync<T>(Expression<Func<T, bool>> delWhere) where T : class
        {
            List<T> listDels = await db.Set<T>().Where(delWhere).ToListAsync();
            listDels.ForEach(model =>
            {
                db.Entry(model).State = EntityState.Deleted;
            });
            return await db.SaveChangesAsync();
        }
        #endregion

        #region 04-单实体修改
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model">修改后的实体</param>
        /// <returns></returns>
        public async Task<int> ModifyAsync<T>(T model) where T : class
        {
            db.Entry(model).State = EntityState.Modified;
            return await db.SaveChangesAsync();
        }
        #endregion

        #region 05-批量修改（非lambda）
        /// <summary>
        /// 批量修改（非lambda）
        /// </summary>
        /// <param name="model">要修改实体中 修改后的属性 </param>
        /// <param name="whereLambda">查询实体的条件</param>
        /// <param name="proNames">lambda的形式表示要修改的实体属性名</param>
        /// <returns></returns>
        public async Task<int> ModifyByAsync<T>(T model, Expression<Func<T, bool>> whereLambda, params string[] proNames) where T : class
        {
            List<T> listModifes = await db.Set<T>().Where(whereLambda).ToListAsync();
            Type t = typeof(T);
            List<PropertyInfo> proInfos = t.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
            Dictionary<string, PropertyInfo> dicPros = new Dictionary<string, PropertyInfo>();
            proInfos.ForEach(p =>
            {
                if (proNames.Contains(p.Name))
                {
                    dicPros.Add(p.Name, p);
                }
            });
            foreach (string proName in proNames)
            {
                if (dicPros.ContainsKey(proName))
                {
                    PropertyInfo proInfo = dicPros[proName];
                    object newValue = proInfo.GetValue(model, null);
                    foreach (T m in listModifes)
                    {
                        proInfo.SetValue(m, newValue, null);
                    }
                }
            }
            return await db.SaveChangesAsync();
        }
        #endregion

        #region 06-根据条件查询
        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="whereLambda">查询条件(lambda表达式的形式生成表达式目录树)</param>
        ///  <param name="isTrack">是否跟踪状态，默认是跟踪的</param>
        /// <returns></returns>
        public async Task<List<T>> GetListByAsync<T>(Expression<Func<T, bool>> whereLambda, bool isTrack = true) where T : class
        {
            if (isTrack)
            {
                return await db.Set<T>().Where(whereLambda).ToListAsync();
            }
            else
            {
                return await db.Set<T>().Where(whereLambda).AsNoTracking().ToListAsync();
            }

        }
        #endregion

        #region 07-根据条件排序和查询
        /// <summary>
        /// 根据条件排序和查询
        /// </summary>
        /// <typeparam name="Tkey">排序字段类型</typeparam>
        /// <param name="whereLambda">查询条件</param>
        /// <param name="orderLambda">排序条件</param>
        /// <param name="isAsc">升序or降序</param>
        ///  <param name="isTrack">是否跟踪状态，默认是跟踪的</param>
        /// <returns></returns>
        public async Task<List<T>> GetListByAsync<T, Tkey>(Expression<Func<T, bool>> whereLambda, Expression<Func<T, Tkey>> orderLambda, bool isAsc = true, bool isTrack = true) where T : class
        {
            IQueryable<T> data = null;
            if (isTrack)
            {
                data = db.Set<T>().Where(whereLambda);
            }
            else
            {
                data = db.Set<T>().Where(whereLambda).AsNoTracking();
            }
            if (isAsc)
            {
                data = data.OrderBy(orderLambda);
            }
            else
            {
                data = data.OrderByDescending(orderLambda);
            }
            return await data.ToListAsync();
        }
        #endregion

        #region 08-分页查询(根据Lambda排序)
        /// <summary>
        /// 根据条件排序和查询
        /// </summary>
        /// <typeparam name="Tkey">排序字段类型</typeparam>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="whereLambda">查询条件</param>
        /// <param name="orderLambda">排序条件</param>
        /// <param name="isAsc">升序or降序</param>
        ///  <param name="isTrack">是否跟踪状态，默认是跟踪的</param>
        /// <returns></returns>
        public async Task<List<T>> GetPageListAsync<T, Tkey>(int pageIndex, int pageSize, Expression<Func<T, bool>> whereLambda, Expression<Func<T, Tkey>> orderLambda, bool isAsc = true, bool isTrack = true) where T : class
        {

            IQueryable<T> data = null;
            if (isTrack)
            {
                data = db.Set<T>().Where(whereLambda);
            }
            else
            {
                data = db.Set<T>().Where(whereLambda).AsNoTracking();
            }
            if (isAsc)
            {
                data = data.OrderBy(orderLambda).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            }
            else
            {
                data = data.OrderByDescending(orderLambda).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            }
            return await data.ToListAsync();
        }
        #endregion

        #region 09-分页查询(根据名称排序)
        /// <summary>
        /// 分页查询输出总行数（根据名称排序）
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="rowCount">输出的总数量</param>
        /// <param name="whereLambda">查询条件</param>
        /// <param name="sortName">排序名称</param>
        /// <param name="sortDirection">asc 或 desc</param>
        ///  <param name="isTrack">是否跟踪状态，默认是跟踪的</param>
        /// <returns></returns>
        public async Task<List<T>> GetPageListByNameAsync<T>(int pageIndex, int pageSize, Expression<Func<T, bool>> whereLambda, string sortName, SortEnum sortDirection, bool isTrack = true) where T : class
        {
            List<T> list = null;
            if (isTrack)
            {
                list = await db.Set<T>().Where(whereLambda).OrderBy(l=> sortName)
                 .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            }
            else
            {
                list = await db.Set<T>().Where(whereLambda).AsNoTracking().OrderBy(l => sortName)
                 .Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            }
            return list;
        }
        #endregion



        //2. SaveChange剥离出来，处理事务

        #region 01-批量处理SaveChange()
        /// <summary>
        /// 事务批量处理
        /// </summary>
        /// <returns></returns>
        public async Task<int> SaveChangeAsync()
        {
            return await db.SaveChangesAsync();
        }
        #endregion

        #region 02-新增
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model">需要新增的实体</param>
        public async Task<EntityEntry<T>> AddNoAsync<T>(T model) where T : class
        {
            return await db.AddAsync(model);
        }
        #endregion

        #region 03-根据条件删除
        /// <summary>
        /// 条件删除
        /// </summary>
        /// <param name="delWhere">需要删除的条件</param>
        public async Task DelByNoAsync<T>(Expression<Func<T, bool>> delWhere) where T : class
        {
            List<T> listDels = await db.Set<T>().Where(delWhere).ToListAsync();
            listDels.ForEach(model =>
            {
                db.Entry(model).State = EntityState.Deleted;
            });
        }
        #endregion


        //3. EF调用sql语句

        #region 01-执行增加,删除,修改操作(或调用存储过程)
        /// <summary>
        /// 执行增加,删除,修改操作(或调用存储过程)
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public async Task<int> ExecuteSqlAsync(string sql, params SqlParameter[] pars)
        {
            return await db.Database.ExecuteSqlRawAsync(sql, pars);
        }

        IQueryable<T> IBaseService.Entities<T>()
        {
            throw new NotImplementedException();
        }

        IQueryable<T> IBaseService.EntitiesNoTrack<T>()
        {
            throw new NotImplementedException();
        }

        public List<T> GetListBy<T>() where T : class
        {
            return db.Set<T>().ToList();
        }
        #endregion

    }
}

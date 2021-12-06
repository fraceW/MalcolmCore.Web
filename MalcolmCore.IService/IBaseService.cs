using MalcolmCore.Utils.Enum;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MalcolmCore.IService
{
    public interface IBaseService
    {
        /****************************************下面进行方法的封装（同步）***********************************************/
        //1. 直接提交数据库

        #region 01-数据源
        IQueryable<T> Entities<T>() where T : class;

        IQueryable<T> EntitiesNoTrack<T>() where T : class;

        #endregion

        #region 02-新增
        int Add<T>(T model) where T : class;

        #endregion

        #region 03-删除
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="model">需要删除的实体</param>
        /// <returns></returns>
        int Del<T>(T model) where T : class;

        #endregion

        #region 04-根据条件删除(支持批量删除)
        /// <summary>
        /// 根据条件删除(支持批量删除)
        /// </summary>
        /// <param name="delWhere">传入Lambda表达式(生成表达式目录树)</param>
        /// <returns></returns>
        int DelBy<T>(Expression<Func<T, bool>> delWhere) where T : class;

        #endregion

        #region 05-单实体修改
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model">修改后的实体</param>
        /// <returns></returns>
        int Modify<T>(T model) where T : class;

        #endregion

        #region 06-批量修改（非lambda）
        /// <summary>
        /// 批量修改（非lambda）
        /// </summary>
        /// <param name="model">要修改实体中 修改后的属性 </param>
        /// <param name="whereLambda">查询实体的条件</param>
        /// <param name="proNames">lambda的形式表示要修改的实体属性名</param>
        /// <returns></returns>
        int ModifyBy<T>(T model, Expression<Func<T, bool>> whereLambda, params string[] proNames) where T : class;

        #endregion

        #region 07-根据条件查询
        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="whereLambda">查询条件(lambda表达式的形式生成表达式目录树)</param>
        ///  <param name="isTrack">是否跟踪状态，默认是跟踪的</param>
        /// <returns></returns>
        List<T> GetListBy<T>(Expression<Func<T, bool>> whereLambda, bool isTrack = true) where T : class;

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
        List<T> GetListBy<T, Tkey>(Expression<Func<T, bool>> whereLambda, Expression<Func<T, Tkey>> orderLambda, bool isAsc = true, bool isTrack = true) where T : class;

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
        List<T> GetPageList<T, Tkey>(int pageIndex, int pageSize, Expression<Func<T, bool>> whereLambda, Expression<Func<T, Tkey>> orderLambda, bool isAsc = true, bool isTrack = true) where T : class;

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
        List<T> GetPageListByName<T>(int pageIndex, int pageSize, Expression<Func<T, bool>> whereLambda, string sortName, SortEnum sortDirection, bool isTrack = true) where T : class;

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
        List<T> GetPageList<T, Tkey>(int pageIndex, int pageSize, out int rowCount, Expression<Func<T, bool>> whereLambda, Expression<Func<T, Tkey>> orderLambda, bool isAsc = true, bool isTrack = true) where T : class;

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
        List<T> GetPageListByName<T>(int pageIndex, int pageSize, out int rowCount, Expression<Func<T, bool>> whereLambda, string sortName, SortEnum sortDirection, bool isTrack = true) where T : class;

        #endregion





        //2. SaveChange剥离出来，处理事务

        #region 01-批量处理SaveChange()
        /// <summary>
        /// 事务批量处理
        /// </summary>
        /// <returns></returns>
        int SaveChange();

        #endregion

        #region 02-新增
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model">需要新增的实体</param>
        void AddNo<T>(T model) where T : class;

        #endregion

        #region 03-删除
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="model">需要删除的实体</param>
        void DelNo<T>(T model) where T : class;

        #endregion

        #region 04-根据条件删除
        /// <summary>
        /// 条件删除
        /// </summary>
        /// <param name="delWhere">需要删除的条件</param>
        void DelByNo<T>(Expression<Func<T, bool>> delWhere) where T : class;

        #endregion

        #region 05-修改
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model">修改后的实体</param>
        void ModifyNo<T>(T model) where T : class;

        #endregion


        //3. EF调用sql语句

        #region 01-执行增加,删除,修改操作(或调用存储过程)
        /// <summary>
        /// 执行增加,删除,修改操作(或调用存储过程)
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        int ExecuteSql(string sql, params SqlParameter[] pars);


        #endregion

        #region 02-执行查询操作
        /// <summary>
        /// 执行查询操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        List<T> ExecuteQuery<T>(string sql, bool isTrack = true, params SqlParameter[] pars) where T : class;

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
        List<T> ExecuteQueryWhere<T>(string sql, Expression<Func<T, bool>> whereLambda, bool isTrack = true, params SqlParameter[] pars) where T : class;

        #endregion



        /****************************************下面进行方法的封装（异步）***********************************************/
        //1. 直接提交数据库

        #region 01-新增
        Task<int> AddAsync<T>(T model) where T : class;

        #endregion

        #region 02-删除
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="model">需要删除的实体</param>
        /// <returns></returns>
        Task<int> DelAsync<T>(T model) where T : class;

        #endregion

        #region 03-根据条件删除(支持批量删除)
        /// <summary>
        /// 根据条件删除(支持批量删除)
        /// </summary>
        /// <param name="delWhere">传入Lambda表达式(生成表达式目录树)</param>
        /// <returns></returns>
        Task<int> DelByAsync<T>(Expression<Func<T, bool>> delWhere) where T : class;

        #endregion

        #region 04-单实体修改
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model">修改后的实体</param>
        /// <returns></returns>
        Task<int> ModifyAsync<T>(T model) where T : class;

        #endregion

        #region 05-批量修改（非lambda）
        /// <summary>
        /// 批量修改（非lambda）
        /// </summary>
        /// <param name="model">要修改实体中 修改后的属性 </param>
        /// <param name="whereLambda">查询实体的条件</param>
        /// <param name="proNames">lambda的形式表示要修改的实体属性名</param>
        /// <returns></returns>
        Task<int> ModifyByAsync<T>(T model, Expression<Func<T, bool>> whereLambda, params string[] proNames) where T : class;

        #endregion

        #region 06-根据条件查询
        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="whereLambda">查询条件(lambda表达式的形式生成表达式目录树)</param>
        ///  <param name="isTrack">是否跟踪状态，默认是跟踪的</param>
        /// <returns></returns>
        Task<List<T>> GetListByAsync<T>(Expression<Func<T, bool>> whereLambda, bool isTrack = true) where T : class;

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
        Task<List<T>> GetListByAsync<T, Tkey>(Expression<Func<T, bool>> whereLambda, Expression<Func<T, Tkey>> orderLambda, bool isAsc = true, bool isTrack = true) where T : class;

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
        Task<List<T>> GetPageListAsync<T, Tkey>(int pageIndex, int pageSize, Expression<Func<T, bool>> whereLambda, Expression<Func<T, Tkey>> orderLambda, bool isAsc = true, bool isTrack = true) where T : class;

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
        Task<List<T>> GetPageListByNameAsync<T>(int pageIndex, int pageSize, Expression<Func<T, bool>> whereLambda, string sortName, SortEnum sortDirection, bool isTrack = true) where T : class;

        #endregion




        //2. SaveChange剥离出来，处理事务

        #region 01-批量处理SaveChange()
        /// <summary>
        /// 事务批量处理
        /// </summary>
        /// <returns></returns>
        Task<int> SaveChangeAsync();

        #endregion

        #region 02-新增
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model">需要新增的实体</param>
        Task<EntityEntry<T>> AddNoAsync<T>(T model) where T : class;

        #endregion

        #region 03-根据条件删除
        /// <summary>
        /// 条件删除
        /// </summary>
        /// <param name="delWhere">需要删除的条件</param>
        Task DelByNoAsync<T>(Expression<Func<T, bool>> delWhere) where T : class;

        #endregion


        //3. EF调用sql语句

        #region 01-执行增加,删除,修改操作(或调用存储过程)
        /// <summary>
        /// 执行增加,删除,修改操作(或调用存储过程)
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        Task<int> ExecuteSqlAsync(string sql, params SqlParameter[] pars);


        #endregion


    }
}

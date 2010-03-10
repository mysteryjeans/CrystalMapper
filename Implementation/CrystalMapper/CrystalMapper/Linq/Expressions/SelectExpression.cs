﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CrystalMapper.Lang;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace CrystalMapper.Linq.Expressions
{
    internal class SelectExpression : SourceExpression
    {
        private List<SortExpression> sortExpressions = new List<SortExpression>();

        public DistinctExpression Distinct { get; private set; }

        public AggregateExpression Aggregate { get; private set; }

        public TakeExpression Take { get; private set; }

        public SourceExpression From { get; private set; }

        public WhereExpression Where { get; private set; }

        public ProjectionExpression Projection { get; private set; }

        public bool UseDefault
        {
            get { return this.Take != null && this.Take.UseDefault; }
        }

        public Type ReturnType
        {
            get
            {
                if (this.Aggregate != null)
                    return this.Aggregate.Type;

                return this.Projection.Type;
            }
        }

        public ReadOnlyCollection<SortExpression> SortExpressions { get { return this.sortExpressions.AsReadOnly(); } }

        public SelectExpression(string alias, DbExpression source, ProjectionExpression projection)
            : base(alias, DbExpressionType.Select, projection.Type)
        {
            if (projection == null)
                throw new ArgumentNullException("projection");

            this.Projection = projection;

            do
            {
                switch (source.DbNodeType)
                {
                    case DbExpressionType.Where:
                        WhereExpression whereExpression = source as WhereExpression;
                        if (this.Where != null)
                        {
                            this.Where = new WhereExpression(whereExpression.Source,
                                                                                    new DbBinaryExpression(this.Where.BinaryExpression,
                                                                                                            whereExpression.BinaryExpression,
                                                                                                            ExpressionType.And,
                                                                                                            typeof(bool)));
                        }
                        else
                            this.Where = whereExpression;

                        source = whereExpression.Source;
                        break;
                    case DbExpressionType.Take:
                        this.Take = source as TakeExpression;
                        source = this.Take.Source;
                        break;
                    case DbExpressionType.Distinct:
                        this.Distinct = source as DistinctExpression;
                        source = this.Distinct.Source;
                        break;
                    case DbExpressionType.OrderBy:
                    case DbExpressionType.ThenBy:
                        SortExpression sortExp = (SortExpression)source;
                        this.sortExpressions.Insert(0, sortExp);
                        source = sortExp.Source;
                        break;
                    case DbExpressionType.Count:
                    case DbExpressionType.LongCount:
                    case DbExpressionType.Sum:
                    case DbExpressionType.Min:
                    case DbExpressionType.Max:
                    case DbExpressionType.Average:
                        this.Aggregate = source as AggregateExpression;
                        source = this.Aggregate.Source;
                        break;
                    case DbExpressionType.Select:
                    case DbExpressionType.Table:
                    case DbExpressionType.MultiSource:
                        this.From = source as SourceExpression;
                        break;
                }
            } while (this.From == null);

            this.AssignAlias();
        }

        public override string GetAlias(Type type)
        {
            return this.ReturnType == type ? this.Alias : (string.IsNullOrEmpty(this.From.GetAlias(type)) ? null : this.Alias);
        }

        public override void WriteQuery(SqlLang sqlLang, QueryWriter queryWriter)
        {
            queryWriter.Write("(SELECT ");

            if (this.Distinct != null)
                this.Distinct.WriteQuery(sqlLang, queryWriter);

            if (sqlLang.SqlLangType == SqlLangType.TSql && this.Take != null)
                this.Take.WriteQuery(sqlLang, queryWriter);

            if (this.Aggregate == null)
                this.Projection.WriteQuery(sqlLang, queryWriter);
            else
                this.Aggregate.WriteQuery(sqlLang, queryWriter);

            queryWriter.WriteLine().Write("FROM ");

            queryWriter.Indent += 1;
            queryWriter.Write(queryWriter.Indentation);
            this.From.WriteQuery(sqlLang, queryWriter);
            queryWriter.Indent -= 1;


            if (sqlLang.SqlLangType == SqlLangType.PSql && this.Take != null)
            {
                queryWriter.WriteLine().Write("WHERE ");

                if (this.Where != null)
                {
                    queryWriter.Write("(");
                    this.Where.WriteQuery(sqlLang, queryWriter);
                    queryWriter.Write(" AND ");
                    this.Take.WriteQuery(sqlLang, queryWriter);
                    queryWriter.Write(")");
                }
                else
                    this.Take.WriteQuery(sqlLang, queryWriter);
            }
            else if (this.Where != null)
            {
                queryWriter.WriteLine().Write("WHERE ");
                this.Where.WriteQuery(sqlLang, queryWriter);
            }


            if (this.Take != null && this.Take.IsReverse)
            {
                if (this.sortExpressions.Count == 0)
                    throw new InvalidOperationException("Query must specify order by clause if last record need to be fatch");

                foreach (SortExpression sortExpression in this.sortExpressions)
                    sortExpression.ReverseSortDirection();
            }

            bool isFirstOrderBy = true;
            foreach (var orderByExp in this.sortExpressions)
            {
                if (isFirstOrderBy)
                {
                    queryWriter.WriteLine().Write("ORDER BY ");
                    isFirstOrderBy = false;
                }
                else
                    queryWriter.Write(", ");

                orderByExp.WriteQuery(sqlLang, queryWriter);
            }

            if (sqlLang.SqlLangType == SqlLangType.Sqlite && this.Take != null)
            {
                queryWriter.WriteLine();
                this.Take.WriteQuery(sqlLang, queryWriter);
            }

            queryWriter.Write(") AS ").Write(Alias);
        }

        private void AssignAlias()
        {
            var dbMembers = this.Projection.GetNodes().Where(e => e.DbNodeType == DbExpressionType.Member).Select(e => (DbMemberExpression)e).ToList();

            dbMembers.ForEach(e => e.TableAlias = this.From.GetAlias(e.MemberMetadata.Member.DeclaringType));

            if (this.Where != null)
            {
                dbMembers = this.Where.GetNodes().Where(e => e.DbNodeType == DbExpressionType.Member).Select(e => (DbMemberExpression)e).ToList();

                dbMembers.ForEach(e => e.TableAlias = this.From.GetAlias(e.MemberMetadata.Member.DeclaringType));
            }

            foreach (SortExpression sortExpression in this.SortExpressions)
            {
                dbMembers = sortExpression.GetNodes().Where(e => e.DbNodeType == DbExpressionType.Member).Select(e => (DbMemberExpression)e).ToList();

                dbMembers.ForEach(e => e.TableAlias = this.From.GetAlias(e.MemberMetadata.Member.DeclaringType));
            }
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluidCurrentModelling2.DataStructures;
using Microsoft.Research.Science.Data;

namespace FluidCurrentModelling2.ModellingMath
{
	public class FluidCurrentSolver
	{
		LayerData prevData;
		NumericalParameters modellingParams;
		DataSet dataSet;
		LayerSolver solver;
		Variable u, v, w, T, div, time;

		public FluidCurrentSolver(NumericalParameters modellingParams)
		{
			this.modellingParams = modellingParams;

			prevData = new LayerData(modellingParams.Nx, modellingParams.Ny, modellingParams.Nz);

			//Задаем начальные условия. Жидкость покоится, температура постоянна по всему объему
			prevData.U.InitializeData(0);
			prevData.V.InitializeData(0);
			prevData.W.InitializeData(0);
			prevData.T.InitializeData(1.0);
			prevData.Div.InitializeData(0);
		}

		public DataSet DataSet
		{
			get { return dataSet; }
		}

		public void Init(string constructionString)
		{
			dataSet = DataSet.Create(constructionString);

			//Инициализируем DataSet
			Variable X = dataSet.AddVariable<double>("X", "x");
			Variable Y = dataSet.AddVariable<double>("Y", "y");
			Variable Z = dataSet.AddVariable<double>("Z", "z");
			time = dataSet.AddVariable<double>("Time", "t");

			u = dataSet.AddVariable<double>("U velocity", "x", "y", "z", "t");
			v = dataSet.AddVariable<double>("V velocity", "x", "y", "z", "t");
			w = dataSet.AddVariable<double>("W velocity", "x", "y", "z", "t");
			T = dataSet.AddVariable<double>("Temperature", "x", "y", "z", "t");
			div = dataSet.AddVariable<double>("Divergence", "x", "y", "z", "t");

			double[] wArr = new double[modellingParams.Nx];
			for (int i = 0; i < modellingParams.Nx; i++)
			{
				wArr[i] = i * modellingParams.Dx;
			}
			X.PutData(wArr);
			wArr = new double[modellingParams.Ny];
			for (int i = 0; i < modellingParams.Ny; i++)
			{
				wArr[i] = i * modellingParams.Dy;
			}
			Y.PutData(wArr);
			wArr = new double[modellingParams.Nz];
			for (int i = 0; i < modellingParams.Nz; i++)
			{
				wArr[i] = i * modellingParams.Dz;
			}
			Z.PutData(wArr);

			//Инициализируем рассчетный модуль для слоя начальными условиями
			solver = new LayerSolver(prevData, modellingParams);
			u.Append(prevData.U.ToArray(), "t");
			v.Append(prevData.V.ToArray(), "t");
			w.Append(prevData.W.ToArray(), "t");
			T.Append(prevData.T.ToArray(), "t");
			div.Append(prevData.Div.ToArray(), "t");

			time.PutData(new double[1] { 0 });
			dataSet.Commit();
		}

		public void PerformIteration(int i)
		{
			//Основной рассчет
			//for (int i = 1; i < modellingParams.Nt; i++)
			//{
			LayerData result = solver.Solve(true);
			//Кладем данные в DataSet
			u.Append(result.U.ToArray(), "t");
			v.Append(result.V.ToArray(), "t");
			w.Append(result.W.ToArray(), "t");
			T.Append(result.T.ToArray(), "t");
			div.Append(result.Div.ToArray(), "t");
			time.Append(new double[1] { (double)i / modellingParams.Nt });
			dataSet.Commit();
			//Переходим на следующий слой
			solver = new LayerSolver(prevData, result, modellingParams);
			prevData = result;

			double temp = 0;
			int count = result.Width * result.Height * result.Thickness;
			for (int ii = 1; ii < result.Width; ii++)
			{
				for (int jj = 1; jj < result.Height; jj++)
				{
					for (int kk = 1; kk < result.Thickness; kk++)
					{
						temp += result.Div[ii, jj, kk];
					}
				}
			}
			temp = temp / count * modellingParams.Dx * modellingParams.Dy * modellingParams.Dz;

			Console.WriteLine((double)i / modellingParams.Nt * 100 + "% Error = " + temp);
			//}
			//dataSet.Commit();
		}
	}
}

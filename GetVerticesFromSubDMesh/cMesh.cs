﻿#region System
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

#region Autodesk
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
#endregion


namespace GetVerticesFromSubDMesh
{
    class cMesh
    {
        //public static void SubDMeshTest()
        //{
        //    Document activeDoc = Application.DocumentManager.MdiActiveDocument;
        //    Database db = activeDoc.Database;
        //    Editor ed = activeDoc.Editor;

        //    TypedValue[] values = { new TypedValue((int)DxfCode.Start, "MESH") };

        //    SelectionFilter filter = new SelectionFilter(values);
        //    PromptSelectionResult psr = ed.SelectAll(filter);
        //    SelectionSet ss = psr.Value;
        //    if (ss == null)
        //        return;

        //    using (Transaction trans = db.TransactionManager.StartTransaction())
        //    {
        //        for (int i = 0; i < ss.Count; ++i)
        //        {
        //            SubDMesh mesh = trans.GetObject(ss[i].ObjectId, OpenMode.ForRead) as SubDMesh;

        //            ed.WriteMessage(String.Format("\n Vertices : {0}", mesh.NumberOfVertices));
        //            ed.WriteMessage(String.Format("\n Edges    : {0}", mesh.NumberOfEdges));
        //            ed.WriteMessage(String.Format("\n Faces    : {0}", mesh.NumberOfFaces));

        //            // Get the Face information
        //            int[] faceArr = mesh.FaceArray.ToArray();
        //            int edges = 0;
        //            int fcount = 0;

        //            for (int x = 0; x < faceArr.Length; x = x + edges + 1)
        //            {
        //                ed.WriteMessage(String.Format("\n Face {0} : ", fcount++));

        //                edges = faceArr[x];
        //                for (int y = x + 1; y <= x + edges; y++)
        //                {
        //                    ed.WriteMessage(String.Format("\n\t Edge - {0}", faceArr[y]));
        //                }
        //            }

        //            // Get the Edge information
        //            int ecount = 0;
        //            int[] edgeArr = mesh.EdgeArray.ToArray();

        //            for (int x = 0; x < edgeArr.Length; x = x + 2)
        //            {
        //                ed.WriteMessage(String.Format("\n Edge {0} : ", ecount++));
        //                ed.WriteMessage(String.Format("\n Vertex - {0}", edgeArr[x]));
        //                ed.WriteMessage(String.Format("\n Vertex - {0}", edgeArr[x + 1]));
        //            }

        //            // Get the vertices information
        //            int vcount = 0;
        //            foreach (Point3d vertex in mesh.Vertices)
        //            {
        //                ed.WriteMessage(String.Format("\n Vertex {0} - {1} {2}", vcount++, vertex.X, vertex.Y));
        //            }
        //        }
        //        trans.Commit();
        //    }
        //}

        public static Point3dCollection GetSubDMeshVertices()
        {
            Document activeDoc = Application.DocumentManager.MdiActiveDocument;
            Database db = activeDoc.Database;
            Editor ed = activeDoc.Editor;

            TypedValue[] values = { new TypedValue((int)DxfCode.Start, "MESH") };

            SelectionFilter filter = new SelectionFilter(values);
            PromptSelectionResult psr = ed.SelectAll(filter);
            SelectionSet ss = psr.Value;
            if (ss == null)
                return null;

            Point3dCollection collPoints = new Point3dCollection();

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                for (int i = 0; i < ss.Count; ++i)
                {
                    SubDMesh mesh = trans.GetObject(ss[i].ObjectId, OpenMode.ForRead) as SubDMesh;
                    
                    
                    // Get the vertices information
                    int vcount = 0;
                    foreach (Point3d vertex in mesh.Vertices)
                    {
                        //ed.WriteMessage(String.Format("\n Vertex {0} - {1} {2}", vcount++, vertex.X, vertex.Y));
                        collPoints.Add(new Point3d(vertex.X, vertex.Y, vertex.Z));
                    }
                }
                trans.Commit();
            }
            return collPoints;
        }

        public static List<MeshDatas> GetMeshDatas()
        {
            Document activeDoc = Application.DocumentManager.MdiActiveDocument;
            Database db = activeDoc.Database;
            Editor ed = activeDoc.Editor;

            TypedValue[] values = { new TypedValue((int)DxfCode.Start, "MESH") };

            SelectionFilter filter = new SelectionFilter(values);
            PromptSelectionResult psr = ed.SelectAll(filter);
            SelectionSet ss = psr.Value;
            if (ss == null)
                return null;

            List<MeshDatas> datas = new List<MeshDatas>();

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                for (int i = 0; i < ss.Count; ++i)
                {
                    SubDMesh mesh = trans.GetObject(ss[i].ObjectId, OpenMode.ForRead) as SubDMesh;
                    Point3dCollection collPoints = new Point3dCollection();

                    // Get the vertices information
                    int vcount = 0;
                    foreach (Point3d vertex in mesh.Vertices)
                    {
                        //ed.WriteMessage(String.Format("\n Vertex {0} - {1} {2}", vcount++, vertex.X, vertex.Y));
                        collPoints.Add(new Point3d(vertex.X, vertex.Y, vertex.Z));
                    }
                    datas.Add(new MeshDatas { Mesh = mesh, Increment = i, Points = collPoints });

                }
                trans.Commit();
            }
            return datas;
        }

        //public static void GetMeshBoundary(Point3dCollection verticies)
        //{
        //    Document acDoc = Application.DocumentManager.MdiActiveDocument;
        //    Database acCurDb = acDoc.Database;

        //    // Start a transaction
        //    using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
        //    {
        //        // Open the Block table for read
        //        BlockTable acBlkTbl;
        //        acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

        //        // Open the Block table record Model space for write
        //        BlockTableRecord acBlkTblRec;
        //        acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

        //        // Create a polyline with two segments (3 points)
        //        using (Polyline acPoly = new Polyline())
        //        {
        //            int vnr = 0;
        //            int incr = 0;

        //            #region All verticies
        //            for (int i = 0; i < verticies.Count; i++)
        //            {
        //                acPoly.AddVertexAt(vnr, new Point2d(verticies[i].X, verticies[i].Y), 0, 0, 0);
        //                vnr++;
        //            }
        //            #endregion

        //            //#region Distance between verticies max 5meter
        //            //while (vnr < verticies.Count && 1+ incr < verticies.Count)
        //            //{
        //            //    if (cPoint.Distance(verticies[incr], verticies[1+ incr]) < 5) //Distance between verticies max 5meter
        //            //    {
        //            //        acPoly.AddVertexAt(vnr, new Point2d(verticies[incr].X, verticies[incr].Y), 0, 0, 0);
        //            //        //verticies.RemoveAt(incr);
        //            //        vnr++;
        //            //    }
        //            //    incr++;
        //            //}
        //            //#endregion

        //            // Add the new object to the block table record and the transaction
        //            acBlkTblRec.AppendEntity(acPoly);
        //            acTrans.AddNewlyCreatedDBObject(acPoly, true);
        //        }

        //        // Save the new object to the database
        //        acTrans.Commit();
        //    }
        //}
    }
}

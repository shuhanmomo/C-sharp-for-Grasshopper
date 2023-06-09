using System;
using System.Collections;
using System.Collections.Generic;

using Rhino;
using Rhino.Geometry;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;



/// <summary>
/// This class will be instantiated on demand by the Script component.
/// </summary>
public class Script_Instance : GH_ScriptInstance
{
#region Utility functions
  /// <summary>Print a String to the [Out] Parameter of the Script component.</summary>
  /// <param name="text">String to print.</param>
  private void Print(string text) { __out.Add(text); }
  /// <summary>Print a formatted String to the [Out] Parameter of the Script component.</summary>
  /// <param name="format">String format.</param>
  /// <param name="args">Formatting parameters.</param>
  private void Print(string format, params object[] args) { __out.Add(string.Format(format, args)); }
  /// <summary>Print useful information about an object instance to the [Out] Parameter of the Script component. </summary>
  /// <param name="obj">Object instance to parse.</param>
  private void Reflect(object obj) { __out.Add(GH_ScriptComponentUtilities.ReflectType_CS(obj)); }
  /// <summary>Print the signatures of all the overloads of a specific method to the [Out] Parameter of the Script component. </summary>
  /// <param name="obj">Object instance to parse.</param>
  private void Reflect(object obj, string method_name) { __out.Add(GH_ScriptComponentUtilities.ReflectType_CS(obj, method_name)); }
#endregion

#region Members
  /// <summary>Gets the current Rhino document.</summary>
  private RhinoDoc RhinoDocument;
  /// <summary>Gets the Grasshopper document that owns this script.</summary>
  private GH_Document GrasshopperDocument;
  /// <summary>Gets the Grasshopper script component that owns this script.</summary>
  private IGH_Component Component; 
  /// <summary>
  /// Gets the current iteration count. The first call to RunScript() is associated with Iteration==0.
  /// Any subsequent call within the same solution will increment the Iteration count.
  /// </summary>
  private int Iteration;
#endregion

  /// <summary>
  /// This procedure contains the user code. Input parameters are provided as regular arguments, 
  /// Output parameters as ref arguments. You don't have to assign output parameters, 
  /// they will have a default value.
  /// </summary>
  private void RunScript(Surface srf, int uCount, int vCount, double dist, ref object A, ref object B)
  {
            srf.SetDomain(0, new Interval(0.0, 1.0));
    srf.SetDomain(1, new Interval(0.0, 1.0));
    double du = 1.0 / (double) (uCount - 1.0);
    double dv = 1.0 / (double) (vCount - 1.0);

    List<Curve> crvs = new List<Curve> ();
    List<Point3d> offsetpts = new List<Point3d> ();
    List<Line> lines = new List<Line> ();


    int i,j;
    for ( i = 0; i < uCount;i++){
      for ( j = 0; j < vCount;j++){

        crvs.Add(srf.IsoCurve(0, dv * j));
        crvs.Add(srf.IsoCurve(1, du * i));
        Vector3d n = srf.NormalAt(du * i, dv * j);
        if(i !=uCount-1){
          Point3d midpt = srf.PointAt((du * i + du * (i + 1)) / 2, dv * j) + n * dist;
          offsetpts.Add(midpt);}
       
      }}
    Surface offsetSrf = NurbsSurface.CreateThroughPoints(offsetpts, uCount-1, vCount, 3, 3, false, false);
    offsetSrf.SetDomain(0, new Interval(0.0, 1.0));
    offsetSrf.SetDomain(1, new Interval(0.0, 1.0));
    double u1;
    double u2;
    double v1;
    double v2;
    for ( i = 0; i < uCount;i++){
      for ( j = 0; j < vCount;j++){
        Vector3d n = srf.NormalAt(du * i, dv * j);
        Point3d offsetpt = srf.PointAt(du * i, dv * j) + n * dist;

        offsetSrf.ClosestPoint(offsetpt, out u1, out v1);
        Curve offsetcrv_V = offsetSrf.IsoCurve(0, v1);
        crvs.Add(offsetcrv_V);
        if(i != uCount - 1){
          Point3d pt1 = srf.PointAt(du * i, dv * j);
          Point3d pt2 = srf.PointAt(du * (i + 1), dv * j);
          Point3d midpt1 = srf.PointAt((du * i + du * (i + 1)) / 2,dv*j);
          Point3d midpt2 = midpt1 + n * dist;
          offsetSrf.ClosestPoint(midpt2, out u2, out v2);
          Point3d midpt3 = offsetSrf.PointAt(u2, v2);
          crvs.Add(offsetSrf.IsoCurve(1,u2));
          lines.Add(new Line(pt1, midpt3));
          lines.Add(new Line(pt2, midpt3));

        }








      }}

    A = crvs;
    B = lines;

  }

  // <Custom additional code> 
  
  // </Custom additional code> 

  private List<string> __err = new List<string>(); //Do not modify this list directly.
  private List<string> __out = new List<string>(); //Do not modify this list directly.
  private RhinoDoc doc = RhinoDoc.ActiveDoc;       //Legacy field.
  private IGH_ActiveObject owner;                  //Legacy field.
  private int runCount;                            //Legacy field.
  
  public override void InvokeRunScript(IGH_Component owner, object rhinoDocument, int iteration, List<object> inputs, IGH_DataAccess DA)
  {
    //Prepare for a new run...
    //1. Reset lists
    this.__out.Clear();
    this.__err.Clear();

    this.Component = owner;
    this.Iteration = iteration;
    this.GrasshopperDocument = owner.OnPingDocument();
    this.RhinoDocument = rhinoDocument as Rhino.RhinoDoc;

    this.owner = this.Component;
    this.runCount = this.Iteration;
    this. doc = this.RhinoDocument;

    //2. Assign input parameters
        Surface srf = default(Surface);
    if (inputs[0] != null)
    {
      srf = (Surface)(inputs[0]);
    }

    int uCount = default(int);
    if (inputs[1] != null)
    {
      uCount = (int)(inputs[1]);
    }

    int vCount = default(int);
    if (inputs[2] != null)
    {
      vCount = (int)(inputs[2]);
    }

    double dist = default(double);
    if (inputs[3] != null)
    {
      dist = (double)(inputs[3]);
    }



    //3. Declare output parameters
      object A = null;
  object B = null;


    //4. Invoke RunScript
    RunScript(srf, uCount, vCount, dist, ref A, ref B);
      
    try
    {
      //5. Assign output parameters to component...
            if (A != null)
      {
        if (GH_Format.TreatAsCollection(A))
        {
          IEnumerable __enum_A = (IEnumerable)(A);
          DA.SetDataList(1, __enum_A);
        }
        else
        {
          if (A is Grasshopper.Kernel.Data.IGH_DataTree)
          {
            //merge tree
            DA.SetDataTree(1, (Grasshopper.Kernel.Data.IGH_DataTree)(A));
          }
          else
          {
            //assign direct
            DA.SetData(1, A);
          }
        }
      }
      else
      {
        DA.SetData(1, null);
      }
      if (B != null)
      {
        if (GH_Format.TreatAsCollection(B))
        {
          IEnumerable __enum_B = (IEnumerable)(B);
          DA.SetDataList(2, __enum_B);
        }
        else
        {
          if (B is Grasshopper.Kernel.Data.IGH_DataTree)
          {
            //merge tree
            DA.SetDataTree(2, (Grasshopper.Kernel.Data.IGH_DataTree)(B));
          }
          else
          {
            //assign direct
            DA.SetData(2, B);
          }
        }
      }
      else
      {
        DA.SetData(2, null);
      }

    }
    catch (Exception ex)
    {
      this.__err.Add(string.Format("Script exception: {0}", ex.Message));
    }
    finally
    {
      //Add errors and messages... 
      if (owner.Params.Output.Count > 0)
      {
        if (owner.Params.Output[0] is Grasshopper.Kernel.Parameters.Param_String)
        {
          List<string> __errors_plus_messages = new List<string>();
          if (this.__err != null) { __errors_plus_messages.AddRange(this.__err); }
          if (this.__out != null) { __errors_plus_messages.AddRange(this.__out); }
          if (__errors_plus_messages.Count > 0) 
            DA.SetDataList(0, __errors_plus_messages);
        }
      }
    }
  }
}
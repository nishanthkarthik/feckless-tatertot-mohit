//------------------------------------------------------------------------------
// <auto-generated />
//
// This file was automatically generated by SWIG (http://www.swig.org).
// Version 3.0.5
//
// Do not make changes to this file unless you know what you are doing--modify
// the SWIG interface file instead.
//------------------------------------------------------------------------------


using System;
using System.Runtime.InteropServices;

public class SsatSimpleState : SS
{
    public enum SsatSimpleStateEnum
    {
        SSAT_MAX_NOT_SET = 0,
        SSAT_MAX_DOESNT_EXIST,
        SSAT_MAX_DOES_EXIST
    }

    private HandleRef swigCPtr;

    internal SsatSimpleState(IntPtr cPtr, bool cMemoryOwn)
        : base(CoolPropPINVOKE.SsatSimpleState_SWIGUpcast(cPtr), cMemoryOwn)
    {
        swigCPtr = new HandleRef(this, cPtr);
    }

    public SsatSimpleState() : this(CoolPropPINVOKE.new_SsatSimpleState(), true)
    {
        if (CoolPropPINVOKE.SWIGPendingException.Pending) throw CoolPropPINVOKE.SWIGPendingException.Retrieve();
    }

    public SsatSimpleStateEnum exists
    {
        set
        {
            CoolPropPINVOKE.SsatSimpleState_exists_set(swigCPtr, (int) value);
            if (CoolPropPINVOKE.SWIGPendingException.Pending) throw CoolPropPINVOKE.SWIGPendingException.Retrieve();
        }
        get
        {
            var ret = (SsatSimpleStateEnum) CoolPropPINVOKE.SsatSimpleState_exists_get(swigCPtr);
            if (CoolPropPINVOKE.SWIGPendingException.Pending) throw CoolPropPINVOKE.SWIGPendingException.Retrieve();
            return ret;
        }
    }

    internal static HandleRef getCPtr(SsatSimpleState obj)
    {
        return (obj == null) ? new HandleRef(null, IntPtr.Zero) : obj.swigCPtr;
    }

    ~SsatSimpleState()
    {
        Dispose();
    }

    public override void Dispose()
    {
        lock (this)
        {
            if (swigCPtr.Handle != IntPtr.Zero)
            {
                if (swigCMemOwn)
                {
                    swigCMemOwn = false;
                    CoolPropPINVOKE.delete_SsatSimpleState(swigCPtr);
                }
                swigCPtr = new HandleRef(null, IntPtr.Zero);
            }
            GC.SuppressFinalize(this);
            base.Dispose();
        }
    }
}
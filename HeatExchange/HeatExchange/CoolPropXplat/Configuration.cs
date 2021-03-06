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

public class Configuration : IDisposable
{
    protected bool swigCMemOwn;
    private HandleRef swigCPtr;

    internal Configuration(IntPtr cPtr, bool cMemoryOwn)
    {
        swigCMemOwn = cMemoryOwn;
        swigCPtr = new HandleRef(this, cPtr);
    }

    public Configuration() : this(CoolPropPINVOKE.new_Configuration(), true)
    {
        if (CoolPropPINVOKE.SWIGPendingException.Pending) throw CoolPropPINVOKE.SWIGPendingException.Retrieve();
    }

    public virtual void Dispose()
    {
        lock (this)
        {
            if (swigCPtr.Handle != IntPtr.Zero)
            {
                if (swigCMemOwn)
                {
                    swigCMemOwn = false;
                    CoolPropPINVOKE.delete_Configuration(swigCPtr);
                }
                swigCPtr = new HandleRef(null, IntPtr.Zero);
            }
            GC.SuppressFinalize(this);
        }
    }

    internal static HandleRef getCPtr(Configuration obj)
    {
        return (obj == null) ? new HandleRef(null, IntPtr.Zero) : obj.swigCPtr;
    }

    ~Configuration()
    {
        Dispose();
    }

    public ConfigurationItem get_item(configuration_keys key)
    {
        var ret = new ConfigurationItem(CoolPropPINVOKE.Configuration_get_item(swigCPtr, (int) key), false);
        if (CoolPropPINVOKE.SWIGPendingException.Pending) throw CoolPropPINVOKE.SWIGPendingException.Retrieve();
        return ret;
    }

    public void add_item(ConfigurationItem item)
    {
        CoolPropPINVOKE.Configuration_add_item(swigCPtr, ConfigurationItem.getCPtr(item));
        if (CoolPropPINVOKE.SWIGPendingException.Pending) throw CoolPropPINVOKE.SWIGPendingException.Retrieve();
    }

    public SWIGTYPE_p_std__mapT_configuration_keys_CoolProp__ConfigurationItem_t get_items()
    {
        var ret =
            new SWIGTYPE_p_std__mapT_configuration_keys_CoolProp__ConfigurationItem_t(
                CoolPropPINVOKE.Configuration_get_items(swigCPtr), false);
        if (CoolPropPINVOKE.SWIGPendingException.Pending) throw CoolPropPINVOKE.SWIGPendingException.Retrieve();
        return ret;
    }

    public void set_defaults()
    {
        CoolPropPINVOKE.Configuration_set_defaults(swigCPtr);
        if (CoolPropPINVOKE.SWIGPendingException.Pending) throw CoolPropPINVOKE.SWIGPendingException.Retrieve();
    }
}
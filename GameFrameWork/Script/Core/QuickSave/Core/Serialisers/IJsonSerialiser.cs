////////////////////////////////////////////////////////////////////////////////
//  
// @module Quick Save for Unity3D 
// @author Michael Clayton
// @support clayton.inds+support@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////

namespace FastFrameWork.Core.Serialisers
{
    public interface IJsonSerialiser
    {
        string Serialise<T>(T value);
        T Deserialise<T>(string json);
    }
}
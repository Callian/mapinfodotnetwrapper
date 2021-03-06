﻿using MapInfo.Wrapper.Core.Exceptions;
using Mapinfo.Wrapper.Core.Extensions;
using Mapinfo.Wrapper.Exceptions;
using System;
using Mapinfo.Wrapper.Mapinfo;

namespace Mapinfo.Wrapper.MapbasicOperations
{
    internal class VariableFactory : IVariableFactory
    {
        private readonly MapinfoSession misession;

        public VariableFactory(MapinfoSession MISession) 
        {
            this.misession = MISession;
        }

        public Variable DefineVariableWithGUID(VariableType type)
        {
            Guid id = Guid.NewGuid();
            string striped = id.ToString().Replace("-","");
            int index = 0;
            foreach (char c in striped) {
                if (Char.IsLetter(c)) {
                    index = striped.IndexOf(c);
                    break;      
                }
            }
            string variablename = striped.Substring(index);

            string typename = Enum.GetName(typeof(VariableType), type);
            this.misession.Do("Dim {0} as {1}".FormatWith(variablename, typename));
            
            Variable variable = new Variable(variablename,type,this.misession);

            return variable;
        }

        public Variable DefineVariable(string name, VariableType type)
        {
            try
            {
                string typename = Enum.GetName(typeof(VariableType), type);
                this.misession.Do("Dim {0} as {1}".FormatWith(name, typename));

                return new Variable(name, type, this.misession);
            }
            catch (MapinfoException mapinfoex)
            {
                if (mapinfoex.MapinfoErrorCode == 579)
                {
                    throw new MapbasicVariableException("Variable {0} already defined.".FormatWith(name), mapinfoex);
                }
                throw;
            }
        }
    }
}

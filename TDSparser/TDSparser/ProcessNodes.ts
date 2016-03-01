import {readFileSync, writeFileSync} from "fs";
import * as _ from "lodash";

export function GenerateTypeName(typeRef: TypeRef): string {
    switch (typeRef._t) {
        case ts.SyntaxKind.VoidKeyword:
            return "void";
        case ts.SyntaxKind.AnyKeyword:
            return "object";
        case ts.SyntaxKind.StringKeyword:
            return "string";
        case ts.SyntaxKind.NumberKeyword:
            return "Number";
        case ts.SyntaxKind.BooleanKeyword:
            return "bool";
        case ts.SyntaxKind.TypeReference:
            return typeRef.name;
        default:
            return "object";
    }
}

function ExpandUnionType(t: TypeRef): TypeRef[] {
    if (t._t == ts.SyntaxKind.UnionType) {
        return (<UnionTypeRef>t).types;
    } else if (t._t == ts.SyntaxKind.TypeLiteral) {
        t = CreateAnonymousType(<TypeLitDecl><any>t);
    }

    return [t];
} 

class Class {
    name: string;
    isSecondary: boolean;
    staticIFace: TypeDecl;
    instanceIFace: TypeDecl;
    isIFace: boolean;
    isDelegate: boolean;

    constructors: Method[];
    selfMethods: Method[];
    staticSelfMethods: Method[];
    instanceMethods: Method[];
    instanceProperties: Property[];
    staticMethods: Method[];
    staticProperties: Property[];

    constructor(
        name: string,
        isSecondary: boolean,
        staticIFace: TypeDecl,
        instanceIFace: TypeDecl,
        isIFace?: boolean) {

        this.name = name;
        this.isSecondary = isSecondary,
        this.staticIFace = staticIFace,
        this.instanceIFace = instanceIFace;
        this.isIFace = !!isIFace;

        this.constructors = [];
        this.selfMethods = [];
        this.staticSelfMethods = [];

        var tmp = { methods: <Method[]>[], properties: <Property[]>[] };

        Class.ExtractMethodsAndProperties(instanceIFace, tmp);
        this.instanceMethods = tmp.methods;
        this.instanceProperties = tmp.properties;

        tmp = { methods: <Method[]>[], properties: <Property[]>[] };
        Class.ExtractMethodsAndProperties(staticIFace, tmp);
        this.staticMethods = tmp.methods;
        this.staticProperties = tmp.properties;

        if (staticIFace) {
            for (var iM = 0; iM < staticIFace.members.length; ++iM) {
                var member = staticIFace.members[iM];
                if (member._t == ts.SyntaxKind.ConstructSignature) {
                    this.constructors = this.constructors.concat(
                        Method.CreateMethods(<CoreMethodSig>member));
                } else if (member._t == ts.SyntaxKind.CallSignature) {
                    this.staticSelfMethods = this.staticSelfMethods.concat(
                        Method.CreateMethods(<CoreMethodSig>member, true));
                }
            }
        } else {
            if (instanceIFace.members)
            for (var iM = 0; iM < instanceIFace.members.length; ++iM) {
                var member = instanceIFace.members[iM];
                if (member._t == ts.SyntaxKind.Constructor) {
                    this.constructors = this.constructors.concat(
                        Method.CreateMethods(<CoreMethodSig>member));
                } else if (member._t == ts.SyntaxKind.CallSignature) {
                    this.selfMethods = this.selfMethods.concat(
                        Method.CreateMethods(<CoreMethodSig>member, true));
                }
            }
        }

        if (this.selfMethods.length == 1
            && this.constructors.length == 0
            && this.instanceMethods.length == 0
            && this.instanceProperties.length == 0
            && staticIFace == null) {
            this.isIFace = false;
            this.isDelegate = true;
            this.selfMethods[0].name = this.name;
            this.selfMethods[0].originalName = this.name;
        }
    }

    private static generator: (arg: any) => string;
    private static staticMembersGenerator: (arg: any) => string;
    private static instanceMembersGenerator: (arg: any) => string;

    private static ExtractMethodsAndProperties(
        t: TypeDecl,
        lists: { methods: Method[], properties: Property[] }) {

        if (!t) {
            return;
        }

        if (t.members)
        for (var iMember = 0; iMember < t.members.length; ++iMember) {
            var member = t.members[iMember];

            if (member._t == ts.SyntaxKind.MethodDeclaration) {
                lists.methods = lists.methods.concat(
                    Method.CreateMethods(<CoreMemberMethodDecl>member));
            } else if (member._t == ts.SyntaxKind.PropertySignature) {
                lists.properties = lists.properties.concat(
                    Property.CreateProperties(<PropertyMemberDecl>member));
            }
        }
    }

    public generateCsFile(): string {
        Class.GenerateTemplate();
        var rv = Class.generator({ obj: this });
        return rv;
    }

    private static GenerateTemplate() {
        if (Class.generator) {
            return;
        }

        var tmp = _.template(readFileSync("./classTemplate.tmpl").toString());
        writeFileSync("./classTemplate.js",
            "var ts = require('typescript'); exports.classTemplate = " + tmp.source);

        var tmplate = require("./classTemplate");
        Class.generator = <(arg: any) => string>tmplate.classTemplate;
    }
}

class Method {
    public name: string;
    public originalName: string;
    public parameters: ParameterDecl[];
    public typeParameters: TypeParam[];
    public returnType: TypeRef;
    public undefinable: boolean;

    /// longest: this is really passed in for making sure that we do not create many 
    ///         delegates for optional parameters.
    public static CreateMethods(method: CoreMethodSig, longest: boolean = false): Method[]{
        var parameterLists: ParameterDecl[][] = [];
        var unionTypeExpandedParamList: ParameterDecl[][] = [];
        var returnTypes: TypeRef[] = null;
        if (method._t != ts.SyntaxKind.Constructor) {
            returnTypes = ExpandUnionType(method.returnType);
        }
        else {
            returnTypes = [];
        }

        var paramList: ParameterDecl[] = [];
        // expand the parameterList with optional so that we create
        // method with and without optional parameter.
        for (var iParam = 0; iParam < method.parameters.length; ++iParam) {
            var param = method.parameters[iParam];
            if (param.optional) {
                parameterLists.push(paramList.concat([]));
            }

            paramList.push({
                _t: param._t,
                name: param.name,
                argType: param.argType,
                dotdotdot: !!param.dotdotdot
            });
        }

        if (longest) {
            paramList = [];
        }

        parameterLists.push(paramList);

        for (var iParamList = 0; iParamList < parameterLists.length; ++iParamList) {
            Method.ExpandUnionParameters(parameterLists[iParamList], unionTypeExpandedParamList);
        }

        var rv: Method[] = [];
        for (var iR = 0; iR < returnTypes.length; ++iR) {
            for (var iP = 0; iP < unionTypeExpandedParamList.length; ++iP) {
                var rvMethod = new Method();
                rvMethod.returnType = returnTypes[iR];
                rvMethod.parameters = unionTypeExpandedParamList[iP];

                if (method._t == ts.SyntaxKind.MethodDeclaration) {
                    var md: CoreMemberMethodDecl = <CoreMemberMethodDecl>method;
                    rvMethod.typeParameters = md.typeParameters;
                    rvMethod.undefinable = md.undefinable;
                    rvMethod.originalName = md.name;

                    if (iR > 0) {
                        rvMethod.name = md.name + iR;
                    } else {
                        rvMethod.name = md.name;
                    }

                    rvMethod.name = CapitalizeForDotNet(rvMethod.name);
                }

                rv.push(rvMethod);
            }
        }

        return rv;
    }

    private static ExpandUnionParameters(paramList: ParameterDecl[], paramLists: ParameterDecl[][]){
        function Looper(
            paramList: ParameterDecl[],
            outParamList: ParameterDecl[],
            finalParamsLists: ParameterDecl[][],
            idx: number) {
            if (idx == paramList.length) {
                finalParamsLists.push(paramList.concat([]));
                return;
            }

            var param = paramList[idx];
            var types = ExpandUnionType(<TypeRef>param.argType);
            for (var iType = 0; iType < types.length; ++iType) {
                outParamList[idx] = {
                    _t: param._t,
                    name: param.name,
                    argType: types[iType],
                    dotdotdot: !!param.dotdotdot
                };

                Looper(paramList, outParamList, finalParamsLists, idx + 1);
            }
        }

        Looper(paramList, [], paramLists, 0);
    }
}

class Property {
    public name: string;
    public originalName: string;
    public readonly: boolean;
    public undefinable: boolean;
    public propertyType: TypeRef;

    public static CreateProperties(property: PropertyMemberDecl): Property[] {

        var returnTypes = ExpandUnionType(property.returnType);
        var rv : Property[] = [];
        for (var iR = 0; iR < returnTypes.length; ++iR) {
            var p = new Property();
            p.propertyType = returnTypes[iR];
            p.originalName = property.name;
            if (iR > 0) {
                p.name = property.name + iR;
            } else {
                p.name = property.name;
            }

            p.name = CapitalizeForDotNet(p.name);
            p.undefinable = !!property.undefinable;
            p.readonly = p.originalName.toLowerCase() == p.originalName
                && (p.propertyType._t == ts.SyntaxKind.NumberKeyword
                    || p.propertyType._t == ts.SyntaxKind.BooleanKeyword
                    || p.propertyType._t == ts.SyntaxKind.StringKeyword);
            rv.push(p);
        }

        return rv;
    }
}

function CapitalizeForDotNet(name: string): string {
    if (name[0].toLowerCase() == name[0]) {
        return name[0].toUpperCase() + name.substr(1);
    }

    return name;
}

interface VarIfaces {
    name: string;
    iface: TypeDecl;
}

export var instanceIFaces: TypeDecl[] = [];
export var classIFaces: VarIfaces[] = [];

interface TypeNameMap {
    [key: string]: TypeDecl;
}

export var typeNames: TypeNameMap = {};
export var referencedObjects: Object = {};
export var classes: Class[] = [];
var processed: { [idx: string]: ParsedNode } = {};

export function ProcessPass1(nodes: ParsedNode[], moduleName: string = null) {
    for (var i = 0; i < nodes.length; ++i) {
        var node = nodes[i];
        switch (node._t) {
            case ts.SyntaxKind.ClassDeclaration:
                var typeDecl = <TypeDecl>node;
                instanceIFaces.push(typeDecl);
                classIFaces.push({
                    name: typeDecl.name,
                    iface: typeDecl
                });

                typeNames[typeDecl.name] = typeDecl;
                break;
            case ts.SyntaxKind.InterfaceDeclaration:
                var typeDecl = <TypeDecl>node;
                typeNames[typeDecl.name] = typeDecl;
                break;
        }
    }
}


interface StaticIfaceCombo {
    staticTypeName: string,
    instanceTypeName: string,
    staticIface: TypeDecl,
    instanceIface: TypeDecl
}

interface VariablesToProcess {
    fullName: string,
    vartype: TypeRef
}

export var typeNameToClassMap: {
    [key: string]: Class;
} = {};

var declarationQueue: VariablesToProcess[] = [];
export function ProcessPass2(nodes: ParsedNode[], moduleName: string = null) {
    for (var i = 0; i < nodes.length; ++i) {
        var node = nodes[i];
        switch (node._t) {
            case ts.SyntaxKind.VariableDeclaration:
                var varDecl = <Variable>node;
                declarationQueue.push({ fullName: varDecl.name, vartype: varDecl.varType });
                break;
            case ts.SyntaxKind.ClassDeclaration:
                var typeDecl = <TypeDecl>node;
                processed[typeDecl.name] = typeDecl;
                classes.push(
                    new Class(
                        typeDecl.name,
                        false,
                        null,
                        typeDecl
                    ));
                break;
        }
    }

    while (declarationQueue.length > 0) {
        var entry = declarationQueue.shift();
        var classPair = GetStaticClassInterfacePair(typeNames[entry.vartype.name]);
        if (classPair == null) {
            continue;
        }

        var cls = new Class(
            entry.fullName,
            false,
            classPair.staticIface,
            classPair.instanceIface);
        classes.push(cls);
        typeNameToClassMap[classPair.staticIface.name] = cls;
        typeNameToClassMap[classPair.instanceIface.name] = cls;

        if (classPair.staticTypeName) {
            processed[classPair.staticTypeName] = classPair.staticIface;
        }

        if (classPair.instanceTypeName) {
            processed[classPair.instanceTypeName] = classPair.instanceIface;
        }

        for (var i = 0; i < cls.staticProperties.length; ++i) {
            var prop = cls.staticProperties[i];
            if (prop.propertyType._t == ts.SyntaxKind.TypeReference) {
                declarationQueue.push(
                    {
                        fullName: entry.fullName + "." + cls.staticProperties[i].name,
                        vartype: cls.staticProperties[i].propertyType
                    });
            }
        }
    }

    var done = true;
    do {
        done = true;
        for (var key in typeNames) {
            if (processed[key]) {
                continue;
            }

            switch (typeNames[key]._t) {
                case ts.SyntaxKind.InterfaceDeclaration:
                    var typeDecl = <TypeDecl>typeNames[key];
                    processed[typeDecl.name] = typeDecl;
                    classes.push(
                        new Class(
                            typeDecl.name,
                            false,
                            null,
                            typeDecl,
                            true));
                    done = false;
            }
        }
    } while (!done);
}

function GetStaticClassInterfacePair(t: TypeDecl): StaticIfaceCombo {
    for (var i = 0; i < t.members.length; ++i) {
        var member = t.members[i];
        if (member._t == ts.SyntaxKind.ConstructSignature) {
            var constructSig = <CoreMethodSig>member;
            var typeRef = <TypeRef>constructSig.returnType;

            if (typeRef._t == ts.SyntaxKind.InterfaceDeclaration) {
                return {
                    staticTypeName: t.name,
                    instanceTypeName: typeRef.name,
                    staticIface: t,
                    instanceIface: <TypeDecl>typeRef
                };
            }
            else {
                return {
                    staticTypeName: t.name,
                    instanceTypeName: typeRef.name,
                    staticIface: t,
                    instanceIface: typeNames[typeRef.name]
                };
            }
        }
    }

    return null;
}


var anons = 0;
function CreateAnonymousType(t: TypeLitDecl): TypeRef {
    var name = "Anony_" + anons++;
    var typeDecl: TypeDecl = {
        _t: ts.SyntaxKind.InterfaceDeclaration,
        name: name,
        members: t.members,
        typeParameters: [],
        extnds: null
    };

    typeNames[name] = typeDecl;

    return { _t: ts.SyntaxKind.TypeReference, name: name };
}

function CreateClass(
    name: string,
    staticTypeInterface: TypeDecl) {

    var returnTypes: TypeNameMap = {};
    var newReturnTypes: TypeDecl[] = [];
    var staticMembers = staticTypeInterface.members;
    if (staticMembers)
    for (var iMember = 0; iMember < staticMembers.length; ++iMember) {
        var member = staticMembers[iMember];
        if (member._t == ts.SyntaxKind.ConstructSignature) {
            var constructSig = <CoreMethodSig>member;
            var typeRef = <TypeRef>constructSig.returnType;
            if (typeNames[typeRef.name]) {
                if (!returnTypes[typeRef.name]) {
                    newReturnTypes.push(typeNames[typeRef.name]);
                    returnTypes[typeRef.name] = typeNames[typeRef.name];
                }
            }
            else {
                console.log("can't find type for " + JSON.stringify(typeRef));
            }
        }
    }

    for (var iConstruct = 0; iConstruct < newReturnTypes.length; ++iConstruct) {
        if (iConstruct == 0) {
            classes.push(
                new Class(
                    name,
                    false,
                    staticTypeInterface,
                    newReturnTypes[iConstruct]
                ));
        }
        else {
            classes.push(
                new Class(
                    name,
                    true,
                    staticTypeInterface,
                    newReturnTypes[iConstruct]
                    ));
        }
    }

    return;
}

import * as ts from "./node_modules/typescript/lib/typescript";

export var reverseMap =
    getReverseMap({
        Unknown : 0,
        EndOfFileToken : 1,
        SingleLineCommentTrivia : 2,
        MultiLineCommentTrivia : 3,
        NewLineTrivia : 4,
        WhitespaceTrivia : 5,
        ShebangTrivia : 6,
        ConflictMarkerTrivia : 7,
        NumericLiteral : 8,
        StringLiteral : 9,
        RegularExpressionLiteral : 10,
        NoSubstitutionTemplateLiteral : 11,
        TemplateHead : 12,
        TemplateMiddle : 13,
        TemplateTail : 14,
        OpenBraceToken : 15,
        CloseBraceToken : 16,
        OpenParenToken : 17,
        CloseParenToken : 18,
        OpenBracketToken : 19,
        CloseBracketToken : 20,
        DotToken : 21,
        DotDotDotToken : 22,
        SemicolonToken : 23,
        CommaToken : 24,
        LessThanToken : 25,
        LessThanSlashToken : 26,
        GreaterThanToken : 27,
        LessThanEqualsToken : 28,
        GreaterThanEqualsToken : 29,
        EqualsEqualsToken : 30,
        ExclamationEqualsToken : 31,
        EqualsEqualsEqualsToken : 32,
        ExclamationEqualsEqualsToken : 33,
        EqualsGreaterThanToken : 34,
        PlusToken : 35,
        MinusToken : 36,
        AsteriskToken : 37,
        AsteriskAsteriskToken : 38,
        SlashToken : 39,
        PercentToken : 40,
        PlusPlusToken : 41,
        MinusMinusToken : 42,
        LessThanLessThanToken : 43,
        GreaterThanGreaterThanToken : 44,
        GreaterThanGreaterThanGreaterThanToken : 45,
        AmpersandToken : 46,
        BarToken : 47,
        CaretToken : 48,
        ExclamationToken : 49,
        TildeToken : 50,
        AmpersandAmpersandToken : 51,
        BarBarToken : 52,
        QuestionToken : 53,
        ColonToken : 54,
        AtToken : 55,
        EqualsToken : 56,
        PlusEqualsToken : 57,
        MinusEqualsToken : 58,
        AsteriskEqualsToken : 59,
        AsteriskAsteriskEqualsToken : 60,
        SlashEqualsToken : 61,
        PercentEqualsToken : 62,
        LessThanLessThanEqualsToken : 63,
        GreaterThanGreaterThanEqualsToken : 64,
        GreaterThanGreaterThanGreaterThanEqualsToken : 65,
        AmpersandEqualsToken : 66,
        BarEqualsToken : 67,
        CaretEqualsToken: 68,
        Identifier: 69,
        BreakKeyword : 70,
        CaseKeyword : 71,
        CatchKeyword : 72,
        ClassKeyword : 73,
        ConstKeyword : 74,
        ContinueKeyword : 75,
        DebuggerKeyword : 76,
        DefaultKeyword : 77,
        DeleteKeyword : 78,
        DoKeyword : 79,
        ElseKeyword : 80,
        EnumKeyword : 81,
        ExportKeyword : 82,
        ExtendsKeyword : 83,
        FalseKeyword : 84,
        FinallyKeyword : 85,
        ForKeyword : 86,
        FunctionKeyword : 87,
        IfKeyword : 88,
        ImportKeyword : 89,
        InKeyword : 90,
        InstanceOfKeyword : 91,
        NewKeyword : 92,
        NullKeyword : 93,
        ReturnKeyword : 94,
        SuperKeyword : 95,
        SwitchKeyword : 96,
        ThisKeyword : 97,
        ThrowKeyword : 98,
        TrueKeyword : 99,
        TryKeyword : 100,
        TypeOfKeyword : 101,
        VarKeyword : 102,
        VoidKeyword : 103,
        WhileKeyword : 104,
        WithKeyword : 105,
        ImplementsKeyword : 106,
        InterfaceKeyword : 107,
        LetKeyword : 108,
        PackageKeyword : 109,
        PrivateKeyword : 110,
        ProtectedKeyword : 111,
        PublicKeyword : 112,
        StaticKeyword : 113,
        YieldKeyword : 114,
        AbstractKeyword : 115,
        AsKeyword : 116,
        AnyKeyword : 117,
        AsyncKeyword : 118,
        AwaitKeyword : 119,
        BooleanKeyword : 120,
        ConstructorKeyword : 121,
        DeclareKeyword : 122,
        GetKeyword : 123,
        IsKeyword : 124,
        ModuleKeyword : 125,
        NamespaceKeyword : 126,
        RequireKeyword : 127,
        NumberKeyword : 128,
        SetKeyword : 129,
        StringKeyword : 130,
        SymbolKeyword : 131,
        TypeKeyword : 132,
        FromKeyword : 133,
        GlobalKeyword : 134,
        OfKeyword : 135,
        QualifiedName : 136,
        ComputedPropertyName : 137,
        TypeParameter : 138,
        Parameter : 139,
        Decorator : 140,
        PropertySignature : 141,
        PropertyDeclaration : 142,
        MethodSignature : 143,
        MethodDeclaration : 144,
        Constructor : 145,
        GetAccessor : 146,
        SetAccessor : 147,
        CallSignature : 148,
        ConstructSignature : 149,
        IndexSignature : 150,
        TypePredicate: 151,
        TypeReference: 152,
        FunctionType : 153,
        ConstructorType : 154,
        TypeQuery : 155,
        TypeLiteral : 156,
        ArrayType : 157,
        TupleType : 158,
        UnionType : 159,
        IntersectionType : 160,
        ParenthesizedType : 161,
        ThisType : 162,
        StringLiteralType : 163,
        ObjectBindingPattern : 164,
        ArrayBindingPattern : 165,
        BindingElement : 166,
        ArrayLiteralExpression : 167,
        ObjectLiteralExpression : 168,
        PropertyAccessExpression : 169,
        ElementAccessExpression : 170,
        CallExpression : 171,
        NewExpression : 172,
        TaggedTemplateExpression : 173,
        TypeAssertionExpression : 174,
        ParenthesizedExpression : 175,
        FunctionExpression : 176,
        ArrowFunction : 177,
        DeleteExpression : 178,
        TypeOfExpression : 179,
        VoidExpression : 180,
        AwaitExpression : 181,
        PrefixUnaryExpression : 182,
        PostfixUnaryExpression : 183,
        BinaryExpression : 184,
        ConditionalExpression : 185,
        TemplateExpression : 186,
        YieldExpression : 187,
        SpreadElementExpression : 188,
        ClassExpression : 189,
        OmittedExpression : 190,
        ExpressionWithTypeArguments : 191,
        AsExpression : 192,
        TemplateSpan : 193,
        SemicolonClassElement : 194,
        Block : 195,
        VariableStatement : 196,
        EmptyStatement : 197,
        ExpressionStatement : 198,
        IfStatement : 199,
        DoStatement : 200,
        WhileStatement : 201,
        ForStatement : 202,
        ForInStatement : 203,
        ForOfStatement : 204,
        ContinueStatement : 205,
        BreakStatement : 206,
        ReturnStatement : 207,
        WithStatement : 208,
        SwitchStatement : 209,
        LabeledStatement : 210,
        ThrowStatement : 211,
        TryStatement : 212,
        DebuggerStatement : 213,
        VariableDeclaration : 214,
        VariableDeclarationList : 215,
        FunctionDeclaration : 216,
        ClassDeclaration : 217,
        InterfaceDeclaration : 218,
        TypeAliasDeclaration : 219,
        EnumDeclaration : 220,
        ModuleDeclaration : 221,
        ModuleBlock : 222,
        CaseBlock : 223,
        ImportEqualsDeclaration : 224,
        ImportDeclaration : 225,
        ImportClause : 226,
        NamespaceImport : 227,
        NamedImports : 228,
        ImportSpecifier : 229,
        ExportAssignment : 230,
        ExportDeclaration : 231,
        NamedExports : 232,
        ExportSpecifier : 233,
        MissingDeclaration : 234,
        ExternalModuleReference : 235,
        JsxElement : 236,
        JsxSelfClosingElement : 237,
        JsxOpeningElement : 238,
        JsxText : 239,
        JsxClosingElement : 240,
        JsxAttribute : 241,
        JsxSpreadAttribute : 242,
        JsxExpression : 243,
        CaseClause : 244,
        DefaultClause: 245,
        HeritageClause: 246,
        CatchClause : 247,
        PropertyAssignment : 248,
        ShorthandPropertyAssignment : 249,
        EnumMember : 250,
        SourceFile : 251,
        JSDocTypeExpression : 252,
        JSDocAllType : 253,
        JSDocUnknownType : 254,
        JSDocArrayType : 255,
        JSDocUnionType : 256,
        JSDocTupleType : 257,
        JSDocNullableType : 258,
        JSDocNonNullableType : 259,
        JSDocRecordType : 260,
        JSDocRecordMember : 261,
        JSDocTypeReference : 262,
        JSDocOptionalType : 263,
        JSDocFunctionType : 264,
        JSDocVariadicType : 265,
        JSDocConstructorType : 266,
        JSDocThisType : 267,
        JSDocComment : 268,
        JSDocTag : 269,
        JSDocParameterTag : 270,
        JSDocReturnTag : 271,
        JSDocTypeTag : 272,
        JSDocTemplateTag : 273,
        SyntaxList : 274,
        Count : 275,
        FirstAssignment : 56,
        LastAssignment : 68,
        FirstReservedWord : 70,
        LastReservedWord : 105,
        FirstKeyword : 70,
        LastKeyword : 135,
        FirstFutureReservedWord : 106,
        LastFutureReservedWord : 114,
        FirstTypeNode : 151,
        LastTypeNode : 163,
        FirstPunctuation : 15,
        LastPunctuation : 68,
        FirstToken : 0,
        LastToken : 135,
        FirstTriviaToken : 2,
        LastTriviaToken : 7,
        FirstLiteralToken : 8,
        LastLiteralToken : 11,
        FirstTemplateToken : 11,
        LastTemplateToken : 14,
        FirstBinaryOperator : 25,
        LastBinaryOperator: 68,
        FirstNode: 136,
    });

export function getReverseMap(obj: any) {
    var rv: string[] = [];
    for (var key in obj) {
        rv[<number>obj[key]] = key;
    }

    return rv;
}

export function makeJson(sourceFile: ts.SourceFile): ParsedNode[]{
    let depth: number = 1;
    var rv = nodeToJson(sourceFile);

    function nodeToJson(node: ts.Node): ParsedNode | ParsedNode[]| string{
        outNode(node.kind);
        depth++;
        var rv: (ParsedNode|ParsedNode[]) = null;
        switch (node.kind) {
            case ts.SyntaxKind.VariableStatement:
                rv = variableStatementToJson(<ts.VariableStatement>node);
                break;
            case ts.SyntaxKind.VariableDeclaration:
                rv = variableDeclarationToJson(<ts.VariableDeclaration>node);
                break;
            case ts.SyntaxKind.FirstTypeNode:
                rv = firstTypeNodeToJson(<ts.TypeReferenceNode>node);
                break;
            case ts.SyntaxKind.ModuleDeclaration:
                rv = moduleDeclarationToJson(<ts.ModuleDeclaration>node);
                break;
            case ts.SyntaxKind.InterfaceDeclaration:
                rv = ifaceToJson(<ts.InterfaceDeclaration>node);
                break;
            case ts.SyntaxKind.ClassDeclaration:
                rv = classDeclToJson(<ts.ClassDeclaration>node);
                break;
            case ts.SyntaxKind.TypeLiteral:
                rv = typeLiteralToJson(<ts.TypeLiteralNode>node);
                break;
            case ts.SyntaxKind.TypeAliasDeclaration:
                rv = typeAliasToJson(<ts.TypeAliasDeclaration> node);
                break;
            case ts.SyntaxKind.FunctionDeclaration:
                rv = methodSigToJson(<ts.MethodDeclaration>node);
                (<ParsedNode>rv)._t = node.kind;
                break;
            case ts.SyntaxKind.ConstructSignature:
                rv = constructorSigToJson(<ts.ConstructorDeclaration>node);
                break;
            case ts.SyntaxKind.MethodDeclaration:
            case ts.SyntaxKind.MethodSignature:
                rv = methodSigToJson(<ts.MethodDeclaration>node);
                break;
            case ts.SyntaxKind.PropertyDeclaration:
            case ts.SyntaxKind.PropertySignature:
                rv = propertySigToJson(<ts.PropertyDeclaration>node);
                break;
            case ts.SyntaxKind.IndexSignature:
                rv = indexSigTojson(<ts.IndexSignatureDeclaration>node);
                break;
            case ts.SyntaxKind.Constructor:
                rv = constructorToJson(<ts.FunctionOrConstructorTypeNode>node);
                break;
            case ts.SyntaxKind.CallSignature:
                rv = callSigToJson(<ts.FunctionOrConstructorTypeNode>node);
                break;
            case ts.SyntaxKind.FunctionType:
                rv = functionTypToJson(<ts.FunctionOrConstructorTypeNode>node);
                break;
            case ts.SyntaxKind.Parameter:
                rv = parameterToJson(<ts.ParameterDeclaration>node);
                break;
            case ts.SyntaxKind.HeritageClause:
                rv = heritageClauseToJson(<ts.HeritageClause>node);
                break;
            case ts.SyntaxKind.Identifier:
                rv = identifierToJson(<ts.Identifier>node);
                break;
            case ts.SyntaxKind.ArrayType:
                rv = arrayTypeToJson(<ts.ArrayTypeNode>node);
                break;
            case ts.SyntaxKind.UnionType:
                rv = unionTypeToJson(<ts.UnionTypeNode>node);
                break;
            case ts.SyntaxKind.TypeParameter:
                rv = typeParameterToJson(<ts.TypeParameterDeclaration>node);
                break;
            case ts.SyntaxKind.StringLiteral:
                rv = <Literal>{ _t: node.kind, value: (<ts.StringLiteral>node).text };
            case ts.SyntaxKind.AnyKeyword:
            case ts.SyntaxKind.VoidKeyword:
            case ts.SyntaxKind.NumberKeyword:
            case ts.SyntaxKind.StringKeyword:
            case ts.SyntaxKind.BooleanKeyword:
            case ts.SyntaxKind.SymbolKeyword:
                rv = { _t: node.kind };
                break;
            case ts.SyntaxKind.SourceFile:
                break;
            case ts.SyntaxKind.TypeReference:
                rv = unknownNodeToJson(node);
                break;
            default:
                rv = unknownNodeToJson(node);
                break;
        }

        if (rv == null) {
            rv = <ParsedNode | ParsedNode[]>ts.forEachChild(node, nodeToJson, nodesToJsonArr);
        }

        depth--;

        return rv;
    }

    function nodesToJsonArr(nodes: ts.Node[]): ParsedNode[]{
        if (!nodes || nodes.length == 0) { return null; }

        var rv: ParsedNode[] = [];
        for (var i = 0; i < nodes.length; ++i) {
            rv.push(<ParsedNode>nodeToJson(nodes[i]));
        }

        return rv;
    }

    function variableStatementToJson(node: ts.VariableStatement): any {
        let variables: Variable[] = [];
        let declList = node.declarationList;
        for (let iDecl = 0; iDecl < declList.declarations.length; ++iDecl) {
            variables[iDecl] = <Variable>nodeToJson(declList.declarations[iDecl]);
        }

        return variables;
    }

    function variableDeclarationToJson(node: ts.VariableDeclaration): Variable {
        var rv = <Variable>
            {
                _t: node.kind,
                name: (<ts.Identifier>node.name).text,
                varType: node.type != null ? nodeToJson(node.type) : null
            };

        return rv;
    }

    function firstTypeNodeToJson(firstTypeNode: ts.TypeReferenceNode): TypeRef {
        let typeArgs: TypeRef[];
        if (firstTypeNode.typeArguments) {
            typeArgs = [];
            for (var i = 0; i < firstTypeNode.typeArguments.length; ++i) {
                typeArgs.push(<TypeRef>nodeToJson(firstTypeNode.typeArguments[i]));
            }
        }

        return {
            _t: ts.SyntaxKind.TypeReference,
            name: (<ts.Identifier>firstTypeNode.typeName).text,
            typeArgs: typeArgs
        };
    }

    function exportAssignmentToJson(node: ts.ExportAssignment): Export {
        if (node.expression.kind != ts.SyntaxKind.Identifier) debugger;

        return { _t: ts.SyntaxKind.ExportAssignment, name: (<ts.Identifier>node.expression).text };
    }

    function moduleDeclarationToJson(modDecl: ts.ModuleDeclaration): Module {
        let subModules : Module[] = [];
        let exports : Export[] = [];
        let ifaces : TypeDecl[] = [];
        let classes: TypeDecl[] = [];
        let variables: Variable[] = [];

        let name = modDecl.name.text;

        var stmts = (<ts.ModuleBlock>modDecl.body).statements;
        for (let iStmt = 0; iStmt < stmts.length; iStmt++) {
            let stmt = stmts[iStmt];
            switch (stmt.kind) {
                case ts.SyntaxKind.ExportAssignment:
                    exports.push(exportAssignmentToJson(<ts.ExportAssignment>stmt));
                    break;
                case ts.SyntaxKind.InterfaceDeclaration:
                    ifaces.push(ifaceToJson(<ts.InterfaceDeclaration>stmt));
                    break;
                case ts.SyntaxKind.ClassDeclaration:
                    classes.push(classDeclToJson(<ts.ClassDeclaration>stmt));
                    break;
                case ts.SyntaxKind.VariableStatement:
                    variables = variables.concat(<any[]>variableStatementToJson(<ts.VariableStatement>stmt).variables);
                    break;
                default:
                    break;

            }
        }

        return {
            _t: ts.SyntaxKind.ModuleDeclaration,
            name: name,
            exports: exports,
            ifaces: ifaces,
            variables: variables,
            classes: classes,
            subModules: subModules
        };
    }

    function identifierToJson(node: ts.Identifier): Identifier {
        return { _t: ts.SyntaxKind.Identifier, name: node.text };
    }

    function ifaceToJson(node: ts.InterfaceDeclaration): TypeDecl {
        var rv = <TypeDecl>{
            _t: ts.SyntaxKind.InterfaceDeclaration,
            name: node.name.text,
            members: nodesToJsonArr(node.members),
        };

        if (node.typeParameters) {
            rv.typeParameters = typeParametersToJson(node.typeParameters);
        }

        if (node.heritageClauses) {
            rv.extnds = <TypeRef[]>nodesToJsonArr(node.heritageClauses);
        }

        return rv;
    }

    function classDeclToJson(node: ts.ClassDeclaration): TypeDecl {
        var rv = <TypeDecl>{
            _t: ts.SyntaxKind.ClassDeclaration,
            name: node.name.text,
            members: nodesToJsonArr(node.members),
        };

        if (node.typeParameters) {
            rv.typeParameters = typeParametersToJson(node.typeParameters);
        }

        if (node.heritageClauses) {
            rv.extnds = <TypeRef[]>nodesToJsonArr(node.heritageClauses);
        }

        return rv;
    }

    function typeLiteralToJson(node: ts.TypeLiteralNode): TypeLitDecl {
        return {
            _t: ts.SyntaxKind.TypeLiteral,
            members: nodesToJsonArr(node.members),
        };
    }

    function constructorSigToJson(node: ts.ConstructorDeclaration): CoreMethodSig {
        assertKind(node, ts.SyntaxKind.ConstructSignature);
        return {
            _t: ts.SyntaxKind.ConstructSignature,
            parameters: parametersToJson(node.parameters),
            typeParameters: typeParametersToJson(node.typeParameters),
            returnType: <TypeRef>nodeToJson(node.type)
        };
    }

    function methodSigToJson(node: ts.MethodDeclaration): CoreMemberMethodDecl {
        return {
            _t: ts.SyntaxKind.MethodDeclaration,
            isClassConstructor: false,
            name: (<ts.Identifier>node.name).text,
            parameters: parametersToJson(node.parameters),
            typeParameters: typeParametersToJson(node.typeParameters),
            returnType: <TypeRef>nodeToJson(node.type),
            undefinable: node.questionToken != null
        };
    }

    function functionTypToJson(node: ts.FunctionOrConstructorTypeNode): CoreMethodSig {
        assertKind(node, ts.SyntaxKind.FunctionType);
        return {
            _t: ts.SyntaxKind.FunctionType,
            parameters: parametersToJson(node.parameters),
            typeParameters: typeParametersToJson(node.typeParameters),
            returnType: <TypeRef>nodeToJson(node.type)
        };
    }

    function callSigToJson(node: ts.FunctionOrConstructorTypeNode): CoreMethodSig {
        assertKind(node, ts.SyntaxKind.CallSignature);
        return {
            _t: ts.SyntaxKind.CallSignature,
            parameters: parametersToJson(node.parameters),
            typeParameters: typeParametersToJson(node.typeParameters),
            returnType: <TypeRef>nodeToJson(node.type)
        };
    }

    function constructorToJson(node: ts.FunctionOrConstructorTypeNode): CoreMethodSig {
        assertKind(node, ts.SyntaxKind.Constructor);
        return {
            _t: node.kind,
            parameters: parametersToJson(node.parameters),
        };
    }

    function propertySigToJson(node: ts.PropertyDeclaration): PropertyMemberDecl {
        if (node.kind != ts.SyntaxKind.PropertySignature
            && node.kind != ts.SyntaxKind.PropertyDeclaration) {
            debugger;
        }

        var rv = <PropertyMemberDecl>{
            _t: ts.SyntaxKind.PropertySignature,
            name: (<ts.Identifier>node.name).text,
            returnType: nodeToJson(node.type),
        }

        if (node.questionToken) {
            rv.undefinable = true;
        }

        return rv;
    }

    function indexSigTojson(node: ts.IndexSignatureDeclaration): any {
        assertKind(node, ts.SyntaxKind.IndexSignature);
        return {
            _t: node.kind,
            "type": nodeToJson(node.type),
            param: nodeToJson(node.parameters[0])
        }
    }

    function parametersToJson(nodes: ts.Node[]): ParameterDecl[]{
        var rv : ParameterDecl[] = [];
        for (var i = 0; i < nodes.length; ++i) {
            rv.push(parameterToJson(<ts.ParameterDeclaration>nodes[i]));
        }

        return rv;
    }

    function typeParametersToJson(nodes: ts.Node[]): TypeParam[]{
        var rv: TypeParam[] = [];
        if (nodes && nodes.length) {
            for (var i = 0; i < nodes.length; ++i) {
                rv.push(typeParameterToJson(<ts.TypeParameterDeclaration>nodes[i]));
            }
        }

        return rv;
    }

    function parameterToJson(node: ts.ParameterDeclaration): ParameterDecl {
        assertKind(node, ts.SyntaxKind.Parameter);
        var rv: ParameterDecl = {
            _t: ts.SyntaxKind.Parameter,
            name: (<ts.Identifier>node.name).text,
            argType: <TypeRef>nodeToJson(node.type),
        };

        if (node.dotDotDotToken) {
            rv.dotdotdot = true;
        }

        if (node.questionToken) {
            rv.optional = true;
        }

        return rv;
    }

    function typeParameterToJson(node: ts.TypeParameterDeclaration): TypeParam {
        assertKind(node, ts.SyntaxKind.TypeParameter);
        return {
            _t: node.kind,
            name: node.name.text,
            constraint: node.constraint ? <TypeRef[]>nodeToJson(node.constraint) : null
        };
    }

    function arrayTypeToJson(node: ts.ArrayTypeNode): ArrayType {
        assertKind(node, ts.SyntaxKind.ArrayType);
        return {
            _t: ts.SyntaxKind.ArrayType,
            elementType: <TypeRef>nodeToJson(node.elementType),
            name: null
        };
    }

    function heritageClauseToJson(node: ts.HeritageClause): TypeRef[]{
        assertKind(node, ts.SyntaxKind.HeritageClause);
        var rv: TypeRef[] = [];
        for (var i = 0; i < node.types.length; ++i) {
            rv.push(heritageElementToJson(<ts.ExpressionWithTypeArguments>node.types[i]));
        }

        return rv;
    }

    function heritageElementToJson(node: ts.ExpressionWithTypeArguments): TypeRef {
        assertKind(node, ts.SyntaxKind.ExpressionWithTypeArguments);

        return {
            _t: ts.SyntaxKind.ExpressionWithTypeArguments,
            name: (<ts.Identifier>node.expression).text,
        }
    }

    function unionTypeToJson(node: ts.UnionTypeNode): UnionTypeRef {
        assertKind(node, ts.SyntaxKind.UnionType);
        return {
            _t: ts.SyntaxKind.UnionType,
            name: null,
            types: <TypeRef[]>nodesToJsonArr(node.types)
        };
    }

    function typeAliasToJson(node: ts.TypeAliasDeclaration): Alias {
        assertKind(node, ts.SyntaxKind.TypeAliasDeclaration);
        return {
            _t: node.kind,
            name: node.name.text,
            aliased: <TypeRef>nodeToJson(node.type)
        };
    }

    function unknownNodeToJson(node: ts.Node): ParsedNode {
        var rv : ParsedNode = null;
        switch (node.kind) {
            default:
                break;
        }

        return rv;
    }

    function assertKind(node: ts.Node, kind: ts.SyntaxKind) {
        if (node.kind != kind) debugger;
    }

    function report(node: ts.Node, message: string) {
        let { line, character } = sourceFile.getLineAndCharacterOfPosition(node.getStart());
        console.log(`${sourceFile.fileName} (${line + 1},${character + 1}): ${message}`);
    }

    function outNode(kind: ts.SyntaxKind) {
        let str: string = "";
        for (let i = 1; i < depth; i++)
        { str += " "; }

        str += reverseMap[kind];
        // console.log(str);
    }

    return <ParsedNode[]>rv;
}

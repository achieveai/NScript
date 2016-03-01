import * as ts from "typescript";

export var reverseMap =
    getReverseMap({ Unknown: 0, EndOfFileToken: 1, SingleLineCommentTrivia: 2, MultiLineCommentTrivia: 3, NewLineTrivia: 4, WhitespaceTrivia: 5, ConflictMarkerTrivia: 6, NumericLiteral: 7, StringLiteral: 8, RegularExpressionLiteral: 9, NoSubstitutionTemplateLiteral: 10, TemplateHead: 11, TemplateMiddle: 12, TemplateTail: 13, OpenBraceToken: 14, CloseBraceToken: 15, OpenParenToken: 16, CloseParenToken: 17, OpenBracketToken: 18, CloseBracketToken: 19, DotToken: 20, DotDotDotToken: 21, SemicolonToken: 22, CommaToken: 23, LessThanToken: 24, GreaterThanToken: 25, LessThanEqualsToken: 26, GreaterThanEqualsToken: 27, EqualsEqualsToken: 28, ExclamationEqualsToken: 29, EqualsEqualsEqualsToken: 30, ExclamationEqualsEqualsToken: 31, EqualsGreaterThanToken: 32, PlusToken: 33, MinusToken: 34, AsteriskToken: 35, SlashToken: 36, PercentToken: 37, PlusPlusToken: 38, MinusMinusToken: 39, LessThanLessThanToken: 40, GreaterThanGreaterThanToken: 41, GreaterThanGreaterThanGreaterThanToken: 42, AmpersandToken: 43, BarToken: 44, CaretToken: 45, ExclamationToken: 46, TildeToken: 47, AmpersandAmpersandToken: 48, BarBarToken: 49, QuestionToken: 50, ColonToken: 51, AtToken: 52, EqualsToken: 53, PlusEqualsToken: 54, MinusEqualsToken: 55, AsteriskEqualsToken: 56, SlashEqualsToken: 57, PercentEqualsToken: 58, LessThanLessThanEqualsToken: 59, GreaterThanGreaterThanEqualsToken: 60, GreaterThanGreaterThanGreaterThanEqualsToken: 61, AmpersandEqualsToken: 62, BarEqualsToken: 63, CaretEqualsToken: 64, Identifier: 65, BreakKeyword: 66, CaseKeyword: 67, CatchKeyword: 68, ClassKeyword: 69, ConstKeyword: 70, ContinueKeyword: 71, DebuggerKeyword: 72, DefaultKeyword: 73, DeleteKeyword: 74, DoKeyword: 75, ElseKeyword: 76, EnumKeyword: 77, ExportKeyword: 78, ExtendsKeyword: 79, FalseKeyword: 80, FinallyKeyword: 81, ForKeyword: 82, FunctionKeyword: 83, IfKeyword: 84, ImportKeyword: 85, InKeyword: 86, InstanceOfKeyword: 87, NewKeyword: 88, NullKeyword: 89, ReturnKeyword: 90, SuperKeyword: 91, SwitchKeyword: 92, ThisKeyword: 93, ThrowKeyword: 94, TrueKeyword: 95, TryKeyword: 96, TypeOfKeyword: 97, VarKeyword: 98, VoidKeyword: 99, WhileKeyword: 100, WithKeyword: 101, ImplementsKeyword: 102, InterfaceKeyword: 103, LetKeyword: 104, PackageKeyword: 105, PrivateKeyword: 106, ProtectedKeyword: 107, PublicKeyword: 108, StaticKeyword: 109, YieldKeyword: 110, AsKeyword: 111, AnyKeyword: 112, BooleanKeyword: 113, ConstructorKeyword: 114, DeclareKeyword: 115, GetKeyword: 116, ModuleKeyword: 117, RequireKeyword: 118, NumberKeyword: 119, SetKeyword: 120, StringKeyword: 121, SymbolKeyword: 122, TypeKeyword: 123, FromKeyword: 124, OfKeyword: 125, QualifiedName: 126, ComputedPropertyName: 127, TypeParameter: 128, Parameter: 129, Decorator: 130, PropertySignature: 131, PropertyDeclaration: 132, MethodSignature: 133, MethodDeclaration: 134, Constructor: 135, GetAccessor: 136, SetAccessor: 137, CallSignature: 138, ConstructSignature: 139, IndexSignature: 140, TypeReference: 141, FunctionType: 142, ConstructorType: 143, TypeQuery: 144, TypeLiteral: 145, ArrayType: 146, TupleType: 147, UnionType: 148, ParenthesizedType: 149, ObjectBindingPattern: 150, ArrayBindingPattern: 151, BindingElement: 152, ArrayLiteralExpression: 153, ObjectLiteralExpression: 154, PropertyAccessExpression: 155, ElementAccessExpression: 156, CallExpression: 157, NewExpression: 158, TaggedTemplateExpression: 159, TypeAssertionExpression: 160, ParenthesizedExpression: 161, FunctionExpression: 162, ArrowFunction: 163, DeleteExpression: 164, TypeOfExpression: 165, VoidExpression: 166, PrefixUnaryExpression: 167, PostfixUnaryExpression: 168, BinaryExpression: 169, ConditionalExpression: 170, TemplateExpression: 171, YieldExpression: 172, SpreadElementExpression: 173, ClassExpression: 174, OmittedExpression: 175, TemplateSpan: 176, HeritageClauseElement: 177, SemicolonClassElement: 178, Block: 179, VariableStatement: 180, EmptyStatement: 181, ExpressionStatement: 182, IfStatement: 183, DoStatement: 184, WhileStatement: 185, ForStatement: 186, ForInStatement: 187, ForOfStatement: 188, ContinueStatement: 189, BreakStatement: 190, ReturnStatement: 191, WithStatement: 192, SwitchStatement: 193, LabeledStatement: 194, ThrowStatement: 195, TryStatement: 196, DebuggerStatement: 197, VariableDeclaration: 198, VariableDeclarationList: 199, FunctionDeclaration: 200, ClassDeclaration: 201, InterfaceDeclaration: 202, TypeAliasDeclaration: 203, EnumDeclaration: 204, ModuleDeclaration: 205, ModuleBlock: 206, CaseBlock: 207, ImportEqualsDeclaration: 208, ImportDeclaration: 209, ImportClause: 210, NamespaceImport: 211, NamedImports: 212, ImportSpecifier: 213, ExportAssignment: 214, ExportDeclaration: 215, NamedExports: 216, ExportSpecifier: 217, MissingDeclaration: 218, ExternalModuleReference: 219, CaseClause: 220, DefaultClause: 221, HeritageClause: 222, CatchClause: 223, PropertyAssignment: 224, ShorthandPropertyAssignment: 225, EnumMember: 226, SourceFile: 227, SyntaxList: 228, Count: 229, FirstAssignment: 53, LastAssignment: 64, FirstReservedWord: 66, LastReservedWord: 101, FirstKeyword: 66, LastKeyword: 125, FirstFutureReservedWord: 102, LastFutureReservedWord: 110, FirstTypeNode: 141, LastTypeNode: 149, FirstPunctuation: 14, LastPunctuation: 64, FirstToken: 0, LastToken: 125, FirstTriviaToken: 2, LastTriviaToken: 6, FirstLiteralToken: 7, LastLiteralToken: 10, FirstTemplateToken: 10, LastTemplateToken: 13, FirstBinaryOperator: 24, LastBinaryOperator: 64, FirstNode: 126 });

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
            case ts.SyntaxKind.HeritageClauseElement:
                rv = heritageElementToJson(<ts.HeritageClauseElement>node);
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
            rv.push(heritageElementToJson(<ts.HeritageClauseElement>node.types[i]));
        }

        return rv;
    }

    function heritageElementToJson(node: ts.HeritageClauseElement): TypeRef {
        assertKind(node, ts.SyntaxKind.HeritageClauseElement);

        return {
            _t: ts.SyntaxKind.HeritageClauseElement,
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

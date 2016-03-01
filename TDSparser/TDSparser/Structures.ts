interface ParsedNode {
    _t: number;
}

interface TypeParam extends ParsedNode {
    name: string;
    constraint: TypeRef[];
}

interface Literal extends ParsedNode {
    value: boolean | number | string;
}

interface TypeRef extends ParsedNode {
    name: string;
    typeArgs?: TypeRef[];
}

interface SystemType extends TypeRef {
}

interface ArrayType extends TypeRef {
    elementType: TypeRef;
}

interface UnionTypeRef extends TypeRef {
    types: TypeRef[];
}

interface Variable extends ParsedNode {
    name: string;
    varType: TypeRef;
}

interface Export extends ParsedNode {
    name: string;
}

interface Module extends ParsedNode {
    name: string;
    exports: Export[];
    ifaces: TypeDecl[];
    variables: Variable[];
    classes: TypeDecl[];
    subModules: Module[];
}

interface Identifier extends ParsedNode {
    name: string;
}

interface TypeLitDecl extends ParsedNode {
    members: MemberDecl[];
}

interface CoreType extends TypeLitDecl {
    name: string;
}

interface TypeDecl extends CoreType {
    typeParameters: TypeParam[];
    extnds: TypeRef[];
}

interface MemberDecl extends ParsedNode {
    undefinable?: boolean;
    parentType?: TypeDecl;
}

interface ParameterDecl extends ParsedNode {
    name: string;
    dotdotdot?: boolean;
    optional?: boolean;
    argType: TypeRef | UnionTypeRef | Literal;
}

interface CoreMethodSig extends ParsedNode {
    parameters: ParameterDecl[];
    typeParameters?: TypeParam[];
    returnType?: TypeRef;
}

interface NamedMemberDecl extends MemberDecl {
    name: string;
}

interface CoreMemberMethodDecl extends CoreMethodSig, NamedMemberDecl {
    isClassConstructor: boolean;
}

interface IndexerMemberDecl extends MemberDecl {
    returnType: TypeRef;
    param: ParameterDecl;
}

interface PropertyMemberDecl extends NamedMemberDecl {
    returnType: TypeRef;
}

interface Alias extends ParsedNode {
    name: string;
    aliased: TypeRef;
}
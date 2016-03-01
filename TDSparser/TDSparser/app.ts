/// <reference path="./Scripts/TypeScript/TypeScript.d.ts" />
// <reference path="./Scripts/TypeScript/TypeScriptServices.d.ts" />
// <reference path="./typings/lodash/lodash.d.ts" />
// <reference path="Structures.ts" />
// <reference path="ProcessNodes.ts" />

/************************************************************************
Things to fix
1. Handle identifiers that are keywords in C#, e.g. interface delegate{(arg: any) : any;}
2. Handle modules (also figure out how to make it work with namespaces etc.)
3. Handle nested types:
    Take a look at Hammerjs.d.ts example, there we will see many static properties that point to
    staticInterface with new method. example below.

    var v : FooStatic;
    interface FooStatic {
        new (arg: any): Foo;
        Boo: BooStatic;
    }

    interface Foo {}

    interface BooStatic {
        new (arg: any) : Boo;
    }

    interface Boo {}

    Rule:
        1. Presence of Static really means that the real type is NonStatic one, but who ever references Static
            is really a Type, e.g. in above example FooStatic.Boo is really a type "v.Boo", which in c# should be
            "Foo.Boo"
        2. If more than one variable or proeprties point to same type, do not emit one of them.
*/

import {readFileSync, writeFileSync} from "fs";
import * as ts from "./node_modules/typescript/lib/typescript";
import * as _ from "lodash";
import * as ProcessNodes from "./ProcessNodes";
import {makeJson} from "./TdsToJson";

var cl = ts.parseCommandLine(["test"]);
var program = ts.createProgram(["tmpe"], cl.options, null);
var typeChecker = program.getTypeChecker();

var args = process.argv.slice(2);
let sourceFile = ts.createSourceFile(
    "fileName",
    readFileSync(
        args[0]).toString(),
        // "/Users/Gautamb/Documents/Visual Studio 2013/Projects/TDSparser/TDSparser/testTDS.txt").toString(),
    ts.ScriptTarget.Latest, false);

var nodes = <ParsedNode[]>makeJson(sourceFile);
nodes = _.flatten(nodes);
ProcessNodes.ProcessPass1(nodes, null);
ProcessNodes.ProcessPass2(nodes, null);
console.log(ProcessNodes.classes.length);

var output = "";
for (var i = 0; i < ProcessNodes.classes.length; ++i) {
    var strPart = ProcessNodes.classes[i].generateCsFile();
    console.log(strPart);
    output += strPart;
}

writeFileSync(args[1], output);
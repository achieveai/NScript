// $ANTLR 3.3.0.7239 JavaScript.g3 2021-07-05 17:31:21

// The variable 'variable' is assigned but its value is never used.
#pragma warning disable 219
// Unreachable code detected.
#pragma warning disable 162


using System.Collections.Generic;
using Antlr.Runtime;
using Stack = System.Collections.Generic.Stack<object>;
using List = System.Collections.IList;
using ArrayList = System.Collections.Generic.List<object>;
using Map = System.Collections.IDictionary;
using HashMap = System.Collections.Generic.Dictionary<object, object>;

using Antlr.Runtime.Tree;
using RewriteRuleITokenStream = Antlr.Runtime.Tree.RewriteRuleTokenStream;

namespace NScript.JSParser
{
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "3.3.0.7239")]
[System.CLSCompliant(false)]
public partial class JavaScriptParser : Antlr.Runtime.Parser
{
	internal static readonly string[] tokenNames = new string[] {
		"<invalid>", "<EOR>", "<DOWN>", "<UP>", "ACCESSOR_EXPR", "AND", "ARGS", "ARGUMENTS", "ASN", "BINARY_EXPR", "BIT_AND", "BIT_AND_ASN", "BIT_NOT", "BIT_OR", "BIT_OR_ASN", "BIT_XOR", "BIT_XOR_ASN", "BOOL_LIT", "BREAK", "BoolLiteral", "CATCH", "COMMA", "CONDITIONAL_EXPR", "CONTINUE", "CharacterEscapeSequence", "Comment", "DELETE", "DIV", "DIV_ASN", "DOT_ACCESSOR_EXPR", "DOUBLE_NOT", "DO_BLOCK", "DecimalDigit", "DecimalLiteral", "DoubleStringCharacter", "EMPTY_STATEMENT", "EQ", "EXPR", "EscapeCharacter", "EscapeSequence", "ExponentPart", "FALSE", "FINALLY", "FOR_BLOCK", "FOR_IN_BLOCK", "FUNCTION", "FUNCTION_BODY", "GT", "GTE", "GenericSignature", "HexDigit", "HexEscapeSequence", "HexIntegerLiteral", "IDENTIFIER", "IF_BLOCK", "IMPORT_START", "IN", "INDEX_ARG", "INDEX_EXPR", "INLINE_ARRAY_INIT", "INLINE_OBJ_INIT", "INSTANCE_JSNI", "INST_OF", "Identifier", "IdentifierPart", "IdentifierStart", "LT", "LTE", "LineComment", "MEMBER_NAME", "METHOD_CALL", "MINUS", "MINUS_ASN", "MINUS_INC", "MOD", "MOD_ASN", "MUL", "MUL_ASN", "NEQ", "NEW", "NEWLINE", "NEW_OBJ_EXPR", "NOT", "NULL", "NUM_LIT", "NonEscapeCharacter", "NumericLiteral", "OR", "PLUS", "PLUS_ASN", "PLUS_INC", "POSTFIX", "PREFIX", "PROP_NAME_VALUE", "REQ", "RETURN", "RETURN_STATEMENT", "RNQ", "SEMI", "SHL", "SHL_ASN", "SHR", "SHR_ASN", "STATEMENTS", "STATEMENT_BLOCK", "STR_LIT", "SingleEscapeCharacter", "SingleStringCharacter", "SkipSpace", "StringLiteral", "THIS", "THROW", "TRUE", "TRY", "TYPENAME", "TYPEPART", "TYPE_OF", "TYPE_SEPERATOR", "TypeNameIdentifier", "USHR", "USHR_ASN", "UnicodeCombiningMark", "UnicodeConnectorPunctuation", "UnicodeDigit", "UnicodeEscapeSequence", "UnicodeLetter", "VAR", "VOID", "WHILE_BLOCK", "WhiteSpace", "'('", "')'", "'.'", "':'", "'?'", "'['", "']'", "'case'", "'default'", "'do'", "'else'", "'for'", "'function'", "'if'", "'switch'", "'while'", "'with'", "'{'", "'}'"
	};
	public const int EOF=-1;
	public const int ACCESSOR_EXPR=4;
	public const int AND=5;
	public const int ARGS=6;
	public const int ARGUMENTS=7;
	public const int ASN=8;
	public const int BINARY_EXPR=9;
	public const int BIT_AND=10;
	public const int BIT_AND_ASN=11;
	public const int BIT_NOT=12;
	public const int BIT_OR=13;
	public const int BIT_OR_ASN=14;
	public const int BIT_XOR=15;
	public const int BIT_XOR_ASN=16;
	public const int BOOL_LIT=17;
	public const int BREAK=18;
	public const int BoolLiteral=19;
	public const int CATCH=20;
	public const int COMMA=21;
	public const int CONDITIONAL_EXPR=22;
	public const int CONTINUE=23;
	public const int CharacterEscapeSequence=24;
	public const int Comment=25;
	public const int DELETE=26;
	public const int DIV=27;
	public const int DIV_ASN=28;
	public const int DOT_ACCESSOR_EXPR=29;
	public const int DOUBLE_NOT=30;
	public const int DO_BLOCK=31;
	public const int DecimalDigit=32;
	public const int DecimalLiteral=33;
	public const int DoubleStringCharacter=34;
	public const int EMPTY_STATEMENT=35;
	public const int EQ=36;
	public const int EXPR=37;
	public const int EscapeCharacter=38;
	public const int EscapeSequence=39;
	public const int ExponentPart=40;
	public const int FALSE=41;
	public const int FINALLY=42;
	public const int FOR_BLOCK=43;
	public const int FOR_IN_BLOCK=44;
	public const int FUNCTION=45;
	public const int FUNCTION_BODY=46;
	public const int GT=47;
	public const int GTE=48;
	public const int GenericSignature=49;
	public const int HexDigit=50;
	public const int HexEscapeSequence=51;
	public const int HexIntegerLiteral=52;
	public const int IDENTIFIER=53;
	public const int IF_BLOCK=54;
	public const int IMPORT_START=55;
	public const int IN=56;
	public const int INDEX_ARG=57;
	public const int INDEX_EXPR=58;
	public const int INLINE_ARRAY_INIT=59;
	public const int INLINE_OBJ_INIT=60;
	public const int INSTANCE_JSNI=61;
	public const int INST_OF=62;
	public const int Identifier=63;
	public const int IdentifierPart=64;
	public const int IdentifierStart=65;
	public const int LT=66;
	public const int LTE=67;
	public const int LineComment=68;
	public const int MEMBER_NAME=69;
	public const int METHOD_CALL=70;
	public const int MINUS=71;
	public const int MINUS_ASN=72;
	public const int MINUS_INC=73;
	public const int MOD=74;
	public const int MOD_ASN=75;
	public const int MUL=76;
	public const int MUL_ASN=77;
	public const int NEQ=78;
	public const int NEW=79;
	public const int NEWLINE=80;
	public const int NEW_OBJ_EXPR=81;
	public const int NOT=82;
	public const int NULL=83;
	public const int NUM_LIT=84;
	public const int NonEscapeCharacter=85;
	public const int NumericLiteral=86;
	public const int OR=87;
	public const int PLUS=88;
	public const int PLUS_ASN=89;
	public const int PLUS_INC=90;
	public const int POSTFIX=91;
	public const int PREFIX=92;
	public const int PROP_NAME_VALUE=93;
	public const int REQ=94;
	public const int RETURN=95;
	public const int RETURN_STATEMENT=96;
	public const int RNQ=97;
	public const int SEMI=98;
	public const int SHL=99;
	public const int SHL_ASN=100;
	public const int SHR=101;
	public const int SHR_ASN=102;
	public const int STATEMENTS=103;
	public const int STATEMENT_BLOCK=104;
	public const int STR_LIT=105;
	public const int SingleEscapeCharacter=106;
	public const int SingleStringCharacter=107;
	public const int SkipSpace=108;
	public const int StringLiteral=109;
	public const int THIS=110;
	public const int THROW=111;
	public const int TRUE=112;
	public const int TRY=113;
	public const int TYPENAME=114;
	public const int TYPEPART=115;
	public const int TYPE_OF=116;
	public const int TYPE_SEPERATOR=117;
	public const int TypeNameIdentifier=118;
	public const int USHR=119;
	public const int USHR_ASN=120;
	public const int UnicodeCombiningMark=121;
	public const int UnicodeConnectorPunctuation=122;
	public const int UnicodeDigit=123;
	public const int UnicodeEscapeSequence=124;
	public const int UnicodeLetter=125;
	public const int VAR=126;
	public const int VOID=127;
	public const int WHILE_BLOCK=128;
	public const int WhiteSpace=129;
	public const int T__130=130;
	public const int T__131=131;
	public const int T__132=132;
	public const int T__133=133;
	public const int T__134=134;
	public const int T__135=135;
	public const int T__136=136;
	public const int T__137=137;
	public const int T__138=138;
	public const int T__139=139;
	public const int T__140=140;
	public const int T__141=141;
	public const int T__142=142;
	public const int T__143=143;
	public const int T__144=144;
	public const int T__145=145;
	public const int T__146=146;
	public const int T__147=147;
	public const int T__148=148;

	// delegates
	// delegators

	#if ANTLR_DEBUG
		private static readonly bool[] decisionCanBacktrack =
			new bool[]
			{
				false, // invalid decision
				false, false, false, false, true, false, false, false, true, false, 
				false, false, false, false, false, false, false, false, false, false, 
				true, true, false, false, false, false, false, false, false, false, 
				false, false, false, false, false, false, false, false, false, false, 
				false, false, false, false, false, false, false, true, true, false, 
				false, false, false, false, false, false, false, false, true, false, 
				false, true, false, false, true, false, false, false, false, false, 
				false, false, false, false, false, false, false, false, false, false, 
				false, false, false, false, false, false, false, false, false, false, 
				false, false, false, false, false, false, false, false, false, false, 
				false, false, false, false, false, true, false, false, true, false, 
				false, false, false, false, false, false, false, false, false, false, 
				false, false, false, false, false, false, false, false, false, false, 
				true, false, false, true, true, false, true, false, false, false, false, 
				false, false, false, false, false, false, false, false, false, false, 
				false, false, false, false, false, false, false, false, false, false, 
				false, true, false, false, false, false, true, false, false, false, 
				false, false, false, false, false, false, false, false, false, false, 
				false, false, false, false, false, false, false, false, false, false, 
				false, false, false, false, false, false, false, false, false, false, 
				false, false, false, false, false, false, false, false, false, false, 
				false, false, false, false, false, false, false, false, false, false, 
				false, true, false, false, false, true, false, false, false, false, 
				false, false, false, false, false, false, false, false, false, false, 
				false, false, false, false, false, false, false, false, false, false, 
				false, false, false, false, false, false, false, false, false, false, 
				false, false, false, false, false, false, false, false, false, false, 
				false, false, false, false, false, false, false, false, false, false, 
				false, false, false, false, false, false, false, false, false, false, 
				false, false, false, false, false, false, false, false, false, false, 
				false, false, false, false, false, false, false, false, false, false, 
				false, false, false, false, false, false, false, false, false, false, 
				false, false, false, false, false, false, false, false, false, false, 
				false, false, false, false, false, false, false, false, false, false, 
				false, false, false, false, false, false, false, false
			};
	#else
		private static readonly bool[] decisionCanBacktrack = new bool[0];
	#endif
	public JavaScriptParser( ITokenStream input )
		: this( input, new RecognizerSharedState() )
	{
	}
	public JavaScriptParser(ITokenStream input, RecognizerSharedState state)
		: base(input, state)
	{
		this.state.ruleMemo = new System.Collections.Generic.Dictionary<int, int>[416+1];

		ITreeAdaptor treeAdaptor = null;
		CreateTreeAdaptor(ref treeAdaptor);
		TreeAdaptor = treeAdaptor ?? new CommonTreeAdaptor();

		OnCreated();
	}
		
	// Implement this function in your helper file to use a custom tree adaptor
	partial void CreateTreeAdaptor(ref ITreeAdaptor adaptor);

	private ITreeAdaptor adaptor;

	public ITreeAdaptor TreeAdaptor
	{
		get
		{
			return adaptor;
		}
		set
		{
			this.adaptor = value;
		}
	}

	public override string[] TokenNames { get { return JavaScriptParser.tokenNames; } }
	public override string GrammarFileName { get { return "JavaScript.g3"; } }


	partial void OnCreated();
	partial void EnterRule(string ruleName, int ruleIndex);
	partial void LeaveRule(string ruleName, int ruleIndex);

	#region Rules
	public class program_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_program();
	partial void Leave_program();

	// $ANTLR start "program"
	// JavaScript.g3:64:1: public program : ( SkipSpace )* sourceElements ( SkipSpace )* EOF ;
	[GrammarRule("program")]
	public JavaScriptParser.program_return program()
	{
		Enter_program();
		EnterRule("program", 1);
		TraceIn("program", 1);
		JavaScriptParser.program_return retval = new JavaScriptParser.program_return();
		retval.Start = (CommonToken)input.LT(1);
		int program_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken SkipSpace1=null;
		CommonToken SkipSpace3=null;
		CommonToken EOF4=null;
		JavaScriptParser.sourceElements_return sourceElements2 = default(JavaScriptParser.sourceElements_return);

		CommonTree SkipSpace1_tree=null;
		CommonTree SkipSpace3_tree=null;
		CommonTree EOF4_tree=null;

		try { DebugEnterRule(GrammarFileName, "program");
		DebugLocation(64, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 1)) { return retval; }
			// JavaScript.g3:65:5: ( ( SkipSpace )* sourceElements ( SkipSpace )* EOF )
			DebugEnterAlt(1);
			// JavaScript.g3:65:7: ( SkipSpace )* sourceElements ( SkipSpace )* EOF
			{
			root_0 = (CommonTree)adaptor.Nil();

			DebugLocation(65, 16);
			// JavaScript.g3:65:16: ( SkipSpace )*
			try { DebugEnterSubRule(1);
			while (true)
			{
				int alt1=2;
				try { DebugEnterDecision(1, decisionCanBacktrack[1]);
				int LA1_0 = input.LA(1);

				if ((LA1_0==SkipSpace))
				{
					alt1=1;
				}


				} finally { DebugExitDecision(1); }
				switch ( alt1 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:65:16: SkipSpace
					{
					DebugLocation(65, 16);
					SkipSpace1=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_program351); if (state.failed) return retval;

					}
					break;

				default:
					goto loop1;
				}
			}

			loop1:
				;

			} finally { DebugExitSubRule(1); }

			DebugLocation(65, 19);
			PushFollow(Follow._sourceElements_in_program355);
			sourceElements2=sourceElements();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) adaptor.AddChild(root_0, sourceElements2.Tree);
			DebugLocation(65, 43);
			// JavaScript.g3:65:43: ( SkipSpace )*
			try { DebugEnterSubRule(2);
			while (true)
			{
				int alt2=2;
				try { DebugEnterDecision(2, decisionCanBacktrack[2]);
				int LA2_0 = input.LA(1);

				if ((LA2_0==SkipSpace))
				{
					alt2=1;
				}


				} finally { DebugExitDecision(2); }
				switch ( alt2 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:65:43: SkipSpace
					{
					DebugLocation(65, 43);
					SkipSpace3=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_program357); if (state.failed) return retval;

					}
					break;

				default:
					goto loop2;
				}
			}

			loop2:
				;

			} finally { DebugExitSubRule(2); }

			DebugLocation(65, 49);
			EOF4=(CommonToken)Match(input,EOF,Follow._EOF_in_program361); if (state.failed) return retval;

			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("program", 1);
			LeaveRule("program", 1);
			Leave_program();
			if (state.backtracking > 0) { Memoize(input, 1, program_StartIndex); }
		}
		DebugLocation(66, 4);
		} finally { DebugExitRule(GrammarFileName, "program"); }
		return retval;

	}
	// $ANTLR end "program"

	public class sourceElements_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_sourceElements();
	partial void Leave_sourceElements();

	// $ANTLR start "sourceElements"
	// JavaScript.g3:68:1: sourceElements : sourceElement ( ( SkipSpace )* sourceElement )* -> ^( STATEMENT_BLOCK ( sourceElement )+ ) ;
	[GrammarRule("sourceElements")]
	private JavaScriptParser.sourceElements_return sourceElements()
	{
		Enter_sourceElements();
		EnterRule("sourceElements", 2);
		TraceIn("sourceElements", 2);
		JavaScriptParser.sourceElements_return retval = new JavaScriptParser.sourceElements_return();
		retval.Start = (CommonToken)input.LT(1);
		int sourceElements_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken SkipSpace6=null;
		JavaScriptParser.sourceElement_return sourceElement5 = default(JavaScriptParser.sourceElement_return);
		JavaScriptParser.sourceElement_return sourceElement7 = default(JavaScriptParser.sourceElement_return);

		CommonTree SkipSpace6_tree=null;
		RewriteRuleITokenStream stream_SkipSpace=new RewriteRuleITokenStream(adaptor,"token SkipSpace");
		RewriteRuleSubtreeStream stream_sourceElement=new RewriteRuleSubtreeStream(adaptor,"rule sourceElement");
		try { DebugEnterRule(GrammarFileName, "sourceElements");
		DebugLocation(68, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 2)) { return retval; }
			// JavaScript.g3:69:5: ( sourceElement ( ( SkipSpace )* sourceElement )* -> ^( STATEMENT_BLOCK ( sourceElement )+ ) )
			DebugEnterAlt(1);
			// JavaScript.g3:69:7: sourceElement ( ( SkipSpace )* sourceElement )*
			{
			DebugLocation(69, 7);
			PushFollow(Follow._sourceElement_in_sourceElements379);
			sourceElement5=sourceElement();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_sourceElement.Add(sourceElement5.Tree);
			DebugLocation(69, 21);
			// JavaScript.g3:69:21: ( ( SkipSpace )* sourceElement )*
			try { DebugEnterSubRule(4);
			while (true)
			{
				int alt4=2;
				try { DebugEnterDecision(4, decisionCanBacktrack[4]);
				try
				{
					alt4 = dfa4.Predict(input);
				}
				catch (NoViableAltException nvae)
				{
					DebugRecognitionException(nvae);
					throw;
				}
				} finally { DebugExitDecision(4); }
				switch ( alt4 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:69:22: ( SkipSpace )* sourceElement
					{
					DebugLocation(69, 22);
					// JavaScript.g3:69:22: ( SkipSpace )*
					try { DebugEnterSubRule(3);
					while (true)
					{
						int alt3=2;
						try { DebugEnterDecision(3, decisionCanBacktrack[3]);
						int LA3_0 = input.LA(1);

						if ((LA3_0==SkipSpace))
						{
							alt3=1;
						}


						} finally { DebugExitDecision(3); }
						switch ( alt3 )
						{
						case 1:
							DebugEnterAlt(1);
							// JavaScript.g3:69:22: SkipSpace
							{
							DebugLocation(69, 22);
							SkipSpace6=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_sourceElements382); if (state.failed) return retval; 
							if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace6);


							}
							break;

						default:
							goto loop3;
						}
					}

					loop3:
						;

					} finally { DebugExitSubRule(3); }

					DebugLocation(69, 33);
					PushFollow(Follow._sourceElement_in_sourceElements385);
					sourceElement7=sourceElement();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_sourceElement.Add(sourceElement7.Tree);

					}
					break;

				default:
					goto loop4;
				}
			}

			loop4:
				;

			} finally { DebugExitSubRule(4); }



			{
			// AST REWRITE
			// elements: sourceElement
			// token labels: 
			// rule labels: retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 70:9: -> ^( STATEMENT_BLOCK ( sourceElement )+ )
			{
				DebugLocation(70, 12);
				// JavaScript.g3:70:12: ^( STATEMENT_BLOCK ( sourceElement )+ )
				{
				CommonTree root_1 = (CommonTree)adaptor.Nil();
				DebugLocation(70, 14);
				root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(STATEMENT_BLOCK, "STATEMENT_BLOCK"), root_1);

				DebugLocation(70, 30);
				if ( !(stream_sourceElement.HasNext) )
				{
					throw new RewriteEarlyExitException();
				}
				while ( stream_sourceElement.HasNext )
				{
					DebugLocation(70, 30);
					adaptor.AddChild(root_1, stream_sourceElement.NextTree());

				}
				stream_sourceElement.Reset();

				adaptor.AddChild(root_0, root_1);
				}

			}

			retval.Tree = root_0;
			}
			}

			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("sourceElements", 2);
			LeaveRule("sourceElements", 2);
			Leave_sourceElements();
			if (state.backtracking > 0) { Memoize(input, 2, sourceElements_StartIndex); }
		}
		DebugLocation(71, 4);
		} finally { DebugExitRule(GrammarFileName, "sourceElements"); }
		return retval;

	}
	// $ANTLR end "sourceElements"

	public class sourceElement_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_sourceElement();
	partial void Leave_sourceElement();

	// $ANTLR start "sourceElement"
	// JavaScript.g3:73:1: sourceElement : ( functionDeclaration | statement );
	[GrammarRule("sourceElement")]
	private JavaScriptParser.sourceElement_return sourceElement()
	{
		Enter_sourceElement();
		EnterRule("sourceElement", 3);
		TraceIn("sourceElement", 3);
		JavaScriptParser.sourceElement_return retval = new JavaScriptParser.sourceElement_return();
		retval.Start = (CommonToken)input.LT(1);
		int sourceElement_StartIndex = input.Index;
		CommonTree root_0 = null;

		JavaScriptParser.functionDeclaration_return functionDeclaration8 = default(JavaScriptParser.functionDeclaration_return);
		JavaScriptParser.statement_return statement9 = default(JavaScriptParser.statement_return);


		try { DebugEnterRule(GrammarFileName, "sourceElement");
		DebugLocation(73, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 3)) { return retval; }
			// JavaScript.g3:74:5: ( functionDeclaration | statement )
			int alt5=2;
			try { DebugEnterDecision(5, decisionCanBacktrack[5]);
			try
			{
				alt5 = dfa5.Predict(input);
			}
			catch (NoViableAltException nvae)
			{
				DebugRecognitionException(nvae);
				throw;
			}
			} finally { DebugExitDecision(5); }
			switch (alt5)
			{
			case 1:
				DebugEnterAlt(1);
				// JavaScript.g3:74:7: functionDeclaration
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(74, 7);
				PushFollow(Follow._functionDeclaration_in_sourceElement421);
				functionDeclaration8=functionDeclaration();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, functionDeclaration8.Tree);

				}
				break;
			case 2:
				DebugEnterAlt(2);
				// JavaScript.g3:75:7: statement
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(75, 7);
				PushFollow(Follow._statement_in_sourceElement429);
				statement9=statement();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, statement9.Tree);

				}
				break;

			}
			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("sourceElement", 3);
			LeaveRule("sourceElement", 3);
			Leave_sourceElement();
			if (state.backtracking > 0) { Memoize(input, 3, sourceElement_StartIndex); }
		}
		DebugLocation(76, 4);
		} finally { DebugExitRule(GrammarFileName, "sourceElement"); }
		return retval;

	}
	// $ANTLR end "sourceElement"

	public class functionDeclaration_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_functionDeclaration();
	partial void Leave_functionDeclaration();

	// $ANTLR start "functionDeclaration"
	// JavaScript.g3:79:1: functionDeclaration : 'function' ( SkipSpace )* name= Identifier ( SkipSpace )* args= formalParameterList ( SkipSpace )* body= functionBody -> ^( FUNCTION $name $args $body) ;
	[GrammarRule("functionDeclaration")]
	private JavaScriptParser.functionDeclaration_return functionDeclaration()
	{
		Enter_functionDeclaration();
		EnterRule("functionDeclaration", 4);
		TraceIn("functionDeclaration", 4);
		JavaScriptParser.functionDeclaration_return retval = new JavaScriptParser.functionDeclaration_return();
		retval.Start = (CommonToken)input.LT(1);
		int functionDeclaration_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken name=null;
		CommonToken string_literal10=null;
		CommonToken SkipSpace11=null;
		CommonToken SkipSpace12=null;
		CommonToken SkipSpace13=null;
		JavaScriptParser.formalParameterList_return args = default(JavaScriptParser.formalParameterList_return);
		JavaScriptParser.functionBody_return body = default(JavaScriptParser.functionBody_return);

		CommonTree name_tree=null;
		CommonTree string_literal10_tree=null;
		CommonTree SkipSpace11_tree=null;
		CommonTree SkipSpace12_tree=null;
		CommonTree SkipSpace13_tree=null;
		RewriteRuleITokenStream stream_142=new RewriteRuleITokenStream(adaptor,"token 142");
		RewriteRuleITokenStream stream_SkipSpace=new RewriteRuleITokenStream(adaptor,"token SkipSpace");
		RewriteRuleITokenStream stream_Identifier=new RewriteRuleITokenStream(adaptor,"token Identifier");
		RewriteRuleSubtreeStream stream_formalParameterList=new RewriteRuleSubtreeStream(adaptor,"rule formalParameterList");
		RewriteRuleSubtreeStream stream_functionBody=new RewriteRuleSubtreeStream(adaptor,"rule functionBody");
		try { DebugEnterRule(GrammarFileName, "functionDeclaration");
		DebugLocation(79, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 4)) { return retval; }
			// JavaScript.g3:80:5: ( 'function' ( SkipSpace )* name= Identifier ( SkipSpace )* args= formalParameterList ( SkipSpace )* body= functionBody -> ^( FUNCTION $name $args $body) )
			DebugEnterAlt(1);
			// JavaScript.g3:80:7: 'function' ( SkipSpace )* name= Identifier ( SkipSpace )* args= formalParameterList ( SkipSpace )* body= functionBody
			{
			DebugLocation(80, 7);
			string_literal10=(CommonToken)Match(input,142,Follow._142_in_functionDeclaration447); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_142.Add(string_literal10);

			DebugLocation(80, 18);
			// JavaScript.g3:80:18: ( SkipSpace )*
			try { DebugEnterSubRule(6);
			while (true)
			{
				int alt6=2;
				try { DebugEnterDecision(6, decisionCanBacktrack[6]);
				int LA6_0 = input.LA(1);

				if ((LA6_0==SkipSpace))
				{
					alt6=1;
				}


				} finally { DebugExitDecision(6); }
				switch ( alt6 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:80:18: SkipSpace
					{
					DebugLocation(80, 18);
					SkipSpace11=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_functionDeclaration449); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace11);


					}
					break;

				default:
					goto loop6;
				}
			}

			loop6:
				;

			} finally { DebugExitSubRule(6); }

			DebugLocation(80, 33);
			name=(CommonToken)Match(input,Identifier,Follow._Identifier_in_functionDeclaration454); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_Identifier.Add(name);

			DebugLocation(80, 45);
			// JavaScript.g3:80:45: ( SkipSpace )*
			try { DebugEnterSubRule(7);
			while (true)
			{
				int alt7=2;
				try { DebugEnterDecision(7, decisionCanBacktrack[7]);
				int LA7_0 = input.LA(1);

				if ((LA7_0==SkipSpace))
				{
					alt7=1;
				}


				} finally { DebugExitDecision(7); }
				switch ( alt7 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:80:45: SkipSpace
					{
					DebugLocation(80, 45);
					SkipSpace12=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_functionDeclaration456); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace12);


					}
					break;

				default:
					goto loop7;
				}
			}

			loop7:
				;

			} finally { DebugExitSubRule(7); }

			DebugLocation(80, 60);
			PushFollow(Follow._formalParameterList_in_functionDeclaration461);
			args=formalParameterList();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_formalParameterList.Add(args.Tree);
			DebugLocation(80, 81);
			// JavaScript.g3:80:81: ( SkipSpace )*
			try { DebugEnterSubRule(8);
			while (true)
			{
				int alt8=2;
				try { DebugEnterDecision(8, decisionCanBacktrack[8]);
				int LA8_0 = input.LA(1);

				if ((LA8_0==SkipSpace))
				{
					alt8=1;
				}


				} finally { DebugExitDecision(8); }
				switch ( alt8 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:80:81: SkipSpace
					{
					DebugLocation(80, 81);
					SkipSpace13=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_functionDeclaration463); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace13);


					}
					break;

				default:
					goto loop8;
				}
			}

			loop8:
				;

			} finally { DebugExitSubRule(8); }

			DebugLocation(80, 96);
			PushFollow(Follow._functionBody_in_functionDeclaration468);
			body=functionBody();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_functionBody.Add(body.Tree);


			{
			// AST REWRITE
			// elements: name, args, body
			// token labels: name
			// rule labels: args, body, retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleITokenStream stream_name=new RewriteRuleITokenStream(adaptor,"token name",name);
			RewriteRuleSubtreeStream stream_args=new RewriteRuleSubtreeStream(adaptor,"rule args",args!=null?args.Tree:null);
			RewriteRuleSubtreeStream stream_body=new RewriteRuleSubtreeStream(adaptor,"rule body",body!=null?body.Tree:null);
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 81:9: -> ^( FUNCTION $name $args $body)
			{
				DebugLocation(81, 12);
				// JavaScript.g3:81:12: ^( FUNCTION $name $args $body)
				{
				CommonTree root_1 = (CommonTree)adaptor.Nil();
				DebugLocation(81, 14);
				root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(FUNCTION, "FUNCTION"), root_1);

				DebugLocation(81, 24);
				adaptor.AddChild(root_1, stream_name.NextNode());
				DebugLocation(81, 30);
				adaptor.AddChild(root_1, stream_args.NextTree());
				DebugLocation(81, 36);
				adaptor.AddChild(root_1, stream_body.NextTree());

				adaptor.AddChild(root_0, root_1);
				}

			}

			retval.Tree = root_0;
			}
			}

			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("functionDeclaration", 4);
			LeaveRule("functionDeclaration", 4);
			Leave_functionDeclaration();
			if (state.backtracking > 0) { Memoize(input, 4, functionDeclaration_StartIndex); }
		}
		DebugLocation(82, 4);
		} finally { DebugExitRule(GrammarFileName, "functionDeclaration"); }
		return retval;

	}
	// $ANTLR end "functionDeclaration"

	public class functionExpression_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_functionExpression();
	partial void Leave_functionExpression();

	// $ANTLR start "functionExpression"
	// JavaScript.g3:84:1: functionExpression : 'function' ( SkipSpace )* (name= Identifier )? ( SkipSpace )* args= formalParameterList ( SkipSpace )* body= functionBody -> ^( FUNCTION ( $name)? $args $body) ;
	[GrammarRule("functionExpression")]
	private JavaScriptParser.functionExpression_return functionExpression()
	{
		Enter_functionExpression();
		EnterRule("functionExpression", 5);
		TraceIn("functionExpression", 5);
		JavaScriptParser.functionExpression_return retval = new JavaScriptParser.functionExpression_return();
		retval.Start = (CommonToken)input.LT(1);
		int functionExpression_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken name=null;
		CommonToken string_literal14=null;
		CommonToken SkipSpace15=null;
		CommonToken SkipSpace16=null;
		CommonToken SkipSpace17=null;
		JavaScriptParser.formalParameterList_return args = default(JavaScriptParser.formalParameterList_return);
		JavaScriptParser.functionBody_return body = default(JavaScriptParser.functionBody_return);

		CommonTree name_tree=null;
		CommonTree string_literal14_tree=null;
		CommonTree SkipSpace15_tree=null;
		CommonTree SkipSpace16_tree=null;
		CommonTree SkipSpace17_tree=null;
		RewriteRuleITokenStream stream_142=new RewriteRuleITokenStream(adaptor,"token 142");
		RewriteRuleITokenStream stream_SkipSpace=new RewriteRuleITokenStream(adaptor,"token SkipSpace");
		RewriteRuleITokenStream stream_Identifier=new RewriteRuleITokenStream(adaptor,"token Identifier");
		RewriteRuleSubtreeStream stream_formalParameterList=new RewriteRuleSubtreeStream(adaptor,"rule formalParameterList");
		RewriteRuleSubtreeStream stream_functionBody=new RewriteRuleSubtreeStream(adaptor,"rule functionBody");
		try { DebugEnterRule(GrammarFileName, "functionExpression");
		DebugLocation(84, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 5)) { return retval; }
			// JavaScript.g3:85:5: ( 'function' ( SkipSpace )* (name= Identifier )? ( SkipSpace )* args= formalParameterList ( SkipSpace )* body= functionBody -> ^( FUNCTION ( $name)? $args $body) )
			DebugEnterAlt(1);
			// JavaScript.g3:85:7: 'function' ( SkipSpace )* (name= Identifier )? ( SkipSpace )* args= formalParameterList ( SkipSpace )* body= functionBody
			{
			DebugLocation(85, 7);
			string_literal14=(CommonToken)Match(input,142,Follow._142_in_functionExpression508); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_142.Add(string_literal14);

			DebugLocation(85, 18);
			// JavaScript.g3:85:18: ( SkipSpace )*
			try { DebugEnterSubRule(9);
			while (true)
			{
				int alt9=2;
				try { DebugEnterDecision(9, decisionCanBacktrack[9]);
				int LA9_0 = input.LA(1);

				if ((LA9_0==SkipSpace))
				{
					int LA9_2 = input.LA(2);

					if ((EvaluatePredicate(synpred9_JavaScript_fragment)))
					{
						alt9=1;
					}


				}


				} finally { DebugExitDecision(9); }
				switch ( alt9 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:85:18: SkipSpace
					{
					DebugLocation(85, 18);
					SkipSpace15=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_functionExpression510); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace15);


					}
					break;

				default:
					goto loop9;
				}
			}

			loop9:
				;

			} finally { DebugExitSubRule(9); }

			DebugLocation(85, 33);
			// JavaScript.g3:85:33: (name= Identifier )?
			int alt10=2;
			try { DebugEnterSubRule(10);
			try { DebugEnterDecision(10, decisionCanBacktrack[10]);
			int LA10_0 = input.LA(1);

			if ((LA10_0==Identifier))
			{
				alt10=1;
			}
			} finally { DebugExitDecision(10); }
			switch (alt10)
			{
			case 1:
				DebugEnterAlt(1);
				// JavaScript.g3:85:33: name= Identifier
				{
				DebugLocation(85, 33);
				name=(CommonToken)Match(input,Identifier,Follow._Identifier_in_functionExpression515); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_Identifier.Add(name);


				}
				break;

			}
			} finally { DebugExitSubRule(10); }

			DebugLocation(85, 46);
			// JavaScript.g3:85:46: ( SkipSpace )*
			try { DebugEnterSubRule(11);
			while (true)
			{
				int alt11=2;
				try { DebugEnterDecision(11, decisionCanBacktrack[11]);
				int LA11_0 = input.LA(1);

				if ((LA11_0==SkipSpace))
				{
					alt11=1;
				}


				} finally { DebugExitDecision(11); }
				switch ( alt11 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:85:46: SkipSpace
					{
					DebugLocation(85, 46);
					SkipSpace16=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_functionExpression518); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace16);


					}
					break;

				default:
					goto loop11;
				}
			}

			loop11:
				;

			} finally { DebugExitSubRule(11); }

			DebugLocation(85, 61);
			PushFollow(Follow._formalParameterList_in_functionExpression523);
			args=formalParameterList();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_formalParameterList.Add(args.Tree);
			DebugLocation(85, 82);
			// JavaScript.g3:85:82: ( SkipSpace )*
			try { DebugEnterSubRule(12);
			while (true)
			{
				int alt12=2;
				try { DebugEnterDecision(12, decisionCanBacktrack[12]);
				int LA12_0 = input.LA(1);

				if ((LA12_0==SkipSpace))
				{
					alt12=1;
				}


				} finally { DebugExitDecision(12); }
				switch ( alt12 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:85:82: SkipSpace
					{
					DebugLocation(85, 82);
					SkipSpace17=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_functionExpression525); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace17);


					}
					break;

				default:
					goto loop12;
				}
			}

			loop12:
				;

			} finally { DebugExitSubRule(12); }

			DebugLocation(85, 97);
			PushFollow(Follow._functionBody_in_functionExpression530);
			body=functionBody();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_functionBody.Add(body.Tree);


			{
			// AST REWRITE
			// elements: name, args, body
			// token labels: name
			// rule labels: args, body, retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleITokenStream stream_name=new RewriteRuleITokenStream(adaptor,"token name",name);
			RewriteRuleSubtreeStream stream_args=new RewriteRuleSubtreeStream(adaptor,"rule args",args!=null?args.Tree:null);
			RewriteRuleSubtreeStream stream_body=new RewriteRuleSubtreeStream(adaptor,"rule body",body!=null?body.Tree:null);
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 86:9: -> ^( FUNCTION ( $name)? $args $body)
			{
				DebugLocation(86, 12);
				// JavaScript.g3:86:12: ^( FUNCTION ( $name)? $args $body)
				{
				CommonTree root_1 = (CommonTree)adaptor.Nil();
				DebugLocation(86, 14);
				root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(FUNCTION, "FUNCTION"), root_1);

				DebugLocation(86, 24);
				// JavaScript.g3:86:24: ( $name)?
				if ( stream_name.HasNext )
				{
					DebugLocation(86, 24);
					adaptor.AddChild(root_1, stream_name.NextNode());

				}
				stream_name.Reset();
				DebugLocation(86, 31);
				adaptor.AddChild(root_1, stream_args.NextTree());
				DebugLocation(86, 37);
				adaptor.AddChild(root_1, stream_body.NextTree());

				adaptor.AddChild(root_0, root_1);
				}

			}

			retval.Tree = root_0;
			}
			}

			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("functionExpression", 5);
			LeaveRule("functionExpression", 5);
			Leave_functionExpression();
			if (state.backtracking > 0) { Memoize(input, 5, functionExpression_StartIndex); }
		}
		DebugLocation(87, 4);
		} finally { DebugExitRule(GrammarFileName, "functionExpression"); }
		return retval;

	}
	// $ANTLR end "functionExpression"

	public class formalParameterList_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_formalParameterList();
	partial void Leave_formalParameterList();

	// $ANTLR start "formalParameterList"
	// JavaScript.g3:89:1: formalParameterList : '(' ( ( SkipSpace )* Identifier ( ( SkipSpace )* ',' ( SkipSpace )* Identifier )* )? ( SkipSpace )* ')' -> ^( ARGS ( Identifier )* ) ;
	[GrammarRule("formalParameterList")]
	private JavaScriptParser.formalParameterList_return formalParameterList()
	{
		Enter_formalParameterList();
		EnterRule("formalParameterList", 6);
		TraceIn("formalParameterList", 6);
		JavaScriptParser.formalParameterList_return retval = new JavaScriptParser.formalParameterList_return();
		retval.Start = (CommonToken)input.LT(1);
		int formalParameterList_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken char_literal18=null;
		CommonToken SkipSpace19=null;
		CommonToken Identifier20=null;
		CommonToken SkipSpace21=null;
		CommonToken char_literal22=null;
		CommonToken SkipSpace23=null;
		CommonToken Identifier24=null;
		CommonToken SkipSpace25=null;
		CommonToken char_literal26=null;

		CommonTree char_literal18_tree=null;
		CommonTree SkipSpace19_tree=null;
		CommonTree Identifier20_tree=null;
		CommonTree SkipSpace21_tree=null;
		CommonTree char_literal22_tree=null;
		CommonTree SkipSpace23_tree=null;
		CommonTree Identifier24_tree=null;
		CommonTree SkipSpace25_tree=null;
		CommonTree char_literal26_tree=null;
		RewriteRuleITokenStream stream_130=new RewriteRuleITokenStream(adaptor,"token 130");
		RewriteRuleITokenStream stream_SkipSpace=new RewriteRuleITokenStream(adaptor,"token SkipSpace");
		RewriteRuleITokenStream stream_Identifier=new RewriteRuleITokenStream(adaptor,"token Identifier");
		RewriteRuleITokenStream stream_COMMA=new RewriteRuleITokenStream(adaptor,"token COMMA");
		RewriteRuleITokenStream stream_131=new RewriteRuleITokenStream(adaptor,"token 131");

		try { DebugEnterRule(GrammarFileName, "formalParameterList");
		DebugLocation(89, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 6)) { return retval; }
			// JavaScript.g3:90:5: ( '(' ( ( SkipSpace )* Identifier ( ( SkipSpace )* ',' ( SkipSpace )* Identifier )* )? ( SkipSpace )* ')' -> ^( ARGS ( Identifier )* ) )
			DebugEnterAlt(1);
			// JavaScript.g3:90:7: '(' ( ( SkipSpace )* Identifier ( ( SkipSpace )* ',' ( SkipSpace )* Identifier )* )? ( SkipSpace )* ')'
			{
			DebugLocation(90, 7);
			char_literal18=(CommonToken)Match(input,130,Follow._130_in_formalParameterList571); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_130.Add(char_literal18);

			DebugLocation(90, 11);
			// JavaScript.g3:90:11: ( ( SkipSpace )* Identifier ( ( SkipSpace )* ',' ( SkipSpace )* Identifier )* )?
			int alt17=2;
			try { DebugEnterSubRule(17);
			try { DebugEnterDecision(17, decisionCanBacktrack[17]);
			try
			{
				alt17 = dfa17.Predict(input);
			}
			catch (NoViableAltException nvae)
			{
				DebugRecognitionException(nvae);
				throw;
			}
			} finally { DebugExitDecision(17); }
			switch (alt17)
			{
			case 1:
				DebugEnterAlt(1);
				// JavaScript.g3:90:12: ( SkipSpace )* Identifier ( ( SkipSpace )* ',' ( SkipSpace )* Identifier )*
				{
				DebugLocation(90, 12);
				// JavaScript.g3:90:12: ( SkipSpace )*
				try { DebugEnterSubRule(13);
				while (true)
				{
					int alt13=2;
					try { DebugEnterDecision(13, decisionCanBacktrack[13]);
					int LA13_0 = input.LA(1);

					if ((LA13_0==SkipSpace))
					{
						alt13=1;
					}


					} finally { DebugExitDecision(13); }
					switch ( alt13 )
					{
					case 1:
						DebugEnterAlt(1);
						// JavaScript.g3:90:12: SkipSpace
						{
						DebugLocation(90, 12);
						SkipSpace19=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_formalParameterList574); if (state.failed) return retval; 
						if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace19);


						}
						break;

					default:
						goto loop13;
					}
				}

				loop13:
					;

				} finally { DebugExitSubRule(13); }

				DebugLocation(90, 23);
				Identifier20=(CommonToken)Match(input,Identifier,Follow._Identifier_in_formalParameterList577); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_Identifier.Add(Identifier20);

				DebugLocation(90, 34);
				// JavaScript.g3:90:34: ( ( SkipSpace )* ',' ( SkipSpace )* Identifier )*
				try { DebugEnterSubRule(16);
				while (true)
				{
					int alt16=2;
					try { DebugEnterDecision(16, decisionCanBacktrack[16]);
					try
					{
						alt16 = dfa16.Predict(input);
					}
					catch (NoViableAltException nvae)
					{
						DebugRecognitionException(nvae);
						throw;
					}
					} finally { DebugExitDecision(16); }
					switch ( alt16 )
					{
					case 1:
						DebugEnterAlt(1);
						// JavaScript.g3:90:35: ( SkipSpace )* ',' ( SkipSpace )* Identifier
						{
						DebugLocation(90, 35);
						// JavaScript.g3:90:35: ( SkipSpace )*
						try { DebugEnterSubRule(14);
						while (true)
						{
							int alt14=2;
							try { DebugEnterDecision(14, decisionCanBacktrack[14]);
							int LA14_0 = input.LA(1);

							if ((LA14_0==SkipSpace))
							{
								alt14=1;
							}


							} finally { DebugExitDecision(14); }
							switch ( alt14 )
							{
							case 1:
								DebugEnterAlt(1);
								// JavaScript.g3:90:35: SkipSpace
								{
								DebugLocation(90, 35);
								SkipSpace21=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_formalParameterList580); if (state.failed) return retval; 
								if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace21);


								}
								break;

							default:
								goto loop14;
							}
						}

						loop14:
							;

						} finally { DebugExitSubRule(14); }

						DebugLocation(90, 46);
						char_literal22=(CommonToken)Match(input,COMMA,Follow._COMMA_in_formalParameterList583); if (state.failed) return retval; 
						if ( state.backtracking == 0 ) stream_COMMA.Add(char_literal22);

						DebugLocation(90, 50);
						// JavaScript.g3:90:50: ( SkipSpace )*
						try { DebugEnterSubRule(15);
						while (true)
						{
							int alt15=2;
							try { DebugEnterDecision(15, decisionCanBacktrack[15]);
							int LA15_0 = input.LA(1);

							if ((LA15_0==SkipSpace))
							{
								alt15=1;
							}


							} finally { DebugExitDecision(15); }
							switch ( alt15 )
							{
							case 1:
								DebugEnterAlt(1);
								// JavaScript.g3:90:50: SkipSpace
								{
								DebugLocation(90, 50);
								SkipSpace23=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_formalParameterList585); if (state.failed) return retval; 
								if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace23);


								}
								break;

							default:
								goto loop15;
							}
						}

						loop15:
							;

						} finally { DebugExitSubRule(15); }

						DebugLocation(90, 61);
						Identifier24=(CommonToken)Match(input,Identifier,Follow._Identifier_in_formalParameterList588); if (state.failed) return retval; 
						if ( state.backtracking == 0 ) stream_Identifier.Add(Identifier24);


						}
						break;

					default:
						goto loop16;
					}
				}

				loop16:
					;

				} finally { DebugExitSubRule(16); }


				}
				break;

			}
			} finally { DebugExitSubRule(17); }

			DebugLocation(90, 76);
			// JavaScript.g3:90:76: ( SkipSpace )*
			try { DebugEnterSubRule(18);
			while (true)
			{
				int alt18=2;
				try { DebugEnterDecision(18, decisionCanBacktrack[18]);
				int LA18_0 = input.LA(1);

				if ((LA18_0==SkipSpace))
				{
					alt18=1;
				}


				} finally { DebugExitDecision(18); }
				switch ( alt18 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:90:76: SkipSpace
					{
					DebugLocation(90, 76);
					SkipSpace25=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_formalParameterList594); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace25);


					}
					break;

				default:
					goto loop18;
				}
			}

			loop18:
				;

			} finally { DebugExitSubRule(18); }

			DebugLocation(90, 87);
			char_literal26=(CommonToken)Match(input,131,Follow._131_in_formalParameterList597); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_131.Add(char_literal26);



			{
			// AST REWRITE
			// elements: Identifier
			// token labels: 
			// rule labels: retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 91:9: -> ^( ARGS ( Identifier )* )
			{
				DebugLocation(91, 12);
				// JavaScript.g3:91:12: ^( ARGS ( Identifier )* )
				{
				CommonTree root_1 = (CommonTree)adaptor.Nil();
				DebugLocation(91, 14);
				root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(ARGS, "ARGS"), root_1);

				DebugLocation(91, 19);
				// JavaScript.g3:91:19: ( Identifier )*
				while ( stream_Identifier.HasNext )
				{
					DebugLocation(91, 19);
					adaptor.AddChild(root_1, stream_Identifier.NextNode());

				}
				stream_Identifier.Reset();

				adaptor.AddChild(root_0, root_1);
				}

			}

			retval.Tree = root_0;
			}
			}

			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("formalParameterList", 6);
			LeaveRule("formalParameterList", 6);
			Leave_formalParameterList();
			if (state.backtracking > 0) { Memoize(input, 6, formalParameterList_StartIndex); }
		}
		DebugLocation(92, 4);
		} finally { DebugExitRule(GrammarFileName, "formalParameterList"); }
		return retval;

	}
	// $ANTLR end "formalParameterList"

	public class functionBody_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_functionBody();
	partial void Leave_functionBody();

	// $ANTLR start "functionBody"
	// JavaScript.g3:94:1: functionBody : '{' ( SkipSpace )* body= sourceElements ( SkipSpace )* '}' -> ^( FUNCTION_BODY $body) ;
	[GrammarRule("functionBody")]
	private JavaScriptParser.functionBody_return functionBody()
	{
		Enter_functionBody();
		EnterRule("functionBody", 7);
		TraceIn("functionBody", 7);
		JavaScriptParser.functionBody_return retval = new JavaScriptParser.functionBody_return();
		retval.Start = (CommonToken)input.LT(1);
		int functionBody_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken char_literal27=null;
		CommonToken SkipSpace28=null;
		CommonToken SkipSpace29=null;
		CommonToken char_literal30=null;
		JavaScriptParser.sourceElements_return body = default(JavaScriptParser.sourceElements_return);

		CommonTree char_literal27_tree=null;
		CommonTree SkipSpace28_tree=null;
		CommonTree SkipSpace29_tree=null;
		CommonTree char_literal30_tree=null;
		RewriteRuleITokenStream stream_147=new RewriteRuleITokenStream(adaptor,"token 147");
		RewriteRuleITokenStream stream_SkipSpace=new RewriteRuleITokenStream(adaptor,"token SkipSpace");
		RewriteRuleITokenStream stream_148=new RewriteRuleITokenStream(adaptor,"token 148");
		RewriteRuleSubtreeStream stream_sourceElements=new RewriteRuleSubtreeStream(adaptor,"rule sourceElements");
		try { DebugEnterRule(GrammarFileName, "functionBody");
		DebugLocation(94, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 7)) { return retval; }
			// JavaScript.g3:95:5: ( '{' ( SkipSpace )* body= sourceElements ( SkipSpace )* '}' -> ^( FUNCTION_BODY $body) )
			DebugEnterAlt(1);
			// JavaScript.g3:95:7: '{' ( SkipSpace )* body= sourceElements ( SkipSpace )* '}'
			{
			DebugLocation(95, 7);
			char_literal27=(CommonToken)Match(input,147,Follow._147_in_functionBody631); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_147.Add(char_literal27);

			DebugLocation(95, 11);
			// JavaScript.g3:95:11: ( SkipSpace )*
			try { DebugEnterSubRule(19);
			while (true)
			{
				int alt19=2;
				try { DebugEnterDecision(19, decisionCanBacktrack[19]);
				int LA19_0 = input.LA(1);

				if ((LA19_0==SkipSpace))
				{
					alt19=1;
				}


				} finally { DebugExitDecision(19); }
				switch ( alt19 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:95:11: SkipSpace
					{
					DebugLocation(95, 11);
					SkipSpace28=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_functionBody633); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace28);


					}
					break;

				default:
					goto loop19;
				}
			}

			loop19:
				;

			} finally { DebugExitSubRule(19); }

			DebugLocation(95, 26);
			PushFollow(Follow._sourceElements_in_functionBody638);
			body=sourceElements();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_sourceElements.Add(body.Tree);
			DebugLocation(95, 42);
			// JavaScript.g3:95:42: ( SkipSpace )*
			try { DebugEnterSubRule(20);
			while (true)
			{
				int alt20=2;
				try { DebugEnterDecision(20, decisionCanBacktrack[20]);
				int LA20_0 = input.LA(1);

				if ((LA20_0==SkipSpace))
				{
					alt20=1;
				}


				} finally { DebugExitDecision(20); }
				switch ( alt20 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:95:42: SkipSpace
					{
					DebugLocation(95, 42);
					SkipSpace29=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_functionBody640); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace29);


					}
					break;

				default:
					goto loop20;
				}
			}

			loop20:
				;

			} finally { DebugExitSubRule(20); }

			DebugLocation(95, 53);
			char_literal30=(CommonToken)Match(input,148,Follow._148_in_functionBody643); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_148.Add(char_literal30);



			{
			// AST REWRITE
			// elements: body
			// token labels: 
			// rule labels: body, retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_body=new RewriteRuleSubtreeStream(adaptor,"rule body",body!=null?body.Tree:null);
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 96:9: -> ^( FUNCTION_BODY $body)
			{
				DebugLocation(96, 12);
				// JavaScript.g3:96:12: ^( FUNCTION_BODY $body)
				{
				CommonTree root_1 = (CommonTree)adaptor.Nil();
				DebugLocation(96, 14);
				root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(FUNCTION_BODY, "FUNCTION_BODY"), root_1);

				DebugLocation(96, 29);
				adaptor.AddChild(root_1, stream_body.NextTree());

				adaptor.AddChild(root_0, root_1);
				}

			}

			retval.Tree = root_0;
			}
			}

			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("functionBody", 7);
			LeaveRule("functionBody", 7);
			Leave_functionBody();
			if (state.backtracking > 0) { Memoize(input, 7, functionBody_StartIndex); }
		}
		DebugLocation(97, 4);
		} finally { DebugExitRule(GrammarFileName, "functionBody"); }
		return retval;

	}
	// $ANTLR end "functionBody"

	public class statement_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_statement();
	partial void Leave_statement();

	// $ANTLR start "statement"
	// JavaScript.g3:100:1: statement : ( statementBlock | variableStatement | emptyStatement | expressionStatement | ifStatement | iterationStatement | continueStatement | breakStatement | returnStatement | withStatement | labelledStatement | switchStatement | throwStatement | tryStatement );
	[GrammarRule("statement")]
	private JavaScriptParser.statement_return statement()
	{
		Enter_statement();
		EnterRule("statement", 8);
		TraceIn("statement", 8);
		JavaScriptParser.statement_return retval = new JavaScriptParser.statement_return();
		retval.Start = (CommonToken)input.LT(1);
		int statement_StartIndex = input.Index;
		CommonTree root_0 = null;

		JavaScriptParser.statementBlock_return statementBlock31 = default(JavaScriptParser.statementBlock_return);
		JavaScriptParser.variableStatement_return variableStatement32 = default(JavaScriptParser.variableStatement_return);
		JavaScriptParser.emptyStatement_return emptyStatement33 = default(JavaScriptParser.emptyStatement_return);
		JavaScriptParser.expressionStatement_return expressionStatement34 = default(JavaScriptParser.expressionStatement_return);
		JavaScriptParser.ifStatement_return ifStatement35 = default(JavaScriptParser.ifStatement_return);
		JavaScriptParser.iterationStatement_return iterationStatement36 = default(JavaScriptParser.iterationStatement_return);
		JavaScriptParser.continueStatement_return continueStatement37 = default(JavaScriptParser.continueStatement_return);
		JavaScriptParser.breakStatement_return breakStatement38 = default(JavaScriptParser.breakStatement_return);
		JavaScriptParser.returnStatement_return returnStatement39 = default(JavaScriptParser.returnStatement_return);
		JavaScriptParser.withStatement_return withStatement40 = default(JavaScriptParser.withStatement_return);
		JavaScriptParser.labelledStatement_return labelledStatement41 = default(JavaScriptParser.labelledStatement_return);
		JavaScriptParser.switchStatement_return switchStatement42 = default(JavaScriptParser.switchStatement_return);
		JavaScriptParser.throwStatement_return throwStatement43 = default(JavaScriptParser.throwStatement_return);
		JavaScriptParser.tryStatement_return tryStatement44 = default(JavaScriptParser.tryStatement_return);


		try { DebugEnterRule(GrammarFileName, "statement");
		DebugLocation(100, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 8)) { return retval; }
			// JavaScript.g3:101:5: ( statementBlock | variableStatement | emptyStatement | expressionStatement | ifStatement | iterationStatement | continueStatement | breakStatement | returnStatement | withStatement | labelledStatement | switchStatement | throwStatement | tryStatement )
			int alt21=14;
			try { DebugEnterDecision(21, decisionCanBacktrack[21]);
			try
			{
				alt21 = dfa21.Predict(input);
			}
			catch (NoViableAltException nvae)
			{
				DebugRecognitionException(nvae);
				throw;
			}
			} finally { DebugExitDecision(21); }
			switch (alt21)
			{
			case 1:
				DebugEnterAlt(1);
				// JavaScript.g3:101:7: statementBlock
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(101, 7);
				PushFollow(Follow._statementBlock_in_statement678);
				statementBlock31=statementBlock();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, statementBlock31.Tree);

				}
				break;
			case 2:
				DebugEnterAlt(2);
				// JavaScript.g3:102:7: variableStatement
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(102, 7);
				PushFollow(Follow._variableStatement_in_statement686);
				variableStatement32=variableStatement();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, variableStatement32.Tree);

				}
				break;
			case 3:
				DebugEnterAlt(3);
				// JavaScript.g3:103:7: emptyStatement
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(103, 7);
				PushFollow(Follow._emptyStatement_in_statement694);
				emptyStatement33=emptyStatement();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, emptyStatement33.Tree);

				}
				break;
			case 4:
				DebugEnterAlt(4);
				// JavaScript.g3:104:7: expressionStatement
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(104, 7);
				PushFollow(Follow._expressionStatement_in_statement702);
				expressionStatement34=expressionStatement();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, expressionStatement34.Tree);

				}
				break;
			case 5:
				DebugEnterAlt(5);
				// JavaScript.g3:105:7: ifStatement
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(105, 7);
				PushFollow(Follow._ifStatement_in_statement710);
				ifStatement35=ifStatement();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, ifStatement35.Tree);

				}
				break;
			case 6:
				DebugEnterAlt(6);
				// JavaScript.g3:106:7: iterationStatement
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(106, 7);
				PushFollow(Follow._iterationStatement_in_statement718);
				iterationStatement36=iterationStatement();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, iterationStatement36.Tree);

				}
				break;
			case 7:
				DebugEnterAlt(7);
				// JavaScript.g3:107:7: continueStatement
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(107, 7);
				PushFollow(Follow._continueStatement_in_statement726);
				continueStatement37=continueStatement();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, continueStatement37.Tree);

				}
				break;
			case 8:
				DebugEnterAlt(8);
				// JavaScript.g3:108:7: breakStatement
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(108, 7);
				PushFollow(Follow._breakStatement_in_statement734);
				breakStatement38=breakStatement();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, breakStatement38.Tree);

				}
				break;
			case 9:
				DebugEnterAlt(9);
				// JavaScript.g3:109:7: returnStatement
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(109, 7);
				PushFollow(Follow._returnStatement_in_statement742);
				returnStatement39=returnStatement();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, returnStatement39.Tree);

				}
				break;
			case 10:
				DebugEnterAlt(10);
				// JavaScript.g3:110:7: withStatement
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(110, 7);
				PushFollow(Follow._withStatement_in_statement750);
				withStatement40=withStatement();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, withStatement40.Tree);

				}
				break;
			case 11:
				DebugEnterAlt(11);
				// JavaScript.g3:111:7: labelledStatement
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(111, 7);
				PushFollow(Follow._labelledStatement_in_statement758);
				labelledStatement41=labelledStatement();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, labelledStatement41.Tree);

				}
				break;
			case 12:
				DebugEnterAlt(12);
				// JavaScript.g3:112:7: switchStatement
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(112, 7);
				PushFollow(Follow._switchStatement_in_statement766);
				switchStatement42=switchStatement();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, switchStatement42.Tree);

				}
				break;
			case 13:
				DebugEnterAlt(13);
				// JavaScript.g3:113:7: throwStatement
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(113, 7);
				PushFollow(Follow._throwStatement_in_statement774);
				throwStatement43=throwStatement();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, throwStatement43.Tree);

				}
				break;
			case 14:
				DebugEnterAlt(14);
				// JavaScript.g3:114:7: tryStatement
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(114, 7);
				PushFollow(Follow._tryStatement_in_statement782);
				tryStatement44=tryStatement();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, tryStatement44.Tree);

				}
				break;

			}
			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("statement", 8);
			LeaveRule("statement", 8);
			Leave_statement();
			if (state.backtracking > 0) { Memoize(input, 8, statement_StartIndex); }
		}
		DebugLocation(115, 4);
		} finally { DebugExitRule(GrammarFileName, "statement"); }
		return retval;

	}
	// $ANTLR end "statement"

	public class statementBlock_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_statementBlock();
	partial void Leave_statementBlock();

	// $ANTLR start "statementBlock"
	// JavaScript.g3:117:1: statementBlock : '{' ( SkipSpace )* ( statementList )? ( SkipSpace )* '}' -> ^( STATEMENT_BLOCK ( statementList )? ) ;
	[GrammarRule("statementBlock")]
	private JavaScriptParser.statementBlock_return statementBlock()
	{
		Enter_statementBlock();
		EnterRule("statementBlock", 9);
		TraceIn("statementBlock", 9);
		JavaScriptParser.statementBlock_return retval = new JavaScriptParser.statementBlock_return();
		retval.Start = (CommonToken)input.LT(1);
		int statementBlock_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken char_literal45=null;
		CommonToken SkipSpace46=null;
		CommonToken SkipSpace48=null;
		CommonToken char_literal49=null;
		JavaScriptParser.statementList_return statementList47 = default(JavaScriptParser.statementList_return);

		CommonTree char_literal45_tree=null;
		CommonTree SkipSpace46_tree=null;
		CommonTree SkipSpace48_tree=null;
		CommonTree char_literal49_tree=null;
		RewriteRuleITokenStream stream_147=new RewriteRuleITokenStream(adaptor,"token 147");
		RewriteRuleITokenStream stream_SkipSpace=new RewriteRuleITokenStream(adaptor,"token SkipSpace");
		RewriteRuleITokenStream stream_148=new RewriteRuleITokenStream(adaptor,"token 148");
		RewriteRuleSubtreeStream stream_statementList=new RewriteRuleSubtreeStream(adaptor,"rule statementList");
		try { DebugEnterRule(GrammarFileName, "statementBlock");
		DebugLocation(117, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 9)) { return retval; }
			// JavaScript.g3:118:5: ( '{' ( SkipSpace )* ( statementList )? ( SkipSpace )* '}' -> ^( STATEMENT_BLOCK ( statementList )? ) )
			DebugEnterAlt(1);
			// JavaScript.g3:118:7: '{' ( SkipSpace )* ( statementList )? ( SkipSpace )* '}'
			{
			DebugLocation(118, 7);
			char_literal45=(CommonToken)Match(input,147,Follow._147_in_statementBlock799); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_147.Add(char_literal45);

			DebugLocation(118, 11);
			// JavaScript.g3:118:11: ( SkipSpace )*
			try { DebugEnterSubRule(22);
			while (true)
			{
				int alt22=2;
				try { DebugEnterDecision(22, decisionCanBacktrack[22]);
				int LA22_0 = input.LA(1);

				if ((LA22_0==SkipSpace))
				{
					int LA22_2 = input.LA(2);

					if ((EvaluatePredicate(synpred34_JavaScript_fragment)))
					{
						alt22=1;
					}


				}


				} finally { DebugExitDecision(22); }
				switch ( alt22 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:118:11: SkipSpace
					{
					DebugLocation(118, 11);
					SkipSpace46=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_statementBlock801); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace46);


					}
					break;

				default:
					goto loop22;
				}
			}

			loop22:
				;

			} finally { DebugExitSubRule(22); }

			DebugLocation(118, 22);
			// JavaScript.g3:118:22: ( statementList )?
			int alt23=2;
			try { DebugEnterSubRule(23);
			try { DebugEnterDecision(23, decisionCanBacktrack[23]);
			int LA23_0 = input.LA(1);

			if ((LA23_0==BIT_NOT||(LA23_0>=BREAK && LA23_0<=BoolLiteral)||LA23_0==CONTINUE||LA23_0==DELETE||LA23_0==DOUBLE_NOT||LA23_0==FALSE||LA23_0==IMPORT_START||LA23_0==Identifier||LA23_0==MINUS||LA23_0==MINUS_INC||LA23_0==NEW||(LA23_0>=NOT && LA23_0<=NULL)||LA23_0==NumericLiteral||LA23_0==PLUS||LA23_0==PLUS_INC||LA23_0==RETURN||LA23_0==SEMI||(LA23_0>=StringLiteral && LA23_0<=THROW)||LA23_0==TRY||LA23_0==TYPE_OF||(LA23_0>=VAR && LA23_0<=VOID)||LA23_0==130||LA23_0==135||LA23_0==139||(LA23_0>=141 && LA23_0<=147)))
			{
				alt23=1;
			}
			} finally { DebugExitDecision(23); }
			switch (alt23)
			{
			case 1:
				DebugEnterAlt(1);
				// JavaScript.g3:118:22: statementList
				{
				DebugLocation(118, 22);
				PushFollow(Follow._statementList_in_statementBlock804);
				statementList47=statementList();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_statementList.Add(statementList47.Tree);

				}
				break;

			}
			} finally { DebugExitSubRule(23); }

			DebugLocation(118, 37);
			// JavaScript.g3:118:37: ( SkipSpace )*
			try { DebugEnterSubRule(24);
			while (true)
			{
				int alt24=2;
				try { DebugEnterDecision(24, decisionCanBacktrack[24]);
				int LA24_0 = input.LA(1);

				if ((LA24_0==SkipSpace))
				{
					alt24=1;
				}


				} finally { DebugExitDecision(24); }
				switch ( alt24 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:118:37: SkipSpace
					{
					DebugLocation(118, 37);
					SkipSpace48=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_statementBlock807); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace48);


					}
					break;

				default:
					goto loop24;
				}
			}

			loop24:
				;

			} finally { DebugExitSubRule(24); }

			DebugLocation(118, 48);
			char_literal49=(CommonToken)Match(input,148,Follow._148_in_statementBlock810); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_148.Add(char_literal49);



			{
			// AST REWRITE
			// elements: statementList
			// token labels: 
			// rule labels: retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 119:9: -> ^( STATEMENT_BLOCK ( statementList )? )
			{
				DebugLocation(119, 12);
				// JavaScript.g3:119:12: ^( STATEMENT_BLOCK ( statementList )? )
				{
				CommonTree root_1 = (CommonTree)adaptor.Nil();
				DebugLocation(119, 14);
				root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(STATEMENT_BLOCK, "STATEMENT_BLOCK"), root_1);

				DebugLocation(119, 30);
				// JavaScript.g3:119:30: ( statementList )?
				if ( stream_statementList.HasNext )
				{
					DebugLocation(119, 30);
					adaptor.AddChild(root_1, stream_statementList.NextTree());

				}
				stream_statementList.Reset();

				adaptor.AddChild(root_0, root_1);
				}

			}

			retval.Tree = root_0;
			}
			}

			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("statementBlock", 9);
			LeaveRule("statementBlock", 9);
			Leave_statementBlock();
			if (state.backtracking > 0) { Memoize(input, 9, statementBlock_StartIndex); }
		}
		DebugLocation(120, 4);
		} finally { DebugExitRule(GrammarFileName, "statementBlock"); }
		return retval;

	}
	// $ANTLR end "statementBlock"

	public class statementList_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_statementList();
	partial void Leave_statementList();

	// $ANTLR start "statementList"
	// JavaScript.g3:122:1: statementList : statement ( ( SkipSpace )* statement )* ;
	[GrammarRule("statementList")]
	private JavaScriptParser.statementList_return statementList()
	{
		Enter_statementList();
		EnterRule("statementList", 10);
		TraceIn("statementList", 10);
		JavaScriptParser.statementList_return retval = new JavaScriptParser.statementList_return();
		retval.Start = (CommonToken)input.LT(1);
		int statementList_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken SkipSpace51=null;
		JavaScriptParser.statement_return statement50 = default(JavaScriptParser.statement_return);
		JavaScriptParser.statement_return statement52 = default(JavaScriptParser.statement_return);

		CommonTree SkipSpace51_tree=null;

		try { DebugEnterRule(GrammarFileName, "statementList");
		DebugLocation(122, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 10)) { return retval; }
			// JavaScript.g3:123:5: ( statement ( ( SkipSpace )* statement )* )
			DebugEnterAlt(1);
			// JavaScript.g3:123:7: statement ( ( SkipSpace )* statement )*
			{
			root_0 = (CommonTree)adaptor.Nil();

			DebugLocation(123, 7);
			PushFollow(Follow._statement_in_statementList844);
			statement50=statement();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) adaptor.AddChild(root_0, statement50.Tree);
			DebugLocation(123, 17);
			// JavaScript.g3:123:17: ( ( SkipSpace )* statement )*
			try { DebugEnterSubRule(26);
			while (true)
			{
				int alt26=2;
				try { DebugEnterDecision(26, decisionCanBacktrack[26]);
				try
				{
					alt26 = dfa26.Predict(input);
				}
				catch (NoViableAltException nvae)
				{
					DebugRecognitionException(nvae);
					throw;
				}
				} finally { DebugExitDecision(26); }
				switch ( alt26 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:123:18: ( SkipSpace )* statement
					{
					DebugLocation(123, 27);
					// JavaScript.g3:123:27: ( SkipSpace )*
					try { DebugEnterSubRule(25);
					while (true)
					{
						int alt25=2;
						try { DebugEnterDecision(25, decisionCanBacktrack[25]);
						int LA25_0 = input.LA(1);

						if ((LA25_0==SkipSpace))
						{
							alt25=1;
						}


						} finally { DebugExitDecision(25); }
						switch ( alt25 )
						{
						case 1:
							DebugEnterAlt(1);
							// JavaScript.g3:123:27: SkipSpace
							{
							DebugLocation(123, 27);
							SkipSpace51=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_statementList847); if (state.failed) return retval;

							}
							break;

						default:
							goto loop25;
						}
					}

					loop25:
						;

					} finally { DebugExitSubRule(25); }

					DebugLocation(123, 30);
					PushFollow(Follow._statement_in_statementList851);
					statement52=statement();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) adaptor.AddChild(root_0, statement52.Tree);

					}
					break;

				default:
					goto loop26;
				}
			}

			loop26:
				;

			} finally { DebugExitSubRule(26); }


			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("statementList", 10);
			LeaveRule("statementList", 10);
			Leave_statementList();
			if (state.backtracking > 0) { Memoize(input, 10, statementList_StartIndex); }
		}
		DebugLocation(124, 4);
		} finally { DebugExitRule(GrammarFileName, "statementList"); }
		return retval;

	}
	// $ANTLR end "statementList"

	public class variableStatement_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_variableStatement();
	partial void Leave_variableStatement();

	// $ANTLR start "variableStatement"
	// JavaScript.g3:126:1: variableStatement : VAR ( SkipSpace )* variableDeclarationList ( SkipSpace )* SEMI ;
	[GrammarRule("variableStatement")]
	private JavaScriptParser.variableStatement_return variableStatement()
	{
		Enter_variableStatement();
		EnterRule("variableStatement", 11);
		TraceIn("variableStatement", 11);
		JavaScriptParser.variableStatement_return retval = new JavaScriptParser.variableStatement_return();
		retval.Start = (CommonToken)input.LT(1);
		int variableStatement_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken VAR53=null;
		CommonToken SkipSpace54=null;
		CommonToken SkipSpace56=null;
		CommonToken SEMI57=null;
		JavaScriptParser.variableDeclarationList_return variableDeclarationList55 = default(JavaScriptParser.variableDeclarationList_return);

		CommonTree VAR53_tree=null;
		CommonTree SkipSpace54_tree=null;
		CommonTree SkipSpace56_tree=null;
		CommonTree SEMI57_tree=null;

		try { DebugEnterRule(GrammarFileName, "variableStatement");
		DebugLocation(126, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 11)) { return retval; }
			// JavaScript.g3:127:5: ( VAR ( SkipSpace )* variableDeclarationList ( SkipSpace )* SEMI )
			DebugEnterAlt(1);
			// JavaScript.g3:127:7: VAR ( SkipSpace )* variableDeclarationList ( SkipSpace )* SEMI
			{
			root_0 = (CommonTree)adaptor.Nil();

			DebugLocation(127, 10);
			VAR53=(CommonToken)Match(input,VAR,Follow._VAR_in_variableStatement870); if (state.failed) return retval;
			if ( state.backtracking == 0 ) {
			VAR53_tree = (CommonTree)adaptor.Create(VAR53);
			root_0 = (CommonTree)adaptor.BecomeRoot(VAR53_tree, root_0);
			}
			DebugLocation(127, 21);
			// JavaScript.g3:127:21: ( SkipSpace )*
			try { DebugEnterSubRule(27);
			while (true)
			{
				int alt27=2;
				try { DebugEnterDecision(27, decisionCanBacktrack[27]);
				int LA27_0 = input.LA(1);

				if ((LA27_0==SkipSpace))
				{
					alt27=1;
				}


				} finally { DebugExitDecision(27); }
				switch ( alt27 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:127:21: SkipSpace
					{
					DebugLocation(127, 21);
					SkipSpace54=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_variableStatement873); if (state.failed) return retval;

					}
					break;

				default:
					goto loop27;
				}
			}

			loop27:
				;

			} finally { DebugExitSubRule(27); }

			DebugLocation(127, 24);
			PushFollow(Follow._variableDeclarationList_in_variableStatement877);
			variableDeclarationList55=variableDeclarationList();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) adaptor.AddChild(root_0, variableDeclarationList55.Tree);
			DebugLocation(127, 57);
			// JavaScript.g3:127:57: ( SkipSpace )*
			try { DebugEnterSubRule(28);
			while (true)
			{
				int alt28=2;
				try { DebugEnterDecision(28, decisionCanBacktrack[28]);
				int LA28_0 = input.LA(1);

				if ((LA28_0==SkipSpace))
				{
					alt28=1;
				}


				} finally { DebugExitDecision(28); }
				switch ( alt28 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:127:57: SkipSpace
					{
					DebugLocation(127, 57);
					SkipSpace56=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_variableStatement879); if (state.failed) return retval;

					}
					break;

				default:
					goto loop28;
				}
			}

			loop28:
				;

			} finally { DebugExitSubRule(28); }

			DebugLocation(127, 64);
			SEMI57=(CommonToken)Match(input,SEMI,Follow._SEMI_in_variableStatement883); if (state.failed) return retval;

			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("variableStatement", 11);
			LeaveRule("variableStatement", 11);
			Leave_variableStatement();
			if (state.backtracking > 0) { Memoize(input, 11, variableStatement_StartIndex); }
		}
		DebugLocation(128, 4);
		} finally { DebugExitRule(GrammarFileName, "variableStatement"); }
		return retval;

	}
	// $ANTLR end "variableStatement"

	public class variableDeclarationList_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_variableDeclarationList();
	partial void Leave_variableDeclarationList();

	// $ANTLR start "variableDeclarationList"
	// JavaScript.g3:130:1: variableDeclarationList : variableDeclaration ( ( SkipSpace )* ',' ( SkipSpace )* variableDeclaration )* -> ( variableDeclaration )+ ;
	[GrammarRule("variableDeclarationList")]
	private JavaScriptParser.variableDeclarationList_return variableDeclarationList()
	{
		Enter_variableDeclarationList();
		EnterRule("variableDeclarationList", 12);
		TraceIn("variableDeclarationList", 12);
		JavaScriptParser.variableDeclarationList_return retval = new JavaScriptParser.variableDeclarationList_return();
		retval.Start = (CommonToken)input.LT(1);
		int variableDeclarationList_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken SkipSpace59=null;
		CommonToken char_literal60=null;
		CommonToken SkipSpace61=null;
		JavaScriptParser.variableDeclaration_return variableDeclaration58 = default(JavaScriptParser.variableDeclaration_return);
		JavaScriptParser.variableDeclaration_return variableDeclaration62 = default(JavaScriptParser.variableDeclaration_return);

		CommonTree SkipSpace59_tree=null;
		CommonTree char_literal60_tree=null;
		CommonTree SkipSpace61_tree=null;
		RewriteRuleITokenStream stream_SkipSpace=new RewriteRuleITokenStream(adaptor,"token SkipSpace");
		RewriteRuleITokenStream stream_COMMA=new RewriteRuleITokenStream(adaptor,"token COMMA");
		RewriteRuleSubtreeStream stream_variableDeclaration=new RewriteRuleSubtreeStream(adaptor,"rule variableDeclaration");
		try { DebugEnterRule(GrammarFileName, "variableDeclarationList");
		DebugLocation(130, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 12)) { return retval; }
			// JavaScript.g3:131:5: ( variableDeclaration ( ( SkipSpace )* ',' ( SkipSpace )* variableDeclaration )* -> ( variableDeclaration )+ )
			DebugEnterAlt(1);
			// JavaScript.g3:131:7: variableDeclaration ( ( SkipSpace )* ',' ( SkipSpace )* variableDeclaration )*
			{
			DebugLocation(131, 7);
			PushFollow(Follow._variableDeclaration_in_variableDeclarationList901);
			variableDeclaration58=variableDeclaration();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_variableDeclaration.Add(variableDeclaration58.Tree);
			DebugLocation(131, 27);
			// JavaScript.g3:131:27: ( ( SkipSpace )* ',' ( SkipSpace )* variableDeclaration )*
			try { DebugEnterSubRule(31);
			while (true)
			{
				int alt31=2;
				try { DebugEnterDecision(31, decisionCanBacktrack[31]);
				try
				{
					alt31 = dfa31.Predict(input);
				}
				catch (NoViableAltException nvae)
				{
					DebugRecognitionException(nvae);
					throw;
				}
				} finally { DebugExitDecision(31); }
				switch ( alt31 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:131:28: ( SkipSpace )* ',' ( SkipSpace )* variableDeclaration
					{
					DebugLocation(131, 28);
					// JavaScript.g3:131:28: ( SkipSpace )*
					try { DebugEnterSubRule(29);
					while (true)
					{
						int alt29=2;
						try { DebugEnterDecision(29, decisionCanBacktrack[29]);
						int LA29_0 = input.LA(1);

						if ((LA29_0==SkipSpace))
						{
							alt29=1;
						}


						} finally { DebugExitDecision(29); }
						switch ( alt29 )
						{
						case 1:
							DebugEnterAlt(1);
							// JavaScript.g3:131:28: SkipSpace
							{
							DebugLocation(131, 28);
							SkipSpace59=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_variableDeclarationList904); if (state.failed) return retval; 
							if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace59);


							}
							break;

						default:
							goto loop29;
						}
					}

					loop29:
						;

					} finally { DebugExitSubRule(29); }

					DebugLocation(131, 39);
					char_literal60=(CommonToken)Match(input,COMMA,Follow._COMMA_in_variableDeclarationList907); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_COMMA.Add(char_literal60);

					DebugLocation(131, 43);
					// JavaScript.g3:131:43: ( SkipSpace )*
					try { DebugEnterSubRule(30);
					while (true)
					{
						int alt30=2;
						try { DebugEnterDecision(30, decisionCanBacktrack[30]);
						int LA30_0 = input.LA(1);

						if ((LA30_0==SkipSpace))
						{
							alt30=1;
						}


						} finally { DebugExitDecision(30); }
						switch ( alt30 )
						{
						case 1:
							DebugEnterAlt(1);
							// JavaScript.g3:131:43: SkipSpace
							{
							DebugLocation(131, 43);
							SkipSpace61=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_variableDeclarationList909); if (state.failed) return retval; 
							if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace61);


							}
							break;

						default:
							goto loop30;
						}
					}

					loop30:
						;

					} finally { DebugExitSubRule(30); }

					DebugLocation(131, 54);
					PushFollow(Follow._variableDeclaration_in_variableDeclarationList912);
					variableDeclaration62=variableDeclaration();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_variableDeclaration.Add(variableDeclaration62.Tree);

					}
					break;

				default:
					goto loop31;
				}
			}

			loop31:
				;

			} finally { DebugExitSubRule(31); }



			{
			// AST REWRITE
			// elements: variableDeclaration
			// token labels: 
			// rule labels: retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 131:76: -> ( variableDeclaration )+
			{
				DebugLocation(131, 79);
				if ( !(stream_variableDeclaration.HasNext) )
				{
					throw new RewriteEarlyExitException();
				}
				while ( stream_variableDeclaration.HasNext )
				{
					DebugLocation(131, 79);
					adaptor.AddChild(root_0, stream_variableDeclaration.NextTree());

				}
				stream_variableDeclaration.Reset();

			}

			retval.Tree = root_0;
			}
			}

			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("variableDeclarationList", 12);
			LeaveRule("variableDeclarationList", 12);
			Leave_variableDeclarationList();
			if (state.backtracking > 0) { Memoize(input, 12, variableDeclarationList_StartIndex); }
		}
		DebugLocation(132, 4);
		} finally { DebugExitRule(GrammarFileName, "variableDeclarationList"); }
		return retval;

	}
	// $ANTLR end "variableDeclarationList"

	public class variableDeclarationListNoIn_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_variableDeclarationListNoIn();
	partial void Leave_variableDeclarationListNoIn();

	// $ANTLR start "variableDeclarationListNoIn"
	// JavaScript.g3:134:1: variableDeclarationListNoIn : variableDeclarationNoIn ( ( SkipSpace )* ',' ( SkipSpace )* variableDeclarationNoIn )* -> ( variableDeclarationNoIn )+ ;
	[GrammarRule("variableDeclarationListNoIn")]
	private JavaScriptParser.variableDeclarationListNoIn_return variableDeclarationListNoIn()
	{
		Enter_variableDeclarationListNoIn();
		EnterRule("variableDeclarationListNoIn", 13);
		TraceIn("variableDeclarationListNoIn", 13);
		JavaScriptParser.variableDeclarationListNoIn_return retval = new JavaScriptParser.variableDeclarationListNoIn_return();
		retval.Start = (CommonToken)input.LT(1);
		int variableDeclarationListNoIn_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken SkipSpace64=null;
		CommonToken char_literal65=null;
		CommonToken SkipSpace66=null;
		JavaScriptParser.variableDeclarationNoIn_return variableDeclarationNoIn63 = default(JavaScriptParser.variableDeclarationNoIn_return);
		JavaScriptParser.variableDeclarationNoIn_return variableDeclarationNoIn67 = default(JavaScriptParser.variableDeclarationNoIn_return);

		CommonTree SkipSpace64_tree=null;
		CommonTree char_literal65_tree=null;
		CommonTree SkipSpace66_tree=null;
		RewriteRuleITokenStream stream_SkipSpace=new RewriteRuleITokenStream(adaptor,"token SkipSpace");
		RewriteRuleITokenStream stream_COMMA=new RewriteRuleITokenStream(adaptor,"token COMMA");
		RewriteRuleSubtreeStream stream_variableDeclarationNoIn=new RewriteRuleSubtreeStream(adaptor,"rule variableDeclarationNoIn");
		try { DebugEnterRule(GrammarFileName, "variableDeclarationListNoIn");
		DebugLocation(134, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 13)) { return retval; }
			// JavaScript.g3:135:5: ( variableDeclarationNoIn ( ( SkipSpace )* ',' ( SkipSpace )* variableDeclarationNoIn )* -> ( variableDeclarationNoIn )+ )
			DebugEnterAlt(1);
			// JavaScript.g3:135:7: variableDeclarationNoIn ( ( SkipSpace )* ',' ( SkipSpace )* variableDeclarationNoIn )*
			{
			DebugLocation(135, 7);
			PushFollow(Follow._variableDeclarationNoIn_in_variableDeclarationListNoIn936);
			variableDeclarationNoIn63=variableDeclarationNoIn();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_variableDeclarationNoIn.Add(variableDeclarationNoIn63.Tree);
			DebugLocation(135, 31);
			// JavaScript.g3:135:31: ( ( SkipSpace )* ',' ( SkipSpace )* variableDeclarationNoIn )*
			try { DebugEnterSubRule(34);
			while (true)
			{
				int alt34=2;
				try { DebugEnterDecision(34, decisionCanBacktrack[34]);
				try
				{
					alt34 = dfa34.Predict(input);
				}
				catch (NoViableAltException nvae)
				{
					DebugRecognitionException(nvae);
					throw;
				}
				} finally { DebugExitDecision(34); }
				switch ( alt34 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:135:32: ( SkipSpace )* ',' ( SkipSpace )* variableDeclarationNoIn
					{
					DebugLocation(135, 32);
					// JavaScript.g3:135:32: ( SkipSpace )*
					try { DebugEnterSubRule(32);
					while (true)
					{
						int alt32=2;
						try { DebugEnterDecision(32, decisionCanBacktrack[32]);
						int LA32_0 = input.LA(1);

						if ((LA32_0==SkipSpace))
						{
							alt32=1;
						}


						} finally { DebugExitDecision(32); }
						switch ( alt32 )
						{
						case 1:
							DebugEnterAlt(1);
							// JavaScript.g3:135:32: SkipSpace
							{
							DebugLocation(135, 32);
							SkipSpace64=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_variableDeclarationListNoIn939); if (state.failed) return retval; 
							if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace64);


							}
							break;

						default:
							goto loop32;
						}
					}

					loop32:
						;

					} finally { DebugExitSubRule(32); }

					DebugLocation(135, 43);
					char_literal65=(CommonToken)Match(input,COMMA,Follow._COMMA_in_variableDeclarationListNoIn942); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_COMMA.Add(char_literal65);

					DebugLocation(135, 47);
					// JavaScript.g3:135:47: ( SkipSpace )*
					try { DebugEnterSubRule(33);
					while (true)
					{
						int alt33=2;
						try { DebugEnterDecision(33, decisionCanBacktrack[33]);
						int LA33_0 = input.LA(1);

						if ((LA33_0==SkipSpace))
						{
							alt33=1;
						}


						} finally { DebugExitDecision(33); }
						switch ( alt33 )
						{
						case 1:
							DebugEnterAlt(1);
							// JavaScript.g3:135:47: SkipSpace
							{
							DebugLocation(135, 47);
							SkipSpace66=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_variableDeclarationListNoIn944); if (state.failed) return retval; 
							if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace66);


							}
							break;

						default:
							goto loop33;
						}
					}

					loop33:
						;

					} finally { DebugExitSubRule(33); }

					DebugLocation(135, 58);
					PushFollow(Follow._variableDeclarationNoIn_in_variableDeclarationListNoIn947);
					variableDeclarationNoIn67=variableDeclarationNoIn();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_variableDeclarationNoIn.Add(variableDeclarationNoIn67.Tree);

					}
					break;

				default:
					goto loop34;
				}
			}

			loop34:
				;

			} finally { DebugExitSubRule(34); }



			{
			// AST REWRITE
			// elements: variableDeclarationNoIn
			// token labels: 
			// rule labels: retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 135:84: -> ( variableDeclarationNoIn )+
			{
				DebugLocation(135, 87);
				if ( !(stream_variableDeclarationNoIn.HasNext) )
				{
					throw new RewriteEarlyExitException();
				}
				while ( stream_variableDeclarationNoIn.HasNext )
				{
					DebugLocation(135, 87);
					adaptor.AddChild(root_0, stream_variableDeclarationNoIn.NextTree());

				}
				stream_variableDeclarationNoIn.Reset();

			}

			retval.Tree = root_0;
			}
			}

			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("variableDeclarationListNoIn", 13);
			LeaveRule("variableDeclarationListNoIn", 13);
			Leave_variableDeclarationListNoIn();
			if (state.backtracking > 0) { Memoize(input, 13, variableDeclarationListNoIn_StartIndex); }
		}
		DebugLocation(136, 4);
		} finally { DebugExitRule(GrammarFileName, "variableDeclarationListNoIn"); }
		return retval;

	}
	// $ANTLR end "variableDeclarationListNoIn"

	public class variableDeclaration_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_variableDeclaration();
	partial void Leave_variableDeclaration();

	// $ANTLR start "variableDeclaration"
	// JavaScript.g3:138:1: variableDeclaration : ( Identifier |lhs= Identifier ( SkipSpace )* op= assignmentOnlyOperator ( SkipSpace )* rhs= assignmentExpression -> ^( BINARY_EXPR $lhs $op $rhs) );
	[GrammarRule("variableDeclaration")]
	private JavaScriptParser.variableDeclaration_return variableDeclaration()
	{
		Enter_variableDeclaration();
		EnterRule("variableDeclaration", 14);
		TraceIn("variableDeclaration", 14);
		JavaScriptParser.variableDeclaration_return retval = new JavaScriptParser.variableDeclaration_return();
		retval.Start = (CommonToken)input.LT(1);
		int variableDeclaration_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken lhs=null;
		CommonToken Identifier68=null;
		CommonToken SkipSpace69=null;
		CommonToken SkipSpace70=null;
		JavaScriptParser.assignmentOnlyOperator_return op = default(JavaScriptParser.assignmentOnlyOperator_return);
		JavaScriptParser.assignmentExpression_return rhs = default(JavaScriptParser.assignmentExpression_return);

		CommonTree lhs_tree=null;
		CommonTree Identifier68_tree=null;
		CommonTree SkipSpace69_tree=null;
		CommonTree SkipSpace70_tree=null;
		RewriteRuleITokenStream stream_Identifier=new RewriteRuleITokenStream(adaptor,"token Identifier");
		RewriteRuleITokenStream stream_SkipSpace=new RewriteRuleITokenStream(adaptor,"token SkipSpace");
		RewriteRuleSubtreeStream stream_assignmentOnlyOperator=new RewriteRuleSubtreeStream(adaptor,"rule assignmentOnlyOperator");
		RewriteRuleSubtreeStream stream_assignmentExpression=new RewriteRuleSubtreeStream(adaptor,"rule assignmentExpression");
		try { DebugEnterRule(GrammarFileName, "variableDeclaration");
		DebugLocation(138, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 14)) { return retval; }
			// JavaScript.g3:139:5: ( Identifier |lhs= Identifier ( SkipSpace )* op= assignmentOnlyOperator ( SkipSpace )* rhs= assignmentExpression -> ^( BINARY_EXPR $lhs $op $rhs) )
			int alt37=2;
			try { DebugEnterDecision(37, decisionCanBacktrack[37]);
			try
			{
				alt37 = dfa37.Predict(input);
			}
			catch (NoViableAltException nvae)
			{
				DebugRecognitionException(nvae);
				throw;
			}
			} finally { DebugExitDecision(37); }
			switch (alt37)
			{
			case 1:
				DebugEnterAlt(1);
				// JavaScript.g3:139:7: Identifier
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(139, 7);
				Identifier68=(CommonToken)Match(input,Identifier,Follow._Identifier_in_variableDeclaration971); if (state.failed) return retval;
				if ( state.backtracking==0 ) {
				Identifier68_tree = (CommonTree)adaptor.Create(Identifier68);
				adaptor.AddChild(root_0, Identifier68_tree);
				}

				}
				break;
			case 2:
				DebugEnterAlt(2);
				// JavaScript.g3:140:7: lhs= Identifier ( SkipSpace )* op= assignmentOnlyOperator ( SkipSpace )* rhs= assignmentExpression
				{
				DebugLocation(140, 10);
				lhs=(CommonToken)Match(input,Identifier,Follow._Identifier_in_variableDeclaration981); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_Identifier.Add(lhs);

				DebugLocation(140, 22);
				// JavaScript.g3:140:22: ( SkipSpace )*
				try { DebugEnterSubRule(35);
				while (true)
				{
					int alt35=2;
					try { DebugEnterDecision(35, decisionCanBacktrack[35]);
					int LA35_0 = input.LA(1);

					if ((LA35_0==SkipSpace))
					{
						alt35=1;
					}


					} finally { DebugExitDecision(35); }
					switch ( alt35 )
					{
					case 1:
						DebugEnterAlt(1);
						// JavaScript.g3:140:22: SkipSpace
						{
						DebugLocation(140, 22);
						SkipSpace69=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_variableDeclaration983); if (state.failed) return retval; 
						if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace69);


						}
						break;

					default:
						goto loop35;
					}
				}

				loop35:
					;

				} finally { DebugExitSubRule(35); }

				DebugLocation(140, 35);
				PushFollow(Follow._assignmentOnlyOperator_in_variableDeclaration988);
				op=assignmentOnlyOperator();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_assignmentOnlyOperator.Add(op.Tree);
				DebugLocation(140, 59);
				// JavaScript.g3:140:59: ( SkipSpace )*
				try { DebugEnterSubRule(36);
				while (true)
				{
					int alt36=2;
					try { DebugEnterDecision(36, decisionCanBacktrack[36]);
					int LA36_0 = input.LA(1);

					if ((LA36_0==SkipSpace))
					{
						alt36=1;
					}


					} finally { DebugExitDecision(36); }
					switch ( alt36 )
					{
					case 1:
						DebugEnterAlt(1);
						// JavaScript.g3:140:59: SkipSpace
						{
						DebugLocation(140, 59);
						SkipSpace70=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_variableDeclaration990); if (state.failed) return retval; 
						if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace70);


						}
						break;

					default:
						goto loop36;
					}
				}

				loop36:
					;

				} finally { DebugExitSubRule(36); }

				DebugLocation(140, 73);
				PushFollow(Follow._assignmentExpression_in_variableDeclaration995);
				rhs=assignmentExpression();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_assignmentExpression.Add(rhs.Tree);


				{
				// AST REWRITE
				// elements: lhs, op, rhs
				// token labels: lhs
				// rule labels: op, rhs, retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.Tree = root_0;
				RewriteRuleITokenStream stream_lhs=new RewriteRuleITokenStream(adaptor,"token lhs",lhs);
				RewriteRuleSubtreeStream stream_op=new RewriteRuleSubtreeStream(adaptor,"rule op",op!=null?op.Tree:null);
				RewriteRuleSubtreeStream stream_rhs=new RewriteRuleSubtreeStream(adaptor,"rule rhs",rhs!=null?rhs.Tree:null);
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

				root_0 = (CommonTree)adaptor.Nil();
				// 141:9: -> ^( BINARY_EXPR $lhs $op $rhs)
				{
					DebugLocation(141, 12);
					// JavaScript.g3:141:12: ^( BINARY_EXPR $lhs $op $rhs)
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(141, 14);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(BINARY_EXPR, "BINARY_EXPR"), root_1);

					DebugLocation(141, 27);
					adaptor.AddChild(root_1, stream_lhs.NextNode());
					DebugLocation(141, 32);
					adaptor.AddChild(root_1, stream_op.NextTree());
					DebugLocation(141, 36);
					adaptor.AddChild(root_1, stream_rhs.NextTree());

					adaptor.AddChild(root_0, root_1);
					}

				}

				retval.Tree = root_0;
				}
				}

				}
				break;

			}
			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("variableDeclaration", 14);
			LeaveRule("variableDeclaration", 14);
			Leave_variableDeclaration();
			if (state.backtracking > 0) { Memoize(input, 14, variableDeclaration_StartIndex); }
		}
		DebugLocation(142, 4);
		} finally { DebugExitRule(GrammarFileName, "variableDeclaration"); }
		return retval;

	}
	// $ANTLR end "variableDeclaration"

	public class variableDeclarationNoIn_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_variableDeclarationNoIn();
	partial void Leave_variableDeclarationNoIn();

	// $ANTLR start "variableDeclarationNoIn"
	// JavaScript.g3:144:1: variableDeclarationNoIn : ( Identifier |lhs= Identifier ( SkipSpace )* op= assignmentOnlyOperator ( SkipSpace )* rhs= assignmentExpressionNoIn -> ^( BINARY_EXPR $lhs $op $rhs) );
	[GrammarRule("variableDeclarationNoIn")]
	private JavaScriptParser.variableDeclarationNoIn_return variableDeclarationNoIn()
	{
		Enter_variableDeclarationNoIn();
		EnterRule("variableDeclarationNoIn", 15);
		TraceIn("variableDeclarationNoIn", 15);
		JavaScriptParser.variableDeclarationNoIn_return retval = new JavaScriptParser.variableDeclarationNoIn_return();
		retval.Start = (CommonToken)input.LT(1);
		int variableDeclarationNoIn_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken lhs=null;
		CommonToken Identifier71=null;
		CommonToken SkipSpace72=null;
		CommonToken SkipSpace73=null;
		JavaScriptParser.assignmentOnlyOperator_return op = default(JavaScriptParser.assignmentOnlyOperator_return);
		JavaScriptParser.assignmentExpressionNoIn_return rhs = default(JavaScriptParser.assignmentExpressionNoIn_return);

		CommonTree lhs_tree=null;
		CommonTree Identifier71_tree=null;
		CommonTree SkipSpace72_tree=null;
		CommonTree SkipSpace73_tree=null;
		RewriteRuleITokenStream stream_Identifier=new RewriteRuleITokenStream(adaptor,"token Identifier");
		RewriteRuleITokenStream stream_SkipSpace=new RewriteRuleITokenStream(adaptor,"token SkipSpace");
		RewriteRuleSubtreeStream stream_assignmentOnlyOperator=new RewriteRuleSubtreeStream(adaptor,"rule assignmentOnlyOperator");
		RewriteRuleSubtreeStream stream_assignmentExpressionNoIn=new RewriteRuleSubtreeStream(adaptor,"rule assignmentExpressionNoIn");
		try { DebugEnterRule(GrammarFileName, "variableDeclarationNoIn");
		DebugLocation(144, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 15)) { return retval; }
			// JavaScript.g3:145:5: ( Identifier |lhs= Identifier ( SkipSpace )* op= assignmentOnlyOperator ( SkipSpace )* rhs= assignmentExpressionNoIn -> ^( BINARY_EXPR $lhs $op $rhs) )
			int alt40=2;
			try { DebugEnterDecision(40, decisionCanBacktrack[40]);
			try
			{
				alt40 = dfa40.Predict(input);
			}
			catch (NoViableAltException nvae)
			{
				DebugRecognitionException(nvae);
				throw;
			}
			} finally { DebugExitDecision(40); }
			switch (alt40)
			{
			case 1:
				DebugEnterAlt(1);
				// JavaScript.g3:145:7: Identifier
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(145, 7);
				Identifier71=(CommonToken)Match(input,Identifier,Follow._Identifier_in_variableDeclarationNoIn1035); if (state.failed) return retval;
				if ( state.backtracking==0 ) {
				Identifier71_tree = (CommonTree)adaptor.Create(Identifier71);
				adaptor.AddChild(root_0, Identifier71_tree);
				}

				}
				break;
			case 2:
				DebugEnterAlt(2);
				// JavaScript.g3:146:7: lhs= Identifier ( SkipSpace )* op= assignmentOnlyOperator ( SkipSpace )* rhs= assignmentExpressionNoIn
				{
				DebugLocation(146, 10);
				lhs=(CommonToken)Match(input,Identifier,Follow._Identifier_in_variableDeclarationNoIn1045); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_Identifier.Add(lhs);

				DebugLocation(146, 22);
				// JavaScript.g3:146:22: ( SkipSpace )*
				try { DebugEnterSubRule(38);
				while (true)
				{
					int alt38=2;
					try { DebugEnterDecision(38, decisionCanBacktrack[38]);
					int LA38_0 = input.LA(1);

					if ((LA38_0==SkipSpace))
					{
						alt38=1;
					}


					} finally { DebugExitDecision(38); }
					switch ( alt38 )
					{
					case 1:
						DebugEnterAlt(1);
						// JavaScript.g3:146:22: SkipSpace
						{
						DebugLocation(146, 22);
						SkipSpace72=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_variableDeclarationNoIn1047); if (state.failed) return retval; 
						if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace72);


						}
						break;

					default:
						goto loop38;
					}
				}

				loop38:
					;

				} finally { DebugExitSubRule(38); }

				DebugLocation(146, 35);
				PushFollow(Follow._assignmentOnlyOperator_in_variableDeclarationNoIn1052);
				op=assignmentOnlyOperator();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_assignmentOnlyOperator.Add(op.Tree);
				DebugLocation(146, 59);
				// JavaScript.g3:146:59: ( SkipSpace )*
				try { DebugEnterSubRule(39);
				while (true)
				{
					int alt39=2;
					try { DebugEnterDecision(39, decisionCanBacktrack[39]);
					int LA39_0 = input.LA(1);

					if ((LA39_0==SkipSpace))
					{
						alt39=1;
					}


					} finally { DebugExitDecision(39); }
					switch ( alt39 )
					{
					case 1:
						DebugEnterAlt(1);
						// JavaScript.g3:146:59: SkipSpace
						{
						DebugLocation(146, 59);
						SkipSpace73=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_variableDeclarationNoIn1054); if (state.failed) return retval; 
						if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace73);


						}
						break;

					default:
						goto loop39;
					}
				}

				loop39:
					;

				} finally { DebugExitSubRule(39); }

				DebugLocation(146, 73);
				PushFollow(Follow._assignmentExpressionNoIn_in_variableDeclarationNoIn1059);
				rhs=assignmentExpressionNoIn();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_assignmentExpressionNoIn.Add(rhs.Tree);


				{
				// AST REWRITE
				// elements: lhs, op, rhs
				// token labels: lhs
				// rule labels: op, rhs, retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.Tree = root_0;
				RewriteRuleITokenStream stream_lhs=new RewriteRuleITokenStream(adaptor,"token lhs",lhs);
				RewriteRuleSubtreeStream stream_op=new RewriteRuleSubtreeStream(adaptor,"rule op",op!=null?op.Tree:null);
				RewriteRuleSubtreeStream stream_rhs=new RewriteRuleSubtreeStream(adaptor,"rule rhs",rhs!=null?rhs.Tree:null);
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

				root_0 = (CommonTree)adaptor.Nil();
				// 147:9: -> ^( BINARY_EXPR $lhs $op $rhs)
				{
					DebugLocation(147, 12);
					// JavaScript.g3:147:12: ^( BINARY_EXPR $lhs $op $rhs)
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(147, 14);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(BINARY_EXPR, "BINARY_EXPR"), root_1);

					DebugLocation(147, 27);
					adaptor.AddChild(root_1, stream_lhs.NextNode());
					DebugLocation(147, 32);
					adaptor.AddChild(root_1, stream_op.NextTree());
					DebugLocation(147, 36);
					adaptor.AddChild(root_1, stream_rhs.NextTree());

					adaptor.AddChild(root_0, root_1);
					}

				}

				retval.Tree = root_0;
				}
				}

				}
				break;

			}
			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("variableDeclarationNoIn", 15);
			LeaveRule("variableDeclarationNoIn", 15);
			Leave_variableDeclarationNoIn();
			if (state.backtracking > 0) { Memoize(input, 15, variableDeclarationNoIn_StartIndex); }
		}
		DebugLocation(148, 4);
		} finally { DebugExitRule(GrammarFileName, "variableDeclarationNoIn"); }
		return retval;

	}
	// $ANTLR end "variableDeclarationNoIn"

	public class assignmentOnlyOperator_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_assignmentOnlyOperator();
	partial void Leave_assignmentOnlyOperator();

	// $ANTLR start "assignmentOnlyOperator"
	// JavaScript.g3:150:1: assignmentOnlyOperator : ASN ;
	[GrammarRule("assignmentOnlyOperator")]
	private JavaScriptParser.assignmentOnlyOperator_return assignmentOnlyOperator()
	{
		Enter_assignmentOnlyOperator();
		EnterRule("assignmentOnlyOperator", 16);
		TraceIn("assignmentOnlyOperator", 16);
		JavaScriptParser.assignmentOnlyOperator_return retval = new JavaScriptParser.assignmentOnlyOperator_return();
		retval.Start = (CommonToken)input.LT(1);
		int assignmentOnlyOperator_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken ASN74=null;

		CommonTree ASN74_tree=null;

		try { DebugEnterRule(GrammarFileName, "assignmentOnlyOperator");
		DebugLocation(150, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 16)) { return retval; }
			// JavaScript.g3:151:5: ( ASN )
			DebugEnterAlt(1);
			// JavaScript.g3:151:7: ASN
			{
			root_0 = (CommonTree)adaptor.Nil();

			DebugLocation(151, 7);
			ASN74=(CommonToken)Match(input,ASN,Follow._ASN_in_assignmentOnlyOperator1099); if (state.failed) return retval;
			if ( state.backtracking==0 ) {
			ASN74_tree = (CommonTree)adaptor.Create(ASN74);
			adaptor.AddChild(root_0, ASN74_tree);
			}

			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("assignmentOnlyOperator", 16);
			LeaveRule("assignmentOnlyOperator", 16);
			Leave_assignmentOnlyOperator();
			if (state.backtracking > 0) { Memoize(input, 16, assignmentOnlyOperator_StartIndex); }
		}
		DebugLocation(152, 4);
		} finally { DebugExitRule(GrammarFileName, "assignmentOnlyOperator"); }
		return retval;

	}
	// $ANTLR end "assignmentOnlyOperator"

	public class emptyStatement_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_emptyStatement();
	partial void Leave_emptyStatement();

	// $ANTLR start "emptyStatement"
	// JavaScript.g3:154:1: emptyStatement : SEMI -> ^( EMPTY_STATEMENT SEMI ) ;
	[GrammarRule("emptyStatement")]
	private JavaScriptParser.emptyStatement_return emptyStatement()
	{
		Enter_emptyStatement();
		EnterRule("emptyStatement", 17);
		TraceIn("emptyStatement", 17);
		JavaScriptParser.emptyStatement_return retval = new JavaScriptParser.emptyStatement_return();
		retval.Start = (CommonToken)input.LT(1);
		int emptyStatement_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken SEMI75=null;

		CommonTree SEMI75_tree=null;
		RewriteRuleITokenStream stream_SEMI=new RewriteRuleITokenStream(adaptor,"token SEMI");

		try { DebugEnterRule(GrammarFileName, "emptyStatement");
		DebugLocation(154, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 17)) { return retval; }
			// JavaScript.g3:155:5: ( SEMI -> ^( EMPTY_STATEMENT SEMI ) )
			DebugEnterAlt(1);
			// JavaScript.g3:155:7: SEMI
			{
			DebugLocation(155, 7);
			SEMI75=(CommonToken)Match(input,SEMI,Follow._SEMI_in_emptyStatement1116); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_SEMI.Add(SEMI75);



			{
			// AST REWRITE
			// elements: SEMI
			// token labels: 
			// rule labels: retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 156:9: -> ^( EMPTY_STATEMENT SEMI )
			{
				DebugLocation(156, 12);
				// JavaScript.g3:156:12: ^( EMPTY_STATEMENT SEMI )
				{
				CommonTree root_1 = (CommonTree)adaptor.Nil();
				DebugLocation(156, 14);
				root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(EMPTY_STATEMENT, "EMPTY_STATEMENT"), root_1);

				DebugLocation(156, 30);
				adaptor.AddChild(root_1, stream_SEMI.NextNode());

				adaptor.AddChild(root_0, root_1);
				}

			}

			retval.Tree = root_0;
			}
			}

			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("emptyStatement", 17);
			LeaveRule("emptyStatement", 17);
			Leave_emptyStatement();
			if (state.backtracking > 0) { Memoize(input, 17, emptyStatement_StartIndex); }
		}
		DebugLocation(157, 4);
		} finally { DebugExitRule(GrammarFileName, "emptyStatement"); }
		return retval;

	}
	// $ANTLR end "emptyStatement"

	public class expressionStatement_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_expressionStatement();
	partial void Leave_expressionStatement();

	// $ANTLR start "expressionStatement"
	// JavaScript.g3:159:1: expressionStatement : expression ( SkipSpace )* SEMI ;
	[GrammarRule("expressionStatement")]
	private JavaScriptParser.expressionStatement_return expressionStatement()
	{
		Enter_expressionStatement();
		EnterRule("expressionStatement", 18);
		TraceIn("expressionStatement", 18);
		JavaScriptParser.expressionStatement_return retval = new JavaScriptParser.expressionStatement_return();
		retval.Start = (CommonToken)input.LT(1);
		int expressionStatement_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken SkipSpace77=null;
		CommonToken SEMI78=null;
		JavaScriptParser.expression_return expression76 = default(JavaScriptParser.expression_return);

		CommonTree SkipSpace77_tree=null;
		CommonTree SEMI78_tree=null;

		try { DebugEnterRule(GrammarFileName, "expressionStatement");
		DebugLocation(159, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 18)) { return retval; }
			// JavaScript.g3:160:5: ( expression ( SkipSpace )* SEMI )
			DebugEnterAlt(1);
			// JavaScript.g3:160:7: expression ( SkipSpace )* SEMI
			{
			root_0 = (CommonTree)adaptor.Nil();

			DebugLocation(160, 7);
			PushFollow(Follow._expression_in_expressionStatement1149);
			expression76=expression();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) adaptor.AddChild(root_0, expression76.Tree);
			DebugLocation(160, 27);
			// JavaScript.g3:160:27: ( SkipSpace )*
			try { DebugEnterSubRule(41);
			while (true)
			{
				int alt41=2;
				try { DebugEnterDecision(41, decisionCanBacktrack[41]);
				int LA41_0 = input.LA(1);

				if ((LA41_0==SkipSpace))
				{
					alt41=1;
				}


				} finally { DebugExitDecision(41); }
				switch ( alt41 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:160:27: SkipSpace
					{
					DebugLocation(160, 27);
					SkipSpace77=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_expressionStatement1151); if (state.failed) return retval;

					}
					break;

				default:
					goto loop41;
				}
			}

			loop41:
				;

			} finally { DebugExitSubRule(41); }

			DebugLocation(160, 34);
			SEMI78=(CommonToken)Match(input,SEMI,Follow._SEMI_in_expressionStatement1155); if (state.failed) return retval;

			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("expressionStatement", 18);
			LeaveRule("expressionStatement", 18);
			Leave_expressionStatement();
			if (state.backtracking > 0) { Memoize(input, 18, expressionStatement_StartIndex); }
		}
		DebugLocation(161, 4);
		} finally { DebugExitRule(GrammarFileName, "expressionStatement"); }
		return retval;

	}
	// $ANTLR end "expressionStatement"

	public class ifStatement_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_ifStatement();
	partial void Leave_ifStatement();

	// $ANTLR start "ifStatement"
	// JavaScript.g3:163:1: ifStatement : 'if' ( SkipSpace )* '(' ( SkipSpace )* cond= expression ( SkipSpace )* ')' ( SkipSpace )* ifs= statement ( ( SkipSpace )* 'else' ( SkipSpace )* els= statement )? -> ^( IF_BLOCK $cond $ifs ( $els)? ) ;
	[GrammarRule("ifStatement")]
	private JavaScriptParser.ifStatement_return ifStatement()
	{
		Enter_ifStatement();
		EnterRule("ifStatement", 19);
		TraceIn("ifStatement", 19);
		JavaScriptParser.ifStatement_return retval = new JavaScriptParser.ifStatement_return();
		retval.Start = (CommonToken)input.LT(1);
		int ifStatement_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken string_literal79=null;
		CommonToken SkipSpace80=null;
		CommonToken char_literal81=null;
		CommonToken SkipSpace82=null;
		CommonToken SkipSpace83=null;
		CommonToken char_literal84=null;
		CommonToken SkipSpace85=null;
		CommonToken SkipSpace86=null;
		CommonToken string_literal87=null;
		CommonToken SkipSpace88=null;
		JavaScriptParser.expression_return cond = default(JavaScriptParser.expression_return);
		JavaScriptParser.statement_return ifs = default(JavaScriptParser.statement_return);
		JavaScriptParser.statement_return els = default(JavaScriptParser.statement_return);

		CommonTree string_literal79_tree=null;
		CommonTree SkipSpace80_tree=null;
		CommonTree char_literal81_tree=null;
		CommonTree SkipSpace82_tree=null;
		CommonTree SkipSpace83_tree=null;
		CommonTree char_literal84_tree=null;
		CommonTree SkipSpace85_tree=null;
		CommonTree SkipSpace86_tree=null;
		CommonTree string_literal87_tree=null;
		CommonTree SkipSpace88_tree=null;
		RewriteRuleITokenStream stream_143=new RewriteRuleITokenStream(adaptor,"token 143");
		RewriteRuleITokenStream stream_SkipSpace=new RewriteRuleITokenStream(adaptor,"token SkipSpace");
		RewriteRuleITokenStream stream_130=new RewriteRuleITokenStream(adaptor,"token 130");
		RewriteRuleITokenStream stream_131=new RewriteRuleITokenStream(adaptor,"token 131");
		RewriteRuleITokenStream stream_140=new RewriteRuleITokenStream(adaptor,"token 140");
		RewriteRuleSubtreeStream stream_expression=new RewriteRuleSubtreeStream(adaptor,"rule expression");
		RewriteRuleSubtreeStream stream_statement=new RewriteRuleSubtreeStream(adaptor,"rule statement");
		try { DebugEnterRule(GrammarFileName, "ifStatement");
		DebugLocation(163, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 19)) { return retval; }
			// JavaScript.g3:164:5: ( 'if' ( SkipSpace )* '(' ( SkipSpace )* cond= expression ( SkipSpace )* ')' ( SkipSpace )* ifs= statement ( ( SkipSpace )* 'else' ( SkipSpace )* els= statement )? -> ^( IF_BLOCK $cond $ifs ( $els)? ) )
			DebugEnterAlt(1);
			// JavaScript.g3:164:7: 'if' ( SkipSpace )* '(' ( SkipSpace )* cond= expression ( SkipSpace )* ')' ( SkipSpace )* ifs= statement ( ( SkipSpace )* 'else' ( SkipSpace )* els= statement )?
			{
			DebugLocation(164, 7);
			string_literal79=(CommonToken)Match(input,143,Follow._143_in_ifStatement1173); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_143.Add(string_literal79);

			DebugLocation(164, 12);
			// JavaScript.g3:164:12: ( SkipSpace )*
			try { DebugEnterSubRule(42);
			while (true)
			{
				int alt42=2;
				try { DebugEnterDecision(42, decisionCanBacktrack[42]);
				int LA42_0 = input.LA(1);

				if ((LA42_0==SkipSpace))
				{
					alt42=1;
				}


				} finally { DebugExitDecision(42); }
				switch ( alt42 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:164:12: SkipSpace
					{
					DebugLocation(164, 12);
					SkipSpace80=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_ifStatement1175); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace80);


					}
					break;

				default:
					goto loop42;
				}
			}

			loop42:
				;

			} finally { DebugExitSubRule(42); }

			DebugLocation(164, 23);
			char_literal81=(CommonToken)Match(input,130,Follow._130_in_ifStatement1178); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_130.Add(char_literal81);

			DebugLocation(164, 27);
			// JavaScript.g3:164:27: ( SkipSpace )*
			try { DebugEnterSubRule(43);
			while (true)
			{
				int alt43=2;
				try { DebugEnterDecision(43, decisionCanBacktrack[43]);
				int LA43_0 = input.LA(1);

				if ((LA43_0==SkipSpace))
				{
					alt43=1;
				}


				} finally { DebugExitDecision(43); }
				switch ( alt43 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:164:27: SkipSpace
					{
					DebugLocation(164, 27);
					SkipSpace82=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_ifStatement1180); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace82);


					}
					break;

				default:
					goto loop43;
				}
			}

			loop43:
				;

			} finally { DebugExitSubRule(43); }

			DebugLocation(164, 42);
			PushFollow(Follow._expression_in_ifStatement1185);
			cond=expression();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_expression.Add(cond.Tree);
			DebugLocation(164, 54);
			// JavaScript.g3:164:54: ( SkipSpace )*
			try { DebugEnterSubRule(44);
			while (true)
			{
				int alt44=2;
				try { DebugEnterDecision(44, decisionCanBacktrack[44]);
				int LA44_0 = input.LA(1);

				if ((LA44_0==SkipSpace))
				{
					alt44=1;
				}


				} finally { DebugExitDecision(44); }
				switch ( alt44 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:164:54: SkipSpace
					{
					DebugLocation(164, 54);
					SkipSpace83=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_ifStatement1187); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace83);


					}
					break;

				default:
					goto loop44;
				}
			}

			loop44:
				;

			} finally { DebugExitSubRule(44); }

			DebugLocation(164, 65);
			char_literal84=(CommonToken)Match(input,131,Follow._131_in_ifStatement1190); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_131.Add(char_literal84);

			DebugLocation(164, 69);
			// JavaScript.g3:164:69: ( SkipSpace )*
			try { DebugEnterSubRule(45);
			while (true)
			{
				int alt45=2;
				try { DebugEnterDecision(45, decisionCanBacktrack[45]);
				int LA45_0 = input.LA(1);

				if ((LA45_0==SkipSpace))
				{
					alt45=1;
				}


				} finally { DebugExitDecision(45); }
				switch ( alt45 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:164:69: SkipSpace
					{
					DebugLocation(164, 69);
					SkipSpace85=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_ifStatement1192); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace85);


					}
					break;

				default:
					goto loop45;
				}
			}

			loop45:
				;

			} finally { DebugExitSubRule(45); }

			DebugLocation(164, 83);
			PushFollow(Follow._statement_in_ifStatement1197);
			ifs=statement();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_statement.Add(ifs.Tree);
			DebugLocation(164, 94);
			// JavaScript.g3:164:94: ( ( SkipSpace )* 'else' ( SkipSpace )* els= statement )?
			int alt48=2;
			try { DebugEnterSubRule(48);
			try { DebugEnterDecision(48, decisionCanBacktrack[48]);
			int LA48_0 = input.LA(1);

			if ((LA48_0==SkipSpace))
			{
				int LA48_1 = input.LA(2);

				if ((EvaluatePredicate(synpred60_JavaScript_fragment)))
				{
					alt48=1;
				}
			}
			else if ((LA48_0==140))
			{
				int LA48_2 = input.LA(2);

				if ((EvaluatePredicate(synpred60_JavaScript_fragment)))
				{
					alt48=1;
				}
			}
			} finally { DebugExitDecision(48); }
			switch (alt48)
			{
			case 1:
				DebugEnterAlt(1);
				// JavaScript.g3:164:95: ( SkipSpace )* 'else' ( SkipSpace )* els= statement
				{
				DebugLocation(164, 95);
				// JavaScript.g3:164:95: ( SkipSpace )*
				try { DebugEnterSubRule(46);
				while (true)
				{
					int alt46=2;
					try { DebugEnterDecision(46, decisionCanBacktrack[46]);
					int LA46_0 = input.LA(1);

					if ((LA46_0==SkipSpace))
					{
						alt46=1;
					}


					} finally { DebugExitDecision(46); }
					switch ( alt46 )
					{
					case 1:
						DebugEnterAlt(1);
						// JavaScript.g3:164:95: SkipSpace
						{
						DebugLocation(164, 95);
						SkipSpace86=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_ifStatement1200); if (state.failed) return retval; 
						if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace86);


						}
						break;

					default:
						goto loop46;
					}
				}

				loop46:
					;

				} finally { DebugExitSubRule(46); }

				DebugLocation(164, 106);
				string_literal87=(CommonToken)Match(input,140,Follow._140_in_ifStatement1203); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_140.Add(string_literal87);

				DebugLocation(164, 113);
				// JavaScript.g3:164:113: ( SkipSpace )*
				try { DebugEnterSubRule(47);
				while (true)
				{
					int alt47=2;
					try { DebugEnterDecision(47, decisionCanBacktrack[47]);
					int LA47_0 = input.LA(1);

					if ((LA47_0==SkipSpace))
					{
						alt47=1;
					}


					} finally { DebugExitDecision(47); }
					switch ( alt47 )
					{
					case 1:
						DebugEnterAlt(1);
						// JavaScript.g3:164:113: SkipSpace
						{
						DebugLocation(164, 113);
						SkipSpace88=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_ifStatement1205); if (state.failed) return retval; 
						if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace88);


						}
						break;

					default:
						goto loop47;
					}
				}

				loop47:
					;

				} finally { DebugExitSubRule(47); }

				DebugLocation(164, 127);
				PushFollow(Follow._statement_in_ifStatement1210);
				els=statement();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_statement.Add(els.Tree);

				}
				break;

			}
			} finally { DebugExitSubRule(48); }



			{
			// AST REWRITE
			// elements: cond, ifs, els
			// token labels: 
			// rule labels: cond, ifs, els, retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_cond=new RewriteRuleSubtreeStream(adaptor,"rule cond",cond!=null?cond.Tree:null);
			RewriteRuleSubtreeStream stream_ifs=new RewriteRuleSubtreeStream(adaptor,"rule ifs",ifs!=null?ifs.Tree:null);
			RewriteRuleSubtreeStream stream_els=new RewriteRuleSubtreeStream(adaptor,"rule els",els!=null?els.Tree:null);
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 165:9: -> ^( IF_BLOCK $cond $ifs ( $els)? )
			{
				DebugLocation(165, 12);
				// JavaScript.g3:165:12: ^( IF_BLOCK $cond $ifs ( $els)? )
				{
				CommonTree root_1 = (CommonTree)adaptor.Nil();
				DebugLocation(165, 14);
				root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(IF_BLOCK, "IF_BLOCK"), root_1);

				DebugLocation(165, 24);
				adaptor.AddChild(root_1, stream_cond.NextTree());
				DebugLocation(165, 30);
				adaptor.AddChild(root_1, stream_ifs.NextTree());
				DebugLocation(165, 35);
				// JavaScript.g3:165:35: ( $els)?
				if ( stream_els.HasNext )
				{
					DebugLocation(165, 35);
					adaptor.AddChild(root_1, stream_els.NextTree());

				}
				stream_els.Reset();

				adaptor.AddChild(root_0, root_1);
				}

			}

			retval.Tree = root_0;
			}
			}

			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("ifStatement", 19);
			LeaveRule("ifStatement", 19);
			Leave_ifStatement();
			if (state.backtracking > 0) { Memoize(input, 19, ifStatement_StartIndex); }
		}
		DebugLocation(166, 4);
		} finally { DebugExitRule(GrammarFileName, "ifStatement"); }
		return retval;

	}
	// $ANTLR end "ifStatement"

	public class iterationStatement_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_iterationStatement();
	partial void Leave_iterationStatement();

	// $ANTLR start "iterationStatement"
	// JavaScript.g3:168:1: iterationStatement : ( doWhileStatement | whileStatement | forStatement | forInStatement );
	[GrammarRule("iterationStatement")]
	private JavaScriptParser.iterationStatement_return iterationStatement()
	{
		Enter_iterationStatement();
		EnterRule("iterationStatement", 20);
		TraceIn("iterationStatement", 20);
		JavaScriptParser.iterationStatement_return retval = new JavaScriptParser.iterationStatement_return();
		retval.Start = (CommonToken)input.LT(1);
		int iterationStatement_StartIndex = input.Index;
		CommonTree root_0 = null;

		JavaScriptParser.doWhileStatement_return doWhileStatement89 = default(JavaScriptParser.doWhileStatement_return);
		JavaScriptParser.whileStatement_return whileStatement90 = default(JavaScriptParser.whileStatement_return);
		JavaScriptParser.forStatement_return forStatement91 = default(JavaScriptParser.forStatement_return);
		JavaScriptParser.forInStatement_return forInStatement92 = default(JavaScriptParser.forInStatement_return);


		try { DebugEnterRule(GrammarFileName, "iterationStatement");
		DebugLocation(168, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 20)) { return retval; }
			// JavaScript.g3:169:5: ( doWhileStatement | whileStatement | forStatement | forInStatement )
			int alt49=4;
			try { DebugEnterDecision(49, decisionCanBacktrack[49]);
			switch (input.LA(1))
			{
			case 139:
				{
				alt49=1;
				}
				break;
			case 145:
				{
				alt49=2;
				}
				break;
			case 141:
				{
				int LA49_3 = input.LA(2);

				if ((EvaluatePredicate(synpred63_JavaScript_fragment)))
				{
					alt49=3;
				}
				else if ((true))
				{
					alt49=4;
				}
				else
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 49, 3, input);

					DebugRecognitionException(nvae);
					throw nvae;
				}
				}
				break;
			default:
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 49, 0, input);

					DebugRecognitionException(nvae);
					throw nvae;
				}
			}

			} finally { DebugExitDecision(49); }
			switch (alt49)
			{
			case 1:
				DebugEnterAlt(1);
				// JavaScript.g3:169:7: doWhileStatement
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(169, 7);
				PushFollow(Follow._doWhileStatement_in_iterationStatement1253);
				doWhileStatement89=doWhileStatement();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, doWhileStatement89.Tree);

				}
				break;
			case 2:
				DebugEnterAlt(2);
				// JavaScript.g3:170:7: whileStatement
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(170, 7);
				PushFollow(Follow._whileStatement_in_iterationStatement1261);
				whileStatement90=whileStatement();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, whileStatement90.Tree);

				}
				break;
			case 3:
				DebugEnterAlt(3);
				// JavaScript.g3:171:7: forStatement
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(171, 7);
				PushFollow(Follow._forStatement_in_iterationStatement1269);
				forStatement91=forStatement();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, forStatement91.Tree);

				}
				break;
			case 4:
				DebugEnterAlt(4);
				// JavaScript.g3:172:7: forInStatement
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(172, 7);
				PushFollow(Follow._forInStatement_in_iterationStatement1277);
				forInStatement92=forInStatement();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, forInStatement92.Tree);

				}
				break;

			}
			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("iterationStatement", 20);
			LeaveRule("iterationStatement", 20);
			Leave_iterationStatement();
			if (state.backtracking > 0) { Memoize(input, 20, iterationStatement_StartIndex); }
		}
		DebugLocation(173, 4);
		} finally { DebugExitRule(GrammarFileName, "iterationStatement"); }
		return retval;

	}
	// $ANTLR end "iterationStatement"

	public class doWhileStatement_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_doWhileStatement();
	partial void Leave_doWhileStatement();

	// $ANTLR start "doWhileStatement"
	// JavaScript.g3:175:1: doWhileStatement : 'do' ( SkipSpace )* blk= statement ( SkipSpace )* 'while' ( SkipSpace )* '(' cond= expression ')' ( SkipSpace )* SEMI -> ^( DO_BLOCK $cond $blk) ;
	[GrammarRule("doWhileStatement")]
	private JavaScriptParser.doWhileStatement_return doWhileStatement()
	{
		Enter_doWhileStatement();
		EnterRule("doWhileStatement", 21);
		TraceIn("doWhileStatement", 21);
		JavaScriptParser.doWhileStatement_return retval = new JavaScriptParser.doWhileStatement_return();
		retval.Start = (CommonToken)input.LT(1);
		int doWhileStatement_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken string_literal93=null;
		CommonToken SkipSpace94=null;
		CommonToken SkipSpace95=null;
		CommonToken string_literal96=null;
		CommonToken SkipSpace97=null;
		CommonToken char_literal98=null;
		CommonToken char_literal99=null;
		CommonToken SkipSpace100=null;
		CommonToken SEMI101=null;
		JavaScriptParser.statement_return blk = default(JavaScriptParser.statement_return);
		JavaScriptParser.expression_return cond = default(JavaScriptParser.expression_return);

		CommonTree string_literal93_tree=null;
		CommonTree SkipSpace94_tree=null;
		CommonTree SkipSpace95_tree=null;
		CommonTree string_literal96_tree=null;
		CommonTree SkipSpace97_tree=null;
		CommonTree char_literal98_tree=null;
		CommonTree char_literal99_tree=null;
		CommonTree SkipSpace100_tree=null;
		CommonTree SEMI101_tree=null;
		RewriteRuleITokenStream stream_139=new RewriteRuleITokenStream(adaptor,"token 139");
		RewriteRuleITokenStream stream_SkipSpace=new RewriteRuleITokenStream(adaptor,"token SkipSpace");
		RewriteRuleITokenStream stream_145=new RewriteRuleITokenStream(adaptor,"token 145");
		RewriteRuleITokenStream stream_130=new RewriteRuleITokenStream(adaptor,"token 130");
		RewriteRuleITokenStream stream_131=new RewriteRuleITokenStream(adaptor,"token 131");
		RewriteRuleITokenStream stream_SEMI=new RewriteRuleITokenStream(adaptor,"token SEMI");
		RewriteRuleSubtreeStream stream_statement=new RewriteRuleSubtreeStream(adaptor,"rule statement");
		RewriteRuleSubtreeStream stream_expression=new RewriteRuleSubtreeStream(adaptor,"rule expression");
		try { DebugEnterRule(GrammarFileName, "doWhileStatement");
		DebugLocation(175, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 21)) { return retval; }
			// JavaScript.g3:176:5: ( 'do' ( SkipSpace )* blk= statement ( SkipSpace )* 'while' ( SkipSpace )* '(' cond= expression ')' ( SkipSpace )* SEMI -> ^( DO_BLOCK $cond $blk) )
			DebugEnterAlt(1);
			// JavaScript.g3:176:7: 'do' ( SkipSpace )* blk= statement ( SkipSpace )* 'while' ( SkipSpace )* '(' cond= expression ')' ( SkipSpace )* SEMI
			{
			DebugLocation(176, 7);
			string_literal93=(CommonToken)Match(input,139,Follow._139_in_doWhileStatement1294); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_139.Add(string_literal93);

			DebugLocation(176, 12);
			// JavaScript.g3:176:12: ( SkipSpace )*
			try { DebugEnterSubRule(50);
			while (true)
			{
				int alt50=2;
				try { DebugEnterDecision(50, decisionCanBacktrack[50]);
				int LA50_0 = input.LA(1);

				if ((LA50_0==SkipSpace))
				{
					alt50=1;
				}


				} finally { DebugExitDecision(50); }
				switch ( alt50 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:176:12: SkipSpace
					{
					DebugLocation(176, 12);
					SkipSpace94=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_doWhileStatement1296); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace94);


					}
					break;

				default:
					goto loop50;
				}
			}

			loop50:
				;

			} finally { DebugExitSubRule(50); }

			DebugLocation(176, 26);
			PushFollow(Follow._statement_in_doWhileStatement1301);
			blk=statement();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_statement.Add(blk.Tree);
			DebugLocation(176, 37);
			// JavaScript.g3:176:37: ( SkipSpace )*
			try { DebugEnterSubRule(51);
			while (true)
			{
				int alt51=2;
				try { DebugEnterDecision(51, decisionCanBacktrack[51]);
				int LA51_0 = input.LA(1);

				if ((LA51_0==SkipSpace))
				{
					alt51=1;
				}


				} finally { DebugExitDecision(51); }
				switch ( alt51 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:176:37: SkipSpace
					{
					DebugLocation(176, 37);
					SkipSpace95=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_doWhileStatement1303); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace95);


					}
					break;

				default:
					goto loop51;
				}
			}

			loop51:
				;

			} finally { DebugExitSubRule(51); }

			DebugLocation(176, 48);
			string_literal96=(CommonToken)Match(input,145,Follow._145_in_doWhileStatement1306); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_145.Add(string_literal96);

			DebugLocation(176, 56);
			// JavaScript.g3:176:56: ( SkipSpace )*
			try { DebugEnterSubRule(52);
			while (true)
			{
				int alt52=2;
				try { DebugEnterDecision(52, decisionCanBacktrack[52]);
				int LA52_0 = input.LA(1);

				if ((LA52_0==SkipSpace))
				{
					alt52=1;
				}


				} finally { DebugExitDecision(52); }
				switch ( alt52 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:176:56: SkipSpace
					{
					DebugLocation(176, 56);
					SkipSpace97=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_doWhileStatement1308); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace97);


					}
					break;

				default:
					goto loop52;
				}
			}

			loop52:
				;

			} finally { DebugExitSubRule(52); }

			DebugLocation(176, 67);
			char_literal98=(CommonToken)Match(input,130,Follow._130_in_doWhileStatement1311); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_130.Add(char_literal98);

			DebugLocation(176, 75);
			PushFollow(Follow._expression_in_doWhileStatement1315);
			cond=expression();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_expression.Add(cond.Tree);
			DebugLocation(176, 87);
			char_literal99=(CommonToken)Match(input,131,Follow._131_in_doWhileStatement1317); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_131.Add(char_literal99);

			DebugLocation(176, 91);
			// JavaScript.g3:176:91: ( SkipSpace )*
			try { DebugEnterSubRule(53);
			while (true)
			{
				int alt53=2;
				try { DebugEnterDecision(53, decisionCanBacktrack[53]);
				int LA53_0 = input.LA(1);

				if ((LA53_0==SkipSpace))
				{
					alt53=1;
				}


				} finally { DebugExitDecision(53); }
				switch ( alt53 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:176:91: SkipSpace
					{
					DebugLocation(176, 91);
					SkipSpace100=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_doWhileStatement1319); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace100);


					}
					break;

				default:
					goto loop53;
				}
			}

			loop53:
				;

			} finally { DebugExitSubRule(53); }

			DebugLocation(176, 102);
			SEMI101=(CommonToken)Match(input,SEMI,Follow._SEMI_in_doWhileStatement1322); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_SEMI.Add(SEMI101);



			{
			// AST REWRITE
			// elements: cond, blk
			// token labels: 
			// rule labels: cond, blk, retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_cond=new RewriteRuleSubtreeStream(adaptor,"rule cond",cond!=null?cond.Tree:null);
			RewriteRuleSubtreeStream stream_blk=new RewriteRuleSubtreeStream(adaptor,"rule blk",blk!=null?blk.Tree:null);
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 177:9: -> ^( DO_BLOCK $cond $blk)
			{
				DebugLocation(177, 12);
				// JavaScript.g3:177:12: ^( DO_BLOCK $cond $blk)
				{
				CommonTree root_1 = (CommonTree)adaptor.Nil();
				DebugLocation(177, 14);
				root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(DO_BLOCK, "DO_BLOCK"), root_1);

				DebugLocation(177, 24);
				adaptor.AddChild(root_1, stream_cond.NextTree());
				DebugLocation(177, 30);
				adaptor.AddChild(root_1, stream_blk.NextTree());

				adaptor.AddChild(root_0, root_1);
				}

			}

			retval.Tree = root_0;
			}
			}

			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("doWhileStatement", 21);
			LeaveRule("doWhileStatement", 21);
			Leave_doWhileStatement();
			if (state.backtracking > 0) { Memoize(input, 21, doWhileStatement_StartIndex); }
		}
		DebugLocation(178, 4);
		} finally { DebugExitRule(GrammarFileName, "doWhileStatement"); }
		return retval;

	}
	// $ANTLR end "doWhileStatement"

	public class whileStatement_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_whileStatement();
	partial void Leave_whileStatement();

	// $ANTLR start "whileStatement"
	// JavaScript.g3:180:1: whileStatement : 'while' ( SkipSpace )* '(' ( SkipSpace )* cond= expression ( SkipSpace )* ')' ( SkipSpace )* blk= statement -> ^( WHILE_BLOCK $cond $blk) ;
	[GrammarRule("whileStatement")]
	private JavaScriptParser.whileStatement_return whileStatement()
	{
		Enter_whileStatement();
		EnterRule("whileStatement", 22);
		TraceIn("whileStatement", 22);
		JavaScriptParser.whileStatement_return retval = new JavaScriptParser.whileStatement_return();
		retval.Start = (CommonToken)input.LT(1);
		int whileStatement_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken string_literal102=null;
		CommonToken SkipSpace103=null;
		CommonToken char_literal104=null;
		CommonToken SkipSpace105=null;
		CommonToken SkipSpace106=null;
		CommonToken char_literal107=null;
		CommonToken SkipSpace108=null;
		JavaScriptParser.expression_return cond = default(JavaScriptParser.expression_return);
		JavaScriptParser.statement_return blk = default(JavaScriptParser.statement_return);

		CommonTree string_literal102_tree=null;
		CommonTree SkipSpace103_tree=null;
		CommonTree char_literal104_tree=null;
		CommonTree SkipSpace105_tree=null;
		CommonTree SkipSpace106_tree=null;
		CommonTree char_literal107_tree=null;
		CommonTree SkipSpace108_tree=null;
		RewriteRuleITokenStream stream_145=new RewriteRuleITokenStream(adaptor,"token 145");
		RewriteRuleITokenStream stream_SkipSpace=new RewriteRuleITokenStream(adaptor,"token SkipSpace");
		RewriteRuleITokenStream stream_130=new RewriteRuleITokenStream(adaptor,"token 130");
		RewriteRuleITokenStream stream_131=new RewriteRuleITokenStream(adaptor,"token 131");
		RewriteRuleSubtreeStream stream_expression=new RewriteRuleSubtreeStream(adaptor,"rule expression");
		RewriteRuleSubtreeStream stream_statement=new RewriteRuleSubtreeStream(adaptor,"rule statement");
		try { DebugEnterRule(GrammarFileName, "whileStatement");
		DebugLocation(180, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 22)) { return retval; }
			// JavaScript.g3:181:5: ( 'while' ( SkipSpace )* '(' ( SkipSpace )* cond= expression ( SkipSpace )* ')' ( SkipSpace )* blk= statement -> ^( WHILE_BLOCK $cond $blk) )
			DebugEnterAlt(1);
			// JavaScript.g3:181:7: 'while' ( SkipSpace )* '(' ( SkipSpace )* cond= expression ( SkipSpace )* ')' ( SkipSpace )* blk= statement
			{
			DebugLocation(181, 7);
			string_literal102=(CommonToken)Match(input,145,Follow._145_in_whileStatement1359); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_145.Add(string_literal102);

			DebugLocation(181, 15);
			// JavaScript.g3:181:15: ( SkipSpace )*
			try { DebugEnterSubRule(54);
			while (true)
			{
				int alt54=2;
				try { DebugEnterDecision(54, decisionCanBacktrack[54]);
				int LA54_0 = input.LA(1);

				if ((LA54_0==SkipSpace))
				{
					alt54=1;
				}


				} finally { DebugExitDecision(54); }
				switch ( alt54 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:181:15: SkipSpace
					{
					DebugLocation(181, 15);
					SkipSpace103=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_whileStatement1361); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace103);


					}
					break;

				default:
					goto loop54;
				}
			}

			loop54:
				;

			} finally { DebugExitSubRule(54); }

			DebugLocation(181, 26);
			char_literal104=(CommonToken)Match(input,130,Follow._130_in_whileStatement1364); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_130.Add(char_literal104);

			DebugLocation(181, 30);
			// JavaScript.g3:181:30: ( SkipSpace )*
			try { DebugEnterSubRule(55);
			while (true)
			{
				int alt55=2;
				try { DebugEnterDecision(55, decisionCanBacktrack[55]);
				int LA55_0 = input.LA(1);

				if ((LA55_0==SkipSpace))
				{
					alt55=1;
				}


				} finally { DebugExitDecision(55); }
				switch ( alt55 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:181:30: SkipSpace
					{
					DebugLocation(181, 30);
					SkipSpace105=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_whileStatement1366); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace105);


					}
					break;

				default:
					goto loop55;
				}
			}

			loop55:
				;

			} finally { DebugExitSubRule(55); }

			DebugLocation(181, 45);
			PushFollow(Follow._expression_in_whileStatement1371);
			cond=expression();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_expression.Add(cond.Tree);
			DebugLocation(181, 57);
			// JavaScript.g3:181:57: ( SkipSpace )*
			try { DebugEnterSubRule(56);
			while (true)
			{
				int alt56=2;
				try { DebugEnterDecision(56, decisionCanBacktrack[56]);
				int LA56_0 = input.LA(1);

				if ((LA56_0==SkipSpace))
				{
					alt56=1;
				}


				} finally { DebugExitDecision(56); }
				switch ( alt56 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:181:57: SkipSpace
					{
					DebugLocation(181, 57);
					SkipSpace106=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_whileStatement1373); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace106);


					}
					break;

				default:
					goto loop56;
				}
			}

			loop56:
				;

			} finally { DebugExitSubRule(56); }

			DebugLocation(181, 68);
			char_literal107=(CommonToken)Match(input,131,Follow._131_in_whileStatement1376); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_131.Add(char_literal107);

			DebugLocation(181, 72);
			// JavaScript.g3:181:72: ( SkipSpace )*
			try { DebugEnterSubRule(57);
			while (true)
			{
				int alt57=2;
				try { DebugEnterDecision(57, decisionCanBacktrack[57]);
				int LA57_0 = input.LA(1);

				if ((LA57_0==SkipSpace))
				{
					alt57=1;
				}


				} finally { DebugExitDecision(57); }
				switch ( alt57 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:181:72: SkipSpace
					{
					DebugLocation(181, 72);
					SkipSpace108=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_whileStatement1378); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace108);


					}
					break;

				default:
					goto loop57;
				}
			}

			loop57:
				;

			} finally { DebugExitSubRule(57); }

			DebugLocation(181, 86);
			PushFollow(Follow._statement_in_whileStatement1383);
			blk=statement();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_statement.Add(blk.Tree);


			{
			// AST REWRITE
			// elements: cond, blk
			// token labels: 
			// rule labels: cond, blk, retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_cond=new RewriteRuleSubtreeStream(adaptor,"rule cond",cond!=null?cond.Tree:null);
			RewriteRuleSubtreeStream stream_blk=new RewriteRuleSubtreeStream(adaptor,"rule blk",blk!=null?blk.Tree:null);
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 182:9: -> ^( WHILE_BLOCK $cond $blk)
			{
				DebugLocation(182, 12);
				// JavaScript.g3:182:12: ^( WHILE_BLOCK $cond $blk)
				{
				CommonTree root_1 = (CommonTree)adaptor.Nil();
				DebugLocation(182, 14);
				root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(WHILE_BLOCK, "WHILE_BLOCK"), root_1);

				DebugLocation(182, 27);
				adaptor.AddChild(root_1, stream_cond.NextTree());
				DebugLocation(182, 33);
				adaptor.AddChild(root_1, stream_blk.NextTree());

				adaptor.AddChild(root_0, root_1);
				}

			}

			retval.Tree = root_0;
			}
			}

			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("whileStatement", 22);
			LeaveRule("whileStatement", 22);
			Leave_whileStatement();
			if (state.backtracking > 0) { Memoize(input, 22, whileStatement_StartIndex); }
		}
		DebugLocation(183, 4);
		} finally { DebugExitRule(GrammarFileName, "whileStatement"); }
		return retval;

	}
	// $ANTLR end "whileStatement"

	public class forStatement_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_forStatement();
	partial void Leave_forStatement();

	// $ANTLR start "forStatement"
	// JavaScript.g3:185:1: forStatement : 'for' ( SkipSpace )* '(' ( SkipSpace )* (ini= forStatementInitialiserPart )? ( SkipSpace )* s1= SEMI ( SkipSpace )* (compareExp= expression )? ( SkipSpace )* s2= SEMI ( SkipSpace )* (incrementExp= expression )? ( SkipSpace )* ')' ( SkipSpace )* blk= statement -> ^( FOR_BLOCK ( $ini)? $s1 ( $compareExp)? $s2 ( $incrementExp)? $blk) ;
	[GrammarRule("forStatement")]
	private JavaScriptParser.forStatement_return forStatement()
	{
		Enter_forStatement();
		EnterRule("forStatement", 23);
		TraceIn("forStatement", 23);
		JavaScriptParser.forStatement_return retval = new JavaScriptParser.forStatement_return();
		retval.Start = (CommonToken)input.LT(1);
		int forStatement_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken s1=null;
		CommonToken s2=null;
		CommonToken string_literal109=null;
		CommonToken SkipSpace110=null;
		CommonToken char_literal111=null;
		CommonToken SkipSpace112=null;
		CommonToken SkipSpace113=null;
		CommonToken SkipSpace114=null;
		CommonToken SkipSpace115=null;
		CommonToken SkipSpace116=null;
		CommonToken SkipSpace117=null;
		CommonToken char_literal118=null;
		CommonToken SkipSpace119=null;
		JavaScriptParser.forStatementInitialiserPart_return ini = default(JavaScriptParser.forStatementInitialiserPart_return);
		JavaScriptParser.expression_return compareExp = default(JavaScriptParser.expression_return);
		JavaScriptParser.expression_return incrementExp = default(JavaScriptParser.expression_return);
		JavaScriptParser.statement_return blk = default(JavaScriptParser.statement_return);

		CommonTree s1_tree=null;
		CommonTree s2_tree=null;
		CommonTree string_literal109_tree=null;
		CommonTree SkipSpace110_tree=null;
		CommonTree char_literal111_tree=null;
		CommonTree SkipSpace112_tree=null;
		CommonTree SkipSpace113_tree=null;
		CommonTree SkipSpace114_tree=null;
		CommonTree SkipSpace115_tree=null;
		CommonTree SkipSpace116_tree=null;
		CommonTree SkipSpace117_tree=null;
		CommonTree char_literal118_tree=null;
		CommonTree SkipSpace119_tree=null;
		RewriteRuleITokenStream stream_141=new RewriteRuleITokenStream(adaptor,"token 141");
		RewriteRuleITokenStream stream_SkipSpace=new RewriteRuleITokenStream(adaptor,"token SkipSpace");
		RewriteRuleITokenStream stream_130=new RewriteRuleITokenStream(adaptor,"token 130");
		RewriteRuleITokenStream stream_SEMI=new RewriteRuleITokenStream(adaptor,"token SEMI");
		RewriteRuleITokenStream stream_131=new RewriteRuleITokenStream(adaptor,"token 131");
		RewriteRuleSubtreeStream stream_forStatementInitialiserPart=new RewriteRuleSubtreeStream(adaptor,"rule forStatementInitialiserPart");
		RewriteRuleSubtreeStream stream_expression=new RewriteRuleSubtreeStream(adaptor,"rule expression");
		RewriteRuleSubtreeStream stream_statement=new RewriteRuleSubtreeStream(adaptor,"rule statement");
		try { DebugEnterRule(GrammarFileName, "forStatement");
		DebugLocation(185, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 23)) { return retval; }
			// JavaScript.g3:186:5: ( 'for' ( SkipSpace )* '(' ( SkipSpace )* (ini= forStatementInitialiserPart )? ( SkipSpace )* s1= SEMI ( SkipSpace )* (compareExp= expression )? ( SkipSpace )* s2= SEMI ( SkipSpace )* (incrementExp= expression )? ( SkipSpace )* ')' ( SkipSpace )* blk= statement -> ^( FOR_BLOCK ( $ini)? $s1 ( $compareExp)? $s2 ( $incrementExp)? $blk) )
			DebugEnterAlt(1);
			// JavaScript.g3:186:7: 'for' ( SkipSpace )* '(' ( SkipSpace )* (ini= forStatementInitialiserPart )? ( SkipSpace )* s1= SEMI ( SkipSpace )* (compareExp= expression )? ( SkipSpace )* s2= SEMI ( SkipSpace )* (incrementExp= expression )? ( SkipSpace )* ')' ( SkipSpace )* blk= statement
			{
			DebugLocation(186, 7);
			string_literal109=(CommonToken)Match(input,141,Follow._141_in_forStatement1420); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_141.Add(string_literal109);

			DebugLocation(186, 13);
			// JavaScript.g3:186:13: ( SkipSpace )*
			try { DebugEnterSubRule(58);
			while (true)
			{
				int alt58=2;
				try { DebugEnterDecision(58, decisionCanBacktrack[58]);
				int LA58_0 = input.LA(1);

				if ((LA58_0==SkipSpace))
				{
					alt58=1;
				}


				} finally { DebugExitDecision(58); }
				switch ( alt58 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:186:13: SkipSpace
					{
					DebugLocation(186, 13);
					SkipSpace110=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_forStatement1422); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace110);


					}
					break;

				default:
					goto loop58;
				}
			}

			loop58:
				;

			} finally { DebugExitSubRule(58); }

			DebugLocation(186, 24);
			char_literal111=(CommonToken)Match(input,130,Follow._130_in_forStatement1425); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_130.Add(char_literal111);

			DebugLocation(186, 28);
			// JavaScript.g3:186:28: ( SkipSpace )*
			try { DebugEnterSubRule(59);
			while (true)
			{
				int alt59=2;
				try { DebugEnterDecision(59, decisionCanBacktrack[59]);
				int LA59_0 = input.LA(1);

				if ((LA59_0==SkipSpace))
				{
					int LA59_2 = input.LA(2);

					if ((EvaluatePredicate(synpred73_JavaScript_fragment)))
					{
						alt59=1;
					}


				}


				} finally { DebugExitDecision(59); }
				switch ( alt59 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:186:28: SkipSpace
					{
					DebugLocation(186, 28);
					SkipSpace112=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_forStatement1427); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace112);


					}
					break;

				default:
					goto loop59;
				}
			}

			loop59:
				;

			} finally { DebugExitSubRule(59); }

			DebugLocation(186, 39);
			// JavaScript.g3:186:39: (ini= forStatementInitialiserPart )?
			int alt60=2;
			try { DebugEnterSubRule(60);
			try { DebugEnterDecision(60, decisionCanBacktrack[60]);
			int LA60_0 = input.LA(1);

			if ((LA60_0==BIT_NOT||LA60_0==BoolLiteral||LA60_0==DELETE||LA60_0==DOUBLE_NOT||LA60_0==FALSE||LA60_0==IMPORT_START||LA60_0==Identifier||LA60_0==MINUS||LA60_0==MINUS_INC||LA60_0==NEW||(LA60_0>=NOT && LA60_0<=NULL)||LA60_0==NumericLiteral||LA60_0==PLUS||LA60_0==PLUS_INC||(LA60_0>=StringLiteral && LA60_0<=THIS)||LA60_0==TYPE_OF||(LA60_0>=VAR && LA60_0<=VOID)||LA60_0==130||LA60_0==135||LA60_0==142||LA60_0==147))
			{
				alt60=1;
			}
			} finally { DebugExitDecision(60); }
			switch (alt60)
			{
			case 1:
				DebugEnterAlt(1);
				// JavaScript.g3:186:40: ini= forStatementInitialiserPart
				{
				DebugLocation(186, 43);
				PushFollow(Follow._forStatementInitialiserPart_in_forStatement1433);
				ini=forStatementInitialiserPart();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_forStatementInitialiserPart.Add(ini.Tree);

				}
				break;

			}
			} finally { DebugExitSubRule(60); }

			DebugLocation(186, 74);
			// JavaScript.g3:186:74: ( SkipSpace )*
			try { DebugEnterSubRule(61);
			while (true)
			{
				int alt61=2;
				try { DebugEnterDecision(61, decisionCanBacktrack[61]);
				int LA61_0 = input.LA(1);

				if ((LA61_0==SkipSpace))
				{
					alt61=1;
				}


				} finally { DebugExitDecision(61); }
				switch ( alt61 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:186:74: SkipSpace
					{
					DebugLocation(186, 74);
					SkipSpace113=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_forStatement1437); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace113);


					}
					break;

				default:
					goto loop61;
				}
			}

			loop61:
				;

			} finally { DebugExitSubRule(61); }

			DebugLocation(186, 87);
			s1=(CommonToken)Match(input,SEMI,Follow._SEMI_in_forStatement1442); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_SEMI.Add(s1);

			DebugLocation(186, 93);
			// JavaScript.g3:186:93: ( SkipSpace )*
			try { DebugEnterSubRule(62);
			while (true)
			{
				int alt62=2;
				try { DebugEnterDecision(62, decisionCanBacktrack[62]);
				int LA62_0 = input.LA(1);

				if ((LA62_0==SkipSpace))
				{
					int LA62_2 = input.LA(2);

					if ((EvaluatePredicate(synpred76_JavaScript_fragment)))
					{
						alt62=1;
					}


				}


				} finally { DebugExitDecision(62); }
				switch ( alt62 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:186:93: SkipSpace
					{
					DebugLocation(186, 93);
					SkipSpace114=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_forStatement1444); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace114);


					}
					break;

				default:
					goto loop62;
				}
			}

			loop62:
				;

			} finally { DebugExitSubRule(62); }

			DebugLocation(186, 104);
			// JavaScript.g3:186:104: (compareExp= expression )?
			int alt63=2;
			try { DebugEnterSubRule(63);
			try { DebugEnterDecision(63, decisionCanBacktrack[63]);
			int LA63_0 = input.LA(1);

			if ((LA63_0==BIT_NOT||LA63_0==BoolLiteral||LA63_0==DELETE||LA63_0==DOUBLE_NOT||LA63_0==FALSE||LA63_0==IMPORT_START||LA63_0==Identifier||LA63_0==MINUS||LA63_0==MINUS_INC||LA63_0==NEW||(LA63_0>=NOT && LA63_0<=NULL)||LA63_0==NumericLiteral||LA63_0==PLUS||LA63_0==PLUS_INC||(LA63_0>=StringLiteral && LA63_0<=THIS)||LA63_0==TYPE_OF||LA63_0==VOID||LA63_0==130||LA63_0==135||LA63_0==142||LA63_0==147))
			{
				alt63=1;
			}
			} finally { DebugExitDecision(63); }
			switch (alt63)
			{
			case 1:
				DebugEnterAlt(1);
				// JavaScript.g3:186:105: compareExp= expression
				{
				DebugLocation(186, 115);
				PushFollow(Follow._expression_in_forStatement1450);
				compareExp=expression();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_expression.Add(compareExp.Tree);

				}
				break;

			}
			} finally { DebugExitSubRule(63); }

			DebugLocation(186, 129);
			// JavaScript.g3:186:129: ( SkipSpace )*
			try { DebugEnterSubRule(64);
			while (true)
			{
				int alt64=2;
				try { DebugEnterDecision(64, decisionCanBacktrack[64]);
				int LA64_0 = input.LA(1);

				if ((LA64_0==SkipSpace))
				{
					alt64=1;
				}


				} finally { DebugExitDecision(64); }
				switch ( alt64 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:186:129: SkipSpace
					{
					DebugLocation(186, 129);
					SkipSpace115=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_forStatement1454); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace115);


					}
					break;

				default:
					goto loop64;
				}
			}

			loop64:
				;

			} finally { DebugExitSubRule(64); }

			DebugLocation(186, 142);
			s2=(CommonToken)Match(input,SEMI,Follow._SEMI_in_forStatement1459); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_SEMI.Add(s2);

			DebugLocation(186, 148);
			// JavaScript.g3:186:148: ( SkipSpace )*
			try { DebugEnterSubRule(65);
			while (true)
			{
				int alt65=2;
				try { DebugEnterDecision(65, decisionCanBacktrack[65]);
				int LA65_0 = input.LA(1);

				if ((LA65_0==SkipSpace))
				{
					int LA65_2 = input.LA(2);

					if ((EvaluatePredicate(synpred79_JavaScript_fragment)))
					{
						alt65=1;
					}


				}


				} finally { DebugExitDecision(65); }
				switch ( alt65 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:186:148: SkipSpace
					{
					DebugLocation(186, 148);
					SkipSpace116=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_forStatement1461); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace116);


					}
					break;

				default:
					goto loop65;
				}
			}

			loop65:
				;

			} finally { DebugExitSubRule(65); }

			DebugLocation(186, 159);
			// JavaScript.g3:186:159: (incrementExp= expression )?
			int alt66=2;
			try { DebugEnterSubRule(66);
			try { DebugEnterDecision(66, decisionCanBacktrack[66]);
			int LA66_0 = input.LA(1);

			if ((LA66_0==BIT_NOT||LA66_0==BoolLiteral||LA66_0==DELETE||LA66_0==DOUBLE_NOT||LA66_0==FALSE||LA66_0==IMPORT_START||LA66_0==Identifier||LA66_0==MINUS||LA66_0==MINUS_INC||LA66_0==NEW||(LA66_0>=NOT && LA66_0<=NULL)||LA66_0==NumericLiteral||LA66_0==PLUS||LA66_0==PLUS_INC||(LA66_0>=StringLiteral && LA66_0<=THIS)||LA66_0==TYPE_OF||LA66_0==VOID||LA66_0==130||LA66_0==135||LA66_0==142||LA66_0==147))
			{
				alt66=1;
			}
			} finally { DebugExitDecision(66); }
			switch (alt66)
			{
			case 1:
				DebugEnterAlt(1);
				// JavaScript.g3:186:160: incrementExp= expression
				{
				DebugLocation(186, 172);
				PushFollow(Follow._expression_in_forStatement1467);
				incrementExp=expression();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_expression.Add(incrementExp.Tree);

				}
				break;

			}
			} finally { DebugExitSubRule(66); }

			DebugLocation(186, 186);
			// JavaScript.g3:186:186: ( SkipSpace )*
			try { DebugEnterSubRule(67);
			while (true)
			{
				int alt67=2;
				try { DebugEnterDecision(67, decisionCanBacktrack[67]);
				int LA67_0 = input.LA(1);

				if ((LA67_0==SkipSpace))
				{
					alt67=1;
				}


				} finally { DebugExitDecision(67); }
				switch ( alt67 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:186:186: SkipSpace
					{
					DebugLocation(186, 186);
					SkipSpace117=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_forStatement1471); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace117);


					}
					break;

				default:
					goto loop67;
				}
			}

			loop67:
				;

			} finally { DebugExitSubRule(67); }

			DebugLocation(186, 197);
			char_literal118=(CommonToken)Match(input,131,Follow._131_in_forStatement1474); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_131.Add(char_literal118);

			DebugLocation(186, 201);
			// JavaScript.g3:186:201: ( SkipSpace )*
			try { DebugEnterSubRule(68);
			while (true)
			{
				int alt68=2;
				try { DebugEnterDecision(68, decisionCanBacktrack[68]);
				int LA68_0 = input.LA(1);

				if ((LA68_0==SkipSpace))
				{
					alt68=1;
				}


				} finally { DebugExitDecision(68); }
				switch ( alt68 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:186:201: SkipSpace
					{
					DebugLocation(186, 201);
					SkipSpace119=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_forStatement1476); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace119);


					}
					break;

				default:
					goto loop68;
				}
			}

			loop68:
				;

			} finally { DebugExitSubRule(68); }

			DebugLocation(186, 215);
			PushFollow(Follow._statement_in_forStatement1481);
			blk=statement();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_statement.Add(blk.Tree);


			{
			// AST REWRITE
			// elements: ini, s1, compareExp, s2, incrementExp, blk
			// token labels: s1, s2
			// rule labels: ini, compareExp, incrementExp, blk, retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleITokenStream stream_s1=new RewriteRuleITokenStream(adaptor,"token s1",s1);
			RewriteRuleITokenStream stream_s2=new RewriteRuleITokenStream(adaptor,"token s2",s2);
			RewriteRuleSubtreeStream stream_ini=new RewriteRuleSubtreeStream(adaptor,"rule ini",ini!=null?ini.Tree:null);
			RewriteRuleSubtreeStream stream_compareExp=new RewriteRuleSubtreeStream(adaptor,"rule compareExp",compareExp!=null?compareExp.Tree:null);
			RewriteRuleSubtreeStream stream_incrementExp=new RewriteRuleSubtreeStream(adaptor,"rule incrementExp",incrementExp!=null?incrementExp.Tree:null);
			RewriteRuleSubtreeStream stream_blk=new RewriteRuleSubtreeStream(adaptor,"rule blk",blk!=null?blk.Tree:null);
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 187:9: -> ^( FOR_BLOCK ( $ini)? $s1 ( $compareExp)? $s2 ( $incrementExp)? $blk)
			{
				DebugLocation(187, 12);
				// JavaScript.g3:187:12: ^( FOR_BLOCK ( $ini)? $s1 ( $compareExp)? $s2 ( $incrementExp)? $blk)
				{
				CommonTree root_1 = (CommonTree)adaptor.Nil();
				DebugLocation(187, 14);
				root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(FOR_BLOCK, "FOR_BLOCK"), root_1);

				DebugLocation(187, 25);
				// JavaScript.g3:187:25: ( $ini)?
				if ( stream_ini.HasNext )
				{
					DebugLocation(187, 25);
					adaptor.AddChild(root_1, stream_ini.NextTree());

				}
				stream_ini.Reset();
				DebugLocation(187, 31);
				adaptor.AddChild(root_1, stream_s1.NextNode());
				DebugLocation(187, 35);
				// JavaScript.g3:187:35: ( $compareExp)?
				if ( stream_compareExp.HasNext )
				{
					DebugLocation(187, 35);
					adaptor.AddChild(root_1, stream_compareExp.NextTree());

				}
				stream_compareExp.Reset();
				DebugLocation(187, 48);
				adaptor.AddChild(root_1, stream_s2.NextNode());
				DebugLocation(187, 52);
				// JavaScript.g3:187:52: ( $incrementExp)?
				if ( stream_incrementExp.HasNext )
				{
					DebugLocation(187, 52);
					adaptor.AddChild(root_1, stream_incrementExp.NextTree());

				}
				stream_incrementExp.Reset();
				DebugLocation(187, 67);
				adaptor.AddChild(root_1, stream_blk.NextTree());

				adaptor.AddChild(root_0, root_1);
				}

			}

			retval.Tree = root_0;
			}
			}

			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("forStatement", 23);
			LeaveRule("forStatement", 23);
			Leave_forStatement();
			if (state.backtracking > 0) { Memoize(input, 23, forStatement_StartIndex); }
		}
		DebugLocation(188, 4);
		} finally { DebugExitRule(GrammarFileName, "forStatement"); }
		return retval;

	}
	// $ANTLR end "forStatement"

	public class forStatementInitialiserPart_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_forStatementInitialiserPart();
	partial void Leave_forStatementInitialiserPart();

	// $ANTLR start "forStatementInitialiserPart"
	// JavaScript.g3:190:1: forStatementInitialiserPart : ( expressionNoIn | VAR ( SkipSpace )* variableDeclarationListNoIn );
	[GrammarRule("forStatementInitialiserPart")]
	private JavaScriptParser.forStatementInitialiserPart_return forStatementInitialiserPart()
	{
		Enter_forStatementInitialiserPart();
		EnterRule("forStatementInitialiserPart", 24);
		TraceIn("forStatementInitialiserPart", 24);
		JavaScriptParser.forStatementInitialiserPart_return retval = new JavaScriptParser.forStatementInitialiserPart_return();
		retval.Start = (CommonToken)input.LT(1);
		int forStatementInitialiserPart_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken VAR121=null;
		CommonToken SkipSpace122=null;
		JavaScriptParser.expressionNoIn_return expressionNoIn120 = default(JavaScriptParser.expressionNoIn_return);
		JavaScriptParser.variableDeclarationListNoIn_return variableDeclarationListNoIn123 = default(JavaScriptParser.variableDeclarationListNoIn_return);

		CommonTree VAR121_tree=null;
		CommonTree SkipSpace122_tree=null;

		try { DebugEnterRule(GrammarFileName, "forStatementInitialiserPart");
		DebugLocation(190, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 24)) { return retval; }
			// JavaScript.g3:191:5: ( expressionNoIn | VAR ( SkipSpace )* variableDeclarationListNoIn )
			int alt70=2;
			try { DebugEnterDecision(70, decisionCanBacktrack[70]);
			int LA70_0 = input.LA(1);

			if ((LA70_0==BIT_NOT||LA70_0==BoolLiteral||LA70_0==DELETE||LA70_0==DOUBLE_NOT||LA70_0==FALSE||LA70_0==IMPORT_START||LA70_0==Identifier||LA70_0==MINUS||LA70_0==MINUS_INC||LA70_0==NEW||(LA70_0>=NOT && LA70_0<=NULL)||LA70_0==NumericLiteral||LA70_0==PLUS||LA70_0==PLUS_INC||(LA70_0>=StringLiteral && LA70_0<=THIS)||LA70_0==TYPE_OF||LA70_0==VOID||LA70_0==130||LA70_0==135||LA70_0==142||LA70_0==147))
			{
				alt70=1;
			}
			else if ((LA70_0==VAR))
			{
				alt70=2;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return retval;}
				NoViableAltException nvae = new NoViableAltException("", 70, 0, input);

				DebugRecognitionException(nvae);
				throw nvae;
			}
			} finally { DebugExitDecision(70); }
			switch (alt70)
			{
			case 1:
				DebugEnterAlt(1);
				// JavaScript.g3:191:7: expressionNoIn
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(191, 7);
				PushFollow(Follow._expressionNoIn_in_forStatementInitialiserPart1534);
				expressionNoIn120=expressionNoIn();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, expressionNoIn120.Tree);

				}
				break;
			case 2:
				DebugEnterAlt(2);
				// JavaScript.g3:192:7: VAR ( SkipSpace )* variableDeclarationListNoIn
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(192, 7);
				VAR121=(CommonToken)Match(input,VAR,Follow._VAR_in_forStatementInitialiserPart1542); if (state.failed) return retval;
				if ( state.backtracking==0 ) {
				VAR121_tree = (CommonTree)adaptor.Create(VAR121);
				adaptor.AddChild(root_0, VAR121_tree);
				}
				DebugLocation(192, 20);
				// JavaScript.g3:192:20: ( SkipSpace )*
				try { DebugEnterSubRule(69);
				while (true)
				{
					int alt69=2;
					try { DebugEnterDecision(69, decisionCanBacktrack[69]);
					int LA69_0 = input.LA(1);

					if ((LA69_0==SkipSpace))
					{
						alt69=1;
					}


					} finally { DebugExitDecision(69); }
					switch ( alt69 )
					{
					case 1:
						DebugEnterAlt(1);
						// JavaScript.g3:192:20: SkipSpace
						{
						DebugLocation(192, 20);
						SkipSpace122=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_forStatementInitialiserPart1544); if (state.failed) return retval;

						}
						break;

					default:
						goto loop69;
					}
				}

				loop69:
					;

				} finally { DebugExitSubRule(69); }

				DebugLocation(192, 23);
				PushFollow(Follow._variableDeclarationListNoIn_in_forStatementInitialiserPart1548);
				variableDeclarationListNoIn123=variableDeclarationListNoIn();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, variableDeclarationListNoIn123.Tree);

				}
				break;

			}
			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("forStatementInitialiserPart", 24);
			LeaveRule("forStatementInitialiserPart", 24);
			Leave_forStatementInitialiserPart();
			if (state.backtracking > 0) { Memoize(input, 24, forStatementInitialiserPart_StartIndex); }
		}
		DebugLocation(193, 4);
		} finally { DebugExitRule(GrammarFileName, "forStatementInitialiserPart"); }
		return retval;

	}
	// $ANTLR end "forStatementInitialiserPart"

	public class forInStatement_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_forInStatement();
	partial void Leave_forInStatement();

	// $ANTLR start "forInStatement"
	// JavaScript.g3:195:1: forInStatement : 'for' ( SkipSpace )* '(' ( SkipSpace )* forInStatementInitialiserPart ( SkipSpace )* 'in' ( SkipSpace )* expression ( SkipSpace )* ')' ( SkipSpace )* statement -> ^( FOR_IN_BLOCK forInStatementInitialiserPart expression statement ) ;
	[GrammarRule("forInStatement")]
	private JavaScriptParser.forInStatement_return forInStatement()
	{
		Enter_forInStatement();
		EnterRule("forInStatement", 25);
		TraceIn("forInStatement", 25);
		JavaScriptParser.forInStatement_return retval = new JavaScriptParser.forInStatement_return();
		retval.Start = (CommonToken)input.LT(1);
		int forInStatement_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken string_literal124=null;
		CommonToken SkipSpace125=null;
		CommonToken char_literal126=null;
		CommonToken SkipSpace127=null;
		CommonToken SkipSpace129=null;
		CommonToken string_literal130=null;
		CommonToken SkipSpace131=null;
		CommonToken SkipSpace133=null;
		CommonToken char_literal134=null;
		CommonToken SkipSpace135=null;
		JavaScriptParser.forInStatementInitialiserPart_return forInStatementInitialiserPart128 = default(JavaScriptParser.forInStatementInitialiserPart_return);
		JavaScriptParser.expression_return expression132 = default(JavaScriptParser.expression_return);
		JavaScriptParser.statement_return statement136 = default(JavaScriptParser.statement_return);

		CommonTree string_literal124_tree=null;
		CommonTree SkipSpace125_tree=null;
		CommonTree char_literal126_tree=null;
		CommonTree SkipSpace127_tree=null;
		CommonTree SkipSpace129_tree=null;
		CommonTree string_literal130_tree=null;
		CommonTree SkipSpace131_tree=null;
		CommonTree SkipSpace133_tree=null;
		CommonTree char_literal134_tree=null;
		CommonTree SkipSpace135_tree=null;
		RewriteRuleITokenStream stream_141=new RewriteRuleITokenStream(adaptor,"token 141");
		RewriteRuleITokenStream stream_SkipSpace=new RewriteRuleITokenStream(adaptor,"token SkipSpace");
		RewriteRuleITokenStream stream_130=new RewriteRuleITokenStream(adaptor,"token 130");
		RewriteRuleITokenStream stream_IN=new RewriteRuleITokenStream(adaptor,"token IN");
		RewriteRuleITokenStream stream_131=new RewriteRuleITokenStream(adaptor,"token 131");
		RewriteRuleSubtreeStream stream_forInStatementInitialiserPart=new RewriteRuleSubtreeStream(adaptor,"rule forInStatementInitialiserPart");
		RewriteRuleSubtreeStream stream_expression=new RewriteRuleSubtreeStream(adaptor,"rule expression");
		RewriteRuleSubtreeStream stream_statement=new RewriteRuleSubtreeStream(adaptor,"rule statement");
		try { DebugEnterRule(GrammarFileName, "forInStatement");
		DebugLocation(195, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 25)) { return retval; }
			// JavaScript.g3:196:5: ( 'for' ( SkipSpace )* '(' ( SkipSpace )* forInStatementInitialiserPart ( SkipSpace )* 'in' ( SkipSpace )* expression ( SkipSpace )* ')' ( SkipSpace )* statement -> ^( FOR_IN_BLOCK forInStatementInitialiserPart expression statement ) )
			DebugEnterAlt(1);
			// JavaScript.g3:196:7: 'for' ( SkipSpace )* '(' ( SkipSpace )* forInStatementInitialiserPart ( SkipSpace )* 'in' ( SkipSpace )* expression ( SkipSpace )* ')' ( SkipSpace )* statement
			{
			DebugLocation(196, 7);
			string_literal124=(CommonToken)Match(input,141,Follow._141_in_forInStatement1565); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_141.Add(string_literal124);

			DebugLocation(196, 13);
			// JavaScript.g3:196:13: ( SkipSpace )*
			try { DebugEnterSubRule(71);
			while (true)
			{
				int alt71=2;
				try { DebugEnterDecision(71, decisionCanBacktrack[71]);
				int LA71_0 = input.LA(1);

				if ((LA71_0==SkipSpace))
				{
					alt71=1;
				}


				} finally { DebugExitDecision(71); }
				switch ( alt71 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:196:13: SkipSpace
					{
					DebugLocation(196, 13);
					SkipSpace125=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_forInStatement1567); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace125);


					}
					break;

				default:
					goto loop71;
				}
			}

			loop71:
				;

			} finally { DebugExitSubRule(71); }

			DebugLocation(196, 24);
			char_literal126=(CommonToken)Match(input,130,Follow._130_in_forInStatement1570); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_130.Add(char_literal126);

			DebugLocation(196, 28);
			// JavaScript.g3:196:28: ( SkipSpace )*
			try { DebugEnterSubRule(72);
			while (true)
			{
				int alt72=2;
				try { DebugEnterDecision(72, decisionCanBacktrack[72]);
				int LA72_0 = input.LA(1);

				if ((LA72_0==SkipSpace))
				{
					alt72=1;
				}


				} finally { DebugExitDecision(72); }
				switch ( alt72 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:196:28: SkipSpace
					{
					DebugLocation(196, 28);
					SkipSpace127=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_forInStatement1572); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace127);


					}
					break;

				default:
					goto loop72;
				}
			}

			loop72:
				;

			} finally { DebugExitSubRule(72); }

			DebugLocation(196, 39);
			PushFollow(Follow._forInStatementInitialiserPart_in_forInStatement1575);
			forInStatementInitialiserPart128=forInStatementInitialiserPart();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_forInStatementInitialiserPart.Add(forInStatementInitialiserPart128.Tree);
			DebugLocation(196, 69);
			// JavaScript.g3:196:69: ( SkipSpace )*
			try { DebugEnterSubRule(73);
			while (true)
			{
				int alt73=2;
				try { DebugEnterDecision(73, decisionCanBacktrack[73]);
				int LA73_0 = input.LA(1);

				if ((LA73_0==SkipSpace))
				{
					alt73=1;
				}


				} finally { DebugExitDecision(73); }
				switch ( alt73 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:196:69: SkipSpace
					{
					DebugLocation(196, 69);
					SkipSpace129=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_forInStatement1577); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace129);


					}
					break;

				default:
					goto loop73;
				}
			}

			loop73:
				;

			} finally { DebugExitSubRule(73); }

			DebugLocation(196, 80);
			string_literal130=(CommonToken)Match(input,IN,Follow._IN_in_forInStatement1580); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_IN.Add(string_literal130);

			DebugLocation(196, 85);
			// JavaScript.g3:196:85: ( SkipSpace )*
			try { DebugEnterSubRule(74);
			while (true)
			{
				int alt74=2;
				try { DebugEnterDecision(74, decisionCanBacktrack[74]);
				int LA74_0 = input.LA(1);

				if ((LA74_0==SkipSpace))
				{
					alt74=1;
				}


				} finally { DebugExitDecision(74); }
				switch ( alt74 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:196:85: SkipSpace
					{
					DebugLocation(196, 85);
					SkipSpace131=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_forInStatement1582); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace131);


					}
					break;

				default:
					goto loop74;
				}
			}

			loop74:
				;

			} finally { DebugExitSubRule(74); }

			DebugLocation(196, 96);
			PushFollow(Follow._expression_in_forInStatement1585);
			expression132=expression();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_expression.Add(expression132.Tree);
			DebugLocation(196, 107);
			// JavaScript.g3:196:107: ( SkipSpace )*
			try { DebugEnterSubRule(75);
			while (true)
			{
				int alt75=2;
				try { DebugEnterDecision(75, decisionCanBacktrack[75]);
				int LA75_0 = input.LA(1);

				if ((LA75_0==SkipSpace))
				{
					alt75=1;
				}


				} finally { DebugExitDecision(75); }
				switch ( alt75 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:196:107: SkipSpace
					{
					DebugLocation(196, 107);
					SkipSpace133=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_forInStatement1587); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace133);


					}
					break;

				default:
					goto loop75;
				}
			}

			loop75:
				;

			} finally { DebugExitSubRule(75); }

			DebugLocation(196, 118);
			char_literal134=(CommonToken)Match(input,131,Follow._131_in_forInStatement1590); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_131.Add(char_literal134);

			DebugLocation(196, 122);
			// JavaScript.g3:196:122: ( SkipSpace )*
			try { DebugEnterSubRule(76);
			while (true)
			{
				int alt76=2;
				try { DebugEnterDecision(76, decisionCanBacktrack[76]);
				int LA76_0 = input.LA(1);

				if ((LA76_0==SkipSpace))
				{
					alt76=1;
				}


				} finally { DebugExitDecision(76); }
				switch ( alt76 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:196:122: SkipSpace
					{
					DebugLocation(196, 122);
					SkipSpace135=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_forInStatement1592); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace135);


					}
					break;

				default:
					goto loop76;
				}
			}

			loop76:
				;

			} finally { DebugExitSubRule(76); }

			DebugLocation(196, 133);
			PushFollow(Follow._statement_in_forInStatement1595);
			statement136=statement();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_statement.Add(statement136.Tree);


			{
			// AST REWRITE
			// elements: forInStatementInitialiserPart, expression, statement
			// token labels: 
			// rule labels: retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 197:9: -> ^( FOR_IN_BLOCK forInStatementInitialiserPart expression statement )
			{
				DebugLocation(197, 12);
				// JavaScript.g3:197:12: ^( FOR_IN_BLOCK forInStatementInitialiserPart expression statement )
				{
				CommonTree root_1 = (CommonTree)adaptor.Nil();
				DebugLocation(197, 14);
				root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(FOR_IN_BLOCK, "FOR_IN_BLOCK"), root_1);

				DebugLocation(197, 27);
				adaptor.AddChild(root_1, stream_forInStatementInitialiserPart.NextTree());
				DebugLocation(197, 57);
				adaptor.AddChild(root_1, stream_expression.NextTree());
				DebugLocation(197, 68);
				adaptor.AddChild(root_1, stream_statement.NextTree());

				adaptor.AddChild(root_0, root_1);
				}

			}

			retval.Tree = root_0;
			}
			}

			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("forInStatement", 25);
			LeaveRule("forInStatement", 25);
			Leave_forInStatement();
			if (state.backtracking > 0) { Memoize(input, 25, forInStatement_StartIndex); }
		}
		DebugLocation(198, 4);
		} finally { DebugExitRule(GrammarFileName, "forInStatement"); }
		return retval;

	}
	// $ANTLR end "forInStatement"

	public class forInStatementInitialiserPart_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_forInStatementInitialiserPart();
	partial void Leave_forInStatementInitialiserPart();

	// $ANTLR start "forInStatementInitialiserPart"
	// JavaScript.g3:200:1: forInStatementInitialiserPart : ( leftHandSideExpression | VAR ( SkipSpace )* variableDeclarationNoIn );
	[GrammarRule("forInStatementInitialiserPart")]
	private JavaScriptParser.forInStatementInitialiserPart_return forInStatementInitialiserPart()
	{
		Enter_forInStatementInitialiserPart();
		EnterRule("forInStatementInitialiserPart", 26);
		TraceIn("forInStatementInitialiserPart", 26);
		JavaScriptParser.forInStatementInitialiserPart_return retval = new JavaScriptParser.forInStatementInitialiserPart_return();
		retval.Start = (CommonToken)input.LT(1);
		int forInStatementInitialiserPart_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken VAR138=null;
		CommonToken SkipSpace139=null;
		JavaScriptParser.leftHandSideExpression_return leftHandSideExpression137 = default(JavaScriptParser.leftHandSideExpression_return);
		JavaScriptParser.variableDeclarationNoIn_return variableDeclarationNoIn140 = default(JavaScriptParser.variableDeclarationNoIn_return);

		CommonTree VAR138_tree=null;
		CommonTree SkipSpace139_tree=null;

		try { DebugEnterRule(GrammarFileName, "forInStatementInitialiserPart");
		DebugLocation(200, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 26)) { return retval; }
			// JavaScript.g3:201:5: ( leftHandSideExpression | VAR ( SkipSpace )* variableDeclarationNoIn )
			int alt78=2;
			try { DebugEnterDecision(78, decisionCanBacktrack[78]);
			int LA78_0 = input.LA(1);

			if ((LA78_0==BoolLiteral||LA78_0==FALSE||LA78_0==IMPORT_START||LA78_0==Identifier||LA78_0==NEW||LA78_0==NULL||LA78_0==NumericLiteral||(LA78_0>=StringLiteral && LA78_0<=THIS)||LA78_0==130||LA78_0==135||LA78_0==142||LA78_0==147))
			{
				alt78=1;
			}
			else if ((LA78_0==VAR))
			{
				alt78=2;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return retval;}
				NoViableAltException nvae = new NoViableAltException("", 78, 0, input);

				DebugRecognitionException(nvae);
				throw nvae;
			}
			} finally { DebugExitDecision(78); }
			switch (alt78)
			{
			case 1:
				DebugEnterAlt(1);
				// JavaScript.g3:201:7: leftHandSideExpression
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(201, 7);
				PushFollow(Follow._leftHandSideExpression_in_forInStatementInitialiserPart1632);
				leftHandSideExpression137=leftHandSideExpression();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, leftHandSideExpression137.Tree);

				}
				break;
			case 2:
				DebugEnterAlt(2);
				// JavaScript.g3:202:7: VAR ( SkipSpace )* variableDeclarationNoIn
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(202, 7);
				VAR138=(CommonToken)Match(input,VAR,Follow._VAR_in_forInStatementInitialiserPart1640); if (state.failed) return retval;
				if ( state.backtracking==0 ) {
				VAR138_tree = (CommonTree)adaptor.Create(VAR138);
				adaptor.AddChild(root_0, VAR138_tree);
				}
				DebugLocation(202, 20);
				// JavaScript.g3:202:20: ( SkipSpace )*
				try { DebugEnterSubRule(77);
				while (true)
				{
					int alt77=2;
					try { DebugEnterDecision(77, decisionCanBacktrack[77]);
					int LA77_0 = input.LA(1);

					if ((LA77_0==SkipSpace))
					{
						alt77=1;
					}


					} finally { DebugExitDecision(77); }
					switch ( alt77 )
					{
					case 1:
						DebugEnterAlt(1);
						// JavaScript.g3:202:20: SkipSpace
						{
						DebugLocation(202, 20);
						SkipSpace139=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_forInStatementInitialiserPart1642); if (state.failed) return retval;

						}
						break;

					default:
						goto loop77;
					}
				}

				loop77:
					;

				} finally { DebugExitSubRule(77); }

				DebugLocation(202, 23);
				PushFollow(Follow._variableDeclarationNoIn_in_forInStatementInitialiserPart1646);
				variableDeclarationNoIn140=variableDeclarationNoIn();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, variableDeclarationNoIn140.Tree);

				}
				break;

			}
			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("forInStatementInitialiserPart", 26);
			LeaveRule("forInStatementInitialiserPart", 26);
			Leave_forInStatementInitialiserPart();
			if (state.backtracking > 0) { Memoize(input, 26, forInStatementInitialiserPart_StartIndex); }
		}
		DebugLocation(203, 4);
		} finally { DebugExitRule(GrammarFileName, "forInStatementInitialiserPart"); }
		return retval;

	}
	// $ANTLR end "forInStatementInitialiserPart"

	public class continueStatement_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_continueStatement();
	partial void Leave_continueStatement();

	// $ANTLR start "continueStatement"
	// JavaScript.g3:205:1: continueStatement : CONTINUE ( Identifier )? ( SkipSpace )* SEMI ;
	[GrammarRule("continueStatement")]
	private JavaScriptParser.continueStatement_return continueStatement()
	{
		Enter_continueStatement();
		EnterRule("continueStatement", 27);
		TraceIn("continueStatement", 27);
		JavaScriptParser.continueStatement_return retval = new JavaScriptParser.continueStatement_return();
		retval.Start = (CommonToken)input.LT(1);
		int continueStatement_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken CONTINUE141=null;
		CommonToken Identifier142=null;
		CommonToken SkipSpace143=null;
		CommonToken SEMI144=null;

		CommonTree CONTINUE141_tree=null;
		CommonTree Identifier142_tree=null;
		CommonTree SkipSpace143_tree=null;
		CommonTree SEMI144_tree=null;

		try { DebugEnterRule(GrammarFileName, "continueStatement");
		DebugLocation(205, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 27)) { return retval; }
			// JavaScript.g3:206:5: ( CONTINUE ( Identifier )? ( SkipSpace )* SEMI )
			DebugEnterAlt(1);
			// JavaScript.g3:206:7: CONTINUE ( Identifier )? ( SkipSpace )* SEMI
			{
			root_0 = (CommonTree)adaptor.Nil();

			DebugLocation(206, 15);
			CONTINUE141=(CommonToken)Match(input,CONTINUE,Follow._CONTINUE_in_continueStatement1663); if (state.failed) return retval;
			if ( state.backtracking == 0 ) {
			CONTINUE141_tree = (CommonTree)adaptor.Create(CONTINUE141);
			root_0 = (CommonTree)adaptor.BecomeRoot(CONTINUE141_tree, root_0);
			}
			DebugLocation(206, 17);
			// JavaScript.g3:206:17: ( Identifier )?
			int alt79=2;
			try { DebugEnterSubRule(79);
			try { DebugEnterDecision(79, decisionCanBacktrack[79]);
			int LA79_0 = input.LA(1);

			if ((LA79_0==Identifier))
			{
				alt79=1;
			}
			} finally { DebugExitDecision(79); }
			switch (alt79)
			{
			case 1:
				DebugEnterAlt(1);
				// JavaScript.g3:206:17: Identifier
				{
				DebugLocation(206, 17);
				Identifier142=(CommonToken)Match(input,Identifier,Follow._Identifier_in_continueStatement1666); if (state.failed) return retval;
				if ( state.backtracking==0 ) {
				Identifier142_tree = (CommonTree)adaptor.Create(Identifier142);
				adaptor.AddChild(root_0, Identifier142_tree);
				}

				}
				break;

			}
			} finally { DebugExitSubRule(79); }

			DebugLocation(206, 38);
			// JavaScript.g3:206:38: ( SkipSpace )*
			try { DebugEnterSubRule(80);
			while (true)
			{
				int alt80=2;
				try { DebugEnterDecision(80, decisionCanBacktrack[80]);
				int LA80_0 = input.LA(1);

				if ((LA80_0==SkipSpace))
				{
					alt80=1;
				}


				} finally { DebugExitDecision(80); }
				switch ( alt80 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:206:38: SkipSpace
					{
					DebugLocation(206, 38);
					SkipSpace143=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_continueStatement1669); if (state.failed) return retval;

					}
					break;

				default:
					goto loop80;
				}
			}

			loop80:
				;

			} finally { DebugExitSubRule(80); }

			DebugLocation(206, 45);
			SEMI144=(CommonToken)Match(input,SEMI,Follow._SEMI_in_continueStatement1673); if (state.failed) return retval;

			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("continueStatement", 27);
			LeaveRule("continueStatement", 27);
			Leave_continueStatement();
			if (state.backtracking > 0) { Memoize(input, 27, continueStatement_StartIndex); }
		}
		DebugLocation(207, 4);
		} finally { DebugExitRule(GrammarFileName, "continueStatement"); }
		return retval;

	}
	// $ANTLR end "continueStatement"

	public class breakStatement_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_breakStatement();
	partial void Leave_breakStatement();

	// $ANTLR start "breakStatement"
	// JavaScript.g3:209:1: breakStatement : BREAK ( Identifier )? ( SkipSpace )* SEMI ;
	[GrammarRule("breakStatement")]
	private JavaScriptParser.breakStatement_return breakStatement()
	{
		Enter_breakStatement();
		EnterRule("breakStatement", 28);
		TraceIn("breakStatement", 28);
		JavaScriptParser.breakStatement_return retval = new JavaScriptParser.breakStatement_return();
		retval.Start = (CommonToken)input.LT(1);
		int breakStatement_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken BREAK145=null;
		CommonToken Identifier146=null;
		CommonToken SkipSpace147=null;
		CommonToken SEMI148=null;

		CommonTree BREAK145_tree=null;
		CommonTree Identifier146_tree=null;
		CommonTree SkipSpace147_tree=null;
		CommonTree SEMI148_tree=null;

		try { DebugEnterRule(GrammarFileName, "breakStatement");
		DebugLocation(209, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 28)) { return retval; }
			// JavaScript.g3:210:5: ( BREAK ( Identifier )? ( SkipSpace )* SEMI )
			DebugEnterAlt(1);
			// JavaScript.g3:210:7: BREAK ( Identifier )? ( SkipSpace )* SEMI
			{
			root_0 = (CommonTree)adaptor.Nil();

			DebugLocation(210, 12);
			BREAK145=(CommonToken)Match(input,BREAK,Follow._BREAK_in_breakStatement1691); if (state.failed) return retval;
			if ( state.backtracking == 0 ) {
			BREAK145_tree = (CommonTree)adaptor.Create(BREAK145);
			root_0 = (CommonTree)adaptor.BecomeRoot(BREAK145_tree, root_0);
			}
			DebugLocation(210, 14);
			// JavaScript.g3:210:14: ( Identifier )?
			int alt81=2;
			try { DebugEnterSubRule(81);
			try { DebugEnterDecision(81, decisionCanBacktrack[81]);
			int LA81_0 = input.LA(1);

			if ((LA81_0==Identifier))
			{
				alt81=1;
			}
			} finally { DebugExitDecision(81); }
			switch (alt81)
			{
			case 1:
				DebugEnterAlt(1);
				// JavaScript.g3:210:14: Identifier
				{
				DebugLocation(210, 14);
				Identifier146=(CommonToken)Match(input,Identifier,Follow._Identifier_in_breakStatement1694); if (state.failed) return retval;
				if ( state.backtracking==0 ) {
				Identifier146_tree = (CommonTree)adaptor.Create(Identifier146);
				adaptor.AddChild(root_0, Identifier146_tree);
				}

				}
				break;

			}
			} finally { DebugExitSubRule(81); }

			DebugLocation(210, 35);
			// JavaScript.g3:210:35: ( SkipSpace )*
			try { DebugEnterSubRule(82);
			while (true)
			{
				int alt82=2;
				try { DebugEnterDecision(82, decisionCanBacktrack[82]);
				int LA82_0 = input.LA(1);

				if ((LA82_0==SkipSpace))
				{
					alt82=1;
				}


				} finally { DebugExitDecision(82); }
				switch ( alt82 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:210:35: SkipSpace
					{
					DebugLocation(210, 35);
					SkipSpace147=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_breakStatement1697); if (state.failed) return retval;

					}
					break;

				default:
					goto loop82;
				}
			}

			loop82:
				;

			} finally { DebugExitSubRule(82); }

			DebugLocation(210, 42);
			SEMI148=(CommonToken)Match(input,SEMI,Follow._SEMI_in_breakStatement1701); if (state.failed) return retval;

			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("breakStatement", 28);
			LeaveRule("breakStatement", 28);
			Leave_breakStatement();
			if (state.backtracking > 0) { Memoize(input, 28, breakStatement_StartIndex); }
		}
		DebugLocation(211, 4);
		} finally { DebugExitRule(GrammarFileName, "breakStatement"); }
		return retval;

	}
	// $ANTLR end "breakStatement"

	public class returnStatement_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_returnStatement();
	partial void Leave_returnStatement();

	// $ANTLR start "returnStatement"
	// JavaScript.g3:213:1: returnStatement : ( RETURN ( SkipSpace )* exp= expression ( SkipSpace )* SEMI -> ^( RETURN_STATEMENT $exp) | RETURN ( SkipSpace )* SEMI -> ^( RETURN_STATEMENT ) );
	[GrammarRule("returnStatement")]
	private JavaScriptParser.returnStatement_return returnStatement()
	{
		Enter_returnStatement();
		EnterRule("returnStatement", 29);
		TraceIn("returnStatement", 29);
		JavaScriptParser.returnStatement_return retval = new JavaScriptParser.returnStatement_return();
		retval.Start = (CommonToken)input.LT(1);
		int returnStatement_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken RETURN149=null;
		CommonToken SkipSpace150=null;
		CommonToken SkipSpace151=null;
		CommonToken SEMI152=null;
		CommonToken RETURN153=null;
		CommonToken SkipSpace154=null;
		CommonToken SEMI155=null;
		JavaScriptParser.expression_return exp = default(JavaScriptParser.expression_return);

		CommonTree RETURN149_tree=null;
		CommonTree SkipSpace150_tree=null;
		CommonTree SkipSpace151_tree=null;
		CommonTree SEMI152_tree=null;
		CommonTree RETURN153_tree=null;
		CommonTree SkipSpace154_tree=null;
		CommonTree SEMI155_tree=null;
		RewriteRuleITokenStream stream_RETURN=new RewriteRuleITokenStream(adaptor,"token RETURN");
		RewriteRuleITokenStream stream_SkipSpace=new RewriteRuleITokenStream(adaptor,"token SkipSpace");
		RewriteRuleITokenStream stream_SEMI=new RewriteRuleITokenStream(adaptor,"token SEMI");
		RewriteRuleSubtreeStream stream_expression=new RewriteRuleSubtreeStream(adaptor,"rule expression");
		try { DebugEnterRule(GrammarFileName, "returnStatement");
		DebugLocation(213, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 29)) { return retval; }
			// JavaScript.g3:214:5: ( RETURN ( SkipSpace )* exp= expression ( SkipSpace )* SEMI -> ^( RETURN_STATEMENT $exp) | RETURN ( SkipSpace )* SEMI -> ^( RETURN_STATEMENT ) )
			int alt86=2;
			try { DebugEnterDecision(86, decisionCanBacktrack[86]);
			try
			{
				alt86 = dfa86.Predict(input);
			}
			catch (NoViableAltException nvae)
			{
				DebugRecognitionException(nvae);
				throw;
			}
			} finally { DebugExitDecision(86); }
			switch (alt86)
			{
			case 1:
				DebugEnterAlt(1);
				// JavaScript.g3:214:7: RETURN ( SkipSpace )* exp= expression ( SkipSpace )* SEMI
				{
				DebugLocation(214, 7);
				RETURN149=(CommonToken)Match(input,RETURN,Follow._RETURN_in_returnStatement1719); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_RETURN.Add(RETURN149);

				DebugLocation(214, 14);
				// JavaScript.g3:214:14: ( SkipSpace )*
				try { DebugEnterSubRule(83);
				while (true)
				{
					int alt83=2;
					try { DebugEnterDecision(83, decisionCanBacktrack[83]);
					int LA83_0 = input.LA(1);

					if ((LA83_0==SkipSpace))
					{
						alt83=1;
					}


					} finally { DebugExitDecision(83); }
					switch ( alt83 )
					{
					case 1:
						DebugEnterAlt(1);
						// JavaScript.g3:214:14: SkipSpace
						{
						DebugLocation(214, 14);
						SkipSpace150=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_returnStatement1721); if (state.failed) return retval; 
						if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace150);


						}
						break;

					default:
						goto loop83;
					}
				}

				loop83:
					;

				} finally { DebugExitSubRule(83); }

				DebugLocation(214, 28);
				PushFollow(Follow._expression_in_returnStatement1726);
				exp=expression();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_expression.Add(exp.Tree);
				DebugLocation(214, 40);
				// JavaScript.g3:214:40: ( SkipSpace )*
				try { DebugEnterSubRule(84);
				while (true)
				{
					int alt84=2;
					try { DebugEnterDecision(84, decisionCanBacktrack[84]);
					int LA84_0 = input.LA(1);

					if ((LA84_0==SkipSpace))
					{
						alt84=1;
					}


					} finally { DebugExitDecision(84); }
					switch ( alt84 )
					{
					case 1:
						DebugEnterAlt(1);
						// JavaScript.g3:214:40: SkipSpace
						{
						DebugLocation(214, 40);
						SkipSpace151=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_returnStatement1728); if (state.failed) return retval; 
						if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace151);


						}
						break;

					default:
						goto loop84;
					}
				}

				loop84:
					;

				} finally { DebugExitSubRule(84); }

				DebugLocation(214, 51);
				SEMI152=(CommonToken)Match(input,SEMI,Follow._SEMI_in_returnStatement1731); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_SEMI.Add(SEMI152);



				{
				// AST REWRITE
				// elements: exp
				// token labels: 
				// rule labels: exp, retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.Tree = root_0;
				RewriteRuleSubtreeStream stream_exp=new RewriteRuleSubtreeStream(adaptor,"rule exp",exp!=null?exp.Tree:null);
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

				root_0 = (CommonTree)adaptor.Nil();
				// 215:9: -> ^( RETURN_STATEMENT $exp)
				{
					DebugLocation(215, 12);
					// JavaScript.g3:215:12: ^( RETURN_STATEMENT $exp)
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(215, 14);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(RETURN_STATEMENT, "RETURN_STATEMENT"), root_1);

					DebugLocation(215, 32);
					adaptor.AddChild(root_1, stream_exp.NextTree());

					adaptor.AddChild(root_0, root_1);
					}

				}

				retval.Tree = root_0;
				}
				}

				}
				break;
			case 2:
				DebugEnterAlt(2);
				// JavaScript.g3:216:7: RETURN ( SkipSpace )* SEMI
				{
				DebugLocation(216, 7);
				RETURN153=(CommonToken)Match(input,RETURN,Follow._RETURN_in_returnStatement1756); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_RETURN.Add(RETURN153);

				DebugLocation(216, 14);
				// JavaScript.g3:216:14: ( SkipSpace )*
				try { DebugEnterSubRule(85);
				while (true)
				{
					int alt85=2;
					try { DebugEnterDecision(85, decisionCanBacktrack[85]);
					int LA85_0 = input.LA(1);

					if ((LA85_0==SkipSpace))
					{
						alt85=1;
					}


					} finally { DebugExitDecision(85); }
					switch ( alt85 )
					{
					case 1:
						DebugEnterAlt(1);
						// JavaScript.g3:216:14: SkipSpace
						{
						DebugLocation(216, 14);
						SkipSpace154=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_returnStatement1758); if (state.failed) return retval; 
						if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace154);


						}
						break;

					default:
						goto loop85;
					}
				}

				loop85:
					;

				} finally { DebugExitSubRule(85); }

				DebugLocation(216, 25);
				SEMI155=(CommonToken)Match(input,SEMI,Follow._SEMI_in_returnStatement1761); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_SEMI.Add(SEMI155);



				{
				// AST REWRITE
				// elements: 
				// token labels: 
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.Tree = root_0;
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

				root_0 = (CommonTree)adaptor.Nil();
				// 217:9: -> ^( RETURN_STATEMENT )
				{
					DebugLocation(217, 12);
					// JavaScript.g3:217:12: ^( RETURN_STATEMENT )
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(217, 14);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(RETURN_STATEMENT, "RETURN_STATEMENT"), root_1);

					adaptor.AddChild(root_0, root_1);
					}

				}

				retval.Tree = root_0;
				}
				}

				}
				break;

			}
			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("returnStatement", 29);
			LeaveRule("returnStatement", 29);
			Leave_returnStatement();
			if (state.backtracking > 0) { Memoize(input, 29, returnStatement_StartIndex); }
		}
		DebugLocation(218, 4);
		} finally { DebugExitRule(GrammarFileName, "returnStatement"); }
		return retval;

	}
	// $ANTLR end "returnStatement"

	public class withStatement_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_withStatement();
	partial void Leave_withStatement();

	// $ANTLR start "withStatement"
	// JavaScript.g3:220:1: withStatement : 'with' ( SkipSpace )* '(' ( SkipSpace )* expression ( SkipSpace )* ')' ( SkipSpace )* statement ;
	[GrammarRule("withStatement")]
	private JavaScriptParser.withStatement_return withStatement()
	{
		Enter_withStatement();
		EnterRule("withStatement", 30);
		TraceIn("withStatement", 30);
		JavaScriptParser.withStatement_return retval = new JavaScriptParser.withStatement_return();
		retval.Start = (CommonToken)input.LT(1);
		int withStatement_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken string_literal156=null;
		CommonToken SkipSpace157=null;
		CommonToken char_literal158=null;
		CommonToken SkipSpace159=null;
		CommonToken SkipSpace161=null;
		CommonToken char_literal162=null;
		CommonToken SkipSpace163=null;
		JavaScriptParser.expression_return expression160 = default(JavaScriptParser.expression_return);
		JavaScriptParser.statement_return statement164 = default(JavaScriptParser.statement_return);

		CommonTree string_literal156_tree=null;
		CommonTree SkipSpace157_tree=null;
		CommonTree char_literal158_tree=null;
		CommonTree SkipSpace159_tree=null;
		CommonTree SkipSpace161_tree=null;
		CommonTree char_literal162_tree=null;
		CommonTree SkipSpace163_tree=null;

		try { DebugEnterRule(GrammarFileName, "withStatement");
		DebugLocation(220, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 30)) { return retval; }
			// JavaScript.g3:221:5: ( 'with' ( SkipSpace )* '(' ( SkipSpace )* expression ( SkipSpace )* ')' ( SkipSpace )* statement )
			DebugEnterAlt(1);
			// JavaScript.g3:221:7: 'with' ( SkipSpace )* '(' ( SkipSpace )* expression ( SkipSpace )* ')' ( SkipSpace )* statement
			{
			root_0 = (CommonTree)adaptor.Nil();

			DebugLocation(221, 7);
			string_literal156=(CommonToken)Match(input,146,Follow._146_in_withStatement1792); if (state.failed) return retval;
			if ( state.backtracking==0 ) {
			string_literal156_tree = (CommonTree)adaptor.Create(string_literal156);
			adaptor.AddChild(root_0, string_literal156_tree);
			}
			DebugLocation(221, 23);
			// JavaScript.g3:221:23: ( SkipSpace )*
			try { DebugEnterSubRule(87);
			while (true)
			{
				int alt87=2;
				try { DebugEnterDecision(87, decisionCanBacktrack[87]);
				int LA87_0 = input.LA(1);

				if ((LA87_0==SkipSpace))
				{
					alt87=1;
				}


				} finally { DebugExitDecision(87); }
				switch ( alt87 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:221:23: SkipSpace
					{
					DebugLocation(221, 23);
					SkipSpace157=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_withStatement1794); if (state.failed) return retval;

					}
					break;

				default:
					goto loop87;
				}
			}

			loop87:
				;

			} finally { DebugExitSubRule(87); }

			DebugLocation(221, 26);
			char_literal158=(CommonToken)Match(input,130,Follow._130_in_withStatement1798); if (state.failed) return retval;
			if ( state.backtracking==0 ) {
			char_literal158_tree = (CommonTree)adaptor.Create(char_literal158);
			adaptor.AddChild(root_0, char_literal158_tree);
			}
			DebugLocation(221, 39);
			// JavaScript.g3:221:39: ( SkipSpace )*
			try { DebugEnterSubRule(88);
			while (true)
			{
				int alt88=2;
				try { DebugEnterDecision(88, decisionCanBacktrack[88]);
				int LA88_0 = input.LA(1);

				if ((LA88_0==SkipSpace))
				{
					alt88=1;
				}


				} finally { DebugExitDecision(88); }
				switch ( alt88 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:221:39: SkipSpace
					{
					DebugLocation(221, 39);
					SkipSpace159=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_withStatement1800); if (state.failed) return retval;

					}
					break;

				default:
					goto loop88;
				}
			}

			loop88:
				;

			} finally { DebugExitSubRule(88); }

			DebugLocation(221, 42);
			PushFollow(Follow._expression_in_withStatement1804);
			expression160=expression();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) adaptor.AddChild(root_0, expression160.Tree);
			DebugLocation(221, 62);
			// JavaScript.g3:221:62: ( SkipSpace )*
			try { DebugEnterSubRule(89);
			while (true)
			{
				int alt89=2;
				try { DebugEnterDecision(89, decisionCanBacktrack[89]);
				int LA89_0 = input.LA(1);

				if ((LA89_0==SkipSpace))
				{
					alt89=1;
				}


				} finally { DebugExitDecision(89); }
				switch ( alt89 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:221:62: SkipSpace
					{
					DebugLocation(221, 62);
					SkipSpace161=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_withStatement1806); if (state.failed) return retval;

					}
					break;

				default:
					goto loop89;
				}
			}

			loop89:
				;

			} finally { DebugExitSubRule(89); }

			DebugLocation(221, 65);
			char_literal162=(CommonToken)Match(input,131,Follow._131_in_withStatement1810); if (state.failed) return retval;
			if ( state.backtracking==0 ) {
			char_literal162_tree = (CommonTree)adaptor.Create(char_literal162);
			adaptor.AddChild(root_0, char_literal162_tree);
			}
			DebugLocation(221, 78);
			// JavaScript.g3:221:78: ( SkipSpace )*
			try { DebugEnterSubRule(90);
			while (true)
			{
				int alt90=2;
				try { DebugEnterDecision(90, decisionCanBacktrack[90]);
				int LA90_0 = input.LA(1);

				if ((LA90_0==SkipSpace))
				{
					alt90=1;
				}


				} finally { DebugExitDecision(90); }
				switch ( alt90 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:221:78: SkipSpace
					{
					DebugLocation(221, 78);
					SkipSpace163=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_withStatement1812); if (state.failed) return retval;

					}
					break;

				default:
					goto loop90;
				}
			}

			loop90:
				;

			} finally { DebugExitSubRule(90); }

			DebugLocation(221, 81);
			PushFollow(Follow._statement_in_withStatement1816);
			statement164=statement();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) adaptor.AddChild(root_0, statement164.Tree);

			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("withStatement", 30);
			LeaveRule("withStatement", 30);
			Leave_withStatement();
			if (state.backtracking > 0) { Memoize(input, 30, withStatement_StartIndex); }
		}
		DebugLocation(222, 4);
		} finally { DebugExitRule(GrammarFileName, "withStatement"); }
		return retval;

	}
	// $ANTLR end "withStatement"

	public class labelledStatement_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_labelledStatement();
	partial void Leave_labelledStatement();

	// $ANTLR start "labelledStatement"
	// JavaScript.g3:224:1: labelledStatement : Identifier ( SkipSpace )* ':' ( SkipSpace )* statement ;
	[GrammarRule("labelledStatement")]
	private JavaScriptParser.labelledStatement_return labelledStatement()
	{
		Enter_labelledStatement();
		EnterRule("labelledStatement", 31);
		TraceIn("labelledStatement", 31);
		JavaScriptParser.labelledStatement_return retval = new JavaScriptParser.labelledStatement_return();
		retval.Start = (CommonToken)input.LT(1);
		int labelledStatement_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken Identifier165=null;
		CommonToken SkipSpace166=null;
		CommonToken char_literal167=null;
		CommonToken SkipSpace168=null;
		JavaScriptParser.statement_return statement169 = default(JavaScriptParser.statement_return);

		CommonTree Identifier165_tree=null;
		CommonTree SkipSpace166_tree=null;
		CommonTree char_literal167_tree=null;
		CommonTree SkipSpace168_tree=null;

		try { DebugEnterRule(GrammarFileName, "labelledStatement");
		DebugLocation(224, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 31)) { return retval; }
			// JavaScript.g3:225:5: ( Identifier ( SkipSpace )* ':' ( SkipSpace )* statement )
			DebugEnterAlt(1);
			// JavaScript.g3:225:7: Identifier ( SkipSpace )* ':' ( SkipSpace )* statement
			{
			root_0 = (CommonTree)adaptor.Nil();

			DebugLocation(225, 7);
			Identifier165=(CommonToken)Match(input,Identifier,Follow._Identifier_in_labelledStatement1833); if (state.failed) return retval;
			if ( state.backtracking==0 ) {
			Identifier165_tree = (CommonTree)adaptor.Create(Identifier165);
			adaptor.AddChild(root_0, Identifier165_tree);
			}
			DebugLocation(225, 27);
			// JavaScript.g3:225:27: ( SkipSpace )*
			try { DebugEnterSubRule(91);
			while (true)
			{
				int alt91=2;
				try { DebugEnterDecision(91, decisionCanBacktrack[91]);
				int LA91_0 = input.LA(1);

				if ((LA91_0==SkipSpace))
				{
					alt91=1;
				}


				} finally { DebugExitDecision(91); }
				switch ( alt91 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:225:27: SkipSpace
					{
					DebugLocation(225, 27);
					SkipSpace166=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_labelledStatement1835); if (state.failed) return retval;

					}
					break;

				default:
					goto loop91;
				}
			}

			loop91:
				;

			} finally { DebugExitSubRule(91); }

			DebugLocation(225, 30);
			char_literal167=(CommonToken)Match(input,133,Follow._133_in_labelledStatement1839); if (state.failed) return retval;
			if ( state.backtracking==0 ) {
			char_literal167_tree = (CommonTree)adaptor.Create(char_literal167);
			adaptor.AddChild(root_0, char_literal167_tree);
			}
			DebugLocation(225, 43);
			// JavaScript.g3:225:43: ( SkipSpace )*
			try { DebugEnterSubRule(92);
			while (true)
			{
				int alt92=2;
				try { DebugEnterDecision(92, decisionCanBacktrack[92]);
				int LA92_0 = input.LA(1);

				if ((LA92_0==SkipSpace))
				{
					alt92=1;
				}


				} finally { DebugExitDecision(92); }
				switch ( alt92 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:225:43: SkipSpace
					{
					DebugLocation(225, 43);
					SkipSpace168=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_labelledStatement1841); if (state.failed) return retval;

					}
					break;

				default:
					goto loop92;
				}
			}

			loop92:
				;

			} finally { DebugExitSubRule(92); }

			DebugLocation(225, 46);
			PushFollow(Follow._statement_in_labelledStatement1845);
			statement169=statement();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) adaptor.AddChild(root_0, statement169.Tree);

			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("labelledStatement", 31);
			LeaveRule("labelledStatement", 31);
			Leave_labelledStatement();
			if (state.backtracking > 0) { Memoize(input, 31, labelledStatement_StartIndex); }
		}
		DebugLocation(226, 4);
		} finally { DebugExitRule(GrammarFileName, "labelledStatement"); }
		return retval;

	}
	// $ANTLR end "labelledStatement"

	public class switchStatement_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_switchStatement();
	partial void Leave_switchStatement();

	// $ANTLR start "switchStatement"
	// JavaScript.g3:228:1: switchStatement : 'switch' ( SkipSpace )* '(' ( SkipSpace )* expression ( SkipSpace )* ')' ( SkipSpace )* caseBlock ;
	[GrammarRule("switchStatement")]
	private JavaScriptParser.switchStatement_return switchStatement()
	{
		Enter_switchStatement();
		EnterRule("switchStatement", 32);
		TraceIn("switchStatement", 32);
		JavaScriptParser.switchStatement_return retval = new JavaScriptParser.switchStatement_return();
		retval.Start = (CommonToken)input.LT(1);
		int switchStatement_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken string_literal170=null;
		CommonToken SkipSpace171=null;
		CommonToken char_literal172=null;
		CommonToken SkipSpace173=null;
		CommonToken SkipSpace175=null;
		CommonToken char_literal176=null;
		CommonToken SkipSpace177=null;
		JavaScriptParser.expression_return expression174 = default(JavaScriptParser.expression_return);
		JavaScriptParser.caseBlock_return caseBlock178 = default(JavaScriptParser.caseBlock_return);

		CommonTree string_literal170_tree=null;
		CommonTree SkipSpace171_tree=null;
		CommonTree char_literal172_tree=null;
		CommonTree SkipSpace173_tree=null;
		CommonTree SkipSpace175_tree=null;
		CommonTree char_literal176_tree=null;
		CommonTree SkipSpace177_tree=null;

		try { DebugEnterRule(GrammarFileName, "switchStatement");
		DebugLocation(228, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 32)) { return retval; }
			// JavaScript.g3:229:5: ( 'switch' ( SkipSpace )* '(' ( SkipSpace )* expression ( SkipSpace )* ')' ( SkipSpace )* caseBlock )
			DebugEnterAlt(1);
			// JavaScript.g3:229:7: 'switch' ( SkipSpace )* '(' ( SkipSpace )* expression ( SkipSpace )* ')' ( SkipSpace )* caseBlock
			{
			root_0 = (CommonTree)adaptor.Nil();

			DebugLocation(229, 7);
			string_literal170=(CommonToken)Match(input,144,Follow._144_in_switchStatement1862); if (state.failed) return retval;
			if ( state.backtracking==0 ) {
			string_literal170_tree = (CommonTree)adaptor.Create(string_literal170);
			adaptor.AddChild(root_0, string_literal170_tree);
			}
			DebugLocation(229, 25);
			// JavaScript.g3:229:25: ( SkipSpace )*
			try { DebugEnterSubRule(93);
			while (true)
			{
				int alt93=2;
				try { DebugEnterDecision(93, decisionCanBacktrack[93]);
				int LA93_0 = input.LA(1);

				if ((LA93_0==SkipSpace))
				{
					alt93=1;
				}


				} finally { DebugExitDecision(93); }
				switch ( alt93 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:229:25: SkipSpace
					{
					DebugLocation(229, 25);
					SkipSpace171=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_switchStatement1864); if (state.failed) return retval;

					}
					break;

				default:
					goto loop93;
				}
			}

			loop93:
				;

			} finally { DebugExitSubRule(93); }

			DebugLocation(229, 28);
			char_literal172=(CommonToken)Match(input,130,Follow._130_in_switchStatement1868); if (state.failed) return retval;
			if ( state.backtracking==0 ) {
			char_literal172_tree = (CommonTree)adaptor.Create(char_literal172);
			adaptor.AddChild(root_0, char_literal172_tree);
			}
			DebugLocation(229, 41);
			// JavaScript.g3:229:41: ( SkipSpace )*
			try { DebugEnterSubRule(94);
			while (true)
			{
				int alt94=2;
				try { DebugEnterDecision(94, decisionCanBacktrack[94]);
				int LA94_0 = input.LA(1);

				if ((LA94_0==SkipSpace))
				{
					alt94=1;
				}


				} finally { DebugExitDecision(94); }
				switch ( alt94 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:229:41: SkipSpace
					{
					DebugLocation(229, 41);
					SkipSpace173=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_switchStatement1870); if (state.failed) return retval;

					}
					break;

				default:
					goto loop94;
				}
			}

			loop94:
				;

			} finally { DebugExitSubRule(94); }

			DebugLocation(229, 44);
			PushFollow(Follow._expression_in_switchStatement1874);
			expression174=expression();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) adaptor.AddChild(root_0, expression174.Tree);
			DebugLocation(229, 64);
			// JavaScript.g3:229:64: ( SkipSpace )*
			try { DebugEnterSubRule(95);
			while (true)
			{
				int alt95=2;
				try { DebugEnterDecision(95, decisionCanBacktrack[95]);
				int LA95_0 = input.LA(1);

				if ((LA95_0==SkipSpace))
				{
					alt95=1;
				}


				} finally { DebugExitDecision(95); }
				switch ( alt95 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:229:64: SkipSpace
					{
					DebugLocation(229, 64);
					SkipSpace175=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_switchStatement1876); if (state.failed) return retval;

					}
					break;

				default:
					goto loop95;
				}
			}

			loop95:
				;

			} finally { DebugExitSubRule(95); }

			DebugLocation(229, 67);
			char_literal176=(CommonToken)Match(input,131,Follow._131_in_switchStatement1880); if (state.failed) return retval;
			if ( state.backtracking==0 ) {
			char_literal176_tree = (CommonTree)adaptor.Create(char_literal176);
			adaptor.AddChild(root_0, char_literal176_tree);
			}
			DebugLocation(229, 80);
			// JavaScript.g3:229:80: ( SkipSpace )*
			try { DebugEnterSubRule(96);
			while (true)
			{
				int alt96=2;
				try { DebugEnterDecision(96, decisionCanBacktrack[96]);
				int LA96_0 = input.LA(1);

				if ((LA96_0==SkipSpace))
				{
					alt96=1;
				}


				} finally { DebugExitDecision(96); }
				switch ( alt96 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:229:80: SkipSpace
					{
					DebugLocation(229, 80);
					SkipSpace177=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_switchStatement1882); if (state.failed) return retval;

					}
					break;

				default:
					goto loop96;
				}
			}

			loop96:
				;

			} finally { DebugExitSubRule(96); }

			DebugLocation(229, 83);
			PushFollow(Follow._caseBlock_in_switchStatement1886);
			caseBlock178=caseBlock();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) adaptor.AddChild(root_0, caseBlock178.Tree);

			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("switchStatement", 32);
			LeaveRule("switchStatement", 32);
			Leave_switchStatement();
			if (state.backtracking > 0) { Memoize(input, 32, switchStatement_StartIndex); }
		}
		DebugLocation(230, 4);
		} finally { DebugExitRule(GrammarFileName, "switchStatement"); }
		return retval;

	}
	// $ANTLR end "switchStatement"

	public class caseBlock_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_caseBlock();
	partial void Leave_caseBlock();

	// $ANTLR start "caseBlock"
	// JavaScript.g3:232:1: caseBlock : '{' ( ( SkipSpace )* caseClause )* ( ( SkipSpace )* defaultClause ( ( SkipSpace )* caseClause )* )? ( SkipSpace )* '}' ;
	[GrammarRule("caseBlock")]
	private JavaScriptParser.caseBlock_return caseBlock()
	{
		Enter_caseBlock();
		EnterRule("caseBlock", 33);
		TraceIn("caseBlock", 33);
		JavaScriptParser.caseBlock_return retval = new JavaScriptParser.caseBlock_return();
		retval.Start = (CommonToken)input.LT(1);
		int caseBlock_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken char_literal179=null;
		CommonToken SkipSpace180=null;
		CommonToken SkipSpace182=null;
		CommonToken SkipSpace184=null;
		CommonToken SkipSpace186=null;
		CommonToken char_literal187=null;
		JavaScriptParser.caseClause_return caseClause181 = default(JavaScriptParser.caseClause_return);
		JavaScriptParser.defaultClause_return defaultClause183 = default(JavaScriptParser.defaultClause_return);
		JavaScriptParser.caseClause_return caseClause185 = default(JavaScriptParser.caseClause_return);

		CommonTree char_literal179_tree=null;
		CommonTree SkipSpace180_tree=null;
		CommonTree SkipSpace182_tree=null;
		CommonTree SkipSpace184_tree=null;
		CommonTree SkipSpace186_tree=null;
		CommonTree char_literal187_tree=null;

		try { DebugEnterRule(GrammarFileName, "caseBlock");
		DebugLocation(232, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 33)) { return retval; }
			// JavaScript.g3:233:5: ( '{' ( ( SkipSpace )* caseClause )* ( ( SkipSpace )* defaultClause ( ( SkipSpace )* caseClause )* )? ( SkipSpace )* '}' )
			DebugEnterAlt(1);
			// JavaScript.g3:233:7: '{' ( ( SkipSpace )* caseClause )* ( ( SkipSpace )* defaultClause ( ( SkipSpace )* caseClause )* )? ( SkipSpace )* '}'
			{
			root_0 = (CommonTree)adaptor.Nil();

			DebugLocation(233, 7);
			char_literal179=(CommonToken)Match(input,147,Follow._147_in_caseBlock1903); if (state.failed) return retval;
			if ( state.backtracking==0 ) {
			char_literal179_tree = (CommonTree)adaptor.Create(char_literal179);
			adaptor.AddChild(root_0, char_literal179_tree);
			}
			DebugLocation(233, 11);
			// JavaScript.g3:233:11: ( ( SkipSpace )* caseClause )*
			try { DebugEnterSubRule(98);
			while (true)
			{
				int alt98=2;
				try { DebugEnterDecision(98, decisionCanBacktrack[98]);
				try
				{
					alt98 = dfa98.Predict(input);
				}
				catch (NoViableAltException nvae)
				{
					DebugRecognitionException(nvae);
					throw;
				}
				} finally { DebugExitDecision(98); }
				switch ( alt98 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:233:12: ( SkipSpace )* caseClause
					{
					DebugLocation(233, 21);
					// JavaScript.g3:233:21: ( SkipSpace )*
					try { DebugEnterSubRule(97);
					while (true)
					{
						int alt97=2;
						try { DebugEnterDecision(97, decisionCanBacktrack[97]);
						int LA97_0 = input.LA(1);

						if ((LA97_0==SkipSpace))
						{
							alt97=1;
						}


						} finally { DebugExitDecision(97); }
						switch ( alt97 )
						{
						case 1:
							DebugEnterAlt(1);
							// JavaScript.g3:233:21: SkipSpace
							{
							DebugLocation(233, 21);
							SkipSpace180=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_caseBlock1906); if (state.failed) return retval;

							}
							break;

						default:
							goto loop97;
						}
					}

					loop97:
						;

					} finally { DebugExitSubRule(97); }

					DebugLocation(233, 24);
					PushFollow(Follow._caseClause_in_caseBlock1910);
					caseClause181=caseClause();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) adaptor.AddChild(root_0, caseClause181.Tree);

					}
					break;

				default:
					goto loop98;
				}
			}

			loop98:
				;

			} finally { DebugExitSubRule(98); }

			DebugLocation(233, 37);
			// JavaScript.g3:233:37: ( ( SkipSpace )* defaultClause ( ( SkipSpace )* caseClause )* )?
			int alt102=2;
			try { DebugEnterSubRule(102);
			try { DebugEnterDecision(102, decisionCanBacktrack[102]);
			try
			{
				alt102 = dfa102.Predict(input);
			}
			catch (NoViableAltException nvae)
			{
				DebugRecognitionException(nvae);
				throw;
			}
			} finally { DebugExitDecision(102); }
			switch (alt102)
			{
			case 1:
				DebugEnterAlt(1);
				// JavaScript.g3:233:38: ( SkipSpace )* defaultClause ( ( SkipSpace )* caseClause )*
				{
				DebugLocation(233, 47);
				// JavaScript.g3:233:47: ( SkipSpace )*
				try { DebugEnterSubRule(99);
				while (true)
				{
					int alt99=2;
					try { DebugEnterDecision(99, decisionCanBacktrack[99]);
					int LA99_0 = input.LA(1);

					if ((LA99_0==SkipSpace))
					{
						alt99=1;
					}


					} finally { DebugExitDecision(99); }
					switch ( alt99 )
					{
					case 1:
						DebugEnterAlt(1);
						// JavaScript.g3:233:47: SkipSpace
						{
						DebugLocation(233, 47);
						SkipSpace182=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_caseBlock1915); if (state.failed) return retval;

						}
						break;

					default:
						goto loop99;
					}
				}

				loop99:
					;

				} finally { DebugExitSubRule(99); }

				DebugLocation(233, 50);
				PushFollow(Follow._defaultClause_in_caseBlock1919);
				defaultClause183=defaultClause();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, defaultClause183.Tree);
				DebugLocation(233, 64);
				// JavaScript.g3:233:64: ( ( SkipSpace )* caseClause )*
				try { DebugEnterSubRule(101);
				while (true)
				{
					int alt101=2;
					try { DebugEnterDecision(101, decisionCanBacktrack[101]);
					try
					{
						alt101 = dfa101.Predict(input);
					}
					catch (NoViableAltException nvae)
					{
						DebugRecognitionException(nvae);
						throw;
					}
					} finally { DebugExitDecision(101); }
					switch ( alt101 )
					{
					case 1:
						DebugEnterAlt(1);
						// JavaScript.g3:233:65: ( SkipSpace )* caseClause
						{
						DebugLocation(233, 74);
						// JavaScript.g3:233:74: ( SkipSpace )*
						try { DebugEnterSubRule(100);
						while (true)
						{
							int alt100=2;
							try { DebugEnterDecision(100, decisionCanBacktrack[100]);
							int LA100_0 = input.LA(1);

							if ((LA100_0==SkipSpace))
							{
								alt100=1;
							}


							} finally { DebugExitDecision(100); }
							switch ( alt100 )
							{
							case 1:
								DebugEnterAlt(1);
								// JavaScript.g3:233:74: SkipSpace
								{
								DebugLocation(233, 74);
								SkipSpace184=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_caseBlock1922); if (state.failed) return retval;

								}
								break;

							default:
								goto loop100;
							}
						}

						loop100:
							;

						} finally { DebugExitSubRule(100); }

						DebugLocation(233, 77);
						PushFollow(Follow._caseClause_in_caseBlock1926);
						caseClause185=caseClause();
						PopFollow();
						if (state.failed) return retval;
						if ( state.backtracking == 0 ) adaptor.AddChild(root_0, caseClause185.Tree);

						}
						break;

					default:
						goto loop101;
					}
				}

				loop101:
					;

				} finally { DebugExitSubRule(101); }


				}
				break;

			}
			} finally { DebugExitSubRule(102); }

			DebugLocation(233, 101);
			// JavaScript.g3:233:101: ( SkipSpace )*
			try { DebugEnterSubRule(103);
			while (true)
			{
				int alt103=2;
				try { DebugEnterDecision(103, decisionCanBacktrack[103]);
				int LA103_0 = input.LA(1);

				if ((LA103_0==SkipSpace))
				{
					alt103=1;
				}


				} finally { DebugExitDecision(103); }
				switch ( alt103 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:233:101: SkipSpace
					{
					DebugLocation(233, 101);
					SkipSpace186=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_caseBlock1932); if (state.failed) return retval;

					}
					break;

				default:
					goto loop103;
				}
			}

			loop103:
				;

			} finally { DebugExitSubRule(103); }

			DebugLocation(233, 104);
			char_literal187=(CommonToken)Match(input,148,Follow._148_in_caseBlock1936); if (state.failed) return retval;
			if ( state.backtracking==0 ) {
			char_literal187_tree = (CommonTree)adaptor.Create(char_literal187);
			adaptor.AddChild(root_0, char_literal187_tree);
			}

			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("caseBlock", 33);
			LeaveRule("caseBlock", 33);
			Leave_caseBlock();
			if (state.backtracking > 0) { Memoize(input, 33, caseBlock_StartIndex); }
		}
		DebugLocation(234, 4);
		} finally { DebugExitRule(GrammarFileName, "caseBlock"); }
		return retval;

	}
	// $ANTLR end "caseBlock"

	public class caseClause_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_caseClause();
	partial void Leave_caseClause();

	// $ANTLR start "caseClause"
	// JavaScript.g3:236:1: caseClause : 'case' ( SkipSpace )* expression ( SkipSpace )* ':' ( SkipSpace )* ( statementList )? ;
	[GrammarRule("caseClause")]
	private JavaScriptParser.caseClause_return caseClause()
	{
		Enter_caseClause();
		EnterRule("caseClause", 34);
		TraceIn("caseClause", 34);
		JavaScriptParser.caseClause_return retval = new JavaScriptParser.caseClause_return();
		retval.Start = (CommonToken)input.LT(1);
		int caseClause_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken string_literal188=null;
		CommonToken SkipSpace189=null;
		CommonToken SkipSpace191=null;
		CommonToken char_literal192=null;
		CommonToken SkipSpace193=null;
		JavaScriptParser.expression_return expression190 = default(JavaScriptParser.expression_return);
		JavaScriptParser.statementList_return statementList194 = default(JavaScriptParser.statementList_return);

		CommonTree string_literal188_tree=null;
		CommonTree SkipSpace189_tree=null;
		CommonTree SkipSpace191_tree=null;
		CommonTree char_literal192_tree=null;
		CommonTree SkipSpace193_tree=null;

		try { DebugEnterRule(GrammarFileName, "caseClause");
		DebugLocation(236, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 34)) { return retval; }
			// JavaScript.g3:237:5: ( 'case' ( SkipSpace )* expression ( SkipSpace )* ':' ( SkipSpace )* ( statementList )? )
			DebugEnterAlt(1);
			// JavaScript.g3:237:7: 'case' ( SkipSpace )* expression ( SkipSpace )* ':' ( SkipSpace )* ( statementList )?
			{
			root_0 = (CommonTree)adaptor.Nil();

			DebugLocation(237, 7);
			string_literal188=(CommonToken)Match(input,137,Follow._137_in_caseClause1953); if (state.failed) return retval;
			if ( state.backtracking==0 ) {
			string_literal188_tree = (CommonTree)adaptor.Create(string_literal188);
			adaptor.AddChild(root_0, string_literal188_tree);
			}
			DebugLocation(237, 23);
			// JavaScript.g3:237:23: ( SkipSpace )*
			try { DebugEnterSubRule(104);
			while (true)
			{
				int alt104=2;
				try { DebugEnterDecision(104, decisionCanBacktrack[104]);
				int LA104_0 = input.LA(1);

				if ((LA104_0==SkipSpace))
				{
					alt104=1;
				}


				} finally { DebugExitDecision(104); }
				switch ( alt104 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:237:23: SkipSpace
					{
					DebugLocation(237, 23);
					SkipSpace189=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_caseClause1955); if (state.failed) return retval;

					}
					break;

				default:
					goto loop104;
				}
			}

			loop104:
				;

			} finally { DebugExitSubRule(104); }

			DebugLocation(237, 26);
			PushFollow(Follow._expression_in_caseClause1959);
			expression190=expression();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) adaptor.AddChild(root_0, expression190.Tree);
			DebugLocation(237, 46);
			// JavaScript.g3:237:46: ( SkipSpace )*
			try { DebugEnterSubRule(105);
			while (true)
			{
				int alt105=2;
				try { DebugEnterDecision(105, decisionCanBacktrack[105]);
				int LA105_0 = input.LA(1);

				if ((LA105_0==SkipSpace))
				{
					alt105=1;
				}


				} finally { DebugExitDecision(105); }
				switch ( alt105 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:237:46: SkipSpace
					{
					DebugLocation(237, 46);
					SkipSpace191=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_caseClause1961); if (state.failed) return retval;

					}
					break;

				default:
					goto loop105;
				}
			}

			loop105:
				;

			} finally { DebugExitSubRule(105); }

			DebugLocation(237, 49);
			char_literal192=(CommonToken)Match(input,133,Follow._133_in_caseClause1965); if (state.failed) return retval;
			if ( state.backtracking==0 ) {
			char_literal192_tree = (CommonTree)adaptor.Create(char_literal192);
			adaptor.AddChild(root_0, char_literal192_tree);
			}
			DebugLocation(237, 62);
			// JavaScript.g3:237:62: ( SkipSpace )*
			try { DebugEnterSubRule(106);
			while (true)
			{
				int alt106=2;
				try { DebugEnterDecision(106, decisionCanBacktrack[106]);
				int LA106_0 = input.LA(1);

				if ((LA106_0==SkipSpace))
				{
					int LA106_2 = input.LA(2);

					if ((EvaluatePredicate(synpred120_JavaScript_fragment)))
					{
						alt106=1;
					}


				}


				} finally { DebugExitDecision(106); }
				switch ( alt106 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:237:62: SkipSpace
					{
					DebugLocation(237, 62);
					SkipSpace193=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_caseClause1967); if (state.failed) return retval;

					}
					break;

				default:
					goto loop106;
				}
			}

			loop106:
				;

			} finally { DebugExitSubRule(106); }

			DebugLocation(237, 65);
			// JavaScript.g3:237:65: ( statementList )?
			int alt107=2;
			try { DebugEnterSubRule(107);
			try { DebugEnterDecision(107, decisionCanBacktrack[107]);
			int LA107_0 = input.LA(1);

			if ((LA107_0==BIT_NOT||(LA107_0>=BREAK && LA107_0<=BoolLiteral)||LA107_0==CONTINUE||LA107_0==DELETE||LA107_0==DOUBLE_NOT||LA107_0==FALSE||LA107_0==IMPORT_START||LA107_0==Identifier||LA107_0==MINUS||LA107_0==MINUS_INC||LA107_0==NEW||(LA107_0>=NOT && LA107_0<=NULL)||LA107_0==NumericLiteral||LA107_0==PLUS||LA107_0==PLUS_INC||LA107_0==RETURN||LA107_0==SEMI||(LA107_0>=StringLiteral && LA107_0<=THROW)||LA107_0==TRY||LA107_0==TYPE_OF||(LA107_0>=VAR && LA107_0<=VOID)||LA107_0==130||LA107_0==135||LA107_0==139||(LA107_0>=141 && LA107_0<=147)))
			{
				alt107=1;
			}
			} finally { DebugExitDecision(107); }
			switch (alt107)
			{
			case 1:
				DebugEnterAlt(1);
				// JavaScript.g3:237:65: statementList
				{
				DebugLocation(237, 65);
				PushFollow(Follow._statementList_in_caseClause1971);
				statementList194=statementList();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, statementList194.Tree);

				}
				break;

			}
			} finally { DebugExitSubRule(107); }


			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("caseClause", 34);
			LeaveRule("caseClause", 34);
			Leave_caseClause();
			if (state.backtracking > 0) { Memoize(input, 34, caseClause_StartIndex); }
		}
		DebugLocation(238, 4);
		} finally { DebugExitRule(GrammarFileName, "caseClause"); }
		return retval;

	}
	// $ANTLR end "caseClause"

	public class defaultClause_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_defaultClause();
	partial void Leave_defaultClause();

	// $ANTLR start "defaultClause"
	// JavaScript.g3:240:1: defaultClause : 'default' ( SkipSpace )* ':' ( SkipSpace )* ( statementList )? ;
	[GrammarRule("defaultClause")]
	private JavaScriptParser.defaultClause_return defaultClause()
	{
		Enter_defaultClause();
		EnterRule("defaultClause", 35);
		TraceIn("defaultClause", 35);
		JavaScriptParser.defaultClause_return retval = new JavaScriptParser.defaultClause_return();
		retval.Start = (CommonToken)input.LT(1);
		int defaultClause_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken string_literal195=null;
		CommonToken SkipSpace196=null;
		CommonToken char_literal197=null;
		CommonToken SkipSpace198=null;
		JavaScriptParser.statementList_return statementList199 = default(JavaScriptParser.statementList_return);

		CommonTree string_literal195_tree=null;
		CommonTree SkipSpace196_tree=null;
		CommonTree char_literal197_tree=null;
		CommonTree SkipSpace198_tree=null;

		try { DebugEnterRule(GrammarFileName, "defaultClause");
		DebugLocation(240, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 35)) { return retval; }
			// JavaScript.g3:241:5: ( 'default' ( SkipSpace )* ':' ( SkipSpace )* ( statementList )? )
			DebugEnterAlt(1);
			// JavaScript.g3:241:7: 'default' ( SkipSpace )* ':' ( SkipSpace )* ( statementList )?
			{
			root_0 = (CommonTree)adaptor.Nil();

			DebugLocation(241, 7);
			string_literal195=(CommonToken)Match(input,138,Follow._138_in_defaultClause1989); if (state.failed) return retval;
			if ( state.backtracking==0 ) {
			string_literal195_tree = (CommonTree)adaptor.Create(string_literal195);
			adaptor.AddChild(root_0, string_literal195_tree);
			}
			DebugLocation(241, 26);
			// JavaScript.g3:241:26: ( SkipSpace )*
			try { DebugEnterSubRule(108);
			while (true)
			{
				int alt108=2;
				try { DebugEnterDecision(108, decisionCanBacktrack[108]);
				int LA108_0 = input.LA(1);

				if ((LA108_0==SkipSpace))
				{
					alt108=1;
				}


				} finally { DebugExitDecision(108); }
				switch ( alt108 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:241:26: SkipSpace
					{
					DebugLocation(241, 26);
					SkipSpace196=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_defaultClause1991); if (state.failed) return retval;

					}
					break;

				default:
					goto loop108;
				}
			}

			loop108:
				;

			} finally { DebugExitSubRule(108); }

			DebugLocation(241, 29);
			char_literal197=(CommonToken)Match(input,133,Follow._133_in_defaultClause1995); if (state.failed) return retval;
			if ( state.backtracking==0 ) {
			char_literal197_tree = (CommonTree)adaptor.Create(char_literal197);
			adaptor.AddChild(root_0, char_literal197_tree);
			}
			DebugLocation(241, 42);
			// JavaScript.g3:241:42: ( SkipSpace )*
			try { DebugEnterSubRule(109);
			while (true)
			{
				int alt109=2;
				try { DebugEnterDecision(109, decisionCanBacktrack[109]);
				int LA109_0 = input.LA(1);

				if ((LA109_0==SkipSpace))
				{
					int LA109_2 = input.LA(2);

					if ((EvaluatePredicate(synpred123_JavaScript_fragment)))
					{
						alt109=1;
					}


				}


				} finally { DebugExitDecision(109); }
				switch ( alt109 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:241:42: SkipSpace
					{
					DebugLocation(241, 42);
					SkipSpace198=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_defaultClause1997); if (state.failed) return retval;

					}
					break;

				default:
					goto loop109;
				}
			}

			loop109:
				;

			} finally { DebugExitSubRule(109); }

			DebugLocation(241, 45);
			// JavaScript.g3:241:45: ( statementList )?
			int alt110=2;
			try { DebugEnterSubRule(110);
			try { DebugEnterDecision(110, decisionCanBacktrack[110]);
			int LA110_0 = input.LA(1);

			if ((LA110_0==BIT_NOT||(LA110_0>=BREAK && LA110_0<=BoolLiteral)||LA110_0==CONTINUE||LA110_0==DELETE||LA110_0==DOUBLE_NOT||LA110_0==FALSE||LA110_0==IMPORT_START||LA110_0==Identifier||LA110_0==MINUS||LA110_0==MINUS_INC||LA110_0==NEW||(LA110_0>=NOT && LA110_0<=NULL)||LA110_0==NumericLiteral||LA110_0==PLUS||LA110_0==PLUS_INC||LA110_0==RETURN||LA110_0==SEMI||(LA110_0>=StringLiteral && LA110_0<=THROW)||LA110_0==TRY||LA110_0==TYPE_OF||(LA110_0>=VAR && LA110_0<=VOID)||LA110_0==130||LA110_0==135||LA110_0==139||(LA110_0>=141 && LA110_0<=147)))
			{
				alt110=1;
			}
			} finally { DebugExitDecision(110); }
			switch (alt110)
			{
			case 1:
				DebugEnterAlt(1);
				// JavaScript.g3:241:45: statementList
				{
				DebugLocation(241, 45);
				PushFollow(Follow._statementList_in_defaultClause2001);
				statementList199=statementList();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, statementList199.Tree);

				}
				break;

			}
			} finally { DebugExitSubRule(110); }


			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("defaultClause", 35);
			LeaveRule("defaultClause", 35);
			Leave_defaultClause();
			if (state.backtracking > 0) { Memoize(input, 35, defaultClause_StartIndex); }
		}
		DebugLocation(242, 4);
		} finally { DebugExitRule(GrammarFileName, "defaultClause"); }
		return retval;

	}
	// $ANTLR end "defaultClause"

	public class throwStatement_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_throwStatement();
	partial void Leave_throwStatement();

	// $ANTLR start "throwStatement"
	// JavaScript.g3:244:1: throwStatement : THROW ( SkipSpace )* expression ( SkipSpace )* SEMI -> ^( THROW expression ) ;
	[GrammarRule("throwStatement")]
	private JavaScriptParser.throwStatement_return throwStatement()
	{
		Enter_throwStatement();
		EnterRule("throwStatement", 36);
		TraceIn("throwStatement", 36);
		JavaScriptParser.throwStatement_return retval = new JavaScriptParser.throwStatement_return();
		retval.Start = (CommonToken)input.LT(1);
		int throwStatement_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken THROW200=null;
		CommonToken SkipSpace201=null;
		CommonToken SkipSpace203=null;
		CommonToken SEMI204=null;
		JavaScriptParser.expression_return expression202 = default(JavaScriptParser.expression_return);

		CommonTree THROW200_tree=null;
		CommonTree SkipSpace201_tree=null;
		CommonTree SkipSpace203_tree=null;
		CommonTree SEMI204_tree=null;
		RewriteRuleITokenStream stream_THROW=new RewriteRuleITokenStream(adaptor,"token THROW");
		RewriteRuleITokenStream stream_SkipSpace=new RewriteRuleITokenStream(adaptor,"token SkipSpace");
		RewriteRuleITokenStream stream_SEMI=new RewriteRuleITokenStream(adaptor,"token SEMI");
		RewriteRuleSubtreeStream stream_expression=new RewriteRuleSubtreeStream(adaptor,"rule expression");
		try { DebugEnterRule(GrammarFileName, "throwStatement");
		DebugLocation(244, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 36)) { return retval; }
			// JavaScript.g3:245:5: ( THROW ( SkipSpace )* expression ( SkipSpace )* SEMI -> ^( THROW expression ) )
			DebugEnterAlt(1);
			// JavaScript.g3:245:7: THROW ( SkipSpace )* expression ( SkipSpace )* SEMI
			{
			DebugLocation(245, 7);
			THROW200=(CommonToken)Match(input,THROW,Follow._THROW_in_throwStatement2019); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_THROW.Add(THROW200);

			DebugLocation(245, 13);
			// JavaScript.g3:245:13: ( SkipSpace )*
			try { DebugEnterSubRule(111);
			while (true)
			{
				int alt111=2;
				try { DebugEnterDecision(111, decisionCanBacktrack[111]);
				int LA111_0 = input.LA(1);

				if ((LA111_0==SkipSpace))
				{
					alt111=1;
				}


				} finally { DebugExitDecision(111); }
				switch ( alt111 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:245:13: SkipSpace
					{
					DebugLocation(245, 13);
					SkipSpace201=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_throwStatement2021); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace201);


					}
					break;

				default:
					goto loop111;
				}
			}

			loop111:
				;

			} finally { DebugExitSubRule(111); }

			DebugLocation(245, 24);
			PushFollow(Follow._expression_in_throwStatement2024);
			expression202=expression();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_expression.Add(expression202.Tree);
			DebugLocation(245, 35);
			// JavaScript.g3:245:35: ( SkipSpace )*
			try { DebugEnterSubRule(112);
			while (true)
			{
				int alt112=2;
				try { DebugEnterDecision(112, decisionCanBacktrack[112]);
				int LA112_0 = input.LA(1);

				if ((LA112_0==SkipSpace))
				{
					alt112=1;
				}


				} finally { DebugExitDecision(112); }
				switch ( alt112 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:245:35: SkipSpace
					{
					DebugLocation(245, 35);
					SkipSpace203=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_throwStatement2026); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace203);


					}
					break;

				default:
					goto loop112;
				}
			}

			loop112:
				;

			} finally { DebugExitSubRule(112); }

			DebugLocation(245, 46);
			SEMI204=(CommonToken)Match(input,SEMI,Follow._SEMI_in_throwStatement2029); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_SEMI.Add(SEMI204);



			{
			// AST REWRITE
			// elements: THROW, expression
			// token labels: 
			// rule labels: retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 246:9: -> ^( THROW expression )
			{
				DebugLocation(246, 12);
				// JavaScript.g3:246:12: ^( THROW expression )
				{
				CommonTree root_1 = (CommonTree)adaptor.Nil();
				DebugLocation(246, 14);
				root_1 = (CommonTree)adaptor.BecomeRoot(stream_THROW.NextNode(), root_1);

				DebugLocation(246, 20);
				adaptor.AddChild(root_1, stream_expression.NextTree());

				adaptor.AddChild(root_0, root_1);
				}

			}

			retval.Tree = root_0;
			}
			}

			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("throwStatement", 36);
			LeaveRule("throwStatement", 36);
			Leave_throwStatement();
			if (state.backtracking > 0) { Memoize(input, 36, throwStatement_StartIndex); }
		}
		DebugLocation(247, 4);
		} finally { DebugExitRule(GrammarFileName, "throwStatement"); }
		return retval;

	}
	// $ANTLR end "throwStatement"

	public class tryStatement_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_tryStatement();
	partial void Leave_tryStatement();

	// $ANTLR start "tryStatement"
	// JavaScript.g3:249:1: tryStatement : TRY ( SkipSpace )* statementBlock ( SkipSpace )* ( finallyClause | catchClause ( ( SkipSpace )* finallyClause )? ) ;
	[GrammarRule("tryStatement")]
	private JavaScriptParser.tryStatement_return tryStatement()
	{
		Enter_tryStatement();
		EnterRule("tryStatement", 37);
		TraceIn("tryStatement", 37);
		JavaScriptParser.tryStatement_return retval = new JavaScriptParser.tryStatement_return();
		retval.Start = (CommonToken)input.LT(1);
		int tryStatement_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken TRY205=null;
		CommonToken SkipSpace206=null;
		CommonToken SkipSpace208=null;
		CommonToken SkipSpace211=null;
		JavaScriptParser.statementBlock_return statementBlock207 = default(JavaScriptParser.statementBlock_return);
		JavaScriptParser.finallyClause_return finallyClause209 = default(JavaScriptParser.finallyClause_return);
		JavaScriptParser.catchClause_return catchClause210 = default(JavaScriptParser.catchClause_return);
		JavaScriptParser.finallyClause_return finallyClause212 = default(JavaScriptParser.finallyClause_return);

		CommonTree TRY205_tree=null;
		CommonTree SkipSpace206_tree=null;
		CommonTree SkipSpace208_tree=null;
		CommonTree SkipSpace211_tree=null;

		try { DebugEnterRule(GrammarFileName, "tryStatement");
		DebugLocation(249, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 37)) { return retval; }
			// JavaScript.g3:250:5: ( TRY ( SkipSpace )* statementBlock ( SkipSpace )* ( finallyClause | catchClause ( ( SkipSpace )* finallyClause )? ) )
			DebugEnterAlt(1);
			// JavaScript.g3:250:7: TRY ( SkipSpace )* statementBlock ( SkipSpace )* ( finallyClause | catchClause ( ( SkipSpace )* finallyClause )? )
			{
			root_0 = (CommonTree)adaptor.Nil();

			DebugLocation(250, 7);
			TRY205=(CommonToken)Match(input,TRY,Follow._TRY_in_tryStatement2062); if (state.failed) return retval;
			if ( state.backtracking==0 ) {
			TRY205_tree = (CommonTree)adaptor.Create(TRY205);
			adaptor.AddChild(root_0, TRY205_tree);
			}
			DebugLocation(250, 20);
			// JavaScript.g3:250:20: ( SkipSpace )*
			try { DebugEnterSubRule(113);
			while (true)
			{
				int alt113=2;
				try { DebugEnterDecision(113, decisionCanBacktrack[113]);
				int LA113_0 = input.LA(1);

				if ((LA113_0==SkipSpace))
				{
					alt113=1;
				}


				} finally { DebugExitDecision(113); }
				switch ( alt113 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:250:20: SkipSpace
					{
					DebugLocation(250, 20);
					SkipSpace206=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_tryStatement2064); if (state.failed) return retval;

					}
					break;

				default:
					goto loop113;
				}
			}

			loop113:
				;

			} finally { DebugExitSubRule(113); }

			DebugLocation(250, 23);
			PushFollow(Follow._statementBlock_in_tryStatement2068);
			statementBlock207=statementBlock();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) adaptor.AddChild(root_0, statementBlock207.Tree);
			DebugLocation(250, 47);
			// JavaScript.g3:250:47: ( SkipSpace )*
			try { DebugEnterSubRule(114);
			while (true)
			{
				int alt114=2;
				try { DebugEnterDecision(114, decisionCanBacktrack[114]);
				int LA114_0 = input.LA(1);

				if ((LA114_0==SkipSpace))
				{
					alt114=1;
				}


				} finally { DebugExitDecision(114); }
				switch ( alt114 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:250:47: SkipSpace
					{
					DebugLocation(250, 47);
					SkipSpace208=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_tryStatement2070); if (state.failed) return retval;

					}
					break;

				default:
					goto loop114;
				}
			}

			loop114:
				;

			} finally { DebugExitSubRule(114); }

			DebugLocation(250, 50);
			// JavaScript.g3:250:50: ( finallyClause | catchClause ( ( SkipSpace )* finallyClause )? )
			int alt117=2;
			try { DebugEnterSubRule(117);
			try { DebugEnterDecision(117, decisionCanBacktrack[117]);
			int LA117_0 = input.LA(1);

			if ((LA117_0==FINALLY))
			{
				alt117=1;
			}
			else if ((LA117_0==CATCH))
			{
				alt117=2;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return retval;}
				NoViableAltException nvae = new NoViableAltException("", 117, 0, input);

				DebugRecognitionException(nvae);
				throw nvae;
			}
			} finally { DebugExitDecision(117); }
			switch (alt117)
			{
			case 1:
				DebugEnterAlt(1);
				// JavaScript.g3:250:51: finallyClause
				{
				DebugLocation(250, 51);
				PushFollow(Follow._finallyClause_in_tryStatement2075);
				finallyClause209=finallyClause();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, finallyClause209.Tree);

				}
				break;
			case 2:
				DebugEnterAlt(2);
				// JavaScript.g3:250:67: catchClause ( ( SkipSpace )* finallyClause )?
				{
				DebugLocation(250, 67);
				PushFollow(Follow._catchClause_in_tryStatement2079);
				catchClause210=catchClause();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, catchClause210.Tree);
				DebugLocation(250, 79);
				// JavaScript.g3:250:79: ( ( SkipSpace )* finallyClause )?
				int alt116=2;
				try { DebugEnterSubRule(116);
				try { DebugEnterDecision(116, decisionCanBacktrack[116]);
				try
				{
					alt116 = dfa116.Predict(input);
				}
				catch (NoViableAltException nvae)
				{
					DebugRecognitionException(nvae);
					throw;
				}
				} finally { DebugExitDecision(116); }
				switch (alt116)
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:250:80: ( SkipSpace )* finallyClause
					{
					DebugLocation(250, 89);
					// JavaScript.g3:250:89: ( SkipSpace )*
					try { DebugEnterSubRule(115);
					while (true)
					{
						int alt115=2;
						try { DebugEnterDecision(115, decisionCanBacktrack[115]);
						int LA115_0 = input.LA(1);

						if ((LA115_0==SkipSpace))
						{
							alt115=1;
						}


						} finally { DebugExitDecision(115); }
						switch ( alt115 )
						{
						case 1:
							DebugEnterAlt(1);
							// JavaScript.g3:250:89: SkipSpace
							{
							DebugLocation(250, 89);
							SkipSpace211=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_tryStatement2082); if (state.failed) return retval;

							}
							break;

						default:
							goto loop115;
						}
					}

					loop115:
						;

					} finally { DebugExitSubRule(115); }

					DebugLocation(250, 92);
					PushFollow(Follow._finallyClause_in_tryStatement2086);
					finallyClause212=finallyClause();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) adaptor.AddChild(root_0, finallyClause212.Tree);

					}
					break;

				}
				} finally { DebugExitSubRule(116); }


				}
				break;

			}
			} finally { DebugExitSubRule(117); }


			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("tryStatement", 37);
			LeaveRule("tryStatement", 37);
			Leave_tryStatement();
			if (state.backtracking > 0) { Memoize(input, 37, tryStatement_StartIndex); }
		}
		DebugLocation(251, 4);
		} finally { DebugExitRule(GrammarFileName, "tryStatement"); }
		return retval;

	}
	// $ANTLR end "tryStatement"

	public class catchClause_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_catchClause();
	partial void Leave_catchClause();

	// $ANTLR start "catchClause"
	// JavaScript.g3:253:1: catchClause : CATCH ( SkipSpace )* '(' ( SkipSpace )* Identifier ( SkipSpace )* ')' ( SkipSpace )* statementBlock ;
	[GrammarRule("catchClause")]
	private JavaScriptParser.catchClause_return catchClause()
	{
		Enter_catchClause();
		EnterRule("catchClause", 38);
		TraceIn("catchClause", 38);
		JavaScriptParser.catchClause_return retval = new JavaScriptParser.catchClause_return();
		retval.Start = (CommonToken)input.LT(1);
		int catchClause_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken CATCH213=null;
		CommonToken SkipSpace214=null;
		CommonToken char_literal215=null;
		CommonToken SkipSpace216=null;
		CommonToken Identifier217=null;
		CommonToken SkipSpace218=null;
		CommonToken char_literal219=null;
		CommonToken SkipSpace220=null;
		JavaScriptParser.statementBlock_return statementBlock221 = default(JavaScriptParser.statementBlock_return);

		CommonTree CATCH213_tree=null;
		CommonTree SkipSpace214_tree=null;
		CommonTree char_literal215_tree=null;
		CommonTree SkipSpace216_tree=null;
		CommonTree Identifier217_tree=null;
		CommonTree SkipSpace218_tree=null;
		CommonTree char_literal219_tree=null;
		CommonTree SkipSpace220_tree=null;

		try { DebugEnterRule(GrammarFileName, "catchClause");
		DebugLocation(253, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 38)) { return retval; }
			// JavaScript.g3:254:5: ( CATCH ( SkipSpace )* '(' ( SkipSpace )* Identifier ( SkipSpace )* ')' ( SkipSpace )* statementBlock )
			DebugEnterAlt(1);
			// JavaScript.g3:254:7: CATCH ( SkipSpace )* '(' ( SkipSpace )* Identifier ( SkipSpace )* ')' ( SkipSpace )* statementBlock
			{
			root_0 = (CommonTree)adaptor.Nil();

			DebugLocation(254, 7);
			CATCH213=(CommonToken)Match(input,CATCH,Follow._CATCH_in_catchClause2106); if (state.failed) return retval;
			if ( state.backtracking==0 ) {
			CATCH213_tree = (CommonTree)adaptor.Create(CATCH213);
			adaptor.AddChild(root_0, CATCH213_tree);
			}
			DebugLocation(254, 22);
			// JavaScript.g3:254:22: ( SkipSpace )*
			try { DebugEnterSubRule(118);
			while (true)
			{
				int alt118=2;
				try { DebugEnterDecision(118, decisionCanBacktrack[118]);
				int LA118_0 = input.LA(1);

				if ((LA118_0==SkipSpace))
				{
					alt118=1;
				}


				} finally { DebugExitDecision(118); }
				switch ( alt118 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:254:22: SkipSpace
					{
					DebugLocation(254, 22);
					SkipSpace214=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_catchClause2108); if (state.failed) return retval;

					}
					break;

				default:
					goto loop118;
				}
			}

			loop118:
				;

			} finally { DebugExitSubRule(118); }

			DebugLocation(254, 25);
			char_literal215=(CommonToken)Match(input,130,Follow._130_in_catchClause2112); if (state.failed) return retval;
			if ( state.backtracking==0 ) {
			char_literal215_tree = (CommonTree)adaptor.Create(char_literal215);
			adaptor.AddChild(root_0, char_literal215_tree);
			}
			DebugLocation(254, 38);
			// JavaScript.g3:254:38: ( SkipSpace )*
			try { DebugEnterSubRule(119);
			while (true)
			{
				int alt119=2;
				try { DebugEnterDecision(119, decisionCanBacktrack[119]);
				int LA119_0 = input.LA(1);

				if ((LA119_0==SkipSpace))
				{
					alt119=1;
				}


				} finally { DebugExitDecision(119); }
				switch ( alt119 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:254:38: SkipSpace
					{
					DebugLocation(254, 38);
					SkipSpace216=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_catchClause2114); if (state.failed) return retval;

					}
					break;

				default:
					goto loop119;
				}
			}

			loop119:
				;

			} finally { DebugExitSubRule(119); }

			DebugLocation(254, 41);
			Identifier217=(CommonToken)Match(input,Identifier,Follow._Identifier_in_catchClause2118); if (state.failed) return retval;
			if ( state.backtracking==0 ) {
			Identifier217_tree = (CommonTree)adaptor.Create(Identifier217);
			adaptor.AddChild(root_0, Identifier217_tree);
			}
			DebugLocation(254, 61);
			// JavaScript.g3:254:61: ( SkipSpace )*
			try { DebugEnterSubRule(120);
			while (true)
			{
				int alt120=2;
				try { DebugEnterDecision(120, decisionCanBacktrack[120]);
				int LA120_0 = input.LA(1);

				if ((LA120_0==SkipSpace))
				{
					alt120=1;
				}


				} finally { DebugExitDecision(120); }
				switch ( alt120 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:254:61: SkipSpace
					{
					DebugLocation(254, 61);
					SkipSpace218=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_catchClause2120); if (state.failed) return retval;

					}
					break;

				default:
					goto loop120;
				}
			}

			loop120:
				;

			} finally { DebugExitSubRule(120); }

			DebugLocation(254, 64);
			char_literal219=(CommonToken)Match(input,131,Follow._131_in_catchClause2124); if (state.failed) return retval;
			if ( state.backtracking==0 ) {
			char_literal219_tree = (CommonTree)adaptor.Create(char_literal219);
			adaptor.AddChild(root_0, char_literal219_tree);
			}
			DebugLocation(254, 77);
			// JavaScript.g3:254:77: ( SkipSpace )*
			try { DebugEnterSubRule(121);
			while (true)
			{
				int alt121=2;
				try { DebugEnterDecision(121, decisionCanBacktrack[121]);
				int LA121_0 = input.LA(1);

				if ((LA121_0==SkipSpace))
				{
					alt121=1;
				}


				} finally { DebugExitDecision(121); }
				switch ( alt121 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:254:77: SkipSpace
					{
					DebugLocation(254, 77);
					SkipSpace220=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_catchClause2126); if (state.failed) return retval;

					}
					break;

				default:
					goto loop121;
				}
			}

			loop121:
				;

			} finally { DebugExitSubRule(121); }

			DebugLocation(254, 80);
			PushFollow(Follow._statementBlock_in_catchClause2130);
			statementBlock221=statementBlock();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) adaptor.AddChild(root_0, statementBlock221.Tree);

			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("catchClause", 38);
			LeaveRule("catchClause", 38);
			Leave_catchClause();
			if (state.backtracking > 0) { Memoize(input, 38, catchClause_StartIndex); }
		}
		DebugLocation(255, 4);
		} finally { DebugExitRule(GrammarFileName, "catchClause"); }
		return retval;

	}
	// $ANTLR end "catchClause"

	public class finallyClause_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_finallyClause();
	partial void Leave_finallyClause();

	// $ANTLR start "finallyClause"
	// JavaScript.g3:257:1: finallyClause : FINALLY ( SkipSpace )* statementBlock ;
	[GrammarRule("finallyClause")]
	private JavaScriptParser.finallyClause_return finallyClause()
	{
		Enter_finallyClause();
		EnterRule("finallyClause", 39);
		TraceIn("finallyClause", 39);
		JavaScriptParser.finallyClause_return retval = new JavaScriptParser.finallyClause_return();
		retval.Start = (CommonToken)input.LT(1);
		int finallyClause_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken FINALLY222=null;
		CommonToken SkipSpace223=null;
		JavaScriptParser.statementBlock_return statementBlock224 = default(JavaScriptParser.statementBlock_return);

		CommonTree FINALLY222_tree=null;
		CommonTree SkipSpace223_tree=null;

		try { DebugEnterRule(GrammarFileName, "finallyClause");
		DebugLocation(257, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 39)) { return retval; }
			// JavaScript.g3:258:5: ( FINALLY ( SkipSpace )* statementBlock )
			DebugEnterAlt(1);
			// JavaScript.g3:258:7: FINALLY ( SkipSpace )* statementBlock
			{
			root_0 = (CommonTree)adaptor.Nil();

			DebugLocation(258, 7);
			FINALLY222=(CommonToken)Match(input,FINALLY,Follow._FINALLY_in_finallyClause2147); if (state.failed) return retval;
			if ( state.backtracking==0 ) {
			FINALLY222_tree = (CommonTree)adaptor.Create(FINALLY222);
			adaptor.AddChild(root_0, FINALLY222_tree);
			}
			DebugLocation(258, 24);
			// JavaScript.g3:258:24: ( SkipSpace )*
			try { DebugEnterSubRule(122);
			while (true)
			{
				int alt122=2;
				try { DebugEnterDecision(122, decisionCanBacktrack[122]);
				int LA122_0 = input.LA(1);

				if ((LA122_0==SkipSpace))
				{
					alt122=1;
				}


				} finally { DebugExitDecision(122); }
				switch ( alt122 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:258:24: SkipSpace
					{
					DebugLocation(258, 24);
					SkipSpace223=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_finallyClause2149); if (state.failed) return retval;

					}
					break;

				default:
					goto loop122;
				}
			}

			loop122:
				;

			} finally { DebugExitSubRule(122); }

			DebugLocation(258, 27);
			PushFollow(Follow._statementBlock_in_finallyClause2153);
			statementBlock224=statementBlock();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) adaptor.AddChild(root_0, statementBlock224.Tree);

			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("finallyClause", 39);
			LeaveRule("finallyClause", 39);
			Leave_finallyClause();
			if (state.backtracking > 0) { Memoize(input, 39, finallyClause_StartIndex); }
		}
		DebugLocation(259, 4);
		} finally { DebugExitRule(GrammarFileName, "finallyClause"); }
		return retval;

	}
	// $ANTLR end "finallyClause"

	public class expression_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_expression();
	partial void Leave_expression();

	// $ANTLR start "expression"
	// JavaScript.g3:262:1: expression : assignmentExpression ( ( SkipSpace )* ',' ( SkipSpace )* assignmentExpression )* -> ( assignmentExpression )+ ;
	[GrammarRule("expression")]
	private JavaScriptParser.expression_return expression()
	{
		Enter_expression();
		EnterRule("expression", 40);
		TraceIn("expression", 40);
		JavaScriptParser.expression_return retval = new JavaScriptParser.expression_return();
		retval.Start = (CommonToken)input.LT(1);
		int expression_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken SkipSpace226=null;
		CommonToken char_literal227=null;
		CommonToken SkipSpace228=null;
		JavaScriptParser.assignmentExpression_return assignmentExpression225 = default(JavaScriptParser.assignmentExpression_return);
		JavaScriptParser.assignmentExpression_return assignmentExpression229 = default(JavaScriptParser.assignmentExpression_return);

		CommonTree SkipSpace226_tree=null;
		CommonTree char_literal227_tree=null;
		CommonTree SkipSpace228_tree=null;
		RewriteRuleITokenStream stream_SkipSpace=new RewriteRuleITokenStream(adaptor,"token SkipSpace");
		RewriteRuleITokenStream stream_COMMA=new RewriteRuleITokenStream(adaptor,"token COMMA");
		RewriteRuleSubtreeStream stream_assignmentExpression=new RewriteRuleSubtreeStream(adaptor,"rule assignmentExpression");
		try { DebugEnterRule(GrammarFileName, "expression");
		DebugLocation(262, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 40)) { return retval; }
			// JavaScript.g3:263:5: ( assignmentExpression ( ( SkipSpace )* ',' ( SkipSpace )* assignmentExpression )* -> ( assignmentExpression )+ )
			DebugEnterAlt(1);
			// JavaScript.g3:263:7: assignmentExpression ( ( SkipSpace )* ',' ( SkipSpace )* assignmentExpression )*
			{
			DebugLocation(263, 7);
			PushFollow(Follow._assignmentExpression_in_expression2171);
			assignmentExpression225=assignmentExpression();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_assignmentExpression.Add(assignmentExpression225.Tree);
			DebugLocation(263, 28);
			// JavaScript.g3:263:28: ( ( SkipSpace )* ',' ( SkipSpace )* assignmentExpression )*
			try { DebugEnterSubRule(125);
			while (true)
			{
				int alt125=2;
				try { DebugEnterDecision(125, decisionCanBacktrack[125]);
				try
				{
					alt125 = dfa125.Predict(input);
				}
				catch (NoViableAltException nvae)
				{
					DebugRecognitionException(nvae);
					throw;
				}
				} finally { DebugExitDecision(125); }
				switch ( alt125 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:263:29: ( SkipSpace )* ',' ( SkipSpace )* assignmentExpression
					{
					DebugLocation(263, 29);
					// JavaScript.g3:263:29: ( SkipSpace )*
					try { DebugEnterSubRule(123);
					while (true)
					{
						int alt123=2;
						try { DebugEnterDecision(123, decisionCanBacktrack[123]);
						int LA123_0 = input.LA(1);

						if ((LA123_0==SkipSpace))
						{
							alt123=1;
						}


						} finally { DebugExitDecision(123); }
						switch ( alt123 )
						{
						case 1:
							DebugEnterAlt(1);
							// JavaScript.g3:263:29: SkipSpace
							{
							DebugLocation(263, 29);
							SkipSpace226=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_expression2174); if (state.failed) return retval; 
							if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace226);


							}
							break;

						default:
							goto loop123;
						}
					}

					loop123:
						;

					} finally { DebugExitSubRule(123); }

					DebugLocation(263, 40);
					char_literal227=(CommonToken)Match(input,COMMA,Follow._COMMA_in_expression2177); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_COMMA.Add(char_literal227);

					DebugLocation(263, 44);
					// JavaScript.g3:263:44: ( SkipSpace )*
					try { DebugEnterSubRule(124);
					while (true)
					{
						int alt124=2;
						try { DebugEnterDecision(124, decisionCanBacktrack[124]);
						int LA124_0 = input.LA(1);

						if ((LA124_0==SkipSpace))
						{
							alt124=1;
						}


						} finally { DebugExitDecision(124); }
						switch ( alt124 )
						{
						case 1:
							DebugEnterAlt(1);
							// JavaScript.g3:263:44: SkipSpace
							{
							DebugLocation(263, 44);
							SkipSpace228=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_expression2179); if (state.failed) return retval; 
							if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace228);


							}
							break;

						default:
							goto loop124;
						}
					}

					loop124:
						;

					} finally { DebugExitSubRule(124); }

					DebugLocation(263, 55);
					PushFollow(Follow._assignmentExpression_in_expression2182);
					assignmentExpression229=assignmentExpression();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_assignmentExpression.Add(assignmentExpression229.Tree);

					}
					break;

				default:
					goto loop125;
				}
			}

			loop125:
				;

			} finally { DebugExitSubRule(125); }



			{
			// AST REWRITE
			// elements: assignmentExpression
			// token labels: 
			// rule labels: retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 264:9: -> ( assignmentExpression )+
			{
				DebugLocation(264, 12);
				if ( !(stream_assignmentExpression.HasNext) )
				{
					throw new RewriteEarlyExitException();
				}
				while ( stream_assignmentExpression.HasNext )
				{
					DebugLocation(264, 12);
					adaptor.AddChild(root_0, stream_assignmentExpression.NextTree());

				}
				stream_assignmentExpression.Reset();

			}

			retval.Tree = root_0;
			}
			}

			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("expression", 40);
			LeaveRule("expression", 40);
			Leave_expression();
			if (state.backtracking > 0) { Memoize(input, 40, expression_StartIndex); }
		}
		DebugLocation(265, 4);
		} finally { DebugExitRule(GrammarFileName, "expression"); }
		return retval;

	}
	// $ANTLR end "expression"

	public class expressionNoIn_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_expressionNoIn();
	partial void Leave_expressionNoIn();

	// $ANTLR start "expressionNoIn"
	// JavaScript.g3:267:1: expressionNoIn : assignmentExpressionNoIn ( ( SkipSpace )* ',' ( SkipSpace )* assignmentExpressionNoIn )* -> ( assignmentExpressionNoIn )+ ;
	[GrammarRule("expressionNoIn")]
	private JavaScriptParser.expressionNoIn_return expressionNoIn()
	{
		Enter_expressionNoIn();
		EnterRule("expressionNoIn", 41);
		TraceIn("expressionNoIn", 41);
		JavaScriptParser.expressionNoIn_return retval = new JavaScriptParser.expressionNoIn_return();
		retval.Start = (CommonToken)input.LT(1);
		int expressionNoIn_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken SkipSpace231=null;
		CommonToken char_literal232=null;
		CommonToken SkipSpace233=null;
		JavaScriptParser.assignmentExpressionNoIn_return assignmentExpressionNoIn230 = default(JavaScriptParser.assignmentExpressionNoIn_return);
		JavaScriptParser.assignmentExpressionNoIn_return assignmentExpressionNoIn234 = default(JavaScriptParser.assignmentExpressionNoIn_return);

		CommonTree SkipSpace231_tree=null;
		CommonTree char_literal232_tree=null;
		CommonTree SkipSpace233_tree=null;
		RewriteRuleITokenStream stream_SkipSpace=new RewriteRuleITokenStream(adaptor,"token SkipSpace");
		RewriteRuleITokenStream stream_COMMA=new RewriteRuleITokenStream(adaptor,"token COMMA");
		RewriteRuleSubtreeStream stream_assignmentExpressionNoIn=new RewriteRuleSubtreeStream(adaptor,"rule assignmentExpressionNoIn");
		try { DebugEnterRule(GrammarFileName, "expressionNoIn");
		DebugLocation(267, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 41)) { return retval; }
			// JavaScript.g3:268:5: ( assignmentExpressionNoIn ( ( SkipSpace )* ',' ( SkipSpace )* assignmentExpressionNoIn )* -> ( assignmentExpressionNoIn )+ )
			DebugEnterAlt(1);
			// JavaScript.g3:268:7: assignmentExpressionNoIn ( ( SkipSpace )* ',' ( SkipSpace )* assignmentExpressionNoIn )*
			{
			DebugLocation(268, 7);
			PushFollow(Follow._assignmentExpressionNoIn_in_expressionNoIn2214);
			assignmentExpressionNoIn230=assignmentExpressionNoIn();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_assignmentExpressionNoIn.Add(assignmentExpressionNoIn230.Tree);
			DebugLocation(268, 32);
			// JavaScript.g3:268:32: ( ( SkipSpace )* ',' ( SkipSpace )* assignmentExpressionNoIn )*
			try { DebugEnterSubRule(128);
			while (true)
			{
				int alt128=2;
				try { DebugEnterDecision(128, decisionCanBacktrack[128]);
				try
				{
					alt128 = dfa128.Predict(input);
				}
				catch (NoViableAltException nvae)
				{
					DebugRecognitionException(nvae);
					throw;
				}
				} finally { DebugExitDecision(128); }
				switch ( alt128 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:268:33: ( SkipSpace )* ',' ( SkipSpace )* assignmentExpressionNoIn
					{
					DebugLocation(268, 33);
					// JavaScript.g3:268:33: ( SkipSpace )*
					try { DebugEnterSubRule(126);
					while (true)
					{
						int alt126=2;
						try { DebugEnterDecision(126, decisionCanBacktrack[126]);
						int LA126_0 = input.LA(1);

						if ((LA126_0==SkipSpace))
						{
							alt126=1;
						}


						} finally { DebugExitDecision(126); }
						switch ( alt126 )
						{
						case 1:
							DebugEnterAlt(1);
							// JavaScript.g3:268:33: SkipSpace
							{
							DebugLocation(268, 33);
							SkipSpace231=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_expressionNoIn2217); if (state.failed) return retval; 
							if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace231);


							}
							break;

						default:
							goto loop126;
						}
					}

					loop126:
						;

					} finally { DebugExitSubRule(126); }

					DebugLocation(268, 44);
					char_literal232=(CommonToken)Match(input,COMMA,Follow._COMMA_in_expressionNoIn2220); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_COMMA.Add(char_literal232);

					DebugLocation(268, 48);
					// JavaScript.g3:268:48: ( SkipSpace )*
					try { DebugEnterSubRule(127);
					while (true)
					{
						int alt127=2;
						try { DebugEnterDecision(127, decisionCanBacktrack[127]);
						int LA127_0 = input.LA(1);

						if ((LA127_0==SkipSpace))
						{
							alt127=1;
						}


						} finally { DebugExitDecision(127); }
						switch ( alt127 )
						{
						case 1:
							DebugEnterAlt(1);
							// JavaScript.g3:268:48: SkipSpace
							{
							DebugLocation(268, 48);
							SkipSpace233=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_expressionNoIn2222); if (state.failed) return retval; 
							if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace233);


							}
							break;

						default:
							goto loop127;
						}
					}

					loop127:
						;

					} finally { DebugExitSubRule(127); }

					DebugLocation(268, 59);
					PushFollow(Follow._assignmentExpressionNoIn_in_expressionNoIn2225);
					assignmentExpressionNoIn234=assignmentExpressionNoIn();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_assignmentExpressionNoIn.Add(assignmentExpressionNoIn234.Tree);

					}
					break;

				default:
					goto loop128;
				}
			}

			loop128:
				;

			} finally { DebugExitSubRule(128); }



			{
			// AST REWRITE
			// elements: assignmentExpressionNoIn
			// token labels: 
			// rule labels: retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 269:9: -> ( assignmentExpressionNoIn )+
			{
				DebugLocation(269, 12);
				if ( !(stream_assignmentExpressionNoIn.HasNext) )
				{
					throw new RewriteEarlyExitException();
				}
				while ( stream_assignmentExpressionNoIn.HasNext )
				{
					DebugLocation(269, 12);
					adaptor.AddChild(root_0, stream_assignmentExpressionNoIn.NextTree());

				}
				stream_assignmentExpressionNoIn.Reset();

			}

			retval.Tree = root_0;
			}
			}

			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("expressionNoIn", 41);
			LeaveRule("expressionNoIn", 41);
			Leave_expressionNoIn();
			if (state.backtracking > 0) { Memoize(input, 41, expressionNoIn_StartIndex); }
		}
		DebugLocation(270, 4);
		} finally { DebugExitRule(GrammarFileName, "expressionNoIn"); }
		return retval;

	}
	// $ANTLR end "expressionNoIn"

	public class assignmentExpression_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_assignmentExpression();
	partial void Leave_assignmentExpression();

	// $ANTLR start "assignmentExpression"
	// JavaScript.g3:272:1: assignmentExpression : ( conditionalExpression |lhs= leftHandSideExpression ( SkipSpace )* op= assignmentOperator ( SkipSpace )* rhs= assignmentExpression -> ^( BINARY_EXPR $op $lhs $rhs) );
	[GrammarRule("assignmentExpression")]
	private JavaScriptParser.assignmentExpression_return assignmentExpression()
	{
		Enter_assignmentExpression();
		EnterRule("assignmentExpression", 42);
		TraceIn("assignmentExpression", 42);
		JavaScriptParser.assignmentExpression_return retval = new JavaScriptParser.assignmentExpression_return();
		retval.Start = (CommonToken)input.LT(1);
		int assignmentExpression_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken SkipSpace236=null;
		CommonToken SkipSpace237=null;
		JavaScriptParser.leftHandSideExpression_return lhs = default(JavaScriptParser.leftHandSideExpression_return);
		JavaScriptParser.assignmentOperator_return op = default(JavaScriptParser.assignmentOperator_return);
		JavaScriptParser.assignmentExpression_return rhs = default(JavaScriptParser.assignmentExpression_return);
		JavaScriptParser.conditionalExpression_return conditionalExpression235 = default(JavaScriptParser.conditionalExpression_return);

		CommonTree SkipSpace236_tree=null;
		CommonTree SkipSpace237_tree=null;
		RewriteRuleITokenStream stream_SkipSpace=new RewriteRuleITokenStream(adaptor,"token SkipSpace");
		RewriteRuleSubtreeStream stream_leftHandSideExpression=new RewriteRuleSubtreeStream(adaptor,"rule leftHandSideExpression");
		RewriteRuleSubtreeStream stream_assignmentOperator=new RewriteRuleSubtreeStream(adaptor,"rule assignmentOperator");
		RewriteRuleSubtreeStream stream_assignmentExpression=new RewriteRuleSubtreeStream(adaptor,"rule assignmentExpression");
		try { DebugEnterRule(GrammarFileName, "assignmentExpression");
		DebugLocation(272, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 42)) { return retval; }
			// JavaScript.g3:273:5: ( conditionalExpression |lhs= leftHandSideExpression ( SkipSpace )* op= assignmentOperator ( SkipSpace )* rhs= assignmentExpression -> ^( BINARY_EXPR $op $lhs $rhs) )
			int alt131=2;
			try { DebugEnterDecision(131, decisionCanBacktrack[131]);
			try
			{
				alt131 = dfa131.Predict(input);
			}
			catch (NoViableAltException nvae)
			{
				DebugRecognitionException(nvae);
				throw;
			}
			} finally { DebugExitDecision(131); }
			switch (alt131)
			{
			case 1:
				DebugEnterAlt(1);
				// JavaScript.g3:273:7: conditionalExpression
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(273, 7);
				PushFollow(Follow._conditionalExpression_in_assignmentExpression2257);
				conditionalExpression235=conditionalExpression();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, conditionalExpression235.Tree);

				}
				break;
			case 2:
				DebugEnterAlt(2);
				// JavaScript.g3:274:7: lhs= leftHandSideExpression ( SkipSpace )* op= assignmentOperator ( SkipSpace )* rhs= assignmentExpression
				{
				DebugLocation(274, 10);
				PushFollow(Follow._leftHandSideExpression_in_assignmentExpression2267);
				lhs=leftHandSideExpression();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_leftHandSideExpression.Add(lhs.Tree);
				DebugLocation(274, 34);
				// JavaScript.g3:274:34: ( SkipSpace )*
				try { DebugEnterSubRule(129);
				while (true)
				{
					int alt129=2;
					try { DebugEnterDecision(129, decisionCanBacktrack[129]);
					int LA129_0 = input.LA(1);

					if ((LA129_0==SkipSpace))
					{
						alt129=1;
					}


					} finally { DebugExitDecision(129); }
					switch ( alt129 )
					{
					case 1:
						DebugEnterAlt(1);
						// JavaScript.g3:274:34: SkipSpace
						{
						DebugLocation(274, 34);
						SkipSpace236=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_assignmentExpression2269); if (state.failed) return retval; 
						if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace236);


						}
						break;

					default:
						goto loop129;
					}
				}

				loop129:
					;

				} finally { DebugExitSubRule(129); }

				DebugLocation(274, 47);
				PushFollow(Follow._assignmentOperator_in_assignmentExpression2274);
				op=assignmentOperator();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_assignmentOperator.Add(op.Tree);
				DebugLocation(274, 67);
				// JavaScript.g3:274:67: ( SkipSpace )*
				try { DebugEnterSubRule(130);
				while (true)
				{
					int alt130=2;
					try { DebugEnterDecision(130, decisionCanBacktrack[130]);
					int LA130_0 = input.LA(1);

					if ((LA130_0==SkipSpace))
					{
						alt130=1;
					}


					} finally { DebugExitDecision(130); }
					switch ( alt130 )
					{
					case 1:
						DebugEnterAlt(1);
						// JavaScript.g3:274:67: SkipSpace
						{
						DebugLocation(274, 67);
						SkipSpace237=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_assignmentExpression2276); if (state.failed) return retval; 
						if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace237);


						}
						break;

					default:
						goto loop130;
					}
				}

				loop130:
					;

				} finally { DebugExitSubRule(130); }

				DebugLocation(274, 81);
				PushFollow(Follow._assignmentExpression_in_assignmentExpression2281);
				rhs=assignmentExpression();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_assignmentExpression.Add(rhs.Tree);


				{
				// AST REWRITE
				// elements: op, lhs, rhs
				// token labels: 
				// rule labels: op, lhs, rhs, retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.Tree = root_0;
				RewriteRuleSubtreeStream stream_op=new RewriteRuleSubtreeStream(adaptor,"rule op",op!=null?op.Tree:null);
				RewriteRuleSubtreeStream stream_lhs=new RewriteRuleSubtreeStream(adaptor,"rule lhs",lhs!=null?lhs.Tree:null);
				RewriteRuleSubtreeStream stream_rhs=new RewriteRuleSubtreeStream(adaptor,"rule rhs",rhs!=null?rhs.Tree:null);
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

				root_0 = (CommonTree)adaptor.Nil();
				// 275:9: -> ^( BINARY_EXPR $op $lhs $rhs)
				{
					DebugLocation(275, 12);
					// JavaScript.g3:275:12: ^( BINARY_EXPR $op $lhs $rhs)
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(275, 14);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(BINARY_EXPR, "BINARY_EXPR"), root_1);

					DebugLocation(275, 27);
					adaptor.AddChild(root_1, stream_op.NextTree());
					DebugLocation(275, 31);
					adaptor.AddChild(root_1, stream_lhs.NextTree());
					DebugLocation(275, 36);
					adaptor.AddChild(root_1, stream_rhs.NextTree());

					adaptor.AddChild(root_0, root_1);
					}

				}

				retval.Tree = root_0;
				}
				}

				}
				break;

			}
			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("assignmentExpression", 42);
			LeaveRule("assignmentExpression", 42);
			Leave_assignmentExpression();
			if (state.backtracking > 0) { Memoize(input, 42, assignmentExpression_StartIndex); }
		}
		DebugLocation(276, 4);
		} finally { DebugExitRule(GrammarFileName, "assignmentExpression"); }
		return retval;

	}
	// $ANTLR end "assignmentExpression"

	public class assignmentExpressionNoIn_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_assignmentExpressionNoIn();
	partial void Leave_assignmentExpressionNoIn();

	// $ANTLR start "assignmentExpressionNoIn"
	// JavaScript.g3:278:1: assignmentExpressionNoIn : ( conditionalExpressionNoIn |lhs= leftHandSideExpression ( SkipSpace )* op= assignmentOperator ( SkipSpace )* rhs= assignmentExpressionNoIn -> ^( BINARY_EXPR $op $lhs $rhs) );
	[GrammarRule("assignmentExpressionNoIn")]
	private JavaScriptParser.assignmentExpressionNoIn_return assignmentExpressionNoIn()
	{
		Enter_assignmentExpressionNoIn();
		EnterRule("assignmentExpressionNoIn", 43);
		TraceIn("assignmentExpressionNoIn", 43);
		JavaScriptParser.assignmentExpressionNoIn_return retval = new JavaScriptParser.assignmentExpressionNoIn_return();
		retval.Start = (CommonToken)input.LT(1);
		int assignmentExpressionNoIn_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken SkipSpace239=null;
		CommonToken SkipSpace240=null;
		JavaScriptParser.leftHandSideExpression_return lhs = default(JavaScriptParser.leftHandSideExpression_return);
		JavaScriptParser.assignmentOperator_return op = default(JavaScriptParser.assignmentOperator_return);
		JavaScriptParser.assignmentExpressionNoIn_return rhs = default(JavaScriptParser.assignmentExpressionNoIn_return);
		JavaScriptParser.conditionalExpressionNoIn_return conditionalExpressionNoIn238 = default(JavaScriptParser.conditionalExpressionNoIn_return);

		CommonTree SkipSpace239_tree=null;
		CommonTree SkipSpace240_tree=null;
		RewriteRuleITokenStream stream_SkipSpace=new RewriteRuleITokenStream(adaptor,"token SkipSpace");
		RewriteRuleSubtreeStream stream_leftHandSideExpression=new RewriteRuleSubtreeStream(adaptor,"rule leftHandSideExpression");
		RewriteRuleSubtreeStream stream_assignmentOperator=new RewriteRuleSubtreeStream(adaptor,"rule assignmentOperator");
		RewriteRuleSubtreeStream stream_assignmentExpressionNoIn=new RewriteRuleSubtreeStream(adaptor,"rule assignmentExpressionNoIn");
		try { DebugEnterRule(GrammarFileName, "assignmentExpressionNoIn");
		DebugLocation(278, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 43)) { return retval; }
			// JavaScript.g3:279:5: ( conditionalExpressionNoIn |lhs= leftHandSideExpression ( SkipSpace )* op= assignmentOperator ( SkipSpace )* rhs= assignmentExpressionNoIn -> ^( BINARY_EXPR $op $lhs $rhs) )
			int alt134=2;
			try { DebugEnterDecision(134, decisionCanBacktrack[134]);
			try
			{
				alt134 = dfa134.Predict(input);
			}
			catch (NoViableAltException nvae)
			{
				DebugRecognitionException(nvae);
				throw;
			}
			} finally { DebugExitDecision(134); }
			switch (alt134)
			{
			case 1:
				DebugEnterAlt(1);
				// JavaScript.g3:279:7: conditionalExpressionNoIn
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(279, 7);
				PushFollow(Follow._conditionalExpressionNoIn_in_assignmentExpressionNoIn2321);
				conditionalExpressionNoIn238=conditionalExpressionNoIn();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, conditionalExpressionNoIn238.Tree);

				}
				break;
			case 2:
				DebugEnterAlt(2);
				// JavaScript.g3:280:7: lhs= leftHandSideExpression ( SkipSpace )* op= assignmentOperator ( SkipSpace )* rhs= assignmentExpressionNoIn
				{
				DebugLocation(280, 10);
				PushFollow(Follow._leftHandSideExpression_in_assignmentExpressionNoIn2331);
				lhs=leftHandSideExpression();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_leftHandSideExpression.Add(lhs.Tree);
				DebugLocation(280, 34);
				// JavaScript.g3:280:34: ( SkipSpace )*
				try { DebugEnterSubRule(132);
				while (true)
				{
					int alt132=2;
					try { DebugEnterDecision(132, decisionCanBacktrack[132]);
					int LA132_0 = input.LA(1);

					if ((LA132_0==SkipSpace))
					{
						alt132=1;
					}


					} finally { DebugExitDecision(132); }
					switch ( alt132 )
					{
					case 1:
						DebugEnterAlt(1);
						// JavaScript.g3:280:34: SkipSpace
						{
						DebugLocation(280, 34);
						SkipSpace239=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_assignmentExpressionNoIn2333); if (state.failed) return retval; 
						if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace239);


						}
						break;

					default:
						goto loop132;
					}
				}

				loop132:
					;

				} finally { DebugExitSubRule(132); }

				DebugLocation(280, 47);
				PushFollow(Follow._assignmentOperator_in_assignmentExpressionNoIn2338);
				op=assignmentOperator();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_assignmentOperator.Add(op.Tree);
				DebugLocation(280, 67);
				// JavaScript.g3:280:67: ( SkipSpace )*
				try { DebugEnterSubRule(133);
				while (true)
				{
					int alt133=2;
					try { DebugEnterDecision(133, decisionCanBacktrack[133]);
					int LA133_0 = input.LA(1);

					if ((LA133_0==SkipSpace))
					{
						alt133=1;
					}


					} finally { DebugExitDecision(133); }
					switch ( alt133 )
					{
					case 1:
						DebugEnterAlt(1);
						// JavaScript.g3:280:67: SkipSpace
						{
						DebugLocation(280, 67);
						SkipSpace240=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_assignmentExpressionNoIn2340); if (state.failed) return retval; 
						if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace240);


						}
						break;

					default:
						goto loop133;
					}
				}

				loop133:
					;

				} finally { DebugExitSubRule(133); }

				DebugLocation(280, 81);
				PushFollow(Follow._assignmentExpressionNoIn_in_assignmentExpressionNoIn2345);
				rhs=assignmentExpressionNoIn();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_assignmentExpressionNoIn.Add(rhs.Tree);


				{
				// AST REWRITE
				// elements: op, lhs, rhs
				// token labels: 
				// rule labels: op, lhs, rhs, retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.Tree = root_0;
				RewriteRuleSubtreeStream stream_op=new RewriteRuleSubtreeStream(adaptor,"rule op",op!=null?op.Tree:null);
				RewriteRuleSubtreeStream stream_lhs=new RewriteRuleSubtreeStream(adaptor,"rule lhs",lhs!=null?lhs.Tree:null);
				RewriteRuleSubtreeStream stream_rhs=new RewriteRuleSubtreeStream(adaptor,"rule rhs",rhs!=null?rhs.Tree:null);
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

				root_0 = (CommonTree)adaptor.Nil();
				// 281:9: -> ^( BINARY_EXPR $op $lhs $rhs)
				{
					DebugLocation(281, 12);
					// JavaScript.g3:281:12: ^( BINARY_EXPR $op $lhs $rhs)
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(281, 14);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(BINARY_EXPR, "BINARY_EXPR"), root_1);

					DebugLocation(281, 27);
					adaptor.AddChild(root_1, stream_op.NextTree());
					DebugLocation(281, 31);
					adaptor.AddChild(root_1, stream_lhs.NextTree());
					DebugLocation(281, 36);
					adaptor.AddChild(root_1, stream_rhs.NextTree());

					adaptor.AddChild(root_0, root_1);
					}

				}

				retval.Tree = root_0;
				}
				}

				}
				break;

			}
			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("assignmentExpressionNoIn", 43);
			LeaveRule("assignmentExpressionNoIn", 43);
			Leave_assignmentExpressionNoIn();
			if (state.backtracking > 0) { Memoize(input, 43, assignmentExpressionNoIn_StartIndex); }
		}
		DebugLocation(282, 4);
		} finally { DebugExitRule(GrammarFileName, "assignmentExpressionNoIn"); }
		return retval;

	}
	// $ANTLR end "assignmentExpressionNoIn"

	public class assignmentOperator_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_assignmentOperator();
	partial void Leave_assignmentOperator();

	// $ANTLR start "assignmentOperator"
	// JavaScript.g3:284:1: assignmentOperator : ( ASN | MUL_ASN | DIV_ASN | MOD_ASN | PLUS_ASN | MINUS_ASN | SHL_ASN | SHR_ASN | USHR_ASN | BIT_AND_ASN | BIT_XOR_ASN | BIT_OR_ASN );
	[GrammarRule("assignmentOperator")]
	private JavaScriptParser.assignmentOperator_return assignmentOperator()
	{
		Enter_assignmentOperator();
		EnterRule("assignmentOperator", 44);
		TraceIn("assignmentOperator", 44);
		JavaScriptParser.assignmentOperator_return retval = new JavaScriptParser.assignmentOperator_return();
		retval.Start = (CommonToken)input.LT(1);
		int assignmentOperator_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken set241=null;

		CommonTree set241_tree=null;

		try { DebugEnterRule(GrammarFileName, "assignmentOperator");
		DebugLocation(284, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 44)) { return retval; }
			// JavaScript.g3:285:5: ( ASN | MUL_ASN | DIV_ASN | MOD_ASN | PLUS_ASN | MINUS_ASN | SHL_ASN | SHR_ASN | USHR_ASN | BIT_AND_ASN | BIT_XOR_ASN | BIT_OR_ASN )
			DebugEnterAlt(1);
			// JavaScript.g3:
			{
			root_0 = (CommonTree)adaptor.Nil();

			DebugLocation(285, 5);
			set241=(CommonToken)input.LT(1);
			if (input.LA(1)==ASN||input.LA(1)==BIT_AND_ASN||input.LA(1)==BIT_OR_ASN||input.LA(1)==BIT_XOR_ASN||input.LA(1)==DIV_ASN||input.LA(1)==MINUS_ASN||input.LA(1)==MOD_ASN||input.LA(1)==MUL_ASN||input.LA(1)==PLUS_ASN||input.LA(1)==SHL_ASN||input.LA(1)==SHR_ASN||input.LA(1)==USHR_ASN)
			{
				input.Consume();
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, (CommonTree)adaptor.Create(set241));
				state.errorRecovery=false;state.failed=false;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return retval;}
				MismatchedSetException mse = new MismatchedSetException(null,input);
				DebugRecognitionException(mse);
				throw mse;
			}


			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("assignmentOperator", 44);
			LeaveRule("assignmentOperator", 44);
			Leave_assignmentOperator();
			if (state.backtracking > 0) { Memoize(input, 44, assignmentOperator_StartIndex); }
		}
		DebugLocation(297, 4);
		} finally { DebugExitRule(GrammarFileName, "assignmentOperator"); }
		return retval;

	}
	// $ANTLR end "assignmentOperator"

	public class leftHandSideExpression_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_leftHandSideExpression();
	partial void Leave_leftHandSideExpression();

	// $ANTLR start "leftHandSideExpression"
	// JavaScript.g3:299:1: leftHandSideExpression : ( callExpression | newExpression );
	[GrammarRule("leftHandSideExpression")]
	private JavaScriptParser.leftHandSideExpression_return leftHandSideExpression()
	{
		Enter_leftHandSideExpression();
		EnterRule("leftHandSideExpression", 45);
		TraceIn("leftHandSideExpression", 45);
		JavaScriptParser.leftHandSideExpression_return retval = new JavaScriptParser.leftHandSideExpression_return();
		retval.Start = (CommonToken)input.LT(1);
		int leftHandSideExpression_StartIndex = input.Index;
		CommonTree root_0 = null;

		JavaScriptParser.callExpression_return callExpression242 = default(JavaScriptParser.callExpression_return);
		JavaScriptParser.newExpression_return newExpression243 = default(JavaScriptParser.newExpression_return);


		try { DebugEnterRule(GrammarFileName, "leftHandSideExpression");
		DebugLocation(299, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 45)) { return retval; }
			// JavaScript.g3:300:5: ( callExpression | newExpression )
			int alt135=2;
			try { DebugEnterDecision(135, decisionCanBacktrack[135]);
			try
			{
				alt135 = dfa135.Predict(input);
			}
			catch (NoViableAltException nvae)
			{
				DebugRecognitionException(nvae);
				throw;
			}
			} finally { DebugExitDecision(135); }
			switch (alt135)
			{
			case 1:
				DebugEnterAlt(1);
				// JavaScript.g3:300:7: callExpression
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(300, 7);
				PushFollow(Follow._callExpression_in_leftHandSideExpression2490);
				callExpression242=callExpression();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, callExpression242.Tree);

				}
				break;
			case 2:
				DebugEnterAlt(2);
				// JavaScript.g3:301:7: newExpression
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(301, 7);
				PushFollow(Follow._newExpression_in_leftHandSideExpression2498);
				newExpression243=newExpression();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, newExpression243.Tree);

				}
				break;

			}
			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("leftHandSideExpression", 45);
			LeaveRule("leftHandSideExpression", 45);
			Leave_leftHandSideExpression();
			if (state.backtracking > 0) { Memoize(input, 45, leftHandSideExpression_StartIndex); }
		}
		DebugLocation(302, 4);
		} finally { DebugExitRule(GrammarFileName, "leftHandSideExpression"); }
		return retval;

	}
	// $ANTLR end "leftHandSideExpression"

	public class newExpression_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_newExpression();
	partial void Leave_newExpression();

	// $ANTLR start "newExpression"
	// JavaScript.g3:304:1: newExpression : ( memberExpression | 'new' ( SkipSpace )* newExpression );
	[GrammarRule("newExpression")]
	private JavaScriptParser.newExpression_return newExpression()
	{
		Enter_newExpression();
		EnterRule("newExpression", 46);
		TraceIn("newExpression", 46);
		JavaScriptParser.newExpression_return retval = new JavaScriptParser.newExpression_return();
		retval.Start = (CommonToken)input.LT(1);
		int newExpression_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken string_literal245=null;
		CommonToken SkipSpace246=null;
		JavaScriptParser.memberExpression_return memberExpression244 = default(JavaScriptParser.memberExpression_return);
		JavaScriptParser.newExpression_return newExpression247 = default(JavaScriptParser.newExpression_return);

		CommonTree string_literal245_tree=null;
		CommonTree SkipSpace246_tree=null;

		try { DebugEnterRule(GrammarFileName, "newExpression");
		DebugLocation(304, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 46)) { return retval; }
			// JavaScript.g3:305:5: ( memberExpression | 'new' ( SkipSpace )* newExpression )
			int alt137=2;
			try { DebugEnterDecision(137, decisionCanBacktrack[137]);
			try
			{
				alt137 = dfa137.Predict(input);
			}
			catch (NoViableAltException nvae)
			{
				DebugRecognitionException(nvae);
				throw;
			}
			} finally { DebugExitDecision(137); }
			switch (alt137)
			{
			case 1:
				DebugEnterAlt(1);
				// JavaScript.g3:305:7: memberExpression
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(305, 7);
				PushFollow(Follow._memberExpression_in_newExpression2515);
				memberExpression244=memberExpression();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, memberExpression244.Tree);

				}
				break;
			case 2:
				DebugEnterAlt(2);
				// JavaScript.g3:306:7: 'new' ( SkipSpace )* newExpression
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(306, 7);
				string_literal245=(CommonToken)Match(input,NEW,Follow._NEW_in_newExpression2523); if (state.failed) return retval;
				if ( state.backtracking==0 ) {
				string_literal245_tree = (CommonTree)adaptor.Create(string_literal245);
				adaptor.AddChild(root_0, string_literal245_tree);
				}
				DebugLocation(306, 22);
				// JavaScript.g3:306:22: ( SkipSpace )*
				try { DebugEnterSubRule(136);
				while (true)
				{
					int alt136=2;
					try { DebugEnterDecision(136, decisionCanBacktrack[136]);
					int LA136_0 = input.LA(1);

					if ((LA136_0==SkipSpace))
					{
						alt136=1;
					}


					} finally { DebugExitDecision(136); }
					switch ( alt136 )
					{
					case 1:
						DebugEnterAlt(1);
						// JavaScript.g3:306:22: SkipSpace
						{
						DebugLocation(306, 22);
						SkipSpace246=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_newExpression2525); if (state.failed) return retval;

						}
						break;

					default:
						goto loop136;
					}
				}

				loop136:
					;

				} finally { DebugExitSubRule(136); }

				DebugLocation(306, 25);
				PushFollow(Follow._newExpression_in_newExpression2529);
				newExpression247=newExpression();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, newExpression247.Tree);

				}
				break;

			}
			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("newExpression", 46);
			LeaveRule("newExpression", 46);
			Leave_newExpression();
			if (state.backtracking > 0) { Memoize(input, 46, newExpression_StartIndex); }
		}
		DebugLocation(307, 4);
		} finally { DebugExitRule(GrammarFileName, "newExpression"); }
		return retval;

	}
	// $ANTLR end "newExpression"

	public class memberExpression_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_memberExpression();
	partial void Leave_memberExpression();

	// $ANTLR start "memberExpression"
	// JavaScript.g3:309:1: memberExpression : (lhs= memberExpressionLHS -> $lhs) ( ( SkipSpace )* rhs= memberExpressionSuffix -> ^( ACCESSOR_EXPR $memberExpression $rhs) )* ;
	[GrammarRule("memberExpression")]
	private JavaScriptParser.memberExpression_return memberExpression()
	{
		Enter_memberExpression();
		EnterRule("memberExpression", 47);
		TraceIn("memberExpression", 47);
		JavaScriptParser.memberExpression_return retval = new JavaScriptParser.memberExpression_return();
		retval.Start = (CommonToken)input.LT(1);
		int memberExpression_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken SkipSpace248=null;
		JavaScriptParser.memberExpressionLHS_return lhs = default(JavaScriptParser.memberExpressionLHS_return);
		JavaScriptParser.memberExpressionSuffix_return rhs = default(JavaScriptParser.memberExpressionSuffix_return);

		CommonTree SkipSpace248_tree=null;
		RewriteRuleITokenStream stream_SkipSpace=new RewriteRuleITokenStream(adaptor,"token SkipSpace");
		RewriteRuleSubtreeStream stream_memberExpressionLHS=new RewriteRuleSubtreeStream(adaptor,"rule memberExpressionLHS");
		RewriteRuleSubtreeStream stream_memberExpressionSuffix=new RewriteRuleSubtreeStream(adaptor,"rule memberExpressionSuffix");
		try { DebugEnterRule(GrammarFileName, "memberExpression");
		DebugLocation(309, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 47)) { return retval; }
			// JavaScript.g3:310:5: ( (lhs= memberExpressionLHS -> $lhs) ( ( SkipSpace )* rhs= memberExpressionSuffix -> ^( ACCESSOR_EXPR $memberExpression $rhs) )* )
			DebugEnterAlt(1);
			// JavaScript.g3:310:7: (lhs= memberExpressionLHS -> $lhs) ( ( SkipSpace )* rhs= memberExpressionSuffix -> ^( ACCESSOR_EXPR $memberExpression $rhs) )*
			{
			DebugLocation(310, 7);
			// JavaScript.g3:310:7: (lhs= memberExpressionLHS -> $lhs)
			DebugEnterAlt(1);
			// JavaScript.g3:310:8: lhs= memberExpressionLHS
			{
			DebugLocation(310, 11);
			PushFollow(Follow._memberExpressionLHS_in_memberExpression2549);
			lhs=memberExpressionLHS();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_memberExpressionLHS.Add(lhs.Tree);


			{
			// AST REWRITE
			// elements: lhs
			// token labels: 
			// rule labels: lhs, retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_lhs=new RewriteRuleSubtreeStream(adaptor,"rule lhs",lhs!=null?lhs.Tree:null);
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 310:32: -> $lhs
			{
				DebugLocation(310, 36);
				adaptor.AddChild(root_0, stream_lhs.NextTree());

			}

			retval.Tree = root_0;
			}
			}

			}

			DebugLocation(311, 9);
			// JavaScript.g3:311:9: ( ( SkipSpace )* rhs= memberExpressionSuffix -> ^( ACCESSOR_EXPR $memberExpression $rhs) )*
			try { DebugEnterSubRule(139);
			while (true)
			{
				int alt139=2;
				try { DebugEnterDecision(139, decisionCanBacktrack[139]);
				try
				{
					alt139 = dfa139.Predict(input);
				}
				catch (NoViableAltException nvae)
				{
					DebugRecognitionException(nvae);
					throw;
				}
				} finally { DebugExitDecision(139); }
				switch ( alt139 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:311:10: ( SkipSpace )* rhs= memberExpressionSuffix
					{
					DebugLocation(311, 10);
					// JavaScript.g3:311:10: ( SkipSpace )*
					try { DebugEnterSubRule(138);
					while (true)
					{
						int alt138=2;
						try { DebugEnterDecision(138, decisionCanBacktrack[138]);
						int LA138_0 = input.LA(1);

						if ((LA138_0==SkipSpace))
						{
							alt138=1;
						}


						} finally { DebugExitDecision(138); }
						switch ( alt138 )
						{
						case 1:
							DebugEnterAlt(1);
							// JavaScript.g3:311:10: SkipSpace
							{
							DebugLocation(311, 10);
							SkipSpace248=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_memberExpression2566); if (state.failed) return retval; 
							if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace248);


							}
							break;

						default:
							goto loop138;
						}
					}

					loop138:
						;

					} finally { DebugExitSubRule(138); }

					DebugLocation(311, 24);
					PushFollow(Follow._memberExpressionSuffix_in_memberExpression2571);
					rhs=memberExpressionSuffix();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_memberExpressionSuffix.Add(rhs.Tree);


					{
					// AST REWRITE
					// elements: memberExpression, rhs
					// token labels: 
					// rule labels: rhs, retval
					// token list labels: 
					// rule list labels: 
					// wildcard labels: 
					if ( state.backtracking == 0 ) {
					retval.Tree = root_0;
					RewriteRuleSubtreeStream stream_rhs=new RewriteRuleSubtreeStream(adaptor,"rule rhs",rhs!=null?rhs.Tree:null);
					RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

					root_0 = (CommonTree)adaptor.Nil();
					// 311:48: -> ^( ACCESSOR_EXPR $memberExpression $rhs)
					{
						DebugLocation(311, 51);
						// JavaScript.g3:311:51: ^( ACCESSOR_EXPR $memberExpression $rhs)
						{
						CommonTree root_1 = (CommonTree)adaptor.Nil();
						DebugLocation(311, 53);
						root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(ACCESSOR_EXPR, "ACCESSOR_EXPR"), root_1);

						DebugLocation(311, 68);
						adaptor.AddChild(root_1, stream_retval.NextTree());
						DebugLocation(311, 86);
						adaptor.AddChild(root_1, stream_rhs.NextTree());

						adaptor.AddChild(root_0, root_1);
						}

					}

					retval.Tree = root_0;
					}
					}

					}
					break;

				default:
					goto loop139;
				}
			}

			loop139:
				;

			} finally { DebugExitSubRule(139); }


			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("memberExpression", 47);
			LeaveRule("memberExpression", 47);
			Leave_memberExpression();
			if (state.backtracking > 0) { Memoize(input, 47, memberExpression_StartIndex); }
		}
		DebugLocation(312, 4);
		} finally { DebugExitRule(GrammarFileName, "memberExpression"); }
		return retval;

	}
	// $ANTLR end "memberExpression"

	public class memberExpressionLHS_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_memberExpressionLHS();
	partial void Leave_memberExpressionLHS();

	// $ANTLR start "memberExpressionLHS"
	// JavaScript.g3:314:1: memberExpressionLHS : ( primaryExpression | functionExpression | newObjectExpression );
	[GrammarRule("memberExpressionLHS")]
	private JavaScriptParser.memberExpressionLHS_return memberExpressionLHS()
	{
		Enter_memberExpressionLHS();
		EnterRule("memberExpressionLHS", 48);
		TraceIn("memberExpressionLHS", 48);
		JavaScriptParser.memberExpressionLHS_return retval = new JavaScriptParser.memberExpressionLHS_return();
		retval.Start = (CommonToken)input.LT(1);
		int memberExpressionLHS_StartIndex = input.Index;
		CommonTree root_0 = null;

		JavaScriptParser.primaryExpression_return primaryExpression249 = default(JavaScriptParser.primaryExpression_return);
		JavaScriptParser.functionExpression_return functionExpression250 = default(JavaScriptParser.functionExpression_return);
		JavaScriptParser.newObjectExpression_return newObjectExpression251 = default(JavaScriptParser.newObjectExpression_return);


		try { DebugEnterRule(GrammarFileName, "memberExpressionLHS");
		DebugLocation(314, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 48)) { return retval; }
			// JavaScript.g3:315:5: ( primaryExpression | functionExpression | newObjectExpression )
			int alt140=3;
			try { DebugEnterDecision(140, decisionCanBacktrack[140]);
			switch (input.LA(1))
			{
			case BoolLiteral:
			case FALSE:
			case IMPORT_START:
			case Identifier:
			case NULL:
			case NumericLiteral:
			case StringLiteral:
			case THIS:
			case 130:
			case 135:
			case 147:
				{
				alt140=1;
				}
				break;
			case 142:
				{
				alt140=2;
				}
				break;
			case NEW:
				{
				alt140=3;
				}
				break;
			default:
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 140, 0, input);

					DebugRecognitionException(nvae);
					throw nvae;
				}
			}

			} finally { DebugExitDecision(140); }
			switch (alt140)
			{
			case 1:
				DebugEnterAlt(1);
				// JavaScript.g3:315:7: primaryExpression
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(315, 7);
				PushFollow(Follow._primaryExpression_in_memberExpressionLHS2603);
				primaryExpression249=primaryExpression();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, primaryExpression249.Tree);

				}
				break;
			case 2:
				DebugEnterAlt(2);
				// JavaScript.g3:316:7: functionExpression
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(316, 7);
				PushFollow(Follow._functionExpression_in_memberExpressionLHS2611);
				functionExpression250=functionExpression();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, functionExpression250.Tree);

				}
				break;
			case 3:
				DebugEnterAlt(3);
				// JavaScript.g3:317:7: newObjectExpression
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(317, 7);
				PushFollow(Follow._newObjectExpression_in_memberExpressionLHS2619);
				newObjectExpression251=newObjectExpression();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, newObjectExpression251.Tree);

				}
				break;

			}
			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("memberExpressionLHS", 48);
			LeaveRule("memberExpressionLHS", 48);
			Leave_memberExpressionLHS();
			if (state.backtracking > 0) { Memoize(input, 48, memberExpressionLHS_StartIndex); }
		}
		DebugLocation(318, 4);
		} finally { DebugExitRule(GrammarFileName, "memberExpressionLHS"); }
		return retval;

	}
	// $ANTLR end "memberExpressionLHS"

	public class newObjectExpression_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_newObjectExpression();
	partial void Leave_newObjectExpression();

	// $ANTLR start "newObjectExpression"
	// JavaScript.g3:320:1: newObjectExpression : NEW ( SkipSpace )* memb= memberExpression ( SkipSpace )* args= arguments -> ^( NEW_OBJ_EXPR $memb $args) ;
	[GrammarRule("newObjectExpression")]
	private JavaScriptParser.newObjectExpression_return newObjectExpression()
	{
		Enter_newObjectExpression();
		EnterRule("newObjectExpression", 49);
		TraceIn("newObjectExpression", 49);
		JavaScriptParser.newObjectExpression_return retval = new JavaScriptParser.newObjectExpression_return();
		retval.Start = (CommonToken)input.LT(1);
		int newObjectExpression_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken NEW252=null;
		CommonToken SkipSpace253=null;
		CommonToken SkipSpace254=null;
		JavaScriptParser.memberExpression_return memb = default(JavaScriptParser.memberExpression_return);
		JavaScriptParser.arguments_return args = default(JavaScriptParser.arguments_return);

		CommonTree NEW252_tree=null;
		CommonTree SkipSpace253_tree=null;
		CommonTree SkipSpace254_tree=null;
		RewriteRuleITokenStream stream_NEW=new RewriteRuleITokenStream(adaptor,"token NEW");
		RewriteRuleITokenStream stream_SkipSpace=new RewriteRuleITokenStream(adaptor,"token SkipSpace");
		RewriteRuleSubtreeStream stream_memberExpression=new RewriteRuleSubtreeStream(adaptor,"rule memberExpression");
		RewriteRuleSubtreeStream stream_arguments=new RewriteRuleSubtreeStream(adaptor,"rule arguments");
		try { DebugEnterRule(GrammarFileName, "newObjectExpression");
		DebugLocation(320, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 49)) { return retval; }
			// JavaScript.g3:321:5: ( NEW ( SkipSpace )* memb= memberExpression ( SkipSpace )* args= arguments -> ^( NEW_OBJ_EXPR $memb $args) )
			DebugEnterAlt(1);
			// JavaScript.g3:321:7: NEW ( SkipSpace )* memb= memberExpression ( SkipSpace )* args= arguments
			{
			DebugLocation(321, 7);
			NEW252=(CommonToken)Match(input,NEW,Follow._NEW_in_newObjectExpression2636); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_NEW.Add(NEW252);

			DebugLocation(321, 11);
			// JavaScript.g3:321:11: ( SkipSpace )*
			try { DebugEnterSubRule(141);
			while (true)
			{
				int alt141=2;
				try { DebugEnterDecision(141, decisionCanBacktrack[141]);
				int LA141_0 = input.LA(1);

				if ((LA141_0==SkipSpace))
				{
					alt141=1;
				}


				} finally { DebugExitDecision(141); }
				switch ( alt141 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:321:11: SkipSpace
					{
					DebugLocation(321, 11);
					SkipSpace253=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_newObjectExpression2638); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace253);


					}
					break;

				default:
					goto loop141;
				}
			}

			loop141:
				;

			} finally { DebugExitSubRule(141); }

			DebugLocation(321, 26);
			PushFollow(Follow._memberExpression_in_newObjectExpression2643);
			memb=memberExpression();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_memberExpression.Add(memb.Tree);
			DebugLocation(321, 44);
			// JavaScript.g3:321:44: ( SkipSpace )*
			try { DebugEnterSubRule(142);
			while (true)
			{
				int alt142=2;
				try { DebugEnterDecision(142, decisionCanBacktrack[142]);
				int LA142_0 = input.LA(1);

				if ((LA142_0==SkipSpace))
				{
					alt142=1;
				}


				} finally { DebugExitDecision(142); }
				switch ( alt142 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:321:44: SkipSpace
					{
					DebugLocation(321, 44);
					SkipSpace254=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_newObjectExpression2645); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace254);


					}
					break;

				default:
					goto loop142;
				}
			}

			loop142:
				;

			} finally { DebugExitSubRule(142); }

			DebugLocation(321, 59);
			PushFollow(Follow._arguments_in_newObjectExpression2650);
			args=arguments();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_arguments.Add(args.Tree);


			{
			// AST REWRITE
			// elements: memb, args
			// token labels: 
			// rule labels: memb, args, retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_memb=new RewriteRuleSubtreeStream(adaptor,"rule memb",memb!=null?memb.Tree:null);
			RewriteRuleSubtreeStream stream_args=new RewriteRuleSubtreeStream(adaptor,"rule args",args!=null?args.Tree:null);
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 322:9: -> ^( NEW_OBJ_EXPR $memb $args)
			{
				DebugLocation(322, 12);
				// JavaScript.g3:322:12: ^( NEW_OBJ_EXPR $memb $args)
				{
				CommonTree root_1 = (CommonTree)adaptor.Nil();
				DebugLocation(322, 14);
				root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(NEW_OBJ_EXPR, "NEW_OBJ_EXPR"), root_1);

				DebugLocation(322, 28);
				adaptor.AddChild(root_1, stream_memb.NextTree());
				DebugLocation(322, 34);
				adaptor.AddChild(root_1, stream_args.NextTree());

				adaptor.AddChild(root_0, root_1);
				}

			}

			retval.Tree = root_0;
			}
			}

			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("newObjectExpression", 49);
			LeaveRule("newObjectExpression", 49);
			Leave_newObjectExpression();
			if (state.backtracking > 0) { Memoize(input, 49, newObjectExpression_StartIndex); }
		}
		DebugLocation(323, 4);
		} finally { DebugExitRule(GrammarFileName, "newObjectExpression"); }
		return retval;

	}
	// $ANTLR end "newObjectExpression"

	public class memberExpressionSuffix_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_memberExpressionSuffix();
	partial void Leave_memberExpressionSuffix();

	// $ANTLR start "memberExpressionSuffix"
	// JavaScript.g3:325:1: memberExpressionSuffix : ( indexSuffix | propertyReferenceSuffix );
	[GrammarRule("memberExpressionSuffix")]
	private JavaScriptParser.memberExpressionSuffix_return memberExpressionSuffix()
	{
		Enter_memberExpressionSuffix();
		EnterRule("memberExpressionSuffix", 50);
		TraceIn("memberExpressionSuffix", 50);
		JavaScriptParser.memberExpressionSuffix_return retval = new JavaScriptParser.memberExpressionSuffix_return();
		retval.Start = (CommonToken)input.LT(1);
		int memberExpressionSuffix_StartIndex = input.Index;
		CommonTree root_0 = null;

		JavaScriptParser.indexSuffix_return indexSuffix255 = default(JavaScriptParser.indexSuffix_return);
		JavaScriptParser.propertyReferenceSuffix_return propertyReferenceSuffix256 = default(JavaScriptParser.propertyReferenceSuffix_return);


		try { DebugEnterRule(GrammarFileName, "memberExpressionSuffix");
		DebugLocation(325, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 50)) { return retval; }
			// JavaScript.g3:326:5: ( indexSuffix | propertyReferenceSuffix )
			int alt143=2;
			try { DebugEnterDecision(143, decisionCanBacktrack[143]);
			int LA143_0 = input.LA(1);

			if ((LA143_0==135))
			{
				alt143=1;
			}
			else if ((LA143_0==132))
			{
				alt143=2;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return retval;}
				NoViableAltException nvae = new NoViableAltException("", 143, 0, input);

				DebugRecognitionException(nvae);
				throw nvae;
			}
			} finally { DebugExitDecision(143); }
			switch (alt143)
			{
			case 1:
				DebugEnterAlt(1);
				// JavaScript.g3:326:7: indexSuffix
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(326, 7);
				PushFollow(Follow._indexSuffix_in_memberExpressionSuffix2687);
				indexSuffix255=indexSuffix();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, indexSuffix255.Tree);

				}
				break;
			case 2:
				DebugEnterAlt(2);
				// JavaScript.g3:327:7: propertyReferenceSuffix
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(327, 7);
				PushFollow(Follow._propertyReferenceSuffix_in_memberExpressionSuffix2695);
				propertyReferenceSuffix256=propertyReferenceSuffix();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, propertyReferenceSuffix256.Tree);

				}
				break;

			}
			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("memberExpressionSuffix", 50);
			LeaveRule("memberExpressionSuffix", 50);
			Leave_memberExpressionSuffix();
			if (state.backtracking > 0) { Memoize(input, 50, memberExpressionSuffix_StartIndex); }
		}
		DebugLocation(328, 4);
		} finally { DebugExitRule(GrammarFileName, "memberExpressionSuffix"); }
		return retval;

	}
	// $ANTLR end "memberExpressionSuffix"

	public class callExpression_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_callExpression();
	partial void Leave_callExpression();

	// $ANTLR start "callExpression"
	// JavaScript.g3:330:1: callExpression : (lhs= methodCallExpression -> $lhs) ( ( SkipSpace )* rhs= callExpressionSuffix -> ^( ACCESSOR_EXPR $callExpression $rhs) )* ;
	[GrammarRule("callExpression")]
	private JavaScriptParser.callExpression_return callExpression()
	{
		Enter_callExpression();
		EnterRule("callExpression", 51);
		TraceIn("callExpression", 51);
		JavaScriptParser.callExpression_return retval = new JavaScriptParser.callExpression_return();
		retval.Start = (CommonToken)input.LT(1);
		int callExpression_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken SkipSpace257=null;
		JavaScriptParser.methodCallExpression_return lhs = default(JavaScriptParser.methodCallExpression_return);
		JavaScriptParser.callExpressionSuffix_return rhs = default(JavaScriptParser.callExpressionSuffix_return);

		CommonTree SkipSpace257_tree=null;
		RewriteRuleITokenStream stream_SkipSpace=new RewriteRuleITokenStream(adaptor,"token SkipSpace");
		RewriteRuleSubtreeStream stream_methodCallExpression=new RewriteRuleSubtreeStream(adaptor,"rule methodCallExpression");
		RewriteRuleSubtreeStream stream_callExpressionSuffix=new RewriteRuleSubtreeStream(adaptor,"rule callExpressionSuffix");
		try { DebugEnterRule(GrammarFileName, "callExpression");
		DebugLocation(330, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 51)) { return retval; }
			// JavaScript.g3:331:5: ( (lhs= methodCallExpression -> $lhs) ( ( SkipSpace )* rhs= callExpressionSuffix -> ^( ACCESSOR_EXPR $callExpression $rhs) )* )
			DebugEnterAlt(1);
			// JavaScript.g3:331:7: (lhs= methodCallExpression -> $lhs) ( ( SkipSpace )* rhs= callExpressionSuffix -> ^( ACCESSOR_EXPR $callExpression $rhs) )*
			{
			DebugLocation(331, 7);
			// JavaScript.g3:331:7: (lhs= methodCallExpression -> $lhs)
			DebugEnterAlt(1);
			// JavaScript.g3:331:8: lhs= methodCallExpression
			{
			DebugLocation(331, 11);
			PushFollow(Follow._methodCallExpression_in_callExpression2715);
			lhs=methodCallExpression();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_methodCallExpression.Add(lhs.Tree);


			{
			// AST REWRITE
			// elements: lhs
			// token labels: 
			// rule labels: lhs, retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_lhs=new RewriteRuleSubtreeStream(adaptor,"rule lhs",lhs!=null?lhs.Tree:null);
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 331:33: -> $lhs
			{
				DebugLocation(331, 37);
				adaptor.AddChild(root_0, stream_lhs.NextTree());

			}

			retval.Tree = root_0;
			}
			}

			}

			DebugLocation(332, 9);
			// JavaScript.g3:332:9: ( ( SkipSpace )* rhs= callExpressionSuffix -> ^( ACCESSOR_EXPR $callExpression $rhs) )*
			try { DebugEnterSubRule(145);
			while (true)
			{
				int alt145=2;
				try { DebugEnterDecision(145, decisionCanBacktrack[145]);
				try
				{
					alt145 = dfa145.Predict(input);
				}
				catch (NoViableAltException nvae)
				{
					DebugRecognitionException(nvae);
					throw;
				}
				} finally { DebugExitDecision(145); }
				switch ( alt145 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:332:10: ( SkipSpace )* rhs= callExpressionSuffix
					{
					DebugLocation(332, 10);
					// JavaScript.g3:332:10: ( SkipSpace )*
					try { DebugEnterSubRule(144);
					while (true)
					{
						int alt144=2;
						try { DebugEnterDecision(144, decisionCanBacktrack[144]);
						int LA144_0 = input.LA(1);

						if ((LA144_0==SkipSpace))
						{
							alt144=1;
						}


						} finally { DebugExitDecision(144); }
						switch ( alt144 )
						{
						case 1:
							DebugEnterAlt(1);
							// JavaScript.g3:332:10: SkipSpace
							{
							DebugLocation(332, 10);
							SkipSpace257=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_callExpression2732); if (state.failed) return retval; 
							if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace257);


							}
							break;

						default:
							goto loop144;
						}
					}

					loop144:
						;

					} finally { DebugExitSubRule(144); }

					DebugLocation(332, 24);
					PushFollow(Follow._callExpressionSuffix_in_callExpression2737);
					rhs=callExpressionSuffix();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_callExpressionSuffix.Add(rhs.Tree);


					{
					// AST REWRITE
					// elements: callExpression, rhs
					// token labels: 
					// rule labels: rhs, retval
					// token list labels: 
					// rule list labels: 
					// wildcard labels: 
					if ( state.backtracking == 0 ) {
					retval.Tree = root_0;
					RewriteRuleSubtreeStream stream_rhs=new RewriteRuleSubtreeStream(adaptor,"rule rhs",rhs!=null?rhs.Tree:null);
					RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

					root_0 = (CommonTree)adaptor.Nil();
					// 332:46: -> ^( ACCESSOR_EXPR $callExpression $rhs)
					{
						DebugLocation(332, 49);
						// JavaScript.g3:332:49: ^( ACCESSOR_EXPR $callExpression $rhs)
						{
						CommonTree root_1 = (CommonTree)adaptor.Nil();
						DebugLocation(332, 51);
						root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(ACCESSOR_EXPR, "ACCESSOR_EXPR"), root_1);

						DebugLocation(332, 66);
						adaptor.AddChild(root_1, stream_retval.NextTree());
						DebugLocation(332, 82);
						adaptor.AddChild(root_1, stream_rhs.NextTree());

						adaptor.AddChild(root_0, root_1);
						}

					}

					retval.Tree = root_0;
					}
					}

					}
					break;

				default:
					goto loop145;
				}
			}

			loop145:
				;

			} finally { DebugExitSubRule(145); }


			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("callExpression", 51);
			LeaveRule("callExpression", 51);
			Leave_callExpression();
			if (state.backtracking > 0) { Memoize(input, 51, callExpression_StartIndex); }
		}
		DebugLocation(333, 4);
		} finally { DebugExitRule(GrammarFileName, "callExpression"); }
		return retval;

	}
	// $ANTLR end "callExpression"

	public class methodCallExpression_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_methodCallExpression();
	partial void Leave_methodCallExpression();

	// $ANTLR start "methodCallExpression"
	// JavaScript.g3:335:1: methodCallExpression : method= memberExpression ( SkipSpace )* args= arguments -> ^( METHOD_CALL $method $args) ;
	[GrammarRule("methodCallExpression")]
	private JavaScriptParser.methodCallExpression_return methodCallExpression()
	{
		Enter_methodCallExpression();
		EnterRule("methodCallExpression", 52);
		TraceIn("methodCallExpression", 52);
		JavaScriptParser.methodCallExpression_return retval = new JavaScriptParser.methodCallExpression_return();
		retval.Start = (CommonToken)input.LT(1);
		int methodCallExpression_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken SkipSpace258=null;
		JavaScriptParser.memberExpression_return method = default(JavaScriptParser.memberExpression_return);
		JavaScriptParser.arguments_return args = default(JavaScriptParser.arguments_return);

		CommonTree SkipSpace258_tree=null;
		RewriteRuleITokenStream stream_SkipSpace=new RewriteRuleITokenStream(adaptor,"token SkipSpace");
		RewriteRuleSubtreeStream stream_memberExpression=new RewriteRuleSubtreeStream(adaptor,"rule memberExpression");
		RewriteRuleSubtreeStream stream_arguments=new RewriteRuleSubtreeStream(adaptor,"rule arguments");
		try { DebugEnterRule(GrammarFileName, "methodCallExpression");
		DebugLocation(335, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 52)) { return retval; }
			// JavaScript.g3:336:5: (method= memberExpression ( SkipSpace )* args= arguments -> ^( METHOD_CALL $method $args) )
			DebugEnterAlt(1);
			// JavaScript.g3:336:9: method= memberExpression ( SkipSpace )* args= arguments
			{
			DebugLocation(336, 15);
			PushFollow(Follow._memberExpression_in_methodCallExpression2773);
			method=memberExpression();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_memberExpression.Add(method.Tree);
			DebugLocation(336, 33);
			// JavaScript.g3:336:33: ( SkipSpace )*
			try { DebugEnterSubRule(146);
			while (true)
			{
				int alt146=2;
				try { DebugEnterDecision(146, decisionCanBacktrack[146]);
				int LA146_0 = input.LA(1);

				if ((LA146_0==SkipSpace))
				{
					alt146=1;
				}


				} finally { DebugExitDecision(146); }
				switch ( alt146 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:336:33: SkipSpace
					{
					DebugLocation(336, 33);
					SkipSpace258=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_methodCallExpression2775); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace258);


					}
					break;

				default:
					goto loop146;
				}
			}

			loop146:
				;

			} finally { DebugExitSubRule(146); }

			DebugLocation(336, 48);
			PushFollow(Follow._arguments_in_methodCallExpression2780);
			args=arguments();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_arguments.Add(args.Tree);


			{
			// AST REWRITE
			// elements: method, args
			// token labels: 
			// rule labels: method, args, retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_method=new RewriteRuleSubtreeStream(adaptor,"rule method",method!=null?method.Tree:null);
			RewriteRuleSubtreeStream stream_args=new RewriteRuleSubtreeStream(adaptor,"rule args",args!=null?args.Tree:null);
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 337:9: -> ^( METHOD_CALL $method $args)
			{
				DebugLocation(337, 12);
				// JavaScript.g3:337:12: ^( METHOD_CALL $method $args)
				{
				CommonTree root_1 = (CommonTree)adaptor.Nil();
				DebugLocation(337, 14);
				root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(METHOD_CALL, "METHOD_CALL"), root_1);

				DebugLocation(337, 27);
				adaptor.AddChild(root_1, stream_method.NextTree());
				DebugLocation(337, 35);
				adaptor.AddChild(root_1, stream_args.NextTree());

				adaptor.AddChild(root_0, root_1);
				}

			}

			retval.Tree = root_0;
			}
			}

			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("methodCallExpression", 52);
			LeaveRule("methodCallExpression", 52);
			Leave_methodCallExpression();
			if (state.backtracking > 0) { Memoize(input, 52, methodCallExpression_StartIndex); }
		}
		DebugLocation(338, 4);
		} finally { DebugExitRule(GrammarFileName, "methodCallExpression"); }
		return retval;

	}
	// $ANTLR end "methodCallExpression"

	public class callExpressionSuffix_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_callExpressionSuffix();
	partial void Leave_callExpressionSuffix();

	// $ANTLR start "callExpressionSuffix"
	// JavaScript.g3:340:1: callExpressionSuffix : ( arguments | indexSuffix | propertyReferenceSuffix );
	[GrammarRule("callExpressionSuffix")]
	private JavaScriptParser.callExpressionSuffix_return callExpressionSuffix()
	{
		Enter_callExpressionSuffix();
		EnterRule("callExpressionSuffix", 53);
		TraceIn("callExpressionSuffix", 53);
		JavaScriptParser.callExpressionSuffix_return retval = new JavaScriptParser.callExpressionSuffix_return();
		retval.Start = (CommonToken)input.LT(1);
		int callExpressionSuffix_StartIndex = input.Index;
		CommonTree root_0 = null;

		JavaScriptParser.arguments_return arguments259 = default(JavaScriptParser.arguments_return);
		JavaScriptParser.indexSuffix_return indexSuffix260 = default(JavaScriptParser.indexSuffix_return);
		JavaScriptParser.propertyReferenceSuffix_return propertyReferenceSuffix261 = default(JavaScriptParser.propertyReferenceSuffix_return);


		try { DebugEnterRule(GrammarFileName, "callExpressionSuffix");
		DebugLocation(340, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 53)) { return retval; }
			// JavaScript.g3:341:5: ( arguments | indexSuffix | propertyReferenceSuffix )
			int alt147=3;
			try { DebugEnterDecision(147, decisionCanBacktrack[147]);
			switch (input.LA(1))
			{
			case 130:
				{
				alt147=1;
				}
				break;
			case 135:
				{
				alt147=2;
				}
				break;
			case 132:
				{
				alt147=3;
				}
				break;
			default:
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 147, 0, input);

					DebugRecognitionException(nvae);
					throw nvae;
				}
			}

			} finally { DebugExitDecision(147); }
			switch (alt147)
			{
			case 1:
				DebugEnterAlt(1);
				// JavaScript.g3:341:7: arguments
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(341, 7);
				PushFollow(Follow._arguments_in_callExpressionSuffix2817);
				arguments259=arguments();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, arguments259.Tree);

				}
				break;
			case 2:
				DebugEnterAlt(2);
				// JavaScript.g3:342:7: indexSuffix
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(342, 7);
				PushFollow(Follow._indexSuffix_in_callExpressionSuffix2825);
				indexSuffix260=indexSuffix();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, indexSuffix260.Tree);

				}
				break;
			case 3:
				DebugEnterAlt(3);
				// JavaScript.g3:343:7: propertyReferenceSuffix
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(343, 7);
				PushFollow(Follow._propertyReferenceSuffix_in_callExpressionSuffix2833);
				propertyReferenceSuffix261=propertyReferenceSuffix();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, propertyReferenceSuffix261.Tree);

				}
				break;

			}
			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("callExpressionSuffix", 53);
			LeaveRule("callExpressionSuffix", 53);
			Leave_callExpressionSuffix();
			if (state.backtracking > 0) { Memoize(input, 53, callExpressionSuffix_StartIndex); }
		}
		DebugLocation(344, 4);
		} finally { DebugExitRule(GrammarFileName, "callExpressionSuffix"); }
		return retval;

	}
	// $ANTLR end "callExpressionSuffix"

	public class arguments_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_arguments();
	partial void Leave_arguments();

	// $ANTLR start "arguments"
	// JavaScript.g3:346:1: arguments : '(' ( ( SkipSpace )* assignmentExpression ( ( SkipSpace )* ',' ( SkipSpace )* assignmentExpression )* )? ( SkipSpace )* ')' -> ^( ARGS ( assignmentExpression )* ) ;
	[GrammarRule("arguments")]
	private JavaScriptParser.arguments_return arguments()
	{
		Enter_arguments();
		EnterRule("arguments", 54);
		TraceIn("arguments", 54);
		JavaScriptParser.arguments_return retval = new JavaScriptParser.arguments_return();
		retval.Start = (CommonToken)input.LT(1);
		int arguments_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken char_literal262=null;
		CommonToken SkipSpace263=null;
		CommonToken SkipSpace265=null;
		CommonToken char_literal266=null;
		CommonToken SkipSpace267=null;
		CommonToken SkipSpace269=null;
		CommonToken char_literal270=null;
		JavaScriptParser.assignmentExpression_return assignmentExpression264 = default(JavaScriptParser.assignmentExpression_return);
		JavaScriptParser.assignmentExpression_return assignmentExpression268 = default(JavaScriptParser.assignmentExpression_return);

		CommonTree char_literal262_tree=null;
		CommonTree SkipSpace263_tree=null;
		CommonTree SkipSpace265_tree=null;
		CommonTree char_literal266_tree=null;
		CommonTree SkipSpace267_tree=null;
		CommonTree SkipSpace269_tree=null;
		CommonTree char_literal270_tree=null;
		RewriteRuleITokenStream stream_130=new RewriteRuleITokenStream(adaptor,"token 130");
		RewriteRuleITokenStream stream_SkipSpace=new RewriteRuleITokenStream(adaptor,"token SkipSpace");
		RewriteRuleITokenStream stream_COMMA=new RewriteRuleITokenStream(adaptor,"token COMMA");
		RewriteRuleITokenStream stream_131=new RewriteRuleITokenStream(adaptor,"token 131");
		RewriteRuleSubtreeStream stream_assignmentExpression=new RewriteRuleSubtreeStream(adaptor,"rule assignmentExpression");
		try { DebugEnterRule(GrammarFileName, "arguments");
		DebugLocation(346, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 54)) { return retval; }
			// JavaScript.g3:347:5: ( '(' ( ( SkipSpace )* assignmentExpression ( ( SkipSpace )* ',' ( SkipSpace )* assignmentExpression )* )? ( SkipSpace )* ')' -> ^( ARGS ( assignmentExpression )* ) )
			DebugEnterAlt(1);
			// JavaScript.g3:347:7: '(' ( ( SkipSpace )* assignmentExpression ( ( SkipSpace )* ',' ( SkipSpace )* assignmentExpression )* )? ( SkipSpace )* ')'
			{
			DebugLocation(347, 7);
			char_literal262=(CommonToken)Match(input,130,Follow._130_in_arguments2850); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_130.Add(char_literal262);

			DebugLocation(347, 11);
			// JavaScript.g3:347:11: ( ( SkipSpace )* assignmentExpression ( ( SkipSpace )* ',' ( SkipSpace )* assignmentExpression )* )?
			int alt152=2;
			try { DebugEnterSubRule(152);
			try { DebugEnterDecision(152, decisionCanBacktrack[152]);
			try
			{
				alt152 = dfa152.Predict(input);
			}
			catch (NoViableAltException nvae)
			{
				DebugRecognitionException(nvae);
				throw;
			}
			} finally { DebugExitDecision(152); }
			switch (alt152)
			{
			case 1:
				DebugEnterAlt(1);
				// JavaScript.g3:347:12: ( SkipSpace )* assignmentExpression ( ( SkipSpace )* ',' ( SkipSpace )* assignmentExpression )*
				{
				DebugLocation(347, 12);
				// JavaScript.g3:347:12: ( SkipSpace )*
				try { DebugEnterSubRule(148);
				while (true)
				{
					int alt148=2;
					try { DebugEnterDecision(148, decisionCanBacktrack[148]);
					int LA148_0 = input.LA(1);

					if ((LA148_0==SkipSpace))
					{
						alt148=1;
					}


					} finally { DebugExitDecision(148); }
					switch ( alt148 )
					{
					case 1:
						DebugEnterAlt(1);
						// JavaScript.g3:347:12: SkipSpace
						{
						DebugLocation(347, 12);
						SkipSpace263=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_arguments2853); if (state.failed) return retval; 
						if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace263);


						}
						break;

					default:
						goto loop148;
					}
				}

				loop148:
					;

				} finally { DebugExitSubRule(148); }

				DebugLocation(347, 23);
				PushFollow(Follow._assignmentExpression_in_arguments2856);
				assignmentExpression264=assignmentExpression();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_assignmentExpression.Add(assignmentExpression264.Tree);
				DebugLocation(347, 44);
				// JavaScript.g3:347:44: ( ( SkipSpace )* ',' ( SkipSpace )* assignmentExpression )*
				try { DebugEnterSubRule(151);
				while (true)
				{
					int alt151=2;
					try { DebugEnterDecision(151, decisionCanBacktrack[151]);
					try
					{
						alt151 = dfa151.Predict(input);
					}
					catch (NoViableAltException nvae)
					{
						DebugRecognitionException(nvae);
						throw;
					}
					} finally { DebugExitDecision(151); }
					switch ( alt151 )
					{
					case 1:
						DebugEnterAlt(1);
						// JavaScript.g3:347:45: ( SkipSpace )* ',' ( SkipSpace )* assignmentExpression
						{
						DebugLocation(347, 45);
						// JavaScript.g3:347:45: ( SkipSpace )*
						try { DebugEnterSubRule(149);
						while (true)
						{
							int alt149=2;
							try { DebugEnterDecision(149, decisionCanBacktrack[149]);
							int LA149_0 = input.LA(1);

							if ((LA149_0==SkipSpace))
							{
								alt149=1;
							}


							} finally { DebugExitDecision(149); }
							switch ( alt149 )
							{
							case 1:
								DebugEnterAlt(1);
								// JavaScript.g3:347:45: SkipSpace
								{
								DebugLocation(347, 45);
								SkipSpace265=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_arguments2859); if (state.failed) return retval; 
								if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace265);


								}
								break;

							default:
								goto loop149;
							}
						}

						loop149:
							;

						} finally { DebugExitSubRule(149); }

						DebugLocation(347, 56);
						char_literal266=(CommonToken)Match(input,COMMA,Follow._COMMA_in_arguments2862); if (state.failed) return retval; 
						if ( state.backtracking == 0 ) stream_COMMA.Add(char_literal266);

						DebugLocation(347, 60);
						// JavaScript.g3:347:60: ( SkipSpace )*
						try { DebugEnterSubRule(150);
						while (true)
						{
							int alt150=2;
							try { DebugEnterDecision(150, decisionCanBacktrack[150]);
							int LA150_0 = input.LA(1);

							if ((LA150_0==SkipSpace))
							{
								alt150=1;
							}


							} finally { DebugExitDecision(150); }
							switch ( alt150 )
							{
							case 1:
								DebugEnterAlt(1);
								// JavaScript.g3:347:60: SkipSpace
								{
								DebugLocation(347, 60);
								SkipSpace267=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_arguments2864); if (state.failed) return retval; 
								if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace267);


								}
								break;

							default:
								goto loop150;
							}
						}

						loop150:
							;

						} finally { DebugExitSubRule(150); }

						DebugLocation(347, 71);
						PushFollow(Follow._assignmentExpression_in_arguments2867);
						assignmentExpression268=assignmentExpression();
						PopFollow();
						if (state.failed) return retval;
						if ( state.backtracking == 0 ) stream_assignmentExpression.Add(assignmentExpression268.Tree);

						}
						break;

					default:
						goto loop151;
					}
				}

				loop151:
					;

				} finally { DebugExitSubRule(151); }


				}
				break;

			}
			} finally { DebugExitSubRule(152); }

			DebugLocation(347, 96);
			// JavaScript.g3:347:96: ( SkipSpace )*
			try { DebugEnterSubRule(153);
			while (true)
			{
				int alt153=2;
				try { DebugEnterDecision(153, decisionCanBacktrack[153]);
				int LA153_0 = input.LA(1);

				if ((LA153_0==SkipSpace))
				{
					alt153=1;
				}


				} finally { DebugExitDecision(153); }
				switch ( alt153 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:347:96: SkipSpace
					{
					DebugLocation(347, 96);
					SkipSpace269=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_arguments2873); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace269);


					}
					break;

				default:
					goto loop153;
				}
			}

			loop153:
				;

			} finally { DebugExitSubRule(153); }

			DebugLocation(347, 107);
			char_literal270=(CommonToken)Match(input,131,Follow._131_in_arguments2876); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_131.Add(char_literal270);



			{
			// AST REWRITE
			// elements: assignmentExpression
			// token labels: 
			// rule labels: retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 348:9: -> ^( ARGS ( assignmentExpression )* )
			{
				DebugLocation(348, 12);
				// JavaScript.g3:348:12: ^( ARGS ( assignmentExpression )* )
				{
				CommonTree root_1 = (CommonTree)adaptor.Nil();
				DebugLocation(348, 14);
				root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(ARGS, "ARGS"), root_1);

				DebugLocation(348, 19);
				// JavaScript.g3:348:19: ( assignmentExpression )*
				while ( stream_assignmentExpression.HasNext )
				{
					DebugLocation(348, 19);
					adaptor.AddChild(root_1, stream_assignmentExpression.NextTree());

				}
				stream_assignmentExpression.Reset();

				adaptor.AddChild(root_0, root_1);
				}

			}

			retval.Tree = root_0;
			}
			}

			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("arguments", 54);
			LeaveRule("arguments", 54);
			Leave_arguments();
			if (state.backtracking > 0) { Memoize(input, 54, arguments_StartIndex); }
		}
		DebugLocation(349, 4);
		} finally { DebugExitRule(GrammarFileName, "arguments"); }
		return retval;

	}
	// $ANTLR end "arguments"

	public class indexSuffix_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_indexSuffix();
	partial void Leave_indexSuffix();

	// $ANTLR start "indexSuffix"
	// JavaScript.g3:351:1: indexSuffix : '[' ( SkipSpace )* expr= expression ( SkipSpace )* ']' -> ^( INDEX_EXPR $expr) ;
	[GrammarRule("indexSuffix")]
	private JavaScriptParser.indexSuffix_return indexSuffix()
	{
		Enter_indexSuffix();
		EnterRule("indexSuffix", 55);
		TraceIn("indexSuffix", 55);
		JavaScriptParser.indexSuffix_return retval = new JavaScriptParser.indexSuffix_return();
		retval.Start = (CommonToken)input.LT(1);
		int indexSuffix_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken char_literal271=null;
		CommonToken SkipSpace272=null;
		CommonToken SkipSpace273=null;
		CommonToken char_literal274=null;
		JavaScriptParser.expression_return expr = default(JavaScriptParser.expression_return);

		CommonTree char_literal271_tree=null;
		CommonTree SkipSpace272_tree=null;
		CommonTree SkipSpace273_tree=null;
		CommonTree char_literal274_tree=null;
		RewriteRuleITokenStream stream_135=new RewriteRuleITokenStream(adaptor,"token 135");
		RewriteRuleITokenStream stream_SkipSpace=new RewriteRuleITokenStream(adaptor,"token SkipSpace");
		RewriteRuleITokenStream stream_136=new RewriteRuleITokenStream(adaptor,"token 136");
		RewriteRuleSubtreeStream stream_expression=new RewriteRuleSubtreeStream(adaptor,"rule expression");
		try { DebugEnterRule(GrammarFileName, "indexSuffix");
		DebugLocation(351, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 55)) { return retval; }
			// JavaScript.g3:352:5: ( '[' ( SkipSpace )* expr= expression ( SkipSpace )* ']' -> ^( INDEX_EXPR $expr) )
			DebugEnterAlt(1);
			// JavaScript.g3:352:7: '[' ( SkipSpace )* expr= expression ( SkipSpace )* ']'
			{
			DebugLocation(352, 7);
			char_literal271=(CommonToken)Match(input,135,Follow._135_in_indexSuffix2910); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_135.Add(char_literal271);

			DebugLocation(352, 11);
			// JavaScript.g3:352:11: ( SkipSpace )*
			try { DebugEnterSubRule(154);
			while (true)
			{
				int alt154=2;
				try { DebugEnterDecision(154, decisionCanBacktrack[154]);
				int LA154_0 = input.LA(1);

				if ((LA154_0==SkipSpace))
				{
					alt154=1;
				}


				} finally { DebugExitDecision(154); }
				switch ( alt154 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:352:11: SkipSpace
					{
					DebugLocation(352, 11);
					SkipSpace272=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_indexSuffix2912); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace272);


					}
					break;

				default:
					goto loop154;
				}
			}

			loop154:
				;

			} finally { DebugExitSubRule(154); }

			DebugLocation(352, 26);
			PushFollow(Follow._expression_in_indexSuffix2917);
			expr=expression();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_expression.Add(expr.Tree);
			DebugLocation(352, 38);
			// JavaScript.g3:352:38: ( SkipSpace )*
			try { DebugEnterSubRule(155);
			while (true)
			{
				int alt155=2;
				try { DebugEnterDecision(155, decisionCanBacktrack[155]);
				int LA155_0 = input.LA(1);

				if ((LA155_0==SkipSpace))
				{
					alt155=1;
				}


				} finally { DebugExitDecision(155); }
				switch ( alt155 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:352:38: SkipSpace
					{
					DebugLocation(352, 38);
					SkipSpace273=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_indexSuffix2919); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace273);


					}
					break;

				default:
					goto loop155;
				}
			}

			loop155:
				;

			} finally { DebugExitSubRule(155); }

			DebugLocation(352, 49);
			char_literal274=(CommonToken)Match(input,136,Follow._136_in_indexSuffix2922); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_136.Add(char_literal274);



			{
			// AST REWRITE
			// elements: expr
			// token labels: 
			// rule labels: expr, retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_expr=new RewriteRuleSubtreeStream(adaptor,"rule expr",expr!=null?expr.Tree:null);
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 353:9: -> ^( INDEX_EXPR $expr)
			{
				DebugLocation(353, 12);
				// JavaScript.g3:353:12: ^( INDEX_EXPR $expr)
				{
				CommonTree root_1 = (CommonTree)adaptor.Nil();
				DebugLocation(353, 14);
				root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(INDEX_EXPR, "INDEX_EXPR"), root_1);

				DebugLocation(353, 26);
				adaptor.AddChild(root_1, stream_expr.NextTree());

				adaptor.AddChild(root_0, root_1);
				}

			}

			retval.Tree = root_0;
			}
			}

			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("indexSuffix", 55);
			LeaveRule("indexSuffix", 55);
			Leave_indexSuffix();
			if (state.backtracking > 0) { Memoize(input, 55, indexSuffix_StartIndex); }
		}
		DebugLocation(354, 4);
		} finally { DebugExitRule(GrammarFileName, "indexSuffix"); }
		return retval;

	}
	// $ANTLR end "indexSuffix"

	public class propertyReferenceSuffix_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_propertyReferenceSuffix();
	partial void Leave_propertyReferenceSuffix();

	// $ANTLR start "propertyReferenceSuffix"
	// JavaScript.g3:356:1: propertyReferenceSuffix : ( '.' ( SkipSpace )* id= Identifier -> ^( DOT_ACCESSOR_EXPR $id) | '.' ( SkipSpace )* importId= importedNotation -> ^( INSTANCE_JSNI $importId) );
	[GrammarRule("propertyReferenceSuffix")]
	private JavaScriptParser.propertyReferenceSuffix_return propertyReferenceSuffix()
	{
		Enter_propertyReferenceSuffix();
		EnterRule("propertyReferenceSuffix", 56);
		TraceIn("propertyReferenceSuffix", 56);
		JavaScriptParser.propertyReferenceSuffix_return retval = new JavaScriptParser.propertyReferenceSuffix_return();
		retval.Start = (CommonToken)input.LT(1);
		int propertyReferenceSuffix_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken id=null;
		CommonToken char_literal275=null;
		CommonToken SkipSpace276=null;
		CommonToken char_literal277=null;
		CommonToken SkipSpace278=null;
		JavaScriptParser.importedNotation_return importId = default(JavaScriptParser.importedNotation_return);

		CommonTree id_tree=null;
		CommonTree char_literal275_tree=null;
		CommonTree SkipSpace276_tree=null;
		CommonTree char_literal277_tree=null;
		CommonTree SkipSpace278_tree=null;
		RewriteRuleITokenStream stream_132=new RewriteRuleITokenStream(adaptor,"token 132");
		RewriteRuleITokenStream stream_SkipSpace=new RewriteRuleITokenStream(adaptor,"token SkipSpace");
		RewriteRuleITokenStream stream_Identifier=new RewriteRuleITokenStream(adaptor,"token Identifier");
		RewriteRuleSubtreeStream stream_importedNotation=new RewriteRuleSubtreeStream(adaptor,"rule importedNotation");
		try { DebugEnterRule(GrammarFileName, "propertyReferenceSuffix");
		DebugLocation(356, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 56)) { return retval; }
			// JavaScript.g3:357:5: ( '.' ( SkipSpace )* id= Identifier -> ^( DOT_ACCESSOR_EXPR $id) | '.' ( SkipSpace )* importId= importedNotation -> ^( INSTANCE_JSNI $importId) )
			int alt158=2;
			try { DebugEnterDecision(158, decisionCanBacktrack[158]);
			try
			{
				alt158 = dfa158.Predict(input);
			}
			catch (NoViableAltException nvae)
			{
				DebugRecognitionException(nvae);
				throw;
			}
			} finally { DebugExitDecision(158); }
			switch (alt158)
			{
			case 1:
				DebugEnterAlt(1);
				// JavaScript.g3:357:7: '.' ( SkipSpace )* id= Identifier
				{
				DebugLocation(357, 7);
				char_literal275=(CommonToken)Match(input,132,Follow._132_in_propertyReferenceSuffix2960); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_132.Add(char_literal275);

				DebugLocation(357, 11);
				// JavaScript.g3:357:11: ( SkipSpace )*
				try { DebugEnterSubRule(156);
				while (true)
				{
					int alt156=2;
					try { DebugEnterDecision(156, decisionCanBacktrack[156]);
					int LA156_0 = input.LA(1);

					if ((LA156_0==SkipSpace))
					{
						alt156=1;
					}


					} finally { DebugExitDecision(156); }
					switch ( alt156 )
					{
					case 1:
						DebugEnterAlt(1);
						// JavaScript.g3:357:11: SkipSpace
						{
						DebugLocation(357, 11);
						SkipSpace276=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_propertyReferenceSuffix2962); if (state.failed) return retval; 
						if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace276);


						}
						break;

					default:
						goto loop156;
					}
				}

				loop156:
					;

				} finally { DebugExitSubRule(156); }

				DebugLocation(357, 24);
				id=(CommonToken)Match(input,Identifier,Follow._Identifier_in_propertyReferenceSuffix2967); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_Identifier.Add(id);



				{
				// AST REWRITE
				// elements: id
				// token labels: id
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.Tree = root_0;
				RewriteRuleITokenStream stream_id=new RewriteRuleITokenStream(adaptor,"token id",id);
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

				root_0 = (CommonTree)adaptor.Nil();
				// 358:9: -> ^( DOT_ACCESSOR_EXPR $id)
				{
					DebugLocation(358, 12);
					// JavaScript.g3:358:12: ^( DOT_ACCESSOR_EXPR $id)
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(358, 14);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(DOT_ACCESSOR_EXPR, "DOT_ACCESSOR_EXPR"), root_1);

					DebugLocation(358, 33);
					adaptor.AddChild(root_1, stream_id.NextNode());

					adaptor.AddChild(root_0, root_1);
					}

				}

				retval.Tree = root_0;
				}
				}

				}
				break;
			case 2:
				DebugEnterAlt(2);
				// JavaScript.g3:359:7: '.' ( SkipSpace )* importId= importedNotation
				{
				DebugLocation(359, 7);
				char_literal277=(CommonToken)Match(input,132,Follow._132_in_propertyReferenceSuffix2992); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_132.Add(char_literal277);

				DebugLocation(359, 11);
				// JavaScript.g3:359:11: ( SkipSpace )*
				try { DebugEnterSubRule(157);
				while (true)
				{
					int alt157=2;
					try { DebugEnterDecision(157, decisionCanBacktrack[157]);
					int LA157_0 = input.LA(1);

					if ((LA157_0==SkipSpace))
					{
						alt157=1;
					}


					} finally { DebugExitDecision(157); }
					switch ( alt157 )
					{
					case 1:
						DebugEnterAlt(1);
						// JavaScript.g3:359:11: SkipSpace
						{
						DebugLocation(359, 11);
						SkipSpace278=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_propertyReferenceSuffix2994); if (state.failed) return retval; 
						if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace278);


						}
						break;

					default:
						goto loop157;
					}
				}

				loop157:
					;

				} finally { DebugExitSubRule(157); }

				DebugLocation(359, 30);
				PushFollow(Follow._importedNotation_in_propertyReferenceSuffix2999);
				importId=importedNotation();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_importedNotation.Add(importId.Tree);


				{
				// AST REWRITE
				// elements: importId
				// token labels: 
				// rule labels: importId, retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.Tree = root_0;
				RewriteRuleSubtreeStream stream_importId=new RewriteRuleSubtreeStream(adaptor,"rule importId",importId!=null?importId.Tree:null);
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

				root_0 = (CommonTree)adaptor.Nil();
				// 360:9: -> ^( INSTANCE_JSNI $importId)
				{
					DebugLocation(360, 12);
					// JavaScript.g3:360:12: ^( INSTANCE_JSNI $importId)
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(360, 14);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(INSTANCE_JSNI, "INSTANCE_JSNI"), root_1);

					DebugLocation(360, 29);
					adaptor.AddChild(root_1, stream_importId.NextTree());

					adaptor.AddChild(root_0, root_1);
					}

				}

				retval.Tree = root_0;
				}
				}

				}
				break;

			}
			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("propertyReferenceSuffix", 56);
			LeaveRule("propertyReferenceSuffix", 56);
			Leave_propertyReferenceSuffix();
			if (state.backtracking > 0) { Memoize(input, 56, propertyReferenceSuffix_StartIndex); }
		}
		DebugLocation(361, 4);
		} finally { DebugExitRule(GrammarFileName, "propertyReferenceSuffix"); }
		return retval;

	}
	// $ANTLR end "propertyReferenceSuffix"

	public class conditionalExpression_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_conditionalExpression();
	partial void Leave_conditionalExpression();

	// $ANTLR start "conditionalExpression"
	// JavaScript.g3:363:1: conditionalExpression : (cond= logicalORExpression ( SkipSpace )* '?' ( SkipSpace )* tV= assignmentExpression ( SkipSpace )* ':' ( SkipSpace )* fV= assignmentExpression -> ^( CONDITIONAL_EXPR $cond $tV $fV) | logicalORExpression );
	[GrammarRule("conditionalExpression")]
	private JavaScriptParser.conditionalExpression_return conditionalExpression()
	{
		Enter_conditionalExpression();
		EnterRule("conditionalExpression", 57);
		TraceIn("conditionalExpression", 57);
		JavaScriptParser.conditionalExpression_return retval = new JavaScriptParser.conditionalExpression_return();
		retval.Start = (CommonToken)input.LT(1);
		int conditionalExpression_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken SkipSpace279=null;
		CommonToken char_literal280=null;
		CommonToken SkipSpace281=null;
		CommonToken SkipSpace282=null;
		CommonToken char_literal283=null;
		CommonToken SkipSpace284=null;
		JavaScriptParser.logicalORExpression_return cond = default(JavaScriptParser.logicalORExpression_return);
		JavaScriptParser.assignmentExpression_return tV = default(JavaScriptParser.assignmentExpression_return);
		JavaScriptParser.assignmentExpression_return fV = default(JavaScriptParser.assignmentExpression_return);
		JavaScriptParser.logicalORExpression_return logicalORExpression285 = default(JavaScriptParser.logicalORExpression_return);

		CommonTree SkipSpace279_tree=null;
		CommonTree char_literal280_tree=null;
		CommonTree SkipSpace281_tree=null;
		CommonTree SkipSpace282_tree=null;
		CommonTree char_literal283_tree=null;
		CommonTree SkipSpace284_tree=null;
		RewriteRuleITokenStream stream_SkipSpace=new RewriteRuleITokenStream(adaptor,"token SkipSpace");
		RewriteRuleITokenStream stream_134=new RewriteRuleITokenStream(adaptor,"token 134");
		RewriteRuleITokenStream stream_133=new RewriteRuleITokenStream(adaptor,"token 133");
		RewriteRuleSubtreeStream stream_logicalORExpression=new RewriteRuleSubtreeStream(adaptor,"rule logicalORExpression");
		RewriteRuleSubtreeStream stream_assignmentExpression=new RewriteRuleSubtreeStream(adaptor,"rule assignmentExpression");
		try { DebugEnterRule(GrammarFileName, "conditionalExpression");
		DebugLocation(363, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 57)) { return retval; }
			// JavaScript.g3:364:5: (cond= logicalORExpression ( SkipSpace )* '?' ( SkipSpace )* tV= assignmentExpression ( SkipSpace )* ':' ( SkipSpace )* fV= assignmentExpression -> ^( CONDITIONAL_EXPR $cond $tV $fV) | logicalORExpression )
			int alt163=2;
			try { DebugEnterDecision(163, decisionCanBacktrack[163]);
			try
			{
				alt163 = dfa163.Predict(input);
			}
			catch (NoViableAltException nvae)
			{
				DebugRecognitionException(nvae);
				throw;
			}
			} finally { DebugExitDecision(163); }
			switch (alt163)
			{
			case 1:
				DebugEnterAlt(1);
				// JavaScript.g3:364:7: cond= logicalORExpression ( SkipSpace )* '?' ( SkipSpace )* tV= assignmentExpression ( SkipSpace )* ':' ( SkipSpace )* fV= assignmentExpression
				{
				DebugLocation(364, 11);
				PushFollow(Follow._logicalORExpression_in_conditionalExpression3035);
				cond=logicalORExpression();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_logicalORExpression.Add(cond.Tree);
				DebugLocation(364, 32);
				// JavaScript.g3:364:32: ( SkipSpace )*
				try { DebugEnterSubRule(159);
				while (true)
				{
					int alt159=2;
					try { DebugEnterDecision(159, decisionCanBacktrack[159]);
					int LA159_0 = input.LA(1);

					if ((LA159_0==SkipSpace))
					{
						alt159=1;
					}


					} finally { DebugExitDecision(159); }
					switch ( alt159 )
					{
					case 1:
						DebugEnterAlt(1);
						// JavaScript.g3:364:32: SkipSpace
						{
						DebugLocation(364, 32);
						SkipSpace279=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_conditionalExpression3037); if (state.failed) return retval; 
						if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace279);


						}
						break;

					default:
						goto loop159;
					}
				}

				loop159:
					;

				} finally { DebugExitSubRule(159); }

				DebugLocation(364, 43);
				char_literal280=(CommonToken)Match(input,134,Follow._134_in_conditionalExpression3040); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_134.Add(char_literal280);

				DebugLocation(364, 47);
				// JavaScript.g3:364:47: ( SkipSpace )*
				try { DebugEnterSubRule(160);
				while (true)
				{
					int alt160=2;
					try { DebugEnterDecision(160, decisionCanBacktrack[160]);
					int LA160_0 = input.LA(1);

					if ((LA160_0==SkipSpace))
					{
						alt160=1;
					}


					} finally { DebugExitDecision(160); }
					switch ( alt160 )
					{
					case 1:
						DebugEnterAlt(1);
						// JavaScript.g3:364:47: SkipSpace
						{
						DebugLocation(364, 47);
						SkipSpace281=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_conditionalExpression3042); if (state.failed) return retval; 
						if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace281);


						}
						break;

					default:
						goto loop160;
					}
				}

				loop160:
					;

				} finally { DebugExitSubRule(160); }

				DebugLocation(364, 60);
				PushFollow(Follow._assignmentExpression_in_conditionalExpression3047);
				tV=assignmentExpression();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_assignmentExpression.Add(tV.Tree);
				DebugLocation(364, 82);
				// JavaScript.g3:364:82: ( SkipSpace )*
				try { DebugEnterSubRule(161);
				while (true)
				{
					int alt161=2;
					try { DebugEnterDecision(161, decisionCanBacktrack[161]);
					int LA161_0 = input.LA(1);

					if ((LA161_0==SkipSpace))
					{
						alt161=1;
					}


					} finally { DebugExitDecision(161); }
					switch ( alt161 )
					{
					case 1:
						DebugEnterAlt(1);
						// JavaScript.g3:364:82: SkipSpace
						{
						DebugLocation(364, 82);
						SkipSpace282=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_conditionalExpression3049); if (state.failed) return retval; 
						if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace282);


						}
						break;

					default:
						goto loop161;
					}
				}

				loop161:
					;

				} finally { DebugExitSubRule(161); }

				DebugLocation(364, 93);
				char_literal283=(CommonToken)Match(input,133,Follow._133_in_conditionalExpression3052); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_133.Add(char_literal283);

				DebugLocation(364, 97);
				// JavaScript.g3:364:97: ( SkipSpace )*
				try { DebugEnterSubRule(162);
				while (true)
				{
					int alt162=2;
					try { DebugEnterDecision(162, decisionCanBacktrack[162]);
					int LA162_0 = input.LA(1);

					if ((LA162_0==SkipSpace))
					{
						alt162=1;
					}


					} finally { DebugExitDecision(162); }
					switch ( alt162 )
					{
					case 1:
						DebugEnterAlt(1);
						// JavaScript.g3:364:97: SkipSpace
						{
						DebugLocation(364, 97);
						SkipSpace284=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_conditionalExpression3054); if (state.failed) return retval; 
						if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace284);


						}
						break;

					default:
						goto loop162;
					}
				}

				loop162:
					;

				} finally { DebugExitSubRule(162); }

				DebugLocation(364, 110);
				PushFollow(Follow._assignmentExpression_in_conditionalExpression3059);
				fV=assignmentExpression();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_assignmentExpression.Add(fV.Tree);


				{
				// AST REWRITE
				// elements: cond, tV, fV
				// token labels: 
				// rule labels: cond, tV, fV, retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.Tree = root_0;
				RewriteRuleSubtreeStream stream_cond=new RewriteRuleSubtreeStream(adaptor,"rule cond",cond!=null?cond.Tree:null);
				RewriteRuleSubtreeStream stream_tV=new RewriteRuleSubtreeStream(adaptor,"rule tV",tV!=null?tV.Tree:null);
				RewriteRuleSubtreeStream stream_fV=new RewriteRuleSubtreeStream(adaptor,"rule fV",fV!=null?fV.Tree:null);
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

				root_0 = (CommonTree)adaptor.Nil();
				// 365:9: -> ^( CONDITIONAL_EXPR $cond $tV $fV)
				{
					DebugLocation(365, 12);
					// JavaScript.g3:365:12: ^( CONDITIONAL_EXPR $cond $tV $fV)
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(365, 14);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(CONDITIONAL_EXPR, "CONDITIONAL_EXPR"), root_1);

					DebugLocation(365, 32);
					adaptor.AddChild(root_1, stream_cond.NextTree());
					DebugLocation(365, 38);
					adaptor.AddChild(root_1, stream_tV.NextTree());
					DebugLocation(365, 42);
					adaptor.AddChild(root_1, stream_fV.NextTree());

					adaptor.AddChild(root_0, root_1);
					}

				}

				retval.Tree = root_0;
				}
				}

				}
				break;
			case 2:
				DebugEnterAlt(2);
				// JavaScript.g3:366:7: logicalORExpression
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(366, 7);
				PushFollow(Follow._logicalORExpression_in_conditionalExpression3090);
				logicalORExpression285=logicalORExpression();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, logicalORExpression285.Tree);

				}
				break;

			}
			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("conditionalExpression", 57);
			LeaveRule("conditionalExpression", 57);
			Leave_conditionalExpression();
			if (state.backtracking > 0) { Memoize(input, 57, conditionalExpression_StartIndex); }
		}
		DebugLocation(367, 4);
		} finally { DebugExitRule(GrammarFileName, "conditionalExpression"); }
		return retval;

	}
	// $ANTLR end "conditionalExpression"

	public class conditionalExpressionNoIn_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_conditionalExpressionNoIn();
	partial void Leave_conditionalExpressionNoIn();

	// $ANTLR start "conditionalExpressionNoIn"
	// JavaScript.g3:369:1: conditionalExpressionNoIn : (cond= logicalORExpressionNoIn ( SkipSpace )* '?' ( SkipSpace )* tV= assignmentExpressionNoIn ( SkipSpace )* ':' ( SkipSpace )* fV= assignmentExpressionNoIn -> ^( CONDITIONAL_EXPR $cond $tV $fV) | logicalORExpressionNoIn );
	[GrammarRule("conditionalExpressionNoIn")]
	private JavaScriptParser.conditionalExpressionNoIn_return conditionalExpressionNoIn()
	{
		Enter_conditionalExpressionNoIn();
		EnterRule("conditionalExpressionNoIn", 58);
		TraceIn("conditionalExpressionNoIn", 58);
		JavaScriptParser.conditionalExpressionNoIn_return retval = new JavaScriptParser.conditionalExpressionNoIn_return();
		retval.Start = (CommonToken)input.LT(1);
		int conditionalExpressionNoIn_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken SkipSpace286=null;
		CommonToken char_literal287=null;
		CommonToken SkipSpace288=null;
		CommonToken SkipSpace289=null;
		CommonToken char_literal290=null;
		CommonToken SkipSpace291=null;
		JavaScriptParser.logicalORExpressionNoIn_return cond = default(JavaScriptParser.logicalORExpressionNoIn_return);
		JavaScriptParser.assignmentExpressionNoIn_return tV = default(JavaScriptParser.assignmentExpressionNoIn_return);
		JavaScriptParser.assignmentExpressionNoIn_return fV = default(JavaScriptParser.assignmentExpressionNoIn_return);
		JavaScriptParser.logicalORExpressionNoIn_return logicalORExpressionNoIn292 = default(JavaScriptParser.logicalORExpressionNoIn_return);

		CommonTree SkipSpace286_tree=null;
		CommonTree char_literal287_tree=null;
		CommonTree SkipSpace288_tree=null;
		CommonTree SkipSpace289_tree=null;
		CommonTree char_literal290_tree=null;
		CommonTree SkipSpace291_tree=null;
		RewriteRuleITokenStream stream_SkipSpace=new RewriteRuleITokenStream(adaptor,"token SkipSpace");
		RewriteRuleITokenStream stream_134=new RewriteRuleITokenStream(adaptor,"token 134");
		RewriteRuleITokenStream stream_133=new RewriteRuleITokenStream(adaptor,"token 133");
		RewriteRuleSubtreeStream stream_logicalORExpressionNoIn=new RewriteRuleSubtreeStream(adaptor,"rule logicalORExpressionNoIn");
		RewriteRuleSubtreeStream stream_assignmentExpressionNoIn=new RewriteRuleSubtreeStream(adaptor,"rule assignmentExpressionNoIn");
		try { DebugEnterRule(GrammarFileName, "conditionalExpressionNoIn");
		DebugLocation(369, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 58)) { return retval; }
			// JavaScript.g3:370:5: (cond= logicalORExpressionNoIn ( SkipSpace )* '?' ( SkipSpace )* tV= assignmentExpressionNoIn ( SkipSpace )* ':' ( SkipSpace )* fV= assignmentExpressionNoIn -> ^( CONDITIONAL_EXPR $cond $tV $fV) | logicalORExpressionNoIn )
			int alt168=2;
			try { DebugEnterDecision(168, decisionCanBacktrack[168]);
			try
			{
				alt168 = dfa168.Predict(input);
			}
			catch (NoViableAltException nvae)
			{
				DebugRecognitionException(nvae);
				throw;
			}
			} finally { DebugExitDecision(168); }
			switch (alt168)
			{
			case 1:
				DebugEnterAlt(1);
				// JavaScript.g3:370:7: cond= logicalORExpressionNoIn ( SkipSpace )* '?' ( SkipSpace )* tV= assignmentExpressionNoIn ( SkipSpace )* ':' ( SkipSpace )* fV= assignmentExpressionNoIn
				{
				DebugLocation(370, 11);
				PushFollow(Follow._logicalORExpressionNoIn_in_conditionalExpressionNoIn3109);
				cond=logicalORExpressionNoIn();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_logicalORExpressionNoIn.Add(cond.Tree);
				DebugLocation(370, 36);
				// JavaScript.g3:370:36: ( SkipSpace )*
				try { DebugEnterSubRule(164);
				while (true)
				{
					int alt164=2;
					try { DebugEnterDecision(164, decisionCanBacktrack[164]);
					int LA164_0 = input.LA(1);

					if ((LA164_0==SkipSpace))
					{
						alt164=1;
					}


					} finally { DebugExitDecision(164); }
					switch ( alt164 )
					{
					case 1:
						DebugEnterAlt(1);
						// JavaScript.g3:370:36: SkipSpace
						{
						DebugLocation(370, 36);
						SkipSpace286=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_conditionalExpressionNoIn3111); if (state.failed) return retval; 
						if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace286);


						}
						break;

					default:
						goto loop164;
					}
				}

				loop164:
					;

				} finally { DebugExitSubRule(164); }

				DebugLocation(370, 47);
				char_literal287=(CommonToken)Match(input,134,Follow._134_in_conditionalExpressionNoIn3114); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_134.Add(char_literal287);

				DebugLocation(370, 51);
				// JavaScript.g3:370:51: ( SkipSpace )*
				try { DebugEnterSubRule(165);
				while (true)
				{
					int alt165=2;
					try { DebugEnterDecision(165, decisionCanBacktrack[165]);
					int LA165_0 = input.LA(1);

					if ((LA165_0==SkipSpace))
					{
						alt165=1;
					}


					} finally { DebugExitDecision(165); }
					switch ( alt165 )
					{
					case 1:
						DebugEnterAlt(1);
						// JavaScript.g3:370:51: SkipSpace
						{
						DebugLocation(370, 51);
						SkipSpace288=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_conditionalExpressionNoIn3116); if (state.failed) return retval; 
						if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace288);


						}
						break;

					default:
						goto loop165;
					}
				}

				loop165:
					;

				} finally { DebugExitSubRule(165); }

				DebugLocation(370, 64);
				PushFollow(Follow._assignmentExpressionNoIn_in_conditionalExpressionNoIn3121);
				tV=assignmentExpressionNoIn();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_assignmentExpressionNoIn.Add(tV.Tree);
				DebugLocation(370, 90);
				// JavaScript.g3:370:90: ( SkipSpace )*
				try { DebugEnterSubRule(166);
				while (true)
				{
					int alt166=2;
					try { DebugEnterDecision(166, decisionCanBacktrack[166]);
					int LA166_0 = input.LA(1);

					if ((LA166_0==SkipSpace))
					{
						alt166=1;
					}


					} finally { DebugExitDecision(166); }
					switch ( alt166 )
					{
					case 1:
						DebugEnterAlt(1);
						// JavaScript.g3:370:90: SkipSpace
						{
						DebugLocation(370, 90);
						SkipSpace289=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_conditionalExpressionNoIn3123); if (state.failed) return retval; 
						if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace289);


						}
						break;

					default:
						goto loop166;
					}
				}

				loop166:
					;

				} finally { DebugExitSubRule(166); }

				DebugLocation(370, 101);
				char_literal290=(CommonToken)Match(input,133,Follow._133_in_conditionalExpressionNoIn3126); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_133.Add(char_literal290);

				DebugLocation(370, 105);
				// JavaScript.g3:370:105: ( SkipSpace )*
				try { DebugEnterSubRule(167);
				while (true)
				{
					int alt167=2;
					try { DebugEnterDecision(167, decisionCanBacktrack[167]);
					int LA167_0 = input.LA(1);

					if ((LA167_0==SkipSpace))
					{
						alt167=1;
					}


					} finally { DebugExitDecision(167); }
					switch ( alt167 )
					{
					case 1:
						DebugEnterAlt(1);
						// JavaScript.g3:370:105: SkipSpace
						{
						DebugLocation(370, 105);
						SkipSpace291=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_conditionalExpressionNoIn3128); if (state.failed) return retval; 
						if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace291);


						}
						break;

					default:
						goto loop167;
					}
				}

				loop167:
					;

				} finally { DebugExitSubRule(167); }

				DebugLocation(370, 118);
				PushFollow(Follow._assignmentExpressionNoIn_in_conditionalExpressionNoIn3133);
				fV=assignmentExpressionNoIn();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_assignmentExpressionNoIn.Add(fV.Tree);


				{
				// AST REWRITE
				// elements: cond, tV, fV
				// token labels: 
				// rule labels: cond, tV, fV, retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.Tree = root_0;
				RewriteRuleSubtreeStream stream_cond=new RewriteRuleSubtreeStream(adaptor,"rule cond",cond!=null?cond.Tree:null);
				RewriteRuleSubtreeStream stream_tV=new RewriteRuleSubtreeStream(adaptor,"rule tV",tV!=null?tV.Tree:null);
				RewriteRuleSubtreeStream stream_fV=new RewriteRuleSubtreeStream(adaptor,"rule fV",fV!=null?fV.Tree:null);
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

				root_0 = (CommonTree)adaptor.Nil();
				// 371:9: -> ^( CONDITIONAL_EXPR $cond $tV $fV)
				{
					DebugLocation(371, 12);
					// JavaScript.g3:371:12: ^( CONDITIONAL_EXPR $cond $tV $fV)
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(371, 14);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(CONDITIONAL_EXPR, "CONDITIONAL_EXPR"), root_1);

					DebugLocation(371, 32);
					adaptor.AddChild(root_1, stream_cond.NextTree());
					DebugLocation(371, 38);
					adaptor.AddChild(root_1, stream_tV.NextTree());
					DebugLocation(371, 42);
					adaptor.AddChild(root_1, stream_fV.NextTree());

					adaptor.AddChild(root_0, root_1);
					}

				}

				retval.Tree = root_0;
				}
				}

				}
				break;
			case 2:
				DebugEnterAlt(2);
				// JavaScript.g3:372:7: logicalORExpressionNoIn
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(372, 7);
				PushFollow(Follow._logicalORExpressionNoIn_in_conditionalExpressionNoIn3164);
				logicalORExpressionNoIn292=logicalORExpressionNoIn();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, logicalORExpressionNoIn292.Tree);

				}
				break;

			}
			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("conditionalExpressionNoIn", 58);
			LeaveRule("conditionalExpressionNoIn", 58);
			Leave_conditionalExpressionNoIn();
			if (state.backtracking > 0) { Memoize(input, 58, conditionalExpressionNoIn_StartIndex); }
		}
		DebugLocation(373, 4);
		} finally { DebugExitRule(GrammarFileName, "conditionalExpressionNoIn"); }
		return retval;

	}
	// $ANTLR end "conditionalExpressionNoIn"

	public class logicalORExpression_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_logicalORExpression();
	partial void Leave_logicalORExpression();

	// $ANTLR start "logicalORExpression"
	// JavaScript.g3:375:1: logicalORExpression : (lhs= logicalANDExpression -> $lhs) ( ( SkipSpace )* op= OR ( SkipSpace )* rhs= logicalANDExpression -> ^( BINARY_EXPR $op $logicalORExpression $rhs) )* ;
	[GrammarRule("logicalORExpression")]
	private JavaScriptParser.logicalORExpression_return logicalORExpression()
	{
		Enter_logicalORExpression();
		EnterRule("logicalORExpression", 59);
		TraceIn("logicalORExpression", 59);
		JavaScriptParser.logicalORExpression_return retval = new JavaScriptParser.logicalORExpression_return();
		retval.Start = (CommonToken)input.LT(1);
		int logicalORExpression_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken op=null;
		CommonToken SkipSpace293=null;
		CommonToken SkipSpace294=null;
		JavaScriptParser.logicalANDExpression_return lhs = default(JavaScriptParser.logicalANDExpression_return);
		JavaScriptParser.logicalANDExpression_return rhs = default(JavaScriptParser.logicalANDExpression_return);

		CommonTree op_tree=null;
		CommonTree SkipSpace293_tree=null;
		CommonTree SkipSpace294_tree=null;
		RewriteRuleITokenStream stream_SkipSpace=new RewriteRuleITokenStream(adaptor,"token SkipSpace");
		RewriteRuleITokenStream stream_OR=new RewriteRuleITokenStream(adaptor,"token OR");
		RewriteRuleSubtreeStream stream_logicalANDExpression=new RewriteRuleSubtreeStream(adaptor,"rule logicalANDExpression");
		try { DebugEnterRule(GrammarFileName, "logicalORExpression");
		DebugLocation(375, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 59)) { return retval; }
			// JavaScript.g3:376:5: ( (lhs= logicalANDExpression -> $lhs) ( ( SkipSpace )* op= OR ( SkipSpace )* rhs= logicalANDExpression -> ^( BINARY_EXPR $op $logicalORExpression $rhs) )* )
			DebugEnterAlt(1);
			// JavaScript.g3:376:7: (lhs= logicalANDExpression -> $lhs) ( ( SkipSpace )* op= OR ( SkipSpace )* rhs= logicalANDExpression -> ^( BINARY_EXPR $op $logicalORExpression $rhs) )*
			{
			DebugLocation(376, 7);
			// JavaScript.g3:376:7: (lhs= logicalANDExpression -> $lhs)
			DebugEnterAlt(1);
			// JavaScript.g3:376:8: lhs= logicalANDExpression
			{
			DebugLocation(376, 11);
			PushFollow(Follow._logicalANDExpression_in_logicalORExpression3184);
			lhs=logicalANDExpression();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_logicalANDExpression.Add(lhs.Tree);


			{
			// AST REWRITE
			// elements: lhs
			// token labels: 
			// rule labels: lhs, retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_lhs=new RewriteRuleSubtreeStream(adaptor,"rule lhs",lhs!=null?lhs.Tree:null);
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 376:33: -> $lhs
			{
				DebugLocation(376, 37);
				adaptor.AddChild(root_0, stream_lhs.NextTree());

			}

			retval.Tree = root_0;
			}
			}

			}

			DebugLocation(377, 9);
			// JavaScript.g3:377:9: ( ( SkipSpace )* op= OR ( SkipSpace )* rhs= logicalANDExpression -> ^( BINARY_EXPR $op $logicalORExpression $rhs) )*
			try { DebugEnterSubRule(171);
			while (true)
			{
				int alt171=2;
				try { DebugEnterDecision(171, decisionCanBacktrack[171]);
				try
				{
					alt171 = dfa171.Predict(input);
				}
				catch (NoViableAltException nvae)
				{
					DebugRecognitionException(nvae);
					throw;
				}
				} finally { DebugExitDecision(171); }
				switch ( alt171 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:377:10: ( SkipSpace )* op= OR ( SkipSpace )* rhs= logicalANDExpression
					{
					DebugLocation(377, 10);
					// JavaScript.g3:377:10: ( SkipSpace )*
					try { DebugEnterSubRule(169);
					while (true)
					{
						int alt169=2;
						try { DebugEnterDecision(169, decisionCanBacktrack[169]);
						int LA169_0 = input.LA(1);

						if ((LA169_0==SkipSpace))
						{
							alt169=1;
						}


						} finally { DebugExitDecision(169); }
						switch ( alt169 )
						{
						case 1:
							DebugEnterAlt(1);
							// JavaScript.g3:377:10: SkipSpace
							{
							DebugLocation(377, 10);
							SkipSpace293=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_logicalORExpression3201); if (state.failed) return retval; 
							if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace293);


							}
							break;

						default:
							goto loop169;
						}
					}

					loop169:
						;

					} finally { DebugExitSubRule(169); }

					DebugLocation(377, 23);
					op=(CommonToken)Match(input,OR,Follow._OR_in_logicalORExpression3206); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_OR.Add(op);

					DebugLocation(377, 27);
					// JavaScript.g3:377:27: ( SkipSpace )*
					try { DebugEnterSubRule(170);
					while (true)
					{
						int alt170=2;
						try { DebugEnterDecision(170, decisionCanBacktrack[170]);
						int LA170_0 = input.LA(1);

						if ((LA170_0==SkipSpace))
						{
							alt170=1;
						}


						} finally { DebugExitDecision(170); }
						switch ( alt170 )
						{
						case 1:
							DebugEnterAlt(1);
							// JavaScript.g3:377:27: SkipSpace
							{
							DebugLocation(377, 27);
							SkipSpace294=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_logicalORExpression3208); if (state.failed) return retval; 
							if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace294);


							}
							break;

						default:
							goto loop170;
						}
					}

					loop170:
						;

					} finally { DebugExitSubRule(170); }

					DebugLocation(377, 41);
					PushFollow(Follow._logicalANDExpression_in_logicalORExpression3213);
					rhs=logicalANDExpression();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_logicalANDExpression.Add(rhs.Tree);


					{
					// AST REWRITE
					// elements: op, logicalORExpression, rhs
					// token labels: op
					// rule labels: rhs, retval
					// token list labels: 
					// rule list labels: 
					// wildcard labels: 
					if ( state.backtracking == 0 ) {
					retval.Tree = root_0;
					RewriteRuleITokenStream stream_op=new RewriteRuleITokenStream(adaptor,"token op",op);
					RewriteRuleSubtreeStream stream_rhs=new RewriteRuleSubtreeStream(adaptor,"rule rhs",rhs!=null?rhs.Tree:null);
					RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

					root_0 = (CommonTree)adaptor.Nil();
					// 377:63: -> ^( BINARY_EXPR $op $logicalORExpression $rhs)
					{
						DebugLocation(377, 66);
						// JavaScript.g3:377:66: ^( BINARY_EXPR $op $logicalORExpression $rhs)
						{
						CommonTree root_1 = (CommonTree)adaptor.Nil();
						DebugLocation(377, 68);
						root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(BINARY_EXPR, "BINARY_EXPR"), root_1);

						DebugLocation(377, 81);
						adaptor.AddChild(root_1, stream_op.NextNode());
						DebugLocation(377, 85);
						adaptor.AddChild(root_1, stream_retval.NextTree());
						DebugLocation(377, 106);
						adaptor.AddChild(root_1, stream_rhs.NextTree());

						adaptor.AddChild(root_0, root_1);
						}

					}

					retval.Tree = root_0;
					}
					}

					}
					break;

				default:
					goto loop171;
				}
			}

			loop171:
				;

			} finally { DebugExitSubRule(171); }


			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("logicalORExpression", 59);
			LeaveRule("logicalORExpression", 59);
			Leave_logicalORExpression();
			if (state.backtracking > 0) { Memoize(input, 59, logicalORExpression_StartIndex); }
		}
		DebugLocation(378, 4);
		} finally { DebugExitRule(GrammarFileName, "logicalORExpression"); }
		return retval;

	}
	// $ANTLR end "logicalORExpression"

	public class logicalORExpressionNoIn_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_logicalORExpressionNoIn();
	partial void Leave_logicalORExpressionNoIn();

	// $ANTLR start "logicalORExpressionNoIn"
	// JavaScript.g3:380:1: logicalORExpressionNoIn : (lhs= logicalANDExpressionNoIn -> $lhs) ( ( SkipSpace )* op= OR ( SkipSpace )* rhs= logicalANDExpressionNoIn -> ^( BINARY_EXPR $op $logicalORExpressionNoIn $rhs) )* ;
	[GrammarRule("logicalORExpressionNoIn")]
	private JavaScriptParser.logicalORExpressionNoIn_return logicalORExpressionNoIn()
	{
		Enter_logicalORExpressionNoIn();
		EnterRule("logicalORExpressionNoIn", 60);
		TraceIn("logicalORExpressionNoIn", 60);
		JavaScriptParser.logicalORExpressionNoIn_return retval = new JavaScriptParser.logicalORExpressionNoIn_return();
		retval.Start = (CommonToken)input.LT(1);
		int logicalORExpressionNoIn_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken op=null;
		CommonToken SkipSpace295=null;
		CommonToken SkipSpace296=null;
		JavaScriptParser.logicalANDExpressionNoIn_return lhs = default(JavaScriptParser.logicalANDExpressionNoIn_return);
		JavaScriptParser.logicalANDExpressionNoIn_return rhs = default(JavaScriptParser.logicalANDExpressionNoIn_return);

		CommonTree op_tree=null;
		CommonTree SkipSpace295_tree=null;
		CommonTree SkipSpace296_tree=null;
		RewriteRuleITokenStream stream_SkipSpace=new RewriteRuleITokenStream(adaptor,"token SkipSpace");
		RewriteRuleITokenStream stream_OR=new RewriteRuleITokenStream(adaptor,"token OR");
		RewriteRuleSubtreeStream stream_logicalANDExpressionNoIn=new RewriteRuleSubtreeStream(adaptor,"rule logicalANDExpressionNoIn");
		try { DebugEnterRule(GrammarFileName, "logicalORExpressionNoIn");
		DebugLocation(380, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 60)) { return retval; }
			// JavaScript.g3:381:5: ( (lhs= logicalANDExpressionNoIn -> $lhs) ( ( SkipSpace )* op= OR ( SkipSpace )* rhs= logicalANDExpressionNoIn -> ^( BINARY_EXPR $op $logicalORExpressionNoIn $rhs) )* )
			DebugEnterAlt(1);
			// JavaScript.g3:381:7: (lhs= logicalANDExpressionNoIn -> $lhs) ( ( SkipSpace )* op= OR ( SkipSpace )* rhs= logicalANDExpressionNoIn -> ^( BINARY_EXPR $op $logicalORExpressionNoIn $rhs) )*
			{
			DebugLocation(381, 7);
			// JavaScript.g3:381:7: (lhs= logicalANDExpressionNoIn -> $lhs)
			DebugEnterAlt(1);
			// JavaScript.g3:381:8: lhs= logicalANDExpressionNoIn
			{
			DebugLocation(381, 11);
			PushFollow(Follow._logicalANDExpressionNoIn_in_logicalORExpressionNoIn3251);
			lhs=logicalANDExpressionNoIn();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_logicalANDExpressionNoIn.Add(lhs.Tree);


			{
			// AST REWRITE
			// elements: lhs
			// token labels: 
			// rule labels: lhs, retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_lhs=new RewriteRuleSubtreeStream(adaptor,"rule lhs",lhs!=null?lhs.Tree:null);
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 381:37: -> $lhs
			{
				DebugLocation(381, 41);
				adaptor.AddChild(root_0, stream_lhs.NextTree());

			}

			retval.Tree = root_0;
			}
			}

			}

			DebugLocation(382, 9);
			// JavaScript.g3:382:9: ( ( SkipSpace )* op= OR ( SkipSpace )* rhs= logicalANDExpressionNoIn -> ^( BINARY_EXPR $op $logicalORExpressionNoIn $rhs) )*
			try { DebugEnterSubRule(174);
			while (true)
			{
				int alt174=2;
				try { DebugEnterDecision(174, decisionCanBacktrack[174]);
				try
				{
					alt174 = dfa174.Predict(input);
				}
				catch (NoViableAltException nvae)
				{
					DebugRecognitionException(nvae);
					throw;
				}
				} finally { DebugExitDecision(174); }
				switch ( alt174 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:382:10: ( SkipSpace )* op= OR ( SkipSpace )* rhs= logicalANDExpressionNoIn
					{
					DebugLocation(382, 10);
					// JavaScript.g3:382:10: ( SkipSpace )*
					try { DebugEnterSubRule(172);
					while (true)
					{
						int alt172=2;
						try { DebugEnterDecision(172, decisionCanBacktrack[172]);
						int LA172_0 = input.LA(1);

						if ((LA172_0==SkipSpace))
						{
							alt172=1;
						}


						} finally { DebugExitDecision(172); }
						switch ( alt172 )
						{
						case 1:
							DebugEnterAlt(1);
							// JavaScript.g3:382:10: SkipSpace
							{
							DebugLocation(382, 10);
							SkipSpace295=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_logicalORExpressionNoIn3268); if (state.failed) return retval; 
							if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace295);


							}
							break;

						default:
							goto loop172;
						}
					}

					loop172:
						;

					} finally { DebugExitSubRule(172); }

					DebugLocation(382, 23);
					op=(CommonToken)Match(input,OR,Follow._OR_in_logicalORExpressionNoIn3273); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_OR.Add(op);

					DebugLocation(382, 27);
					// JavaScript.g3:382:27: ( SkipSpace )*
					try { DebugEnterSubRule(173);
					while (true)
					{
						int alt173=2;
						try { DebugEnterDecision(173, decisionCanBacktrack[173]);
						int LA173_0 = input.LA(1);

						if ((LA173_0==SkipSpace))
						{
							alt173=1;
						}


						} finally { DebugExitDecision(173); }
						switch ( alt173 )
						{
						case 1:
							DebugEnterAlt(1);
							// JavaScript.g3:382:27: SkipSpace
							{
							DebugLocation(382, 27);
							SkipSpace296=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_logicalORExpressionNoIn3275); if (state.failed) return retval; 
							if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace296);


							}
							break;

						default:
							goto loop173;
						}
					}

					loop173:
						;

					} finally { DebugExitSubRule(173); }

					DebugLocation(382, 41);
					PushFollow(Follow._logicalANDExpressionNoIn_in_logicalORExpressionNoIn3280);
					rhs=logicalANDExpressionNoIn();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_logicalANDExpressionNoIn.Add(rhs.Tree);


					{
					// AST REWRITE
					// elements: op, logicalORExpressionNoIn, rhs
					// token labels: op
					// rule labels: rhs, retval
					// token list labels: 
					// rule list labels: 
					// wildcard labels: 
					if ( state.backtracking == 0 ) {
					retval.Tree = root_0;
					RewriteRuleITokenStream stream_op=new RewriteRuleITokenStream(adaptor,"token op",op);
					RewriteRuleSubtreeStream stream_rhs=new RewriteRuleSubtreeStream(adaptor,"rule rhs",rhs!=null?rhs.Tree:null);
					RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

					root_0 = (CommonTree)adaptor.Nil();
					// 382:67: -> ^( BINARY_EXPR $op $logicalORExpressionNoIn $rhs)
					{
						DebugLocation(382, 70);
						// JavaScript.g3:382:70: ^( BINARY_EXPR $op $logicalORExpressionNoIn $rhs)
						{
						CommonTree root_1 = (CommonTree)adaptor.Nil();
						DebugLocation(382, 72);
						root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(BINARY_EXPR, "BINARY_EXPR"), root_1);

						DebugLocation(382, 85);
						adaptor.AddChild(root_1, stream_op.NextNode());
						DebugLocation(382, 89);
						adaptor.AddChild(root_1, stream_retval.NextTree());
						DebugLocation(382, 114);
						adaptor.AddChild(root_1, stream_rhs.NextTree());

						adaptor.AddChild(root_0, root_1);
						}

					}

					retval.Tree = root_0;
					}
					}

					}
					break;

				default:
					goto loop174;
				}
			}

			loop174:
				;

			} finally { DebugExitSubRule(174); }


			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("logicalORExpressionNoIn", 60);
			LeaveRule("logicalORExpressionNoIn", 60);
			Leave_logicalORExpressionNoIn();
			if (state.backtracking > 0) { Memoize(input, 60, logicalORExpressionNoIn_StartIndex); }
		}
		DebugLocation(383, 4);
		} finally { DebugExitRule(GrammarFileName, "logicalORExpressionNoIn"); }
		return retval;

	}
	// $ANTLR end "logicalORExpressionNoIn"

	public class logicalANDExpression_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_logicalANDExpression();
	partial void Leave_logicalANDExpression();

	// $ANTLR start "logicalANDExpression"
	// JavaScript.g3:385:1: logicalANDExpression : (lhs= bitwiseORExpression -> $lhs) ( ( SkipSpace )* op= AND ( SkipSpace )* rhs= bitwiseORExpression -> ^( BINARY_EXPR $op $logicalANDExpression $rhs) )* ;
	[GrammarRule("logicalANDExpression")]
	private JavaScriptParser.logicalANDExpression_return logicalANDExpression()
	{
		Enter_logicalANDExpression();
		EnterRule("logicalANDExpression", 61);
		TraceIn("logicalANDExpression", 61);
		JavaScriptParser.logicalANDExpression_return retval = new JavaScriptParser.logicalANDExpression_return();
		retval.Start = (CommonToken)input.LT(1);
		int logicalANDExpression_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken op=null;
		CommonToken SkipSpace297=null;
		CommonToken SkipSpace298=null;
		JavaScriptParser.bitwiseORExpression_return lhs = default(JavaScriptParser.bitwiseORExpression_return);
		JavaScriptParser.bitwiseORExpression_return rhs = default(JavaScriptParser.bitwiseORExpression_return);

		CommonTree op_tree=null;
		CommonTree SkipSpace297_tree=null;
		CommonTree SkipSpace298_tree=null;
		RewriteRuleITokenStream stream_SkipSpace=new RewriteRuleITokenStream(adaptor,"token SkipSpace");
		RewriteRuleITokenStream stream_AND=new RewriteRuleITokenStream(adaptor,"token AND");
		RewriteRuleSubtreeStream stream_bitwiseORExpression=new RewriteRuleSubtreeStream(adaptor,"rule bitwiseORExpression");
		try { DebugEnterRule(GrammarFileName, "logicalANDExpression");
		DebugLocation(385, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 61)) { return retval; }
			// JavaScript.g3:386:5: ( (lhs= bitwiseORExpression -> $lhs) ( ( SkipSpace )* op= AND ( SkipSpace )* rhs= bitwiseORExpression -> ^( BINARY_EXPR $op $logicalANDExpression $rhs) )* )
			DebugEnterAlt(1);
			// JavaScript.g3:386:7: (lhs= bitwiseORExpression -> $lhs) ( ( SkipSpace )* op= AND ( SkipSpace )* rhs= bitwiseORExpression -> ^( BINARY_EXPR $op $logicalANDExpression $rhs) )*
			{
			DebugLocation(386, 7);
			// JavaScript.g3:386:7: (lhs= bitwiseORExpression -> $lhs)
			DebugEnterAlt(1);
			// JavaScript.g3:386:8: lhs= bitwiseORExpression
			{
			DebugLocation(386, 11);
			PushFollow(Follow._bitwiseORExpression_in_logicalANDExpression3318);
			lhs=bitwiseORExpression();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_bitwiseORExpression.Add(lhs.Tree);


			{
			// AST REWRITE
			// elements: lhs
			// token labels: 
			// rule labels: lhs, retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_lhs=new RewriteRuleSubtreeStream(adaptor,"rule lhs",lhs!=null?lhs.Tree:null);
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 386:32: -> $lhs
			{
				DebugLocation(386, 36);
				adaptor.AddChild(root_0, stream_lhs.NextTree());

			}

			retval.Tree = root_0;
			}
			}

			}

			DebugLocation(387, 9);
			// JavaScript.g3:387:9: ( ( SkipSpace )* op= AND ( SkipSpace )* rhs= bitwiseORExpression -> ^( BINARY_EXPR $op $logicalANDExpression $rhs) )*
			try { DebugEnterSubRule(177);
			while (true)
			{
				int alt177=2;
				try { DebugEnterDecision(177, decisionCanBacktrack[177]);
				try
				{
					alt177 = dfa177.Predict(input);
				}
				catch (NoViableAltException nvae)
				{
					DebugRecognitionException(nvae);
					throw;
				}
				} finally { DebugExitDecision(177); }
				switch ( alt177 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:387:10: ( SkipSpace )* op= AND ( SkipSpace )* rhs= bitwiseORExpression
					{
					DebugLocation(387, 10);
					// JavaScript.g3:387:10: ( SkipSpace )*
					try { DebugEnterSubRule(175);
					while (true)
					{
						int alt175=2;
						try { DebugEnterDecision(175, decisionCanBacktrack[175]);
						int LA175_0 = input.LA(1);

						if ((LA175_0==SkipSpace))
						{
							alt175=1;
						}


						} finally { DebugExitDecision(175); }
						switch ( alt175 )
						{
						case 1:
							DebugEnterAlt(1);
							// JavaScript.g3:387:10: SkipSpace
							{
							DebugLocation(387, 10);
							SkipSpace297=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_logicalANDExpression3335); if (state.failed) return retval; 
							if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace297);


							}
							break;

						default:
							goto loop175;
						}
					}

					loop175:
						;

					} finally { DebugExitSubRule(175); }

					DebugLocation(387, 23);
					op=(CommonToken)Match(input,AND,Follow._AND_in_logicalANDExpression3340); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_AND.Add(op);

					DebugLocation(387, 28);
					// JavaScript.g3:387:28: ( SkipSpace )*
					try { DebugEnterSubRule(176);
					while (true)
					{
						int alt176=2;
						try { DebugEnterDecision(176, decisionCanBacktrack[176]);
						int LA176_0 = input.LA(1);

						if ((LA176_0==SkipSpace))
						{
							alt176=1;
						}


						} finally { DebugExitDecision(176); }
						switch ( alt176 )
						{
						case 1:
							DebugEnterAlt(1);
							// JavaScript.g3:387:28: SkipSpace
							{
							DebugLocation(387, 28);
							SkipSpace298=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_logicalANDExpression3342); if (state.failed) return retval; 
							if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace298);


							}
							break;

						default:
							goto loop176;
						}
					}

					loop176:
						;

					} finally { DebugExitSubRule(176); }

					DebugLocation(387, 42);
					PushFollow(Follow._bitwiseORExpression_in_logicalANDExpression3347);
					rhs=bitwiseORExpression();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_bitwiseORExpression.Add(rhs.Tree);


					{
					// AST REWRITE
					// elements: op, logicalANDExpression, rhs
					// token labels: op
					// rule labels: rhs, retval
					// token list labels: 
					// rule list labels: 
					// wildcard labels: 
					if ( state.backtracking == 0 ) {
					retval.Tree = root_0;
					RewriteRuleITokenStream stream_op=new RewriteRuleITokenStream(adaptor,"token op",op);
					RewriteRuleSubtreeStream stream_rhs=new RewriteRuleSubtreeStream(adaptor,"rule rhs",rhs!=null?rhs.Tree:null);
					RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

					root_0 = (CommonTree)adaptor.Nil();
					// 387:63: -> ^( BINARY_EXPR $op $logicalANDExpression $rhs)
					{
						DebugLocation(387, 66);
						// JavaScript.g3:387:66: ^( BINARY_EXPR $op $logicalANDExpression $rhs)
						{
						CommonTree root_1 = (CommonTree)adaptor.Nil();
						DebugLocation(387, 68);
						root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(BINARY_EXPR, "BINARY_EXPR"), root_1);

						DebugLocation(387, 81);
						adaptor.AddChild(root_1, stream_op.NextNode());
						DebugLocation(387, 85);
						adaptor.AddChild(root_1, stream_retval.NextTree());
						DebugLocation(387, 107);
						adaptor.AddChild(root_1, stream_rhs.NextTree());

						adaptor.AddChild(root_0, root_1);
						}

					}

					retval.Tree = root_0;
					}
					}

					}
					break;

				default:
					goto loop177;
				}
			}

			loop177:
				;

			} finally { DebugExitSubRule(177); }


			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("logicalANDExpression", 61);
			LeaveRule("logicalANDExpression", 61);
			Leave_logicalANDExpression();
			if (state.backtracking > 0) { Memoize(input, 61, logicalANDExpression_StartIndex); }
		}
		DebugLocation(388, 4);
		} finally { DebugExitRule(GrammarFileName, "logicalANDExpression"); }
		return retval;

	}
	// $ANTLR end "logicalANDExpression"

	public class logicalANDExpressionNoIn_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_logicalANDExpressionNoIn();
	partial void Leave_logicalANDExpressionNoIn();

	// $ANTLR start "logicalANDExpressionNoIn"
	// JavaScript.g3:390:1: logicalANDExpressionNoIn : (lhs= bitwiseORExpressionNoIn -> $lhs) ( ( SkipSpace )* op= AND ( SkipSpace )* rhs= bitwiseORExpressionNoIn -> ^( BINARY_EXPR $op $logicalANDExpressionNoIn $rhs) )* ;
	[GrammarRule("logicalANDExpressionNoIn")]
	private JavaScriptParser.logicalANDExpressionNoIn_return logicalANDExpressionNoIn()
	{
		Enter_logicalANDExpressionNoIn();
		EnterRule("logicalANDExpressionNoIn", 62);
		TraceIn("logicalANDExpressionNoIn", 62);
		JavaScriptParser.logicalANDExpressionNoIn_return retval = new JavaScriptParser.logicalANDExpressionNoIn_return();
		retval.Start = (CommonToken)input.LT(1);
		int logicalANDExpressionNoIn_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken op=null;
		CommonToken SkipSpace299=null;
		CommonToken SkipSpace300=null;
		JavaScriptParser.bitwiseORExpressionNoIn_return lhs = default(JavaScriptParser.bitwiseORExpressionNoIn_return);
		JavaScriptParser.bitwiseORExpressionNoIn_return rhs = default(JavaScriptParser.bitwiseORExpressionNoIn_return);

		CommonTree op_tree=null;
		CommonTree SkipSpace299_tree=null;
		CommonTree SkipSpace300_tree=null;
		RewriteRuleITokenStream stream_SkipSpace=new RewriteRuleITokenStream(adaptor,"token SkipSpace");
		RewriteRuleITokenStream stream_AND=new RewriteRuleITokenStream(adaptor,"token AND");
		RewriteRuleSubtreeStream stream_bitwiseORExpressionNoIn=new RewriteRuleSubtreeStream(adaptor,"rule bitwiseORExpressionNoIn");
		try { DebugEnterRule(GrammarFileName, "logicalANDExpressionNoIn");
		DebugLocation(390, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 62)) { return retval; }
			// JavaScript.g3:391:5: ( (lhs= bitwiseORExpressionNoIn -> $lhs) ( ( SkipSpace )* op= AND ( SkipSpace )* rhs= bitwiseORExpressionNoIn -> ^( BINARY_EXPR $op $logicalANDExpressionNoIn $rhs) )* )
			DebugEnterAlt(1);
			// JavaScript.g3:391:7: (lhs= bitwiseORExpressionNoIn -> $lhs) ( ( SkipSpace )* op= AND ( SkipSpace )* rhs= bitwiseORExpressionNoIn -> ^( BINARY_EXPR $op $logicalANDExpressionNoIn $rhs) )*
			{
			DebugLocation(391, 7);
			// JavaScript.g3:391:7: (lhs= bitwiseORExpressionNoIn -> $lhs)
			DebugEnterAlt(1);
			// JavaScript.g3:391:8: lhs= bitwiseORExpressionNoIn
			{
			DebugLocation(391, 11);
			PushFollow(Follow._bitwiseORExpressionNoIn_in_logicalANDExpressionNoIn3385);
			lhs=bitwiseORExpressionNoIn();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_bitwiseORExpressionNoIn.Add(lhs.Tree);


			{
			// AST REWRITE
			// elements: lhs
			// token labels: 
			// rule labels: lhs, retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_lhs=new RewriteRuleSubtreeStream(adaptor,"rule lhs",lhs!=null?lhs.Tree:null);
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 391:36: -> $lhs
			{
				DebugLocation(391, 40);
				adaptor.AddChild(root_0, stream_lhs.NextTree());

			}

			retval.Tree = root_0;
			}
			}

			}

			DebugLocation(392, 9);
			// JavaScript.g3:392:9: ( ( SkipSpace )* op= AND ( SkipSpace )* rhs= bitwiseORExpressionNoIn -> ^( BINARY_EXPR $op $logicalANDExpressionNoIn $rhs) )*
			try { DebugEnterSubRule(180);
			while (true)
			{
				int alt180=2;
				try { DebugEnterDecision(180, decisionCanBacktrack[180]);
				try
				{
					alt180 = dfa180.Predict(input);
				}
				catch (NoViableAltException nvae)
				{
					DebugRecognitionException(nvae);
					throw;
				}
				} finally { DebugExitDecision(180); }
				switch ( alt180 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:392:10: ( SkipSpace )* op= AND ( SkipSpace )* rhs= bitwiseORExpressionNoIn
					{
					DebugLocation(392, 10);
					// JavaScript.g3:392:10: ( SkipSpace )*
					try { DebugEnterSubRule(178);
					while (true)
					{
						int alt178=2;
						try { DebugEnterDecision(178, decisionCanBacktrack[178]);
						int LA178_0 = input.LA(1);

						if ((LA178_0==SkipSpace))
						{
							alt178=1;
						}


						} finally { DebugExitDecision(178); }
						switch ( alt178 )
						{
						case 1:
							DebugEnterAlt(1);
							// JavaScript.g3:392:10: SkipSpace
							{
							DebugLocation(392, 10);
							SkipSpace299=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_logicalANDExpressionNoIn3402); if (state.failed) return retval; 
							if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace299);


							}
							break;

						default:
							goto loop178;
						}
					}

					loop178:
						;

					} finally { DebugExitSubRule(178); }

					DebugLocation(392, 23);
					op=(CommonToken)Match(input,AND,Follow._AND_in_logicalANDExpressionNoIn3407); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_AND.Add(op);

					DebugLocation(392, 28);
					// JavaScript.g3:392:28: ( SkipSpace )*
					try { DebugEnterSubRule(179);
					while (true)
					{
						int alt179=2;
						try { DebugEnterDecision(179, decisionCanBacktrack[179]);
						int LA179_0 = input.LA(1);

						if ((LA179_0==SkipSpace))
						{
							alt179=1;
						}


						} finally { DebugExitDecision(179); }
						switch ( alt179 )
						{
						case 1:
							DebugEnterAlt(1);
							// JavaScript.g3:392:28: SkipSpace
							{
							DebugLocation(392, 28);
							SkipSpace300=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_logicalANDExpressionNoIn3409); if (state.failed) return retval; 
							if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace300);


							}
							break;

						default:
							goto loop179;
						}
					}

					loop179:
						;

					} finally { DebugExitSubRule(179); }

					DebugLocation(392, 42);
					PushFollow(Follow._bitwiseORExpressionNoIn_in_logicalANDExpressionNoIn3414);
					rhs=bitwiseORExpressionNoIn();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_bitwiseORExpressionNoIn.Add(rhs.Tree);


					{
					// AST REWRITE
					// elements: op, logicalANDExpressionNoIn, rhs
					// token labels: op
					// rule labels: rhs, retval
					// token list labels: 
					// rule list labels: 
					// wildcard labels: 
					if ( state.backtracking == 0 ) {
					retval.Tree = root_0;
					RewriteRuleITokenStream stream_op=new RewriteRuleITokenStream(adaptor,"token op",op);
					RewriteRuleSubtreeStream stream_rhs=new RewriteRuleSubtreeStream(adaptor,"rule rhs",rhs!=null?rhs.Tree:null);
					RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

					root_0 = (CommonTree)adaptor.Nil();
					// 392:67: -> ^( BINARY_EXPR $op $logicalANDExpressionNoIn $rhs)
					{
						DebugLocation(392, 70);
						// JavaScript.g3:392:70: ^( BINARY_EXPR $op $logicalANDExpressionNoIn $rhs)
						{
						CommonTree root_1 = (CommonTree)adaptor.Nil();
						DebugLocation(392, 72);
						root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(BINARY_EXPR, "BINARY_EXPR"), root_1);

						DebugLocation(392, 85);
						adaptor.AddChild(root_1, stream_op.NextNode());
						DebugLocation(392, 89);
						adaptor.AddChild(root_1, stream_retval.NextTree());
						DebugLocation(392, 115);
						adaptor.AddChild(root_1, stream_rhs.NextTree());

						adaptor.AddChild(root_0, root_1);
						}

					}

					retval.Tree = root_0;
					}
					}

					}
					break;

				default:
					goto loop180;
				}
			}

			loop180:
				;

			} finally { DebugExitSubRule(180); }


			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("logicalANDExpressionNoIn", 62);
			LeaveRule("logicalANDExpressionNoIn", 62);
			Leave_logicalANDExpressionNoIn();
			if (state.backtracking > 0) { Memoize(input, 62, logicalANDExpressionNoIn_StartIndex); }
		}
		DebugLocation(393, 4);
		} finally { DebugExitRule(GrammarFileName, "logicalANDExpressionNoIn"); }
		return retval;

	}
	// $ANTLR end "logicalANDExpressionNoIn"

	public class bitwiseORExpression_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_bitwiseORExpression();
	partial void Leave_bitwiseORExpression();

	// $ANTLR start "bitwiseORExpression"
	// JavaScript.g3:395:1: bitwiseORExpression : (lhs= bitwiseXORExpression -> $lhs) ( ( SkipSpace )* op= BIT_OR ( SkipSpace )* rhs= bitwiseXORExpression -> ^( BINARY_EXPR $op $bitwiseORExpression $rhs) )* ;
	[GrammarRule("bitwiseORExpression")]
	private JavaScriptParser.bitwiseORExpression_return bitwiseORExpression()
	{
		Enter_bitwiseORExpression();
		EnterRule("bitwiseORExpression", 63);
		TraceIn("bitwiseORExpression", 63);
		JavaScriptParser.bitwiseORExpression_return retval = new JavaScriptParser.bitwiseORExpression_return();
		retval.Start = (CommonToken)input.LT(1);
		int bitwiseORExpression_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken op=null;
		CommonToken SkipSpace301=null;
		CommonToken SkipSpace302=null;
		JavaScriptParser.bitwiseXORExpression_return lhs = default(JavaScriptParser.bitwiseXORExpression_return);
		JavaScriptParser.bitwiseXORExpression_return rhs = default(JavaScriptParser.bitwiseXORExpression_return);

		CommonTree op_tree=null;
		CommonTree SkipSpace301_tree=null;
		CommonTree SkipSpace302_tree=null;
		RewriteRuleITokenStream stream_SkipSpace=new RewriteRuleITokenStream(adaptor,"token SkipSpace");
		RewriteRuleITokenStream stream_BIT_OR=new RewriteRuleITokenStream(adaptor,"token BIT_OR");
		RewriteRuleSubtreeStream stream_bitwiseXORExpression=new RewriteRuleSubtreeStream(adaptor,"rule bitwiseXORExpression");
		try { DebugEnterRule(GrammarFileName, "bitwiseORExpression");
		DebugLocation(395, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 63)) { return retval; }
			// JavaScript.g3:396:5: ( (lhs= bitwiseXORExpression -> $lhs) ( ( SkipSpace )* op= BIT_OR ( SkipSpace )* rhs= bitwiseXORExpression -> ^( BINARY_EXPR $op $bitwiseORExpression $rhs) )* )
			DebugEnterAlt(1);
			// JavaScript.g3:396:7: (lhs= bitwiseXORExpression -> $lhs) ( ( SkipSpace )* op= BIT_OR ( SkipSpace )* rhs= bitwiseXORExpression -> ^( BINARY_EXPR $op $bitwiseORExpression $rhs) )*
			{
			DebugLocation(396, 7);
			// JavaScript.g3:396:7: (lhs= bitwiseXORExpression -> $lhs)
			DebugEnterAlt(1);
			// JavaScript.g3:396:8: lhs= bitwiseXORExpression
			{
			DebugLocation(396, 11);
			PushFollow(Follow._bitwiseXORExpression_in_bitwiseORExpression3452);
			lhs=bitwiseXORExpression();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_bitwiseXORExpression.Add(lhs.Tree);


			{
			// AST REWRITE
			// elements: lhs
			// token labels: 
			// rule labels: lhs, retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_lhs=new RewriteRuleSubtreeStream(adaptor,"rule lhs",lhs!=null?lhs.Tree:null);
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 396:33: -> $lhs
			{
				DebugLocation(396, 37);
				adaptor.AddChild(root_0, stream_lhs.NextTree());

			}

			retval.Tree = root_0;
			}
			}

			}

			DebugLocation(397, 9);
			// JavaScript.g3:397:9: ( ( SkipSpace )* op= BIT_OR ( SkipSpace )* rhs= bitwiseXORExpression -> ^( BINARY_EXPR $op $bitwiseORExpression $rhs) )*
			try { DebugEnterSubRule(183);
			while (true)
			{
				int alt183=2;
				try { DebugEnterDecision(183, decisionCanBacktrack[183]);
				try
				{
					alt183 = dfa183.Predict(input);
				}
				catch (NoViableAltException nvae)
				{
					DebugRecognitionException(nvae);
					throw;
				}
				} finally { DebugExitDecision(183); }
				switch ( alt183 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:397:10: ( SkipSpace )* op= BIT_OR ( SkipSpace )* rhs= bitwiseXORExpression
					{
					DebugLocation(397, 10);
					// JavaScript.g3:397:10: ( SkipSpace )*
					try { DebugEnterSubRule(181);
					while (true)
					{
						int alt181=2;
						try { DebugEnterDecision(181, decisionCanBacktrack[181]);
						int LA181_0 = input.LA(1);

						if ((LA181_0==SkipSpace))
						{
							alt181=1;
						}


						} finally { DebugExitDecision(181); }
						switch ( alt181 )
						{
						case 1:
							DebugEnterAlt(1);
							// JavaScript.g3:397:10: SkipSpace
							{
							DebugLocation(397, 10);
							SkipSpace301=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_bitwiseORExpression3469); if (state.failed) return retval; 
							if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace301);


							}
							break;

						default:
							goto loop181;
						}
					}

					loop181:
						;

					} finally { DebugExitSubRule(181); }

					DebugLocation(397, 23);
					op=(CommonToken)Match(input,BIT_OR,Follow._BIT_OR_in_bitwiseORExpression3474); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_BIT_OR.Add(op);

					DebugLocation(397, 31);
					// JavaScript.g3:397:31: ( SkipSpace )*
					try { DebugEnterSubRule(182);
					while (true)
					{
						int alt182=2;
						try { DebugEnterDecision(182, decisionCanBacktrack[182]);
						int LA182_0 = input.LA(1);

						if ((LA182_0==SkipSpace))
						{
							alt182=1;
						}


						} finally { DebugExitDecision(182); }
						switch ( alt182 )
						{
						case 1:
							DebugEnterAlt(1);
							// JavaScript.g3:397:31: SkipSpace
							{
							DebugLocation(397, 31);
							SkipSpace302=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_bitwiseORExpression3476); if (state.failed) return retval; 
							if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace302);


							}
							break;

						default:
							goto loop182;
						}
					}

					loop182:
						;

					} finally { DebugExitSubRule(182); }

					DebugLocation(397, 45);
					PushFollow(Follow._bitwiseXORExpression_in_bitwiseORExpression3481);
					rhs=bitwiseXORExpression();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_bitwiseXORExpression.Add(rhs.Tree);


					{
					// AST REWRITE
					// elements: op, bitwiseORExpression, rhs
					// token labels: op
					// rule labels: rhs, retval
					// token list labels: 
					// rule list labels: 
					// wildcard labels: 
					if ( state.backtracking == 0 ) {
					retval.Tree = root_0;
					RewriteRuleITokenStream stream_op=new RewriteRuleITokenStream(adaptor,"token op",op);
					RewriteRuleSubtreeStream stream_rhs=new RewriteRuleSubtreeStream(adaptor,"rule rhs",rhs!=null?rhs.Tree:null);
					RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

					root_0 = (CommonTree)adaptor.Nil();
					// 397:67: -> ^( BINARY_EXPR $op $bitwiseORExpression $rhs)
					{
						DebugLocation(397, 70);
						// JavaScript.g3:397:70: ^( BINARY_EXPR $op $bitwiseORExpression $rhs)
						{
						CommonTree root_1 = (CommonTree)adaptor.Nil();
						DebugLocation(397, 72);
						root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(BINARY_EXPR, "BINARY_EXPR"), root_1);

						DebugLocation(397, 85);
						adaptor.AddChild(root_1, stream_op.NextNode());
						DebugLocation(397, 89);
						adaptor.AddChild(root_1, stream_retval.NextTree());
						DebugLocation(397, 110);
						adaptor.AddChild(root_1, stream_rhs.NextTree());

						adaptor.AddChild(root_0, root_1);
						}

					}

					retval.Tree = root_0;
					}
					}

					}
					break;

				default:
					goto loop183;
				}
			}

			loop183:
				;

			} finally { DebugExitSubRule(183); }


			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("bitwiseORExpression", 63);
			LeaveRule("bitwiseORExpression", 63);
			Leave_bitwiseORExpression();
			if (state.backtracking > 0) { Memoize(input, 63, bitwiseORExpression_StartIndex); }
		}
		DebugLocation(398, 4);
		} finally { DebugExitRule(GrammarFileName, "bitwiseORExpression"); }
		return retval;

	}
	// $ANTLR end "bitwiseORExpression"

	public class bitwiseORExpressionNoIn_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_bitwiseORExpressionNoIn();
	partial void Leave_bitwiseORExpressionNoIn();

	// $ANTLR start "bitwiseORExpressionNoIn"
	// JavaScript.g3:400:1: bitwiseORExpressionNoIn : (lhs= bitwiseXORExpressionNoIn -> $lhs) ( ( SkipSpace )* op= BIT_OR ( SkipSpace )* rhs= bitwiseXORExpressionNoIn -> ^( BINARY_EXPR $op $bitwiseORExpressionNoIn $rhs) )* ;
	[GrammarRule("bitwiseORExpressionNoIn")]
	private JavaScriptParser.bitwiseORExpressionNoIn_return bitwiseORExpressionNoIn()
	{
		Enter_bitwiseORExpressionNoIn();
		EnterRule("bitwiseORExpressionNoIn", 64);
		TraceIn("bitwiseORExpressionNoIn", 64);
		JavaScriptParser.bitwiseORExpressionNoIn_return retval = new JavaScriptParser.bitwiseORExpressionNoIn_return();
		retval.Start = (CommonToken)input.LT(1);
		int bitwiseORExpressionNoIn_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken op=null;
		CommonToken SkipSpace303=null;
		CommonToken SkipSpace304=null;
		JavaScriptParser.bitwiseXORExpressionNoIn_return lhs = default(JavaScriptParser.bitwiseXORExpressionNoIn_return);
		JavaScriptParser.bitwiseXORExpressionNoIn_return rhs = default(JavaScriptParser.bitwiseXORExpressionNoIn_return);

		CommonTree op_tree=null;
		CommonTree SkipSpace303_tree=null;
		CommonTree SkipSpace304_tree=null;
		RewriteRuleITokenStream stream_SkipSpace=new RewriteRuleITokenStream(adaptor,"token SkipSpace");
		RewriteRuleITokenStream stream_BIT_OR=new RewriteRuleITokenStream(adaptor,"token BIT_OR");
		RewriteRuleSubtreeStream stream_bitwiseXORExpressionNoIn=new RewriteRuleSubtreeStream(adaptor,"rule bitwiseXORExpressionNoIn");
		try { DebugEnterRule(GrammarFileName, "bitwiseORExpressionNoIn");
		DebugLocation(400, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 64)) { return retval; }
			// JavaScript.g3:401:5: ( (lhs= bitwiseXORExpressionNoIn -> $lhs) ( ( SkipSpace )* op= BIT_OR ( SkipSpace )* rhs= bitwiseXORExpressionNoIn -> ^( BINARY_EXPR $op $bitwiseORExpressionNoIn $rhs) )* )
			DebugEnterAlt(1);
			// JavaScript.g3:401:7: (lhs= bitwiseXORExpressionNoIn -> $lhs) ( ( SkipSpace )* op= BIT_OR ( SkipSpace )* rhs= bitwiseXORExpressionNoIn -> ^( BINARY_EXPR $op $bitwiseORExpressionNoIn $rhs) )*
			{
			DebugLocation(401, 7);
			// JavaScript.g3:401:7: (lhs= bitwiseXORExpressionNoIn -> $lhs)
			DebugEnterAlt(1);
			// JavaScript.g3:401:8: lhs= bitwiseXORExpressionNoIn
			{
			DebugLocation(401, 11);
			PushFollow(Follow._bitwiseXORExpressionNoIn_in_bitwiseORExpressionNoIn3519);
			lhs=bitwiseXORExpressionNoIn();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_bitwiseXORExpressionNoIn.Add(lhs.Tree);


			{
			// AST REWRITE
			// elements: lhs
			// token labels: 
			// rule labels: lhs, retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_lhs=new RewriteRuleSubtreeStream(adaptor,"rule lhs",lhs!=null?lhs.Tree:null);
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 401:37: -> $lhs
			{
				DebugLocation(401, 41);
				adaptor.AddChild(root_0, stream_lhs.NextTree());

			}

			retval.Tree = root_0;
			}
			}

			}

			DebugLocation(402, 9);
			// JavaScript.g3:402:9: ( ( SkipSpace )* op= BIT_OR ( SkipSpace )* rhs= bitwiseXORExpressionNoIn -> ^( BINARY_EXPR $op $bitwiseORExpressionNoIn $rhs) )*
			try { DebugEnterSubRule(186);
			while (true)
			{
				int alt186=2;
				try { DebugEnterDecision(186, decisionCanBacktrack[186]);
				try
				{
					alt186 = dfa186.Predict(input);
				}
				catch (NoViableAltException nvae)
				{
					DebugRecognitionException(nvae);
					throw;
				}
				} finally { DebugExitDecision(186); }
				switch ( alt186 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:402:10: ( SkipSpace )* op= BIT_OR ( SkipSpace )* rhs= bitwiseXORExpressionNoIn
					{
					DebugLocation(402, 10);
					// JavaScript.g3:402:10: ( SkipSpace )*
					try { DebugEnterSubRule(184);
					while (true)
					{
						int alt184=2;
						try { DebugEnterDecision(184, decisionCanBacktrack[184]);
						int LA184_0 = input.LA(1);

						if ((LA184_0==SkipSpace))
						{
							alt184=1;
						}


						} finally { DebugExitDecision(184); }
						switch ( alt184 )
						{
						case 1:
							DebugEnterAlt(1);
							// JavaScript.g3:402:10: SkipSpace
							{
							DebugLocation(402, 10);
							SkipSpace303=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_bitwiseORExpressionNoIn3536); if (state.failed) return retval; 
							if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace303);


							}
							break;

						default:
							goto loop184;
						}
					}

					loop184:
						;

					} finally { DebugExitSubRule(184); }

					DebugLocation(402, 23);
					op=(CommonToken)Match(input,BIT_OR,Follow._BIT_OR_in_bitwiseORExpressionNoIn3541); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_BIT_OR.Add(op);

					DebugLocation(402, 31);
					// JavaScript.g3:402:31: ( SkipSpace )*
					try { DebugEnterSubRule(185);
					while (true)
					{
						int alt185=2;
						try { DebugEnterDecision(185, decisionCanBacktrack[185]);
						int LA185_0 = input.LA(1);

						if ((LA185_0==SkipSpace))
						{
							alt185=1;
						}


						} finally { DebugExitDecision(185); }
						switch ( alt185 )
						{
						case 1:
							DebugEnterAlt(1);
							// JavaScript.g3:402:31: SkipSpace
							{
							DebugLocation(402, 31);
							SkipSpace304=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_bitwiseORExpressionNoIn3543); if (state.failed) return retval; 
							if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace304);


							}
							break;

						default:
							goto loop185;
						}
					}

					loop185:
						;

					} finally { DebugExitSubRule(185); }

					DebugLocation(402, 45);
					PushFollow(Follow._bitwiseXORExpressionNoIn_in_bitwiseORExpressionNoIn3548);
					rhs=bitwiseXORExpressionNoIn();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_bitwiseXORExpressionNoIn.Add(rhs.Tree);


					{
					// AST REWRITE
					// elements: op, bitwiseORExpressionNoIn, rhs
					// token labels: op
					// rule labels: rhs, retval
					// token list labels: 
					// rule list labels: 
					// wildcard labels: 
					if ( state.backtracking == 0 ) {
					retval.Tree = root_0;
					RewriteRuleITokenStream stream_op=new RewriteRuleITokenStream(adaptor,"token op",op);
					RewriteRuleSubtreeStream stream_rhs=new RewriteRuleSubtreeStream(adaptor,"rule rhs",rhs!=null?rhs.Tree:null);
					RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

					root_0 = (CommonTree)adaptor.Nil();
					// 402:71: -> ^( BINARY_EXPR $op $bitwiseORExpressionNoIn $rhs)
					{
						DebugLocation(402, 74);
						// JavaScript.g3:402:74: ^( BINARY_EXPR $op $bitwiseORExpressionNoIn $rhs)
						{
						CommonTree root_1 = (CommonTree)adaptor.Nil();
						DebugLocation(402, 76);
						root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(BINARY_EXPR, "BINARY_EXPR"), root_1);

						DebugLocation(402, 89);
						adaptor.AddChild(root_1, stream_op.NextNode());
						DebugLocation(402, 93);
						adaptor.AddChild(root_1, stream_retval.NextTree());
						DebugLocation(402, 118);
						adaptor.AddChild(root_1, stream_rhs.NextTree());

						adaptor.AddChild(root_0, root_1);
						}

					}

					retval.Tree = root_0;
					}
					}

					}
					break;

				default:
					goto loop186;
				}
			}

			loop186:
				;

			} finally { DebugExitSubRule(186); }


			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("bitwiseORExpressionNoIn", 64);
			LeaveRule("bitwiseORExpressionNoIn", 64);
			Leave_bitwiseORExpressionNoIn();
			if (state.backtracking > 0) { Memoize(input, 64, bitwiseORExpressionNoIn_StartIndex); }
		}
		DebugLocation(403, 4);
		} finally { DebugExitRule(GrammarFileName, "bitwiseORExpressionNoIn"); }
		return retval;

	}
	// $ANTLR end "bitwiseORExpressionNoIn"

	public class bitwiseXORExpression_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_bitwiseXORExpression();
	partial void Leave_bitwiseXORExpression();

	// $ANTLR start "bitwiseXORExpression"
	// JavaScript.g3:405:1: bitwiseXORExpression : (lhs= bitwiseANDExpression -> $lhs) ( ( SkipSpace )* op= BIT_XOR ( SkipSpace )* rhs= bitwiseANDExpression -> ^( BINARY_EXPR $op $bitwiseXORExpression $rhs) )* ;
	[GrammarRule("bitwiseXORExpression")]
	private JavaScriptParser.bitwiseXORExpression_return bitwiseXORExpression()
	{
		Enter_bitwiseXORExpression();
		EnterRule("bitwiseXORExpression", 65);
		TraceIn("bitwiseXORExpression", 65);
		JavaScriptParser.bitwiseXORExpression_return retval = new JavaScriptParser.bitwiseXORExpression_return();
		retval.Start = (CommonToken)input.LT(1);
		int bitwiseXORExpression_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken op=null;
		CommonToken SkipSpace305=null;
		CommonToken SkipSpace306=null;
		JavaScriptParser.bitwiseANDExpression_return lhs = default(JavaScriptParser.bitwiseANDExpression_return);
		JavaScriptParser.bitwiseANDExpression_return rhs = default(JavaScriptParser.bitwiseANDExpression_return);

		CommonTree op_tree=null;
		CommonTree SkipSpace305_tree=null;
		CommonTree SkipSpace306_tree=null;
		RewriteRuleITokenStream stream_SkipSpace=new RewriteRuleITokenStream(adaptor,"token SkipSpace");
		RewriteRuleITokenStream stream_BIT_XOR=new RewriteRuleITokenStream(adaptor,"token BIT_XOR");
		RewriteRuleSubtreeStream stream_bitwiseANDExpression=new RewriteRuleSubtreeStream(adaptor,"rule bitwiseANDExpression");
		try { DebugEnterRule(GrammarFileName, "bitwiseXORExpression");
		DebugLocation(405, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 65)) { return retval; }
			// JavaScript.g3:406:5: ( (lhs= bitwiseANDExpression -> $lhs) ( ( SkipSpace )* op= BIT_XOR ( SkipSpace )* rhs= bitwiseANDExpression -> ^( BINARY_EXPR $op $bitwiseXORExpression $rhs) )* )
			DebugEnterAlt(1);
			// JavaScript.g3:406:7: (lhs= bitwiseANDExpression -> $lhs) ( ( SkipSpace )* op= BIT_XOR ( SkipSpace )* rhs= bitwiseANDExpression -> ^( BINARY_EXPR $op $bitwiseXORExpression $rhs) )*
			{
			DebugLocation(406, 7);
			// JavaScript.g3:406:7: (lhs= bitwiseANDExpression -> $lhs)
			DebugEnterAlt(1);
			// JavaScript.g3:406:8: lhs= bitwiseANDExpression
			{
			DebugLocation(406, 11);
			PushFollow(Follow._bitwiseANDExpression_in_bitwiseXORExpression3586);
			lhs=bitwiseANDExpression();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_bitwiseANDExpression.Add(lhs.Tree);


			{
			// AST REWRITE
			// elements: lhs
			// token labels: 
			// rule labels: lhs, retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_lhs=new RewriteRuleSubtreeStream(adaptor,"rule lhs",lhs!=null?lhs.Tree:null);
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 406:33: -> $lhs
			{
				DebugLocation(406, 37);
				adaptor.AddChild(root_0, stream_lhs.NextTree());

			}

			retval.Tree = root_0;
			}
			}

			}

			DebugLocation(407, 9);
			// JavaScript.g3:407:9: ( ( SkipSpace )* op= BIT_XOR ( SkipSpace )* rhs= bitwiseANDExpression -> ^( BINARY_EXPR $op $bitwiseXORExpression $rhs) )*
			try { DebugEnterSubRule(189);
			while (true)
			{
				int alt189=2;
				try { DebugEnterDecision(189, decisionCanBacktrack[189]);
				try
				{
					alt189 = dfa189.Predict(input);
				}
				catch (NoViableAltException nvae)
				{
					DebugRecognitionException(nvae);
					throw;
				}
				} finally { DebugExitDecision(189); }
				switch ( alt189 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:407:10: ( SkipSpace )* op= BIT_XOR ( SkipSpace )* rhs= bitwiseANDExpression
					{
					DebugLocation(407, 10);
					// JavaScript.g3:407:10: ( SkipSpace )*
					try { DebugEnterSubRule(187);
					while (true)
					{
						int alt187=2;
						try { DebugEnterDecision(187, decisionCanBacktrack[187]);
						int LA187_0 = input.LA(1);

						if ((LA187_0==SkipSpace))
						{
							alt187=1;
						}


						} finally { DebugExitDecision(187); }
						switch ( alt187 )
						{
						case 1:
							DebugEnterAlt(1);
							// JavaScript.g3:407:10: SkipSpace
							{
							DebugLocation(407, 10);
							SkipSpace305=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_bitwiseXORExpression3603); if (state.failed) return retval; 
							if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace305);


							}
							break;

						default:
							goto loop187;
						}
					}

					loop187:
						;

					} finally { DebugExitSubRule(187); }

					DebugLocation(407, 23);
					op=(CommonToken)Match(input,BIT_XOR,Follow._BIT_XOR_in_bitwiseXORExpression3608); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_BIT_XOR.Add(op);

					DebugLocation(407, 32);
					// JavaScript.g3:407:32: ( SkipSpace )*
					try { DebugEnterSubRule(188);
					while (true)
					{
						int alt188=2;
						try { DebugEnterDecision(188, decisionCanBacktrack[188]);
						int LA188_0 = input.LA(1);

						if ((LA188_0==SkipSpace))
						{
							alt188=1;
						}


						} finally { DebugExitDecision(188); }
						switch ( alt188 )
						{
						case 1:
							DebugEnterAlt(1);
							// JavaScript.g3:407:32: SkipSpace
							{
							DebugLocation(407, 32);
							SkipSpace306=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_bitwiseXORExpression3610); if (state.failed) return retval; 
							if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace306);


							}
							break;

						default:
							goto loop188;
						}
					}

					loop188:
						;

					} finally { DebugExitSubRule(188); }

					DebugLocation(407, 46);
					PushFollow(Follow._bitwiseANDExpression_in_bitwiseXORExpression3615);
					rhs=bitwiseANDExpression();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_bitwiseANDExpression.Add(rhs.Tree);


					{
					// AST REWRITE
					// elements: op, bitwiseXORExpression, rhs
					// token labels: op
					// rule labels: rhs, retval
					// token list labels: 
					// rule list labels: 
					// wildcard labels: 
					if ( state.backtracking == 0 ) {
					retval.Tree = root_0;
					RewriteRuleITokenStream stream_op=new RewriteRuleITokenStream(adaptor,"token op",op);
					RewriteRuleSubtreeStream stream_rhs=new RewriteRuleSubtreeStream(adaptor,"rule rhs",rhs!=null?rhs.Tree:null);
					RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

					root_0 = (CommonTree)adaptor.Nil();
					// 407:68: -> ^( BINARY_EXPR $op $bitwiseXORExpression $rhs)
					{
						DebugLocation(407, 71);
						// JavaScript.g3:407:71: ^( BINARY_EXPR $op $bitwiseXORExpression $rhs)
						{
						CommonTree root_1 = (CommonTree)adaptor.Nil();
						DebugLocation(407, 73);
						root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(BINARY_EXPR, "BINARY_EXPR"), root_1);

						DebugLocation(407, 86);
						adaptor.AddChild(root_1, stream_op.NextNode());
						DebugLocation(407, 90);
						adaptor.AddChild(root_1, stream_retval.NextTree());
						DebugLocation(407, 112);
						adaptor.AddChild(root_1, stream_rhs.NextTree());

						adaptor.AddChild(root_0, root_1);
						}

					}

					retval.Tree = root_0;
					}
					}

					}
					break;

				default:
					goto loop189;
				}
			}

			loop189:
				;

			} finally { DebugExitSubRule(189); }


			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("bitwiseXORExpression", 65);
			LeaveRule("bitwiseXORExpression", 65);
			Leave_bitwiseXORExpression();
			if (state.backtracking > 0) { Memoize(input, 65, bitwiseXORExpression_StartIndex); }
		}
		DebugLocation(408, 4);
		} finally { DebugExitRule(GrammarFileName, "bitwiseXORExpression"); }
		return retval;

	}
	// $ANTLR end "bitwiseXORExpression"

	public class bitwiseXORExpressionNoIn_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_bitwiseXORExpressionNoIn();
	partial void Leave_bitwiseXORExpressionNoIn();

	// $ANTLR start "bitwiseXORExpressionNoIn"
	// JavaScript.g3:410:1: bitwiseXORExpressionNoIn : (lhs= bitwiseANDExpressionNoIn -> $lhs) ( ( SkipSpace )* op= BIT_XOR ( SkipSpace )* rhs= bitwiseANDExpressionNoIn -> ^( BINARY_EXPR $op $bitwiseXORExpressionNoIn $rhs) )* ;
	[GrammarRule("bitwiseXORExpressionNoIn")]
	private JavaScriptParser.bitwiseXORExpressionNoIn_return bitwiseXORExpressionNoIn()
	{
		Enter_bitwiseXORExpressionNoIn();
		EnterRule("bitwiseXORExpressionNoIn", 66);
		TraceIn("bitwiseXORExpressionNoIn", 66);
		JavaScriptParser.bitwiseXORExpressionNoIn_return retval = new JavaScriptParser.bitwiseXORExpressionNoIn_return();
		retval.Start = (CommonToken)input.LT(1);
		int bitwiseXORExpressionNoIn_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken op=null;
		CommonToken SkipSpace307=null;
		CommonToken SkipSpace308=null;
		JavaScriptParser.bitwiseANDExpressionNoIn_return lhs = default(JavaScriptParser.bitwiseANDExpressionNoIn_return);
		JavaScriptParser.bitwiseANDExpressionNoIn_return rhs = default(JavaScriptParser.bitwiseANDExpressionNoIn_return);

		CommonTree op_tree=null;
		CommonTree SkipSpace307_tree=null;
		CommonTree SkipSpace308_tree=null;
		RewriteRuleITokenStream stream_SkipSpace=new RewriteRuleITokenStream(adaptor,"token SkipSpace");
		RewriteRuleITokenStream stream_BIT_XOR=new RewriteRuleITokenStream(adaptor,"token BIT_XOR");
		RewriteRuleSubtreeStream stream_bitwiseANDExpressionNoIn=new RewriteRuleSubtreeStream(adaptor,"rule bitwiseANDExpressionNoIn");
		try { DebugEnterRule(GrammarFileName, "bitwiseXORExpressionNoIn");
		DebugLocation(410, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 66)) { return retval; }
			// JavaScript.g3:411:5: ( (lhs= bitwiseANDExpressionNoIn -> $lhs) ( ( SkipSpace )* op= BIT_XOR ( SkipSpace )* rhs= bitwiseANDExpressionNoIn -> ^( BINARY_EXPR $op $bitwiseXORExpressionNoIn $rhs) )* )
			DebugEnterAlt(1);
			// JavaScript.g3:411:7: (lhs= bitwiseANDExpressionNoIn -> $lhs) ( ( SkipSpace )* op= BIT_XOR ( SkipSpace )* rhs= bitwiseANDExpressionNoIn -> ^( BINARY_EXPR $op $bitwiseXORExpressionNoIn $rhs) )*
			{
			DebugLocation(411, 7);
			// JavaScript.g3:411:7: (lhs= bitwiseANDExpressionNoIn -> $lhs)
			DebugEnterAlt(1);
			// JavaScript.g3:411:8: lhs= bitwiseANDExpressionNoIn
			{
			DebugLocation(411, 11);
			PushFollow(Follow._bitwiseANDExpressionNoIn_in_bitwiseXORExpressionNoIn3653);
			lhs=bitwiseANDExpressionNoIn();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_bitwiseANDExpressionNoIn.Add(lhs.Tree);


			{
			// AST REWRITE
			// elements: lhs
			// token labels: 
			// rule labels: lhs, retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_lhs=new RewriteRuleSubtreeStream(adaptor,"rule lhs",lhs!=null?lhs.Tree:null);
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 411:37: -> $lhs
			{
				DebugLocation(411, 41);
				adaptor.AddChild(root_0, stream_lhs.NextTree());

			}

			retval.Tree = root_0;
			}
			}

			}

			DebugLocation(412, 9);
			// JavaScript.g3:412:9: ( ( SkipSpace )* op= BIT_XOR ( SkipSpace )* rhs= bitwiseANDExpressionNoIn -> ^( BINARY_EXPR $op $bitwiseXORExpressionNoIn $rhs) )*
			try { DebugEnterSubRule(192);
			while (true)
			{
				int alt192=2;
				try { DebugEnterDecision(192, decisionCanBacktrack[192]);
				try
				{
					alt192 = dfa192.Predict(input);
				}
				catch (NoViableAltException nvae)
				{
					DebugRecognitionException(nvae);
					throw;
				}
				} finally { DebugExitDecision(192); }
				switch ( alt192 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:412:10: ( SkipSpace )* op= BIT_XOR ( SkipSpace )* rhs= bitwiseANDExpressionNoIn
					{
					DebugLocation(412, 10);
					// JavaScript.g3:412:10: ( SkipSpace )*
					try { DebugEnterSubRule(190);
					while (true)
					{
						int alt190=2;
						try { DebugEnterDecision(190, decisionCanBacktrack[190]);
						int LA190_0 = input.LA(1);

						if ((LA190_0==SkipSpace))
						{
							alt190=1;
						}


						} finally { DebugExitDecision(190); }
						switch ( alt190 )
						{
						case 1:
							DebugEnterAlt(1);
							// JavaScript.g3:412:10: SkipSpace
							{
							DebugLocation(412, 10);
							SkipSpace307=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_bitwiseXORExpressionNoIn3670); if (state.failed) return retval; 
							if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace307);


							}
							break;

						default:
							goto loop190;
						}
					}

					loop190:
						;

					} finally { DebugExitSubRule(190); }

					DebugLocation(412, 23);
					op=(CommonToken)Match(input,BIT_XOR,Follow._BIT_XOR_in_bitwiseXORExpressionNoIn3675); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_BIT_XOR.Add(op);

					DebugLocation(412, 32);
					// JavaScript.g3:412:32: ( SkipSpace )*
					try { DebugEnterSubRule(191);
					while (true)
					{
						int alt191=2;
						try { DebugEnterDecision(191, decisionCanBacktrack[191]);
						int LA191_0 = input.LA(1);

						if ((LA191_0==SkipSpace))
						{
							alt191=1;
						}


						} finally { DebugExitDecision(191); }
						switch ( alt191 )
						{
						case 1:
							DebugEnterAlt(1);
							// JavaScript.g3:412:32: SkipSpace
							{
							DebugLocation(412, 32);
							SkipSpace308=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_bitwiseXORExpressionNoIn3677); if (state.failed) return retval; 
							if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace308);


							}
							break;

						default:
							goto loop191;
						}
					}

					loop191:
						;

					} finally { DebugExitSubRule(191); }

					DebugLocation(412, 46);
					PushFollow(Follow._bitwiseANDExpressionNoIn_in_bitwiseXORExpressionNoIn3682);
					rhs=bitwiseANDExpressionNoIn();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_bitwiseANDExpressionNoIn.Add(rhs.Tree);


					{
					// AST REWRITE
					// elements: op, bitwiseXORExpressionNoIn, rhs
					// token labels: op
					// rule labels: rhs, retval
					// token list labels: 
					// rule list labels: 
					// wildcard labels: 
					if ( state.backtracking == 0 ) {
					retval.Tree = root_0;
					RewriteRuleITokenStream stream_op=new RewriteRuleITokenStream(adaptor,"token op",op);
					RewriteRuleSubtreeStream stream_rhs=new RewriteRuleSubtreeStream(adaptor,"rule rhs",rhs!=null?rhs.Tree:null);
					RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

					root_0 = (CommonTree)adaptor.Nil();
					// 412:72: -> ^( BINARY_EXPR $op $bitwiseXORExpressionNoIn $rhs)
					{
						DebugLocation(412, 75);
						// JavaScript.g3:412:75: ^( BINARY_EXPR $op $bitwiseXORExpressionNoIn $rhs)
						{
						CommonTree root_1 = (CommonTree)adaptor.Nil();
						DebugLocation(412, 77);
						root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(BINARY_EXPR, "BINARY_EXPR"), root_1);

						DebugLocation(412, 90);
						adaptor.AddChild(root_1, stream_op.NextNode());
						DebugLocation(412, 94);
						adaptor.AddChild(root_1, stream_retval.NextTree());
						DebugLocation(412, 120);
						adaptor.AddChild(root_1, stream_rhs.NextTree());

						adaptor.AddChild(root_0, root_1);
						}

					}

					retval.Tree = root_0;
					}
					}

					}
					break;

				default:
					goto loop192;
				}
			}

			loop192:
				;

			} finally { DebugExitSubRule(192); }


			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("bitwiseXORExpressionNoIn", 66);
			LeaveRule("bitwiseXORExpressionNoIn", 66);
			Leave_bitwiseXORExpressionNoIn();
			if (state.backtracking > 0) { Memoize(input, 66, bitwiseXORExpressionNoIn_StartIndex); }
		}
		DebugLocation(413, 4);
		} finally { DebugExitRule(GrammarFileName, "bitwiseXORExpressionNoIn"); }
		return retval;

	}
	// $ANTLR end "bitwiseXORExpressionNoIn"

	public class bitwiseANDExpression_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_bitwiseANDExpression();
	partial void Leave_bitwiseANDExpression();

	// $ANTLR start "bitwiseANDExpression"
	// JavaScript.g3:415:1: bitwiseANDExpression : (lhs= equalityExpression -> $lhs) ( ( SkipSpace )* op= BIT_AND ( SkipSpace )* rhs= equalityExpression -> ^( BINARY_EXPR $op $bitwiseANDExpression $rhs) )* ;
	[GrammarRule("bitwiseANDExpression")]
	private JavaScriptParser.bitwiseANDExpression_return bitwiseANDExpression()
	{
		Enter_bitwiseANDExpression();
		EnterRule("bitwiseANDExpression", 67);
		TraceIn("bitwiseANDExpression", 67);
		JavaScriptParser.bitwiseANDExpression_return retval = new JavaScriptParser.bitwiseANDExpression_return();
		retval.Start = (CommonToken)input.LT(1);
		int bitwiseANDExpression_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken op=null;
		CommonToken SkipSpace309=null;
		CommonToken SkipSpace310=null;
		JavaScriptParser.equalityExpression_return lhs = default(JavaScriptParser.equalityExpression_return);
		JavaScriptParser.equalityExpression_return rhs = default(JavaScriptParser.equalityExpression_return);

		CommonTree op_tree=null;
		CommonTree SkipSpace309_tree=null;
		CommonTree SkipSpace310_tree=null;
		RewriteRuleITokenStream stream_SkipSpace=new RewriteRuleITokenStream(adaptor,"token SkipSpace");
		RewriteRuleITokenStream stream_BIT_AND=new RewriteRuleITokenStream(adaptor,"token BIT_AND");
		RewriteRuleSubtreeStream stream_equalityExpression=new RewriteRuleSubtreeStream(adaptor,"rule equalityExpression");
		try { DebugEnterRule(GrammarFileName, "bitwiseANDExpression");
		DebugLocation(415, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 67)) { return retval; }
			// JavaScript.g3:416:5: ( (lhs= equalityExpression -> $lhs) ( ( SkipSpace )* op= BIT_AND ( SkipSpace )* rhs= equalityExpression -> ^( BINARY_EXPR $op $bitwiseANDExpression $rhs) )* )
			DebugEnterAlt(1);
			// JavaScript.g3:416:7: (lhs= equalityExpression -> $lhs) ( ( SkipSpace )* op= BIT_AND ( SkipSpace )* rhs= equalityExpression -> ^( BINARY_EXPR $op $bitwiseANDExpression $rhs) )*
			{
			DebugLocation(416, 7);
			// JavaScript.g3:416:7: (lhs= equalityExpression -> $lhs)
			DebugEnterAlt(1);
			// JavaScript.g3:416:8: lhs= equalityExpression
			{
			DebugLocation(416, 11);
			PushFollow(Follow._equalityExpression_in_bitwiseANDExpression3720);
			lhs=equalityExpression();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_equalityExpression.Add(lhs.Tree);


			{
			// AST REWRITE
			// elements: lhs
			// token labels: 
			// rule labels: lhs, retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_lhs=new RewriteRuleSubtreeStream(adaptor,"rule lhs",lhs!=null?lhs.Tree:null);
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 416:31: -> $lhs
			{
				DebugLocation(416, 35);
				adaptor.AddChild(root_0, stream_lhs.NextTree());

			}

			retval.Tree = root_0;
			}
			}

			}

			DebugLocation(417, 9);
			// JavaScript.g3:417:9: ( ( SkipSpace )* op= BIT_AND ( SkipSpace )* rhs= equalityExpression -> ^( BINARY_EXPR $op $bitwiseANDExpression $rhs) )*
			try { DebugEnterSubRule(195);
			while (true)
			{
				int alt195=2;
				try { DebugEnterDecision(195, decisionCanBacktrack[195]);
				try
				{
					alt195 = dfa195.Predict(input);
				}
				catch (NoViableAltException nvae)
				{
					DebugRecognitionException(nvae);
					throw;
				}
				} finally { DebugExitDecision(195); }
				switch ( alt195 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:417:10: ( SkipSpace )* op= BIT_AND ( SkipSpace )* rhs= equalityExpression
					{
					DebugLocation(417, 10);
					// JavaScript.g3:417:10: ( SkipSpace )*
					try { DebugEnterSubRule(193);
					while (true)
					{
						int alt193=2;
						try { DebugEnterDecision(193, decisionCanBacktrack[193]);
						int LA193_0 = input.LA(1);

						if ((LA193_0==SkipSpace))
						{
							alt193=1;
						}


						} finally { DebugExitDecision(193); }
						switch ( alt193 )
						{
						case 1:
							DebugEnterAlt(1);
							// JavaScript.g3:417:10: SkipSpace
							{
							DebugLocation(417, 10);
							SkipSpace309=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_bitwiseANDExpression3737); if (state.failed) return retval; 
							if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace309);


							}
							break;

						default:
							goto loop193;
						}
					}

					loop193:
						;

					} finally { DebugExitSubRule(193); }

					DebugLocation(417, 23);
					op=(CommonToken)Match(input,BIT_AND,Follow._BIT_AND_in_bitwiseANDExpression3742); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_BIT_AND.Add(op);

					DebugLocation(417, 32);
					// JavaScript.g3:417:32: ( SkipSpace )*
					try { DebugEnterSubRule(194);
					while (true)
					{
						int alt194=2;
						try { DebugEnterDecision(194, decisionCanBacktrack[194]);
						int LA194_0 = input.LA(1);

						if ((LA194_0==SkipSpace))
						{
							alt194=1;
						}


						} finally { DebugExitDecision(194); }
						switch ( alt194 )
						{
						case 1:
							DebugEnterAlt(1);
							// JavaScript.g3:417:32: SkipSpace
							{
							DebugLocation(417, 32);
							SkipSpace310=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_bitwiseANDExpression3744); if (state.failed) return retval; 
							if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace310);


							}
							break;

						default:
							goto loop194;
						}
					}

					loop194:
						;

					} finally { DebugExitSubRule(194); }

					DebugLocation(417, 46);
					PushFollow(Follow._equalityExpression_in_bitwiseANDExpression3749);
					rhs=equalityExpression();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_equalityExpression.Add(rhs.Tree);


					{
					// AST REWRITE
					// elements: op, bitwiseANDExpression, rhs
					// token labels: op
					// rule labels: rhs, retval
					// token list labels: 
					// rule list labels: 
					// wildcard labels: 
					if ( state.backtracking == 0 ) {
					retval.Tree = root_0;
					RewriteRuleITokenStream stream_op=new RewriteRuleITokenStream(adaptor,"token op",op);
					RewriteRuleSubtreeStream stream_rhs=new RewriteRuleSubtreeStream(adaptor,"rule rhs",rhs!=null?rhs.Tree:null);
					RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

					root_0 = (CommonTree)adaptor.Nil();
					// 417:66: -> ^( BINARY_EXPR $op $bitwiseANDExpression $rhs)
					{
						DebugLocation(417, 69);
						// JavaScript.g3:417:69: ^( BINARY_EXPR $op $bitwiseANDExpression $rhs)
						{
						CommonTree root_1 = (CommonTree)adaptor.Nil();
						DebugLocation(417, 71);
						root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(BINARY_EXPR, "BINARY_EXPR"), root_1);

						DebugLocation(417, 84);
						adaptor.AddChild(root_1, stream_op.NextNode());
						DebugLocation(417, 88);
						adaptor.AddChild(root_1, stream_retval.NextTree());
						DebugLocation(417, 110);
						adaptor.AddChild(root_1, stream_rhs.NextTree());

						adaptor.AddChild(root_0, root_1);
						}

					}

					retval.Tree = root_0;
					}
					}

					}
					break;

				default:
					goto loop195;
				}
			}

			loop195:
				;

			} finally { DebugExitSubRule(195); }


			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("bitwiseANDExpression", 67);
			LeaveRule("bitwiseANDExpression", 67);
			Leave_bitwiseANDExpression();
			if (state.backtracking > 0) { Memoize(input, 67, bitwiseANDExpression_StartIndex); }
		}
		DebugLocation(418, 4);
		} finally { DebugExitRule(GrammarFileName, "bitwiseANDExpression"); }
		return retval;

	}
	// $ANTLR end "bitwiseANDExpression"

	public class bitwiseANDExpressionNoIn_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_bitwiseANDExpressionNoIn();
	partial void Leave_bitwiseANDExpressionNoIn();

	// $ANTLR start "bitwiseANDExpressionNoIn"
	// JavaScript.g3:420:1: bitwiseANDExpressionNoIn : (lhs= equalityExpressionNoIn -> $lhs) ( ( SkipSpace )* op= BIT_AND ( SkipSpace )* rhs= equalityExpressionNoIn -> ^( BINARY_EXPR $op $bitwiseANDExpressionNoIn $rhs) )* ;
	[GrammarRule("bitwiseANDExpressionNoIn")]
	private JavaScriptParser.bitwiseANDExpressionNoIn_return bitwiseANDExpressionNoIn()
	{
		Enter_bitwiseANDExpressionNoIn();
		EnterRule("bitwiseANDExpressionNoIn", 68);
		TraceIn("bitwiseANDExpressionNoIn", 68);
		JavaScriptParser.bitwiseANDExpressionNoIn_return retval = new JavaScriptParser.bitwiseANDExpressionNoIn_return();
		retval.Start = (CommonToken)input.LT(1);
		int bitwiseANDExpressionNoIn_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken op=null;
		CommonToken SkipSpace311=null;
		CommonToken SkipSpace312=null;
		JavaScriptParser.equalityExpressionNoIn_return lhs = default(JavaScriptParser.equalityExpressionNoIn_return);
		JavaScriptParser.equalityExpressionNoIn_return rhs = default(JavaScriptParser.equalityExpressionNoIn_return);

		CommonTree op_tree=null;
		CommonTree SkipSpace311_tree=null;
		CommonTree SkipSpace312_tree=null;
		RewriteRuleITokenStream stream_SkipSpace=new RewriteRuleITokenStream(adaptor,"token SkipSpace");
		RewriteRuleITokenStream stream_BIT_AND=new RewriteRuleITokenStream(adaptor,"token BIT_AND");
		RewriteRuleSubtreeStream stream_equalityExpressionNoIn=new RewriteRuleSubtreeStream(adaptor,"rule equalityExpressionNoIn");
		try { DebugEnterRule(GrammarFileName, "bitwiseANDExpressionNoIn");
		DebugLocation(420, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 68)) { return retval; }
			// JavaScript.g3:421:5: ( (lhs= equalityExpressionNoIn -> $lhs) ( ( SkipSpace )* op= BIT_AND ( SkipSpace )* rhs= equalityExpressionNoIn -> ^( BINARY_EXPR $op $bitwiseANDExpressionNoIn $rhs) )* )
			DebugEnterAlt(1);
			// JavaScript.g3:421:7: (lhs= equalityExpressionNoIn -> $lhs) ( ( SkipSpace )* op= BIT_AND ( SkipSpace )* rhs= equalityExpressionNoIn -> ^( BINARY_EXPR $op $bitwiseANDExpressionNoIn $rhs) )*
			{
			DebugLocation(421, 7);
			// JavaScript.g3:421:7: (lhs= equalityExpressionNoIn -> $lhs)
			DebugEnterAlt(1);
			// JavaScript.g3:421:8: lhs= equalityExpressionNoIn
			{
			DebugLocation(421, 11);
			PushFollow(Follow._equalityExpressionNoIn_in_bitwiseANDExpressionNoIn3787);
			lhs=equalityExpressionNoIn();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_equalityExpressionNoIn.Add(lhs.Tree);


			{
			// AST REWRITE
			// elements: lhs
			// token labels: 
			// rule labels: lhs, retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_lhs=new RewriteRuleSubtreeStream(adaptor,"rule lhs",lhs!=null?lhs.Tree:null);
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 421:35: -> $lhs
			{
				DebugLocation(421, 39);
				adaptor.AddChild(root_0, stream_lhs.NextTree());

			}

			retval.Tree = root_0;
			}
			}

			}

			DebugLocation(422, 9);
			// JavaScript.g3:422:9: ( ( SkipSpace )* op= BIT_AND ( SkipSpace )* rhs= equalityExpressionNoIn -> ^( BINARY_EXPR $op $bitwiseANDExpressionNoIn $rhs) )*
			try { DebugEnterSubRule(198);
			while (true)
			{
				int alt198=2;
				try { DebugEnterDecision(198, decisionCanBacktrack[198]);
				try
				{
					alt198 = dfa198.Predict(input);
				}
				catch (NoViableAltException nvae)
				{
					DebugRecognitionException(nvae);
					throw;
				}
				} finally { DebugExitDecision(198); }
				switch ( alt198 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:422:10: ( SkipSpace )* op= BIT_AND ( SkipSpace )* rhs= equalityExpressionNoIn
					{
					DebugLocation(422, 10);
					// JavaScript.g3:422:10: ( SkipSpace )*
					try { DebugEnterSubRule(196);
					while (true)
					{
						int alt196=2;
						try { DebugEnterDecision(196, decisionCanBacktrack[196]);
						int LA196_0 = input.LA(1);

						if ((LA196_0==SkipSpace))
						{
							alt196=1;
						}


						} finally { DebugExitDecision(196); }
						switch ( alt196 )
						{
						case 1:
							DebugEnterAlt(1);
							// JavaScript.g3:422:10: SkipSpace
							{
							DebugLocation(422, 10);
							SkipSpace311=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_bitwiseANDExpressionNoIn3804); if (state.failed) return retval; 
							if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace311);


							}
							break;

						default:
							goto loop196;
						}
					}

					loop196:
						;

					} finally { DebugExitSubRule(196); }

					DebugLocation(422, 23);
					op=(CommonToken)Match(input,BIT_AND,Follow._BIT_AND_in_bitwiseANDExpressionNoIn3809); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_BIT_AND.Add(op);

					DebugLocation(422, 32);
					// JavaScript.g3:422:32: ( SkipSpace )*
					try { DebugEnterSubRule(197);
					while (true)
					{
						int alt197=2;
						try { DebugEnterDecision(197, decisionCanBacktrack[197]);
						int LA197_0 = input.LA(1);

						if ((LA197_0==SkipSpace))
						{
							alt197=1;
						}


						} finally { DebugExitDecision(197); }
						switch ( alt197 )
						{
						case 1:
							DebugEnterAlt(1);
							// JavaScript.g3:422:32: SkipSpace
							{
							DebugLocation(422, 32);
							SkipSpace312=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_bitwiseANDExpressionNoIn3811); if (state.failed) return retval; 
							if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace312);


							}
							break;

						default:
							goto loop197;
						}
					}

					loop197:
						;

					} finally { DebugExitSubRule(197); }

					DebugLocation(422, 46);
					PushFollow(Follow._equalityExpressionNoIn_in_bitwiseANDExpressionNoIn3816);
					rhs=equalityExpressionNoIn();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_equalityExpressionNoIn.Add(rhs.Tree);


					{
					// AST REWRITE
					// elements: op, bitwiseANDExpressionNoIn, rhs
					// token labels: op
					// rule labels: rhs, retval
					// token list labels: 
					// rule list labels: 
					// wildcard labels: 
					if ( state.backtracking == 0 ) {
					retval.Tree = root_0;
					RewriteRuleITokenStream stream_op=new RewriteRuleITokenStream(adaptor,"token op",op);
					RewriteRuleSubtreeStream stream_rhs=new RewriteRuleSubtreeStream(adaptor,"rule rhs",rhs!=null?rhs.Tree:null);
					RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

					root_0 = (CommonTree)adaptor.Nil();
					// 422:70: -> ^( BINARY_EXPR $op $bitwiseANDExpressionNoIn $rhs)
					{
						DebugLocation(422, 73);
						// JavaScript.g3:422:73: ^( BINARY_EXPR $op $bitwiseANDExpressionNoIn $rhs)
						{
						CommonTree root_1 = (CommonTree)adaptor.Nil();
						DebugLocation(422, 75);
						root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(BINARY_EXPR, "BINARY_EXPR"), root_1);

						DebugLocation(422, 88);
						adaptor.AddChild(root_1, stream_op.NextNode());
						DebugLocation(422, 92);
						adaptor.AddChild(root_1, stream_retval.NextTree());
						DebugLocation(422, 118);
						adaptor.AddChild(root_1, stream_rhs.NextTree());

						adaptor.AddChild(root_0, root_1);
						}

					}

					retval.Tree = root_0;
					}
					}

					}
					break;

				default:
					goto loop198;
				}
			}

			loop198:
				;

			} finally { DebugExitSubRule(198); }


			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("bitwiseANDExpressionNoIn", 68);
			LeaveRule("bitwiseANDExpressionNoIn", 68);
			Leave_bitwiseANDExpressionNoIn();
			if (state.backtracking > 0) { Memoize(input, 68, bitwiseANDExpressionNoIn_StartIndex); }
		}
		DebugLocation(423, 4);
		} finally { DebugExitRule(GrammarFileName, "bitwiseANDExpressionNoIn"); }
		return retval;

	}
	// $ANTLR end "bitwiseANDExpressionNoIn"

	public class equalityExpression_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_equalityExpression();
	partial void Leave_equalityExpression();

	// $ANTLR start "equalityExpression"
	// JavaScript.g3:425:1: equalityExpression : (rhs= relationalExpression -> $rhs) ( ( SkipSpace )* op= equalityOperator ( SkipSpace )* rhs= relationalExpression -> ^( BINARY_EXPR $op $equalityExpression $rhs) )* ;
	[GrammarRule("equalityExpression")]
	private JavaScriptParser.equalityExpression_return equalityExpression()
	{
		Enter_equalityExpression();
		EnterRule("equalityExpression", 69);
		TraceIn("equalityExpression", 69);
		JavaScriptParser.equalityExpression_return retval = new JavaScriptParser.equalityExpression_return();
		retval.Start = (CommonToken)input.LT(1);
		int equalityExpression_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken SkipSpace313=null;
		CommonToken SkipSpace314=null;
		JavaScriptParser.relationalExpression_return rhs = default(JavaScriptParser.relationalExpression_return);
		JavaScriptParser.equalityOperator_return op = default(JavaScriptParser.equalityOperator_return);

		CommonTree SkipSpace313_tree=null;
		CommonTree SkipSpace314_tree=null;
		RewriteRuleITokenStream stream_SkipSpace=new RewriteRuleITokenStream(adaptor,"token SkipSpace");
		RewriteRuleSubtreeStream stream_relationalExpression=new RewriteRuleSubtreeStream(adaptor,"rule relationalExpression");
		RewriteRuleSubtreeStream stream_equalityOperator=new RewriteRuleSubtreeStream(adaptor,"rule equalityOperator");
		try { DebugEnterRule(GrammarFileName, "equalityExpression");
		DebugLocation(425, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 69)) { return retval; }
			// JavaScript.g3:426:5: ( (rhs= relationalExpression -> $rhs) ( ( SkipSpace )* op= equalityOperator ( SkipSpace )* rhs= relationalExpression -> ^( BINARY_EXPR $op $equalityExpression $rhs) )* )
			DebugEnterAlt(1);
			// JavaScript.g3:426:7: (rhs= relationalExpression -> $rhs) ( ( SkipSpace )* op= equalityOperator ( SkipSpace )* rhs= relationalExpression -> ^( BINARY_EXPR $op $equalityExpression $rhs) )*
			{
			DebugLocation(426, 7);
			// JavaScript.g3:426:7: (rhs= relationalExpression -> $rhs)
			DebugEnterAlt(1);
			// JavaScript.g3:426:8: rhs= relationalExpression
			{
			DebugLocation(426, 11);
			PushFollow(Follow._relationalExpression_in_equalityExpression3854);
			rhs=relationalExpression();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_relationalExpression.Add(rhs.Tree);


			{
			// AST REWRITE
			// elements: rhs
			// token labels: 
			// rule labels: rhs, retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_rhs=new RewriteRuleSubtreeStream(adaptor,"rule rhs",rhs!=null?rhs.Tree:null);
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 426:33: -> $rhs
			{
				DebugLocation(426, 37);
				adaptor.AddChild(root_0, stream_rhs.NextTree());

			}

			retval.Tree = root_0;
			}
			}

			}

			DebugLocation(427, 9);
			// JavaScript.g3:427:9: ( ( SkipSpace )* op= equalityOperator ( SkipSpace )* rhs= relationalExpression -> ^( BINARY_EXPR $op $equalityExpression $rhs) )*
			try { DebugEnterSubRule(201);
			while (true)
			{
				int alt201=2;
				try { DebugEnterDecision(201, decisionCanBacktrack[201]);
				try
				{
					alt201 = dfa201.Predict(input);
				}
				catch (NoViableAltException nvae)
				{
					DebugRecognitionException(nvae);
					throw;
				}
				} finally { DebugExitDecision(201); }
				switch ( alt201 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:427:10: ( SkipSpace )* op= equalityOperator ( SkipSpace )* rhs= relationalExpression
					{
					DebugLocation(427, 10);
					// JavaScript.g3:427:10: ( SkipSpace )*
					try { DebugEnterSubRule(199);
					while (true)
					{
						int alt199=2;
						try { DebugEnterDecision(199, decisionCanBacktrack[199]);
						int LA199_0 = input.LA(1);

						if ((LA199_0==SkipSpace))
						{
							alt199=1;
						}


						} finally { DebugExitDecision(199); }
						switch ( alt199 )
						{
						case 1:
							DebugEnterAlt(1);
							// JavaScript.g3:427:10: SkipSpace
							{
							DebugLocation(427, 10);
							SkipSpace313=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_equalityExpression3871); if (state.failed) return retval; 
							if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace313);


							}
							break;

						default:
							goto loop199;
						}
					}

					loop199:
						;

					} finally { DebugExitSubRule(199); }

					DebugLocation(427, 23);
					PushFollow(Follow._equalityOperator_in_equalityExpression3876);
					op=equalityOperator();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_equalityOperator.Add(op.Tree);
					DebugLocation(427, 41);
					// JavaScript.g3:427:41: ( SkipSpace )*
					try { DebugEnterSubRule(200);
					while (true)
					{
						int alt200=2;
						try { DebugEnterDecision(200, decisionCanBacktrack[200]);
						int LA200_0 = input.LA(1);

						if ((LA200_0==SkipSpace))
						{
							alt200=1;
						}


						} finally { DebugExitDecision(200); }
						switch ( alt200 )
						{
						case 1:
							DebugEnterAlt(1);
							// JavaScript.g3:427:41: SkipSpace
							{
							DebugLocation(427, 41);
							SkipSpace314=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_equalityExpression3878); if (state.failed) return retval; 
							if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace314);


							}
							break;

						default:
							goto loop200;
						}
					}

					loop200:
						;

					} finally { DebugExitSubRule(200); }

					DebugLocation(427, 55);
					PushFollow(Follow._relationalExpression_in_equalityExpression3883);
					rhs=relationalExpression();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_relationalExpression.Add(rhs.Tree);


					{
					// AST REWRITE
					// elements: op, equalityExpression, rhs
					// token labels: 
					// rule labels: op, rhs, retval
					// token list labels: 
					// rule list labels: 
					// wildcard labels: 
					if ( state.backtracking == 0 ) {
					retval.Tree = root_0;
					RewriteRuleSubtreeStream stream_op=new RewriteRuleSubtreeStream(adaptor,"rule op",op!=null?op.Tree:null);
					RewriteRuleSubtreeStream stream_rhs=new RewriteRuleSubtreeStream(adaptor,"rule rhs",rhs!=null?rhs.Tree:null);
					RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

					root_0 = (CommonTree)adaptor.Nil();
					// 427:77: -> ^( BINARY_EXPR $op $equalityExpression $rhs)
					{
						DebugLocation(427, 80);
						// JavaScript.g3:427:80: ^( BINARY_EXPR $op $equalityExpression $rhs)
						{
						CommonTree root_1 = (CommonTree)adaptor.Nil();
						DebugLocation(427, 82);
						root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(BINARY_EXPR, "BINARY_EXPR"), root_1);

						DebugLocation(427, 95);
						adaptor.AddChild(root_1, stream_op.NextTree());
						DebugLocation(427, 99);
						adaptor.AddChild(root_1, stream_retval.NextTree());
						DebugLocation(427, 119);
						adaptor.AddChild(root_1, stream_rhs.NextTree());

						adaptor.AddChild(root_0, root_1);
						}

					}

					retval.Tree = root_0;
					}
					}

					}
					break;

				default:
					goto loop201;
				}
			}

			loop201:
				;

			} finally { DebugExitSubRule(201); }


			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("equalityExpression", 69);
			LeaveRule("equalityExpression", 69);
			Leave_equalityExpression();
			if (state.backtracking > 0) { Memoize(input, 69, equalityExpression_StartIndex); }
		}
		DebugLocation(428, 4);
		} finally { DebugExitRule(GrammarFileName, "equalityExpression"); }
		return retval;

	}
	// $ANTLR end "equalityExpression"

	public class equalityExpressionNoIn_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_equalityExpressionNoIn();
	partial void Leave_equalityExpressionNoIn();

	// $ANTLR start "equalityExpressionNoIn"
	// JavaScript.g3:430:1: equalityExpressionNoIn : (rhs= relationalExpressionNoIn -> $rhs) ( ( SkipSpace )* op= equalityOperator ( SkipSpace )* rhs= relationalExpressionNoIn -> ^( BINARY_EXPR $op $equalityExpressionNoIn $rhs) )* ;
	[GrammarRule("equalityExpressionNoIn")]
	private JavaScriptParser.equalityExpressionNoIn_return equalityExpressionNoIn()
	{
		Enter_equalityExpressionNoIn();
		EnterRule("equalityExpressionNoIn", 70);
		TraceIn("equalityExpressionNoIn", 70);
		JavaScriptParser.equalityExpressionNoIn_return retval = new JavaScriptParser.equalityExpressionNoIn_return();
		retval.Start = (CommonToken)input.LT(1);
		int equalityExpressionNoIn_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken SkipSpace315=null;
		CommonToken SkipSpace316=null;
		JavaScriptParser.relationalExpressionNoIn_return rhs = default(JavaScriptParser.relationalExpressionNoIn_return);
		JavaScriptParser.equalityOperator_return op = default(JavaScriptParser.equalityOperator_return);

		CommonTree SkipSpace315_tree=null;
		CommonTree SkipSpace316_tree=null;
		RewriteRuleITokenStream stream_SkipSpace=new RewriteRuleITokenStream(adaptor,"token SkipSpace");
		RewriteRuleSubtreeStream stream_relationalExpressionNoIn=new RewriteRuleSubtreeStream(adaptor,"rule relationalExpressionNoIn");
		RewriteRuleSubtreeStream stream_equalityOperator=new RewriteRuleSubtreeStream(adaptor,"rule equalityOperator");
		try { DebugEnterRule(GrammarFileName, "equalityExpressionNoIn");
		DebugLocation(430, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 70)) { return retval; }
			// JavaScript.g3:431:5: ( (rhs= relationalExpressionNoIn -> $rhs) ( ( SkipSpace )* op= equalityOperator ( SkipSpace )* rhs= relationalExpressionNoIn -> ^( BINARY_EXPR $op $equalityExpressionNoIn $rhs) )* )
			DebugEnterAlt(1);
			// JavaScript.g3:431:7: (rhs= relationalExpressionNoIn -> $rhs) ( ( SkipSpace )* op= equalityOperator ( SkipSpace )* rhs= relationalExpressionNoIn -> ^( BINARY_EXPR $op $equalityExpressionNoIn $rhs) )*
			{
			DebugLocation(431, 7);
			// JavaScript.g3:431:7: (rhs= relationalExpressionNoIn -> $rhs)
			DebugEnterAlt(1);
			// JavaScript.g3:431:8: rhs= relationalExpressionNoIn
			{
			DebugLocation(431, 11);
			PushFollow(Follow._relationalExpressionNoIn_in_equalityExpressionNoIn3921);
			rhs=relationalExpressionNoIn();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_relationalExpressionNoIn.Add(rhs.Tree);


			{
			// AST REWRITE
			// elements: rhs
			// token labels: 
			// rule labels: rhs, retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_rhs=new RewriteRuleSubtreeStream(adaptor,"rule rhs",rhs!=null?rhs.Tree:null);
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 431:37: -> $rhs
			{
				DebugLocation(431, 41);
				adaptor.AddChild(root_0, stream_rhs.NextTree());

			}

			retval.Tree = root_0;
			}
			}

			}

			DebugLocation(432, 9);
			// JavaScript.g3:432:9: ( ( SkipSpace )* op= equalityOperator ( SkipSpace )* rhs= relationalExpressionNoIn -> ^( BINARY_EXPR $op $equalityExpressionNoIn $rhs) )*
			try { DebugEnterSubRule(204);
			while (true)
			{
				int alt204=2;
				try { DebugEnterDecision(204, decisionCanBacktrack[204]);
				try
				{
					alt204 = dfa204.Predict(input);
				}
				catch (NoViableAltException nvae)
				{
					DebugRecognitionException(nvae);
					throw;
				}
				} finally { DebugExitDecision(204); }
				switch ( alt204 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:432:10: ( SkipSpace )* op= equalityOperator ( SkipSpace )* rhs= relationalExpressionNoIn
					{
					DebugLocation(432, 10);
					// JavaScript.g3:432:10: ( SkipSpace )*
					try { DebugEnterSubRule(202);
					while (true)
					{
						int alt202=2;
						try { DebugEnterDecision(202, decisionCanBacktrack[202]);
						int LA202_0 = input.LA(1);

						if ((LA202_0==SkipSpace))
						{
							alt202=1;
						}


						} finally { DebugExitDecision(202); }
						switch ( alt202 )
						{
						case 1:
							DebugEnterAlt(1);
							// JavaScript.g3:432:10: SkipSpace
							{
							DebugLocation(432, 10);
							SkipSpace315=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_equalityExpressionNoIn3938); if (state.failed) return retval; 
							if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace315);


							}
							break;

						default:
							goto loop202;
						}
					}

					loop202:
						;

					} finally { DebugExitSubRule(202); }

					DebugLocation(432, 23);
					PushFollow(Follow._equalityOperator_in_equalityExpressionNoIn3943);
					op=equalityOperator();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_equalityOperator.Add(op.Tree);
					DebugLocation(432, 41);
					// JavaScript.g3:432:41: ( SkipSpace )*
					try { DebugEnterSubRule(203);
					while (true)
					{
						int alt203=2;
						try { DebugEnterDecision(203, decisionCanBacktrack[203]);
						int LA203_0 = input.LA(1);

						if ((LA203_0==SkipSpace))
						{
							alt203=1;
						}


						} finally { DebugExitDecision(203); }
						switch ( alt203 )
						{
						case 1:
							DebugEnterAlt(1);
							// JavaScript.g3:432:41: SkipSpace
							{
							DebugLocation(432, 41);
							SkipSpace316=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_equalityExpressionNoIn3945); if (state.failed) return retval; 
							if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace316);


							}
							break;

						default:
							goto loop203;
						}
					}

					loop203:
						;

					} finally { DebugExitSubRule(203); }

					DebugLocation(432, 55);
					PushFollow(Follow._relationalExpressionNoIn_in_equalityExpressionNoIn3950);
					rhs=relationalExpressionNoIn();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_relationalExpressionNoIn.Add(rhs.Tree);


					{
					// AST REWRITE
					// elements: op, equalityExpressionNoIn, rhs
					// token labels: 
					// rule labels: op, rhs, retval
					// token list labels: 
					// rule list labels: 
					// wildcard labels: 
					if ( state.backtracking == 0 ) {
					retval.Tree = root_0;
					RewriteRuleSubtreeStream stream_op=new RewriteRuleSubtreeStream(adaptor,"rule op",op!=null?op.Tree:null);
					RewriteRuleSubtreeStream stream_rhs=new RewriteRuleSubtreeStream(adaptor,"rule rhs",rhs!=null?rhs.Tree:null);
					RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

					root_0 = (CommonTree)adaptor.Nil();
					// 432:81: -> ^( BINARY_EXPR $op $equalityExpressionNoIn $rhs)
					{
						DebugLocation(432, 84);
						// JavaScript.g3:432:84: ^( BINARY_EXPR $op $equalityExpressionNoIn $rhs)
						{
						CommonTree root_1 = (CommonTree)adaptor.Nil();
						DebugLocation(432, 86);
						root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(BINARY_EXPR, "BINARY_EXPR"), root_1);

						DebugLocation(432, 99);
						adaptor.AddChild(root_1, stream_op.NextTree());
						DebugLocation(432, 103);
						adaptor.AddChild(root_1, stream_retval.NextTree());
						DebugLocation(432, 127);
						adaptor.AddChild(root_1, stream_rhs.NextTree());

						adaptor.AddChild(root_0, root_1);
						}

					}

					retval.Tree = root_0;
					}
					}

					}
					break;

				default:
					goto loop204;
				}
			}

			loop204:
				;

			} finally { DebugExitSubRule(204); }


			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("equalityExpressionNoIn", 70);
			LeaveRule("equalityExpressionNoIn", 70);
			Leave_equalityExpressionNoIn();
			if (state.backtracking > 0) { Memoize(input, 70, equalityExpressionNoIn_StartIndex); }
		}
		DebugLocation(433, 4);
		} finally { DebugExitRule(GrammarFileName, "equalityExpressionNoIn"); }
		return retval;

	}
	// $ANTLR end "equalityExpressionNoIn"

	public class equalityOperator_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_equalityOperator();
	partial void Leave_equalityOperator();

	// $ANTLR start "equalityOperator"
	// JavaScript.g3:435:1: equalityOperator : ( EQ | NEQ | REQ | RNQ );
	[GrammarRule("equalityOperator")]
	private JavaScriptParser.equalityOperator_return equalityOperator()
	{
		Enter_equalityOperator();
		EnterRule("equalityOperator", 71);
		TraceIn("equalityOperator", 71);
		JavaScriptParser.equalityOperator_return retval = new JavaScriptParser.equalityOperator_return();
		retval.Start = (CommonToken)input.LT(1);
		int equalityOperator_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken set317=null;

		CommonTree set317_tree=null;

		try { DebugEnterRule(GrammarFileName, "equalityOperator");
		DebugLocation(435, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 71)) { return retval; }
			// JavaScript.g3:436:5: ( EQ | NEQ | REQ | RNQ )
			DebugEnterAlt(1);
			// JavaScript.g3:
			{
			root_0 = (CommonTree)adaptor.Nil();

			DebugLocation(436, 5);
			set317=(CommonToken)input.LT(1);
			if (input.LA(1)==EQ||input.LA(1)==NEQ||input.LA(1)==REQ||input.LA(1)==RNQ)
			{
				input.Consume();
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, (CommonTree)adaptor.Create(set317));
				state.errorRecovery=false;state.failed=false;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return retval;}
				MismatchedSetException mse = new MismatchedSetException(null,input);
				DebugRecognitionException(mse);
				throw mse;
			}


			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("equalityOperator", 71);
			LeaveRule("equalityOperator", 71);
			Leave_equalityOperator();
			if (state.backtracking > 0) { Memoize(input, 71, equalityOperator_StartIndex); }
		}
		DebugLocation(437, 4);
		} finally { DebugExitRule(GrammarFileName, "equalityOperator"); }
		return retval;

	}
	// $ANTLR end "equalityOperator"

	public class relationalExpression_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_relationalExpression();
	partial void Leave_relationalExpression();

	// $ANTLR start "relationalExpression"
	// JavaScript.g3:439:1: relationalExpression : (lhs= shiftExpression -> $lhs) ( ( SkipSpace )* op= relationalOperator ( SkipSpace )* rhs= shiftExpression -> ^( BINARY_EXPR $op $relationalExpression $rhs) )* ;
	[GrammarRule("relationalExpression")]
	private JavaScriptParser.relationalExpression_return relationalExpression()
	{
		Enter_relationalExpression();
		EnterRule("relationalExpression", 72);
		TraceIn("relationalExpression", 72);
		JavaScriptParser.relationalExpression_return retval = new JavaScriptParser.relationalExpression_return();
		retval.Start = (CommonToken)input.LT(1);
		int relationalExpression_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken SkipSpace318=null;
		CommonToken SkipSpace319=null;
		JavaScriptParser.shiftExpression_return lhs = default(JavaScriptParser.shiftExpression_return);
		JavaScriptParser.relationalOperator_return op = default(JavaScriptParser.relationalOperator_return);
		JavaScriptParser.shiftExpression_return rhs = default(JavaScriptParser.shiftExpression_return);

		CommonTree SkipSpace318_tree=null;
		CommonTree SkipSpace319_tree=null;
		RewriteRuleITokenStream stream_SkipSpace=new RewriteRuleITokenStream(adaptor,"token SkipSpace");
		RewriteRuleSubtreeStream stream_shiftExpression=new RewriteRuleSubtreeStream(adaptor,"rule shiftExpression");
		RewriteRuleSubtreeStream stream_relationalOperator=new RewriteRuleSubtreeStream(adaptor,"rule relationalOperator");
		try { DebugEnterRule(GrammarFileName, "relationalExpression");
		DebugLocation(439, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 72)) { return retval; }
			// JavaScript.g3:440:5: ( (lhs= shiftExpression -> $lhs) ( ( SkipSpace )* op= relationalOperator ( SkipSpace )* rhs= shiftExpression -> ^( BINARY_EXPR $op $relationalExpression $rhs) )* )
			DebugEnterAlt(1);
			// JavaScript.g3:440:7: (lhs= shiftExpression -> $lhs) ( ( SkipSpace )* op= relationalOperator ( SkipSpace )* rhs= shiftExpression -> ^( BINARY_EXPR $op $relationalExpression $rhs) )*
			{
			DebugLocation(440, 7);
			// JavaScript.g3:440:7: (lhs= shiftExpression -> $lhs)
			DebugEnterAlt(1);
			// JavaScript.g3:440:8: lhs= shiftExpression
			{
			DebugLocation(440, 11);
			PushFollow(Follow._shiftExpression_in_relationalExpression4017);
			lhs=shiftExpression();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_shiftExpression.Add(lhs.Tree);


			{
			// AST REWRITE
			// elements: lhs
			// token labels: 
			// rule labels: lhs, retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_lhs=new RewriteRuleSubtreeStream(adaptor,"rule lhs",lhs!=null?lhs.Tree:null);
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 440:28: -> $lhs
			{
				DebugLocation(440, 32);
				adaptor.AddChild(root_0, stream_lhs.NextTree());

			}

			retval.Tree = root_0;
			}
			}

			}

			DebugLocation(441, 9);
			// JavaScript.g3:441:9: ( ( SkipSpace )* op= relationalOperator ( SkipSpace )* rhs= shiftExpression -> ^( BINARY_EXPR $op $relationalExpression $rhs) )*
			try { DebugEnterSubRule(207);
			while (true)
			{
				int alt207=2;
				try { DebugEnterDecision(207, decisionCanBacktrack[207]);
				try
				{
					alt207 = dfa207.Predict(input);
				}
				catch (NoViableAltException nvae)
				{
					DebugRecognitionException(nvae);
					throw;
				}
				} finally { DebugExitDecision(207); }
				switch ( alt207 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:441:10: ( SkipSpace )* op= relationalOperator ( SkipSpace )* rhs= shiftExpression
					{
					DebugLocation(441, 10);
					// JavaScript.g3:441:10: ( SkipSpace )*
					try { DebugEnterSubRule(205);
					while (true)
					{
						int alt205=2;
						try { DebugEnterDecision(205, decisionCanBacktrack[205]);
						int LA205_0 = input.LA(1);

						if ((LA205_0==SkipSpace))
						{
							alt205=1;
						}


						} finally { DebugExitDecision(205); }
						switch ( alt205 )
						{
						case 1:
							DebugEnterAlt(1);
							// JavaScript.g3:441:10: SkipSpace
							{
							DebugLocation(441, 10);
							SkipSpace318=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_relationalExpression4034); if (state.failed) return retval; 
							if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace318);


							}
							break;

						default:
							goto loop205;
						}
					}

					loop205:
						;

					} finally { DebugExitSubRule(205); }

					DebugLocation(441, 23);
					PushFollow(Follow._relationalOperator_in_relationalExpression4039);
					op=relationalOperator();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_relationalOperator.Add(op.Tree);
					DebugLocation(441, 43);
					// JavaScript.g3:441:43: ( SkipSpace )*
					try { DebugEnterSubRule(206);
					while (true)
					{
						int alt206=2;
						try { DebugEnterDecision(206, decisionCanBacktrack[206]);
						int LA206_0 = input.LA(1);

						if ((LA206_0==SkipSpace))
						{
							alt206=1;
						}


						} finally { DebugExitDecision(206); }
						switch ( alt206 )
						{
						case 1:
							DebugEnterAlt(1);
							// JavaScript.g3:441:43: SkipSpace
							{
							DebugLocation(441, 43);
							SkipSpace319=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_relationalExpression4041); if (state.failed) return retval; 
							if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace319);


							}
							break;

						default:
							goto loop206;
						}
					}

					loop206:
						;

					} finally { DebugExitSubRule(206); }

					DebugLocation(441, 57);
					PushFollow(Follow._shiftExpression_in_relationalExpression4046);
					rhs=shiftExpression();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_shiftExpression.Add(rhs.Tree);


					{
					// AST REWRITE
					// elements: op, relationalExpression, rhs
					// token labels: 
					// rule labels: op, rhs, retval
					// token list labels: 
					// rule list labels: 
					// wildcard labels: 
					if ( state.backtracking == 0 ) {
					retval.Tree = root_0;
					RewriteRuleSubtreeStream stream_op=new RewriteRuleSubtreeStream(adaptor,"rule op",op!=null?op.Tree:null);
					RewriteRuleSubtreeStream stream_rhs=new RewriteRuleSubtreeStream(adaptor,"rule rhs",rhs!=null?rhs.Tree:null);
					RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

					root_0 = (CommonTree)adaptor.Nil();
					// 441:74: -> ^( BINARY_EXPR $op $relationalExpression $rhs)
					{
						DebugLocation(441, 77);
						// JavaScript.g3:441:77: ^( BINARY_EXPR $op $relationalExpression $rhs)
						{
						CommonTree root_1 = (CommonTree)adaptor.Nil();
						DebugLocation(441, 79);
						root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(BINARY_EXPR, "BINARY_EXPR"), root_1);

						DebugLocation(441, 92);
						adaptor.AddChild(root_1, stream_op.NextTree());
						DebugLocation(441, 96);
						adaptor.AddChild(root_1, stream_retval.NextTree());
						DebugLocation(441, 118);
						adaptor.AddChild(root_1, stream_rhs.NextTree());

						adaptor.AddChild(root_0, root_1);
						}

					}

					retval.Tree = root_0;
					}
					}

					}
					break;

				default:
					goto loop207;
				}
			}

			loop207:
				;

			} finally { DebugExitSubRule(207); }


			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("relationalExpression", 72);
			LeaveRule("relationalExpression", 72);
			Leave_relationalExpression();
			if (state.backtracking > 0) { Memoize(input, 72, relationalExpression_StartIndex); }
		}
		DebugLocation(442, 4);
		} finally { DebugExitRule(GrammarFileName, "relationalExpression"); }
		return retval;

	}
	// $ANTLR end "relationalExpression"

	public class relationalExpressionNoIn_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_relationalExpressionNoIn();
	partial void Leave_relationalExpressionNoIn();

	// $ANTLR start "relationalExpressionNoIn"
	// JavaScript.g3:444:1: relationalExpressionNoIn : (lhs= shiftExpression -> $lhs) ( ( SkipSpace )* op= relationalOperatorNoIn ( SkipSpace )* rhs= shiftExpression -> ^( BINARY_EXPR $op $relationalExpressionNoIn $rhs) )* ;
	[GrammarRule("relationalExpressionNoIn")]
	private JavaScriptParser.relationalExpressionNoIn_return relationalExpressionNoIn()
	{
		Enter_relationalExpressionNoIn();
		EnterRule("relationalExpressionNoIn", 73);
		TraceIn("relationalExpressionNoIn", 73);
		JavaScriptParser.relationalExpressionNoIn_return retval = new JavaScriptParser.relationalExpressionNoIn_return();
		retval.Start = (CommonToken)input.LT(1);
		int relationalExpressionNoIn_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken SkipSpace320=null;
		CommonToken SkipSpace321=null;
		JavaScriptParser.shiftExpression_return lhs = default(JavaScriptParser.shiftExpression_return);
		JavaScriptParser.relationalOperatorNoIn_return op = default(JavaScriptParser.relationalOperatorNoIn_return);
		JavaScriptParser.shiftExpression_return rhs = default(JavaScriptParser.shiftExpression_return);

		CommonTree SkipSpace320_tree=null;
		CommonTree SkipSpace321_tree=null;
		RewriteRuleITokenStream stream_SkipSpace=new RewriteRuleITokenStream(adaptor,"token SkipSpace");
		RewriteRuleSubtreeStream stream_shiftExpression=new RewriteRuleSubtreeStream(adaptor,"rule shiftExpression");
		RewriteRuleSubtreeStream stream_relationalOperatorNoIn=new RewriteRuleSubtreeStream(adaptor,"rule relationalOperatorNoIn");
		try { DebugEnterRule(GrammarFileName, "relationalExpressionNoIn");
		DebugLocation(444, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 73)) { return retval; }
			// JavaScript.g3:445:5: ( (lhs= shiftExpression -> $lhs) ( ( SkipSpace )* op= relationalOperatorNoIn ( SkipSpace )* rhs= shiftExpression -> ^( BINARY_EXPR $op $relationalExpressionNoIn $rhs) )* )
			DebugEnterAlt(1);
			// JavaScript.g3:445:7: (lhs= shiftExpression -> $lhs) ( ( SkipSpace )* op= relationalOperatorNoIn ( SkipSpace )* rhs= shiftExpression -> ^( BINARY_EXPR $op $relationalExpressionNoIn $rhs) )*
			{
			DebugLocation(445, 7);
			// JavaScript.g3:445:7: (lhs= shiftExpression -> $lhs)
			DebugEnterAlt(1);
			// JavaScript.g3:445:8: lhs= shiftExpression
			{
			DebugLocation(445, 11);
			PushFollow(Follow._shiftExpression_in_relationalExpressionNoIn4084);
			lhs=shiftExpression();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_shiftExpression.Add(lhs.Tree);


			{
			// AST REWRITE
			// elements: lhs
			// token labels: 
			// rule labels: lhs, retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_lhs=new RewriteRuleSubtreeStream(adaptor,"rule lhs",lhs!=null?lhs.Tree:null);
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 445:28: -> $lhs
			{
				DebugLocation(445, 32);
				adaptor.AddChild(root_0, stream_lhs.NextTree());

			}

			retval.Tree = root_0;
			}
			}

			}

			DebugLocation(446, 9);
			// JavaScript.g3:446:9: ( ( SkipSpace )* op= relationalOperatorNoIn ( SkipSpace )* rhs= shiftExpression -> ^( BINARY_EXPR $op $relationalExpressionNoIn $rhs) )*
			try { DebugEnterSubRule(210);
			while (true)
			{
				int alt210=2;
				try { DebugEnterDecision(210, decisionCanBacktrack[210]);
				try
				{
					alt210 = dfa210.Predict(input);
				}
				catch (NoViableAltException nvae)
				{
					DebugRecognitionException(nvae);
					throw;
				}
				} finally { DebugExitDecision(210); }
				switch ( alt210 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:446:10: ( SkipSpace )* op= relationalOperatorNoIn ( SkipSpace )* rhs= shiftExpression
					{
					DebugLocation(446, 10);
					// JavaScript.g3:446:10: ( SkipSpace )*
					try { DebugEnterSubRule(208);
					while (true)
					{
						int alt208=2;
						try { DebugEnterDecision(208, decisionCanBacktrack[208]);
						int LA208_0 = input.LA(1);

						if ((LA208_0==SkipSpace))
						{
							alt208=1;
						}


						} finally { DebugExitDecision(208); }
						switch ( alt208 )
						{
						case 1:
							DebugEnterAlt(1);
							// JavaScript.g3:446:10: SkipSpace
							{
							DebugLocation(446, 10);
							SkipSpace320=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_relationalExpressionNoIn4101); if (state.failed) return retval; 
							if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace320);


							}
							break;

						default:
							goto loop208;
						}
					}

					loop208:
						;

					} finally { DebugExitSubRule(208); }

					DebugLocation(446, 23);
					PushFollow(Follow._relationalOperatorNoIn_in_relationalExpressionNoIn4106);
					op=relationalOperatorNoIn();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_relationalOperatorNoIn.Add(op.Tree);
					DebugLocation(446, 47);
					// JavaScript.g3:446:47: ( SkipSpace )*
					try { DebugEnterSubRule(209);
					while (true)
					{
						int alt209=2;
						try { DebugEnterDecision(209, decisionCanBacktrack[209]);
						int LA209_0 = input.LA(1);

						if ((LA209_0==SkipSpace))
						{
							alt209=1;
						}


						} finally { DebugExitDecision(209); }
						switch ( alt209 )
						{
						case 1:
							DebugEnterAlt(1);
							// JavaScript.g3:446:47: SkipSpace
							{
							DebugLocation(446, 47);
							SkipSpace321=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_relationalExpressionNoIn4108); if (state.failed) return retval; 
							if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace321);


							}
							break;

						default:
							goto loop209;
						}
					}

					loop209:
						;

					} finally { DebugExitSubRule(209); }

					DebugLocation(446, 61);
					PushFollow(Follow._shiftExpression_in_relationalExpressionNoIn4113);
					rhs=shiftExpression();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_shiftExpression.Add(rhs.Tree);


					{
					// AST REWRITE
					// elements: op, relationalExpressionNoIn, rhs
					// token labels: 
					// rule labels: op, rhs, retval
					// token list labels: 
					// rule list labels: 
					// wildcard labels: 
					if ( state.backtracking == 0 ) {
					retval.Tree = root_0;
					RewriteRuleSubtreeStream stream_op=new RewriteRuleSubtreeStream(adaptor,"rule op",op!=null?op.Tree:null);
					RewriteRuleSubtreeStream stream_rhs=new RewriteRuleSubtreeStream(adaptor,"rule rhs",rhs!=null?rhs.Tree:null);
					RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

					root_0 = (CommonTree)adaptor.Nil();
					// 446:78: -> ^( BINARY_EXPR $op $relationalExpressionNoIn $rhs)
					{
						DebugLocation(446, 81);
						// JavaScript.g3:446:81: ^( BINARY_EXPR $op $relationalExpressionNoIn $rhs)
						{
						CommonTree root_1 = (CommonTree)adaptor.Nil();
						DebugLocation(446, 83);
						root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(BINARY_EXPR, "BINARY_EXPR"), root_1);

						DebugLocation(446, 96);
						adaptor.AddChild(root_1, stream_op.NextTree());
						DebugLocation(446, 100);
						adaptor.AddChild(root_1, stream_retval.NextTree());
						DebugLocation(446, 126);
						adaptor.AddChild(root_1, stream_rhs.NextTree());

						adaptor.AddChild(root_0, root_1);
						}

					}

					retval.Tree = root_0;
					}
					}

					}
					break;

				default:
					goto loop210;
				}
			}

			loop210:
				;

			} finally { DebugExitSubRule(210); }


			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("relationalExpressionNoIn", 73);
			LeaveRule("relationalExpressionNoIn", 73);
			Leave_relationalExpressionNoIn();
			if (state.backtracking > 0) { Memoize(input, 73, relationalExpressionNoIn_StartIndex); }
		}
		DebugLocation(447, 4);
		} finally { DebugExitRule(GrammarFileName, "relationalExpressionNoIn"); }
		return retval;

	}
	// $ANTLR end "relationalExpressionNoIn"

	public class relationalOperator_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_relationalOperator();
	partial void Leave_relationalOperator();

	// $ANTLR start "relationalOperator"
	// JavaScript.g3:449:1: relationalOperator : ( LT | GT | LTE | GTE | INST_OF | IN );
	[GrammarRule("relationalOperator")]
	private JavaScriptParser.relationalOperator_return relationalOperator()
	{
		Enter_relationalOperator();
		EnterRule("relationalOperator", 74);
		TraceIn("relationalOperator", 74);
		JavaScriptParser.relationalOperator_return retval = new JavaScriptParser.relationalOperator_return();
		retval.Start = (CommonToken)input.LT(1);
		int relationalOperator_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken set322=null;

		CommonTree set322_tree=null;

		try { DebugEnterRule(GrammarFileName, "relationalOperator");
		DebugLocation(449, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 74)) { return retval; }
			// JavaScript.g3:450:5: ( LT | GT | LTE | GTE | INST_OF | IN )
			DebugEnterAlt(1);
			// JavaScript.g3:
			{
			root_0 = (CommonTree)adaptor.Nil();

			DebugLocation(450, 5);
			set322=(CommonToken)input.LT(1);
			if ((input.LA(1)>=GT && input.LA(1)<=GTE)||input.LA(1)==IN||input.LA(1)==INST_OF||(input.LA(1)>=LT && input.LA(1)<=LTE))
			{
				input.Consume();
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, (CommonTree)adaptor.Create(set322));
				state.errorRecovery=false;state.failed=false;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return retval;}
				MismatchedSetException mse = new MismatchedSetException(null,input);
				DebugRecognitionException(mse);
				throw mse;
			}


			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("relationalOperator", 74);
			LeaveRule("relationalOperator", 74);
			Leave_relationalOperator();
			if (state.backtracking > 0) { Memoize(input, 74, relationalOperator_StartIndex); }
		}
		DebugLocation(451, 4);
		} finally { DebugExitRule(GrammarFileName, "relationalOperator"); }
		return retval;

	}
	// $ANTLR end "relationalOperator"

	public class relationalOperatorNoIn_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_relationalOperatorNoIn();
	partial void Leave_relationalOperatorNoIn();

	// $ANTLR start "relationalOperatorNoIn"
	// JavaScript.g3:453:1: relationalOperatorNoIn : ( LT | GT | LTE | GTE | INST_OF );
	[GrammarRule("relationalOperatorNoIn")]
	private JavaScriptParser.relationalOperatorNoIn_return relationalOperatorNoIn()
	{
		Enter_relationalOperatorNoIn();
		EnterRule("relationalOperatorNoIn", 75);
		TraceIn("relationalOperatorNoIn", 75);
		JavaScriptParser.relationalOperatorNoIn_return retval = new JavaScriptParser.relationalOperatorNoIn_return();
		retval.Start = (CommonToken)input.LT(1);
		int relationalOperatorNoIn_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken set323=null;

		CommonTree set323_tree=null;

		try { DebugEnterRule(GrammarFileName, "relationalOperatorNoIn");
		DebugLocation(453, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 75)) { return retval; }
			// JavaScript.g3:454:5: ( LT | GT | LTE | GTE | INST_OF )
			DebugEnterAlt(1);
			// JavaScript.g3:
			{
			root_0 = (CommonTree)adaptor.Nil();

			DebugLocation(454, 5);
			set323=(CommonToken)input.LT(1);
			if ((input.LA(1)>=GT && input.LA(1)<=GTE)||input.LA(1)==INST_OF||(input.LA(1)>=LT && input.LA(1)<=LTE))
			{
				input.Consume();
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, (CommonTree)adaptor.Create(set323));
				state.errorRecovery=false;state.failed=false;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return retval;}
				MismatchedSetException mse = new MismatchedSetException(null,input);
				DebugRecognitionException(mse);
				throw mse;
			}


			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("relationalOperatorNoIn", 75);
			LeaveRule("relationalOperatorNoIn", 75);
			Leave_relationalOperatorNoIn();
			if (state.backtracking > 0) { Memoize(input, 75, relationalOperatorNoIn_StartIndex); }
		}
		DebugLocation(455, 4);
		} finally { DebugExitRule(GrammarFileName, "relationalOperatorNoIn"); }
		return retval;

	}
	// $ANTLR end "relationalOperatorNoIn"

	public class shiftExpression_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_shiftExpression();
	partial void Leave_shiftExpression();

	// $ANTLR start "shiftExpression"
	// JavaScript.g3:457:1: shiftExpression : (lhs= additiveExpression -> $lhs) ( ( SkipSpace )* op= shiftOperator ( SkipSpace )* rhs= additiveExpression -> ^( BINARY_EXPR $op $shiftExpression $rhs) )* ;
	[GrammarRule("shiftExpression")]
	private JavaScriptParser.shiftExpression_return shiftExpression()
	{
		Enter_shiftExpression();
		EnterRule("shiftExpression", 76);
		TraceIn("shiftExpression", 76);
		JavaScriptParser.shiftExpression_return retval = new JavaScriptParser.shiftExpression_return();
		retval.Start = (CommonToken)input.LT(1);
		int shiftExpression_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken SkipSpace324=null;
		CommonToken SkipSpace325=null;
		JavaScriptParser.additiveExpression_return lhs = default(JavaScriptParser.additiveExpression_return);
		JavaScriptParser.shiftOperator_return op = default(JavaScriptParser.shiftOperator_return);
		JavaScriptParser.additiveExpression_return rhs = default(JavaScriptParser.additiveExpression_return);

		CommonTree SkipSpace324_tree=null;
		CommonTree SkipSpace325_tree=null;
		RewriteRuleITokenStream stream_SkipSpace=new RewriteRuleITokenStream(adaptor,"token SkipSpace");
		RewriteRuleSubtreeStream stream_additiveExpression=new RewriteRuleSubtreeStream(adaptor,"rule additiveExpression");
		RewriteRuleSubtreeStream stream_shiftOperator=new RewriteRuleSubtreeStream(adaptor,"rule shiftOperator");
		try { DebugEnterRule(GrammarFileName, "shiftExpression");
		DebugLocation(457, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 76)) { return retval; }
			// JavaScript.g3:458:5: ( (lhs= additiveExpression -> $lhs) ( ( SkipSpace )* op= shiftOperator ( SkipSpace )* rhs= additiveExpression -> ^( BINARY_EXPR $op $shiftExpression $rhs) )* )
			DebugEnterAlt(1);
			// JavaScript.g3:458:7: (lhs= additiveExpression -> $lhs) ( ( SkipSpace )* op= shiftOperator ( SkipSpace )* rhs= additiveExpression -> ^( BINARY_EXPR $op $shiftExpression $rhs) )*
			{
			DebugLocation(458, 7);
			// JavaScript.g3:458:7: (lhs= additiveExpression -> $lhs)
			DebugEnterAlt(1);
			// JavaScript.g3:458:8: lhs= additiveExpression
			{
			DebugLocation(458, 11);
			PushFollow(Follow._additiveExpression_in_shiftExpression4221);
			lhs=additiveExpression();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_additiveExpression.Add(lhs.Tree);


			{
			// AST REWRITE
			// elements: lhs
			// token labels: 
			// rule labels: lhs, retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_lhs=new RewriteRuleSubtreeStream(adaptor,"rule lhs",lhs!=null?lhs.Tree:null);
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 458:31: -> $lhs
			{
				DebugLocation(458, 35);
				adaptor.AddChild(root_0, stream_lhs.NextTree());

			}

			retval.Tree = root_0;
			}
			}

			}

			DebugLocation(459, 9);
			// JavaScript.g3:459:9: ( ( SkipSpace )* op= shiftOperator ( SkipSpace )* rhs= additiveExpression -> ^( BINARY_EXPR $op $shiftExpression $rhs) )*
			try { DebugEnterSubRule(213);
			while (true)
			{
				int alt213=2;
				try { DebugEnterDecision(213, decisionCanBacktrack[213]);
				try
				{
					alt213 = dfa213.Predict(input);
				}
				catch (NoViableAltException nvae)
				{
					DebugRecognitionException(nvae);
					throw;
				}
				} finally { DebugExitDecision(213); }
				switch ( alt213 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:459:10: ( SkipSpace )* op= shiftOperator ( SkipSpace )* rhs= additiveExpression
					{
					DebugLocation(459, 10);
					// JavaScript.g3:459:10: ( SkipSpace )*
					try { DebugEnterSubRule(211);
					while (true)
					{
						int alt211=2;
						try { DebugEnterDecision(211, decisionCanBacktrack[211]);
						int LA211_0 = input.LA(1);

						if ((LA211_0==SkipSpace))
						{
							alt211=1;
						}


						} finally { DebugExitDecision(211); }
						switch ( alt211 )
						{
						case 1:
							DebugEnterAlt(1);
							// JavaScript.g3:459:10: SkipSpace
							{
							DebugLocation(459, 10);
							SkipSpace324=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_shiftExpression4238); if (state.failed) return retval; 
							if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace324);


							}
							break;

						default:
							goto loop211;
						}
					}

					loop211:
						;

					} finally { DebugExitSubRule(211); }

					DebugLocation(459, 23);
					PushFollow(Follow._shiftOperator_in_shiftExpression4243);
					op=shiftOperator();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_shiftOperator.Add(op.Tree);
					DebugLocation(459, 38);
					// JavaScript.g3:459:38: ( SkipSpace )*
					try { DebugEnterSubRule(212);
					while (true)
					{
						int alt212=2;
						try { DebugEnterDecision(212, decisionCanBacktrack[212]);
						int LA212_0 = input.LA(1);

						if ((LA212_0==SkipSpace))
						{
							alt212=1;
						}


						} finally { DebugExitDecision(212); }
						switch ( alt212 )
						{
						case 1:
							DebugEnterAlt(1);
							// JavaScript.g3:459:38: SkipSpace
							{
							DebugLocation(459, 38);
							SkipSpace325=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_shiftExpression4245); if (state.failed) return retval; 
							if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace325);


							}
							break;

						default:
							goto loop212;
						}
					}

					loop212:
						;

					} finally { DebugExitSubRule(212); }

					DebugLocation(459, 52);
					PushFollow(Follow._additiveExpression_in_shiftExpression4250);
					rhs=additiveExpression();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_additiveExpression.Add(rhs.Tree);


					{
					// AST REWRITE
					// elements: op, shiftExpression, rhs
					// token labels: 
					// rule labels: op, rhs, retval
					// token list labels: 
					// rule list labels: 
					// wildcard labels: 
					if ( state.backtracking == 0 ) {
					retval.Tree = root_0;
					RewriteRuleSubtreeStream stream_op=new RewriteRuleSubtreeStream(adaptor,"rule op",op!=null?op.Tree:null);
					RewriteRuleSubtreeStream stream_rhs=new RewriteRuleSubtreeStream(adaptor,"rule rhs",rhs!=null?rhs.Tree:null);
					RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

					root_0 = (CommonTree)adaptor.Nil();
					// 459:72: -> ^( BINARY_EXPR $op $shiftExpression $rhs)
					{
						DebugLocation(459, 75);
						// JavaScript.g3:459:75: ^( BINARY_EXPR $op $shiftExpression $rhs)
						{
						CommonTree root_1 = (CommonTree)adaptor.Nil();
						DebugLocation(459, 77);
						root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(BINARY_EXPR, "BINARY_EXPR"), root_1);

						DebugLocation(459, 90);
						adaptor.AddChild(root_1, stream_op.NextTree());
						DebugLocation(459, 94);
						adaptor.AddChild(root_1, stream_retval.NextTree());
						DebugLocation(459, 111);
						adaptor.AddChild(root_1, stream_rhs.NextTree());

						adaptor.AddChild(root_0, root_1);
						}

					}

					retval.Tree = root_0;
					}
					}

					}
					break;

				default:
					goto loop213;
				}
			}

			loop213:
				;

			} finally { DebugExitSubRule(213); }


			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("shiftExpression", 76);
			LeaveRule("shiftExpression", 76);
			Leave_shiftExpression();
			if (state.backtracking > 0) { Memoize(input, 76, shiftExpression_StartIndex); }
		}
		DebugLocation(460, 4);
		} finally { DebugExitRule(GrammarFileName, "shiftExpression"); }
		return retval;

	}
	// $ANTLR end "shiftExpression"

	public class shiftOperator_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_shiftOperator();
	partial void Leave_shiftOperator();

	// $ANTLR start "shiftOperator"
	// JavaScript.g3:462:1: shiftOperator : ( SHL | SHR | USHR );
	[GrammarRule("shiftOperator")]
	private JavaScriptParser.shiftOperator_return shiftOperator()
	{
		Enter_shiftOperator();
		EnterRule("shiftOperator", 77);
		TraceIn("shiftOperator", 77);
		JavaScriptParser.shiftOperator_return retval = new JavaScriptParser.shiftOperator_return();
		retval.Start = (CommonToken)input.LT(1);
		int shiftOperator_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken set326=null;

		CommonTree set326_tree=null;

		try { DebugEnterRule(GrammarFileName, "shiftOperator");
		DebugLocation(462, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 77)) { return retval; }
			// JavaScript.g3:463:5: ( SHL | SHR | USHR )
			DebugEnterAlt(1);
			// JavaScript.g3:
			{
			root_0 = (CommonTree)adaptor.Nil();

			DebugLocation(463, 5);
			set326=(CommonToken)input.LT(1);
			if (input.LA(1)==SHL||input.LA(1)==SHR||input.LA(1)==USHR)
			{
				input.Consume();
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, (CommonTree)adaptor.Create(set326));
				state.errorRecovery=false;state.failed=false;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return retval;}
				MismatchedSetException mse = new MismatchedSetException(null,input);
				DebugRecognitionException(mse);
				throw mse;
			}


			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("shiftOperator", 77);
			LeaveRule("shiftOperator", 77);
			Leave_shiftOperator();
			if (state.backtracking > 0) { Memoize(input, 77, shiftOperator_StartIndex); }
		}
		DebugLocation(464, 4);
		} finally { DebugExitRule(GrammarFileName, "shiftOperator"); }
		return retval;

	}
	// $ANTLR end "shiftOperator"

	public class additiveExpression_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_additiveExpression();
	partial void Leave_additiveExpression();

	// $ANTLR start "additiveExpression"
	// JavaScript.g3:466:1: additiveExpression : (lhs= multiplicativeExpression -> $lhs) ( ( SkipSpace )* op= additiveOperator ( SkipSpace )* rhs= multiplicativeExpression -> ^( BINARY_EXPR $op $additiveExpression $rhs) )* ;
	[GrammarRule("additiveExpression")]
	private JavaScriptParser.additiveExpression_return additiveExpression()
	{
		Enter_additiveExpression();
		EnterRule("additiveExpression", 78);
		TraceIn("additiveExpression", 78);
		JavaScriptParser.additiveExpression_return retval = new JavaScriptParser.additiveExpression_return();
		retval.Start = (CommonToken)input.LT(1);
		int additiveExpression_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken SkipSpace327=null;
		CommonToken SkipSpace328=null;
		JavaScriptParser.multiplicativeExpression_return lhs = default(JavaScriptParser.multiplicativeExpression_return);
		JavaScriptParser.additiveOperator_return op = default(JavaScriptParser.additiveOperator_return);
		JavaScriptParser.multiplicativeExpression_return rhs = default(JavaScriptParser.multiplicativeExpression_return);

		CommonTree SkipSpace327_tree=null;
		CommonTree SkipSpace328_tree=null;
		RewriteRuleITokenStream stream_SkipSpace=new RewriteRuleITokenStream(adaptor,"token SkipSpace");
		RewriteRuleSubtreeStream stream_multiplicativeExpression=new RewriteRuleSubtreeStream(adaptor,"rule multiplicativeExpression");
		RewriteRuleSubtreeStream stream_additiveOperator=new RewriteRuleSubtreeStream(adaptor,"rule additiveOperator");
		try { DebugEnterRule(GrammarFileName, "additiveExpression");
		DebugLocation(466, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 78)) { return retval; }
			// JavaScript.g3:467:5: ( (lhs= multiplicativeExpression -> $lhs) ( ( SkipSpace )* op= additiveOperator ( SkipSpace )* rhs= multiplicativeExpression -> ^( BINARY_EXPR $op $additiveExpression $rhs) )* )
			DebugEnterAlt(1);
			// JavaScript.g3:467:7: (lhs= multiplicativeExpression -> $lhs) ( ( SkipSpace )* op= additiveOperator ( SkipSpace )* rhs= multiplicativeExpression -> ^( BINARY_EXPR $op $additiveExpression $rhs) )*
			{
			DebugLocation(467, 7);
			// JavaScript.g3:467:7: (lhs= multiplicativeExpression -> $lhs)
			DebugEnterAlt(1);
			// JavaScript.g3:467:8: lhs= multiplicativeExpression
			{
			DebugLocation(467, 11);
			PushFollow(Follow._multiplicativeExpression_in_additiveExpression4313);
			lhs=multiplicativeExpression();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_multiplicativeExpression.Add(lhs.Tree);


			{
			// AST REWRITE
			// elements: lhs
			// token labels: 
			// rule labels: lhs, retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_lhs=new RewriteRuleSubtreeStream(adaptor,"rule lhs",lhs!=null?lhs.Tree:null);
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 467:37: -> $lhs
			{
				DebugLocation(467, 41);
				adaptor.AddChild(root_0, stream_lhs.NextTree());

			}

			retval.Tree = root_0;
			}
			}

			}

			DebugLocation(468, 9);
			// JavaScript.g3:468:9: ( ( SkipSpace )* op= additiveOperator ( SkipSpace )* rhs= multiplicativeExpression -> ^( BINARY_EXPR $op $additiveExpression $rhs) )*
			try { DebugEnterSubRule(216);
			while (true)
			{
				int alt216=2;
				try { DebugEnterDecision(216, decisionCanBacktrack[216]);
				try
				{
					alt216 = dfa216.Predict(input);
				}
				catch (NoViableAltException nvae)
				{
					DebugRecognitionException(nvae);
					throw;
				}
				} finally { DebugExitDecision(216); }
				switch ( alt216 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:468:10: ( SkipSpace )* op= additiveOperator ( SkipSpace )* rhs= multiplicativeExpression
					{
					DebugLocation(468, 10);
					// JavaScript.g3:468:10: ( SkipSpace )*
					try { DebugEnterSubRule(214);
					while (true)
					{
						int alt214=2;
						try { DebugEnterDecision(214, decisionCanBacktrack[214]);
						int LA214_0 = input.LA(1);

						if ((LA214_0==SkipSpace))
						{
							alt214=1;
						}


						} finally { DebugExitDecision(214); }
						switch ( alt214 )
						{
						case 1:
							DebugEnterAlt(1);
							// JavaScript.g3:468:10: SkipSpace
							{
							DebugLocation(468, 10);
							SkipSpace327=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_additiveExpression4330); if (state.failed) return retval; 
							if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace327);


							}
							break;

						default:
							goto loop214;
						}
					}

					loop214:
						;

					} finally { DebugExitSubRule(214); }

					DebugLocation(468, 23);
					PushFollow(Follow._additiveOperator_in_additiveExpression4335);
					op=additiveOperator();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_additiveOperator.Add(op.Tree);
					DebugLocation(468, 41);
					// JavaScript.g3:468:41: ( SkipSpace )*
					try { DebugEnterSubRule(215);
					while (true)
					{
						int alt215=2;
						try { DebugEnterDecision(215, decisionCanBacktrack[215]);
						int LA215_0 = input.LA(1);

						if ((LA215_0==SkipSpace))
						{
							alt215=1;
						}


						} finally { DebugExitDecision(215); }
						switch ( alt215 )
						{
						case 1:
							DebugEnterAlt(1);
							// JavaScript.g3:468:41: SkipSpace
							{
							DebugLocation(468, 41);
							SkipSpace328=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_additiveExpression4337); if (state.failed) return retval; 
							if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace328);


							}
							break;

						default:
							goto loop215;
						}
					}

					loop215:
						;

					} finally { DebugExitSubRule(215); }

					DebugLocation(468, 55);
					PushFollow(Follow._multiplicativeExpression_in_additiveExpression4342);
					rhs=multiplicativeExpression();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_multiplicativeExpression.Add(rhs.Tree);


					{
					// AST REWRITE
					// elements: op, additiveExpression, rhs
					// token labels: 
					// rule labels: op, rhs, retval
					// token list labels: 
					// rule list labels: 
					// wildcard labels: 
					if ( state.backtracking == 0 ) {
					retval.Tree = root_0;
					RewriteRuleSubtreeStream stream_op=new RewriteRuleSubtreeStream(adaptor,"rule op",op!=null?op.Tree:null);
					RewriteRuleSubtreeStream stream_rhs=new RewriteRuleSubtreeStream(adaptor,"rule rhs",rhs!=null?rhs.Tree:null);
					RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

					root_0 = (CommonTree)adaptor.Nil();
					// 468:81: -> ^( BINARY_EXPR $op $additiveExpression $rhs)
					{
						DebugLocation(468, 84);
						// JavaScript.g3:468:84: ^( BINARY_EXPR $op $additiveExpression $rhs)
						{
						CommonTree root_1 = (CommonTree)adaptor.Nil();
						DebugLocation(468, 86);
						root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(BINARY_EXPR, "BINARY_EXPR"), root_1);

						DebugLocation(468, 99);
						adaptor.AddChild(root_1, stream_op.NextTree());
						DebugLocation(468, 103);
						adaptor.AddChild(root_1, stream_retval.NextTree());
						DebugLocation(468, 123);
						adaptor.AddChild(root_1, stream_rhs.NextTree());

						adaptor.AddChild(root_0, root_1);
						}

					}

					retval.Tree = root_0;
					}
					}

					}
					break;

				default:
					goto loop216;
				}
			}

			loop216:
				;

			} finally { DebugExitSubRule(216); }


			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("additiveExpression", 78);
			LeaveRule("additiveExpression", 78);
			Leave_additiveExpression();
			if (state.backtracking > 0) { Memoize(input, 78, additiveExpression_StartIndex); }
		}
		DebugLocation(469, 4);
		} finally { DebugExitRule(GrammarFileName, "additiveExpression"); }
		return retval;

	}
	// $ANTLR end "additiveExpression"

	public class additiveOperator_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_additiveOperator();
	partial void Leave_additiveOperator();

	// $ANTLR start "additiveOperator"
	// JavaScript.g3:471:1: additiveOperator : ( PLUS | MINUS );
	[GrammarRule("additiveOperator")]
	private JavaScriptParser.additiveOperator_return additiveOperator()
	{
		Enter_additiveOperator();
		EnterRule("additiveOperator", 79);
		TraceIn("additiveOperator", 79);
		JavaScriptParser.additiveOperator_return retval = new JavaScriptParser.additiveOperator_return();
		retval.Start = (CommonToken)input.LT(1);
		int additiveOperator_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken set329=null;

		CommonTree set329_tree=null;

		try { DebugEnterRule(GrammarFileName, "additiveOperator");
		DebugLocation(471, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 79)) { return retval; }
			// JavaScript.g3:472:5: ( PLUS | MINUS )
			DebugEnterAlt(1);
			// JavaScript.g3:
			{
			root_0 = (CommonTree)adaptor.Nil();

			DebugLocation(472, 5);
			set329=(CommonToken)input.LT(1);
			if (input.LA(1)==MINUS||input.LA(1)==PLUS)
			{
				input.Consume();
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, (CommonTree)adaptor.Create(set329));
				state.errorRecovery=false;state.failed=false;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return retval;}
				MismatchedSetException mse = new MismatchedSetException(null,input);
				DebugRecognitionException(mse);
				throw mse;
			}


			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("additiveOperator", 79);
			LeaveRule("additiveOperator", 79);
			Leave_additiveOperator();
			if (state.backtracking > 0) { Memoize(input, 79, additiveOperator_StartIndex); }
		}
		DebugLocation(473, 4);
		} finally { DebugExitRule(GrammarFileName, "additiveOperator"); }
		return retval;

	}
	// $ANTLR end "additiveOperator"

	public class multiplicativeExpression_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_multiplicativeExpression();
	partial void Leave_multiplicativeExpression();

	// $ANTLR start "multiplicativeExpression"
	// JavaScript.g3:475:1: multiplicativeExpression : (lhs= unaryExpression -> $lhs) ( ( SkipSpace )* op= multiplicativeOperator ( SkipSpace )* rhs= unaryExpression -> ^( BINARY_EXPR $op $multiplicativeExpression $rhs) )* ;
	[GrammarRule("multiplicativeExpression")]
	private JavaScriptParser.multiplicativeExpression_return multiplicativeExpression()
	{
		Enter_multiplicativeExpression();
		EnterRule("multiplicativeExpression", 80);
		TraceIn("multiplicativeExpression", 80);
		JavaScriptParser.multiplicativeExpression_return retval = new JavaScriptParser.multiplicativeExpression_return();
		retval.Start = (CommonToken)input.LT(1);
		int multiplicativeExpression_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken SkipSpace330=null;
		CommonToken SkipSpace331=null;
		JavaScriptParser.unaryExpression_return lhs = default(JavaScriptParser.unaryExpression_return);
		JavaScriptParser.multiplicativeOperator_return op = default(JavaScriptParser.multiplicativeOperator_return);
		JavaScriptParser.unaryExpression_return rhs = default(JavaScriptParser.unaryExpression_return);

		CommonTree SkipSpace330_tree=null;
		CommonTree SkipSpace331_tree=null;
		RewriteRuleITokenStream stream_SkipSpace=new RewriteRuleITokenStream(adaptor,"token SkipSpace");
		RewriteRuleSubtreeStream stream_unaryExpression=new RewriteRuleSubtreeStream(adaptor,"rule unaryExpression");
		RewriteRuleSubtreeStream stream_multiplicativeOperator=new RewriteRuleSubtreeStream(adaptor,"rule multiplicativeOperator");
		try { DebugEnterRule(GrammarFileName, "multiplicativeExpression");
		DebugLocation(475, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 80)) { return retval; }
			// JavaScript.g3:476:5: ( (lhs= unaryExpression -> $lhs) ( ( SkipSpace )* op= multiplicativeOperator ( SkipSpace )* rhs= unaryExpression -> ^( BINARY_EXPR $op $multiplicativeExpression $rhs) )* )
			DebugEnterAlt(1);
			// JavaScript.g3:476:7: (lhs= unaryExpression -> $lhs) ( ( SkipSpace )* op= multiplicativeOperator ( SkipSpace )* rhs= unaryExpression -> ^( BINARY_EXPR $op $multiplicativeExpression $rhs) )*
			{
			DebugLocation(476, 7);
			// JavaScript.g3:476:7: (lhs= unaryExpression -> $lhs)
			DebugEnterAlt(1);
			// JavaScript.g3:476:8: lhs= unaryExpression
			{
			DebugLocation(476, 11);
			PushFollow(Follow._unaryExpression_in_multiplicativeExpression4401);
			lhs=unaryExpression();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_unaryExpression.Add(lhs.Tree);


			{
			// AST REWRITE
			// elements: lhs
			// token labels: 
			// rule labels: lhs, retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_lhs=new RewriteRuleSubtreeStream(adaptor,"rule lhs",lhs!=null?lhs.Tree:null);
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 476:28: -> $lhs
			{
				DebugLocation(476, 32);
				adaptor.AddChild(root_0, stream_lhs.NextTree());

			}

			retval.Tree = root_0;
			}
			}

			}

			DebugLocation(477, 9);
			// JavaScript.g3:477:9: ( ( SkipSpace )* op= multiplicativeOperator ( SkipSpace )* rhs= unaryExpression -> ^( BINARY_EXPR $op $multiplicativeExpression $rhs) )*
			try { DebugEnterSubRule(219);
			while (true)
			{
				int alt219=2;
				try { DebugEnterDecision(219, decisionCanBacktrack[219]);
				try
				{
					alt219 = dfa219.Predict(input);
				}
				catch (NoViableAltException nvae)
				{
					DebugRecognitionException(nvae);
					throw;
				}
				} finally { DebugExitDecision(219); }
				switch ( alt219 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:477:10: ( SkipSpace )* op= multiplicativeOperator ( SkipSpace )* rhs= unaryExpression
					{
					DebugLocation(477, 10);
					// JavaScript.g3:477:10: ( SkipSpace )*
					try { DebugEnterSubRule(217);
					while (true)
					{
						int alt217=2;
						try { DebugEnterDecision(217, decisionCanBacktrack[217]);
						int LA217_0 = input.LA(1);

						if ((LA217_0==SkipSpace))
						{
							alt217=1;
						}


						} finally { DebugExitDecision(217); }
						switch ( alt217 )
						{
						case 1:
							DebugEnterAlt(1);
							// JavaScript.g3:477:10: SkipSpace
							{
							DebugLocation(477, 10);
							SkipSpace330=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_multiplicativeExpression4418); if (state.failed) return retval; 
							if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace330);


							}
							break;

						default:
							goto loop217;
						}
					}

					loop217:
						;

					} finally { DebugExitSubRule(217); }

					DebugLocation(477, 23);
					PushFollow(Follow._multiplicativeOperator_in_multiplicativeExpression4423);
					op=multiplicativeOperator();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_multiplicativeOperator.Add(op.Tree);
					DebugLocation(477, 47);
					// JavaScript.g3:477:47: ( SkipSpace )*
					try { DebugEnterSubRule(218);
					while (true)
					{
						int alt218=2;
						try { DebugEnterDecision(218, decisionCanBacktrack[218]);
						int LA218_0 = input.LA(1);

						if ((LA218_0==SkipSpace))
						{
							alt218=1;
						}


						} finally { DebugExitDecision(218); }
						switch ( alt218 )
						{
						case 1:
							DebugEnterAlt(1);
							// JavaScript.g3:477:47: SkipSpace
							{
							DebugLocation(477, 47);
							SkipSpace331=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_multiplicativeExpression4425); if (state.failed) return retval; 
							if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace331);


							}
							break;

						default:
							goto loop218;
						}
					}

					loop218:
						;

					} finally { DebugExitSubRule(218); }

					DebugLocation(477, 61);
					PushFollow(Follow._unaryExpression_in_multiplicativeExpression4430);
					rhs=unaryExpression();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_unaryExpression.Add(rhs.Tree);


					{
					// AST REWRITE
					// elements: op, multiplicativeExpression, rhs
					// token labels: 
					// rule labels: op, rhs, retval
					// token list labels: 
					// rule list labels: 
					// wildcard labels: 
					if ( state.backtracking == 0 ) {
					retval.Tree = root_0;
					RewriteRuleSubtreeStream stream_op=new RewriteRuleSubtreeStream(adaptor,"rule op",op!=null?op.Tree:null);
					RewriteRuleSubtreeStream stream_rhs=new RewriteRuleSubtreeStream(adaptor,"rule rhs",rhs!=null?rhs.Tree:null);
					RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

					root_0 = (CommonTree)adaptor.Nil();
					// 477:78: -> ^( BINARY_EXPR $op $multiplicativeExpression $rhs)
					{
						DebugLocation(477, 81);
						// JavaScript.g3:477:81: ^( BINARY_EXPR $op $multiplicativeExpression $rhs)
						{
						CommonTree root_1 = (CommonTree)adaptor.Nil();
						DebugLocation(477, 83);
						root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(BINARY_EXPR, "BINARY_EXPR"), root_1);

						DebugLocation(477, 96);
						adaptor.AddChild(root_1, stream_op.NextTree());
						DebugLocation(477, 100);
						adaptor.AddChild(root_1, stream_retval.NextTree());
						DebugLocation(477, 126);
						adaptor.AddChild(root_1, stream_rhs.NextTree());

						adaptor.AddChild(root_0, root_1);
						}

					}

					retval.Tree = root_0;
					}
					}

					}
					break;

				default:
					goto loop219;
				}
			}

			loop219:
				;

			} finally { DebugExitSubRule(219); }


			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("multiplicativeExpression", 80);
			LeaveRule("multiplicativeExpression", 80);
			Leave_multiplicativeExpression();
			if (state.backtracking > 0) { Memoize(input, 80, multiplicativeExpression_StartIndex); }
		}
		DebugLocation(478, 4);
		} finally { DebugExitRule(GrammarFileName, "multiplicativeExpression"); }
		return retval;

	}
	// $ANTLR end "multiplicativeExpression"

	public class multiplicativeOperator_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_multiplicativeOperator();
	partial void Leave_multiplicativeOperator();

	// $ANTLR start "multiplicativeOperator"
	// JavaScript.g3:480:1: multiplicativeOperator : ( MUL | DIV | MOD );
	[GrammarRule("multiplicativeOperator")]
	private JavaScriptParser.multiplicativeOperator_return multiplicativeOperator()
	{
		Enter_multiplicativeOperator();
		EnterRule("multiplicativeOperator", 81);
		TraceIn("multiplicativeOperator", 81);
		JavaScriptParser.multiplicativeOperator_return retval = new JavaScriptParser.multiplicativeOperator_return();
		retval.Start = (CommonToken)input.LT(1);
		int multiplicativeOperator_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken set332=null;

		CommonTree set332_tree=null;

		try { DebugEnterRule(GrammarFileName, "multiplicativeOperator");
		DebugLocation(480, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 81)) { return retval; }
			// JavaScript.g3:481:5: ( MUL | DIV | MOD )
			DebugEnterAlt(1);
			// JavaScript.g3:
			{
			root_0 = (CommonTree)adaptor.Nil();

			DebugLocation(481, 5);
			set332=(CommonToken)input.LT(1);
			if (input.LA(1)==DIV||input.LA(1)==MOD||input.LA(1)==MUL)
			{
				input.Consume();
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, (CommonTree)adaptor.Create(set332));
				state.errorRecovery=false;state.failed=false;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return retval;}
				MismatchedSetException mse = new MismatchedSetException(null,input);
				DebugRecognitionException(mse);
				throw mse;
			}


			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("multiplicativeOperator", 81);
			LeaveRule("multiplicativeOperator", 81);
			Leave_multiplicativeOperator();
			if (state.backtracking > 0) { Memoize(input, 81, multiplicativeOperator_StartIndex); }
		}
		DebugLocation(482, 4);
		} finally { DebugExitRule(GrammarFileName, "multiplicativeOperator"); }
		return retval;

	}
	// $ANTLR end "multiplicativeOperator"

	public class unaryExpression_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_unaryExpression();
	partial void Leave_unaryExpression();

	// $ANTLR start "unaryExpression"
	// JavaScript.g3:484:1: unaryExpression : ( postfixExpression |op= prefixOperator ( SkipSpace )* unaryExpression -> ^( PREFIX $op unaryExpression ) );
	[GrammarRule("unaryExpression")]
	private JavaScriptParser.unaryExpression_return unaryExpression()
	{
		Enter_unaryExpression();
		EnterRule("unaryExpression", 82);
		TraceIn("unaryExpression", 82);
		JavaScriptParser.unaryExpression_return retval = new JavaScriptParser.unaryExpression_return();
		retval.Start = (CommonToken)input.LT(1);
		int unaryExpression_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken SkipSpace334=null;
		JavaScriptParser.prefixOperator_return op = default(JavaScriptParser.prefixOperator_return);
		JavaScriptParser.postfixExpression_return postfixExpression333 = default(JavaScriptParser.postfixExpression_return);
		JavaScriptParser.unaryExpression_return unaryExpression335 = default(JavaScriptParser.unaryExpression_return);

		CommonTree SkipSpace334_tree=null;
		RewriteRuleITokenStream stream_SkipSpace=new RewriteRuleITokenStream(adaptor,"token SkipSpace");
		RewriteRuleSubtreeStream stream_prefixOperator=new RewriteRuleSubtreeStream(adaptor,"rule prefixOperator");
		RewriteRuleSubtreeStream stream_unaryExpression=new RewriteRuleSubtreeStream(adaptor,"rule unaryExpression");
		try { DebugEnterRule(GrammarFileName, "unaryExpression");
		DebugLocation(484, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 82)) { return retval; }
			// JavaScript.g3:485:5: ( postfixExpression |op= prefixOperator ( SkipSpace )* unaryExpression -> ^( PREFIX $op unaryExpression ) )
			int alt221=2;
			try { DebugEnterDecision(221, decisionCanBacktrack[221]);
			int LA221_0 = input.LA(1);

			if ((LA221_0==BoolLiteral||LA221_0==FALSE||LA221_0==IMPORT_START||LA221_0==Identifier||LA221_0==NEW||LA221_0==NULL||LA221_0==NumericLiteral||(LA221_0>=StringLiteral && LA221_0<=THIS)||LA221_0==130||LA221_0==135||LA221_0==142||LA221_0==147))
			{
				alt221=1;
			}
			else if ((LA221_0==BIT_NOT||LA221_0==DELETE||LA221_0==DOUBLE_NOT||LA221_0==MINUS||LA221_0==MINUS_INC||LA221_0==NOT||LA221_0==PLUS||LA221_0==PLUS_INC||LA221_0==TYPE_OF||LA221_0==VOID))
			{
				alt221=2;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return retval;}
				NoViableAltException nvae = new NoViableAltException("", 221, 0, input);

				DebugRecognitionException(nvae);
				throw nvae;
			}
			} finally { DebugExitDecision(221); }
			switch (alt221)
			{
			case 1:
				DebugEnterAlt(1);
				// JavaScript.g3:485:7: postfixExpression
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(485, 7);
				PushFollow(Follow._postfixExpression_in_unaryExpression4490);
				postfixExpression333=postfixExpression();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, postfixExpression333.Tree);

				}
				break;
			case 2:
				DebugEnterAlt(2);
				// JavaScript.g3:486:7: op= prefixOperator ( SkipSpace )* unaryExpression
				{
				DebugLocation(486, 9);
				PushFollow(Follow._prefixOperator_in_unaryExpression4500);
				op=prefixOperator();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_prefixOperator.Add(op.Tree);
				DebugLocation(486, 25);
				// JavaScript.g3:486:25: ( SkipSpace )*
				try { DebugEnterSubRule(220);
				while (true)
				{
					int alt220=2;
					try { DebugEnterDecision(220, decisionCanBacktrack[220]);
					int LA220_0 = input.LA(1);

					if ((LA220_0==SkipSpace))
					{
						alt220=1;
					}


					} finally { DebugExitDecision(220); }
					switch ( alt220 )
					{
					case 1:
						DebugEnterAlt(1);
						// JavaScript.g3:486:25: SkipSpace
						{
						DebugLocation(486, 25);
						SkipSpace334=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_unaryExpression4502); if (state.failed) return retval; 
						if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace334);


						}
						break;

					default:
						goto loop220;
					}
				}

				loop220:
					;

				} finally { DebugExitSubRule(220); }

				DebugLocation(486, 36);
				PushFollow(Follow._unaryExpression_in_unaryExpression4505);
				unaryExpression335=unaryExpression();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_unaryExpression.Add(unaryExpression335.Tree);


				{
				// AST REWRITE
				// elements: op, unaryExpression
				// token labels: 
				// rule labels: op, retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.Tree = root_0;
				RewriteRuleSubtreeStream stream_op=new RewriteRuleSubtreeStream(adaptor,"rule op",op!=null?op.Tree:null);
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

				root_0 = (CommonTree)adaptor.Nil();
				// 487:9: -> ^( PREFIX $op unaryExpression )
				{
					DebugLocation(487, 12);
					// JavaScript.g3:487:12: ^( PREFIX $op unaryExpression )
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(487, 14);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(PREFIX, "PREFIX"), root_1);

					DebugLocation(487, 22);
					adaptor.AddChild(root_1, stream_op.NextTree());
					DebugLocation(487, 25);
					adaptor.AddChild(root_1, stream_unaryExpression.NextTree());

					adaptor.AddChild(root_0, root_1);
					}

				}

				retval.Tree = root_0;
				}
				}

				}
				break;

			}
			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("unaryExpression", 82);
			LeaveRule("unaryExpression", 82);
			Leave_unaryExpression();
			if (state.backtracking > 0) { Memoize(input, 82, unaryExpression_StartIndex); }
		}
		DebugLocation(488, 4);
		} finally { DebugExitRule(GrammarFileName, "unaryExpression"); }
		return retval;

	}
	// $ANTLR end "unaryExpression"

	public class prefixOperator_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_prefixOperator();
	partial void Leave_prefixOperator();

	// $ANTLR start "prefixOperator"
	// JavaScript.g3:490:1: prefixOperator : ( DELETE | VOID | TYPE_OF | PLUS_INC | MINUS_INC | PLUS | MINUS | BIT_NOT | NOT | DOUBLE_NOT );
	[GrammarRule("prefixOperator")]
	private JavaScriptParser.prefixOperator_return prefixOperator()
	{
		Enter_prefixOperator();
		EnterRule("prefixOperator", 83);
		TraceIn("prefixOperator", 83);
		JavaScriptParser.prefixOperator_return retval = new JavaScriptParser.prefixOperator_return();
		retval.Start = (CommonToken)input.LT(1);
		int prefixOperator_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken set336=null;

		CommonTree set336_tree=null;

		try { DebugEnterRule(GrammarFileName, "prefixOperator");
		DebugLocation(490, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 83)) { return retval; }
			// JavaScript.g3:491:5: ( DELETE | VOID | TYPE_OF | PLUS_INC | MINUS_INC | PLUS | MINUS | BIT_NOT | NOT | DOUBLE_NOT )
			DebugEnterAlt(1);
			// JavaScript.g3:
			{
			root_0 = (CommonTree)adaptor.Nil();

			DebugLocation(491, 5);
			set336=(CommonToken)input.LT(1);
			if (input.LA(1)==BIT_NOT||input.LA(1)==DELETE||input.LA(1)==DOUBLE_NOT||input.LA(1)==MINUS||input.LA(1)==MINUS_INC||input.LA(1)==NOT||input.LA(1)==PLUS||input.LA(1)==PLUS_INC||input.LA(1)==TYPE_OF||input.LA(1)==VOID)
			{
				input.Consume();
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, (CommonTree)adaptor.Create(set336));
				state.errorRecovery=false;state.failed=false;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return retval;}
				MismatchedSetException mse = new MismatchedSetException(null,input);
				DebugRecognitionException(mse);
				throw mse;
			}


			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("prefixOperator", 83);
			LeaveRule("prefixOperator", 83);
			Leave_prefixOperator();
			if (state.backtracking > 0) { Memoize(input, 83, prefixOperator_StartIndex); }
		}
		DebugLocation(492, 4);
		} finally { DebugExitRule(GrammarFileName, "prefixOperator"); }
		return retval;

	}
	// $ANTLR end "prefixOperator"

	public class postfixExpression_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_postfixExpression();
	partial void Leave_postfixExpression();

	// $ANTLR start "postfixExpression"
	// JavaScript.g3:494:1: postfixExpression : ( leftHandSideExpression (op1= PLUS_INC |op2= MINUS_INC ) -> ^( POSTFIX ( $op1)? ( $op2)? leftHandSideExpression ) | leftHandSideExpression );
	[GrammarRule("postfixExpression")]
	private JavaScriptParser.postfixExpression_return postfixExpression()
	{
		Enter_postfixExpression();
		EnterRule("postfixExpression", 84);
		TraceIn("postfixExpression", 84);
		JavaScriptParser.postfixExpression_return retval = new JavaScriptParser.postfixExpression_return();
		retval.Start = (CommonToken)input.LT(1);
		int postfixExpression_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken op1=null;
		CommonToken op2=null;
		JavaScriptParser.leftHandSideExpression_return leftHandSideExpression337 = default(JavaScriptParser.leftHandSideExpression_return);
		JavaScriptParser.leftHandSideExpression_return leftHandSideExpression338 = default(JavaScriptParser.leftHandSideExpression_return);

		CommonTree op1_tree=null;
		CommonTree op2_tree=null;
		RewriteRuleITokenStream stream_PLUS_INC=new RewriteRuleITokenStream(adaptor,"token PLUS_INC");
		RewriteRuleITokenStream stream_MINUS_INC=new RewriteRuleITokenStream(adaptor,"token MINUS_INC");
		RewriteRuleSubtreeStream stream_leftHandSideExpression=new RewriteRuleSubtreeStream(adaptor,"rule leftHandSideExpression");
		try { DebugEnterRule(GrammarFileName, "postfixExpression");
		DebugLocation(494, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 84)) { return retval; }
			// JavaScript.g3:495:5: ( leftHandSideExpression (op1= PLUS_INC |op2= MINUS_INC ) -> ^( POSTFIX ( $op1)? ( $op2)? leftHandSideExpression ) | leftHandSideExpression )
			int alt223=2;
			try { DebugEnterDecision(223, decisionCanBacktrack[223]);
			try
			{
				alt223 = dfa223.Predict(input);
			}
			catch (NoViableAltException nvae)
			{
				DebugRecognitionException(nvae);
				throw;
			}
			} finally { DebugExitDecision(223); }
			switch (alt223)
			{
			case 1:
				DebugEnterAlt(1);
				// JavaScript.g3:495:7: leftHandSideExpression (op1= PLUS_INC |op2= MINUS_INC )
				{
				DebugLocation(495, 7);
				PushFollow(Follow._leftHandSideExpression_in_postfixExpression4594);
				leftHandSideExpression337=leftHandSideExpression();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_leftHandSideExpression.Add(leftHandSideExpression337.Tree);
				DebugLocation(495, 30);
				// JavaScript.g3:495:30: (op1= PLUS_INC |op2= MINUS_INC )
				int alt222=2;
				try { DebugEnterSubRule(222);
				try { DebugEnterDecision(222, decisionCanBacktrack[222]);
				int LA222_0 = input.LA(1);

				if ((LA222_0==PLUS_INC))
				{
					alt222=1;
				}
				else if ((LA222_0==MINUS_INC))
				{
					alt222=2;
				}
				else
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 222, 0, input);

					DebugRecognitionException(nvae);
					throw nvae;
				}
				} finally { DebugExitDecision(222); }
				switch (alt222)
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:495:31: op1= PLUS_INC
					{
					DebugLocation(495, 34);
					op1=(CommonToken)Match(input,PLUS_INC,Follow._PLUS_INC_in_postfixExpression4599); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_PLUS_INC.Add(op1);


					}
					break;
				case 2:
					DebugEnterAlt(2);
					// JavaScript.g3:495:46: op2= MINUS_INC
					{
					DebugLocation(495, 49);
					op2=(CommonToken)Match(input,MINUS_INC,Follow._MINUS_INC_in_postfixExpression4605); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_MINUS_INC.Add(op2);


					}
					break;

				}
				} finally { DebugExitSubRule(222); }



				{
				// AST REWRITE
				// elements: op1, op2, leftHandSideExpression
				// token labels: op1, op2
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.Tree = root_0;
				RewriteRuleITokenStream stream_op1=new RewriteRuleITokenStream(adaptor,"token op1",op1);
				RewriteRuleITokenStream stream_op2=new RewriteRuleITokenStream(adaptor,"token op2",op2);
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

				root_0 = (CommonTree)adaptor.Nil();
				// 496:9: -> ^( POSTFIX ( $op1)? ( $op2)? leftHandSideExpression )
				{
					DebugLocation(496, 12);
					// JavaScript.g3:496:12: ^( POSTFIX ( $op1)? ( $op2)? leftHandSideExpression )
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(496, 14);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(POSTFIX, "POSTFIX"), root_1);

					DebugLocation(496, 23);
					// JavaScript.g3:496:23: ( $op1)?
					if ( stream_op1.HasNext )
					{
						DebugLocation(496, 23);
						adaptor.AddChild(root_1, stream_op1.NextNode());

					}
					stream_op1.Reset();
					DebugLocation(496, 29);
					// JavaScript.g3:496:29: ( $op2)?
					if ( stream_op2.HasNext )
					{
						DebugLocation(496, 29);
						adaptor.AddChild(root_1, stream_op2.NextNode());

					}
					stream_op2.Reset();
					DebugLocation(496, 34);
					adaptor.AddChild(root_1, stream_leftHandSideExpression.NextTree());

					adaptor.AddChild(root_0, root_1);
					}

				}

				retval.Tree = root_0;
				}
				}

				}
				break;
			case 2:
				DebugEnterAlt(2);
				// JavaScript.g3:497:7: leftHandSideExpression
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(497, 7);
				PushFollow(Follow._leftHandSideExpression_in_postfixExpression4638);
				leftHandSideExpression338=leftHandSideExpression();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, leftHandSideExpression338.Tree);

				}
				break;

			}
			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("postfixExpression", 84);
			LeaveRule("postfixExpression", 84);
			Leave_postfixExpression();
			if (state.backtracking > 0) { Memoize(input, 84, postfixExpression_StartIndex); }
		}
		DebugLocation(498, 4);
		} finally { DebugExitRule(GrammarFileName, "postfixExpression"); }
		return retval;

	}
	// $ANTLR end "postfixExpression"

	public class primaryExpression_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_primaryExpression();
	partial void Leave_primaryExpression();

	// $ANTLR start "primaryExpression"
	// JavaScript.g3:500:1: primaryExpression : ( THIS | Identifier | importedNotation | literal | arrayLiteral | objectLiteral | '(' ( SkipSpace )* expression ( SkipSpace )* ')' -> expression );
	[GrammarRule("primaryExpression")]
	private JavaScriptParser.primaryExpression_return primaryExpression()
	{
		Enter_primaryExpression();
		EnterRule("primaryExpression", 85);
		TraceIn("primaryExpression", 85);
		JavaScriptParser.primaryExpression_return retval = new JavaScriptParser.primaryExpression_return();
		retval.Start = (CommonToken)input.LT(1);
		int primaryExpression_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken THIS339=null;
		CommonToken Identifier340=null;
		CommonToken char_literal345=null;
		CommonToken SkipSpace346=null;
		CommonToken SkipSpace348=null;
		CommonToken char_literal349=null;
		JavaScriptParser.importedNotation_return importedNotation341 = default(JavaScriptParser.importedNotation_return);
		JavaScriptParser.literal_return literal342 = default(JavaScriptParser.literal_return);
		JavaScriptParser.arrayLiteral_return arrayLiteral343 = default(JavaScriptParser.arrayLiteral_return);
		JavaScriptParser.objectLiteral_return objectLiteral344 = default(JavaScriptParser.objectLiteral_return);
		JavaScriptParser.expression_return expression347 = default(JavaScriptParser.expression_return);

		CommonTree THIS339_tree=null;
		CommonTree Identifier340_tree=null;
		CommonTree char_literal345_tree=null;
		CommonTree SkipSpace346_tree=null;
		CommonTree SkipSpace348_tree=null;
		CommonTree char_literal349_tree=null;
		RewriteRuleITokenStream stream_130=new RewriteRuleITokenStream(adaptor,"token 130");
		RewriteRuleITokenStream stream_SkipSpace=new RewriteRuleITokenStream(adaptor,"token SkipSpace");
		RewriteRuleITokenStream stream_131=new RewriteRuleITokenStream(adaptor,"token 131");
		RewriteRuleSubtreeStream stream_expression=new RewriteRuleSubtreeStream(adaptor,"rule expression");
		try { DebugEnterRule(GrammarFileName, "primaryExpression");
		DebugLocation(500, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 85)) { return retval; }
			// JavaScript.g3:501:5: ( THIS | Identifier | importedNotation | literal | arrayLiteral | objectLiteral | '(' ( SkipSpace )* expression ( SkipSpace )* ')' -> expression )
			int alt226=7;
			try { DebugEnterDecision(226, decisionCanBacktrack[226]);
			switch (input.LA(1))
			{
			case THIS:
				{
				alt226=1;
				}
				break;
			case Identifier:
				{
				alt226=2;
				}
				break;
			case IMPORT_START:
				{
				alt226=3;
				}
				break;
			case BoolLiteral:
			case FALSE:
			case NULL:
			case NumericLiteral:
			case StringLiteral:
				{
				alt226=4;
				}
				break;
			case 135:
				{
				alt226=5;
				}
				break;
			case 147:
				{
				alt226=6;
				}
				break;
			case 130:
				{
				alt226=7;
				}
				break;
			default:
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 226, 0, input);

					DebugRecognitionException(nvae);
					throw nvae;
				}
			}

			} finally { DebugExitDecision(226); }
			switch (alt226)
			{
			case 1:
				DebugEnterAlt(1);
				// JavaScript.g3:501:7: THIS
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(501, 7);
				THIS339=(CommonToken)Match(input,THIS,Follow._THIS_in_primaryExpression4655); if (state.failed) return retval;
				if ( state.backtracking==0 ) {
				THIS339_tree = (CommonTree)adaptor.Create(THIS339);
				adaptor.AddChild(root_0, THIS339_tree);
				}

				}
				break;
			case 2:
				DebugEnterAlt(2);
				// JavaScript.g3:502:7: Identifier
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(502, 7);
				Identifier340=(CommonToken)Match(input,Identifier,Follow._Identifier_in_primaryExpression4663); if (state.failed) return retval;
				if ( state.backtracking==0 ) {
				Identifier340_tree = (CommonTree)adaptor.Create(Identifier340);
				adaptor.AddChild(root_0, Identifier340_tree);
				}

				}
				break;
			case 3:
				DebugEnterAlt(3);
				// JavaScript.g3:503:7: importedNotation
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(503, 7);
				PushFollow(Follow._importedNotation_in_primaryExpression4671);
				importedNotation341=importedNotation();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, importedNotation341.Tree);

				}
				break;
			case 4:
				DebugEnterAlt(4);
				// JavaScript.g3:504:7: literal
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(504, 7);
				PushFollow(Follow._literal_in_primaryExpression4679);
				literal342=literal();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, literal342.Tree);

				}
				break;
			case 5:
				DebugEnterAlt(5);
				// JavaScript.g3:505:7: arrayLiteral
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(505, 7);
				PushFollow(Follow._arrayLiteral_in_primaryExpression4687);
				arrayLiteral343=arrayLiteral();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, arrayLiteral343.Tree);

				}
				break;
			case 6:
				DebugEnterAlt(6);
				// JavaScript.g3:506:7: objectLiteral
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(506, 7);
				PushFollow(Follow._objectLiteral_in_primaryExpression4695);
				objectLiteral344=objectLiteral();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, objectLiteral344.Tree);

				}
				break;
			case 7:
				DebugEnterAlt(7);
				// JavaScript.g3:507:7: '(' ( SkipSpace )* expression ( SkipSpace )* ')'
				{
				DebugLocation(507, 7);
				char_literal345=(CommonToken)Match(input,130,Follow._130_in_primaryExpression4703); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_130.Add(char_literal345);

				DebugLocation(507, 11);
				// JavaScript.g3:507:11: ( SkipSpace )*
				try { DebugEnterSubRule(224);
				while (true)
				{
					int alt224=2;
					try { DebugEnterDecision(224, decisionCanBacktrack[224]);
					int LA224_0 = input.LA(1);

					if ((LA224_0==SkipSpace))
					{
						alt224=1;
					}


					} finally { DebugExitDecision(224); }
					switch ( alt224 )
					{
					case 1:
						DebugEnterAlt(1);
						// JavaScript.g3:507:11: SkipSpace
						{
						DebugLocation(507, 11);
						SkipSpace346=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_primaryExpression4705); if (state.failed) return retval; 
						if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace346);


						}
						break;

					default:
						goto loop224;
					}
				}

				loop224:
					;

				} finally { DebugExitSubRule(224); }

				DebugLocation(507, 22);
				PushFollow(Follow._expression_in_primaryExpression4708);
				expression347=expression();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_expression.Add(expression347.Tree);
				DebugLocation(507, 33);
				// JavaScript.g3:507:33: ( SkipSpace )*
				try { DebugEnterSubRule(225);
				while (true)
				{
					int alt225=2;
					try { DebugEnterDecision(225, decisionCanBacktrack[225]);
					int LA225_0 = input.LA(1);

					if ((LA225_0==SkipSpace))
					{
						alt225=1;
					}


					} finally { DebugExitDecision(225); }
					switch ( alt225 )
					{
					case 1:
						DebugEnterAlt(1);
						// JavaScript.g3:507:33: SkipSpace
						{
						DebugLocation(507, 33);
						SkipSpace348=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_primaryExpression4710); if (state.failed) return retval; 
						if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace348);


						}
						break;

					default:
						goto loop225;
					}
				}

				loop225:
					;

				} finally { DebugExitSubRule(225); }

				DebugLocation(507, 44);
				char_literal349=(CommonToken)Match(input,131,Follow._131_in_primaryExpression4713); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_131.Add(char_literal349);



				{
				// AST REWRITE
				// elements: expression
				// token labels: 
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.Tree = root_0;
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

				root_0 = (CommonTree)adaptor.Nil();
				// 508:9: -> expression
				{
					DebugLocation(508, 12);
					adaptor.AddChild(root_0, stream_expression.NextTree());

				}

				retval.Tree = root_0;
				}
				}

				}
				break;

			}
			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("primaryExpression", 85);
			LeaveRule("primaryExpression", 85);
			Leave_primaryExpression();
			if (state.backtracking > 0) { Memoize(input, 85, primaryExpression_StartIndex); }
		}
		DebugLocation(509, 4);
		} finally { DebugExitRule(GrammarFileName, "primaryExpression"); }
		return retval;

	}
	// $ANTLR end "primaryExpression"

	public class arrayLiteral_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_arrayLiteral();
	partial void Leave_arrayLiteral();

	// $ANTLR start "arrayLiteral"
	// JavaScript.g3:512:1: arrayLiteral : '[' ( SkipSpace )* ( assignmentExpression )? ( ( SkipSpace )* ',' ( ( SkipSpace )* assignmentExpression )? )* ( SkipSpace )* ']' -> ^( INLINE_ARRAY_INIT ( assignmentExpression )* ) ;
	[GrammarRule("arrayLiteral")]
	private JavaScriptParser.arrayLiteral_return arrayLiteral()
	{
		Enter_arrayLiteral();
		EnterRule("arrayLiteral", 86);
		TraceIn("arrayLiteral", 86);
		JavaScriptParser.arrayLiteral_return retval = new JavaScriptParser.arrayLiteral_return();
		retval.Start = (CommonToken)input.LT(1);
		int arrayLiteral_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken char_literal350=null;
		CommonToken SkipSpace351=null;
		CommonToken SkipSpace353=null;
		CommonToken char_literal354=null;
		CommonToken SkipSpace355=null;
		CommonToken SkipSpace357=null;
		CommonToken char_literal358=null;
		JavaScriptParser.assignmentExpression_return assignmentExpression352 = default(JavaScriptParser.assignmentExpression_return);
		JavaScriptParser.assignmentExpression_return assignmentExpression356 = default(JavaScriptParser.assignmentExpression_return);

		CommonTree char_literal350_tree=null;
		CommonTree SkipSpace351_tree=null;
		CommonTree SkipSpace353_tree=null;
		CommonTree char_literal354_tree=null;
		CommonTree SkipSpace355_tree=null;
		CommonTree SkipSpace357_tree=null;
		CommonTree char_literal358_tree=null;
		RewriteRuleITokenStream stream_135=new RewriteRuleITokenStream(adaptor,"token 135");
		RewriteRuleITokenStream stream_SkipSpace=new RewriteRuleITokenStream(adaptor,"token SkipSpace");
		RewriteRuleITokenStream stream_COMMA=new RewriteRuleITokenStream(adaptor,"token COMMA");
		RewriteRuleITokenStream stream_136=new RewriteRuleITokenStream(adaptor,"token 136");
		RewriteRuleSubtreeStream stream_assignmentExpression=new RewriteRuleSubtreeStream(adaptor,"rule assignmentExpression");
		try { DebugEnterRule(GrammarFileName, "arrayLiteral");
		DebugLocation(512, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 86)) { return retval; }
			// JavaScript.g3:513:5: ( '[' ( SkipSpace )* ( assignmentExpression )? ( ( SkipSpace )* ',' ( ( SkipSpace )* assignmentExpression )? )* ( SkipSpace )* ']' -> ^( INLINE_ARRAY_INIT ( assignmentExpression )* ) )
			DebugEnterAlt(1);
			// JavaScript.g3:513:7: '[' ( SkipSpace )* ( assignmentExpression )? ( ( SkipSpace )* ',' ( ( SkipSpace )* assignmentExpression )? )* ( SkipSpace )* ']'
			{
			DebugLocation(513, 7);
			char_literal350=(CommonToken)Match(input,135,Follow._135_in_arrayLiteral4743); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_135.Add(char_literal350);

			DebugLocation(513, 11);
			// JavaScript.g3:513:11: ( SkipSpace )*
			try { DebugEnterSubRule(227);
			while (true)
			{
				int alt227=2;
				try { DebugEnterDecision(227, decisionCanBacktrack[227]);
				int LA227_0 = input.LA(1);

				if ((LA227_0==SkipSpace))
				{
					int LA227_2 = input.LA(2);

					if ((EvaluatePredicate(synpred285_JavaScript_fragment)))
					{
						alt227=1;
					}


				}


				} finally { DebugExitDecision(227); }
				switch ( alt227 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:513:11: SkipSpace
					{
					DebugLocation(513, 11);
					SkipSpace351=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_arrayLiteral4745); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace351);


					}
					break;

				default:
					goto loop227;
				}
			}

			loop227:
				;

			} finally { DebugExitSubRule(227); }

			DebugLocation(513, 22);
			// JavaScript.g3:513:22: ( assignmentExpression )?
			int alt228=2;
			try { DebugEnterSubRule(228);
			try { DebugEnterDecision(228, decisionCanBacktrack[228]);
			int LA228_0 = input.LA(1);

			if ((LA228_0==BIT_NOT||LA228_0==BoolLiteral||LA228_0==DELETE||LA228_0==DOUBLE_NOT||LA228_0==FALSE||LA228_0==IMPORT_START||LA228_0==Identifier||LA228_0==MINUS||LA228_0==MINUS_INC||LA228_0==NEW||(LA228_0>=NOT && LA228_0<=NULL)||LA228_0==NumericLiteral||LA228_0==PLUS||LA228_0==PLUS_INC||(LA228_0>=StringLiteral && LA228_0<=THIS)||LA228_0==TYPE_OF||LA228_0==VOID||LA228_0==130||LA228_0==135||LA228_0==142||LA228_0==147))
			{
				alt228=1;
			}
			} finally { DebugExitDecision(228); }
			switch (alt228)
			{
			case 1:
				DebugEnterAlt(1);
				// JavaScript.g3:513:22: assignmentExpression
				{
				DebugLocation(513, 22);
				PushFollow(Follow._assignmentExpression_in_arrayLiteral4748);
				assignmentExpression352=assignmentExpression();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_assignmentExpression.Add(assignmentExpression352.Tree);

				}
				break;

			}
			} finally { DebugExitSubRule(228); }

			DebugLocation(513, 44);
			// JavaScript.g3:513:44: ( ( SkipSpace )* ',' ( ( SkipSpace )* assignmentExpression )? )*
			try { DebugEnterSubRule(232);
			while (true)
			{
				int alt232=2;
				try { DebugEnterDecision(232, decisionCanBacktrack[232]);
				try
				{
					alt232 = dfa232.Predict(input);
				}
				catch (NoViableAltException nvae)
				{
					DebugRecognitionException(nvae);
					throw;
				}
				} finally { DebugExitDecision(232); }
				switch ( alt232 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:513:45: ( SkipSpace )* ',' ( ( SkipSpace )* assignmentExpression )?
					{
					DebugLocation(513, 45);
					// JavaScript.g3:513:45: ( SkipSpace )*
					try { DebugEnterSubRule(229);
					while (true)
					{
						int alt229=2;
						try { DebugEnterDecision(229, decisionCanBacktrack[229]);
						int LA229_0 = input.LA(1);

						if ((LA229_0==SkipSpace))
						{
							alt229=1;
						}


						} finally { DebugExitDecision(229); }
						switch ( alt229 )
						{
						case 1:
							DebugEnterAlt(1);
							// JavaScript.g3:513:45: SkipSpace
							{
							DebugLocation(513, 45);
							SkipSpace353=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_arrayLiteral4752); if (state.failed) return retval; 
							if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace353);


							}
							break;

						default:
							goto loop229;
						}
					}

					loop229:
						;

					} finally { DebugExitSubRule(229); }

					DebugLocation(513, 56);
					char_literal354=(CommonToken)Match(input,COMMA,Follow._COMMA_in_arrayLiteral4755); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_COMMA.Add(char_literal354);

					DebugLocation(513, 60);
					// JavaScript.g3:513:60: ( ( SkipSpace )* assignmentExpression )?
					int alt231=2;
					try { DebugEnterSubRule(231);
					try { DebugEnterDecision(231, decisionCanBacktrack[231]);
					try
					{
						alt231 = dfa231.Predict(input);
					}
					catch (NoViableAltException nvae)
					{
						DebugRecognitionException(nvae);
						throw;
					}
					} finally { DebugExitDecision(231); }
					switch (alt231)
					{
					case 1:
						DebugEnterAlt(1);
						// JavaScript.g3:513:61: ( SkipSpace )* assignmentExpression
						{
						DebugLocation(513, 61);
						// JavaScript.g3:513:61: ( SkipSpace )*
						try { DebugEnterSubRule(230);
						while (true)
						{
							int alt230=2;
							try { DebugEnterDecision(230, decisionCanBacktrack[230]);
							int LA230_0 = input.LA(1);

							if ((LA230_0==SkipSpace))
							{
								alt230=1;
							}


							} finally { DebugExitDecision(230); }
							switch ( alt230 )
							{
							case 1:
								DebugEnterAlt(1);
								// JavaScript.g3:513:61: SkipSpace
								{
								DebugLocation(513, 61);
								SkipSpace355=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_arrayLiteral4758); if (state.failed) return retval; 
								if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace355);


								}
								break;

							default:
								goto loop230;
							}
						}

						loop230:
							;

						} finally { DebugExitSubRule(230); }

						DebugLocation(513, 72);
						PushFollow(Follow._assignmentExpression_in_arrayLiteral4761);
						assignmentExpression356=assignmentExpression();
						PopFollow();
						if (state.failed) return retval;
						if ( state.backtracking == 0 ) stream_assignmentExpression.Add(assignmentExpression356.Tree);

						}
						break;

					}
					} finally { DebugExitSubRule(231); }


					}
					break;

				default:
					goto loop232;
				}
			}

			loop232:
				;

			} finally { DebugExitSubRule(232); }

			DebugLocation(513, 97);
			// JavaScript.g3:513:97: ( SkipSpace )*
			try { DebugEnterSubRule(233);
			while (true)
			{
				int alt233=2;
				try { DebugEnterDecision(233, decisionCanBacktrack[233]);
				int LA233_0 = input.LA(1);

				if ((LA233_0==SkipSpace))
				{
					alt233=1;
				}


				} finally { DebugExitDecision(233); }
				switch ( alt233 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:513:97: SkipSpace
					{
					DebugLocation(513, 97);
					SkipSpace357=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_arrayLiteral4767); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace357);


					}
					break;

				default:
					goto loop233;
				}
			}

			loop233:
				;

			} finally { DebugExitSubRule(233); }

			DebugLocation(513, 108);
			char_literal358=(CommonToken)Match(input,136,Follow._136_in_arrayLiteral4770); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_136.Add(char_literal358);



			{
			// AST REWRITE
			// elements: assignmentExpression
			// token labels: 
			// rule labels: retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 514:9: -> ^( INLINE_ARRAY_INIT ( assignmentExpression )* )
			{
				DebugLocation(514, 12);
				// JavaScript.g3:514:12: ^( INLINE_ARRAY_INIT ( assignmentExpression )* )
				{
				CommonTree root_1 = (CommonTree)adaptor.Nil();
				DebugLocation(514, 14);
				root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(INLINE_ARRAY_INIT, "INLINE_ARRAY_INIT"), root_1);

				DebugLocation(514, 32);
				// JavaScript.g3:514:32: ( assignmentExpression )*
				while ( stream_assignmentExpression.HasNext )
				{
					DebugLocation(514, 32);
					adaptor.AddChild(root_1, stream_assignmentExpression.NextTree());

				}
				stream_assignmentExpression.Reset();

				adaptor.AddChild(root_0, root_1);
				}

			}

			retval.Tree = root_0;
			}
			}

			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("arrayLiteral", 86);
			LeaveRule("arrayLiteral", 86);
			Leave_arrayLiteral();
			if (state.backtracking > 0) { Memoize(input, 86, arrayLiteral_StartIndex); }
		}
		DebugLocation(515, 4);
		} finally { DebugExitRule(GrammarFileName, "arrayLiteral"); }
		return retval;

	}
	// $ANTLR end "arrayLiteral"

	public class objectLiteral_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_objectLiteral();
	partial void Leave_objectLiteral();

	// $ANTLR start "objectLiteral"
	// JavaScript.g3:518:1: objectLiteral : ( '{' ( SkipSpace )* propertyNameAndValue ( ( SkipSpace )* ',' ( SkipSpace )* pn2= propertyNameAndValue )* ( SkipSpace )* '}' -> ^( INLINE_OBJ_INIT propertyNameAndValue ( $pn2)* ) | '{' ( SkipSpace )* '}' -> ^( INLINE_OBJ_INIT ) );
	[GrammarRule("objectLiteral")]
	private JavaScriptParser.objectLiteral_return objectLiteral()
	{
		Enter_objectLiteral();
		EnterRule("objectLiteral", 87);
		TraceIn("objectLiteral", 87);
		JavaScriptParser.objectLiteral_return retval = new JavaScriptParser.objectLiteral_return();
		retval.Start = (CommonToken)input.LT(1);
		int objectLiteral_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken char_literal359=null;
		CommonToken SkipSpace360=null;
		CommonToken SkipSpace362=null;
		CommonToken char_literal363=null;
		CommonToken SkipSpace364=null;
		CommonToken SkipSpace365=null;
		CommonToken char_literal366=null;
		CommonToken char_literal367=null;
		CommonToken SkipSpace368=null;
		CommonToken char_literal369=null;
		JavaScriptParser.propertyNameAndValue_return pn2 = default(JavaScriptParser.propertyNameAndValue_return);
		JavaScriptParser.propertyNameAndValue_return propertyNameAndValue361 = default(JavaScriptParser.propertyNameAndValue_return);

		CommonTree char_literal359_tree=null;
		CommonTree SkipSpace360_tree=null;
		CommonTree SkipSpace362_tree=null;
		CommonTree char_literal363_tree=null;
		CommonTree SkipSpace364_tree=null;
		CommonTree SkipSpace365_tree=null;
		CommonTree char_literal366_tree=null;
		CommonTree char_literal367_tree=null;
		CommonTree SkipSpace368_tree=null;
		CommonTree char_literal369_tree=null;
		RewriteRuleITokenStream stream_147=new RewriteRuleITokenStream(adaptor,"token 147");
		RewriteRuleITokenStream stream_SkipSpace=new RewriteRuleITokenStream(adaptor,"token SkipSpace");
		RewriteRuleITokenStream stream_COMMA=new RewriteRuleITokenStream(adaptor,"token COMMA");
		RewriteRuleITokenStream stream_148=new RewriteRuleITokenStream(adaptor,"token 148");
		RewriteRuleSubtreeStream stream_propertyNameAndValue=new RewriteRuleSubtreeStream(adaptor,"rule propertyNameAndValue");
		try { DebugEnterRule(GrammarFileName, "objectLiteral");
		DebugLocation(518, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 87)) { return retval; }
			// JavaScript.g3:519:5: ( '{' ( SkipSpace )* propertyNameAndValue ( ( SkipSpace )* ',' ( SkipSpace )* pn2= propertyNameAndValue )* ( SkipSpace )* '}' -> ^( INLINE_OBJ_INIT propertyNameAndValue ( $pn2)* ) | '{' ( SkipSpace )* '}' -> ^( INLINE_OBJ_INIT ) )
			int alt240=2;
			try { DebugEnterDecision(240, decisionCanBacktrack[240]);
			try
			{
				alt240 = dfa240.Predict(input);
			}
			catch (NoViableAltException nvae)
			{
				DebugRecognitionException(nvae);
				throw;
			}
			} finally { DebugExitDecision(240); }
			switch (alt240)
			{
			case 1:
				DebugEnterAlt(1);
				// JavaScript.g3:519:7: '{' ( SkipSpace )* propertyNameAndValue ( ( SkipSpace )* ',' ( SkipSpace )* pn2= propertyNameAndValue )* ( SkipSpace )* '}'
				{
				DebugLocation(519, 7);
				char_literal359=(CommonToken)Match(input,147,Follow._147_in_objectLiteral4805); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_147.Add(char_literal359);

				DebugLocation(519, 11);
				// JavaScript.g3:519:11: ( SkipSpace )*
				try { DebugEnterSubRule(234);
				while (true)
				{
					int alt234=2;
					try { DebugEnterDecision(234, decisionCanBacktrack[234]);
					int LA234_0 = input.LA(1);

					if ((LA234_0==SkipSpace))
					{
						alt234=1;
					}


					} finally { DebugExitDecision(234); }
					switch ( alt234 )
					{
					case 1:
						DebugEnterAlt(1);
						// JavaScript.g3:519:11: SkipSpace
						{
						DebugLocation(519, 11);
						SkipSpace360=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_objectLiteral4807); if (state.failed) return retval; 
						if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace360);


						}
						break;

					default:
						goto loop234;
					}
				}

				loop234:
					;

				} finally { DebugExitSubRule(234); }

				DebugLocation(519, 22);
				PushFollow(Follow._propertyNameAndValue_in_objectLiteral4810);
				propertyNameAndValue361=propertyNameAndValue();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_propertyNameAndValue.Add(propertyNameAndValue361.Tree);
				DebugLocation(519, 43);
				// JavaScript.g3:519:43: ( ( SkipSpace )* ',' ( SkipSpace )* pn2= propertyNameAndValue )*
				try { DebugEnterSubRule(237);
				while (true)
				{
					int alt237=2;
					try { DebugEnterDecision(237, decisionCanBacktrack[237]);
					try
					{
						alt237 = dfa237.Predict(input);
					}
					catch (NoViableAltException nvae)
					{
						DebugRecognitionException(nvae);
						throw;
					}
					} finally { DebugExitDecision(237); }
					switch ( alt237 )
					{
					case 1:
						DebugEnterAlt(1);
						// JavaScript.g3:519:44: ( SkipSpace )* ',' ( SkipSpace )* pn2= propertyNameAndValue
						{
						DebugLocation(519, 44);
						// JavaScript.g3:519:44: ( SkipSpace )*
						try { DebugEnterSubRule(235);
						while (true)
						{
							int alt235=2;
							try { DebugEnterDecision(235, decisionCanBacktrack[235]);
							int LA235_0 = input.LA(1);

							if ((LA235_0==SkipSpace))
							{
								alt235=1;
							}


							} finally { DebugExitDecision(235); }
							switch ( alt235 )
							{
							case 1:
								DebugEnterAlt(1);
								// JavaScript.g3:519:44: SkipSpace
								{
								DebugLocation(519, 44);
								SkipSpace362=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_objectLiteral4813); if (state.failed) return retval; 
								if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace362);


								}
								break;

							default:
								goto loop235;
							}
						}

						loop235:
							;

						} finally { DebugExitSubRule(235); }

						DebugLocation(519, 55);
						char_literal363=(CommonToken)Match(input,COMMA,Follow._COMMA_in_objectLiteral4816); if (state.failed) return retval; 
						if ( state.backtracking == 0 ) stream_COMMA.Add(char_literal363);

						DebugLocation(519, 59);
						// JavaScript.g3:519:59: ( SkipSpace )*
						try { DebugEnterSubRule(236);
						while (true)
						{
							int alt236=2;
							try { DebugEnterDecision(236, decisionCanBacktrack[236]);
							int LA236_0 = input.LA(1);

							if ((LA236_0==SkipSpace))
							{
								alt236=1;
							}


							} finally { DebugExitDecision(236); }
							switch ( alt236 )
							{
							case 1:
								DebugEnterAlt(1);
								// JavaScript.g3:519:59: SkipSpace
								{
								DebugLocation(519, 59);
								SkipSpace364=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_objectLiteral4818); if (state.failed) return retval; 
								if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace364);


								}
								break;

							default:
								goto loop236;
							}
						}

						loop236:
							;

						} finally { DebugExitSubRule(236); }

						DebugLocation(519, 73);
						PushFollow(Follow._propertyNameAndValue_in_objectLiteral4823);
						pn2=propertyNameAndValue();
						PopFollow();
						if (state.failed) return retval;
						if ( state.backtracking == 0 ) stream_propertyNameAndValue.Add(pn2.Tree);

						}
						break;

					default:
						goto loop237;
					}
				}

				loop237:
					;

				} finally { DebugExitSubRule(237); }

				DebugLocation(519, 97);
				// JavaScript.g3:519:97: ( SkipSpace )*
				try { DebugEnterSubRule(238);
				while (true)
				{
					int alt238=2;
					try { DebugEnterDecision(238, decisionCanBacktrack[238]);
					int LA238_0 = input.LA(1);

					if ((LA238_0==SkipSpace))
					{
						alt238=1;
					}


					} finally { DebugExitDecision(238); }
					switch ( alt238 )
					{
					case 1:
						DebugEnterAlt(1);
						// JavaScript.g3:519:97: SkipSpace
						{
						DebugLocation(519, 97);
						SkipSpace365=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_objectLiteral4827); if (state.failed) return retval; 
						if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace365);


						}
						break;

					default:
						goto loop238;
					}
				}

				loop238:
					;

				} finally { DebugExitSubRule(238); }

				DebugLocation(519, 108);
				char_literal366=(CommonToken)Match(input,148,Follow._148_in_objectLiteral4830); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_148.Add(char_literal366);



				{
				// AST REWRITE
				// elements: propertyNameAndValue, pn2
				// token labels: 
				// rule labels: pn2, retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.Tree = root_0;
				RewriteRuleSubtreeStream stream_pn2=new RewriteRuleSubtreeStream(adaptor,"rule pn2",pn2!=null?pn2.Tree:null);
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

				root_0 = (CommonTree)adaptor.Nil();
				// 520:9: -> ^( INLINE_OBJ_INIT propertyNameAndValue ( $pn2)* )
				{
					DebugLocation(520, 12);
					// JavaScript.g3:520:12: ^( INLINE_OBJ_INIT propertyNameAndValue ( $pn2)* )
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(520, 14);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(INLINE_OBJ_INIT, "INLINE_OBJ_INIT"), root_1);

					DebugLocation(520, 30);
					adaptor.AddChild(root_1, stream_propertyNameAndValue.NextTree());
					DebugLocation(520, 52);
					// JavaScript.g3:520:52: ( $pn2)*
					while ( stream_pn2.HasNext )
					{
						DebugLocation(520, 52);
						adaptor.AddChild(root_1, stream_pn2.NextTree());

					}
					stream_pn2.Reset();

					adaptor.AddChild(root_0, root_1);
					}

				}

				retval.Tree = root_0;
				}
				}

				}
				break;
			case 2:
				DebugEnterAlt(2);
				// JavaScript.g3:521:7: '{' ( SkipSpace )* '}'
				{
				DebugLocation(521, 7);
				char_literal367=(CommonToken)Match(input,147,Follow._147_in_objectLiteral4858); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_147.Add(char_literal367);

				DebugLocation(521, 11);
				// JavaScript.g3:521:11: ( SkipSpace )*
				try { DebugEnterSubRule(239);
				while (true)
				{
					int alt239=2;
					try { DebugEnterDecision(239, decisionCanBacktrack[239]);
					int LA239_0 = input.LA(1);

					if ((LA239_0==SkipSpace))
					{
						alt239=1;
					}


					} finally { DebugExitDecision(239); }
					switch ( alt239 )
					{
					case 1:
						DebugEnterAlt(1);
						// JavaScript.g3:521:11: SkipSpace
						{
						DebugLocation(521, 11);
						SkipSpace368=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_objectLiteral4860); if (state.failed) return retval; 
						if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace368);


						}
						break;

					default:
						goto loop239;
					}
				}

				loop239:
					;

				} finally { DebugExitSubRule(239); }

				DebugLocation(521, 22);
				char_literal369=(CommonToken)Match(input,148,Follow._148_in_objectLiteral4863); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_148.Add(char_literal369);



				{
				// AST REWRITE
				// elements: 
				// token labels: 
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.Tree = root_0;
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

				root_0 = (CommonTree)adaptor.Nil();
				// 521:26: -> ^( INLINE_OBJ_INIT )
				{
					DebugLocation(521, 29);
					// JavaScript.g3:521:29: ^( INLINE_OBJ_INIT )
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(521, 31);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(INLINE_OBJ_INIT, "INLINE_OBJ_INIT"), root_1);

					adaptor.AddChild(root_0, root_1);
					}

				}

				retval.Tree = root_0;
				}
				}

				}
				break;

			}
			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("objectLiteral", 87);
			LeaveRule("objectLiteral", 87);
			Leave_objectLiteral();
			if (state.backtracking > 0) { Memoize(input, 87, objectLiteral_StartIndex); }
		}
		DebugLocation(522, 4);
		} finally { DebugExitRule(GrammarFileName, "objectLiteral"); }
		return retval;

	}
	// $ANTLR end "objectLiteral"

	public class propertyInitializers_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_propertyInitializers();
	partial void Leave_propertyInitializers();

	// $ANTLR start "propertyInitializers"
	// JavaScript.g3:524:1: propertyInitializers : propertyNameAndValue ( ( SkipSpace )* ',' propertyInitializers ) ;
	[GrammarRule("propertyInitializers")]
	private JavaScriptParser.propertyInitializers_return propertyInitializers()
	{
		Enter_propertyInitializers();
		EnterRule("propertyInitializers", 88);
		TraceIn("propertyInitializers", 88);
		JavaScriptParser.propertyInitializers_return retval = new JavaScriptParser.propertyInitializers_return();
		retval.Start = (CommonToken)input.LT(1);
		int propertyInitializers_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken SkipSpace371=null;
		CommonToken char_literal372=null;
		JavaScriptParser.propertyNameAndValue_return propertyNameAndValue370 = default(JavaScriptParser.propertyNameAndValue_return);
		JavaScriptParser.propertyInitializers_return propertyInitializers373 = default(JavaScriptParser.propertyInitializers_return);

		CommonTree SkipSpace371_tree=null;
		CommonTree char_literal372_tree=null;

		try { DebugEnterRule(GrammarFileName, "propertyInitializers");
		DebugLocation(524, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 88)) { return retval; }
			// JavaScript.g3:525:5: ( propertyNameAndValue ( ( SkipSpace )* ',' propertyInitializers ) )
			DebugEnterAlt(1);
			// JavaScript.g3:525:7: propertyNameAndValue ( ( SkipSpace )* ',' propertyInitializers )
			{
			root_0 = (CommonTree)adaptor.Nil();

			DebugLocation(525, 7);
			PushFollow(Follow._propertyNameAndValue_in_propertyInitializers4886);
			propertyNameAndValue370=propertyNameAndValue();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) adaptor.AddChild(root_0, propertyNameAndValue370.Tree);
			DebugLocation(525, 28);
			// JavaScript.g3:525:28: ( ( SkipSpace )* ',' propertyInitializers )
			DebugEnterAlt(1);
			// JavaScript.g3:525:29: ( SkipSpace )* ',' propertyInitializers
			{
			DebugLocation(525, 29);
			// JavaScript.g3:525:29: ( SkipSpace )*
			try { DebugEnterSubRule(241);
			while (true)
			{
				int alt241=2;
				try { DebugEnterDecision(241, decisionCanBacktrack[241]);
				int LA241_0 = input.LA(1);

				if ((LA241_0==SkipSpace))
				{
					alt241=1;
				}


				} finally { DebugExitDecision(241); }
				switch ( alt241 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:525:29: SkipSpace
					{
					DebugLocation(525, 29);
					SkipSpace371=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_propertyInitializers4889); if (state.failed) return retval;
					if ( state.backtracking==0 ) {
					SkipSpace371_tree = (CommonTree)adaptor.Create(SkipSpace371);
					adaptor.AddChild(root_0, SkipSpace371_tree);
					}

					}
					break;

				default:
					goto loop241;
				}
			}

			loop241:
				;

			} finally { DebugExitSubRule(241); }

			DebugLocation(525, 40);
			char_literal372=(CommonToken)Match(input,COMMA,Follow._COMMA_in_propertyInitializers4892); if (state.failed) return retval;
			if ( state.backtracking==0 ) {
			char_literal372_tree = (CommonTree)adaptor.Create(char_literal372);
			adaptor.AddChild(root_0, char_literal372_tree);
			}
			DebugLocation(525, 44);
			PushFollow(Follow._propertyInitializers_in_propertyInitializers4894);
			propertyInitializers373=propertyInitializers();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) adaptor.AddChild(root_0, propertyInitializers373.Tree);

			}


			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("propertyInitializers", 88);
			LeaveRule("propertyInitializers", 88);
			Leave_propertyInitializers();
			if (state.backtracking > 0) { Memoize(input, 88, propertyInitializers_StartIndex); }
		}
		DebugLocation(526, 4);
		} finally { DebugExitRule(GrammarFileName, "propertyInitializers"); }
		return retval;

	}
	// $ANTLR end "propertyInitializers"

	public class propertyNameAndValue_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_propertyNameAndValue();
	partial void Leave_propertyNameAndValue();

	// $ANTLR start "propertyNameAndValue"
	// JavaScript.g3:528:1: propertyNameAndValue : propName= propertyName ( SkipSpace )* ':' ( SkipSpace )* assignment= assignmentExpression -> ^( PROP_NAME_VALUE $propName $assignment) ;
	[GrammarRule("propertyNameAndValue")]
	private JavaScriptParser.propertyNameAndValue_return propertyNameAndValue()
	{
		Enter_propertyNameAndValue();
		EnterRule("propertyNameAndValue", 89);
		TraceIn("propertyNameAndValue", 89);
		JavaScriptParser.propertyNameAndValue_return retval = new JavaScriptParser.propertyNameAndValue_return();
		retval.Start = (CommonToken)input.LT(1);
		int propertyNameAndValue_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken SkipSpace374=null;
		CommonToken char_literal375=null;
		CommonToken SkipSpace376=null;
		JavaScriptParser.propertyName_return propName = default(JavaScriptParser.propertyName_return);
		JavaScriptParser.assignmentExpression_return assignment = default(JavaScriptParser.assignmentExpression_return);

		CommonTree SkipSpace374_tree=null;
		CommonTree char_literal375_tree=null;
		CommonTree SkipSpace376_tree=null;
		RewriteRuleITokenStream stream_SkipSpace=new RewriteRuleITokenStream(adaptor,"token SkipSpace");
		RewriteRuleITokenStream stream_133=new RewriteRuleITokenStream(adaptor,"token 133");
		RewriteRuleSubtreeStream stream_propertyName=new RewriteRuleSubtreeStream(adaptor,"rule propertyName");
		RewriteRuleSubtreeStream stream_assignmentExpression=new RewriteRuleSubtreeStream(adaptor,"rule assignmentExpression");
		try { DebugEnterRule(GrammarFileName, "propertyNameAndValue");
		DebugLocation(528, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 89)) { return retval; }
			// JavaScript.g3:529:5: (propName= propertyName ( SkipSpace )* ':' ( SkipSpace )* assignment= assignmentExpression -> ^( PROP_NAME_VALUE $propName $assignment) )
			DebugEnterAlt(1);
			// JavaScript.g3:529:7: propName= propertyName ( SkipSpace )* ':' ( SkipSpace )* assignment= assignmentExpression
			{
			DebugLocation(529, 15);
			PushFollow(Follow._propertyName_in_propertyNameAndValue4914);
			propName=propertyName();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_propertyName.Add(propName.Tree);
			DebugLocation(529, 29);
			// JavaScript.g3:529:29: ( SkipSpace )*
			try { DebugEnterSubRule(242);
			while (true)
			{
				int alt242=2;
				try { DebugEnterDecision(242, decisionCanBacktrack[242]);
				int LA242_0 = input.LA(1);

				if ((LA242_0==SkipSpace))
				{
					alt242=1;
				}


				} finally { DebugExitDecision(242); }
				switch ( alt242 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:529:29: SkipSpace
					{
					DebugLocation(529, 29);
					SkipSpace374=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_propertyNameAndValue4916); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace374);


					}
					break;

				default:
					goto loop242;
				}
			}

			loop242:
				;

			} finally { DebugExitSubRule(242); }

			DebugLocation(529, 40);
			char_literal375=(CommonToken)Match(input,133,Follow._133_in_propertyNameAndValue4919); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_133.Add(char_literal375);

			DebugLocation(529, 44);
			// JavaScript.g3:529:44: ( SkipSpace )*
			try { DebugEnterSubRule(243);
			while (true)
			{
				int alt243=2;
				try { DebugEnterDecision(243, decisionCanBacktrack[243]);
				int LA243_0 = input.LA(1);

				if ((LA243_0==SkipSpace))
				{
					alt243=1;
				}


				} finally { DebugExitDecision(243); }
				switch ( alt243 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:529:44: SkipSpace
					{
					DebugLocation(529, 44);
					SkipSpace376=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_propertyNameAndValue4921); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace376);


					}
					break;

				default:
					goto loop243;
				}
			}

			loop243:
				;

			} finally { DebugExitSubRule(243); }

			DebugLocation(529, 65);
			PushFollow(Follow._assignmentExpression_in_propertyNameAndValue4926);
			assignment=assignmentExpression();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_assignmentExpression.Add(assignment.Tree);


			{
			// AST REWRITE
			// elements: propName, assignment
			// token labels: 
			// rule labels: propName, assignment, retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_propName=new RewriteRuleSubtreeStream(adaptor,"rule propName",propName!=null?propName.Tree:null);
			RewriteRuleSubtreeStream stream_assignment=new RewriteRuleSubtreeStream(adaptor,"rule assignment",assignment!=null?assignment.Tree:null);
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 529:87: -> ^( PROP_NAME_VALUE $propName $assignment)
			{
				DebugLocation(529, 90);
				// JavaScript.g3:529:90: ^( PROP_NAME_VALUE $propName $assignment)
				{
				CommonTree root_1 = (CommonTree)adaptor.Nil();
				DebugLocation(529, 92);
				root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(PROP_NAME_VALUE, "PROP_NAME_VALUE"), root_1);

				DebugLocation(529, 109);
				adaptor.AddChild(root_1, stream_propName.NextTree());
				DebugLocation(529, 119);
				adaptor.AddChild(root_1, stream_assignment.NextTree());

				adaptor.AddChild(root_0, root_1);
				}

			}

			retval.Tree = root_0;
			}
			}

			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("propertyNameAndValue", 89);
			LeaveRule("propertyNameAndValue", 89);
			Leave_propertyNameAndValue();
			if (state.backtracking > 0) { Memoize(input, 89, propertyNameAndValue_StartIndex); }
		}
		DebugLocation(530, 4);
		} finally { DebugExitRule(GrammarFileName, "propertyNameAndValue"); }
		return retval;

	}
	// $ANTLR end "propertyNameAndValue"

	public class propertyName_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_propertyName();
	partial void Leave_propertyName();

	// $ANTLR start "propertyName"
	// JavaScript.g3:532:1: propertyName : ( Identifier | StringLiteral | NumericLiteral );
	[GrammarRule("propertyName")]
	private JavaScriptParser.propertyName_return propertyName()
	{
		Enter_propertyName();
		EnterRule("propertyName", 90);
		TraceIn("propertyName", 90);
		JavaScriptParser.propertyName_return retval = new JavaScriptParser.propertyName_return();
		retval.Start = (CommonToken)input.LT(1);
		int propertyName_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken set377=null;

		CommonTree set377_tree=null;

		try { DebugEnterRule(GrammarFileName, "propertyName");
		DebugLocation(532, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 90)) { return retval; }
			// JavaScript.g3:533:5: ( Identifier | StringLiteral | NumericLiteral )
			DebugEnterAlt(1);
			// JavaScript.g3:
			{
			root_0 = (CommonTree)adaptor.Nil();

			DebugLocation(533, 5);
			set377=(CommonToken)input.LT(1);
			if (input.LA(1)==Identifier||input.LA(1)==NumericLiteral||input.LA(1)==StringLiteral)
			{
				input.Consume();
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, (CommonTree)adaptor.Create(set377));
				state.errorRecovery=false;state.failed=false;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return retval;}
				MismatchedSetException mse = new MismatchedSetException(null,input);
				DebugRecognitionException(mse);
				throw mse;
			}


			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("propertyName", 90);
			LeaveRule("propertyName", 90);
			Leave_propertyName();
			if (state.backtracking > 0) { Memoize(input, 90, propertyName_StartIndex); }
		}
		DebugLocation(536, 4);
		} finally { DebugExitRule(GrammarFileName, "propertyName"); }
		return retval;

	}
	// $ANTLR end "propertyName"

	public class literal_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_literal();
	partial void Leave_literal();

	// $ANTLR start "literal"
	// JavaScript.g3:539:1: literal : ( NULL | BoolLiteral | FALSE | StringLiteral | NumericLiteral );
	[GrammarRule("literal")]
	private JavaScriptParser.literal_return literal()
	{
		Enter_literal();
		EnterRule("literal", 91);
		TraceIn("literal", 91);
		JavaScriptParser.literal_return retval = new JavaScriptParser.literal_return();
		retval.Start = (CommonToken)input.LT(1);
		int literal_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken set378=null;

		CommonTree set378_tree=null;

		try { DebugEnterRule(GrammarFileName, "literal");
		DebugLocation(539, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 91)) { return retval; }
			// JavaScript.g3:540:5: ( NULL | BoolLiteral | FALSE | StringLiteral | NumericLiteral )
			DebugEnterAlt(1);
			// JavaScript.g3:
			{
			root_0 = (CommonTree)adaptor.Nil();

			DebugLocation(540, 5);
			set378=(CommonToken)input.LT(1);
			if (input.LA(1)==BoolLiteral||input.LA(1)==FALSE||input.LA(1)==NULL||input.LA(1)==NumericLiteral||input.LA(1)==StringLiteral)
			{
				input.Consume();
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, (CommonTree)adaptor.Create(set378));
				state.errorRecovery=false;state.failed=false;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return retval;}
				MismatchedSetException mse = new MismatchedSetException(null,input);
				DebugRecognitionException(mse);
				throw mse;
			}


			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("literal", 91);
			LeaveRule("literal", 91);
			Leave_literal();
			if (state.backtracking > 0) { Memoize(input, 91, literal_StartIndex); }
		}
		DebugLocation(545, 4);
		} finally { DebugExitRule(GrammarFileName, "literal"); }
		return retval;

	}
	// $ANTLR end "literal"

	public class importedNotation_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_importedNotation();
	partial void Leave_importedNotation();

	// $ANTLR start "importedNotation"
	// JavaScript.g3:547:1: importedNotation : ( IMPORT_START importedTypeName '}' | IMPORT_START type= importedTypeName TYPE_SEPERATOR ident= Identifier '}' -> ^( TYPE_SEPERATOR $type $ident) | IMPORT_START type= importedTypeName TYPE_SEPERATOR ident= Identifier args= importedMethodArguments '}' -> ^( TYPE_SEPERATOR $type $ident $args) );
	[GrammarRule("importedNotation")]
	private JavaScriptParser.importedNotation_return importedNotation()
	{
		Enter_importedNotation();
		EnterRule("importedNotation", 92);
		TraceIn("importedNotation", 92);
		JavaScriptParser.importedNotation_return retval = new JavaScriptParser.importedNotation_return();
		retval.Start = (CommonToken)input.LT(1);
		int importedNotation_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken ident=null;
		CommonToken IMPORT_START379=null;
		CommonToken char_literal381=null;
		CommonToken IMPORT_START382=null;
		CommonToken TYPE_SEPERATOR383=null;
		CommonToken char_literal384=null;
		CommonToken IMPORT_START385=null;
		CommonToken TYPE_SEPERATOR386=null;
		CommonToken char_literal387=null;
		JavaScriptParser.importedTypeName_return type = default(JavaScriptParser.importedTypeName_return);
		JavaScriptParser.importedMethodArguments_return args = default(JavaScriptParser.importedMethodArguments_return);
		JavaScriptParser.importedTypeName_return importedTypeName380 = default(JavaScriptParser.importedTypeName_return);

		CommonTree ident_tree=null;
		CommonTree IMPORT_START379_tree=null;
		CommonTree char_literal381_tree=null;
		CommonTree IMPORT_START382_tree=null;
		CommonTree TYPE_SEPERATOR383_tree=null;
		CommonTree char_literal384_tree=null;
		CommonTree IMPORT_START385_tree=null;
		CommonTree TYPE_SEPERATOR386_tree=null;
		CommonTree char_literal387_tree=null;
		RewriteRuleITokenStream stream_IMPORT_START=new RewriteRuleITokenStream(adaptor,"token IMPORT_START");
		RewriteRuleITokenStream stream_TYPE_SEPERATOR=new RewriteRuleITokenStream(adaptor,"token TYPE_SEPERATOR");
		RewriteRuleITokenStream stream_Identifier=new RewriteRuleITokenStream(adaptor,"token Identifier");
		RewriteRuleITokenStream stream_148=new RewriteRuleITokenStream(adaptor,"token 148");
		RewriteRuleSubtreeStream stream_importedTypeName=new RewriteRuleSubtreeStream(adaptor,"rule importedTypeName");
		RewriteRuleSubtreeStream stream_importedMethodArguments=new RewriteRuleSubtreeStream(adaptor,"rule importedMethodArguments");
		try { DebugEnterRule(GrammarFileName, "importedNotation");
		DebugLocation(547, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 92)) { return retval; }
			// JavaScript.g3:548:5: ( IMPORT_START importedTypeName '}' | IMPORT_START type= importedTypeName TYPE_SEPERATOR ident= Identifier '}' -> ^( TYPE_SEPERATOR $type $ident) | IMPORT_START type= importedTypeName TYPE_SEPERATOR ident= Identifier args= importedMethodArguments '}' -> ^( TYPE_SEPERATOR $type $ident $args) )
			int alt244=3;
			try { DebugEnterDecision(244, decisionCanBacktrack[244]);
			try
			{
				alt244 = dfa244.Predict(input);
			}
			catch (NoViableAltException nvae)
			{
				DebugRecognitionException(nvae);
				throw;
			}
			} finally { DebugExitDecision(244); }
			switch (alt244)
			{
			case 1:
				DebugEnterAlt(1);
				// JavaScript.g3:548:7: IMPORT_START importedTypeName '}'
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(548, 19);
				IMPORT_START379=(CommonToken)Match(input,IMPORT_START,Follow._IMPORT_START_in_importedNotation5038); if (state.failed) return retval;
				DebugLocation(548, 21);
				PushFollow(Follow._importedTypeName_in_importedNotation5041);
				importedTypeName380=importedTypeName();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, importedTypeName380.Tree);
				DebugLocation(548, 41);
				char_literal381=(CommonToken)Match(input,148,Follow._148_in_importedNotation5043); if (state.failed) return retval;

				}
				break;
			case 2:
				DebugEnterAlt(2);
				// JavaScript.g3:549:7: IMPORT_START type= importedTypeName TYPE_SEPERATOR ident= Identifier '}'
				{
				DebugLocation(549, 7);
				IMPORT_START382=(CommonToken)Match(input,IMPORT_START,Follow._IMPORT_START_in_importedNotation5052); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_IMPORT_START.Add(IMPORT_START382);

				DebugLocation(549, 24);
				PushFollow(Follow._importedTypeName_in_importedNotation5056);
				type=importedTypeName();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_importedTypeName.Add(type.Tree);
				DebugLocation(549, 42);
				TYPE_SEPERATOR383=(CommonToken)Match(input,TYPE_SEPERATOR,Follow._TYPE_SEPERATOR_in_importedNotation5058); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_TYPE_SEPERATOR.Add(TYPE_SEPERATOR383);

				DebugLocation(549, 62);
				ident=(CommonToken)Match(input,Identifier,Follow._Identifier_in_importedNotation5062); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_Identifier.Add(ident);

				DebugLocation(549, 74);
				char_literal384=(CommonToken)Match(input,148,Follow._148_in_importedNotation5064); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_148.Add(char_literal384);



				{
				// AST REWRITE
				// elements: TYPE_SEPERATOR, type, ident
				// token labels: ident
				// rule labels: type, retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.Tree = root_0;
				RewriteRuleITokenStream stream_ident=new RewriteRuleITokenStream(adaptor,"token ident",ident);
				RewriteRuleSubtreeStream stream_type=new RewriteRuleSubtreeStream(adaptor,"rule type",type!=null?type.Tree:null);
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

				root_0 = (CommonTree)adaptor.Nil();
				// 550:9: -> ^( TYPE_SEPERATOR $type $ident)
				{
					DebugLocation(550, 12);
					// JavaScript.g3:550:12: ^( TYPE_SEPERATOR $type $ident)
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(550, 14);
					root_1 = (CommonTree)adaptor.BecomeRoot(stream_TYPE_SEPERATOR.NextNode(), root_1);

					DebugLocation(550, 30);
					adaptor.AddChild(root_1, stream_type.NextTree());
					DebugLocation(550, 36);
					adaptor.AddChild(root_1, stream_ident.NextNode());

					adaptor.AddChild(root_0, root_1);
					}

				}

				retval.Tree = root_0;
				}
				}

				}
				break;
			case 3:
				DebugEnterAlt(3);
				// JavaScript.g3:551:7: IMPORT_START type= importedTypeName TYPE_SEPERATOR ident= Identifier args= importedMethodArguments '}'
				{
				DebugLocation(551, 7);
				IMPORT_START385=(CommonToken)Match(input,IMPORT_START,Follow._IMPORT_START_in_importedNotation5092); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_IMPORT_START.Add(IMPORT_START385);

				DebugLocation(551, 24);
				PushFollow(Follow._importedTypeName_in_importedNotation5096);
				type=importedTypeName();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_importedTypeName.Add(type.Tree);
				DebugLocation(551, 42);
				TYPE_SEPERATOR386=(CommonToken)Match(input,TYPE_SEPERATOR,Follow._TYPE_SEPERATOR_in_importedNotation5098); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_TYPE_SEPERATOR.Add(TYPE_SEPERATOR386);

				DebugLocation(551, 62);
				ident=(CommonToken)Match(input,Identifier,Follow._Identifier_in_importedNotation5102); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_Identifier.Add(ident);

				DebugLocation(551, 78);
				PushFollow(Follow._importedMethodArguments_in_importedNotation5106);
				args=importedMethodArguments();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_importedMethodArguments.Add(args.Tree);
				DebugLocation(551, 103);
				char_literal387=(CommonToken)Match(input,148,Follow._148_in_importedNotation5108); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_148.Add(char_literal387);



				{
				// AST REWRITE
				// elements: TYPE_SEPERATOR, type, ident, args
				// token labels: ident
				// rule labels: type, args, retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.Tree = root_0;
				RewriteRuleITokenStream stream_ident=new RewriteRuleITokenStream(adaptor,"token ident",ident);
				RewriteRuleSubtreeStream stream_type=new RewriteRuleSubtreeStream(adaptor,"rule type",type!=null?type.Tree:null);
				RewriteRuleSubtreeStream stream_args=new RewriteRuleSubtreeStream(adaptor,"rule args",args!=null?args.Tree:null);
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

				root_0 = (CommonTree)adaptor.Nil();
				// 552:9: -> ^( TYPE_SEPERATOR $type $ident $args)
				{
					DebugLocation(552, 12);
					// JavaScript.g3:552:12: ^( TYPE_SEPERATOR $type $ident $args)
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(552, 14);
					root_1 = (CommonTree)adaptor.BecomeRoot(stream_TYPE_SEPERATOR.NextNode(), root_1);

					DebugLocation(552, 30);
					adaptor.AddChild(root_1, stream_type.NextTree());
					DebugLocation(552, 36);
					adaptor.AddChild(root_1, stream_ident.NextNode());
					DebugLocation(552, 43);
					adaptor.AddChild(root_1, stream_args.NextTree());

					adaptor.AddChild(root_0, root_1);
					}

				}

				retval.Tree = root_0;
				}
				}

				}
				break;

			}
			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("importedNotation", 92);
			LeaveRule("importedNotation", 92);
			Leave_importedNotation();
			if (state.backtracking > 0) { Memoize(input, 92, importedNotation_StartIndex); }
		}
		DebugLocation(553, 4);
		} finally { DebugExitRule(GrammarFileName, "importedNotation"); }
		return retval;

	}
	// $ANTLR end "importedNotation"

	public class importedMethodArguments_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_importedMethodArguments();
	partial void Leave_importedMethodArguments();

	// $ANTLR start "importedMethodArguments"
	// JavaScript.g3:555:1: importedMethodArguments : '(' ( ( SkipSpace )* importedTypeName ( ( SkipSpace )* ',' ( SkipSpace )* importedTypeName )* )? ( SkipSpace )* ')' -> ^( ARGS ( importedTypeName )* ) ;
	[GrammarRule("importedMethodArguments")]
	private JavaScriptParser.importedMethodArguments_return importedMethodArguments()
	{
		Enter_importedMethodArguments();
		EnterRule("importedMethodArguments", 93);
		TraceIn("importedMethodArguments", 93);
		JavaScriptParser.importedMethodArguments_return retval = new JavaScriptParser.importedMethodArguments_return();
		retval.Start = (CommonToken)input.LT(1);
		int importedMethodArguments_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken char_literal388=null;
		CommonToken SkipSpace389=null;
		CommonToken SkipSpace391=null;
		CommonToken char_literal392=null;
		CommonToken SkipSpace393=null;
		CommonToken SkipSpace395=null;
		CommonToken char_literal396=null;
		JavaScriptParser.importedTypeName_return importedTypeName390 = default(JavaScriptParser.importedTypeName_return);
		JavaScriptParser.importedTypeName_return importedTypeName394 = default(JavaScriptParser.importedTypeName_return);

		CommonTree char_literal388_tree=null;
		CommonTree SkipSpace389_tree=null;
		CommonTree SkipSpace391_tree=null;
		CommonTree char_literal392_tree=null;
		CommonTree SkipSpace393_tree=null;
		CommonTree SkipSpace395_tree=null;
		CommonTree char_literal396_tree=null;
		RewriteRuleITokenStream stream_130=new RewriteRuleITokenStream(adaptor,"token 130");
		RewriteRuleITokenStream stream_SkipSpace=new RewriteRuleITokenStream(adaptor,"token SkipSpace");
		RewriteRuleITokenStream stream_COMMA=new RewriteRuleITokenStream(adaptor,"token COMMA");
		RewriteRuleITokenStream stream_131=new RewriteRuleITokenStream(adaptor,"token 131");
		RewriteRuleSubtreeStream stream_importedTypeName=new RewriteRuleSubtreeStream(adaptor,"rule importedTypeName");
		try { DebugEnterRule(GrammarFileName, "importedMethodArguments");
		DebugLocation(555, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 93)) { return retval; }
			// JavaScript.g3:556:5: ( '(' ( ( SkipSpace )* importedTypeName ( ( SkipSpace )* ',' ( SkipSpace )* importedTypeName )* )? ( SkipSpace )* ')' -> ^( ARGS ( importedTypeName )* ) )
			DebugEnterAlt(1);
			// JavaScript.g3:556:7: '(' ( ( SkipSpace )* importedTypeName ( ( SkipSpace )* ',' ( SkipSpace )* importedTypeName )* )? ( SkipSpace )* ')'
			{
			DebugLocation(556, 7);
			char_literal388=(CommonToken)Match(input,130,Follow._130_in_importedMethodArguments5148); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_130.Add(char_literal388);

			DebugLocation(556, 11);
			// JavaScript.g3:556:11: ( ( SkipSpace )* importedTypeName ( ( SkipSpace )* ',' ( SkipSpace )* importedTypeName )* )?
			int alt249=2;
			try { DebugEnterSubRule(249);
			try { DebugEnterDecision(249, decisionCanBacktrack[249]);
			try
			{
				alt249 = dfa249.Predict(input);
			}
			catch (NoViableAltException nvae)
			{
				DebugRecognitionException(nvae);
				throw;
			}
			} finally { DebugExitDecision(249); }
			switch (alt249)
			{
			case 1:
				DebugEnterAlt(1);
				// JavaScript.g3:556:12: ( SkipSpace )* importedTypeName ( ( SkipSpace )* ',' ( SkipSpace )* importedTypeName )*
				{
				DebugLocation(556, 12);
				// JavaScript.g3:556:12: ( SkipSpace )*
				try { DebugEnterSubRule(245);
				while (true)
				{
					int alt245=2;
					try { DebugEnterDecision(245, decisionCanBacktrack[245]);
					int LA245_0 = input.LA(1);

					if ((LA245_0==SkipSpace))
					{
						alt245=1;
					}


					} finally { DebugExitDecision(245); }
					switch ( alt245 )
					{
					case 1:
						DebugEnterAlt(1);
						// JavaScript.g3:556:12: SkipSpace
						{
						DebugLocation(556, 12);
						SkipSpace389=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_importedMethodArguments5151); if (state.failed) return retval; 
						if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace389);


						}
						break;

					default:
						goto loop245;
					}
				}

				loop245:
					;

				} finally { DebugExitSubRule(245); }

				DebugLocation(556, 23);
				PushFollow(Follow._importedTypeName_in_importedMethodArguments5154);
				importedTypeName390=importedTypeName();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_importedTypeName.Add(importedTypeName390.Tree);
				DebugLocation(556, 40);
				// JavaScript.g3:556:40: ( ( SkipSpace )* ',' ( SkipSpace )* importedTypeName )*
				try { DebugEnterSubRule(248);
				while (true)
				{
					int alt248=2;
					try { DebugEnterDecision(248, decisionCanBacktrack[248]);
					try
					{
						alt248 = dfa248.Predict(input);
					}
					catch (NoViableAltException nvae)
					{
						DebugRecognitionException(nvae);
						throw;
					}
					} finally { DebugExitDecision(248); }
					switch ( alt248 )
					{
					case 1:
						DebugEnterAlt(1);
						// JavaScript.g3:556:41: ( SkipSpace )* ',' ( SkipSpace )* importedTypeName
						{
						DebugLocation(556, 41);
						// JavaScript.g3:556:41: ( SkipSpace )*
						try { DebugEnterSubRule(246);
						while (true)
						{
							int alt246=2;
							try { DebugEnterDecision(246, decisionCanBacktrack[246]);
							int LA246_0 = input.LA(1);

							if ((LA246_0==SkipSpace))
							{
								alt246=1;
							}


							} finally { DebugExitDecision(246); }
							switch ( alt246 )
							{
							case 1:
								DebugEnterAlt(1);
								// JavaScript.g3:556:41: SkipSpace
								{
								DebugLocation(556, 41);
								SkipSpace391=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_importedMethodArguments5157); if (state.failed) return retval; 
								if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace391);


								}
								break;

							default:
								goto loop246;
							}
						}

						loop246:
							;

						} finally { DebugExitSubRule(246); }

						DebugLocation(556, 52);
						char_literal392=(CommonToken)Match(input,COMMA,Follow._COMMA_in_importedMethodArguments5160); if (state.failed) return retval; 
						if ( state.backtracking == 0 ) stream_COMMA.Add(char_literal392);

						DebugLocation(556, 56);
						// JavaScript.g3:556:56: ( SkipSpace )*
						try { DebugEnterSubRule(247);
						while (true)
						{
							int alt247=2;
							try { DebugEnterDecision(247, decisionCanBacktrack[247]);
							int LA247_0 = input.LA(1);

							if ((LA247_0==SkipSpace))
							{
								alt247=1;
							}


							} finally { DebugExitDecision(247); }
							switch ( alt247 )
							{
							case 1:
								DebugEnterAlt(1);
								// JavaScript.g3:556:56: SkipSpace
								{
								DebugLocation(556, 56);
								SkipSpace393=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_importedMethodArguments5162); if (state.failed) return retval; 
								if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace393);


								}
								break;

							default:
								goto loop247;
							}
						}

						loop247:
							;

						} finally { DebugExitSubRule(247); }

						DebugLocation(556, 67);
						PushFollow(Follow._importedTypeName_in_importedMethodArguments5165);
						importedTypeName394=importedTypeName();
						PopFollow();
						if (state.failed) return retval;
						if ( state.backtracking == 0 ) stream_importedTypeName.Add(importedTypeName394.Tree);

						}
						break;

					default:
						goto loop248;
					}
				}

				loop248:
					;

				} finally { DebugExitSubRule(248); }


				}
				break;

			}
			} finally { DebugExitSubRule(249); }

			DebugLocation(556, 88);
			// JavaScript.g3:556:88: ( SkipSpace )*
			try { DebugEnterSubRule(250);
			while (true)
			{
				int alt250=2;
				try { DebugEnterDecision(250, decisionCanBacktrack[250]);
				int LA250_0 = input.LA(1);

				if ((LA250_0==SkipSpace))
				{
					alt250=1;
				}


				} finally { DebugExitDecision(250); }
				switch ( alt250 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:556:88: SkipSpace
					{
					DebugLocation(556, 88);
					SkipSpace395=(CommonToken)Match(input,SkipSpace,Follow._SkipSpace_in_importedMethodArguments5171); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_SkipSpace.Add(SkipSpace395);


					}
					break;

				default:
					goto loop250;
				}
			}

			loop250:
				;

			} finally { DebugExitSubRule(250); }

			DebugLocation(556, 99);
			char_literal396=(CommonToken)Match(input,131,Follow._131_in_importedMethodArguments5174); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_131.Add(char_literal396);



			{
			// AST REWRITE
			// elements: importedTypeName
			// token labels: 
			// rule labels: retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 557:9: -> ^( ARGS ( importedTypeName )* )
			{
				DebugLocation(557, 12);
				// JavaScript.g3:557:12: ^( ARGS ( importedTypeName )* )
				{
				CommonTree root_1 = (CommonTree)adaptor.Nil();
				DebugLocation(557, 14);
				root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(ARGS, "ARGS"), root_1);

				DebugLocation(557, 19);
				// JavaScript.g3:557:19: ( importedTypeName )*
				while ( stream_importedTypeName.HasNext )
				{
					DebugLocation(557, 19);
					adaptor.AddChild(root_1, stream_importedTypeName.NextTree());

				}
				stream_importedTypeName.Reset();

				adaptor.AddChild(root_0, root_1);
				}

			}

			retval.Tree = root_0;
			}
			}

			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("importedMethodArguments", 93);
			LeaveRule("importedMethodArguments", 93);
			Leave_importedMethodArguments();
			if (state.backtracking > 0) { Memoize(input, 93, importedMethodArguments_StartIndex); }
		}
		DebugLocation(558, 4);
		} finally { DebugExitRule(GrammarFileName, "importedMethodArguments"); }
		return retval;

	}
	// $ANTLR end "importedMethodArguments"

	public class importedTypeName_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_importedTypeName();
	partial void Leave_importedTypeName();

	// $ANTLR start "importedTypeName"
	// JavaScript.g3:560:1: importedTypeName : ( '[' assem= importedTypeNamePart ']' type= importedTypeNamePart -> ^( TYPENAME $assem $type) | Identifier -> ^( TYPENAME Identifier ) | GenericSignature -> ^( TYPENAME GenericSignature ) );
	[GrammarRule("importedTypeName")]
	private JavaScriptParser.importedTypeName_return importedTypeName()
	{
		Enter_importedTypeName();
		EnterRule("importedTypeName", 94);
		TraceIn("importedTypeName", 94);
		JavaScriptParser.importedTypeName_return retval = new JavaScriptParser.importedTypeName_return();
		retval.Start = (CommonToken)input.LT(1);
		int importedTypeName_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken char_literal397=null;
		CommonToken char_literal398=null;
		CommonToken Identifier399=null;
		CommonToken GenericSignature400=null;
		JavaScriptParser.importedTypeNamePart_return assem = default(JavaScriptParser.importedTypeNamePart_return);
		JavaScriptParser.importedTypeNamePart_return type = default(JavaScriptParser.importedTypeNamePart_return);

		CommonTree char_literal397_tree=null;
		CommonTree char_literal398_tree=null;
		CommonTree Identifier399_tree=null;
		CommonTree GenericSignature400_tree=null;
		RewriteRuleITokenStream stream_135=new RewriteRuleITokenStream(adaptor,"token 135");
		RewriteRuleITokenStream stream_136=new RewriteRuleITokenStream(adaptor,"token 136");
		RewriteRuleITokenStream stream_Identifier=new RewriteRuleITokenStream(adaptor,"token Identifier");
		RewriteRuleITokenStream stream_GenericSignature=new RewriteRuleITokenStream(adaptor,"token GenericSignature");
		RewriteRuleSubtreeStream stream_importedTypeNamePart=new RewriteRuleSubtreeStream(adaptor,"rule importedTypeNamePart");
		try { DebugEnterRule(GrammarFileName, "importedTypeName");
		DebugLocation(560, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 94)) { return retval; }
			// JavaScript.g3:561:5: ( '[' assem= importedTypeNamePart ']' type= importedTypeNamePart -> ^( TYPENAME $assem $type) | Identifier -> ^( TYPENAME Identifier ) | GenericSignature -> ^( TYPENAME GenericSignature ) )
			int alt251=3;
			try { DebugEnterDecision(251, decisionCanBacktrack[251]);
			switch (input.LA(1))
			{
			case 135:
				{
				alt251=1;
				}
				break;
			case Identifier:
				{
				alt251=2;
				}
				break;
			case GenericSignature:
				{
				alt251=3;
				}
				break;
			default:
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 251, 0, input);

					DebugRecognitionException(nvae);
					throw nvae;
				}
			}

			} finally { DebugExitDecision(251); }
			switch (alt251)
			{
			case 1:
				DebugEnterAlt(1);
				// JavaScript.g3:561:7: '[' assem= importedTypeNamePart ']' type= importedTypeNamePart
				{
				DebugLocation(561, 7);
				char_literal397=(CommonToken)Match(input,135,Follow._135_in_importedTypeName5208); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_135.Add(char_literal397);

				DebugLocation(561, 16);
				PushFollow(Follow._importedTypeNamePart_in_importedTypeName5212);
				assem=importedTypeNamePart();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_importedTypeNamePart.Add(assem.Tree);
				DebugLocation(561, 38);
				char_literal398=(CommonToken)Match(input,136,Follow._136_in_importedTypeName5214); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_136.Add(char_literal398);

				DebugLocation(561, 46);
				PushFollow(Follow._importedTypeNamePart_in_importedTypeName5218);
				type=importedTypeNamePart();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_importedTypeNamePart.Add(type.Tree);


				{
				// AST REWRITE
				// elements: assem, type
				// token labels: 
				// rule labels: assem, type, retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.Tree = root_0;
				RewriteRuleSubtreeStream stream_assem=new RewriteRuleSubtreeStream(adaptor,"rule assem",assem!=null?assem.Tree:null);
				RewriteRuleSubtreeStream stream_type=new RewriteRuleSubtreeStream(adaptor,"rule type",type!=null?type.Tree:null);
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

				root_0 = (CommonTree)adaptor.Nil();
				// 561:68: -> ^( TYPENAME $assem $type)
				{
					DebugLocation(561, 71);
					// JavaScript.g3:561:71: ^( TYPENAME $assem $type)
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(561, 73);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(TYPENAME, "TYPENAME"), root_1);

					DebugLocation(561, 83);
					adaptor.AddChild(root_1, stream_assem.NextTree());
					DebugLocation(561, 90);
					adaptor.AddChild(root_1, stream_type.NextTree());

					adaptor.AddChild(root_0, root_1);
					}

				}

				retval.Tree = root_0;
				}
				}

				}
				break;
			case 2:
				DebugEnterAlt(2);
				// JavaScript.g3:562:7: Identifier
				{
				DebugLocation(562, 7);
				Identifier399=(CommonToken)Match(input,Identifier,Follow._Identifier_in_importedTypeName5238); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_Identifier.Add(Identifier399);



				{
				// AST REWRITE
				// elements: Identifier
				// token labels: 
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.Tree = root_0;
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

				root_0 = (CommonTree)adaptor.Nil();
				// 562:18: -> ^( TYPENAME Identifier )
				{
					DebugLocation(562, 21);
					// JavaScript.g3:562:21: ^( TYPENAME Identifier )
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(562, 23);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(TYPENAME, "TYPENAME"), root_1);

					DebugLocation(562, 32);
					adaptor.AddChild(root_1, stream_Identifier.NextNode());

					adaptor.AddChild(root_0, root_1);
					}

				}

				retval.Tree = root_0;
				}
				}

				}
				break;
			case 3:
				DebugEnterAlt(3);
				// JavaScript.g3:563:7: GenericSignature
				{
				DebugLocation(563, 7);
				GenericSignature400=(CommonToken)Match(input,GenericSignature,Follow._GenericSignature_in_importedTypeName5254); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_GenericSignature.Add(GenericSignature400);



				{
				// AST REWRITE
				// elements: GenericSignature
				// token labels: 
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.Tree = root_0;
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

				root_0 = (CommonTree)adaptor.Nil();
				// 563:24: -> ^( TYPENAME GenericSignature )
				{
					DebugLocation(563, 27);
					// JavaScript.g3:563:27: ^( TYPENAME GenericSignature )
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(563, 29);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(TYPENAME, "TYPENAME"), root_1);

					DebugLocation(563, 38);
					adaptor.AddChild(root_1, stream_GenericSignature.NextNode());

					adaptor.AddChild(root_0, root_1);
					}

				}

				retval.Tree = root_0;
				}
				}

				}
				break;

			}
			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("importedTypeName", 94);
			LeaveRule("importedTypeName", 94);
			Leave_importedTypeName();
			if (state.backtracking > 0) { Memoize(input, 94, importedTypeName_StartIndex); }
		}
		DebugLocation(564, 4);
		} finally { DebugExitRule(GrammarFileName, "importedTypeName"); }
		return retval;

	}
	// $ANTLR end "importedTypeName"

	public class importedAssemblyNamePart_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_importedAssemblyNamePart();
	partial void Leave_importedAssemblyNamePart();

	// $ANTLR start "importedAssemblyNamePart"
	// JavaScript.g3:566:1: importedAssemblyNamePart : Identifier ( '.' Identifier )* -> ^( TYPEPART ( Identifier )* ) ;
	[GrammarRule("importedAssemblyNamePart")]
	private JavaScriptParser.importedAssemblyNamePart_return importedAssemblyNamePart()
	{
		Enter_importedAssemblyNamePart();
		EnterRule("importedAssemblyNamePart", 95);
		TraceIn("importedAssemblyNamePart", 95);
		JavaScriptParser.importedAssemblyNamePart_return retval = new JavaScriptParser.importedAssemblyNamePart_return();
		retval.Start = (CommonToken)input.LT(1);
		int importedAssemblyNamePart_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken Identifier401=null;
		CommonToken char_literal402=null;
		CommonToken Identifier403=null;

		CommonTree Identifier401_tree=null;
		CommonTree char_literal402_tree=null;
		CommonTree Identifier403_tree=null;
		RewriteRuleITokenStream stream_Identifier=new RewriteRuleITokenStream(adaptor,"token Identifier");
		RewriteRuleITokenStream stream_132=new RewriteRuleITokenStream(adaptor,"token 132");

		try { DebugEnterRule(GrammarFileName, "importedAssemblyNamePart");
		DebugLocation(566, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 95)) { return retval; }
			// JavaScript.g3:567:5: ( Identifier ( '.' Identifier )* -> ^( TYPEPART ( Identifier )* ) )
			DebugEnterAlt(1);
			// JavaScript.g3:567:7: Identifier ( '.' Identifier )*
			{
			DebugLocation(567, 7);
			Identifier401=(CommonToken)Match(input,Identifier,Follow._Identifier_in_importedAssemblyNamePart5279); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_Identifier.Add(Identifier401);

			DebugLocation(567, 18);
			// JavaScript.g3:567:18: ( '.' Identifier )*
			try { DebugEnterSubRule(252);
			while (true)
			{
				int alt252=2;
				try { DebugEnterDecision(252, decisionCanBacktrack[252]);
				int LA252_0 = input.LA(1);

				if ((LA252_0==132))
				{
					alt252=1;
				}


				} finally { DebugExitDecision(252); }
				switch ( alt252 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:567:19: '.' Identifier
					{
					DebugLocation(567, 19);
					char_literal402=(CommonToken)Match(input,132,Follow._132_in_importedAssemblyNamePart5282); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_132.Add(char_literal402);

					DebugLocation(567, 23);
					Identifier403=(CommonToken)Match(input,Identifier,Follow._Identifier_in_importedAssemblyNamePart5284); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_Identifier.Add(Identifier403);


					}
					break;

				default:
					goto loop252;
				}
			}

			loop252:
				;

			} finally { DebugExitSubRule(252); }



			{
			// AST REWRITE
			// elements: Identifier
			// token labels: 
			// rule labels: retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 568:9: -> ^( TYPEPART ( Identifier )* )
			{
				DebugLocation(568, 12);
				// JavaScript.g3:568:12: ^( TYPEPART ( Identifier )* )
				{
				CommonTree root_1 = (CommonTree)adaptor.Nil();
				DebugLocation(568, 14);
				root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(TYPEPART, "TYPEPART"), root_1);

				DebugLocation(568, 23);
				// JavaScript.g3:568:23: ( Identifier )*
				while ( stream_Identifier.HasNext )
				{
					DebugLocation(568, 23);
					adaptor.AddChild(root_1, stream_Identifier.NextNode());

				}
				stream_Identifier.Reset();

				adaptor.AddChild(root_0, root_1);
				}

			}

			retval.Tree = root_0;
			}
			}

			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("importedAssemblyNamePart", 95);
			LeaveRule("importedAssemblyNamePart", 95);
			Leave_importedAssemblyNamePart();
			if (state.backtracking > 0) { Memoize(input, 95, importedAssemblyNamePart_StartIndex); }
		}
		DebugLocation(569, 4);
		} finally { DebugExitRule(GrammarFileName, "importedAssemblyNamePart"); }
		return retval;

	}
	// $ANTLR end "importedAssemblyNamePart"

	public class importedTypeNamePart_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_importedTypeNamePart();
	partial void Leave_importedTypeNamePart();

	// $ANTLR start "importedTypeNamePart"
	// JavaScript.g3:571:1: importedTypeNamePart : Identifier ( '.' Identifier )* ( '.' TypeNameIdentifier )? -> ^( TYPEPART ( Identifier )* ( TypeNameIdentifier )? ) ;
	[GrammarRule("importedTypeNamePart")]
	private JavaScriptParser.importedTypeNamePart_return importedTypeNamePart()
	{
		Enter_importedTypeNamePart();
		EnterRule("importedTypeNamePart", 96);
		TraceIn("importedTypeNamePart", 96);
		JavaScriptParser.importedTypeNamePart_return retval = new JavaScriptParser.importedTypeNamePart_return();
		retval.Start = (CommonToken)input.LT(1);
		int importedTypeNamePart_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken Identifier404=null;
		CommonToken char_literal405=null;
		CommonToken Identifier406=null;
		CommonToken char_literal407=null;
		CommonToken TypeNameIdentifier408=null;

		CommonTree Identifier404_tree=null;
		CommonTree char_literal405_tree=null;
		CommonTree Identifier406_tree=null;
		CommonTree char_literal407_tree=null;
		CommonTree TypeNameIdentifier408_tree=null;
		RewriteRuleITokenStream stream_Identifier=new RewriteRuleITokenStream(adaptor,"token Identifier");
		RewriteRuleITokenStream stream_132=new RewriteRuleITokenStream(adaptor,"token 132");
		RewriteRuleITokenStream stream_TypeNameIdentifier=new RewriteRuleITokenStream(adaptor,"token TypeNameIdentifier");

		try { DebugEnterRule(GrammarFileName, "importedTypeNamePart");
		DebugLocation(571, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 96)) { return retval; }
			// JavaScript.g3:572:5: ( Identifier ( '.' Identifier )* ( '.' TypeNameIdentifier )? -> ^( TYPEPART ( Identifier )* ( TypeNameIdentifier )? ) )
			DebugEnterAlt(1);
			// JavaScript.g3:572:7: Identifier ( '.' Identifier )* ( '.' TypeNameIdentifier )?
			{
			DebugLocation(572, 7);
			Identifier404=(CommonToken)Match(input,Identifier,Follow._Identifier_in_importedTypeNamePart5320); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_Identifier.Add(Identifier404);

			DebugLocation(572, 18);
			// JavaScript.g3:572:18: ( '.' Identifier )*
			try { DebugEnterSubRule(253);
			while (true)
			{
				int alt253=2;
				try { DebugEnterDecision(253, decisionCanBacktrack[253]);
				int LA253_0 = input.LA(1);

				if ((LA253_0==132))
				{
					int LA253_1 = input.LA(2);

					if ((LA253_1==Identifier))
					{
						alt253=1;
					}


				}


				} finally { DebugExitDecision(253); }
				switch ( alt253 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:572:19: '.' Identifier
					{
					DebugLocation(572, 19);
					char_literal405=(CommonToken)Match(input,132,Follow._132_in_importedTypeNamePart5323); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_132.Add(char_literal405);

					DebugLocation(572, 23);
					Identifier406=(CommonToken)Match(input,Identifier,Follow._Identifier_in_importedTypeNamePart5325); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_Identifier.Add(Identifier406);


					}
					break;

				default:
					goto loop253;
				}
			}

			loop253:
				;

			} finally { DebugExitSubRule(253); }

			DebugLocation(572, 36);
			// JavaScript.g3:572:36: ( '.' TypeNameIdentifier )?
			int alt254=2;
			try { DebugEnterSubRule(254);
			try { DebugEnterDecision(254, decisionCanBacktrack[254]);
			int LA254_0 = input.LA(1);

			if ((LA254_0==132))
			{
				alt254=1;
			}
			} finally { DebugExitDecision(254); }
			switch (alt254)
			{
			case 1:
				DebugEnterAlt(1);
				// JavaScript.g3:572:37: '.' TypeNameIdentifier
				{
				DebugLocation(572, 37);
				char_literal407=(CommonToken)Match(input,132,Follow._132_in_importedTypeNamePart5330); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_132.Add(char_literal407);

				DebugLocation(572, 41);
				TypeNameIdentifier408=(CommonToken)Match(input,TypeNameIdentifier,Follow._TypeNameIdentifier_in_importedTypeNamePart5332); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_TypeNameIdentifier.Add(TypeNameIdentifier408);


				}
				break;

			}
			} finally { DebugExitSubRule(254); }



			{
			// AST REWRITE
			// elements: Identifier, TypeNameIdentifier
			// token labels: 
			// rule labels: retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 573:9: -> ^( TYPEPART ( Identifier )* ( TypeNameIdentifier )? )
			{
				DebugLocation(573, 12);
				// JavaScript.g3:573:12: ^( TYPEPART ( Identifier )* ( TypeNameIdentifier )? )
				{
				CommonTree root_1 = (CommonTree)adaptor.Nil();
				DebugLocation(573, 14);
				root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(TYPEPART, "TYPEPART"), root_1);

				DebugLocation(573, 23);
				// JavaScript.g3:573:23: ( Identifier )*
				while ( stream_Identifier.HasNext )
				{
					DebugLocation(573, 23);
					adaptor.AddChild(root_1, stream_Identifier.NextNode());

				}
				stream_Identifier.Reset();
				DebugLocation(573, 35);
				// JavaScript.g3:573:35: ( TypeNameIdentifier )?
				if ( stream_TypeNameIdentifier.HasNext )
				{
					DebugLocation(573, 35);
					adaptor.AddChild(root_1, stream_TypeNameIdentifier.NextNode());

				}
				stream_TypeNameIdentifier.Reset();

				adaptor.AddChild(root_0, root_1);
				}

			}

			retval.Tree = root_0;
			}
			}

			}

			retval.Stop = (CommonToken)input.LT(-1);

			if ( state.backtracking == 0 ) {

			retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
			adaptor.SetTokenBoundaries(retval.Tree, retval.Start, retval.Stop);
			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		retval.Tree = (CommonTree)adaptor.ErrorNode(input, retval.Start, input.LT(-1), re);

		}
		finally
		{
			TraceOut("importedTypeNamePart", 96);
			LeaveRule("importedTypeNamePart", 96);
			Leave_importedTypeNamePart();
			if (state.backtracking > 0) { Memoize(input, 96, importedTypeNamePart_StartIndex); }
		}
		DebugLocation(574, 4);
		} finally { DebugExitRule(GrammarFileName, "importedTypeNamePart"); }
		return retval;

	}
	// $ANTLR end "importedTypeNamePart"

	partial void Enter_synpred5_JavaScript_fragment();
	partial void Leave_synpred5_JavaScript_fragment();

	// $ANTLR start synpred5_JavaScript
	public void synpred5_JavaScript_fragment()
	{
		Enter_synpred5_JavaScript_fragment();
		EnterRule("synpred5_JavaScript_fragment", 101);
		TraceIn("synpred5_JavaScript_fragment", 101);
		try
		{
			// JavaScript.g3:74:7: ( functionDeclaration )
			DebugEnterAlt(1);
			// JavaScript.g3:74:7: functionDeclaration
			{
			DebugLocation(74, 7);
			PushFollow(Follow._functionDeclaration_in_synpred5_JavaScript421);
			functionDeclaration();
			PopFollow();
			if (state.failed) return;

			}

		}
		finally
		{
			TraceOut("synpred5_JavaScript_fragment", 101);
			LeaveRule("synpred5_JavaScript_fragment", 101);
			Leave_synpred5_JavaScript_fragment();
		}
	}
	// $ANTLR end synpred5_JavaScript

	partial void Enter_synpred9_JavaScript_fragment();
	partial void Leave_synpred9_JavaScript_fragment();

	// $ANTLR start synpred9_JavaScript
	public void synpred9_JavaScript_fragment()
	{
		Enter_synpred9_JavaScript_fragment();
		EnterRule("synpred9_JavaScript_fragment", 105);
		TraceIn("synpred9_JavaScript_fragment", 105);
		try
		{
			// JavaScript.g3:85:18: ( SkipSpace )
			DebugEnterAlt(1);
			// JavaScript.g3:85:18: SkipSpace
			{
			DebugLocation(85, 18);
			Match(input,SkipSpace,Follow._SkipSpace_in_synpred9_JavaScript510); if (state.failed) return;

			}

		}
		finally
		{
			TraceOut("synpred9_JavaScript_fragment", 105);
			LeaveRule("synpred9_JavaScript_fragment", 105);
			Leave_synpred9_JavaScript_fragment();
		}
	}
	// $ANTLR end synpred9_JavaScript

	partial void Enter_synpred21_JavaScript_fragment();
	partial void Leave_synpred21_JavaScript_fragment();

	// $ANTLR start synpred21_JavaScript
	public void synpred21_JavaScript_fragment()
	{
		Enter_synpred21_JavaScript_fragment();
		EnterRule("synpred21_JavaScript_fragment", 117);
		TraceIn("synpred21_JavaScript_fragment", 117);
		try
		{
			// JavaScript.g3:101:7: ( statementBlock )
			DebugEnterAlt(1);
			// JavaScript.g3:101:7: statementBlock
			{
			DebugLocation(101, 7);
			PushFollow(Follow._statementBlock_in_synpred21_JavaScript678);
			statementBlock();
			PopFollow();
			if (state.failed) return;

			}

		}
		finally
		{
			TraceOut("synpred21_JavaScript_fragment", 117);
			LeaveRule("synpred21_JavaScript_fragment", 117);
			Leave_synpred21_JavaScript_fragment();
		}
	}
	// $ANTLR end synpred21_JavaScript

	partial void Enter_synpred24_JavaScript_fragment();
	partial void Leave_synpred24_JavaScript_fragment();

	// $ANTLR start synpred24_JavaScript
	public void synpred24_JavaScript_fragment()
	{
		Enter_synpred24_JavaScript_fragment();
		EnterRule("synpred24_JavaScript_fragment", 120);
		TraceIn("synpred24_JavaScript_fragment", 120);
		try
		{
			// JavaScript.g3:104:7: ( expressionStatement )
			DebugEnterAlt(1);
			// JavaScript.g3:104:7: expressionStatement
			{
			DebugLocation(104, 7);
			PushFollow(Follow._expressionStatement_in_synpred24_JavaScript702);
			expressionStatement();
			PopFollow();
			if (state.failed) return;

			}

		}
		finally
		{
			TraceOut("synpred24_JavaScript_fragment", 120);
			LeaveRule("synpred24_JavaScript_fragment", 120);
			Leave_synpred24_JavaScript_fragment();
		}
	}
	// $ANTLR end synpred24_JavaScript

	partial void Enter_synpred31_JavaScript_fragment();
	partial void Leave_synpred31_JavaScript_fragment();

	// $ANTLR start synpred31_JavaScript
	public void synpred31_JavaScript_fragment()
	{
		Enter_synpred31_JavaScript_fragment();
		EnterRule("synpred31_JavaScript_fragment", 127);
		TraceIn("synpred31_JavaScript_fragment", 127);
		try
		{
			// JavaScript.g3:111:7: ( labelledStatement )
			DebugEnterAlt(1);
			// JavaScript.g3:111:7: labelledStatement
			{
			DebugLocation(111, 7);
			PushFollow(Follow._labelledStatement_in_synpred31_JavaScript758);
			labelledStatement();
			PopFollow();
			if (state.failed) return;

			}

		}
		finally
		{
			TraceOut("synpred31_JavaScript_fragment", 127);
			LeaveRule("synpred31_JavaScript_fragment", 127);
			Leave_synpred31_JavaScript_fragment();
		}
	}
	// $ANTLR end synpred31_JavaScript

	partial void Enter_synpred34_JavaScript_fragment();
	partial void Leave_synpred34_JavaScript_fragment();

	// $ANTLR start synpred34_JavaScript
	public void synpred34_JavaScript_fragment()
	{
		Enter_synpred34_JavaScript_fragment();
		EnterRule("synpred34_JavaScript_fragment", 130);
		TraceIn("synpred34_JavaScript_fragment", 130);
		try
		{
			// JavaScript.g3:118:11: ( SkipSpace )
			DebugEnterAlt(1);
			// JavaScript.g3:118:11: SkipSpace
			{
			DebugLocation(118, 11);
			Match(input,SkipSpace,Follow._SkipSpace_in_synpred34_JavaScript801); if (state.failed) return;

			}

		}
		finally
		{
			TraceOut("synpred34_JavaScript_fragment", 130);
			LeaveRule("synpred34_JavaScript_fragment", 130);
			Leave_synpred34_JavaScript_fragment();
		}
	}
	// $ANTLR end synpred34_JavaScript

	partial void Enter_synpred60_JavaScript_fragment();
	partial void Leave_synpred60_JavaScript_fragment();

	// $ANTLR start synpred60_JavaScript
	public void synpred60_JavaScript_fragment()
	{
		JavaScriptParser.statement_return els = default(JavaScriptParser.statement_return);

		Enter_synpred60_JavaScript_fragment();
		EnterRule("synpred60_JavaScript_fragment", 156);
		TraceIn("synpred60_JavaScript_fragment", 156);
		try
		{
			// JavaScript.g3:164:95: ( ( SkipSpace )* 'else' ( SkipSpace )* els= statement )
			DebugEnterAlt(1);
			// JavaScript.g3:164:95: ( SkipSpace )* 'else' ( SkipSpace )* els= statement
			{
			DebugLocation(164, 95);
			// JavaScript.g3:164:95: ( SkipSpace )*
			try { DebugEnterSubRule(267);
			while (true)
			{
				int alt267=2;
				try { DebugEnterDecision(267, decisionCanBacktrack[267]);
				int LA267_0 = input.LA(1);

				if ((LA267_0==SkipSpace))
				{
					alt267=1;
				}


				} finally { DebugExitDecision(267); }
				switch ( alt267 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:164:95: SkipSpace
					{
					DebugLocation(164, 95);
					Match(input,SkipSpace,Follow._SkipSpace_in_synpred60_JavaScript1200); if (state.failed) return;

					}
					break;

				default:
					goto loop267;
				}
			}

			loop267:
				;

			} finally { DebugExitSubRule(267); }

			DebugLocation(164, 106);
			Match(input,140,Follow._140_in_synpred60_JavaScript1203); if (state.failed) return;
			DebugLocation(164, 113);
			// JavaScript.g3:164:113: ( SkipSpace )*
			try { DebugEnterSubRule(268);
			while (true)
			{
				int alt268=2;
				try { DebugEnterDecision(268, decisionCanBacktrack[268]);
				int LA268_0 = input.LA(1);

				if ((LA268_0==SkipSpace))
				{
					alt268=1;
				}


				} finally { DebugExitDecision(268); }
				switch ( alt268 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:164:113: SkipSpace
					{
					DebugLocation(164, 113);
					Match(input,SkipSpace,Follow._SkipSpace_in_synpred60_JavaScript1205); if (state.failed) return;

					}
					break;

				default:
					goto loop268;
				}
			}

			loop268:
				;

			} finally { DebugExitSubRule(268); }

			DebugLocation(164, 127);
			PushFollow(Follow._statement_in_synpred60_JavaScript1210);
			els=statement();
			PopFollow();
			if (state.failed) return;

			}

		}
		finally
		{
			TraceOut("synpred60_JavaScript_fragment", 156);
			LeaveRule("synpred60_JavaScript_fragment", 156);
			Leave_synpred60_JavaScript_fragment();
		}
	}
	// $ANTLR end synpred60_JavaScript

	partial void Enter_synpred63_JavaScript_fragment();
	partial void Leave_synpred63_JavaScript_fragment();

	// $ANTLR start synpred63_JavaScript
	public void synpred63_JavaScript_fragment()
	{
		Enter_synpred63_JavaScript_fragment();
		EnterRule("synpred63_JavaScript_fragment", 159);
		TraceIn("synpred63_JavaScript_fragment", 159);
		try
		{
			// JavaScript.g3:171:7: ( forStatement )
			DebugEnterAlt(1);
			// JavaScript.g3:171:7: forStatement
			{
			DebugLocation(171, 7);
			PushFollow(Follow._forStatement_in_synpred63_JavaScript1269);
			forStatement();
			PopFollow();
			if (state.failed) return;

			}

		}
		finally
		{
			TraceOut("synpred63_JavaScript_fragment", 159);
			LeaveRule("synpred63_JavaScript_fragment", 159);
			Leave_synpred63_JavaScript_fragment();
		}
	}
	// $ANTLR end synpred63_JavaScript

	partial void Enter_synpred73_JavaScript_fragment();
	partial void Leave_synpred73_JavaScript_fragment();

	// $ANTLR start synpred73_JavaScript
	public void synpred73_JavaScript_fragment()
	{
		Enter_synpred73_JavaScript_fragment();
		EnterRule("synpred73_JavaScript_fragment", 169);
		TraceIn("synpred73_JavaScript_fragment", 169);
		try
		{
			// JavaScript.g3:186:28: ( SkipSpace )
			DebugEnterAlt(1);
			// JavaScript.g3:186:28: SkipSpace
			{
			DebugLocation(186, 28);
			Match(input,SkipSpace,Follow._SkipSpace_in_synpred73_JavaScript1427); if (state.failed) return;

			}

		}
		finally
		{
			TraceOut("synpred73_JavaScript_fragment", 169);
			LeaveRule("synpred73_JavaScript_fragment", 169);
			Leave_synpred73_JavaScript_fragment();
		}
	}
	// $ANTLR end synpred73_JavaScript

	partial void Enter_synpred76_JavaScript_fragment();
	partial void Leave_synpred76_JavaScript_fragment();

	// $ANTLR start synpred76_JavaScript
	public void synpred76_JavaScript_fragment()
	{
		Enter_synpred76_JavaScript_fragment();
		EnterRule("synpred76_JavaScript_fragment", 172);
		TraceIn("synpred76_JavaScript_fragment", 172);
		try
		{
			// JavaScript.g3:186:93: ( SkipSpace )
			DebugEnterAlt(1);
			// JavaScript.g3:186:93: SkipSpace
			{
			DebugLocation(186, 93);
			Match(input,SkipSpace,Follow._SkipSpace_in_synpred76_JavaScript1444); if (state.failed) return;

			}

		}
		finally
		{
			TraceOut("synpred76_JavaScript_fragment", 172);
			LeaveRule("synpred76_JavaScript_fragment", 172);
			Leave_synpred76_JavaScript_fragment();
		}
	}
	// $ANTLR end synpred76_JavaScript

	partial void Enter_synpred79_JavaScript_fragment();
	partial void Leave_synpred79_JavaScript_fragment();

	// $ANTLR start synpred79_JavaScript
	public void synpred79_JavaScript_fragment()
	{
		Enter_synpred79_JavaScript_fragment();
		EnterRule("synpred79_JavaScript_fragment", 175);
		TraceIn("synpred79_JavaScript_fragment", 175);
		try
		{
			// JavaScript.g3:186:148: ( SkipSpace )
			DebugEnterAlt(1);
			// JavaScript.g3:186:148: SkipSpace
			{
			DebugLocation(186, 148);
			Match(input,SkipSpace,Follow._SkipSpace_in_synpred79_JavaScript1461); if (state.failed) return;

			}

		}
		finally
		{
			TraceOut("synpred79_JavaScript_fragment", 175);
			LeaveRule("synpred79_JavaScript_fragment", 175);
			Leave_synpred79_JavaScript_fragment();
		}
	}
	// $ANTLR end synpred79_JavaScript

	partial void Enter_synpred120_JavaScript_fragment();
	partial void Leave_synpred120_JavaScript_fragment();

	// $ANTLR start synpred120_JavaScript
	public void synpred120_JavaScript_fragment()
	{
		Enter_synpred120_JavaScript_fragment();
		EnterRule("synpred120_JavaScript_fragment", 216);
		TraceIn("synpred120_JavaScript_fragment", 216);
		try
		{
			// JavaScript.g3:237:62: ( SkipSpace )
			DebugEnterAlt(1);
			// JavaScript.g3:237:62: SkipSpace
			{
			DebugLocation(237, 53);
			Match(input,SkipSpace,Follow._SkipSpace_in_synpred120_JavaScript1967); if (state.failed) return;

			}

		}
		finally
		{
			TraceOut("synpred120_JavaScript_fragment", 216);
			LeaveRule("synpred120_JavaScript_fragment", 216);
			Leave_synpred120_JavaScript_fragment();
		}
	}
	// $ANTLR end synpred120_JavaScript

	partial void Enter_synpred123_JavaScript_fragment();
	partial void Leave_synpred123_JavaScript_fragment();

	// $ANTLR start synpred123_JavaScript
	public void synpred123_JavaScript_fragment()
	{
		Enter_synpred123_JavaScript_fragment();
		EnterRule("synpred123_JavaScript_fragment", 219);
		TraceIn("synpred123_JavaScript_fragment", 219);
		try
		{
			// JavaScript.g3:241:42: ( SkipSpace )
			DebugEnterAlt(1);
			// JavaScript.g3:241:42: SkipSpace
			{
			DebugLocation(241, 33);
			Match(input,SkipSpace,Follow._SkipSpace_in_synpred123_JavaScript1997); if (state.failed) return;

			}

		}
		finally
		{
			TraceOut("synpred123_JavaScript_fragment", 219);
			LeaveRule("synpred123_JavaScript_fragment", 219);
			Leave_synpred123_JavaScript_fragment();
		}
	}
	// $ANTLR end synpred123_JavaScript

	partial void Enter_synpred143_JavaScript_fragment();
	partial void Leave_synpred143_JavaScript_fragment();

	// $ANTLR start synpred143_JavaScript
	public void synpred143_JavaScript_fragment()
	{
		Enter_synpred143_JavaScript_fragment();
		EnterRule("synpred143_JavaScript_fragment", 239);
		TraceIn("synpred143_JavaScript_fragment", 239);
		try
		{
			// JavaScript.g3:273:7: ( conditionalExpression )
			DebugEnterAlt(1);
			// JavaScript.g3:273:7: conditionalExpression
			{
			DebugLocation(273, 7);
			PushFollow(Follow._conditionalExpression_in_synpred143_JavaScript2257);
			conditionalExpression();
			PopFollow();
			if (state.failed) return;

			}

		}
		finally
		{
			TraceOut("synpred143_JavaScript_fragment", 239);
			LeaveRule("synpred143_JavaScript_fragment", 239);
			Leave_synpred143_JavaScript_fragment();
		}
	}
	// $ANTLR end synpred143_JavaScript

	partial void Enter_synpred146_JavaScript_fragment();
	partial void Leave_synpred146_JavaScript_fragment();

	// $ANTLR start synpred146_JavaScript
	public void synpred146_JavaScript_fragment()
	{
		Enter_synpred146_JavaScript_fragment();
		EnterRule("synpred146_JavaScript_fragment", 242);
		TraceIn("synpred146_JavaScript_fragment", 242);
		try
		{
			// JavaScript.g3:279:7: ( conditionalExpressionNoIn )
			DebugEnterAlt(1);
			// JavaScript.g3:279:7: conditionalExpressionNoIn
			{
			DebugLocation(279, 7);
			PushFollow(Follow._conditionalExpressionNoIn_in_synpred146_JavaScript2321);
			conditionalExpressionNoIn();
			PopFollow();
			if (state.failed) return;

			}

		}
		finally
		{
			TraceOut("synpred146_JavaScript_fragment", 242);
			LeaveRule("synpred146_JavaScript_fragment", 242);
			Leave_synpred146_JavaScript_fragment();
		}
	}
	// $ANTLR end synpred146_JavaScript

	partial void Enter_synpred160_JavaScript_fragment();
	partial void Leave_synpred160_JavaScript_fragment();

	// $ANTLR start synpred160_JavaScript
	public void synpred160_JavaScript_fragment()
	{
		Enter_synpred160_JavaScript_fragment();
		EnterRule("synpred160_JavaScript_fragment", 256);
		TraceIn("synpred160_JavaScript_fragment", 256);
		try
		{
			// JavaScript.g3:300:7: ( callExpression )
			DebugEnterAlt(1);
			// JavaScript.g3:300:7: callExpression
			{
			DebugLocation(300, 7);
			PushFollow(Follow._callExpression_in_synpred160_JavaScript2490);
			callExpression();
			PopFollow();
			if (state.failed) return;

			}

		}
		finally
		{
			TraceOut("synpred160_JavaScript_fragment", 256);
			LeaveRule("synpred160_JavaScript_fragment", 256);
			Leave_synpred160_JavaScript_fragment();
		}
	}
	// $ANTLR end synpred160_JavaScript

	partial void Enter_synpred161_JavaScript_fragment();
	partial void Leave_synpred161_JavaScript_fragment();

	// $ANTLR start synpred161_JavaScript
	public void synpred161_JavaScript_fragment()
	{
		Enter_synpred161_JavaScript_fragment();
		EnterRule("synpred161_JavaScript_fragment", 257);
		TraceIn("synpred161_JavaScript_fragment", 257);
		try
		{
			// JavaScript.g3:305:7: ( memberExpression )
			DebugEnterAlt(1);
			// JavaScript.g3:305:7: memberExpression
			{
			DebugLocation(305, 7);
			PushFollow(Follow._memberExpression_in_synpred161_JavaScript2515);
			memberExpression();
			PopFollow();
			if (state.failed) return;

			}

		}
		finally
		{
			TraceOut("synpred161_JavaScript_fragment", 257);
			LeaveRule("synpred161_JavaScript_fragment", 257);
			Leave_synpred161_JavaScript_fragment();
		}
	}
	// $ANTLR end synpred161_JavaScript

	partial void Enter_synpred190_JavaScript_fragment();
	partial void Leave_synpred190_JavaScript_fragment();

	// $ANTLR start synpred190_JavaScript
	public void synpred190_JavaScript_fragment()
	{
		JavaScriptParser.logicalORExpression_return cond = default(JavaScriptParser.logicalORExpression_return);
		JavaScriptParser.assignmentExpression_return tV = default(JavaScriptParser.assignmentExpression_return);
		JavaScriptParser.assignmentExpression_return fV = default(JavaScriptParser.assignmentExpression_return);

		Enter_synpred190_JavaScript_fragment();
		EnterRule("synpred190_JavaScript_fragment", 286);
		TraceIn("synpred190_JavaScript_fragment", 286);
		try
		{
			// JavaScript.g3:364:7: (cond= logicalORExpression ( SkipSpace )* '?' ( SkipSpace )* tV= assignmentExpression ( SkipSpace )* ':' ( SkipSpace )* fV= assignmentExpression )
			DebugEnterAlt(1);
			// JavaScript.g3:364:7: cond= logicalORExpression ( SkipSpace )* '?' ( SkipSpace )* tV= assignmentExpression ( SkipSpace )* ':' ( SkipSpace )* fV= assignmentExpression
			{
			DebugLocation(364, 11);
			PushFollow(Follow._logicalORExpression_in_synpred190_JavaScript3035);
			cond=logicalORExpression();
			PopFollow();
			if (state.failed) return;
			DebugLocation(364, 32);
			// JavaScript.g3:364:32: ( SkipSpace )*
			try { DebugEnterSubRule(290);
			while (true)
			{
				int alt290=2;
				try { DebugEnterDecision(290, decisionCanBacktrack[290]);
				int LA290_0 = input.LA(1);

				if ((LA290_0==SkipSpace))
				{
					alt290=1;
				}


				} finally { DebugExitDecision(290); }
				switch ( alt290 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:364:32: SkipSpace
					{
					DebugLocation(364, 32);
					Match(input,SkipSpace,Follow._SkipSpace_in_synpred190_JavaScript3037); if (state.failed) return;

					}
					break;

				default:
					goto loop290;
				}
			}

			loop290:
				;

			} finally { DebugExitSubRule(290); }

			DebugLocation(364, 43);
			Match(input,134,Follow._134_in_synpred190_JavaScript3040); if (state.failed) return;
			DebugLocation(364, 47);
			// JavaScript.g3:364:47: ( SkipSpace )*
			try { DebugEnterSubRule(291);
			while (true)
			{
				int alt291=2;
				try { DebugEnterDecision(291, decisionCanBacktrack[291]);
				int LA291_0 = input.LA(1);

				if ((LA291_0==SkipSpace))
				{
					alt291=1;
				}


				} finally { DebugExitDecision(291); }
				switch ( alt291 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:364:47: SkipSpace
					{
					DebugLocation(364, 47);
					Match(input,SkipSpace,Follow._SkipSpace_in_synpred190_JavaScript3042); if (state.failed) return;

					}
					break;

				default:
					goto loop291;
				}
			}

			loop291:
				;

			} finally { DebugExitSubRule(291); }

			DebugLocation(364, 60);
			PushFollow(Follow._assignmentExpression_in_synpred190_JavaScript3047);
			tV=assignmentExpression();
			PopFollow();
			if (state.failed) return;
			DebugLocation(364, 82);
			// JavaScript.g3:364:82: ( SkipSpace )*
			try { DebugEnterSubRule(292);
			while (true)
			{
				int alt292=2;
				try { DebugEnterDecision(292, decisionCanBacktrack[292]);
				int LA292_0 = input.LA(1);

				if ((LA292_0==SkipSpace))
				{
					alt292=1;
				}


				} finally { DebugExitDecision(292); }
				switch ( alt292 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:364:82: SkipSpace
					{
					DebugLocation(364, 82);
					Match(input,SkipSpace,Follow._SkipSpace_in_synpred190_JavaScript3049); if (state.failed) return;

					}
					break;

				default:
					goto loop292;
				}
			}

			loop292:
				;

			} finally { DebugExitSubRule(292); }

			DebugLocation(364, 93);
			Match(input,133,Follow._133_in_synpred190_JavaScript3052); if (state.failed) return;
			DebugLocation(364, 97);
			// JavaScript.g3:364:97: ( SkipSpace )*
			try { DebugEnterSubRule(293);
			while (true)
			{
				int alt293=2;
				try { DebugEnterDecision(293, decisionCanBacktrack[293]);
				int LA293_0 = input.LA(1);

				if ((LA293_0==SkipSpace))
				{
					alt293=1;
				}


				} finally { DebugExitDecision(293); }
				switch ( alt293 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:364:97: SkipSpace
					{
					DebugLocation(364, 97);
					Match(input,SkipSpace,Follow._SkipSpace_in_synpred190_JavaScript3054); if (state.failed) return;

					}
					break;

				default:
					goto loop293;
				}
			}

			loop293:
				;

			} finally { DebugExitSubRule(293); }

			DebugLocation(364, 110);
			PushFollow(Follow._assignmentExpression_in_synpred190_JavaScript3059);
			fV=assignmentExpression();
			PopFollow();
			if (state.failed) return;

			}

		}
		finally
		{
			TraceOut("synpred190_JavaScript_fragment", 286);
			LeaveRule("synpred190_JavaScript_fragment", 286);
			Leave_synpred190_JavaScript_fragment();
		}
	}
	// $ANTLR end synpred190_JavaScript

	partial void Enter_synpred195_JavaScript_fragment();
	partial void Leave_synpred195_JavaScript_fragment();

	// $ANTLR start synpred195_JavaScript
	public void synpred195_JavaScript_fragment()
	{
		JavaScriptParser.logicalORExpressionNoIn_return cond = default(JavaScriptParser.logicalORExpressionNoIn_return);
		JavaScriptParser.assignmentExpressionNoIn_return tV = default(JavaScriptParser.assignmentExpressionNoIn_return);
		JavaScriptParser.assignmentExpressionNoIn_return fV = default(JavaScriptParser.assignmentExpressionNoIn_return);

		Enter_synpred195_JavaScript_fragment();
		EnterRule("synpred195_JavaScript_fragment", 291);
		TraceIn("synpred195_JavaScript_fragment", 291);
		try
		{
			// JavaScript.g3:370:7: (cond= logicalORExpressionNoIn ( SkipSpace )* '?' ( SkipSpace )* tV= assignmentExpressionNoIn ( SkipSpace )* ':' ( SkipSpace )* fV= assignmentExpressionNoIn )
			DebugEnterAlt(1);
			// JavaScript.g3:370:7: cond= logicalORExpressionNoIn ( SkipSpace )* '?' ( SkipSpace )* tV= assignmentExpressionNoIn ( SkipSpace )* ':' ( SkipSpace )* fV= assignmentExpressionNoIn
			{
			DebugLocation(370, 11);
			PushFollow(Follow._logicalORExpressionNoIn_in_synpred195_JavaScript3109);
			cond=logicalORExpressionNoIn();
			PopFollow();
			if (state.failed) return;
			DebugLocation(370, 36);
			// JavaScript.g3:370:36: ( SkipSpace )*
			try { DebugEnterSubRule(294);
			while (true)
			{
				int alt294=2;
				try { DebugEnterDecision(294, decisionCanBacktrack[294]);
				int LA294_0 = input.LA(1);

				if ((LA294_0==SkipSpace))
				{
					alt294=1;
				}


				} finally { DebugExitDecision(294); }
				switch ( alt294 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:370:36: SkipSpace
					{
					DebugLocation(370, 36);
					Match(input,SkipSpace,Follow._SkipSpace_in_synpred195_JavaScript3111); if (state.failed) return;

					}
					break;

				default:
					goto loop294;
				}
			}

			loop294:
				;

			} finally { DebugExitSubRule(294); }

			DebugLocation(370, 47);
			Match(input,134,Follow._134_in_synpred195_JavaScript3114); if (state.failed) return;
			DebugLocation(370, 51);
			// JavaScript.g3:370:51: ( SkipSpace )*
			try { DebugEnterSubRule(295);
			while (true)
			{
				int alt295=2;
				try { DebugEnterDecision(295, decisionCanBacktrack[295]);
				int LA295_0 = input.LA(1);

				if ((LA295_0==SkipSpace))
				{
					alt295=1;
				}


				} finally { DebugExitDecision(295); }
				switch ( alt295 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:370:51: SkipSpace
					{
					DebugLocation(370, 51);
					Match(input,SkipSpace,Follow._SkipSpace_in_synpred195_JavaScript3116); if (state.failed) return;

					}
					break;

				default:
					goto loop295;
				}
			}

			loop295:
				;

			} finally { DebugExitSubRule(295); }

			DebugLocation(370, 64);
			PushFollow(Follow._assignmentExpressionNoIn_in_synpred195_JavaScript3121);
			tV=assignmentExpressionNoIn();
			PopFollow();
			if (state.failed) return;
			DebugLocation(370, 90);
			// JavaScript.g3:370:90: ( SkipSpace )*
			try { DebugEnterSubRule(296);
			while (true)
			{
				int alt296=2;
				try { DebugEnterDecision(296, decisionCanBacktrack[296]);
				int LA296_0 = input.LA(1);

				if ((LA296_0==SkipSpace))
				{
					alt296=1;
				}


				} finally { DebugExitDecision(296); }
				switch ( alt296 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:370:90: SkipSpace
					{
					DebugLocation(370, 90);
					Match(input,SkipSpace,Follow._SkipSpace_in_synpred195_JavaScript3123); if (state.failed) return;

					}
					break;

				default:
					goto loop296;
				}
			}

			loop296:
				;

			} finally { DebugExitSubRule(296); }

			DebugLocation(370, 101);
			Match(input,133,Follow._133_in_synpred195_JavaScript3126); if (state.failed) return;
			DebugLocation(370, 105);
			// JavaScript.g3:370:105: ( SkipSpace )*
			try { DebugEnterSubRule(297);
			while (true)
			{
				int alt297=2;
				try { DebugEnterDecision(297, decisionCanBacktrack[297]);
				int LA297_0 = input.LA(1);

				if ((LA297_0==SkipSpace))
				{
					alt297=1;
				}


				} finally { DebugExitDecision(297); }
				switch ( alt297 )
				{
				case 1:
					DebugEnterAlt(1);
					// JavaScript.g3:370:105: SkipSpace
					{
					DebugLocation(370, 105);
					Match(input,SkipSpace,Follow._SkipSpace_in_synpred195_JavaScript3128); if (state.failed) return;

					}
					break;

				default:
					goto loop297;
				}
			}

			loop297:
				;

			} finally { DebugExitSubRule(297); }

			DebugLocation(370, 118);
			PushFollow(Follow._assignmentExpressionNoIn_in_synpred195_JavaScript3133);
			fV=assignmentExpressionNoIn();
			PopFollow();
			if (state.failed) return;

			}

		}
		finally
		{
			TraceOut("synpred195_JavaScript_fragment", 291);
			LeaveRule("synpred195_JavaScript_fragment", 291);
			Leave_synpred195_JavaScript_fragment();
		}
	}
	// $ANTLR end synpred195_JavaScript

	partial void Enter_synpred276_JavaScript_fragment();
	partial void Leave_synpred276_JavaScript_fragment();

	// $ANTLR start synpred276_JavaScript
	public void synpred276_JavaScript_fragment()
	{
		CommonToken op1=null;
		CommonToken op2=null;

		Enter_synpred276_JavaScript_fragment();
		EnterRule("synpred276_JavaScript_fragment", 372);
		TraceIn("synpred276_JavaScript_fragment", 372);
		try
		{
			// JavaScript.g3:495:7: ( leftHandSideExpression (op1= PLUS_INC |op2= MINUS_INC ) )
			DebugEnterAlt(1);
			// JavaScript.g3:495:7: leftHandSideExpression (op1= PLUS_INC |op2= MINUS_INC )
			{
			DebugLocation(495, 7);
			PushFollow(Follow._leftHandSideExpression_in_synpred276_JavaScript4594);
			leftHandSideExpression();
			PopFollow();
			if (state.failed) return;
			DebugLocation(495, 30);
			// JavaScript.g3:495:30: (op1= PLUS_INC |op2= MINUS_INC )
			int alt332=2;
			try { DebugEnterSubRule(332);
			try { DebugEnterDecision(332, decisionCanBacktrack[332]);
			int LA332_0 = input.LA(1);

			if ((LA332_0==PLUS_INC))
			{
				alt332=1;
			}
			else if ((LA332_0==MINUS_INC))
			{
				alt332=2;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return;}
				NoViableAltException nvae = new NoViableAltException("", 332, 0, input);

				DebugRecognitionException(nvae);
				throw nvae;
			}
			} finally { DebugExitDecision(332); }
			switch (alt332)
			{
			case 1:
				DebugEnterAlt(1);
				// JavaScript.g3:495:31: op1= PLUS_INC
				{
				DebugLocation(495, 34);
				op1=(CommonToken)Match(input,PLUS_INC,Follow._PLUS_INC_in_synpred276_JavaScript4599); if (state.failed) return;

				}
				break;
			case 2:
				DebugEnterAlt(2);
				// JavaScript.g3:495:46: op2= MINUS_INC
				{
				DebugLocation(495, 49);
				op2=(CommonToken)Match(input,MINUS_INC,Follow._MINUS_INC_in_synpred276_JavaScript4605); if (state.failed) return;

				}
				break;

			}
			} finally { DebugExitSubRule(332); }


			}

		}
		finally
		{
			TraceOut("synpred276_JavaScript_fragment", 372);
			LeaveRule("synpred276_JavaScript_fragment", 372);
			Leave_synpred276_JavaScript_fragment();
		}
	}
	// $ANTLR end synpred276_JavaScript

	partial void Enter_synpred285_JavaScript_fragment();
	partial void Leave_synpred285_JavaScript_fragment();

	// $ANTLR start synpred285_JavaScript
	public void synpred285_JavaScript_fragment()
	{
		Enter_synpred285_JavaScript_fragment();
		EnterRule("synpred285_JavaScript_fragment", 381);
		TraceIn("synpred285_JavaScript_fragment", 381);
		try
		{
			// JavaScript.g3:513:11: ( SkipSpace )
			DebugEnterAlt(1);
			// JavaScript.g3:513:11: SkipSpace
			{
			DebugLocation(513, 11);
			Match(input,SkipSpace,Follow._SkipSpace_in_synpred285_JavaScript4745); if (state.failed) return;

			}

		}
		finally
		{
			TraceOut("synpred285_JavaScript_fragment", 381);
			LeaveRule("synpred285_JavaScript_fragment", 381);
			Leave_synpred285_JavaScript_fragment();
		}
	}
	// $ANTLR end synpred285_JavaScript
	#endregion Rules

	#region Synpreds
	private bool EvaluatePredicate(System.Action fragment)
	{
		bool success = false;
		state.backtracking++;
		try { DebugBeginBacktrack(state.backtracking);
		int start = input.Mark();
		try
		{
			fragment();
		}
		catch ( RecognitionException re )
		{
			System.Console.Error.WriteLine("impossible: "+re);
		}
		success = !state.failed;
		input.Rewind(start);
		} finally { DebugEndBacktrack(state.backtracking, success); }
		state.backtracking--;
		state.failed=false;
		return success;
	}
	#endregion Synpreds


	#region DFA
	DFA4 dfa4;
	DFA5 dfa5;
	DFA17 dfa17;
	DFA16 dfa16;
	DFA21 dfa21;
	DFA26 dfa26;
	DFA31 dfa31;
	DFA34 dfa34;
	DFA37 dfa37;
	DFA40 dfa40;
	DFA86 dfa86;
	DFA98 dfa98;
	DFA102 dfa102;
	DFA101 dfa101;
	DFA116 dfa116;
	DFA125 dfa125;
	DFA128 dfa128;
	DFA131 dfa131;
	DFA134 dfa134;
	DFA135 dfa135;
	DFA137 dfa137;
	DFA139 dfa139;
	DFA145 dfa145;
	DFA152 dfa152;
	DFA151 dfa151;
	DFA158 dfa158;
	DFA163 dfa163;
	DFA168 dfa168;
	DFA171 dfa171;
	DFA174 dfa174;
	DFA177 dfa177;
	DFA180 dfa180;
	DFA183 dfa183;
	DFA186 dfa186;
	DFA189 dfa189;
	DFA192 dfa192;
	DFA195 dfa195;
	DFA198 dfa198;
	DFA201 dfa201;
	DFA204 dfa204;
	DFA207 dfa207;
	DFA210 dfa210;
	DFA213 dfa213;
	DFA216 dfa216;
	DFA219 dfa219;
	DFA223 dfa223;
	DFA232 dfa232;
	DFA231 dfa231;
	DFA240 dfa240;
	DFA237 dfa237;
	DFA244 dfa244;
	DFA249 dfa249;
	DFA248 dfa248;

	protected override void InitDFAs()
	{
		base.InitDFAs();
		dfa4 = new DFA4( this );
		dfa5 = new DFA5( this, SpecialStateTransition5 );
		dfa17 = new DFA17( this );
		dfa16 = new DFA16( this );
		dfa21 = new DFA21( this, SpecialStateTransition21 );
		dfa26 = new DFA26( this );
		dfa31 = new DFA31( this );
		dfa34 = new DFA34( this );
		dfa37 = new DFA37( this );
		dfa40 = new DFA40( this );
		dfa86 = new DFA86( this );
		dfa98 = new DFA98( this );
		dfa102 = new DFA102( this );
		dfa101 = new DFA101( this );
		dfa116 = new DFA116( this );
		dfa125 = new DFA125( this );
		dfa128 = new DFA128( this );
		dfa131 = new DFA131( this, SpecialStateTransition131 );
		dfa134 = new DFA134( this, SpecialStateTransition134 );
		dfa135 = new DFA135( this, SpecialStateTransition135 );
		dfa137 = new DFA137( this, SpecialStateTransition137 );
		dfa139 = new DFA139( this );
		dfa145 = new DFA145( this );
		dfa152 = new DFA152( this );
		dfa151 = new DFA151( this );
		dfa158 = new DFA158( this );
		dfa163 = new DFA163( this, SpecialStateTransition163 );
		dfa168 = new DFA168( this, SpecialStateTransition168 );
		dfa171 = new DFA171( this );
		dfa174 = new DFA174( this );
		dfa177 = new DFA177( this );
		dfa180 = new DFA180( this );
		dfa183 = new DFA183( this );
		dfa186 = new DFA186( this );
		dfa189 = new DFA189( this );
		dfa192 = new DFA192( this );
		dfa195 = new DFA195( this );
		dfa198 = new DFA198( this );
		dfa201 = new DFA201( this );
		dfa204 = new DFA204( this );
		dfa207 = new DFA207( this );
		dfa210 = new DFA210( this );
		dfa213 = new DFA213( this );
		dfa216 = new DFA216( this );
		dfa219 = new DFA219( this );
		dfa223 = new DFA223( this, SpecialStateTransition223 );
		dfa232 = new DFA232( this );
		dfa231 = new DFA231( this );
		dfa240 = new DFA240( this );
		dfa237 = new DFA237( this );
		dfa244 = new DFA244( this );
		dfa249 = new DFA249( this );
		dfa248 = new DFA248( this );
	}

	private class DFA4 : DFA
	{
		private const string DFA4_eotS =
			"\x4\xFFFF";
		private const string DFA4_eofS =
			"\x2\x2\x2\xFFFF";
		private const string DFA4_minS =
			"\x2\xC\x2\xFFFF";
		private const string DFA4_maxS =
			"\x2\x94\x2\xFFFF";
		private const string DFA4_acceptS =
			"\x2\xFFFF\x1\x2\x1\x1";
		private const string DFA4_specialS =
			"\x4\xFFFF}>";
		private static readonly string[] DFA4_transitionS =
			{
				"\x1\x3\x5\xFFFF\x2\x3\x3\xFFFF\x1\x3\x2\xFFFF\x1\x3\x3\xFFFF\x1\x3\xA"+
				"\xFFFF\x1\x3\xD\xFFFF\x1\x3\x7\xFFFF\x1\x3\x7\xFFFF\x1\x3\x1\xFFFF\x1"+
				"\x3\x5\xFFFF\x1\x3\x2\xFFFF\x2\x3\x2\xFFFF\x1\x3\x1\xFFFF\x1\x3\x1\xFFFF"+
				"\x1\x3\x4\xFFFF\x1\x3\x2\xFFFF\x1\x3\x9\xFFFF\x1\x1\x3\x3\x1\xFFFF\x1"+
				"\x3\x2\xFFFF\x1\x3\x9\xFFFF\x2\x3\x2\xFFFF\x1\x3\x4\xFFFF\x1\x3\x3\xFFFF"+
				"\x1\x3\x1\xFFFF\x7\x3\x1\x2",
				"\x1\x3\x5\xFFFF\x2\x3\x3\xFFFF\x1\x3\x2\xFFFF\x1\x3\x3\xFFFF\x1\x3\xA"+
				"\xFFFF\x1\x3\xD\xFFFF\x1\x3\x7\xFFFF\x1\x3\x7\xFFFF\x1\x3\x1\xFFFF\x1"+
				"\x3\x5\xFFFF\x1\x3\x2\xFFFF\x2\x3\x2\xFFFF\x1\x3\x1\xFFFF\x1\x3\x1\xFFFF"+
				"\x1\x3\x4\xFFFF\x1\x3\x2\xFFFF\x1\x3\x9\xFFFF\x1\x1\x3\x3\x1\xFFFF\x1"+
				"\x3\x2\xFFFF\x1\x3\x9\xFFFF\x2\x3\x2\xFFFF\x1\x3\x4\xFFFF\x1\x3\x3\xFFFF"+
				"\x1\x3\x1\xFFFF\x7\x3\x1\x2",
				"",
				""
			};

		private static readonly short[] DFA4_eot = DFA.UnpackEncodedString(DFA4_eotS);
		private static readonly short[] DFA4_eof = DFA.UnpackEncodedString(DFA4_eofS);
		private static readonly char[] DFA4_min = DFA.UnpackEncodedStringToUnsignedChars(DFA4_minS);
		private static readonly char[] DFA4_max = DFA.UnpackEncodedStringToUnsignedChars(DFA4_maxS);
		private static readonly short[] DFA4_accept = DFA.UnpackEncodedString(DFA4_acceptS);
		private static readonly short[] DFA4_special = DFA.UnpackEncodedString(DFA4_specialS);
		private static readonly short[][] DFA4_transition;

		static DFA4()
		{
			int numStates = DFA4_transitionS.Length;
			DFA4_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA4_transition[i] = DFA.UnpackEncodedString(DFA4_transitionS[i]);
			}
		}

		public DFA4( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 4;
			this.eot = DFA4_eot;
			this.eof = DFA4_eof;
			this.min = DFA4_min;
			this.max = DFA4_max;
			this.accept = DFA4_accept;
			this.special = DFA4_special;
			this.transition = DFA4_transition;
		}

		public override string Description { get { return "()* loopback of 69:21: ( ( SkipSpace )* sourceElement )*"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private class DFA5 : DFA
	{
		private const string DFA5_eotS =
			"\x19\xFFFF";
		private const string DFA5_eofS =
			"\x19\xFFFF";
		private const string DFA5_minS =
			"\x1\xC\x1\x0\x17\xFFFF";
		private const string DFA5_maxS =
			"\x1\x93\x1\x0\x17\xFFFF";
		private const string DFA5_acceptS =
			"\x2\xFFFF\x1\x2\x15\xFFFF\x1\x1";
		private const string DFA5_specialS =
			"\x1\xFFFF\x1\x0\x17\xFFFF}>";
		private static readonly string[] DFA5_transitionS =
			{
				"\x1\x2\x5\xFFFF\x2\x2\x3\xFFFF\x1\x2\x2\xFFFF\x1\x2\x3\xFFFF\x1\x2\xA"+
				"\xFFFF\x1\x2\xD\xFFFF\x1\x2\x7\xFFFF\x1\x2\x7\xFFFF\x1\x2\x1\xFFFF\x1"+
				"\x2\x5\xFFFF\x1\x2\x2\xFFFF\x2\x2\x2\xFFFF\x1\x2\x1\xFFFF\x1\x2\x1\xFFFF"+
				"\x1\x2\x4\xFFFF\x1\x2\x2\xFFFF\x1\x2\xA\xFFFF\x3\x2\x1\xFFFF\x1\x2\x2"+
				"\xFFFF\x1\x2\x9\xFFFF\x2\x2\x2\xFFFF\x1\x2\x4\xFFFF\x1\x2\x3\xFFFF\x1"+
				"\x2\x1\xFFFF\x1\x2\x1\x1\x5\x2",
				"\x1\xFFFF",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				""
			};

		private static readonly short[] DFA5_eot = DFA.UnpackEncodedString(DFA5_eotS);
		private static readonly short[] DFA5_eof = DFA.UnpackEncodedString(DFA5_eofS);
		private static readonly char[] DFA5_min = DFA.UnpackEncodedStringToUnsignedChars(DFA5_minS);
		private static readonly char[] DFA5_max = DFA.UnpackEncodedStringToUnsignedChars(DFA5_maxS);
		private static readonly short[] DFA5_accept = DFA.UnpackEncodedString(DFA5_acceptS);
		private static readonly short[] DFA5_special = DFA.UnpackEncodedString(DFA5_specialS);
		private static readonly short[][] DFA5_transition;

		static DFA5()
		{
			int numStates = DFA5_transitionS.Length;
			DFA5_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA5_transition[i] = DFA.UnpackEncodedString(DFA5_transitionS[i]);
			}
		}

		public DFA5( BaseRecognizer recognizer, SpecialStateTransitionHandler specialStateTransition )
			: base(specialStateTransition)
		{
			this.recognizer = recognizer;
			this.decisionNumber = 5;
			this.eot = DFA5_eot;
			this.eof = DFA5_eof;
			this.min = DFA5_min;
			this.max = DFA5_max;
			this.accept = DFA5_accept;
			this.special = DFA5_special;
			this.transition = DFA5_transition;
		}

		public override string Description { get { return "73:1: sourceElement : ( functionDeclaration | statement );"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private int SpecialStateTransition5(DFA dfa, int s, IIntStream _input)
	{
		ITokenStream input = (ITokenStream)_input;
		int _s = s;
		switch (s)
		{
			case 0:
				int LA5_1 = input.LA(1);


				int index5_1 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred5_JavaScript_fragment)) ) {s = 24;}

				else if ( (true) ) {s = 2;}


				input.Seek(index5_1);
				if ( s>=0 ) return s;
				break;
		}
		if (state.backtracking > 0) {state.failed=true; return -1;}
		NoViableAltException nvae = new NoViableAltException(dfa.Description, 5, _s, input);
		dfa.Error(nvae);
		throw nvae;
	}
	private class DFA17 : DFA
	{
		private const string DFA17_eotS =
			"\x4\xFFFF";
		private const string DFA17_eofS =
			"\x4\xFFFF";
		private const string DFA17_minS =
			"\x2\x3F\x2\xFFFF";
		private const string DFA17_maxS =
			"\x2\x83\x2\xFFFF";
		private const string DFA17_acceptS =
			"\x2\xFFFF\x1\x1\x1\x2";
		private const string DFA17_specialS =
			"\x4\xFFFF}>";
		private static readonly string[] DFA17_transitionS =
			{
				"\x1\x2\x2C\xFFFF\x1\x1\x16\xFFFF\x1\x3",
				"\x1\x2\x2C\xFFFF\x1\x1\x16\xFFFF\x1\x3",
				"",
				""
			};

		private static readonly short[] DFA17_eot = DFA.UnpackEncodedString(DFA17_eotS);
		private static readonly short[] DFA17_eof = DFA.UnpackEncodedString(DFA17_eofS);
		private static readonly char[] DFA17_min = DFA.UnpackEncodedStringToUnsignedChars(DFA17_minS);
		private static readonly char[] DFA17_max = DFA.UnpackEncodedStringToUnsignedChars(DFA17_maxS);
		private static readonly short[] DFA17_accept = DFA.UnpackEncodedString(DFA17_acceptS);
		private static readonly short[] DFA17_special = DFA.UnpackEncodedString(DFA17_specialS);
		private static readonly short[][] DFA17_transition;

		static DFA17()
		{
			int numStates = DFA17_transitionS.Length;
			DFA17_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA17_transition[i] = DFA.UnpackEncodedString(DFA17_transitionS[i]);
			}
		}

		public DFA17( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 17;
			this.eot = DFA17_eot;
			this.eof = DFA17_eof;
			this.min = DFA17_min;
			this.max = DFA17_max;
			this.accept = DFA17_accept;
			this.special = DFA17_special;
			this.transition = DFA17_transition;
		}

		public override string Description { get { return "90:11: ( ( SkipSpace )* Identifier ( ( SkipSpace )* ',' ( SkipSpace )* Identifier )* )?"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private class DFA16 : DFA
	{
		private const string DFA16_eotS =
			"\x4\xFFFF";
		private const string DFA16_eofS =
			"\x4\xFFFF";
		private const string DFA16_minS =
			"\x2\x15\x2\xFFFF";
		private const string DFA16_maxS =
			"\x2\x83\x2\xFFFF";
		private const string DFA16_acceptS =
			"\x2\xFFFF\x1\x2\x1\x1";
		private const string DFA16_specialS =
			"\x4\xFFFF}>";
		private static readonly string[] DFA16_transitionS =
			{
				"\x1\x3\x56\xFFFF\x1\x1\x16\xFFFF\x1\x2",
				"\x1\x3\x56\xFFFF\x1\x1\x16\xFFFF\x1\x2",
				"",
				""
			};

		private static readonly short[] DFA16_eot = DFA.UnpackEncodedString(DFA16_eotS);
		private static readonly short[] DFA16_eof = DFA.UnpackEncodedString(DFA16_eofS);
		private static readonly char[] DFA16_min = DFA.UnpackEncodedStringToUnsignedChars(DFA16_minS);
		private static readonly char[] DFA16_max = DFA.UnpackEncodedStringToUnsignedChars(DFA16_maxS);
		private static readonly short[] DFA16_accept = DFA.UnpackEncodedString(DFA16_acceptS);
		private static readonly short[] DFA16_special = DFA.UnpackEncodedString(DFA16_specialS);
		private static readonly short[][] DFA16_transition;

		static DFA16()
		{
			int numStates = DFA16_transitionS.Length;
			DFA16_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA16_transition[i] = DFA.UnpackEncodedString(DFA16_transitionS[i]);
			}
		}

		public DFA16( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 16;
			this.eot = DFA16_eot;
			this.eof = DFA16_eof;
			this.min = DFA16_min;
			this.max = DFA16_max;
			this.accept = DFA16_accept;
			this.special = DFA16_special;
			this.transition = DFA16_transition;
		}

		public override string Description { get { return "()* loopback of 90:34: ( ( SkipSpace )* ',' ( SkipSpace )* Identifier )*"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private class DFA21 : DFA
	{
		private const string DFA21_eotS =
			"\x1A\xFFFF";
		private const string DFA21_eofS =
			"\x1A\xFFFF";
		private const string DFA21_minS =
			"\x1\xC\x1\x0\x3\xFFFF\x1\x0\x14\xFFFF";
		private const string DFA21_maxS =
			"\x1\x93\x1\x0\x3\xFFFF\x1\x0\x14\xFFFF";
		private const string DFA21_acceptS =
			"\x2\xFFFF\x1\x2\x1\x3\x1\x4\x8\xFFFF\x1\x5\x1\x6\x2\xFFFF\x1\x7\x1\x8"+
			"\x1\x9\x1\xA\x1\xC\x1\xD\x1\xE\x1\x1\x1\xB";
		private const string DFA21_specialS =
			"\x1\xFFFF\x1\x0\x3\xFFFF\x1\x1\x14\xFFFF}>";
		private static readonly string[] DFA21_transitionS =
			{
				"\x1\x4\x5\xFFFF\x1\x12\x1\x4\x3\xFFFF\x1\x11\x2\xFFFF\x1\x4\x3\xFFFF"+
				"\x1\x4\xA\xFFFF\x1\x4\xD\xFFFF\x1\x4\x7\xFFFF\x1\x5\x7\xFFFF\x1\x4\x1"+
				"\xFFFF\x1\x4\x5\xFFFF\x1\x4\x2\xFFFF\x2\x4\x2\xFFFF\x1\x4\x1\xFFFF\x1"+
				"\x4\x1\xFFFF\x1\x4\x4\xFFFF\x1\x13\x2\xFFFF\x1\x3\xA\xFFFF\x2\x4\x1"+
				"\x16\x1\xFFFF\x1\x17\x2\xFFFF\x1\x4\x9\xFFFF\x1\x2\x1\x4\x2\xFFFF\x1"+
				"\x4\x4\xFFFF\x1\x4\x3\xFFFF\x1\xE\x1\xFFFF\x1\xE\x1\x4\x1\xD\x1\x15"+
				"\x1\xE\x1\x14\x1\x1",
				"\x1\xFFFF",
				"",
				"",
				"",
				"\x1\xFFFF",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				""
			};

		private static readonly short[] DFA21_eot = DFA.UnpackEncodedString(DFA21_eotS);
		private static readonly short[] DFA21_eof = DFA.UnpackEncodedString(DFA21_eofS);
		private static readonly char[] DFA21_min = DFA.UnpackEncodedStringToUnsignedChars(DFA21_minS);
		private static readonly char[] DFA21_max = DFA.UnpackEncodedStringToUnsignedChars(DFA21_maxS);
		private static readonly short[] DFA21_accept = DFA.UnpackEncodedString(DFA21_acceptS);
		private static readonly short[] DFA21_special = DFA.UnpackEncodedString(DFA21_specialS);
		private static readonly short[][] DFA21_transition;

		static DFA21()
		{
			int numStates = DFA21_transitionS.Length;
			DFA21_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA21_transition[i] = DFA.UnpackEncodedString(DFA21_transitionS[i]);
			}
		}

		public DFA21( BaseRecognizer recognizer, SpecialStateTransitionHandler specialStateTransition )
			: base(specialStateTransition)
		{
			this.recognizer = recognizer;
			this.decisionNumber = 21;
			this.eot = DFA21_eot;
			this.eof = DFA21_eof;
			this.min = DFA21_min;
			this.max = DFA21_max;
			this.accept = DFA21_accept;
			this.special = DFA21_special;
			this.transition = DFA21_transition;
		}

		public override string Description { get { return "100:1: statement : ( statementBlock | variableStatement | emptyStatement | expressionStatement | ifStatement | iterationStatement | continueStatement | breakStatement | returnStatement | withStatement | labelledStatement | switchStatement | throwStatement | tryStatement );"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private int SpecialStateTransition21(DFA dfa, int s, IIntStream _input)
	{
		ITokenStream input = (ITokenStream)_input;
		int _s = s;
		switch (s)
		{
			case 0:
				int LA21_1 = input.LA(1);


				int index21_1 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred21_JavaScript_fragment)) ) {s = 24;}

				else if ( (EvaluatePredicate(synpred24_JavaScript_fragment)) ) {s = 4;}


				input.Seek(index21_1);
				if ( s>=0 ) return s;
				break;
			case 1:
				int LA21_5 = input.LA(1);


				int index21_5 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred24_JavaScript_fragment)) ) {s = 4;}

				else if ( (EvaluatePredicate(synpred31_JavaScript_fragment)) ) {s = 25;}


				input.Seek(index21_5);
				if ( s>=0 ) return s;
				break;
		}
		if (state.backtracking > 0) {state.failed=true; return -1;}
		NoViableAltException nvae = new NoViableAltException(dfa.Description, 21, _s, input);
		dfa.Error(nvae);
		throw nvae;
	}
	private class DFA26 : DFA
	{
		private const string DFA26_eotS =
			"\x4\xFFFF";
		private const string DFA26_eofS =
			"\x1\x2\x3\xFFFF";
		private const string DFA26_minS =
			"\x2\xC\x2\xFFFF";
		private const string DFA26_maxS =
			"\x2\x94\x2\xFFFF";
		private const string DFA26_acceptS =
			"\x2\xFFFF\x1\x2\x1\x1";
		private const string DFA26_specialS =
			"\x4\xFFFF}>";
		private static readonly string[] DFA26_transitionS =
			{
				"\x1\x3\x5\xFFFF\x2\x3\x3\xFFFF\x1\x3\x2\xFFFF\x1\x3\x3\xFFFF\x1\x3\xA"+
				"\xFFFF\x1\x3\xD\xFFFF\x1\x3\x7\xFFFF\x1\x3\x7\xFFFF\x1\x3\x1\xFFFF\x1"+
				"\x3\x5\xFFFF\x1\x3\x2\xFFFF\x2\x3\x2\xFFFF\x1\x3\x1\xFFFF\x1\x3\x1\xFFFF"+
				"\x1\x3\x4\xFFFF\x1\x3\x2\xFFFF\x1\x3\x9\xFFFF\x1\x1\x3\x3\x1\xFFFF\x1"+
				"\x3\x2\xFFFF\x1\x3\x9\xFFFF\x2\x3\x2\xFFFF\x1\x3\x4\xFFFF\x1\x3\x1\xFFFF"+
				"\x2\x2\x1\x3\x1\xFFFF\x7\x3\x1\x2",
				"\x1\x3\x5\xFFFF\x2\x3\x3\xFFFF\x1\x3\x2\xFFFF\x1\x3\x3\xFFFF\x1\x3\xA"+
				"\xFFFF\x1\x3\xD\xFFFF\x1\x3\x7\xFFFF\x1\x3\x7\xFFFF\x1\x3\x1\xFFFF\x1"+
				"\x3\x5\xFFFF\x1\x3\x2\xFFFF\x2\x3\x2\xFFFF\x1\x3\x1\xFFFF\x1\x3\x1\xFFFF"+
				"\x1\x3\x4\xFFFF\x1\x3\x2\xFFFF\x1\x3\x9\xFFFF\x1\x1\x3\x3\x1\xFFFF\x1"+
				"\x3\x2\xFFFF\x1\x3\x9\xFFFF\x2\x3\x2\xFFFF\x1\x3\x4\xFFFF\x1\x3\x1\xFFFF"+
				"\x2\x2\x1\x3\x1\xFFFF\x7\x3\x1\x2",
				"",
				""
			};

		private static readonly short[] DFA26_eot = DFA.UnpackEncodedString(DFA26_eotS);
		private static readonly short[] DFA26_eof = DFA.UnpackEncodedString(DFA26_eofS);
		private static readonly char[] DFA26_min = DFA.UnpackEncodedStringToUnsignedChars(DFA26_minS);
		private static readonly char[] DFA26_max = DFA.UnpackEncodedStringToUnsignedChars(DFA26_maxS);
		private static readonly short[] DFA26_accept = DFA.UnpackEncodedString(DFA26_acceptS);
		private static readonly short[] DFA26_special = DFA.UnpackEncodedString(DFA26_specialS);
		private static readonly short[][] DFA26_transition;

		static DFA26()
		{
			int numStates = DFA26_transitionS.Length;
			DFA26_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA26_transition[i] = DFA.UnpackEncodedString(DFA26_transitionS[i]);
			}
		}

		public DFA26( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 26;
			this.eot = DFA26_eot;
			this.eof = DFA26_eof;
			this.min = DFA26_min;
			this.max = DFA26_max;
			this.accept = DFA26_accept;
			this.special = DFA26_special;
			this.transition = DFA26_transition;
		}

		public override string Description { get { return "()* loopback of 123:17: ( ( SkipSpace )* statement )*"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private class DFA31 : DFA
	{
		private const string DFA31_eotS =
			"\x4\xFFFF";
		private const string DFA31_eofS =
			"\x4\xFFFF";
		private const string DFA31_minS =
			"\x2\x15\x2\xFFFF";
		private const string DFA31_maxS =
			"\x2\x6C\x2\xFFFF";
		private const string DFA31_acceptS =
			"\x2\xFFFF\x1\x2\x1\x1";
		private const string DFA31_specialS =
			"\x4\xFFFF}>";
		private static readonly string[] DFA31_transitionS =
			{
				"\x1\x3\x4C\xFFFF\x1\x2\x9\xFFFF\x1\x1",
				"\x1\x3\x4C\xFFFF\x1\x2\x9\xFFFF\x1\x1",
				"",
				""
			};

		private static readonly short[] DFA31_eot = DFA.UnpackEncodedString(DFA31_eotS);
		private static readonly short[] DFA31_eof = DFA.UnpackEncodedString(DFA31_eofS);
		private static readonly char[] DFA31_min = DFA.UnpackEncodedStringToUnsignedChars(DFA31_minS);
		private static readonly char[] DFA31_max = DFA.UnpackEncodedStringToUnsignedChars(DFA31_maxS);
		private static readonly short[] DFA31_accept = DFA.UnpackEncodedString(DFA31_acceptS);
		private static readonly short[] DFA31_special = DFA.UnpackEncodedString(DFA31_specialS);
		private static readonly short[][] DFA31_transition;

		static DFA31()
		{
			int numStates = DFA31_transitionS.Length;
			DFA31_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA31_transition[i] = DFA.UnpackEncodedString(DFA31_transitionS[i]);
			}
		}

		public DFA31( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 31;
			this.eot = DFA31_eot;
			this.eof = DFA31_eof;
			this.min = DFA31_min;
			this.max = DFA31_max;
			this.accept = DFA31_accept;
			this.special = DFA31_special;
			this.transition = DFA31_transition;
		}

		public override string Description { get { return "()* loopback of 131:27: ( ( SkipSpace )* ',' ( SkipSpace )* variableDeclaration )*"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private class DFA34 : DFA
	{
		private const string DFA34_eotS =
			"\x4\xFFFF";
		private const string DFA34_eofS =
			"\x1\x2\x3\xFFFF";
		private const string DFA34_minS =
			"\x2\x15\x2\xFFFF";
		private const string DFA34_maxS =
			"\x2\x6C\x2\xFFFF";
		private const string DFA34_acceptS =
			"\x2\xFFFF\x1\x2\x1\x1";
		private const string DFA34_specialS =
			"\x4\xFFFF}>";
		private static readonly string[] DFA34_transitionS =
			{
				"\x1\x3\x4C\xFFFF\x1\x2\x9\xFFFF\x1\x1",
				"\x1\x3\x4C\xFFFF\x1\x2\x9\xFFFF\x1\x1",
				"",
				""
			};

		private static readonly short[] DFA34_eot = DFA.UnpackEncodedString(DFA34_eotS);
		private static readonly short[] DFA34_eof = DFA.UnpackEncodedString(DFA34_eofS);
		private static readonly char[] DFA34_min = DFA.UnpackEncodedStringToUnsignedChars(DFA34_minS);
		private static readonly char[] DFA34_max = DFA.UnpackEncodedStringToUnsignedChars(DFA34_maxS);
		private static readonly short[] DFA34_accept = DFA.UnpackEncodedString(DFA34_acceptS);
		private static readonly short[] DFA34_special = DFA.UnpackEncodedString(DFA34_specialS);
		private static readonly short[][] DFA34_transition;

		static DFA34()
		{
			int numStates = DFA34_transitionS.Length;
			DFA34_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA34_transition[i] = DFA.UnpackEncodedString(DFA34_transitionS[i]);
			}
		}

		public DFA34( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 34;
			this.eot = DFA34_eot;
			this.eof = DFA34_eof;
			this.min = DFA34_min;
			this.max = DFA34_max;
			this.accept = DFA34_accept;
			this.special = DFA34_special;
			this.transition = DFA34_transition;
		}

		public override string Description { get { return "()* loopback of 135:31: ( ( SkipSpace )* ',' ( SkipSpace )* variableDeclarationNoIn )*"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private class DFA37 : DFA
	{
		private const string DFA37_eotS =
			"\x5\xFFFF";
		private const string DFA37_eofS =
			"\x1\xFFFF\x1\x3\x3\xFFFF";
		private const string DFA37_minS =
			"\x1\x3F\x2\x8\x2\xFFFF";
		private const string DFA37_maxS =
			"\x1\x3F\x2\x6C\x2\xFFFF";
		private const string DFA37_acceptS =
			"\x3\xFFFF\x1\x1\x1\x2";
		private const string DFA37_specialS =
			"\x5\xFFFF}>";
		private static readonly string[] DFA37_transitionS =
			{
				"\x1\x1",
				"\x1\x4\xC\xFFFF\x1\x3\x4C\xFFFF\x1\x3\x9\xFFFF\x1\x2",
				"\x1\x4\xC\xFFFF\x1\x3\x4C\xFFFF\x1\x3\x9\xFFFF\x1\x2",
				"",
				""
			};

		private static readonly short[] DFA37_eot = DFA.UnpackEncodedString(DFA37_eotS);
		private static readonly short[] DFA37_eof = DFA.UnpackEncodedString(DFA37_eofS);
		private static readonly char[] DFA37_min = DFA.UnpackEncodedStringToUnsignedChars(DFA37_minS);
		private static readonly char[] DFA37_max = DFA.UnpackEncodedStringToUnsignedChars(DFA37_maxS);
		private static readonly short[] DFA37_accept = DFA.UnpackEncodedString(DFA37_acceptS);
		private static readonly short[] DFA37_special = DFA.UnpackEncodedString(DFA37_specialS);
		private static readonly short[][] DFA37_transition;

		static DFA37()
		{
			int numStates = DFA37_transitionS.Length;
			DFA37_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA37_transition[i] = DFA.UnpackEncodedString(DFA37_transitionS[i]);
			}
		}

		public DFA37( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 37;
			this.eot = DFA37_eot;
			this.eof = DFA37_eof;
			this.min = DFA37_min;
			this.max = DFA37_max;
			this.accept = DFA37_accept;
			this.special = DFA37_special;
			this.transition = DFA37_transition;
		}

		public override string Description { get { return "138:1: variableDeclaration : ( Identifier |lhs= Identifier ( SkipSpace )* op= assignmentOnlyOperator ( SkipSpace )* rhs= assignmentExpression -> ^( BINARY_EXPR $lhs $op $rhs) );"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private class DFA40 : DFA
	{
		private const string DFA40_eotS =
			"\x5\xFFFF";
		private const string DFA40_eofS =
			"\x1\xFFFF\x1\x3\x3\xFFFF";
		private const string DFA40_minS =
			"\x1\x3F\x2\x8\x2\xFFFF";
		private const string DFA40_maxS =
			"\x1\x3F\x2\x6C\x2\xFFFF";
		private const string DFA40_acceptS =
			"\x3\xFFFF\x1\x1\x1\x2";
		private const string DFA40_specialS =
			"\x5\xFFFF}>";
		private static readonly string[] DFA40_transitionS =
			{
				"\x1\x1",
				"\x1\x4\xC\xFFFF\x1\x3\x22\xFFFF\x1\x3\x29\xFFFF\x1\x3\x9\xFFFF\x1\x2",
				"\x1\x4\xC\xFFFF\x1\x3\x22\xFFFF\x1\x3\x29\xFFFF\x1\x3\x9\xFFFF\x1\x2",
				"",
				""
			};

		private static readonly short[] DFA40_eot = DFA.UnpackEncodedString(DFA40_eotS);
		private static readonly short[] DFA40_eof = DFA.UnpackEncodedString(DFA40_eofS);
		private static readonly char[] DFA40_min = DFA.UnpackEncodedStringToUnsignedChars(DFA40_minS);
		private static readonly char[] DFA40_max = DFA.UnpackEncodedStringToUnsignedChars(DFA40_maxS);
		private static readonly short[] DFA40_accept = DFA.UnpackEncodedString(DFA40_acceptS);
		private static readonly short[] DFA40_special = DFA.UnpackEncodedString(DFA40_specialS);
		private static readonly short[][] DFA40_transition;

		static DFA40()
		{
			int numStates = DFA40_transitionS.Length;
			DFA40_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA40_transition[i] = DFA.UnpackEncodedString(DFA40_transitionS[i]);
			}
		}

		public DFA40( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 40;
			this.eot = DFA40_eot;
			this.eof = DFA40_eof;
			this.min = DFA40_min;
			this.max = DFA40_max;
			this.accept = DFA40_accept;
			this.special = DFA40_special;
			this.transition = DFA40_transition;
		}

		public override string Description { get { return "144:1: variableDeclarationNoIn : ( Identifier |lhs= Identifier ( SkipSpace )* op= assignmentOnlyOperator ( SkipSpace )* rhs= assignmentExpressionNoIn -> ^( BINARY_EXPR $lhs $op $rhs) );"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private class DFA86 : DFA
	{
		private const string DFA86_eotS =
			"\x5\xFFFF";
		private const string DFA86_eofS =
			"\x5\xFFFF";
		private const string DFA86_minS =
			"\x1\x5F\x2\xC\x2\xFFFF";
		private const string DFA86_maxS =
			"\x1\x5F\x2\x93\x2\xFFFF";
		private const string DFA86_acceptS =
			"\x3\xFFFF\x1\x1\x1\x2";
		private const string DFA86_specialS =
			"\x5\xFFFF}>";
		private static readonly string[] DFA86_transitionS =
			{
				"\x1\x1",
				"\x1\x3\x6\xFFFF\x1\x3\x6\xFFFF\x1\x3\x3\xFFFF\x1\x3\xA\xFFFF\x1\x3\xD"+
				"\xFFFF\x1\x3\x7\xFFFF\x1\x3\x7\xFFFF\x1\x3\x1\xFFFF\x1\x3\x5\xFFFF\x1"+
				"\x3\x2\xFFFF\x2\x3\x2\xFFFF\x1\x3\x1\xFFFF\x1\x3\x1\xFFFF\x1\x3\x7\xFFFF"+
				"\x1\x4\x9\xFFFF\x1\x2\x2\x3\x5\xFFFF\x1\x3\xA\xFFFF\x1\x3\x2\xFFFF\x1"+
				"\x3\x4\xFFFF\x1\x3\x6\xFFFF\x1\x3\x4\xFFFF\x1\x3",
				"\x1\x3\x6\xFFFF\x1\x3\x6\xFFFF\x1\x3\x3\xFFFF\x1\x3\xA\xFFFF\x1\x3\xD"+
				"\xFFFF\x1\x3\x7\xFFFF\x1\x3\x7\xFFFF\x1\x3\x1\xFFFF\x1\x3\x5\xFFFF\x1"+
				"\x3\x2\xFFFF\x2\x3\x2\xFFFF\x1\x3\x1\xFFFF\x1\x3\x1\xFFFF\x1\x3\x7\xFFFF"+
				"\x1\x4\x9\xFFFF\x1\x2\x2\x3\x5\xFFFF\x1\x3\xA\xFFFF\x1\x3\x2\xFFFF\x1"+
				"\x3\x4\xFFFF\x1\x3\x6\xFFFF\x1\x3\x4\xFFFF\x1\x3",
				"",
				""
			};

		private static readonly short[] DFA86_eot = DFA.UnpackEncodedString(DFA86_eotS);
		private static readonly short[] DFA86_eof = DFA.UnpackEncodedString(DFA86_eofS);
		private static readonly char[] DFA86_min = DFA.UnpackEncodedStringToUnsignedChars(DFA86_minS);
		private static readonly char[] DFA86_max = DFA.UnpackEncodedStringToUnsignedChars(DFA86_maxS);
		private static readonly short[] DFA86_accept = DFA.UnpackEncodedString(DFA86_acceptS);
		private static readonly short[] DFA86_special = DFA.UnpackEncodedString(DFA86_specialS);
		private static readonly short[][] DFA86_transition;

		static DFA86()
		{
			int numStates = DFA86_transitionS.Length;
			DFA86_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA86_transition[i] = DFA.UnpackEncodedString(DFA86_transitionS[i]);
			}
		}

		public DFA86( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 86;
			this.eot = DFA86_eot;
			this.eof = DFA86_eof;
			this.min = DFA86_min;
			this.max = DFA86_max;
			this.accept = DFA86_accept;
			this.special = DFA86_special;
			this.transition = DFA86_transition;
		}

		public override string Description { get { return "213:1: returnStatement : ( RETURN ( SkipSpace )* exp= expression ( SkipSpace )* SEMI -> ^( RETURN_STATEMENT $exp) | RETURN ( SkipSpace )* SEMI -> ^( RETURN_STATEMENT ) );"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private class DFA98 : DFA
	{
		private const string DFA98_eotS =
			"\x4\xFFFF";
		private const string DFA98_eofS =
			"\x4\xFFFF";
		private const string DFA98_minS =
			"\x2\x6C\x2\xFFFF";
		private const string DFA98_maxS =
			"\x2\x94\x2\xFFFF";
		private const string DFA98_acceptS =
			"\x2\xFFFF\x1\x2\x1\x1";
		private const string DFA98_specialS =
			"\x4\xFFFF}>";
		private static readonly string[] DFA98_transitionS =
			{
				"\x1\x1\x1C\xFFFF\x1\x3\x1\x2\x9\xFFFF\x1\x2",
				"\x1\x1\x1C\xFFFF\x1\x3\x1\x2\x9\xFFFF\x1\x2",
				"",
				""
			};

		private static readonly short[] DFA98_eot = DFA.UnpackEncodedString(DFA98_eotS);
		private static readonly short[] DFA98_eof = DFA.UnpackEncodedString(DFA98_eofS);
		private static readonly char[] DFA98_min = DFA.UnpackEncodedStringToUnsignedChars(DFA98_minS);
		private static readonly char[] DFA98_max = DFA.UnpackEncodedStringToUnsignedChars(DFA98_maxS);
		private static readonly short[] DFA98_accept = DFA.UnpackEncodedString(DFA98_acceptS);
		private static readonly short[] DFA98_special = DFA.UnpackEncodedString(DFA98_specialS);
		private static readonly short[][] DFA98_transition;

		static DFA98()
		{
			int numStates = DFA98_transitionS.Length;
			DFA98_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA98_transition[i] = DFA.UnpackEncodedString(DFA98_transitionS[i]);
			}
		}

		public DFA98( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 98;
			this.eot = DFA98_eot;
			this.eof = DFA98_eof;
			this.min = DFA98_min;
			this.max = DFA98_max;
			this.accept = DFA98_accept;
			this.special = DFA98_special;
			this.transition = DFA98_transition;
		}

		public override string Description { get { return "()* loopback of 233:11: ( ( SkipSpace )* caseClause )*"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private class DFA102 : DFA
	{
		private const string DFA102_eotS =
			"\x4\xFFFF";
		private const string DFA102_eofS =
			"\x4\xFFFF";
		private const string DFA102_minS =
			"\x2\x6C\x2\xFFFF";
		private const string DFA102_maxS =
			"\x2\x94\x2\xFFFF";
		private const string DFA102_acceptS =
			"\x2\xFFFF\x1\x1\x1\x2";
		private const string DFA102_specialS =
			"\x4\xFFFF}>";
		private static readonly string[] DFA102_transitionS =
			{
				"\x1\x1\x1D\xFFFF\x1\x2\x9\xFFFF\x1\x3",
				"\x1\x1\x1D\xFFFF\x1\x2\x9\xFFFF\x1\x3",
				"",
				""
			};

		private static readonly short[] DFA102_eot = DFA.UnpackEncodedString(DFA102_eotS);
		private static readonly short[] DFA102_eof = DFA.UnpackEncodedString(DFA102_eofS);
		private static readonly char[] DFA102_min = DFA.UnpackEncodedStringToUnsignedChars(DFA102_minS);
		private static readonly char[] DFA102_max = DFA.UnpackEncodedStringToUnsignedChars(DFA102_maxS);
		private static readonly short[] DFA102_accept = DFA.UnpackEncodedString(DFA102_acceptS);
		private static readonly short[] DFA102_special = DFA.UnpackEncodedString(DFA102_specialS);
		private static readonly short[][] DFA102_transition;

		static DFA102()
		{
			int numStates = DFA102_transitionS.Length;
			DFA102_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA102_transition[i] = DFA.UnpackEncodedString(DFA102_transitionS[i]);
			}
		}

		public DFA102( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 102;
			this.eot = DFA102_eot;
			this.eof = DFA102_eof;
			this.min = DFA102_min;
			this.max = DFA102_max;
			this.accept = DFA102_accept;
			this.special = DFA102_special;
			this.transition = DFA102_transition;
		}

		public override string Description { get { return "233:37: ( ( SkipSpace )* defaultClause ( ( SkipSpace )* caseClause )* )?"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private class DFA101 : DFA
	{
		private const string DFA101_eotS =
			"\x4\xFFFF";
		private const string DFA101_eofS =
			"\x4\xFFFF";
		private const string DFA101_minS =
			"\x2\x6C\x2\xFFFF";
		private const string DFA101_maxS =
			"\x2\x94\x2\xFFFF";
		private const string DFA101_acceptS =
			"\x2\xFFFF\x1\x2\x1\x1";
		private const string DFA101_specialS =
			"\x4\xFFFF}>";
		private static readonly string[] DFA101_transitionS =
			{
				"\x1\x1\x1C\xFFFF\x1\x3\xA\xFFFF\x1\x2",
				"\x1\x1\x1C\xFFFF\x1\x3\xA\xFFFF\x1\x2",
				"",
				""
			};

		private static readonly short[] DFA101_eot = DFA.UnpackEncodedString(DFA101_eotS);
		private static readonly short[] DFA101_eof = DFA.UnpackEncodedString(DFA101_eofS);
		private static readonly char[] DFA101_min = DFA.UnpackEncodedStringToUnsignedChars(DFA101_minS);
		private static readonly char[] DFA101_max = DFA.UnpackEncodedStringToUnsignedChars(DFA101_maxS);
		private static readonly short[] DFA101_accept = DFA.UnpackEncodedString(DFA101_acceptS);
		private static readonly short[] DFA101_special = DFA.UnpackEncodedString(DFA101_specialS);
		private static readonly short[][] DFA101_transition;

		static DFA101()
		{
			int numStates = DFA101_transitionS.Length;
			DFA101_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA101_transition[i] = DFA.UnpackEncodedString(DFA101_transitionS[i]);
			}
		}

		public DFA101( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 101;
			this.eot = DFA101_eot;
			this.eof = DFA101_eof;
			this.min = DFA101_min;
			this.max = DFA101_max;
			this.accept = DFA101_accept;
			this.special = DFA101_special;
			this.transition = DFA101_transition;
		}

		public override string Description { get { return "()* loopback of 233:64: ( ( SkipSpace )* caseClause )*"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private class DFA116 : DFA
	{
		private const string DFA116_eotS =
			"\x4\xFFFF";
		private const string DFA116_eofS =
			"\x2\x3\x2\xFFFF";
		private const string DFA116_minS =
			"\x2\xC\x2\xFFFF";
		private const string DFA116_maxS =
			"\x2\x94\x2\xFFFF";
		private const string DFA116_acceptS =
			"\x2\xFFFF\x1\x1\x1\x2";
		private const string DFA116_specialS =
			"\x4\xFFFF}>";
		private static readonly string[] DFA116_transitionS =
			{
				"\x1\x3\x5\xFFFF\x2\x3\x3\xFFFF\x1\x3\x2\xFFFF\x1\x3\x3\xFFFF\x1\x3\xA"+
				"\xFFFF\x1\x3\x1\x2\xC\xFFFF\x1\x3\x7\xFFFF\x1\x3\x7\xFFFF\x1\x3\x1\xFFFF"+
				"\x1\x3\x5\xFFFF\x1\x3\x2\xFFFF\x2\x3\x2\xFFFF\x1\x3\x1\xFFFF\x1\x3\x1"+
				"\xFFFF\x1\x3\x4\xFFFF\x1\x3\x2\xFFFF\x1\x3\x9\xFFFF\x1\x1\x3\x3\x1\xFFFF"+
				"\x1\x3\x2\xFFFF\x1\x3\x9\xFFFF\x2\x3\x2\xFFFF\x1\x3\x4\xFFFF\x1\x3\x1"+
				"\xFFFF\xC\x3",
				"\x1\x3\x5\xFFFF\x2\x3\x3\xFFFF\x1\x3\x2\xFFFF\x1\x3\x3\xFFFF\x1\x3\xA"+
				"\xFFFF\x1\x3\x1\x2\xC\xFFFF\x1\x3\x7\xFFFF\x1\x3\x7\xFFFF\x1\x3\x1\xFFFF"+
				"\x1\x3\x5\xFFFF\x1\x3\x2\xFFFF\x2\x3\x2\xFFFF\x1\x3\x1\xFFFF\x1\x3\x1"+
				"\xFFFF\x1\x3\x4\xFFFF\x1\x3\x2\xFFFF\x1\x3\x9\xFFFF\x1\x1\x3\x3\x1\xFFFF"+
				"\x1\x3\x2\xFFFF\x1\x3\x9\xFFFF\x2\x3\x2\xFFFF\x1\x3\x4\xFFFF\x1\x3\x1"+
				"\xFFFF\xC\x3",
				"",
				""
			};

		private static readonly short[] DFA116_eot = DFA.UnpackEncodedString(DFA116_eotS);
		private static readonly short[] DFA116_eof = DFA.UnpackEncodedString(DFA116_eofS);
		private static readonly char[] DFA116_min = DFA.UnpackEncodedStringToUnsignedChars(DFA116_minS);
		private static readonly char[] DFA116_max = DFA.UnpackEncodedStringToUnsignedChars(DFA116_maxS);
		private static readonly short[] DFA116_accept = DFA.UnpackEncodedString(DFA116_acceptS);
		private static readonly short[] DFA116_special = DFA.UnpackEncodedString(DFA116_specialS);
		private static readonly short[][] DFA116_transition;

		static DFA116()
		{
			int numStates = DFA116_transitionS.Length;
			DFA116_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA116_transition[i] = DFA.UnpackEncodedString(DFA116_transitionS[i]);
			}
		}

		public DFA116( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 116;
			this.eot = DFA116_eot;
			this.eof = DFA116_eof;
			this.min = DFA116_min;
			this.max = DFA116_max;
			this.accept = DFA116_accept;
			this.special = DFA116_special;
			this.transition = DFA116_transition;
		}

		public override string Description { get { return "250:79: ( ( SkipSpace )* finallyClause )?"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private class DFA125 : DFA
	{
		private const string DFA125_eotS =
			"\x4\xFFFF";
		private const string DFA125_eofS =
			"\x1\x2\x3\xFFFF";
		private const string DFA125_minS =
			"\x2\x15\x2\xFFFF";
		private const string DFA125_maxS =
			"\x2\x88\x2\xFFFF";
		private const string DFA125_acceptS =
			"\x2\xFFFF\x1\x2\x1\x1";
		private const string DFA125_specialS =
			"\x4\xFFFF}>";
		private static readonly string[] DFA125_transitionS =
			{
				"\x1\x3\x4C\xFFFF\x1\x2\x9\xFFFF\x1\x1\x16\xFFFF\x1\x2\x1\xFFFF\x1\x2"+
				"\x2\xFFFF\x1\x2",
				"\x1\x3\x4C\xFFFF\x1\x2\x9\xFFFF\x1\x1\x16\xFFFF\x1\x2\x1\xFFFF\x1\x2"+
				"\x2\xFFFF\x1\x2",
				"",
				""
			};

		private static readonly short[] DFA125_eot = DFA.UnpackEncodedString(DFA125_eotS);
		private static readonly short[] DFA125_eof = DFA.UnpackEncodedString(DFA125_eofS);
		private static readonly char[] DFA125_min = DFA.UnpackEncodedStringToUnsignedChars(DFA125_minS);
		private static readonly char[] DFA125_max = DFA.UnpackEncodedStringToUnsignedChars(DFA125_maxS);
		private static readonly short[] DFA125_accept = DFA.UnpackEncodedString(DFA125_acceptS);
		private static readonly short[] DFA125_special = DFA.UnpackEncodedString(DFA125_specialS);
		private static readonly short[][] DFA125_transition;

		static DFA125()
		{
			int numStates = DFA125_transitionS.Length;
			DFA125_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA125_transition[i] = DFA.UnpackEncodedString(DFA125_transitionS[i]);
			}
		}

		public DFA125( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 125;
			this.eot = DFA125_eot;
			this.eof = DFA125_eof;
			this.min = DFA125_min;
			this.max = DFA125_max;
			this.accept = DFA125_accept;
			this.special = DFA125_special;
			this.transition = DFA125_transition;
		}

		public override string Description { get { return "()* loopback of 263:28: ( ( SkipSpace )* ',' ( SkipSpace )* assignmentExpression )*"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private class DFA128 : DFA
	{
		private const string DFA128_eotS =
			"\x4\xFFFF";
		private const string DFA128_eofS =
			"\x1\x2\x3\xFFFF";
		private const string DFA128_minS =
			"\x2\x15\x2\xFFFF";
		private const string DFA128_maxS =
			"\x2\x6C\x2\xFFFF";
		private const string DFA128_acceptS =
			"\x2\xFFFF\x1\x2\x1\x1";
		private const string DFA128_specialS =
			"\x4\xFFFF}>";
		private static readonly string[] DFA128_transitionS =
			{
				"\x1\x3\x4C\xFFFF\x1\x2\x9\xFFFF\x1\x1",
				"\x1\x3\x4C\xFFFF\x1\x2\x9\xFFFF\x1\x1",
				"",
				""
			};

		private static readonly short[] DFA128_eot = DFA.UnpackEncodedString(DFA128_eotS);
		private static readonly short[] DFA128_eof = DFA.UnpackEncodedString(DFA128_eofS);
		private static readonly char[] DFA128_min = DFA.UnpackEncodedStringToUnsignedChars(DFA128_minS);
		private static readonly char[] DFA128_max = DFA.UnpackEncodedStringToUnsignedChars(DFA128_maxS);
		private static readonly short[] DFA128_accept = DFA.UnpackEncodedString(DFA128_acceptS);
		private static readonly short[] DFA128_special = DFA.UnpackEncodedString(DFA128_specialS);
		private static readonly short[][] DFA128_transition;

		static DFA128()
		{
			int numStates = DFA128_transitionS.Length;
			DFA128_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA128_transition[i] = DFA.UnpackEncodedString(DFA128_transitionS[i]);
			}
		}

		public DFA128( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 128;
			this.eot = DFA128_eot;
			this.eof = DFA128_eof;
			this.min = DFA128_min;
			this.max = DFA128_max;
			this.accept = DFA128_accept;
			this.special = DFA128_special;
			this.transition = DFA128_transition;
		}

		public override string Description { get { return "()* loopback of 268:32: ( ( SkipSpace )* ',' ( SkipSpace )* assignmentExpressionNoIn )*"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private class DFA131 : DFA
	{
		private const string DFA131_eotS =
			"\xC\xFFFF";
		private const string DFA131_eofS =
			"\xC\xFFFF";
		private const string DFA131_minS =
			"\x1\xC\x9\x0\x2\xFFFF";
		private const string DFA131_maxS =
			"\x1\x93\x9\x0\x2\xFFFF";
		private const string DFA131_acceptS =
			"\xA\xFFFF\x1\x1\x1\x2";
		private const string DFA131_specialS =
			"\x1\xFFFF\x1\x0\x1\x1\x1\x2\x1\x3\x1\x4\x1\x5\x1\x6\x1\x7\x1\x8\x2\xFFFF}>";
		private static readonly string[] DFA131_transitionS =
			{
				"\x1\xA\x6\xFFFF\x1\x4\x6\xFFFF\x1\xA\x3\xFFFF\x1\xA\xA\xFFFF\x1\x4\xD"+
				"\xFFFF\x1\x3\x7\xFFFF\x1\x2\x7\xFFFF\x1\xA\x1\xFFFF\x1\xA\x5\xFFFF\x1"+
				"\x9\x2\xFFFF\x1\xA\x1\x4\x2\xFFFF\x1\x4\x1\xFFFF\x1\xA\x1\xFFFF\x1\xA"+
				"\x12\xFFFF\x1\x4\x1\x1\x5\xFFFF\x1\xA\xA\xFFFF\x1\xA\x2\xFFFF\x1\x7"+
				"\x4\xFFFF\x1\x5\x6\xFFFF\x1\x8\x4\xFFFF\x1\x6",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"",
				""
			};

		private static readonly short[] DFA131_eot = DFA.UnpackEncodedString(DFA131_eotS);
		private static readonly short[] DFA131_eof = DFA.UnpackEncodedString(DFA131_eofS);
		private static readonly char[] DFA131_min = DFA.UnpackEncodedStringToUnsignedChars(DFA131_minS);
		private static readonly char[] DFA131_max = DFA.UnpackEncodedStringToUnsignedChars(DFA131_maxS);
		private static readonly short[] DFA131_accept = DFA.UnpackEncodedString(DFA131_acceptS);
		private static readonly short[] DFA131_special = DFA.UnpackEncodedString(DFA131_specialS);
		private static readonly short[][] DFA131_transition;

		static DFA131()
		{
			int numStates = DFA131_transitionS.Length;
			DFA131_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA131_transition[i] = DFA.UnpackEncodedString(DFA131_transitionS[i]);
			}
		}

		public DFA131( BaseRecognizer recognizer, SpecialStateTransitionHandler specialStateTransition )
			: base(specialStateTransition)
		{
			this.recognizer = recognizer;
			this.decisionNumber = 131;
			this.eot = DFA131_eot;
			this.eof = DFA131_eof;
			this.min = DFA131_min;
			this.max = DFA131_max;
			this.accept = DFA131_accept;
			this.special = DFA131_special;
			this.transition = DFA131_transition;
		}

		public override string Description { get { return "272:1: assignmentExpression : ( conditionalExpression |lhs= leftHandSideExpression ( SkipSpace )* op= assignmentOperator ( SkipSpace )* rhs= assignmentExpression -> ^( BINARY_EXPR $op $lhs $rhs) );"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private int SpecialStateTransition131(DFA dfa, int s, IIntStream _input)
	{
		ITokenStream input = (ITokenStream)_input;
		int _s = s;
		switch (s)
		{
			case 0:
				int LA131_1 = input.LA(1);


				int index131_1 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred143_JavaScript_fragment)) ) {s = 10;}

				else if ( (true) ) {s = 11;}


				input.Seek(index131_1);
				if ( s>=0 ) return s;
				break;
			case 1:
				int LA131_2 = input.LA(1);


				int index131_2 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred143_JavaScript_fragment)) ) {s = 10;}

				else if ( (true) ) {s = 11;}


				input.Seek(index131_2);
				if ( s>=0 ) return s;
				break;
			case 2:
				int LA131_3 = input.LA(1);


				int index131_3 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred143_JavaScript_fragment)) ) {s = 10;}

				else if ( (true) ) {s = 11;}


				input.Seek(index131_3);
				if ( s>=0 ) return s;
				break;
			case 3:
				int LA131_4 = input.LA(1);


				int index131_4 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred143_JavaScript_fragment)) ) {s = 10;}

				else if ( (true) ) {s = 11;}


				input.Seek(index131_4);
				if ( s>=0 ) return s;
				break;
			case 4:
				int LA131_5 = input.LA(1);


				int index131_5 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred143_JavaScript_fragment)) ) {s = 10;}

				else if ( (true) ) {s = 11;}


				input.Seek(index131_5);
				if ( s>=0 ) return s;
				break;
			case 5:
				int LA131_6 = input.LA(1);


				int index131_6 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred143_JavaScript_fragment)) ) {s = 10;}

				else if ( (true) ) {s = 11;}


				input.Seek(index131_6);
				if ( s>=0 ) return s;
				break;
			case 6:
				int LA131_7 = input.LA(1);


				int index131_7 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred143_JavaScript_fragment)) ) {s = 10;}

				else if ( (true) ) {s = 11;}


				input.Seek(index131_7);
				if ( s>=0 ) return s;
				break;
			case 7:
				int LA131_8 = input.LA(1);


				int index131_8 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred143_JavaScript_fragment)) ) {s = 10;}

				else if ( (true) ) {s = 11;}


				input.Seek(index131_8);
				if ( s>=0 ) return s;
				break;
			case 8:
				int LA131_9 = input.LA(1);


				int index131_9 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred143_JavaScript_fragment)) ) {s = 10;}

				else if ( (true) ) {s = 11;}


				input.Seek(index131_9);
				if ( s>=0 ) return s;
				break;
		}
		if (state.backtracking > 0) {state.failed=true; return -1;}
		NoViableAltException nvae = new NoViableAltException(dfa.Description, 131, _s, input);
		dfa.Error(nvae);
		throw nvae;
	}
	private class DFA134 : DFA
	{
		private const string DFA134_eotS =
			"\xC\xFFFF";
		private const string DFA134_eofS =
			"\xC\xFFFF";
		private const string DFA134_minS =
			"\x1\xC\x9\x0\x2\xFFFF";
		private const string DFA134_maxS =
			"\x1\x93\x9\x0\x2\xFFFF";
		private const string DFA134_acceptS =
			"\xA\xFFFF\x1\x1\x1\x2";
		private const string DFA134_specialS =
			"\x1\xFFFF\x1\x0\x1\x1\x1\x2\x1\x3\x1\x4\x1\x5\x1\x6\x1\x7\x1\x8\x2\xFFFF}>";
		private static readonly string[] DFA134_transitionS =
			{
				"\x1\xA\x6\xFFFF\x1\x4\x6\xFFFF\x1\xA\x3\xFFFF\x1\xA\xA\xFFFF\x1\x4\xD"+
				"\xFFFF\x1\x3\x7\xFFFF\x1\x2\x7\xFFFF\x1\xA\x1\xFFFF\x1\xA\x5\xFFFF\x1"+
				"\x9\x2\xFFFF\x1\xA\x1\x4\x2\xFFFF\x1\x4\x1\xFFFF\x1\xA\x1\xFFFF\x1\xA"+
				"\x12\xFFFF\x1\x4\x1\x1\x5\xFFFF\x1\xA\xA\xFFFF\x1\xA\x2\xFFFF\x1\x7"+
				"\x4\xFFFF\x1\x5\x6\xFFFF\x1\x8\x4\xFFFF\x1\x6",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"",
				""
			};

		private static readonly short[] DFA134_eot = DFA.UnpackEncodedString(DFA134_eotS);
		private static readonly short[] DFA134_eof = DFA.UnpackEncodedString(DFA134_eofS);
		private static readonly char[] DFA134_min = DFA.UnpackEncodedStringToUnsignedChars(DFA134_minS);
		private static readonly char[] DFA134_max = DFA.UnpackEncodedStringToUnsignedChars(DFA134_maxS);
		private static readonly short[] DFA134_accept = DFA.UnpackEncodedString(DFA134_acceptS);
		private static readonly short[] DFA134_special = DFA.UnpackEncodedString(DFA134_specialS);
		private static readonly short[][] DFA134_transition;

		static DFA134()
		{
			int numStates = DFA134_transitionS.Length;
			DFA134_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA134_transition[i] = DFA.UnpackEncodedString(DFA134_transitionS[i]);
			}
		}

		public DFA134( BaseRecognizer recognizer, SpecialStateTransitionHandler specialStateTransition )
			: base(specialStateTransition)
		{
			this.recognizer = recognizer;
			this.decisionNumber = 134;
			this.eot = DFA134_eot;
			this.eof = DFA134_eof;
			this.min = DFA134_min;
			this.max = DFA134_max;
			this.accept = DFA134_accept;
			this.special = DFA134_special;
			this.transition = DFA134_transition;
		}

		public override string Description { get { return "278:1: assignmentExpressionNoIn : ( conditionalExpressionNoIn |lhs= leftHandSideExpression ( SkipSpace )* op= assignmentOperator ( SkipSpace )* rhs= assignmentExpressionNoIn -> ^( BINARY_EXPR $op $lhs $rhs) );"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private int SpecialStateTransition134(DFA dfa, int s, IIntStream _input)
	{
		ITokenStream input = (ITokenStream)_input;
		int _s = s;
		switch (s)
		{
			case 0:
				int LA134_1 = input.LA(1);


				int index134_1 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred146_JavaScript_fragment)) ) {s = 10;}

				else if ( (true) ) {s = 11;}


				input.Seek(index134_1);
				if ( s>=0 ) return s;
				break;
			case 1:
				int LA134_2 = input.LA(1);


				int index134_2 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred146_JavaScript_fragment)) ) {s = 10;}

				else if ( (true) ) {s = 11;}


				input.Seek(index134_2);
				if ( s>=0 ) return s;
				break;
			case 2:
				int LA134_3 = input.LA(1);


				int index134_3 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred146_JavaScript_fragment)) ) {s = 10;}

				else if ( (true) ) {s = 11;}


				input.Seek(index134_3);
				if ( s>=0 ) return s;
				break;
			case 3:
				int LA134_4 = input.LA(1);


				int index134_4 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred146_JavaScript_fragment)) ) {s = 10;}

				else if ( (true) ) {s = 11;}


				input.Seek(index134_4);
				if ( s>=0 ) return s;
				break;
			case 4:
				int LA134_5 = input.LA(1);


				int index134_5 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred146_JavaScript_fragment)) ) {s = 10;}

				else if ( (true) ) {s = 11;}


				input.Seek(index134_5);
				if ( s>=0 ) return s;
				break;
			case 5:
				int LA134_6 = input.LA(1);


				int index134_6 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred146_JavaScript_fragment)) ) {s = 10;}

				else if ( (true) ) {s = 11;}


				input.Seek(index134_6);
				if ( s>=0 ) return s;
				break;
			case 6:
				int LA134_7 = input.LA(1);


				int index134_7 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred146_JavaScript_fragment)) ) {s = 10;}

				else if ( (true) ) {s = 11;}


				input.Seek(index134_7);
				if ( s>=0 ) return s;
				break;
			case 7:
				int LA134_8 = input.LA(1);


				int index134_8 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred146_JavaScript_fragment)) ) {s = 10;}

				else if ( (true) ) {s = 11;}


				input.Seek(index134_8);
				if ( s>=0 ) return s;
				break;
			case 8:
				int LA134_9 = input.LA(1);


				int index134_9 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred146_JavaScript_fragment)) ) {s = 10;}

				else if ( (true) ) {s = 11;}


				input.Seek(index134_9);
				if ( s>=0 ) return s;
				break;
		}
		if (state.backtracking > 0) {state.failed=true; return -1;}
		NoViableAltException nvae = new NoViableAltException(dfa.Description, 134, _s, input);
		dfa.Error(nvae);
		throw nvae;
	}
	private class DFA135 : DFA
	{
		private const string DFA135_eotS =
			"\xC\xFFFF";
		private const string DFA135_eofS =
			"\xC\xFFFF";
		private const string DFA135_minS =
			"\x1\x13\x9\x0\x2\xFFFF";
		private const string DFA135_maxS =
			"\x1\x93\x9\x0\x2\xFFFF";
		private const string DFA135_acceptS =
			"\xA\xFFFF\x1\x1\x1\x2";
		private const string DFA135_specialS =
			"\x1\xFFFF\x1\x0\x1\x1\x1\x2\x1\x3\x1\x4\x1\x5\x1\x6\x1\x7\x1\x8\x2\xFFFF}>";
		private static readonly string[] DFA135_transitionS =
			{
				"\x1\x4\x15\xFFFF\x1\x4\xD\xFFFF\x1\x3\x7\xFFFF\x1\x2\xF\xFFFF\x1\x9"+
				"\x3\xFFFF\x1\x4\x2\xFFFF\x1\x4\x16\xFFFF\x1\x4\x1\x1\x13\xFFFF\x1\x7"+
				"\x4\xFFFF\x1\x5\x6\xFFFF\x1\x8\x4\xFFFF\x1\x6",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"",
				""
			};

		private static readonly short[] DFA135_eot = DFA.UnpackEncodedString(DFA135_eotS);
		private static readonly short[] DFA135_eof = DFA.UnpackEncodedString(DFA135_eofS);
		private static readonly char[] DFA135_min = DFA.UnpackEncodedStringToUnsignedChars(DFA135_minS);
		private static readonly char[] DFA135_max = DFA.UnpackEncodedStringToUnsignedChars(DFA135_maxS);
		private static readonly short[] DFA135_accept = DFA.UnpackEncodedString(DFA135_acceptS);
		private static readonly short[] DFA135_special = DFA.UnpackEncodedString(DFA135_specialS);
		private static readonly short[][] DFA135_transition;

		static DFA135()
		{
			int numStates = DFA135_transitionS.Length;
			DFA135_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA135_transition[i] = DFA.UnpackEncodedString(DFA135_transitionS[i]);
			}
		}

		public DFA135( BaseRecognizer recognizer, SpecialStateTransitionHandler specialStateTransition )
			: base(specialStateTransition)
		{
			this.recognizer = recognizer;
			this.decisionNumber = 135;
			this.eot = DFA135_eot;
			this.eof = DFA135_eof;
			this.min = DFA135_min;
			this.max = DFA135_max;
			this.accept = DFA135_accept;
			this.special = DFA135_special;
			this.transition = DFA135_transition;
		}

		public override string Description { get { return "299:1: leftHandSideExpression : ( callExpression | newExpression );"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private int SpecialStateTransition135(DFA dfa, int s, IIntStream _input)
	{
		ITokenStream input = (ITokenStream)_input;
		int _s = s;
		switch (s)
		{
			case 0:
				int LA135_1 = input.LA(1);


				int index135_1 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred160_JavaScript_fragment)) ) {s = 10;}

				else if ( (true) ) {s = 11;}


				input.Seek(index135_1);
				if ( s>=0 ) return s;
				break;
			case 1:
				int LA135_2 = input.LA(1);


				int index135_2 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred160_JavaScript_fragment)) ) {s = 10;}

				else if ( (true) ) {s = 11;}


				input.Seek(index135_2);
				if ( s>=0 ) return s;
				break;
			case 2:
				int LA135_3 = input.LA(1);


				int index135_3 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred160_JavaScript_fragment)) ) {s = 10;}

				else if ( (true) ) {s = 11;}


				input.Seek(index135_3);
				if ( s>=0 ) return s;
				break;
			case 3:
				int LA135_4 = input.LA(1);


				int index135_4 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred160_JavaScript_fragment)) ) {s = 10;}

				else if ( (true) ) {s = 11;}


				input.Seek(index135_4);
				if ( s>=0 ) return s;
				break;
			case 4:
				int LA135_5 = input.LA(1);


				int index135_5 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred160_JavaScript_fragment)) ) {s = 10;}

				else if ( (true) ) {s = 11;}


				input.Seek(index135_5);
				if ( s>=0 ) return s;
				break;
			case 5:
				int LA135_6 = input.LA(1);


				int index135_6 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred160_JavaScript_fragment)) ) {s = 10;}

				else if ( (true) ) {s = 11;}


				input.Seek(index135_6);
				if ( s>=0 ) return s;
				break;
			case 6:
				int LA135_7 = input.LA(1);


				int index135_7 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred160_JavaScript_fragment)) ) {s = 10;}

				else if ( (true) ) {s = 11;}


				input.Seek(index135_7);
				if ( s>=0 ) return s;
				break;
			case 7:
				int LA135_8 = input.LA(1);


				int index135_8 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred160_JavaScript_fragment)) ) {s = 10;}

				else if ( (true) ) {s = 11;}


				input.Seek(index135_8);
				if ( s>=0 ) return s;
				break;
			case 8:
				int LA135_9 = input.LA(1);


				int index135_9 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred160_JavaScript_fragment)) ) {s = 10;}

				else if ( (true) ) {s = 11;}


				input.Seek(index135_9);
				if ( s>=0 ) return s;
				break;
		}
		if (state.backtracking > 0) {state.failed=true; return -1;}
		NoViableAltException nvae = new NoViableAltException(dfa.Description, 135, _s, input);
		dfa.Error(nvae);
		throw nvae;
	}
	private class DFA137 : DFA
	{
		private const string DFA137_eotS =
			"\xB\xFFFF";
		private const string DFA137_eofS =
			"\xB\xFFFF";
		private const string DFA137_minS =
			"\x1\x13\x8\xFFFF\x1\x0\x1\xFFFF";
		private const string DFA137_maxS =
			"\x1\x93\x8\xFFFF\x1\x0\x1\xFFFF";
		private const string DFA137_acceptS =
			"\x1\xFFFF\x1\x1\x8\xFFFF\x1\x2";
		private const string DFA137_specialS =
			"\x9\xFFFF\x1\x0\x1\xFFFF}>";
		private static readonly string[] DFA137_transitionS =
			{
				"\x1\x1\x15\xFFFF\x1\x1\xD\xFFFF\x1\x1\x7\xFFFF\x1\x1\xF\xFFFF\x1\x9"+
				"\x3\xFFFF\x1\x1\x2\xFFFF\x1\x1\x16\xFFFF\x2\x1\x13\xFFFF\x1\x1\x4\xFFFF"+
				"\x1\x1\x6\xFFFF\x1\x1\x4\xFFFF\x1\x1",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"\x1\xFFFF",
				""
			};

		private static readonly short[] DFA137_eot = DFA.UnpackEncodedString(DFA137_eotS);
		private static readonly short[] DFA137_eof = DFA.UnpackEncodedString(DFA137_eofS);
		private static readonly char[] DFA137_min = DFA.UnpackEncodedStringToUnsignedChars(DFA137_minS);
		private static readonly char[] DFA137_max = DFA.UnpackEncodedStringToUnsignedChars(DFA137_maxS);
		private static readonly short[] DFA137_accept = DFA.UnpackEncodedString(DFA137_acceptS);
		private static readonly short[] DFA137_special = DFA.UnpackEncodedString(DFA137_specialS);
		private static readonly short[][] DFA137_transition;

		static DFA137()
		{
			int numStates = DFA137_transitionS.Length;
			DFA137_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA137_transition[i] = DFA.UnpackEncodedString(DFA137_transitionS[i]);
			}
		}

		public DFA137( BaseRecognizer recognizer, SpecialStateTransitionHandler specialStateTransition )
			: base(specialStateTransition)
		{
			this.recognizer = recognizer;
			this.decisionNumber = 137;
			this.eot = DFA137_eot;
			this.eof = DFA137_eof;
			this.min = DFA137_min;
			this.max = DFA137_max;
			this.accept = DFA137_accept;
			this.special = DFA137_special;
			this.transition = DFA137_transition;
		}

		public override string Description { get { return "304:1: newExpression : ( memberExpression | 'new' ( SkipSpace )* newExpression );"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private int SpecialStateTransition137(DFA dfa, int s, IIntStream _input)
	{
		ITokenStream input = (ITokenStream)_input;
		int _s = s;
		switch (s)
		{
			case 0:
				int LA137_9 = input.LA(1);


				int index137_9 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred161_JavaScript_fragment)) ) {s = 1;}

				else if ( (true) ) {s = 10;}


				input.Seek(index137_9);
				if ( s>=0 ) return s;
				break;
		}
		if (state.backtracking > 0) {state.failed=true; return -1;}
		NoViableAltException nvae = new NoViableAltException(dfa.Description, 137, _s, input);
		dfa.Error(nvae);
		throw nvae;
	}
	private class DFA139 : DFA
	{
		private const string DFA139_eotS =
			"\x4\xFFFF";
		private const string DFA139_eofS =
			"\x1\x2\x3\xFFFF";
		private const string DFA139_minS =
			"\x2\x5\x2\xFFFF";
		private const string DFA139_maxS =
			"\x2\x94\x2\xFFFF";
		private const string DFA139_acceptS =
			"\x2\xFFFF\x1\x2\x1\x1";
		private const string DFA139_specialS =
			"\x4\xFFFF}>";
		private static readonly string[] DFA139_transitionS =
			{
				"\x1\x2\x2\xFFFF\x1\x2\x1\xFFFF\x2\x2\x1\xFFFF\x4\x2\x4\xFFFF\x1\x2\x5"+
				"\xFFFF\x2\x2\x7\xFFFF\x1\x2\xA\xFFFF\x2\x2\x7\xFFFF\x1\x2\x5\xFFFF\x1"+
				"\x2\x3\xFFFF\x2\x2\x3\xFFFF\x8\x2\x8\xFFFF\x4\x2\x3\xFFFF\x1\x2\x2\xFFFF"+
				"\x6\x2\x5\xFFFF\x1\x1\xA\xFFFF\x2\x2\x9\xFFFF\x2\x2\x1\x3\x2\x2\x1\x3"+
				"\x1\x2\xB\xFFFF\x1\x2",
				"\x1\x2\x2\xFFFF\x1\x2\x1\xFFFF\x2\x2\x1\xFFFF\x4\x2\x4\xFFFF\x1\x2\x5"+
				"\xFFFF\x2\x2\x7\xFFFF\x1\x2\xA\xFFFF\x2\x2\x7\xFFFF\x1\x2\x5\xFFFF\x1"+
				"\x2\x3\xFFFF\x2\x2\x3\xFFFF\x2\x2\x1\xFFFF\x5\x2\x8\xFFFF\x3\x2\x4\xFFFF"+
				"\x1\x2\x2\xFFFF\x6\x2\x5\xFFFF\x1\x1\xA\xFFFF\x2\x2\x9\xFFFF\x2\x2\x1"+
				"\x3\x2\x2\x1\x3\x1\x2\xB\xFFFF\x1\x2",
				"",
				""
			};

		private static readonly short[] DFA139_eot = DFA.UnpackEncodedString(DFA139_eotS);
		private static readonly short[] DFA139_eof = DFA.UnpackEncodedString(DFA139_eofS);
		private static readonly char[] DFA139_min = DFA.UnpackEncodedStringToUnsignedChars(DFA139_minS);
		private static readonly char[] DFA139_max = DFA.UnpackEncodedStringToUnsignedChars(DFA139_maxS);
		private static readonly short[] DFA139_accept = DFA.UnpackEncodedString(DFA139_acceptS);
		private static readonly short[] DFA139_special = DFA.UnpackEncodedString(DFA139_specialS);
		private static readonly short[][] DFA139_transition;

		static DFA139()
		{
			int numStates = DFA139_transitionS.Length;
			DFA139_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA139_transition[i] = DFA.UnpackEncodedString(DFA139_transitionS[i]);
			}
		}

		public DFA139( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 139;
			this.eot = DFA139_eot;
			this.eof = DFA139_eof;
			this.min = DFA139_min;
			this.max = DFA139_max;
			this.accept = DFA139_accept;
			this.special = DFA139_special;
			this.transition = DFA139_transition;
		}

		public override string Description { get { return "()* loopback of 311:9: ( ( SkipSpace )* rhs= memberExpressionSuffix -> ^( ACCESSOR_EXPR $memberExpression $rhs) )*"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private class DFA145 : DFA
	{
		private const string DFA145_eotS =
			"\x4\xFFFF";
		private const string DFA145_eofS =
			"\x1\x2\x3\xFFFF";
		private const string DFA145_minS =
			"\x2\x5\x2\xFFFF";
		private const string DFA145_maxS =
			"\x2\x94\x2\xFFFF";
		private const string DFA145_acceptS =
			"\x2\xFFFF\x1\x2\x1\x1";
		private const string DFA145_specialS =
			"\x4\xFFFF}>";
		private static readonly string[] DFA145_transitionS =
			{
				"\x1\x2\x2\xFFFF\x1\x2\x1\xFFFF\x2\x2\x1\xFFFF\x4\x2\x4\xFFFF\x1\x2\x5"+
				"\xFFFF\x2\x2\x7\xFFFF\x1\x2\xA\xFFFF\x2\x2\x7\xFFFF\x1\x2\x5\xFFFF\x1"+
				"\x2\x3\xFFFF\x2\x2\x3\xFFFF\x8\x2\x8\xFFFF\x4\x2\x3\xFFFF\x1\x2\x2\xFFFF"+
				"\x6\x2\x5\xFFFF\x1\x1\xA\xFFFF\x2\x2\x9\xFFFF\x1\x3\x1\x2\x1\x3\x2\x2"+
				"\x1\x3\x1\x2\xB\xFFFF\x1\x2",
				"\x1\x2\x2\xFFFF\x1\x2\x1\xFFFF\x2\x2\x1\xFFFF\x4\x2\x4\xFFFF\x1\x2\x5"+
				"\xFFFF\x2\x2\x7\xFFFF\x1\x2\xA\xFFFF\x2\x2\x7\xFFFF\x1\x2\x5\xFFFF\x1"+
				"\x2\x3\xFFFF\x2\x2\x3\xFFFF\x2\x2\x1\xFFFF\x5\x2\x8\xFFFF\x3\x2\x4\xFFFF"+
				"\x1\x2\x2\xFFFF\x6\x2\x5\xFFFF\x1\x1\xA\xFFFF\x2\x2\x9\xFFFF\x1\x3\x1"+
				"\x2\x1\x3\x2\x2\x1\x3\x1\x2\xB\xFFFF\x1\x2",
				"",
				""
			};

		private static readonly short[] DFA145_eot = DFA.UnpackEncodedString(DFA145_eotS);
		private static readonly short[] DFA145_eof = DFA.UnpackEncodedString(DFA145_eofS);
		private static readonly char[] DFA145_min = DFA.UnpackEncodedStringToUnsignedChars(DFA145_minS);
		private static readonly char[] DFA145_max = DFA.UnpackEncodedStringToUnsignedChars(DFA145_maxS);
		private static readonly short[] DFA145_accept = DFA.UnpackEncodedString(DFA145_acceptS);
		private static readonly short[] DFA145_special = DFA.UnpackEncodedString(DFA145_specialS);
		private static readonly short[][] DFA145_transition;

		static DFA145()
		{
			int numStates = DFA145_transitionS.Length;
			DFA145_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA145_transition[i] = DFA.UnpackEncodedString(DFA145_transitionS[i]);
			}
		}

		public DFA145( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 145;
			this.eot = DFA145_eot;
			this.eof = DFA145_eof;
			this.min = DFA145_min;
			this.max = DFA145_max;
			this.accept = DFA145_accept;
			this.special = DFA145_special;
			this.transition = DFA145_transition;
		}

		public override string Description { get { return "()* loopback of 332:9: ( ( SkipSpace )* rhs= callExpressionSuffix -> ^( ACCESSOR_EXPR $callExpression $rhs) )*"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private class DFA152 : DFA
	{
		private const string DFA152_eotS =
			"\x4\xFFFF";
		private const string DFA152_eofS =
			"\x4\xFFFF";
		private const string DFA152_minS =
			"\x2\xC\x2\xFFFF";
		private const string DFA152_maxS =
			"\x2\x93\x2\xFFFF";
		private const string DFA152_acceptS =
			"\x2\xFFFF\x1\x1\x1\x2";
		private const string DFA152_specialS =
			"\x4\xFFFF}>";
		private static readonly string[] DFA152_transitionS =
			{
				"\x1\x2\x6\xFFFF\x1\x2\x6\xFFFF\x1\x2\x3\xFFFF\x1\x2\xA\xFFFF\x1\x2\xD"+
				"\xFFFF\x1\x2\x7\xFFFF\x1\x2\x7\xFFFF\x1\x2\x1\xFFFF\x1\x2\x5\xFFFF\x1"+
				"\x2\x2\xFFFF\x2\x2\x2\xFFFF\x1\x2\x1\xFFFF\x1\x2\x1\xFFFF\x1\x2\x11"+
				"\xFFFF\x1\x1\x2\x2\x5\xFFFF\x1\x2\xA\xFFFF\x1\x2\x2\xFFFF\x1\x2\x1\x3"+
				"\x3\xFFFF\x1\x2\x6\xFFFF\x1\x2\x4\xFFFF\x1\x2",
				"\x1\x2\x6\xFFFF\x1\x2\x6\xFFFF\x1\x2\x3\xFFFF\x1\x2\xA\xFFFF\x1\x2\xD"+
				"\xFFFF\x1\x2\x7\xFFFF\x1\x2\x7\xFFFF\x1\x2\x1\xFFFF\x1\x2\x5\xFFFF\x1"+
				"\x2\x2\xFFFF\x2\x2\x2\xFFFF\x1\x2\x1\xFFFF\x1\x2\x1\xFFFF\x1\x2\x11"+
				"\xFFFF\x1\x1\x2\x2\x5\xFFFF\x1\x2\xA\xFFFF\x1\x2\x2\xFFFF\x1\x2\x1\x3"+
				"\x3\xFFFF\x1\x2\x6\xFFFF\x1\x2\x4\xFFFF\x1\x2",
				"",
				""
			};

		private static readonly short[] DFA152_eot = DFA.UnpackEncodedString(DFA152_eotS);
		private static readonly short[] DFA152_eof = DFA.UnpackEncodedString(DFA152_eofS);
		private static readonly char[] DFA152_min = DFA.UnpackEncodedStringToUnsignedChars(DFA152_minS);
		private static readonly char[] DFA152_max = DFA.UnpackEncodedStringToUnsignedChars(DFA152_maxS);
		private static readonly short[] DFA152_accept = DFA.UnpackEncodedString(DFA152_acceptS);
		private static readonly short[] DFA152_special = DFA.UnpackEncodedString(DFA152_specialS);
		private static readonly short[][] DFA152_transition;

		static DFA152()
		{
			int numStates = DFA152_transitionS.Length;
			DFA152_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA152_transition[i] = DFA.UnpackEncodedString(DFA152_transitionS[i]);
			}
		}

		public DFA152( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 152;
			this.eot = DFA152_eot;
			this.eof = DFA152_eof;
			this.min = DFA152_min;
			this.max = DFA152_max;
			this.accept = DFA152_accept;
			this.special = DFA152_special;
			this.transition = DFA152_transition;
		}

		public override string Description { get { return "347:11: ( ( SkipSpace )* assignmentExpression ( ( SkipSpace )* ',' ( SkipSpace )* assignmentExpression )* )?"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private class DFA151 : DFA
	{
		private const string DFA151_eotS =
			"\x4\xFFFF";
		private const string DFA151_eofS =
			"\x4\xFFFF";
		private const string DFA151_minS =
			"\x2\x15\x2\xFFFF";
		private const string DFA151_maxS =
			"\x2\x83\x2\xFFFF";
		private const string DFA151_acceptS =
			"\x2\xFFFF\x1\x2\x1\x1";
		private const string DFA151_specialS =
			"\x4\xFFFF}>";
		private static readonly string[] DFA151_transitionS =
			{
				"\x1\x3\x56\xFFFF\x1\x1\x16\xFFFF\x1\x2",
				"\x1\x3\x56\xFFFF\x1\x1\x16\xFFFF\x1\x2",
				"",
				""
			};

		private static readonly short[] DFA151_eot = DFA.UnpackEncodedString(DFA151_eotS);
		private static readonly short[] DFA151_eof = DFA.UnpackEncodedString(DFA151_eofS);
		private static readonly char[] DFA151_min = DFA.UnpackEncodedStringToUnsignedChars(DFA151_minS);
		private static readonly char[] DFA151_max = DFA.UnpackEncodedStringToUnsignedChars(DFA151_maxS);
		private static readonly short[] DFA151_accept = DFA.UnpackEncodedString(DFA151_acceptS);
		private static readonly short[] DFA151_special = DFA.UnpackEncodedString(DFA151_specialS);
		private static readonly short[][] DFA151_transition;

		static DFA151()
		{
			int numStates = DFA151_transitionS.Length;
			DFA151_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA151_transition[i] = DFA.UnpackEncodedString(DFA151_transitionS[i]);
			}
		}

		public DFA151( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 151;
			this.eot = DFA151_eot;
			this.eof = DFA151_eof;
			this.min = DFA151_min;
			this.max = DFA151_max;
			this.accept = DFA151_accept;
			this.special = DFA151_special;
			this.transition = DFA151_transition;
		}

		public override string Description { get { return "()* loopback of 347:44: ( ( SkipSpace )* ',' ( SkipSpace )* assignmentExpression )*"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private class DFA158 : DFA
	{
		private const string DFA158_eotS =
			"\x5\xFFFF";
		private const string DFA158_eofS =
			"\x5\xFFFF";
		private const string DFA158_minS =
			"\x1\x84\x2\x37\x2\xFFFF";
		private const string DFA158_maxS =
			"\x1\x84\x2\x6C\x2\xFFFF";
		private const string DFA158_acceptS =
			"\x3\xFFFF\x1\x1\x1\x2";
		private const string DFA158_specialS =
			"\x5\xFFFF}>";
		private static readonly string[] DFA158_transitionS =
			{
				"\x1\x1",
				"\x1\x4\x7\xFFFF\x1\x3\x2C\xFFFF\x1\x2",
				"\x1\x4\x7\xFFFF\x1\x3\x2C\xFFFF\x1\x2",
				"",
				""
			};

		private static readonly short[] DFA158_eot = DFA.UnpackEncodedString(DFA158_eotS);
		private static readonly short[] DFA158_eof = DFA.UnpackEncodedString(DFA158_eofS);
		private static readonly char[] DFA158_min = DFA.UnpackEncodedStringToUnsignedChars(DFA158_minS);
		private static readonly char[] DFA158_max = DFA.UnpackEncodedStringToUnsignedChars(DFA158_maxS);
		private static readonly short[] DFA158_accept = DFA.UnpackEncodedString(DFA158_acceptS);
		private static readonly short[] DFA158_special = DFA.UnpackEncodedString(DFA158_specialS);
		private static readonly short[][] DFA158_transition;

		static DFA158()
		{
			int numStates = DFA158_transitionS.Length;
			DFA158_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA158_transition[i] = DFA.UnpackEncodedString(DFA158_transitionS[i]);
			}
		}

		public DFA158( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 158;
			this.eot = DFA158_eot;
			this.eof = DFA158_eof;
			this.min = DFA158_min;
			this.max = DFA158_max;
			this.accept = DFA158_accept;
			this.special = DFA158_special;
			this.transition = DFA158_transition;
		}

		public override string Description { get { return "356:1: propertyReferenceSuffix : ( '.' ( SkipSpace )* id= Identifier -> ^( DOT_ACCESSOR_EXPR $id) | '.' ( SkipSpace )* importId= importedNotation -> ^( INSTANCE_JSNI $importId) );"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private class DFA163 : DFA
	{
		private const string DFA163_eotS =
			"\xD\xFFFF";
		private const string DFA163_eofS =
			"\xD\xFFFF";
		private const string DFA163_minS =
			"\x1\xC\xA\x0\x2\xFFFF";
		private const string DFA163_maxS =
			"\x1\x93\xA\x0\x2\xFFFF";
		private const string DFA163_acceptS =
			"\xB\xFFFF\x1\x1\x1\x2";
		private const string DFA163_specialS =
			"\x1\xFFFF\x1\x0\x1\x1\x1\x2\x1\x3\x1\x4\x1\x5\x1\x6\x1\x7\x1\x8\x1\x9"+
			"\x2\xFFFF}>";
		private static readonly string[] DFA163_transitionS =
			{
				"\x1\xA\x6\xFFFF\x1\x4\x6\xFFFF\x1\xA\x3\xFFFF\x1\xA\xA\xFFFF\x1\x4\xD"+
				"\xFFFF\x1\x3\x7\xFFFF\x1\x2\x7\xFFFF\x1\xA\x1\xFFFF\x1\xA\x5\xFFFF\x1"+
				"\x9\x2\xFFFF\x1\xA\x1\x4\x2\xFFFF\x1\x4\x1\xFFFF\x1\xA\x1\xFFFF\x1\xA"+
				"\x12\xFFFF\x1\x4\x1\x1\x5\xFFFF\x1\xA\xA\xFFFF\x1\xA\x2\xFFFF\x1\x7"+
				"\x4\xFFFF\x1\x5\x6\xFFFF\x1\x8\x4\xFFFF\x1\x6",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"",
				""
			};

		private static readonly short[] DFA163_eot = DFA.UnpackEncodedString(DFA163_eotS);
		private static readonly short[] DFA163_eof = DFA.UnpackEncodedString(DFA163_eofS);
		private static readonly char[] DFA163_min = DFA.UnpackEncodedStringToUnsignedChars(DFA163_minS);
		private static readonly char[] DFA163_max = DFA.UnpackEncodedStringToUnsignedChars(DFA163_maxS);
		private static readonly short[] DFA163_accept = DFA.UnpackEncodedString(DFA163_acceptS);
		private static readonly short[] DFA163_special = DFA.UnpackEncodedString(DFA163_specialS);
		private static readonly short[][] DFA163_transition;

		static DFA163()
		{
			int numStates = DFA163_transitionS.Length;
			DFA163_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA163_transition[i] = DFA.UnpackEncodedString(DFA163_transitionS[i]);
			}
		}

		public DFA163( BaseRecognizer recognizer, SpecialStateTransitionHandler specialStateTransition )
			: base(specialStateTransition)
		{
			this.recognizer = recognizer;
			this.decisionNumber = 163;
			this.eot = DFA163_eot;
			this.eof = DFA163_eof;
			this.min = DFA163_min;
			this.max = DFA163_max;
			this.accept = DFA163_accept;
			this.special = DFA163_special;
			this.transition = DFA163_transition;
		}

		public override string Description { get { return "363:1: conditionalExpression : (cond= logicalORExpression ( SkipSpace )* '?' ( SkipSpace )* tV= assignmentExpression ( SkipSpace )* ':' ( SkipSpace )* fV= assignmentExpression -> ^( CONDITIONAL_EXPR $cond $tV $fV) | logicalORExpression );"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private int SpecialStateTransition163(DFA dfa, int s, IIntStream _input)
	{
		ITokenStream input = (ITokenStream)_input;
		int _s = s;
		switch (s)
		{
			case 0:
				int LA163_1 = input.LA(1);


				int index163_1 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred190_JavaScript_fragment)) ) {s = 11;}

				else if ( (true) ) {s = 12;}


				input.Seek(index163_1);
				if ( s>=0 ) return s;
				break;
			case 1:
				int LA163_2 = input.LA(1);


				int index163_2 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred190_JavaScript_fragment)) ) {s = 11;}

				else if ( (true) ) {s = 12;}


				input.Seek(index163_2);
				if ( s>=0 ) return s;
				break;
			case 2:
				int LA163_3 = input.LA(1);


				int index163_3 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred190_JavaScript_fragment)) ) {s = 11;}

				else if ( (true) ) {s = 12;}


				input.Seek(index163_3);
				if ( s>=0 ) return s;
				break;
			case 3:
				int LA163_4 = input.LA(1);


				int index163_4 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred190_JavaScript_fragment)) ) {s = 11;}

				else if ( (true) ) {s = 12;}


				input.Seek(index163_4);
				if ( s>=0 ) return s;
				break;
			case 4:
				int LA163_5 = input.LA(1);


				int index163_5 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred190_JavaScript_fragment)) ) {s = 11;}

				else if ( (true) ) {s = 12;}


				input.Seek(index163_5);
				if ( s>=0 ) return s;
				break;
			case 5:
				int LA163_6 = input.LA(1);


				int index163_6 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred190_JavaScript_fragment)) ) {s = 11;}

				else if ( (true) ) {s = 12;}


				input.Seek(index163_6);
				if ( s>=0 ) return s;
				break;
			case 6:
				int LA163_7 = input.LA(1);


				int index163_7 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred190_JavaScript_fragment)) ) {s = 11;}

				else if ( (true) ) {s = 12;}


				input.Seek(index163_7);
				if ( s>=0 ) return s;
				break;
			case 7:
				int LA163_8 = input.LA(1);


				int index163_8 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred190_JavaScript_fragment)) ) {s = 11;}

				else if ( (true) ) {s = 12;}


				input.Seek(index163_8);
				if ( s>=0 ) return s;
				break;
			case 8:
				int LA163_9 = input.LA(1);


				int index163_9 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred190_JavaScript_fragment)) ) {s = 11;}

				else if ( (true) ) {s = 12;}


				input.Seek(index163_9);
				if ( s>=0 ) return s;
				break;
			case 9:
				int LA163_10 = input.LA(1);


				int index163_10 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred190_JavaScript_fragment)) ) {s = 11;}

				else if ( (true) ) {s = 12;}


				input.Seek(index163_10);
				if ( s>=0 ) return s;
				break;
		}
		if (state.backtracking > 0) {state.failed=true; return -1;}
		NoViableAltException nvae = new NoViableAltException(dfa.Description, 163, _s, input);
		dfa.Error(nvae);
		throw nvae;
	}
	private class DFA168 : DFA
	{
		private const string DFA168_eotS =
			"\xD\xFFFF";
		private const string DFA168_eofS =
			"\xD\xFFFF";
		private const string DFA168_minS =
			"\x1\xC\xA\x0\x2\xFFFF";
		private const string DFA168_maxS =
			"\x1\x93\xA\x0\x2\xFFFF";
		private const string DFA168_acceptS =
			"\xB\xFFFF\x1\x1\x1\x2";
		private const string DFA168_specialS =
			"\x1\xFFFF\x1\x0\x1\x1\x1\x2\x1\x3\x1\x4\x1\x5\x1\x6\x1\x7\x1\x8\x1\x9"+
			"\x2\xFFFF}>";
		private static readonly string[] DFA168_transitionS =
			{
				"\x1\xA\x6\xFFFF\x1\x4\x6\xFFFF\x1\xA\x3\xFFFF\x1\xA\xA\xFFFF\x1\x4\xD"+
				"\xFFFF\x1\x3\x7\xFFFF\x1\x2\x7\xFFFF\x1\xA\x1\xFFFF\x1\xA\x5\xFFFF\x1"+
				"\x9\x2\xFFFF\x1\xA\x1\x4\x2\xFFFF\x1\x4\x1\xFFFF\x1\xA\x1\xFFFF\x1\xA"+
				"\x12\xFFFF\x1\x4\x1\x1\x5\xFFFF\x1\xA\xA\xFFFF\x1\xA\x2\xFFFF\x1\x7"+
				"\x4\xFFFF\x1\x5\x6\xFFFF\x1\x8\x4\xFFFF\x1\x6",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"",
				""
			};

		private static readonly short[] DFA168_eot = DFA.UnpackEncodedString(DFA168_eotS);
		private static readonly short[] DFA168_eof = DFA.UnpackEncodedString(DFA168_eofS);
		private static readonly char[] DFA168_min = DFA.UnpackEncodedStringToUnsignedChars(DFA168_minS);
		private static readonly char[] DFA168_max = DFA.UnpackEncodedStringToUnsignedChars(DFA168_maxS);
		private static readonly short[] DFA168_accept = DFA.UnpackEncodedString(DFA168_acceptS);
		private static readonly short[] DFA168_special = DFA.UnpackEncodedString(DFA168_specialS);
		private static readonly short[][] DFA168_transition;

		static DFA168()
		{
			int numStates = DFA168_transitionS.Length;
			DFA168_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA168_transition[i] = DFA.UnpackEncodedString(DFA168_transitionS[i]);
			}
		}

		public DFA168( BaseRecognizer recognizer, SpecialStateTransitionHandler specialStateTransition )
			: base(specialStateTransition)
		{
			this.recognizer = recognizer;
			this.decisionNumber = 168;
			this.eot = DFA168_eot;
			this.eof = DFA168_eof;
			this.min = DFA168_min;
			this.max = DFA168_max;
			this.accept = DFA168_accept;
			this.special = DFA168_special;
			this.transition = DFA168_transition;
		}

		public override string Description { get { return "369:1: conditionalExpressionNoIn : (cond= logicalORExpressionNoIn ( SkipSpace )* '?' ( SkipSpace )* tV= assignmentExpressionNoIn ( SkipSpace )* ':' ( SkipSpace )* fV= assignmentExpressionNoIn -> ^( CONDITIONAL_EXPR $cond $tV $fV) | logicalORExpressionNoIn );"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private int SpecialStateTransition168(DFA dfa, int s, IIntStream _input)
	{
		ITokenStream input = (ITokenStream)_input;
		int _s = s;
		switch (s)
		{
			case 0:
				int LA168_1 = input.LA(1);


				int index168_1 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred195_JavaScript_fragment)) ) {s = 11;}

				else if ( (true) ) {s = 12;}


				input.Seek(index168_1);
				if ( s>=0 ) return s;
				break;
			case 1:
				int LA168_2 = input.LA(1);


				int index168_2 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred195_JavaScript_fragment)) ) {s = 11;}

				else if ( (true) ) {s = 12;}


				input.Seek(index168_2);
				if ( s>=0 ) return s;
				break;
			case 2:
				int LA168_3 = input.LA(1);


				int index168_3 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred195_JavaScript_fragment)) ) {s = 11;}

				else if ( (true) ) {s = 12;}


				input.Seek(index168_3);
				if ( s>=0 ) return s;
				break;
			case 3:
				int LA168_4 = input.LA(1);


				int index168_4 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred195_JavaScript_fragment)) ) {s = 11;}

				else if ( (true) ) {s = 12;}


				input.Seek(index168_4);
				if ( s>=0 ) return s;
				break;
			case 4:
				int LA168_5 = input.LA(1);


				int index168_5 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred195_JavaScript_fragment)) ) {s = 11;}

				else if ( (true) ) {s = 12;}


				input.Seek(index168_5);
				if ( s>=0 ) return s;
				break;
			case 5:
				int LA168_6 = input.LA(1);


				int index168_6 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred195_JavaScript_fragment)) ) {s = 11;}

				else if ( (true) ) {s = 12;}


				input.Seek(index168_6);
				if ( s>=0 ) return s;
				break;
			case 6:
				int LA168_7 = input.LA(1);


				int index168_7 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred195_JavaScript_fragment)) ) {s = 11;}

				else if ( (true) ) {s = 12;}


				input.Seek(index168_7);
				if ( s>=0 ) return s;
				break;
			case 7:
				int LA168_8 = input.LA(1);


				int index168_8 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred195_JavaScript_fragment)) ) {s = 11;}

				else if ( (true) ) {s = 12;}


				input.Seek(index168_8);
				if ( s>=0 ) return s;
				break;
			case 8:
				int LA168_9 = input.LA(1);


				int index168_9 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred195_JavaScript_fragment)) ) {s = 11;}

				else if ( (true) ) {s = 12;}


				input.Seek(index168_9);
				if ( s>=0 ) return s;
				break;
			case 9:
				int LA168_10 = input.LA(1);


				int index168_10 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred195_JavaScript_fragment)) ) {s = 11;}

				else if ( (true) ) {s = 12;}


				input.Seek(index168_10);
				if ( s>=0 ) return s;
				break;
		}
		if (state.backtracking > 0) {state.failed=true; return -1;}
		NoViableAltException nvae = new NoViableAltException(dfa.Description, 168, _s, input);
		dfa.Error(nvae);
		throw nvae;
	}
	private class DFA171 : DFA
	{
		private const string DFA171_eotS =
			"\x4\xFFFF";
		private const string DFA171_eofS =
			"\x1\x2\x3\xFFFF";
		private const string DFA171_minS =
			"\x2\x15\x2\xFFFF";
		private const string DFA171_maxS =
			"\x2\x94\x2\xFFFF";
		private const string DFA171_acceptS =
			"\x2\xFFFF\x1\x2\x1\x1";
		private const string DFA171_specialS =
			"\x4\xFFFF}>";
		private static readonly string[] DFA171_transitionS =
			{
				"\x1\x2\x41\xFFFF\x1\x3\xA\xFFFF\x1\x2\x9\xFFFF\x1\x1\x16\xFFFF\x1\x2"+
				"\x1\xFFFF\x2\x2\x1\xFFFF\x1\x2\xB\xFFFF\x1\x2",
				"\x1\x2\x41\xFFFF\x1\x3\xA\xFFFF\x1\x2\x9\xFFFF\x1\x1\x16\xFFFF\x1\x2"+
				"\x1\xFFFF\x2\x2\x1\xFFFF\x1\x2\xB\xFFFF\x1\x2",
				"",
				""
			};

		private static readonly short[] DFA171_eot = DFA.UnpackEncodedString(DFA171_eotS);
		private static readonly short[] DFA171_eof = DFA.UnpackEncodedString(DFA171_eofS);
		private static readonly char[] DFA171_min = DFA.UnpackEncodedStringToUnsignedChars(DFA171_minS);
		private static readonly char[] DFA171_max = DFA.UnpackEncodedStringToUnsignedChars(DFA171_maxS);
		private static readonly short[] DFA171_accept = DFA.UnpackEncodedString(DFA171_acceptS);
		private static readonly short[] DFA171_special = DFA.UnpackEncodedString(DFA171_specialS);
		private static readonly short[][] DFA171_transition;

		static DFA171()
		{
			int numStates = DFA171_transitionS.Length;
			DFA171_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA171_transition[i] = DFA.UnpackEncodedString(DFA171_transitionS[i]);
			}
		}

		public DFA171( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 171;
			this.eot = DFA171_eot;
			this.eof = DFA171_eof;
			this.min = DFA171_min;
			this.max = DFA171_max;
			this.accept = DFA171_accept;
			this.special = DFA171_special;
			this.transition = DFA171_transition;
		}

		public override string Description { get { return "()* loopback of 377:9: ( ( SkipSpace )* op= OR ( SkipSpace )* rhs= logicalANDExpression -> ^( BINARY_EXPR $op $logicalORExpression $rhs) )*"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private class DFA174 : DFA
	{
		private const string DFA174_eotS =
			"\x4\xFFFF";
		private const string DFA174_eofS =
			"\x1\x2\x3\xFFFF";
		private const string DFA174_minS =
			"\x2\x15\x2\xFFFF";
		private const string DFA174_maxS =
			"\x2\x86\x2\xFFFF";
		private const string DFA174_acceptS =
			"\x2\xFFFF\x1\x2\x1\x1";
		private const string DFA174_specialS =
			"\x4\xFFFF}>";
		private static readonly string[] DFA174_transitionS =
			{
				"\x1\x2\x22\xFFFF\x1\x2\x1E\xFFFF\x1\x3\xA\xFFFF\x1\x2\x9\xFFFF\x1\x1"+
				"\x18\xFFFF\x2\x2",
				"\x1\x2\x22\xFFFF\x1\x2\x1E\xFFFF\x1\x3\xA\xFFFF\x1\x2\x9\xFFFF\x1\x1"+
				"\x18\xFFFF\x2\x2",
				"",
				""
			};

		private static readonly short[] DFA174_eot = DFA.UnpackEncodedString(DFA174_eotS);
		private static readonly short[] DFA174_eof = DFA.UnpackEncodedString(DFA174_eofS);
		private static readonly char[] DFA174_min = DFA.UnpackEncodedStringToUnsignedChars(DFA174_minS);
		private static readonly char[] DFA174_max = DFA.UnpackEncodedStringToUnsignedChars(DFA174_maxS);
		private static readonly short[] DFA174_accept = DFA.UnpackEncodedString(DFA174_acceptS);
		private static readonly short[] DFA174_special = DFA.UnpackEncodedString(DFA174_specialS);
		private static readonly short[][] DFA174_transition;

		static DFA174()
		{
			int numStates = DFA174_transitionS.Length;
			DFA174_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA174_transition[i] = DFA.UnpackEncodedString(DFA174_transitionS[i]);
			}
		}

		public DFA174( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 174;
			this.eot = DFA174_eot;
			this.eof = DFA174_eof;
			this.min = DFA174_min;
			this.max = DFA174_max;
			this.accept = DFA174_accept;
			this.special = DFA174_special;
			this.transition = DFA174_transition;
		}

		public override string Description { get { return "()* loopback of 382:9: ( ( SkipSpace )* op= OR ( SkipSpace )* rhs= logicalANDExpressionNoIn -> ^( BINARY_EXPR $op $logicalORExpressionNoIn $rhs) )*"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private class DFA177 : DFA
	{
		private const string DFA177_eotS =
			"\x4\xFFFF";
		private const string DFA177_eofS =
			"\x1\x2\x3\xFFFF";
		private const string DFA177_minS =
			"\x2\x5\x2\xFFFF";
		private const string DFA177_maxS =
			"\x2\x94\x2\xFFFF";
		private const string DFA177_acceptS =
			"\x2\xFFFF\x1\x2\x1\x1";
		private const string DFA177_specialS =
			"\x4\xFFFF}>";
		private static readonly string[] DFA177_transitionS =
			{
				"\x1\x3\xF\xFFFF\x1\x2\x41\xFFFF\x1\x2\xA\xFFFF\x1\x2\x9\xFFFF\x1\x1"+
				"\x16\xFFFF\x1\x2\x1\xFFFF\x2\x2\x1\xFFFF\x1\x2\xB\xFFFF\x1\x2",
				"\x1\x3\xF\xFFFF\x1\x2\x41\xFFFF\x1\x2\xA\xFFFF\x1\x2\x9\xFFFF\x1\x1"+
				"\x16\xFFFF\x1\x2\x1\xFFFF\x2\x2\x1\xFFFF\x1\x2\xB\xFFFF\x1\x2",
				"",
				""
			};

		private static readonly short[] DFA177_eot = DFA.UnpackEncodedString(DFA177_eotS);
		private static readonly short[] DFA177_eof = DFA.UnpackEncodedString(DFA177_eofS);
		private static readonly char[] DFA177_min = DFA.UnpackEncodedStringToUnsignedChars(DFA177_minS);
		private static readonly char[] DFA177_max = DFA.UnpackEncodedStringToUnsignedChars(DFA177_maxS);
		private static readonly short[] DFA177_accept = DFA.UnpackEncodedString(DFA177_acceptS);
		private static readonly short[] DFA177_special = DFA.UnpackEncodedString(DFA177_specialS);
		private static readonly short[][] DFA177_transition;

		static DFA177()
		{
			int numStates = DFA177_transitionS.Length;
			DFA177_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA177_transition[i] = DFA.UnpackEncodedString(DFA177_transitionS[i]);
			}
		}

		public DFA177( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 177;
			this.eot = DFA177_eot;
			this.eof = DFA177_eof;
			this.min = DFA177_min;
			this.max = DFA177_max;
			this.accept = DFA177_accept;
			this.special = DFA177_special;
			this.transition = DFA177_transition;
		}

		public override string Description { get { return "()* loopback of 387:9: ( ( SkipSpace )* op= AND ( SkipSpace )* rhs= bitwiseORExpression -> ^( BINARY_EXPR $op $logicalANDExpression $rhs) )*"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private class DFA180 : DFA
	{
		private const string DFA180_eotS =
			"\x4\xFFFF";
		private const string DFA180_eofS =
			"\x1\x2\x3\xFFFF";
		private const string DFA180_minS =
			"\x2\x5\x2\xFFFF";
		private const string DFA180_maxS =
			"\x2\x86\x2\xFFFF";
		private const string DFA180_acceptS =
			"\x2\xFFFF\x1\x2\x1\x1";
		private const string DFA180_specialS =
			"\x4\xFFFF}>";
		private static readonly string[] DFA180_transitionS =
			{
				"\x1\x3\xF\xFFFF\x1\x2\x22\xFFFF\x1\x2\x1E\xFFFF\x1\x2\xA\xFFFF\x1\x2"+
				"\x9\xFFFF\x1\x1\x18\xFFFF\x2\x2",
				"\x1\x3\xF\xFFFF\x1\x2\x22\xFFFF\x1\x2\x1E\xFFFF\x1\x2\xA\xFFFF\x1\x2"+
				"\x9\xFFFF\x1\x1\x18\xFFFF\x2\x2",
				"",
				""
			};

		private static readonly short[] DFA180_eot = DFA.UnpackEncodedString(DFA180_eotS);
		private static readonly short[] DFA180_eof = DFA.UnpackEncodedString(DFA180_eofS);
		private static readonly char[] DFA180_min = DFA.UnpackEncodedStringToUnsignedChars(DFA180_minS);
		private static readonly char[] DFA180_max = DFA.UnpackEncodedStringToUnsignedChars(DFA180_maxS);
		private static readonly short[] DFA180_accept = DFA.UnpackEncodedString(DFA180_acceptS);
		private static readonly short[] DFA180_special = DFA.UnpackEncodedString(DFA180_specialS);
		private static readonly short[][] DFA180_transition;

		static DFA180()
		{
			int numStates = DFA180_transitionS.Length;
			DFA180_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA180_transition[i] = DFA.UnpackEncodedString(DFA180_transitionS[i]);
			}
		}

		public DFA180( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 180;
			this.eot = DFA180_eot;
			this.eof = DFA180_eof;
			this.min = DFA180_min;
			this.max = DFA180_max;
			this.accept = DFA180_accept;
			this.special = DFA180_special;
			this.transition = DFA180_transition;
		}

		public override string Description { get { return "()* loopback of 392:9: ( ( SkipSpace )* op= AND ( SkipSpace )* rhs= bitwiseORExpressionNoIn -> ^( BINARY_EXPR $op $logicalANDExpressionNoIn $rhs) )*"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private class DFA183 : DFA
	{
		private const string DFA183_eotS =
			"\x4\xFFFF";
		private const string DFA183_eofS =
			"\x1\x2\x3\xFFFF";
		private const string DFA183_minS =
			"\x2\x5\x2\xFFFF";
		private const string DFA183_maxS =
			"\x2\x94\x2\xFFFF";
		private const string DFA183_acceptS =
			"\x2\xFFFF\x1\x2\x1\x1";
		private const string DFA183_specialS =
			"\x4\xFFFF}>";
		private static readonly string[] DFA183_transitionS =
			{
				"\x1\x2\x7\xFFFF\x1\x3\x7\xFFFF\x1\x2\x41\xFFFF\x1\x2\xA\xFFFF\x1\x2"+
				"\x9\xFFFF\x1\x1\x16\xFFFF\x1\x2\x1\xFFFF\x2\x2\x1\xFFFF\x1\x2\xB\xFFFF"+
				"\x1\x2",
				"\x1\x2\x7\xFFFF\x1\x3\x7\xFFFF\x1\x2\x41\xFFFF\x1\x2\xA\xFFFF\x1\x2"+
				"\x9\xFFFF\x1\x1\x16\xFFFF\x1\x2\x1\xFFFF\x2\x2\x1\xFFFF\x1\x2\xB\xFFFF"+
				"\x1\x2",
				"",
				""
			};

		private static readonly short[] DFA183_eot = DFA.UnpackEncodedString(DFA183_eotS);
		private static readonly short[] DFA183_eof = DFA.UnpackEncodedString(DFA183_eofS);
		private static readonly char[] DFA183_min = DFA.UnpackEncodedStringToUnsignedChars(DFA183_minS);
		private static readonly char[] DFA183_max = DFA.UnpackEncodedStringToUnsignedChars(DFA183_maxS);
		private static readonly short[] DFA183_accept = DFA.UnpackEncodedString(DFA183_acceptS);
		private static readonly short[] DFA183_special = DFA.UnpackEncodedString(DFA183_specialS);
		private static readonly short[][] DFA183_transition;

		static DFA183()
		{
			int numStates = DFA183_transitionS.Length;
			DFA183_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA183_transition[i] = DFA.UnpackEncodedString(DFA183_transitionS[i]);
			}
		}

		public DFA183( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 183;
			this.eot = DFA183_eot;
			this.eof = DFA183_eof;
			this.min = DFA183_min;
			this.max = DFA183_max;
			this.accept = DFA183_accept;
			this.special = DFA183_special;
			this.transition = DFA183_transition;
		}

		public override string Description { get { return "()* loopback of 397:9: ( ( SkipSpace )* op= BIT_OR ( SkipSpace )* rhs= bitwiseXORExpression -> ^( BINARY_EXPR $op $bitwiseORExpression $rhs) )*"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private class DFA186 : DFA
	{
		private const string DFA186_eotS =
			"\x4\xFFFF";
		private const string DFA186_eofS =
			"\x1\x2\x3\xFFFF";
		private const string DFA186_minS =
			"\x2\x5\x2\xFFFF";
		private const string DFA186_maxS =
			"\x2\x86\x2\xFFFF";
		private const string DFA186_acceptS =
			"\x2\xFFFF\x1\x2\x1\x1";
		private const string DFA186_specialS =
			"\x4\xFFFF}>";
		private static readonly string[] DFA186_transitionS =
			{
				"\x1\x2\x7\xFFFF\x1\x3\x7\xFFFF\x1\x2\x22\xFFFF\x1\x2\x1E\xFFFF\x1\x2"+
				"\xA\xFFFF\x1\x2\x9\xFFFF\x1\x1\x18\xFFFF\x2\x2",
				"\x1\x2\x7\xFFFF\x1\x3\x7\xFFFF\x1\x2\x22\xFFFF\x1\x2\x1E\xFFFF\x1\x2"+
				"\xA\xFFFF\x1\x2\x9\xFFFF\x1\x1\x18\xFFFF\x2\x2",
				"",
				""
			};

		private static readonly short[] DFA186_eot = DFA.UnpackEncodedString(DFA186_eotS);
		private static readonly short[] DFA186_eof = DFA.UnpackEncodedString(DFA186_eofS);
		private static readonly char[] DFA186_min = DFA.UnpackEncodedStringToUnsignedChars(DFA186_minS);
		private static readonly char[] DFA186_max = DFA.UnpackEncodedStringToUnsignedChars(DFA186_maxS);
		private static readonly short[] DFA186_accept = DFA.UnpackEncodedString(DFA186_acceptS);
		private static readonly short[] DFA186_special = DFA.UnpackEncodedString(DFA186_specialS);
		private static readonly short[][] DFA186_transition;

		static DFA186()
		{
			int numStates = DFA186_transitionS.Length;
			DFA186_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA186_transition[i] = DFA.UnpackEncodedString(DFA186_transitionS[i]);
			}
		}

		public DFA186( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 186;
			this.eot = DFA186_eot;
			this.eof = DFA186_eof;
			this.min = DFA186_min;
			this.max = DFA186_max;
			this.accept = DFA186_accept;
			this.special = DFA186_special;
			this.transition = DFA186_transition;
		}

		public override string Description { get { return "()* loopback of 402:9: ( ( SkipSpace )* op= BIT_OR ( SkipSpace )* rhs= bitwiseXORExpressionNoIn -> ^( BINARY_EXPR $op $bitwiseORExpressionNoIn $rhs) )*"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private class DFA189 : DFA
	{
		private const string DFA189_eotS =
			"\x4\xFFFF";
		private const string DFA189_eofS =
			"\x1\x2\x3\xFFFF";
		private const string DFA189_minS =
			"\x2\x5\x2\xFFFF";
		private const string DFA189_maxS =
			"\x2\x94\x2\xFFFF";
		private const string DFA189_acceptS =
			"\x2\xFFFF\x1\x2\x1\x1";
		private const string DFA189_specialS =
			"\x4\xFFFF}>";
		private static readonly string[] DFA189_transitionS =
			{
				"\x1\x2\x7\xFFFF\x1\x2\x1\xFFFF\x1\x3\x5\xFFFF\x1\x2\x41\xFFFF\x1\x2"+
				"\xA\xFFFF\x1\x2\x9\xFFFF\x1\x1\x16\xFFFF\x1\x2\x1\xFFFF\x2\x2\x1\xFFFF"+
				"\x1\x2\xB\xFFFF\x1\x2",
				"\x1\x2\x7\xFFFF\x1\x2\x1\xFFFF\x1\x3\x5\xFFFF\x1\x2\x41\xFFFF\x1\x2"+
				"\xA\xFFFF\x1\x2\x9\xFFFF\x1\x1\x16\xFFFF\x1\x2\x1\xFFFF\x2\x2\x1\xFFFF"+
				"\x1\x2\xB\xFFFF\x1\x2",
				"",
				""
			};

		private static readonly short[] DFA189_eot = DFA.UnpackEncodedString(DFA189_eotS);
		private static readonly short[] DFA189_eof = DFA.UnpackEncodedString(DFA189_eofS);
		private static readonly char[] DFA189_min = DFA.UnpackEncodedStringToUnsignedChars(DFA189_minS);
		private static readonly char[] DFA189_max = DFA.UnpackEncodedStringToUnsignedChars(DFA189_maxS);
		private static readonly short[] DFA189_accept = DFA.UnpackEncodedString(DFA189_acceptS);
		private static readonly short[] DFA189_special = DFA.UnpackEncodedString(DFA189_specialS);
		private static readonly short[][] DFA189_transition;

		static DFA189()
		{
			int numStates = DFA189_transitionS.Length;
			DFA189_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA189_transition[i] = DFA.UnpackEncodedString(DFA189_transitionS[i]);
			}
		}

		public DFA189( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 189;
			this.eot = DFA189_eot;
			this.eof = DFA189_eof;
			this.min = DFA189_min;
			this.max = DFA189_max;
			this.accept = DFA189_accept;
			this.special = DFA189_special;
			this.transition = DFA189_transition;
		}

		public override string Description { get { return "()* loopback of 407:9: ( ( SkipSpace )* op= BIT_XOR ( SkipSpace )* rhs= bitwiseANDExpression -> ^( BINARY_EXPR $op $bitwiseXORExpression $rhs) )*"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private class DFA192 : DFA
	{
		private const string DFA192_eotS =
			"\x4\xFFFF";
		private const string DFA192_eofS =
			"\x1\x2\x3\xFFFF";
		private const string DFA192_minS =
			"\x2\x5\x2\xFFFF";
		private const string DFA192_maxS =
			"\x2\x86\x2\xFFFF";
		private const string DFA192_acceptS =
			"\x2\xFFFF\x1\x2\x1\x1";
		private const string DFA192_specialS =
			"\x4\xFFFF}>";
		private static readonly string[] DFA192_transitionS =
			{
				"\x1\x2\x7\xFFFF\x1\x2\x1\xFFFF\x1\x3\x5\xFFFF\x1\x2\x22\xFFFF\x1\x2"+
				"\x1E\xFFFF\x1\x2\xA\xFFFF\x1\x2\x9\xFFFF\x1\x1\x18\xFFFF\x2\x2",
				"\x1\x2\x7\xFFFF\x1\x2\x1\xFFFF\x1\x3\x5\xFFFF\x1\x2\x22\xFFFF\x1\x2"+
				"\x1E\xFFFF\x1\x2\xA\xFFFF\x1\x2\x9\xFFFF\x1\x1\x18\xFFFF\x2\x2",
				"",
				""
			};

		private static readonly short[] DFA192_eot = DFA.UnpackEncodedString(DFA192_eotS);
		private static readonly short[] DFA192_eof = DFA.UnpackEncodedString(DFA192_eofS);
		private static readonly char[] DFA192_min = DFA.UnpackEncodedStringToUnsignedChars(DFA192_minS);
		private static readonly char[] DFA192_max = DFA.UnpackEncodedStringToUnsignedChars(DFA192_maxS);
		private static readonly short[] DFA192_accept = DFA.UnpackEncodedString(DFA192_acceptS);
		private static readonly short[] DFA192_special = DFA.UnpackEncodedString(DFA192_specialS);
		private static readonly short[][] DFA192_transition;

		static DFA192()
		{
			int numStates = DFA192_transitionS.Length;
			DFA192_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA192_transition[i] = DFA.UnpackEncodedString(DFA192_transitionS[i]);
			}
		}

		public DFA192( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 192;
			this.eot = DFA192_eot;
			this.eof = DFA192_eof;
			this.min = DFA192_min;
			this.max = DFA192_max;
			this.accept = DFA192_accept;
			this.special = DFA192_special;
			this.transition = DFA192_transition;
		}

		public override string Description { get { return "()* loopback of 412:9: ( ( SkipSpace )* op= BIT_XOR ( SkipSpace )* rhs= bitwiseANDExpressionNoIn -> ^( BINARY_EXPR $op $bitwiseXORExpressionNoIn $rhs) )*"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private class DFA195 : DFA
	{
		private const string DFA195_eotS =
			"\x4\xFFFF";
		private const string DFA195_eofS =
			"\x1\x2\x3\xFFFF";
		private const string DFA195_minS =
			"\x2\x5\x2\xFFFF";
		private const string DFA195_maxS =
			"\x2\x94\x2\xFFFF";
		private const string DFA195_acceptS =
			"\x2\xFFFF\x1\x2\x1\x1";
		private const string DFA195_specialS =
			"\x4\xFFFF}>";
		private static readonly string[] DFA195_transitionS =
			{
				"\x1\x2\x4\xFFFF\x1\x3\x2\xFFFF\x1\x2\x1\xFFFF\x1\x2\x5\xFFFF\x1\x2\x41"+
				"\xFFFF\x1\x2\xA\xFFFF\x1\x2\x9\xFFFF\x1\x1\x16\xFFFF\x1\x2\x1\xFFFF"+
				"\x2\x2\x1\xFFFF\x1\x2\xB\xFFFF\x1\x2",
				"\x1\x2\x4\xFFFF\x1\x3\x2\xFFFF\x1\x2\x1\xFFFF\x1\x2\x5\xFFFF\x1\x2\x41"+
				"\xFFFF\x1\x2\xA\xFFFF\x1\x2\x9\xFFFF\x1\x1\x16\xFFFF\x1\x2\x1\xFFFF"+
				"\x2\x2\x1\xFFFF\x1\x2\xB\xFFFF\x1\x2",
				"",
				""
			};

		private static readonly short[] DFA195_eot = DFA.UnpackEncodedString(DFA195_eotS);
		private static readonly short[] DFA195_eof = DFA.UnpackEncodedString(DFA195_eofS);
		private static readonly char[] DFA195_min = DFA.UnpackEncodedStringToUnsignedChars(DFA195_minS);
		private static readonly char[] DFA195_max = DFA.UnpackEncodedStringToUnsignedChars(DFA195_maxS);
		private static readonly short[] DFA195_accept = DFA.UnpackEncodedString(DFA195_acceptS);
		private static readonly short[] DFA195_special = DFA.UnpackEncodedString(DFA195_specialS);
		private static readonly short[][] DFA195_transition;

		static DFA195()
		{
			int numStates = DFA195_transitionS.Length;
			DFA195_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA195_transition[i] = DFA.UnpackEncodedString(DFA195_transitionS[i]);
			}
		}

		public DFA195( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 195;
			this.eot = DFA195_eot;
			this.eof = DFA195_eof;
			this.min = DFA195_min;
			this.max = DFA195_max;
			this.accept = DFA195_accept;
			this.special = DFA195_special;
			this.transition = DFA195_transition;
		}

		public override string Description { get { return "()* loopback of 417:9: ( ( SkipSpace )* op= BIT_AND ( SkipSpace )* rhs= equalityExpression -> ^( BINARY_EXPR $op $bitwiseANDExpression $rhs) )*"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private class DFA198 : DFA
	{
		private const string DFA198_eotS =
			"\x4\xFFFF";
		private const string DFA198_eofS =
			"\x1\x2\x3\xFFFF";
		private const string DFA198_minS =
			"\x2\x5\x2\xFFFF";
		private const string DFA198_maxS =
			"\x2\x86\x2\xFFFF";
		private const string DFA198_acceptS =
			"\x2\xFFFF\x1\x2\x1\x1";
		private const string DFA198_specialS =
			"\x4\xFFFF}>";
		private static readonly string[] DFA198_transitionS =
			{
				"\x1\x2\x4\xFFFF\x1\x3\x2\xFFFF\x1\x2\x1\xFFFF\x1\x2\x5\xFFFF\x1\x2\x22"+
				"\xFFFF\x1\x2\x1E\xFFFF\x1\x2\xA\xFFFF\x1\x2\x9\xFFFF\x1\x1\x18\xFFFF"+
				"\x2\x2",
				"\x1\x2\x4\xFFFF\x1\x3\x2\xFFFF\x1\x2\x1\xFFFF\x1\x2\x5\xFFFF\x1\x2\x22"+
				"\xFFFF\x1\x2\x1E\xFFFF\x1\x2\xA\xFFFF\x1\x2\x9\xFFFF\x1\x1\x18\xFFFF"+
				"\x2\x2",
				"",
				""
			};

		private static readonly short[] DFA198_eot = DFA.UnpackEncodedString(DFA198_eotS);
		private static readonly short[] DFA198_eof = DFA.UnpackEncodedString(DFA198_eofS);
		private static readonly char[] DFA198_min = DFA.UnpackEncodedStringToUnsignedChars(DFA198_minS);
		private static readonly char[] DFA198_max = DFA.UnpackEncodedStringToUnsignedChars(DFA198_maxS);
		private static readonly short[] DFA198_accept = DFA.UnpackEncodedString(DFA198_acceptS);
		private static readonly short[] DFA198_special = DFA.UnpackEncodedString(DFA198_specialS);
		private static readonly short[][] DFA198_transition;

		static DFA198()
		{
			int numStates = DFA198_transitionS.Length;
			DFA198_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA198_transition[i] = DFA.UnpackEncodedString(DFA198_transitionS[i]);
			}
		}

		public DFA198( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 198;
			this.eot = DFA198_eot;
			this.eof = DFA198_eof;
			this.min = DFA198_min;
			this.max = DFA198_max;
			this.accept = DFA198_accept;
			this.special = DFA198_special;
			this.transition = DFA198_transition;
		}

		public override string Description { get { return "()* loopback of 422:9: ( ( SkipSpace )* op= BIT_AND ( SkipSpace )* rhs= equalityExpressionNoIn -> ^( BINARY_EXPR $op $bitwiseANDExpressionNoIn $rhs) )*"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private class DFA201 : DFA
	{
		private const string DFA201_eotS =
			"\x4\xFFFF";
		private const string DFA201_eofS =
			"\x1\x2\x3\xFFFF";
		private const string DFA201_minS =
			"\x2\x5\x2\xFFFF";
		private const string DFA201_maxS =
			"\x2\x94\x2\xFFFF";
		private const string DFA201_acceptS =
			"\x2\xFFFF\x1\x2\x1\x1";
		private const string DFA201_specialS =
			"\x4\xFFFF}>";
		private static readonly string[] DFA201_transitionS =
			{
				"\x1\x2\x4\xFFFF\x1\x2\x2\xFFFF\x1\x2\x1\xFFFF\x1\x2\x5\xFFFF\x1\x2\xE"+
				"\xFFFF\x1\x3\x29\xFFFF\x1\x3\x8\xFFFF\x1\x2\x6\xFFFF\x1\x3\x2\xFFFF"+
				"\x1\x3\x1\x2\x9\xFFFF\x1\x1\x16\xFFFF\x1\x2\x1\xFFFF\x2\x2\x1\xFFFF"+
				"\x1\x2\xB\xFFFF\x1\x2",
				"\x1\x2\x4\xFFFF\x1\x2\x2\xFFFF\x1\x2\x1\xFFFF\x1\x2\x5\xFFFF\x1\x2\xE"+
				"\xFFFF\x1\x3\x29\xFFFF\x1\x3\x8\xFFFF\x1\x2\x6\xFFFF\x1\x3\x2\xFFFF"+
				"\x1\x3\x1\x2\x9\xFFFF\x1\x1\x16\xFFFF\x1\x2\x1\xFFFF\x2\x2\x1\xFFFF"+
				"\x1\x2\xB\xFFFF\x1\x2",
				"",
				""
			};

		private static readonly short[] DFA201_eot = DFA.UnpackEncodedString(DFA201_eotS);
		private static readonly short[] DFA201_eof = DFA.UnpackEncodedString(DFA201_eofS);
		private static readonly char[] DFA201_min = DFA.UnpackEncodedStringToUnsignedChars(DFA201_minS);
		private static readonly char[] DFA201_max = DFA.UnpackEncodedStringToUnsignedChars(DFA201_maxS);
		private static readonly short[] DFA201_accept = DFA.UnpackEncodedString(DFA201_acceptS);
		private static readonly short[] DFA201_special = DFA.UnpackEncodedString(DFA201_specialS);
		private static readonly short[][] DFA201_transition;

		static DFA201()
		{
			int numStates = DFA201_transitionS.Length;
			DFA201_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA201_transition[i] = DFA.UnpackEncodedString(DFA201_transitionS[i]);
			}
		}

		public DFA201( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 201;
			this.eot = DFA201_eot;
			this.eof = DFA201_eof;
			this.min = DFA201_min;
			this.max = DFA201_max;
			this.accept = DFA201_accept;
			this.special = DFA201_special;
			this.transition = DFA201_transition;
		}

		public override string Description { get { return "()* loopback of 427:9: ( ( SkipSpace )* op= equalityOperator ( SkipSpace )* rhs= relationalExpression -> ^( BINARY_EXPR $op $equalityExpression $rhs) )*"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private class DFA204 : DFA
	{
		private const string DFA204_eotS =
			"\x4\xFFFF";
		private const string DFA204_eofS =
			"\x1\x2\x3\xFFFF";
		private const string DFA204_minS =
			"\x2\x5\x2\xFFFF";
		private const string DFA204_maxS =
			"\x2\x86\x2\xFFFF";
		private const string DFA204_acceptS =
			"\x2\xFFFF\x1\x2\x1\x1";
		private const string DFA204_specialS =
			"\x4\xFFFF}>";
		private static readonly string[] DFA204_transitionS =
			{
				"\x1\x2\x4\xFFFF\x1\x2\x2\xFFFF\x1\x2\x1\xFFFF\x1\x2\x5\xFFFF\x1\x2\xE"+
				"\xFFFF\x1\x3\x13\xFFFF\x1\x2\x15\xFFFF\x1\x3\x8\xFFFF\x1\x2\x6\xFFFF"+
				"\x1\x3\x2\xFFFF\x1\x3\x1\x2\x9\xFFFF\x1\x1\x18\xFFFF\x2\x2",
				"\x1\x2\x4\xFFFF\x1\x2\x2\xFFFF\x1\x2\x1\xFFFF\x1\x2\x5\xFFFF\x1\x2\xE"+
				"\xFFFF\x1\x3\x13\xFFFF\x1\x2\x15\xFFFF\x1\x3\x8\xFFFF\x1\x2\x6\xFFFF"+
				"\x1\x3\x2\xFFFF\x1\x3\x1\x2\x9\xFFFF\x1\x1\x18\xFFFF\x2\x2",
				"",
				""
			};

		private static readonly short[] DFA204_eot = DFA.UnpackEncodedString(DFA204_eotS);
		private static readonly short[] DFA204_eof = DFA.UnpackEncodedString(DFA204_eofS);
		private static readonly char[] DFA204_min = DFA.UnpackEncodedStringToUnsignedChars(DFA204_minS);
		private static readonly char[] DFA204_max = DFA.UnpackEncodedStringToUnsignedChars(DFA204_maxS);
		private static readonly short[] DFA204_accept = DFA.UnpackEncodedString(DFA204_acceptS);
		private static readonly short[] DFA204_special = DFA.UnpackEncodedString(DFA204_specialS);
		private static readonly short[][] DFA204_transition;

		static DFA204()
		{
			int numStates = DFA204_transitionS.Length;
			DFA204_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA204_transition[i] = DFA.UnpackEncodedString(DFA204_transitionS[i]);
			}
		}

		public DFA204( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 204;
			this.eot = DFA204_eot;
			this.eof = DFA204_eof;
			this.min = DFA204_min;
			this.max = DFA204_max;
			this.accept = DFA204_accept;
			this.special = DFA204_special;
			this.transition = DFA204_transition;
		}

		public override string Description { get { return "()* loopback of 432:9: ( ( SkipSpace )* op= equalityOperator ( SkipSpace )* rhs= relationalExpressionNoIn -> ^( BINARY_EXPR $op $equalityExpressionNoIn $rhs) )*"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private class DFA207 : DFA
	{
		private const string DFA207_eotS =
			"\x4\xFFFF";
		private const string DFA207_eofS =
			"\x1\x2\x3\xFFFF";
		private const string DFA207_minS =
			"\x2\x5\x2\xFFFF";
		private const string DFA207_maxS =
			"\x2\x94\x2\xFFFF";
		private const string DFA207_acceptS =
			"\x2\xFFFF\x1\x2\x1\x1";
		private const string DFA207_specialS =
			"\x4\xFFFF}>";
		private static readonly string[] DFA207_transitionS =
			{
				"\x1\x2\x4\xFFFF\x1\x2\x2\xFFFF\x1\x2\x1\xFFFF\x1\x2\x5\xFFFF\x1\x2\xE"+
				"\xFFFF\x1\x2\xA\xFFFF\x2\x3\x7\xFFFF\x1\x3\x5\xFFFF\x1\x3\x3\xFFFF\x2"+
				"\x3\xA\xFFFF\x1\x2\x8\xFFFF\x1\x2\x6\xFFFF\x1\x2\x2\xFFFF\x2\x2\x9\xFFFF"+
				"\x1\x1\x16\xFFFF\x1\x2\x1\xFFFF\x2\x2\x1\xFFFF\x1\x2\xB\xFFFF\x1\x2",
				"\x1\x2\x4\xFFFF\x1\x2\x2\xFFFF\x1\x2\x1\xFFFF\x1\x2\x5\xFFFF\x1\x2\xE"+
				"\xFFFF\x1\x2\xA\xFFFF\x2\x3\x7\xFFFF\x1\x3\x5\xFFFF\x1\x3\x3\xFFFF\x2"+
				"\x3\xA\xFFFF\x1\x2\x8\xFFFF\x1\x2\x6\xFFFF\x1\x2\x2\xFFFF\x2\x2\x9\xFFFF"+
				"\x1\x1\x16\xFFFF\x1\x2\x1\xFFFF\x2\x2\x1\xFFFF\x1\x2\xB\xFFFF\x1\x2",
				"",
				""
			};

		private static readonly short[] DFA207_eot = DFA.UnpackEncodedString(DFA207_eotS);
		private static readonly short[] DFA207_eof = DFA.UnpackEncodedString(DFA207_eofS);
		private static readonly char[] DFA207_min = DFA.UnpackEncodedStringToUnsignedChars(DFA207_minS);
		private static readonly char[] DFA207_max = DFA.UnpackEncodedStringToUnsignedChars(DFA207_maxS);
		private static readonly short[] DFA207_accept = DFA.UnpackEncodedString(DFA207_acceptS);
		private static readonly short[] DFA207_special = DFA.UnpackEncodedString(DFA207_specialS);
		private static readonly short[][] DFA207_transition;

		static DFA207()
		{
			int numStates = DFA207_transitionS.Length;
			DFA207_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA207_transition[i] = DFA.UnpackEncodedString(DFA207_transitionS[i]);
			}
		}

		public DFA207( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 207;
			this.eot = DFA207_eot;
			this.eof = DFA207_eof;
			this.min = DFA207_min;
			this.max = DFA207_max;
			this.accept = DFA207_accept;
			this.special = DFA207_special;
			this.transition = DFA207_transition;
		}

		public override string Description { get { return "()* loopback of 441:9: ( ( SkipSpace )* op= relationalOperator ( SkipSpace )* rhs= shiftExpression -> ^( BINARY_EXPR $op $relationalExpression $rhs) )*"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private class DFA210 : DFA
	{
		private const string DFA210_eotS =
			"\x4\xFFFF";
		private const string DFA210_eofS =
			"\x1\x2\x3\xFFFF";
		private const string DFA210_minS =
			"\x2\x5\x2\xFFFF";
		private const string DFA210_maxS =
			"\x2\x86\x2\xFFFF";
		private const string DFA210_acceptS =
			"\x2\xFFFF\x1\x2\x1\x1";
		private const string DFA210_specialS =
			"\x4\xFFFF}>";
		private static readonly string[] DFA210_transitionS =
			{
				"\x1\x2\x4\xFFFF\x1\x2\x2\xFFFF\x1\x2\x1\xFFFF\x1\x2\x5\xFFFF\x1\x2\xE"+
				"\xFFFF\x1\x2\xA\xFFFF\x2\x3\x7\xFFFF\x1\x2\x5\xFFFF\x1\x3\x3\xFFFF\x2"+
				"\x3\xA\xFFFF\x1\x2\x8\xFFFF\x1\x2\x6\xFFFF\x1\x2\x2\xFFFF\x2\x2\x9\xFFFF"+
				"\x1\x1\x18\xFFFF\x2\x2",
				"\x1\x2\x4\xFFFF\x1\x2\x2\xFFFF\x1\x2\x1\xFFFF\x1\x2\x5\xFFFF\x1\x2\xE"+
				"\xFFFF\x1\x2\xA\xFFFF\x2\x3\x7\xFFFF\x1\x2\x5\xFFFF\x1\x3\x3\xFFFF\x2"+
				"\x3\xA\xFFFF\x1\x2\x8\xFFFF\x1\x2\x6\xFFFF\x1\x2\x2\xFFFF\x2\x2\x9\xFFFF"+
				"\x1\x1\x18\xFFFF\x2\x2",
				"",
				""
			};

		private static readonly short[] DFA210_eot = DFA.UnpackEncodedString(DFA210_eotS);
		private static readonly short[] DFA210_eof = DFA.UnpackEncodedString(DFA210_eofS);
		private static readonly char[] DFA210_min = DFA.UnpackEncodedStringToUnsignedChars(DFA210_minS);
		private static readonly char[] DFA210_max = DFA.UnpackEncodedStringToUnsignedChars(DFA210_maxS);
		private static readonly short[] DFA210_accept = DFA.UnpackEncodedString(DFA210_acceptS);
		private static readonly short[] DFA210_special = DFA.UnpackEncodedString(DFA210_specialS);
		private static readonly short[][] DFA210_transition;

		static DFA210()
		{
			int numStates = DFA210_transitionS.Length;
			DFA210_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA210_transition[i] = DFA.UnpackEncodedString(DFA210_transitionS[i]);
			}
		}

		public DFA210( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 210;
			this.eot = DFA210_eot;
			this.eof = DFA210_eof;
			this.min = DFA210_min;
			this.max = DFA210_max;
			this.accept = DFA210_accept;
			this.special = DFA210_special;
			this.transition = DFA210_transition;
		}

		public override string Description { get { return "()* loopback of 446:9: ( ( SkipSpace )* op= relationalOperatorNoIn ( SkipSpace )* rhs= shiftExpression -> ^( BINARY_EXPR $op $relationalExpressionNoIn $rhs) )*"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private class DFA213 : DFA
	{
		private const string DFA213_eotS =
			"\x4\xFFFF";
		private const string DFA213_eofS =
			"\x1\x2\x3\xFFFF";
		private const string DFA213_minS =
			"\x2\x5\x2\xFFFF";
		private const string DFA213_maxS =
			"\x2\x94\x2\xFFFF";
		private const string DFA213_acceptS =
			"\x2\xFFFF\x1\x2\x1\x1";
		private const string DFA213_specialS =
			"\x4\xFFFF}>";
		private static readonly string[] DFA213_transitionS =
			{
				"\x1\x2\x4\xFFFF\x1\x2\x2\xFFFF\x1\x2\x1\xFFFF\x1\x2\x5\xFFFF\x1\x2\xE"+
				"\xFFFF\x1\x2\xA\xFFFF\x2\x2\x7\xFFFF\x1\x2\x5\xFFFF\x1\x2\x3\xFFFF\x2"+
				"\x2\xA\xFFFF\x1\x2\x8\xFFFF\x1\x2\x6\xFFFF\x1\x2\x2\xFFFF\x2\x2\x1\x3"+
				"\x1\xFFFF\x1\x3\x6\xFFFF\x1\x1\xA\xFFFF\x1\x3\xB\xFFFF\x1\x2\x1\xFFFF"+
				"\x2\x2\x1\xFFFF\x1\x2\xB\xFFFF\x1\x2",
				"\x1\x2\x4\xFFFF\x1\x2\x2\xFFFF\x1\x2\x1\xFFFF\x1\x2\x5\xFFFF\x1\x2\xE"+
				"\xFFFF\x1\x2\xA\xFFFF\x2\x2\x7\xFFFF\x1\x2\x5\xFFFF\x1\x2\x3\xFFFF\x2"+
				"\x2\xA\xFFFF\x1\x2\x8\xFFFF\x1\x2\x6\xFFFF\x1\x2\x2\xFFFF\x2\x2\x1\x3"+
				"\x1\xFFFF\x1\x3\x6\xFFFF\x1\x1\xA\xFFFF\x1\x3\xB\xFFFF\x1\x2\x1\xFFFF"+
				"\x2\x2\x1\xFFFF\x1\x2\xB\xFFFF\x1\x2",
				"",
				""
			};

		private static readonly short[] DFA213_eot = DFA.UnpackEncodedString(DFA213_eotS);
		private static readonly short[] DFA213_eof = DFA.UnpackEncodedString(DFA213_eofS);
		private static readonly char[] DFA213_min = DFA.UnpackEncodedStringToUnsignedChars(DFA213_minS);
		private static readonly char[] DFA213_max = DFA.UnpackEncodedStringToUnsignedChars(DFA213_maxS);
		private static readonly short[] DFA213_accept = DFA.UnpackEncodedString(DFA213_acceptS);
		private static readonly short[] DFA213_special = DFA.UnpackEncodedString(DFA213_specialS);
		private static readonly short[][] DFA213_transition;

		static DFA213()
		{
			int numStates = DFA213_transitionS.Length;
			DFA213_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA213_transition[i] = DFA.UnpackEncodedString(DFA213_transitionS[i]);
			}
		}

		public DFA213( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 213;
			this.eot = DFA213_eot;
			this.eof = DFA213_eof;
			this.min = DFA213_min;
			this.max = DFA213_max;
			this.accept = DFA213_accept;
			this.special = DFA213_special;
			this.transition = DFA213_transition;
		}

		public override string Description { get { return "()* loopback of 459:9: ( ( SkipSpace )* op= shiftOperator ( SkipSpace )* rhs= additiveExpression -> ^( BINARY_EXPR $op $shiftExpression $rhs) )*"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private class DFA216 : DFA
	{
		private const string DFA216_eotS =
			"\x4\xFFFF";
		private const string DFA216_eofS =
			"\x1\x2\x3\xFFFF";
		private const string DFA216_minS =
			"\x2\x5\x2\xFFFF";
		private const string DFA216_maxS =
			"\x2\x94\x2\xFFFF";
		private const string DFA216_acceptS =
			"\x2\xFFFF\x1\x2\x1\x1";
		private const string DFA216_specialS =
			"\x4\xFFFF}>";
		private static readonly string[] DFA216_transitionS =
			{
				"\x1\x2\x4\xFFFF\x1\x2\x2\xFFFF\x1\x2\x1\xFFFF\x1\x2\x5\xFFFF\x1\x2\xE"+
				"\xFFFF\x1\x2\xA\xFFFF\x2\x2\x7\xFFFF\x1\x2\x5\xFFFF\x1\x2\x3\xFFFF\x2"+
				"\x2\x3\xFFFF\x1\x3\x6\xFFFF\x1\x2\x8\xFFFF\x1\x2\x1\x3\x5\xFFFF\x1\x2"+
				"\x2\xFFFF\x3\x2\x1\xFFFF\x1\x2\x6\xFFFF\x1\x1\xA\xFFFF\x1\x2\xB\xFFFF"+
				"\x1\x2\x1\xFFFF\x2\x2\x1\xFFFF\x1\x2\xB\xFFFF\x1\x2",
				"\x1\x2\x4\xFFFF\x1\x2\x2\xFFFF\x1\x2\x1\xFFFF\x1\x2\x5\xFFFF\x1\x2\xE"+
				"\xFFFF\x1\x2\xA\xFFFF\x2\x2\x7\xFFFF\x1\x2\x5\xFFFF\x1\x2\x3\xFFFF\x2"+
				"\x2\x3\xFFFF\x1\x3\x6\xFFFF\x1\x2\x8\xFFFF\x1\x2\x1\x3\x5\xFFFF\x1\x2"+
				"\x2\xFFFF\x3\x2\x1\xFFFF\x1\x2\x6\xFFFF\x1\x1\xA\xFFFF\x1\x2\xB\xFFFF"+
				"\x1\x2\x1\xFFFF\x2\x2\x1\xFFFF\x1\x2\xB\xFFFF\x1\x2",
				"",
				""
			};

		private static readonly short[] DFA216_eot = DFA.UnpackEncodedString(DFA216_eotS);
		private static readonly short[] DFA216_eof = DFA.UnpackEncodedString(DFA216_eofS);
		private static readonly char[] DFA216_min = DFA.UnpackEncodedStringToUnsignedChars(DFA216_minS);
		private static readonly char[] DFA216_max = DFA.UnpackEncodedStringToUnsignedChars(DFA216_maxS);
		private static readonly short[] DFA216_accept = DFA.UnpackEncodedString(DFA216_acceptS);
		private static readonly short[] DFA216_special = DFA.UnpackEncodedString(DFA216_specialS);
		private static readonly short[][] DFA216_transition;

		static DFA216()
		{
			int numStates = DFA216_transitionS.Length;
			DFA216_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA216_transition[i] = DFA.UnpackEncodedString(DFA216_transitionS[i]);
			}
		}

		public DFA216( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 216;
			this.eot = DFA216_eot;
			this.eof = DFA216_eof;
			this.min = DFA216_min;
			this.max = DFA216_max;
			this.accept = DFA216_accept;
			this.special = DFA216_special;
			this.transition = DFA216_transition;
		}

		public override string Description { get { return "()* loopback of 468:9: ( ( SkipSpace )* op= additiveOperator ( SkipSpace )* rhs= multiplicativeExpression -> ^( BINARY_EXPR $op $additiveExpression $rhs) )*"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private class DFA219 : DFA
	{
		private const string DFA219_eotS =
			"\x4\xFFFF";
		private const string DFA219_eofS =
			"\x1\x2\x3\xFFFF";
		private const string DFA219_minS =
			"\x2\x5\x2\xFFFF";
		private const string DFA219_maxS =
			"\x2\x94\x2\xFFFF";
		private const string DFA219_acceptS =
			"\x2\xFFFF\x1\x2\x1\x1";
		private const string DFA219_specialS =
			"\x4\xFFFF}>";
		private static readonly string[] DFA219_transitionS =
			{
				"\x1\x2\x4\xFFFF\x1\x2\x2\xFFFF\x1\x2\x1\xFFFF\x1\x2\x5\xFFFF\x1\x2\x5"+
				"\xFFFF\x1\x3\x8\xFFFF\x1\x2\xA\xFFFF\x2\x2\x7\xFFFF\x1\x2\x5\xFFFF\x1"+
				"\x2\x3\xFFFF\x2\x2\x3\xFFFF\x1\x2\x2\xFFFF\x1\x3\x1\xFFFF\x1\x3\x1\xFFFF"+
				"\x1\x2\x8\xFFFF\x2\x2\x5\xFFFF\x1\x2\x2\xFFFF\x3\x2\x1\xFFFF\x1\x2\x6"+
				"\xFFFF\x1\x1\xA\xFFFF\x1\x2\xB\xFFFF\x1\x2\x1\xFFFF\x2\x2\x1\xFFFF\x1"+
				"\x2\xB\xFFFF\x1\x2",
				"\x1\x2\x4\xFFFF\x1\x2\x2\xFFFF\x1\x2\x1\xFFFF\x1\x2\x5\xFFFF\x1\x2\x5"+
				"\xFFFF\x1\x3\x8\xFFFF\x1\x2\xA\xFFFF\x2\x2\x7\xFFFF\x1\x2\x5\xFFFF\x1"+
				"\x2\x3\xFFFF\x2\x2\x3\xFFFF\x1\x2\x2\xFFFF\x1\x3\x1\xFFFF\x1\x3\x1\xFFFF"+
				"\x1\x2\x8\xFFFF\x2\x2\x5\xFFFF\x1\x2\x2\xFFFF\x3\x2\x1\xFFFF\x1\x2\x6"+
				"\xFFFF\x1\x1\xA\xFFFF\x1\x2\xB\xFFFF\x1\x2\x1\xFFFF\x2\x2\x1\xFFFF\x1"+
				"\x2\xB\xFFFF\x1\x2",
				"",
				""
			};

		private static readonly short[] DFA219_eot = DFA.UnpackEncodedString(DFA219_eotS);
		private static readonly short[] DFA219_eof = DFA.UnpackEncodedString(DFA219_eofS);
		private static readonly char[] DFA219_min = DFA.UnpackEncodedStringToUnsignedChars(DFA219_minS);
		private static readonly char[] DFA219_max = DFA.UnpackEncodedStringToUnsignedChars(DFA219_maxS);
		private static readonly short[] DFA219_accept = DFA.UnpackEncodedString(DFA219_acceptS);
		private static readonly short[] DFA219_special = DFA.UnpackEncodedString(DFA219_specialS);
		private static readonly short[][] DFA219_transition;

		static DFA219()
		{
			int numStates = DFA219_transitionS.Length;
			DFA219_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA219_transition[i] = DFA.UnpackEncodedString(DFA219_transitionS[i]);
			}
		}

		public DFA219( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 219;
			this.eot = DFA219_eot;
			this.eof = DFA219_eof;
			this.min = DFA219_min;
			this.max = DFA219_max;
			this.accept = DFA219_accept;
			this.special = DFA219_special;
			this.transition = DFA219_transition;
		}

		public override string Description { get { return "()* loopback of 477:9: ( ( SkipSpace )* op= multiplicativeOperator ( SkipSpace )* rhs= unaryExpression -> ^( BINARY_EXPR $op $multiplicativeExpression $rhs) )*"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private class DFA223 : DFA
	{
		private const string DFA223_eotS =
			"\xC\xFFFF";
		private const string DFA223_eofS =
			"\xC\xFFFF";
		private const string DFA223_minS =
			"\x1\x13\x9\x0\x2\xFFFF";
		private const string DFA223_maxS =
			"\x1\x93\x9\x0\x2\xFFFF";
		private const string DFA223_acceptS =
			"\xA\xFFFF\x1\x1\x1\x2";
		private const string DFA223_specialS =
			"\x1\xFFFF\x1\x0\x1\x1\x1\x2\x1\x3\x1\x4\x1\x5\x1\x6\x1\x7\x1\x8\x2\xFFFF}>";
		private static readonly string[] DFA223_transitionS =
			{
				"\x1\x4\x15\xFFFF\x1\x4\xD\xFFFF\x1\x3\x7\xFFFF\x1\x2\xF\xFFFF\x1\x9"+
				"\x3\xFFFF\x1\x4\x2\xFFFF\x1\x4\x16\xFFFF\x1\x4\x1\x1\x13\xFFFF\x1\x7"+
				"\x4\xFFFF\x1\x5\x6\xFFFF\x1\x8\x4\xFFFF\x1\x6",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"\x1\xFFFF",
				"",
				""
			};

		private static readonly short[] DFA223_eot = DFA.UnpackEncodedString(DFA223_eotS);
		private static readonly short[] DFA223_eof = DFA.UnpackEncodedString(DFA223_eofS);
		private static readonly char[] DFA223_min = DFA.UnpackEncodedStringToUnsignedChars(DFA223_minS);
		private static readonly char[] DFA223_max = DFA.UnpackEncodedStringToUnsignedChars(DFA223_maxS);
		private static readonly short[] DFA223_accept = DFA.UnpackEncodedString(DFA223_acceptS);
		private static readonly short[] DFA223_special = DFA.UnpackEncodedString(DFA223_specialS);
		private static readonly short[][] DFA223_transition;

		static DFA223()
		{
			int numStates = DFA223_transitionS.Length;
			DFA223_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA223_transition[i] = DFA.UnpackEncodedString(DFA223_transitionS[i]);
			}
		}

		public DFA223( BaseRecognizer recognizer, SpecialStateTransitionHandler specialStateTransition )
			: base(specialStateTransition)
		{
			this.recognizer = recognizer;
			this.decisionNumber = 223;
			this.eot = DFA223_eot;
			this.eof = DFA223_eof;
			this.min = DFA223_min;
			this.max = DFA223_max;
			this.accept = DFA223_accept;
			this.special = DFA223_special;
			this.transition = DFA223_transition;
		}

		public override string Description { get { return "494:1: postfixExpression : ( leftHandSideExpression (op1= PLUS_INC |op2= MINUS_INC ) -> ^( POSTFIX ( $op1)? ( $op2)? leftHandSideExpression ) | leftHandSideExpression );"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private int SpecialStateTransition223(DFA dfa, int s, IIntStream _input)
	{
		ITokenStream input = (ITokenStream)_input;
		int _s = s;
		switch (s)
		{
			case 0:
				int LA223_1 = input.LA(1);


				int index223_1 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred276_JavaScript_fragment)) ) {s = 10;}

				else if ( (true) ) {s = 11;}


				input.Seek(index223_1);
				if ( s>=0 ) return s;
				break;
			case 1:
				int LA223_2 = input.LA(1);


				int index223_2 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred276_JavaScript_fragment)) ) {s = 10;}

				else if ( (true) ) {s = 11;}


				input.Seek(index223_2);
				if ( s>=0 ) return s;
				break;
			case 2:
				int LA223_3 = input.LA(1);


				int index223_3 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred276_JavaScript_fragment)) ) {s = 10;}

				else if ( (true) ) {s = 11;}


				input.Seek(index223_3);
				if ( s>=0 ) return s;
				break;
			case 3:
				int LA223_4 = input.LA(1);


				int index223_4 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred276_JavaScript_fragment)) ) {s = 10;}

				else if ( (true) ) {s = 11;}


				input.Seek(index223_4);
				if ( s>=0 ) return s;
				break;
			case 4:
				int LA223_5 = input.LA(1);


				int index223_5 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred276_JavaScript_fragment)) ) {s = 10;}

				else if ( (true) ) {s = 11;}


				input.Seek(index223_5);
				if ( s>=0 ) return s;
				break;
			case 5:
				int LA223_6 = input.LA(1);


				int index223_6 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred276_JavaScript_fragment)) ) {s = 10;}

				else if ( (true) ) {s = 11;}


				input.Seek(index223_6);
				if ( s>=0 ) return s;
				break;
			case 6:
				int LA223_7 = input.LA(1);


				int index223_7 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred276_JavaScript_fragment)) ) {s = 10;}

				else if ( (true) ) {s = 11;}


				input.Seek(index223_7);
				if ( s>=0 ) return s;
				break;
			case 7:
				int LA223_8 = input.LA(1);


				int index223_8 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred276_JavaScript_fragment)) ) {s = 10;}

				else if ( (true) ) {s = 11;}


				input.Seek(index223_8);
				if ( s>=0 ) return s;
				break;
			case 8:
				int LA223_9 = input.LA(1);


				int index223_9 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred276_JavaScript_fragment)) ) {s = 10;}

				else if ( (true) ) {s = 11;}


				input.Seek(index223_9);
				if ( s>=0 ) return s;
				break;
		}
		if (state.backtracking > 0) {state.failed=true; return -1;}
		NoViableAltException nvae = new NoViableAltException(dfa.Description, 223, _s, input);
		dfa.Error(nvae);
		throw nvae;
	}
	private class DFA232 : DFA
	{
		private const string DFA232_eotS =
			"\x4\xFFFF";
		private const string DFA232_eofS =
			"\x4\xFFFF";
		private const string DFA232_minS =
			"\x2\x15\x2\xFFFF";
		private const string DFA232_maxS =
			"\x2\x88\x2\xFFFF";
		private const string DFA232_acceptS =
			"\x2\xFFFF\x1\x2\x1\x1";
		private const string DFA232_specialS =
			"\x4\xFFFF}>";
		private static readonly string[] DFA232_transitionS =
			{
				"\x1\x3\x56\xFFFF\x1\x1\x1B\xFFFF\x1\x2",
				"\x1\x3\x56\xFFFF\x1\x1\x1B\xFFFF\x1\x2",
				"",
				""
			};

		private static readonly short[] DFA232_eot = DFA.UnpackEncodedString(DFA232_eotS);
		private static readonly short[] DFA232_eof = DFA.UnpackEncodedString(DFA232_eofS);
		private static readonly char[] DFA232_min = DFA.UnpackEncodedStringToUnsignedChars(DFA232_minS);
		private static readonly char[] DFA232_max = DFA.UnpackEncodedStringToUnsignedChars(DFA232_maxS);
		private static readonly short[] DFA232_accept = DFA.UnpackEncodedString(DFA232_acceptS);
		private static readonly short[] DFA232_special = DFA.UnpackEncodedString(DFA232_specialS);
		private static readonly short[][] DFA232_transition;

		static DFA232()
		{
			int numStates = DFA232_transitionS.Length;
			DFA232_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA232_transition[i] = DFA.UnpackEncodedString(DFA232_transitionS[i]);
			}
		}

		public DFA232( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 232;
			this.eot = DFA232_eot;
			this.eof = DFA232_eof;
			this.min = DFA232_min;
			this.max = DFA232_max;
			this.accept = DFA232_accept;
			this.special = DFA232_special;
			this.transition = DFA232_transition;
		}

		public override string Description { get { return "()* loopback of 513:44: ( ( SkipSpace )* ',' ( ( SkipSpace )* assignmentExpression )? )*"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private class DFA231 : DFA
	{
		private const string DFA231_eotS =
			"\x4\xFFFF";
		private const string DFA231_eofS =
			"\x4\xFFFF";
		private const string DFA231_minS =
			"\x2\xC\x2\xFFFF";
		private const string DFA231_maxS =
			"\x2\x93\x2\xFFFF";
		private const string DFA231_acceptS =
			"\x2\xFFFF\x1\x1\x1\x2";
		private const string DFA231_specialS =
			"\x4\xFFFF}>";
		private static readonly string[] DFA231_transitionS =
			{
				"\x1\x2\x6\xFFFF\x1\x2\x1\xFFFF\x1\x3\x4\xFFFF\x1\x2\x3\xFFFF\x1\x2\xA"+
				"\xFFFF\x1\x2\xD\xFFFF\x1\x2\x7\xFFFF\x1\x2\x7\xFFFF\x1\x2\x1\xFFFF\x1"+
				"\x2\x5\xFFFF\x1\x2\x2\xFFFF\x2\x2\x2\xFFFF\x1\x2\x1\xFFFF\x1\x2\x1\xFFFF"+
				"\x1\x2\x11\xFFFF\x1\x1\x2\x2\x5\xFFFF\x1\x2\xA\xFFFF\x1\x2\x2\xFFFF"+
				"\x1\x2\x4\xFFFF\x1\x2\x1\x3\x5\xFFFF\x1\x2\x4\xFFFF\x1\x2",
				"\x1\x2\x6\xFFFF\x1\x2\x1\xFFFF\x1\x3\x4\xFFFF\x1\x2\x3\xFFFF\x1\x2\xA"+
				"\xFFFF\x1\x2\xD\xFFFF\x1\x2\x7\xFFFF\x1\x2\x7\xFFFF\x1\x2\x1\xFFFF\x1"+
				"\x2\x5\xFFFF\x1\x2\x2\xFFFF\x2\x2\x2\xFFFF\x1\x2\x1\xFFFF\x1\x2\x1\xFFFF"+
				"\x1\x2\x11\xFFFF\x1\x1\x2\x2\x5\xFFFF\x1\x2\xA\xFFFF\x1\x2\x2\xFFFF"+
				"\x1\x2\x4\xFFFF\x1\x2\x1\x3\x5\xFFFF\x1\x2\x4\xFFFF\x1\x2",
				"",
				""
			};

		private static readonly short[] DFA231_eot = DFA.UnpackEncodedString(DFA231_eotS);
		private static readonly short[] DFA231_eof = DFA.UnpackEncodedString(DFA231_eofS);
		private static readonly char[] DFA231_min = DFA.UnpackEncodedStringToUnsignedChars(DFA231_minS);
		private static readonly char[] DFA231_max = DFA.UnpackEncodedStringToUnsignedChars(DFA231_maxS);
		private static readonly short[] DFA231_accept = DFA.UnpackEncodedString(DFA231_acceptS);
		private static readonly short[] DFA231_special = DFA.UnpackEncodedString(DFA231_specialS);
		private static readonly short[][] DFA231_transition;

		static DFA231()
		{
			int numStates = DFA231_transitionS.Length;
			DFA231_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA231_transition[i] = DFA.UnpackEncodedString(DFA231_transitionS[i]);
			}
		}

		public DFA231( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 231;
			this.eot = DFA231_eot;
			this.eof = DFA231_eof;
			this.min = DFA231_min;
			this.max = DFA231_max;
			this.accept = DFA231_accept;
			this.special = DFA231_special;
			this.transition = DFA231_transition;
		}

		public override string Description { get { return "513:60: ( ( SkipSpace )* assignmentExpression )?"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private class DFA240 : DFA
	{
		private const string DFA240_eotS =
			"\x5\xFFFF";
		private const string DFA240_eofS =
			"\x5\xFFFF";
		private const string DFA240_minS =
			"\x1\x93\x2\x3F\x2\xFFFF";
		private const string DFA240_maxS =
			"\x1\x93\x2\x94\x2\xFFFF";
		private const string DFA240_acceptS =
			"\x3\xFFFF\x1\x1\x1\x2";
		private const string DFA240_specialS =
			"\x5\xFFFF}>";
		private static readonly string[] DFA240_transitionS =
			{
				"\x1\x1",
				"\x1\x3\x16\xFFFF\x1\x3\x15\xFFFF\x1\x2\x1\x3\x26\xFFFF\x1\x4",
				"\x1\x3\x16\xFFFF\x1\x3\x15\xFFFF\x1\x2\x1\x3\x26\xFFFF\x1\x4",
				"",
				""
			};

		private static readonly short[] DFA240_eot = DFA.UnpackEncodedString(DFA240_eotS);
		private static readonly short[] DFA240_eof = DFA.UnpackEncodedString(DFA240_eofS);
		private static readonly char[] DFA240_min = DFA.UnpackEncodedStringToUnsignedChars(DFA240_minS);
		private static readonly char[] DFA240_max = DFA.UnpackEncodedStringToUnsignedChars(DFA240_maxS);
		private static readonly short[] DFA240_accept = DFA.UnpackEncodedString(DFA240_acceptS);
		private static readonly short[] DFA240_special = DFA.UnpackEncodedString(DFA240_specialS);
		private static readonly short[][] DFA240_transition;

		static DFA240()
		{
			int numStates = DFA240_transitionS.Length;
			DFA240_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA240_transition[i] = DFA.UnpackEncodedString(DFA240_transitionS[i]);
			}
		}

		public DFA240( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 240;
			this.eot = DFA240_eot;
			this.eof = DFA240_eof;
			this.min = DFA240_min;
			this.max = DFA240_max;
			this.accept = DFA240_accept;
			this.special = DFA240_special;
			this.transition = DFA240_transition;
		}

		public override string Description { get { return "518:1: objectLiteral : ( '{' ( SkipSpace )* propertyNameAndValue ( ( SkipSpace )* ',' ( SkipSpace )* pn2= propertyNameAndValue )* ( SkipSpace )* '}' -> ^( INLINE_OBJ_INIT propertyNameAndValue ( $pn2)* ) | '{' ( SkipSpace )* '}' -> ^( INLINE_OBJ_INIT ) );"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private class DFA237 : DFA
	{
		private const string DFA237_eotS =
			"\x4\xFFFF";
		private const string DFA237_eofS =
			"\x4\xFFFF";
		private const string DFA237_minS =
			"\x2\x15\x2\xFFFF";
		private const string DFA237_maxS =
			"\x2\x94\x2\xFFFF";
		private const string DFA237_acceptS =
			"\x2\xFFFF\x1\x2\x1\x1";
		private const string DFA237_specialS =
			"\x4\xFFFF}>";
		private static readonly string[] DFA237_transitionS =
			{
				"\x1\x3\x56\xFFFF\x1\x1\x27\xFFFF\x1\x2",
				"\x1\x3\x56\xFFFF\x1\x1\x27\xFFFF\x1\x2",
				"",
				""
			};

		private static readonly short[] DFA237_eot = DFA.UnpackEncodedString(DFA237_eotS);
		private static readonly short[] DFA237_eof = DFA.UnpackEncodedString(DFA237_eofS);
		private static readonly char[] DFA237_min = DFA.UnpackEncodedStringToUnsignedChars(DFA237_minS);
		private static readonly char[] DFA237_max = DFA.UnpackEncodedStringToUnsignedChars(DFA237_maxS);
		private static readonly short[] DFA237_accept = DFA.UnpackEncodedString(DFA237_acceptS);
		private static readonly short[] DFA237_special = DFA.UnpackEncodedString(DFA237_specialS);
		private static readonly short[][] DFA237_transition;

		static DFA237()
		{
			int numStates = DFA237_transitionS.Length;
			DFA237_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA237_transition[i] = DFA.UnpackEncodedString(DFA237_transitionS[i]);
			}
		}

		public DFA237( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 237;
			this.eot = DFA237_eot;
			this.eof = DFA237_eof;
			this.min = DFA237_min;
			this.max = DFA237_max;
			this.accept = DFA237_accept;
			this.special = DFA237_special;
			this.transition = DFA237_transition;
		}

		public override string Description { get { return "()* loopback of 519:43: ( ( SkipSpace )* ',' ( SkipSpace )* pn2= propertyNameAndValue )*"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private class DFA244 : DFA
	{
		private const string DFA244_eotS =
			"\x15\xFFFF";
		private const string DFA244_eofS =
			"\x15\xFFFF";
		private const string DFA244_minS =
			"\x1\x37\x1\x31\x1\x3F\x2\x75\x1\x84\x1\xFFFF\x3\x3F\x1\x82\x1\x84\x1"+
			"\x88\x1\x75\x2\xFFFF\x2\x3F\x2\x75\x1\x3F";
		private const string DFA244_maxS =
			"\x1\x37\x1\x87\x1\x3F\x2\x94\x1\x88\x1\xFFFF\x1\x3F\x1\x76\x1\x3F\x1"+
			"\x94\x2\x88\x1\x94\x2\xFFFF\x2\x76\x2\x94\x1\x76";
		private const string DFA244_acceptS =
			"\x6\xFFFF\x1\x1\x7\xFFFF\x1\x2\x1\x3\x5\xFFFF";
		private const string DFA244_specialS =
			"\x15\xFFFF}>";
		private static readonly string[] DFA244_transitionS =
			{
				"\x1\x1",
				"\x1\x4\xD\xFFFF\x1\x3\x47\xFFFF\x1\x2",
				"\x1\x5",
				"\x1\x7\x1E\xFFFF\x1\x6",
				"\x1\x7\x1E\xFFFF\x1\x6",
				"\x1\x8\x3\xFFFF\x1\x9",
				"",
				"\x1\xA",
				"\x1\xB\x36\xFFFF\x1\xC",
				"\x1\xD",
				"\x1\xF\x11\xFFFF\x1\xE",
				"\x1\x10\x3\xFFFF\x1\x9",
				"\x1\x9",
				"\x1\x7\xE\xFFFF\x1\x11\xF\xFFFF\x1\x6",
				"",
				"",
				"\x1\xB\x36\xFFFF\x1\xC",
				"\x1\x12\x36\xFFFF\x1\x13",
				"\x1\x7\xE\xFFFF\x1\x14\xF\xFFFF\x1\x6",
				"\x1\x7\x1E\xFFFF\x1\x6",
				"\x1\x12\x36\xFFFF\x1\x13"
			};

		private static readonly short[] DFA244_eot = DFA.UnpackEncodedString(DFA244_eotS);
		private static readonly short[] DFA244_eof = DFA.UnpackEncodedString(DFA244_eofS);
		private static readonly char[] DFA244_min = DFA.UnpackEncodedStringToUnsignedChars(DFA244_minS);
		private static readonly char[] DFA244_max = DFA.UnpackEncodedStringToUnsignedChars(DFA244_maxS);
		private static readonly short[] DFA244_accept = DFA.UnpackEncodedString(DFA244_acceptS);
		private static readonly short[] DFA244_special = DFA.UnpackEncodedString(DFA244_specialS);
		private static readonly short[][] DFA244_transition;

		static DFA244()
		{
			int numStates = DFA244_transitionS.Length;
			DFA244_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA244_transition[i] = DFA.UnpackEncodedString(DFA244_transitionS[i]);
			}
		}

		public DFA244( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 244;
			this.eot = DFA244_eot;
			this.eof = DFA244_eof;
			this.min = DFA244_min;
			this.max = DFA244_max;
			this.accept = DFA244_accept;
			this.special = DFA244_special;
			this.transition = DFA244_transition;
		}

		public override string Description { get { return "547:1: importedNotation : ( IMPORT_START importedTypeName '}' | IMPORT_START type= importedTypeName TYPE_SEPERATOR ident= Identifier '}' -> ^( TYPE_SEPERATOR $type $ident) | IMPORT_START type= importedTypeName TYPE_SEPERATOR ident= Identifier args= importedMethodArguments '}' -> ^( TYPE_SEPERATOR $type $ident $args) );"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private class DFA249 : DFA
	{
		private const string DFA249_eotS =
			"\x4\xFFFF";
		private const string DFA249_eofS =
			"\x4\xFFFF";
		private const string DFA249_minS =
			"\x2\x31\x2\xFFFF";
		private const string DFA249_maxS =
			"\x2\x87\x2\xFFFF";
		private const string DFA249_acceptS =
			"\x2\xFFFF\x1\x1\x1\x2";
		private const string DFA249_specialS =
			"\x4\xFFFF}>";
		private static readonly string[] DFA249_transitionS =
			{
				"\x1\x2\xD\xFFFF\x1\x2\x2C\xFFFF\x1\x1\x16\xFFFF\x1\x3\x3\xFFFF\x1\x2",
				"\x1\x2\xD\xFFFF\x1\x2\x2C\xFFFF\x1\x1\x16\xFFFF\x1\x3\x3\xFFFF\x1\x2",
				"",
				""
			};

		private static readonly short[] DFA249_eot = DFA.UnpackEncodedString(DFA249_eotS);
		private static readonly short[] DFA249_eof = DFA.UnpackEncodedString(DFA249_eofS);
		private static readonly char[] DFA249_min = DFA.UnpackEncodedStringToUnsignedChars(DFA249_minS);
		private static readonly char[] DFA249_max = DFA.UnpackEncodedStringToUnsignedChars(DFA249_maxS);
		private static readonly short[] DFA249_accept = DFA.UnpackEncodedString(DFA249_acceptS);
		private static readonly short[] DFA249_special = DFA.UnpackEncodedString(DFA249_specialS);
		private static readonly short[][] DFA249_transition;

		static DFA249()
		{
			int numStates = DFA249_transitionS.Length;
			DFA249_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA249_transition[i] = DFA.UnpackEncodedString(DFA249_transitionS[i]);
			}
		}

		public DFA249( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 249;
			this.eot = DFA249_eot;
			this.eof = DFA249_eof;
			this.min = DFA249_min;
			this.max = DFA249_max;
			this.accept = DFA249_accept;
			this.special = DFA249_special;
			this.transition = DFA249_transition;
		}

		public override string Description { get { return "556:11: ( ( SkipSpace )* importedTypeName ( ( SkipSpace )* ',' ( SkipSpace )* importedTypeName )* )?"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private class DFA248 : DFA
	{
		private const string DFA248_eotS =
			"\x4\xFFFF";
		private const string DFA248_eofS =
			"\x4\xFFFF";
		private const string DFA248_minS =
			"\x2\x15\x2\xFFFF";
		private const string DFA248_maxS =
			"\x2\x83\x2\xFFFF";
		private const string DFA248_acceptS =
			"\x2\xFFFF\x1\x2\x1\x1";
		private const string DFA248_specialS =
			"\x4\xFFFF}>";
		private static readonly string[] DFA248_transitionS =
			{
				"\x1\x3\x56\xFFFF\x1\x1\x16\xFFFF\x1\x2",
				"\x1\x3\x56\xFFFF\x1\x1\x16\xFFFF\x1\x2",
				"",
				""
			};

		private static readonly short[] DFA248_eot = DFA.UnpackEncodedString(DFA248_eotS);
		private static readonly short[] DFA248_eof = DFA.UnpackEncodedString(DFA248_eofS);
		private static readonly char[] DFA248_min = DFA.UnpackEncodedStringToUnsignedChars(DFA248_minS);
		private static readonly char[] DFA248_max = DFA.UnpackEncodedStringToUnsignedChars(DFA248_maxS);
		private static readonly short[] DFA248_accept = DFA.UnpackEncodedString(DFA248_acceptS);
		private static readonly short[] DFA248_special = DFA.UnpackEncodedString(DFA248_specialS);
		private static readonly short[][] DFA248_transition;

		static DFA248()
		{
			int numStates = DFA248_transitionS.Length;
			DFA248_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA248_transition[i] = DFA.UnpackEncodedString(DFA248_transitionS[i]);
			}
		}

		public DFA248( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 248;
			this.eot = DFA248_eot;
			this.eof = DFA248_eof;
			this.min = DFA248_min;
			this.max = DFA248_max;
			this.accept = DFA248_accept;
			this.special = DFA248_special;
			this.transition = DFA248_transition;
		}

		public override string Description { get { return "()* loopback of 556:40: ( ( SkipSpace )* ',' ( SkipSpace )* importedTypeName )*"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}


	#endregion DFA

	#region Follow sets
	private static class Follow
	{
		public static readonly BitSet _SkipSpace_in_program351 = new BitSet(new ulong[]{0x80800200448C1000UL,0xC012F004854C8280UL,0xFE884UL});
		public static readonly BitSet _sourceElements_in_program355 = new BitSet(new ulong[]{0x0UL,0x100000000000UL});
		public static readonly BitSet _SkipSpace_in_program357 = new BitSet(new ulong[]{0x0UL,0x100000000000UL});
		public static readonly BitSet _EOF_in_program361 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _sourceElement_in_sourceElements379 = new BitSet(new ulong[]{0x80800200448C1002UL,0xC012F004854C8280UL,0xFE884UL});
		public static readonly BitSet _SkipSpace_in_sourceElements382 = new BitSet(new ulong[]{0x80800200448C1000UL,0xC012F004854C8280UL,0xFE884UL});
		public static readonly BitSet _sourceElement_in_sourceElements385 = new BitSet(new ulong[]{0x80800200448C1002UL,0xC012F004854C8280UL,0xFE884UL});
		public static readonly BitSet _functionDeclaration_in_sourceElement421 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _statement_in_sourceElement429 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _142_in_functionDeclaration447 = new BitSet(new ulong[]{0x8000000000000000UL,0x100000000000UL});
		public static readonly BitSet _SkipSpace_in_functionDeclaration449 = new BitSet(new ulong[]{0x8000000000000000UL,0x100000000000UL});
		public static readonly BitSet _Identifier_in_functionDeclaration454 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x4UL});
		public static readonly BitSet _SkipSpace_in_functionDeclaration456 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x4UL});
		public static readonly BitSet _formalParameterList_in_functionDeclaration461 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x80000UL});
		public static readonly BitSet _SkipSpace_in_functionDeclaration463 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x80000UL});
		public static readonly BitSet _functionBody_in_functionDeclaration468 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _142_in_functionExpression508 = new BitSet(new ulong[]{0x8000000000000000UL,0x100000000000UL,0x4UL});
		public static readonly BitSet _SkipSpace_in_functionExpression510 = new BitSet(new ulong[]{0x8000000000000000UL,0x100000000000UL,0x4UL});
		public static readonly BitSet _Identifier_in_functionExpression515 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x4UL});
		public static readonly BitSet _SkipSpace_in_functionExpression518 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x4UL});
		public static readonly BitSet _formalParameterList_in_functionExpression523 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x80000UL});
		public static readonly BitSet _SkipSpace_in_functionExpression525 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x80000UL});
		public static readonly BitSet _functionBody_in_functionExpression530 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _130_in_formalParameterList571 = new BitSet(new ulong[]{0x8000000000000000UL,0x100000000000UL,0x8UL});
		public static readonly BitSet _SkipSpace_in_formalParameterList574 = new BitSet(new ulong[]{0x8000000000000000UL,0x100000000000UL});
		public static readonly BitSet _Identifier_in_formalParameterList577 = new BitSet(new ulong[]{0x200000UL,0x100000000000UL,0x8UL});
		public static readonly BitSet _SkipSpace_in_formalParameterList580 = new BitSet(new ulong[]{0x200000UL,0x100000000000UL});
		public static readonly BitSet _COMMA_in_formalParameterList583 = new BitSet(new ulong[]{0x8000000000000000UL,0x100000000000UL});
		public static readonly BitSet _SkipSpace_in_formalParameterList585 = new BitSet(new ulong[]{0x8000000000000000UL,0x100000000000UL});
		public static readonly BitSet _Identifier_in_formalParameterList588 = new BitSet(new ulong[]{0x200000UL,0x100000000000UL,0x8UL});
		public static readonly BitSet _SkipSpace_in_formalParameterList594 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x8UL});
		public static readonly BitSet _131_in_formalParameterList597 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _147_in_functionBody631 = new BitSet(new ulong[]{0x80800200448C1000UL,0xC012F004854C8280UL,0xFE884UL});
		public static readonly BitSet _SkipSpace_in_functionBody633 = new BitSet(new ulong[]{0x80800200448C1000UL,0xC012F004854C8280UL,0xFE884UL});
		public static readonly BitSet _sourceElements_in_functionBody638 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x100000UL});
		public static readonly BitSet _SkipSpace_in_functionBody640 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x100000UL});
		public static readonly BitSet _148_in_functionBody643 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _statementBlock_in_statement678 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _variableStatement_in_statement686 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _emptyStatement_in_statement694 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _expressionStatement_in_statement702 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _ifStatement_in_statement710 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _iterationStatement_in_statement718 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _continueStatement_in_statement726 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _breakStatement_in_statement734 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _returnStatement_in_statement742 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _withStatement_in_statement750 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _labelledStatement_in_statement758 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _switchStatement_in_statement766 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _throwStatement_in_statement774 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _tryStatement_in_statement782 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _147_in_statementBlock799 = new BitSet(new ulong[]{0x80800200448C1000UL,0xC012F004854C8280UL,0x1FE884UL});
		public static readonly BitSet _SkipSpace_in_statementBlock801 = new BitSet(new ulong[]{0x80800200448C1000UL,0xC012F004854C8280UL,0x1FE884UL});
		public static readonly BitSet _statementList_in_statementBlock804 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x100000UL});
		public static readonly BitSet _SkipSpace_in_statementBlock807 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x100000UL});
		public static readonly BitSet _148_in_statementBlock810 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _statement_in_statementList844 = new BitSet(new ulong[]{0x80800200448C1002UL,0xC012F004854C8280UL,0xFE884UL});
		public static readonly BitSet _SkipSpace_in_statementList847 = new BitSet(new ulong[]{0x80800200448C1000UL,0xC012F004854C8280UL,0xFE884UL});
		public static readonly BitSet _statement_in_statementList851 = new BitSet(new ulong[]{0x80800200448C1002UL,0xC012F004854C8280UL,0xFE884UL});
		public static readonly BitSet _VAR_in_variableStatement870 = new BitSet(new ulong[]{0x8000000000000000UL,0x100000000000UL});
		public static readonly BitSet _SkipSpace_in_variableStatement873 = new BitSet(new ulong[]{0x8000000000000000UL,0x100000000000UL});
		public static readonly BitSet _variableDeclarationList_in_variableStatement877 = new BitSet(new ulong[]{0x0UL,0x100400000000UL});
		public static readonly BitSet _SkipSpace_in_variableStatement879 = new BitSet(new ulong[]{0x0UL,0x100400000000UL});
		public static readonly BitSet _SEMI_in_variableStatement883 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _variableDeclaration_in_variableDeclarationList901 = new BitSet(new ulong[]{0x200002UL,0x100000000000UL});
		public static readonly BitSet _SkipSpace_in_variableDeclarationList904 = new BitSet(new ulong[]{0x200000UL,0x100000000000UL});
		public static readonly BitSet _COMMA_in_variableDeclarationList907 = new BitSet(new ulong[]{0x8000000000000000UL,0x100000000000UL});
		public static readonly BitSet _SkipSpace_in_variableDeclarationList909 = new BitSet(new ulong[]{0x8000000000000000UL,0x100000000000UL});
		public static readonly BitSet _variableDeclaration_in_variableDeclarationList912 = new BitSet(new ulong[]{0x200002UL,0x100000000000UL});
		public static readonly BitSet _variableDeclarationNoIn_in_variableDeclarationListNoIn936 = new BitSet(new ulong[]{0x200002UL,0x100000000000UL});
		public static readonly BitSet _SkipSpace_in_variableDeclarationListNoIn939 = new BitSet(new ulong[]{0x200000UL,0x100000000000UL});
		public static readonly BitSet _COMMA_in_variableDeclarationListNoIn942 = new BitSet(new ulong[]{0x8000000000000000UL,0x100000000000UL});
		public static readonly BitSet _SkipSpace_in_variableDeclarationListNoIn944 = new BitSet(new ulong[]{0x8000000000000000UL,0x100000000000UL});
		public static readonly BitSet _variableDeclarationNoIn_in_variableDeclarationListNoIn947 = new BitSet(new ulong[]{0x200002UL,0x100000000000UL});
		public static readonly BitSet _Identifier_in_variableDeclaration971 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _Identifier_in_variableDeclaration981 = new BitSet(new ulong[]{0x100UL,0x100000000000UL});
		public static readonly BitSet _SkipSpace_in_variableDeclaration983 = new BitSet(new ulong[]{0x100UL,0x100000000000UL});
		public static readonly BitSet _assignmentOnlyOperator_in_variableDeclaration988 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _SkipSpace_in_variableDeclaration990 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _assignmentExpression_in_variableDeclaration995 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _Identifier_in_variableDeclarationNoIn1035 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _Identifier_in_variableDeclarationNoIn1045 = new BitSet(new ulong[]{0x100UL,0x100000000000UL});
		public static readonly BitSet _SkipSpace_in_variableDeclarationNoIn1047 = new BitSet(new ulong[]{0x100UL,0x100000000000UL});
		public static readonly BitSet _assignmentOnlyOperator_in_variableDeclarationNoIn1052 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _SkipSpace_in_variableDeclarationNoIn1054 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _assignmentExpressionNoIn_in_variableDeclarationNoIn1059 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _ASN_in_assignmentOnlyOperator1099 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _SEMI_in_emptyStatement1116 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _expression_in_expressionStatement1149 = new BitSet(new ulong[]{0x0UL,0x100400000000UL});
		public static readonly BitSet _SkipSpace_in_expressionStatement1151 = new BitSet(new ulong[]{0x0UL,0x100400000000UL});
		public static readonly BitSet _SEMI_in_expressionStatement1155 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _143_in_ifStatement1173 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x4UL});
		public static readonly BitSet _SkipSpace_in_ifStatement1175 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x4UL});
		public static readonly BitSet _130_in_ifStatement1178 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _SkipSpace_in_ifStatement1180 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _expression_in_ifStatement1185 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x8UL});
		public static readonly BitSet _SkipSpace_in_ifStatement1187 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x8UL});
		public static readonly BitSet _131_in_ifStatement1190 = new BitSet(new ulong[]{0x80800200448C1000UL,0xC012F004854C8280UL,0xFE884UL});
		public static readonly BitSet _SkipSpace_in_ifStatement1192 = new BitSet(new ulong[]{0x80800200448C1000UL,0xC012F004854C8280UL,0xFE884UL});
		public static readonly BitSet _statement_in_ifStatement1197 = new BitSet(new ulong[]{0x2UL,0x100000000000UL,0x1000UL});
		public static readonly BitSet _SkipSpace_in_ifStatement1200 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x1000UL});
		public static readonly BitSet _140_in_ifStatement1203 = new BitSet(new ulong[]{0x80800200448C1000UL,0xC012F004854C8280UL,0xFE884UL});
		public static readonly BitSet _SkipSpace_in_ifStatement1205 = new BitSet(new ulong[]{0x80800200448C1000UL,0xC012F004854C8280UL,0xFE884UL});
		public static readonly BitSet _statement_in_ifStatement1210 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _doWhileStatement_in_iterationStatement1253 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _whileStatement_in_iterationStatement1261 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _forStatement_in_iterationStatement1269 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _forInStatement_in_iterationStatement1277 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _139_in_doWhileStatement1294 = new BitSet(new ulong[]{0x80800200448C1000UL,0xC012F004854C8280UL,0xFE884UL});
		public static readonly BitSet _SkipSpace_in_doWhileStatement1296 = new BitSet(new ulong[]{0x80800200448C1000UL,0xC012F004854C8280UL,0xFE884UL});
		public static readonly BitSet _statement_in_doWhileStatement1301 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x20000UL});
		public static readonly BitSet _SkipSpace_in_doWhileStatement1303 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x20000UL});
		public static readonly BitSet _145_in_doWhileStatement1306 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x4UL});
		public static readonly BitSet _SkipSpace_in_doWhileStatement1308 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x4UL});
		public static readonly BitSet _130_in_doWhileStatement1311 = new BitSet(new ulong[]{0x8080020044081000UL,0x80106000054C8280UL,0x84084UL});
		public static readonly BitSet _expression_in_doWhileStatement1315 = new BitSet(new ulong[]{0x0UL,0x0UL,0x8UL});
		public static readonly BitSet _131_in_doWhileStatement1317 = new BitSet(new ulong[]{0x0UL,0x100400000000UL});
		public static readonly BitSet _SkipSpace_in_doWhileStatement1319 = new BitSet(new ulong[]{0x0UL,0x100400000000UL});
		public static readonly BitSet _SEMI_in_doWhileStatement1322 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _145_in_whileStatement1359 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x4UL});
		public static readonly BitSet _SkipSpace_in_whileStatement1361 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x4UL});
		public static readonly BitSet _130_in_whileStatement1364 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _SkipSpace_in_whileStatement1366 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _expression_in_whileStatement1371 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x8UL});
		public static readonly BitSet _SkipSpace_in_whileStatement1373 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x8UL});
		public static readonly BitSet _131_in_whileStatement1376 = new BitSet(new ulong[]{0x80800200448C1000UL,0xC012F004854C8280UL,0xFE884UL});
		public static readonly BitSet _SkipSpace_in_whileStatement1378 = new BitSet(new ulong[]{0x80800200448C1000UL,0xC012F004854C8280UL,0xFE884UL});
		public static readonly BitSet _statement_in_whileStatement1383 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _141_in_forStatement1420 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x4UL});
		public static readonly BitSet _SkipSpace_in_forStatement1422 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x4UL});
		public static readonly BitSet _130_in_forStatement1425 = new BitSet(new ulong[]{0x8080020044081000UL,0xC0107004054C8280UL,0x84084UL});
		public static readonly BitSet _SkipSpace_in_forStatement1427 = new BitSet(new ulong[]{0x8080020044081000UL,0xC0107004054C8280UL,0x84084UL});
		public static readonly BitSet _forStatementInitialiserPart_in_forStatement1433 = new BitSet(new ulong[]{0x0UL,0x100400000000UL});
		public static readonly BitSet _SkipSpace_in_forStatement1437 = new BitSet(new ulong[]{0x0UL,0x100400000000UL});
		public static readonly BitSet _SEMI_in_forStatement1442 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107004054C8280UL,0x84084UL});
		public static readonly BitSet _SkipSpace_in_forStatement1444 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107004054C8280UL,0x84084UL});
		public static readonly BitSet _expression_in_forStatement1450 = new BitSet(new ulong[]{0x0UL,0x100400000000UL});
		public static readonly BitSet _SkipSpace_in_forStatement1454 = new BitSet(new ulong[]{0x0UL,0x100400000000UL});
		public static readonly BitSet _SEMI_in_forStatement1459 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x8408CUL});
		public static readonly BitSet _SkipSpace_in_forStatement1461 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x8408CUL});
		public static readonly BitSet _expression_in_forStatement1467 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x8UL});
		public static readonly BitSet _SkipSpace_in_forStatement1471 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x8UL});
		public static readonly BitSet _131_in_forStatement1474 = new BitSet(new ulong[]{0x80800200448C1000UL,0xC012F004854C8280UL,0xFE884UL});
		public static readonly BitSet _SkipSpace_in_forStatement1476 = new BitSet(new ulong[]{0x80800200448C1000UL,0xC012F004854C8280UL,0xFE884UL});
		public static readonly BitSet _statement_in_forStatement1481 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _expressionNoIn_in_forStatementInitialiserPart1534 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _VAR_in_forStatementInitialiserPart1542 = new BitSet(new ulong[]{0x8000000000000000UL,0x100000000000UL});
		public static readonly BitSet _SkipSpace_in_forStatementInitialiserPart1544 = new BitSet(new ulong[]{0x8000000000000000UL,0x100000000000UL});
		public static readonly BitSet _variableDeclarationListNoIn_in_forStatementInitialiserPart1548 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _141_in_forInStatement1565 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x4UL});
		public static readonly BitSet _SkipSpace_in_forInStatement1567 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x4UL});
		public static readonly BitSet _130_in_forInStatement1570 = new BitSet(new ulong[]{0x8080020000080000UL,0x4000700000488000UL,0x84084UL});
		public static readonly BitSet _SkipSpace_in_forInStatement1572 = new BitSet(new ulong[]{0x8080020000080000UL,0x4000700000488000UL,0x84084UL});
		public static readonly BitSet _forInStatementInitialiserPart_in_forInStatement1575 = new BitSet(new ulong[]{0x100000000000000UL,0x100000000000UL});
		public static readonly BitSet _SkipSpace_in_forInStatement1577 = new BitSet(new ulong[]{0x100000000000000UL,0x100000000000UL});
		public static readonly BitSet _IN_in_forInStatement1580 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _SkipSpace_in_forInStatement1582 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _expression_in_forInStatement1585 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x8UL});
		public static readonly BitSet _SkipSpace_in_forInStatement1587 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x8UL});
		public static readonly BitSet _131_in_forInStatement1590 = new BitSet(new ulong[]{0x80800200448C1000UL,0xC012F004854C8280UL,0xFE884UL});
		public static readonly BitSet _SkipSpace_in_forInStatement1592 = new BitSet(new ulong[]{0x80800200448C1000UL,0xC012F004854C8280UL,0xFE884UL});
		public static readonly BitSet _statement_in_forInStatement1595 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _leftHandSideExpression_in_forInStatementInitialiserPart1632 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _VAR_in_forInStatementInitialiserPart1640 = new BitSet(new ulong[]{0x8000000000000000UL,0x100000000000UL});
		public static readonly BitSet _SkipSpace_in_forInStatementInitialiserPart1642 = new BitSet(new ulong[]{0x8000000000000000UL,0x100000000000UL});
		public static readonly BitSet _variableDeclarationNoIn_in_forInStatementInitialiserPart1646 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _CONTINUE_in_continueStatement1663 = new BitSet(new ulong[]{0x8000000000000000UL,0x100400000000UL});
		public static readonly BitSet _Identifier_in_continueStatement1666 = new BitSet(new ulong[]{0x0UL,0x100400000000UL});
		public static readonly BitSet _SkipSpace_in_continueStatement1669 = new BitSet(new ulong[]{0x0UL,0x100400000000UL});
		public static readonly BitSet _SEMI_in_continueStatement1673 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _BREAK_in_breakStatement1691 = new BitSet(new ulong[]{0x8000000000000000UL,0x100400000000UL});
		public static readonly BitSet _Identifier_in_breakStatement1694 = new BitSet(new ulong[]{0x0UL,0x100400000000UL});
		public static readonly BitSet _SkipSpace_in_breakStatement1697 = new BitSet(new ulong[]{0x0UL,0x100400000000UL});
		public static readonly BitSet _SEMI_in_breakStatement1701 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _RETURN_in_returnStatement1719 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _SkipSpace_in_returnStatement1721 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _expression_in_returnStatement1726 = new BitSet(new ulong[]{0x0UL,0x100400000000UL});
		public static readonly BitSet _SkipSpace_in_returnStatement1728 = new BitSet(new ulong[]{0x0UL,0x100400000000UL});
		public static readonly BitSet _SEMI_in_returnStatement1731 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _RETURN_in_returnStatement1756 = new BitSet(new ulong[]{0x0UL,0x100400000000UL});
		public static readonly BitSet _SkipSpace_in_returnStatement1758 = new BitSet(new ulong[]{0x0UL,0x100400000000UL});
		public static readonly BitSet _SEMI_in_returnStatement1761 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _146_in_withStatement1792 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x4UL});
		public static readonly BitSet _SkipSpace_in_withStatement1794 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x4UL});
		public static readonly BitSet _130_in_withStatement1798 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _SkipSpace_in_withStatement1800 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _expression_in_withStatement1804 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x8UL});
		public static readonly BitSet _SkipSpace_in_withStatement1806 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x8UL});
		public static readonly BitSet _131_in_withStatement1810 = new BitSet(new ulong[]{0x80800200448C1000UL,0xC012F004854C8280UL,0xFE884UL});
		public static readonly BitSet _SkipSpace_in_withStatement1812 = new BitSet(new ulong[]{0x80800200448C1000UL,0xC012F004854C8280UL,0xFE884UL});
		public static readonly BitSet _statement_in_withStatement1816 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _Identifier_in_labelledStatement1833 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x20UL});
		public static readonly BitSet _SkipSpace_in_labelledStatement1835 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x20UL});
		public static readonly BitSet _133_in_labelledStatement1839 = new BitSet(new ulong[]{0x80800200448C1000UL,0xC012F004854C8280UL,0xFE884UL});
		public static readonly BitSet _SkipSpace_in_labelledStatement1841 = new BitSet(new ulong[]{0x80800200448C1000UL,0xC012F004854C8280UL,0xFE884UL});
		public static readonly BitSet _statement_in_labelledStatement1845 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _144_in_switchStatement1862 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x4UL});
		public static readonly BitSet _SkipSpace_in_switchStatement1864 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x4UL});
		public static readonly BitSet _130_in_switchStatement1868 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _SkipSpace_in_switchStatement1870 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _expression_in_switchStatement1874 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x8UL});
		public static readonly BitSet _SkipSpace_in_switchStatement1876 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x8UL});
		public static readonly BitSet _131_in_switchStatement1880 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x80000UL});
		public static readonly BitSet _SkipSpace_in_switchStatement1882 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x80000UL});
		public static readonly BitSet _caseBlock_in_switchStatement1886 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _147_in_caseBlock1903 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x100600UL});
		public static readonly BitSet _SkipSpace_in_caseBlock1906 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x200UL});
		public static readonly BitSet _caseClause_in_caseBlock1910 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x100600UL});
		public static readonly BitSet _SkipSpace_in_caseBlock1915 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x400UL});
		public static readonly BitSet _defaultClause_in_caseBlock1919 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x100200UL});
		public static readonly BitSet _SkipSpace_in_caseBlock1922 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x200UL});
		public static readonly BitSet _caseClause_in_caseBlock1926 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x100200UL});
		public static readonly BitSet _SkipSpace_in_caseBlock1932 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x100000UL});
		public static readonly BitSet _148_in_caseBlock1936 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _137_in_caseClause1953 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _SkipSpace_in_caseClause1955 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _expression_in_caseClause1959 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x20UL});
		public static readonly BitSet _SkipSpace_in_caseClause1961 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x20UL});
		public static readonly BitSet _133_in_caseClause1965 = new BitSet(new ulong[]{0x80800200448C1002UL,0xC012F004854C8280UL,0xFE884UL});
		public static readonly BitSet _SkipSpace_in_caseClause1967 = new BitSet(new ulong[]{0x80800200448C1002UL,0xC012F004854C8280UL,0xFE884UL});
		public static readonly BitSet _statementList_in_caseClause1971 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _138_in_defaultClause1989 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x20UL});
		public static readonly BitSet _SkipSpace_in_defaultClause1991 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x20UL});
		public static readonly BitSet _133_in_defaultClause1995 = new BitSet(new ulong[]{0x80800200448C1002UL,0xC012F004854C8280UL,0xFE884UL});
		public static readonly BitSet _SkipSpace_in_defaultClause1997 = new BitSet(new ulong[]{0x80800200448C1002UL,0xC012F004854C8280UL,0xFE884UL});
		public static readonly BitSet _statementList_in_defaultClause2001 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _THROW_in_throwStatement2019 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _SkipSpace_in_throwStatement2021 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _expression_in_throwStatement2024 = new BitSet(new ulong[]{0x0UL,0x100400000000UL});
		public static readonly BitSet _SkipSpace_in_throwStatement2026 = new BitSet(new ulong[]{0x0UL,0x100400000000UL});
		public static readonly BitSet _SEMI_in_throwStatement2029 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _TRY_in_tryStatement2062 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x80000UL});
		public static readonly BitSet _SkipSpace_in_tryStatement2064 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x80000UL});
		public static readonly BitSet _statementBlock_in_tryStatement2068 = new BitSet(new ulong[]{0x40000100000UL,0x100000000000UL});
		public static readonly BitSet _SkipSpace_in_tryStatement2070 = new BitSet(new ulong[]{0x40000100000UL,0x100000000000UL});
		public static readonly BitSet _finallyClause_in_tryStatement2075 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _catchClause_in_tryStatement2079 = new BitSet(new ulong[]{0x40000000002UL,0x100000000000UL});
		public static readonly BitSet _SkipSpace_in_tryStatement2082 = new BitSet(new ulong[]{0x40000000000UL,0x100000000000UL});
		public static readonly BitSet _finallyClause_in_tryStatement2086 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _CATCH_in_catchClause2106 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x4UL});
		public static readonly BitSet _SkipSpace_in_catchClause2108 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x4UL});
		public static readonly BitSet _130_in_catchClause2112 = new BitSet(new ulong[]{0x8000000000000000UL,0x100000000000UL});
		public static readonly BitSet _SkipSpace_in_catchClause2114 = new BitSet(new ulong[]{0x8000000000000000UL,0x100000000000UL});
		public static readonly BitSet _Identifier_in_catchClause2118 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x8UL});
		public static readonly BitSet _SkipSpace_in_catchClause2120 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x8UL});
		public static readonly BitSet _131_in_catchClause2124 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x80000UL});
		public static readonly BitSet _SkipSpace_in_catchClause2126 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x80000UL});
		public static readonly BitSet _statementBlock_in_catchClause2130 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _FINALLY_in_finallyClause2147 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x80000UL});
		public static readonly BitSet _SkipSpace_in_finallyClause2149 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x80000UL});
		public static readonly BitSet _statementBlock_in_finallyClause2153 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _assignmentExpression_in_expression2171 = new BitSet(new ulong[]{0x200002UL,0x100000000000UL});
		public static readonly BitSet _SkipSpace_in_expression2174 = new BitSet(new ulong[]{0x200000UL,0x100000000000UL});
		public static readonly BitSet _COMMA_in_expression2177 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _SkipSpace_in_expression2179 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _assignmentExpression_in_expression2182 = new BitSet(new ulong[]{0x200002UL,0x100000000000UL});
		public static readonly BitSet _assignmentExpressionNoIn_in_expressionNoIn2214 = new BitSet(new ulong[]{0x200002UL,0x100000000000UL});
		public static readonly BitSet _SkipSpace_in_expressionNoIn2217 = new BitSet(new ulong[]{0x200000UL,0x100000000000UL});
		public static readonly BitSet _COMMA_in_expressionNoIn2220 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _SkipSpace_in_expressionNoIn2222 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _assignmentExpressionNoIn_in_expressionNoIn2225 = new BitSet(new ulong[]{0x200002UL,0x100000000000UL});
		public static readonly BitSet _conditionalExpression_in_assignmentExpression2257 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _leftHandSideExpression_in_assignmentExpression2267 = new BitSet(new ulong[]{0x10014900UL,0x100105002002900UL});
		public static readonly BitSet _SkipSpace_in_assignmentExpression2269 = new BitSet(new ulong[]{0x10014900UL,0x100105002002900UL});
		public static readonly BitSet _assignmentOperator_in_assignmentExpression2274 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _SkipSpace_in_assignmentExpression2276 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _assignmentExpression_in_assignmentExpression2281 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _conditionalExpressionNoIn_in_assignmentExpressionNoIn2321 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _leftHandSideExpression_in_assignmentExpressionNoIn2331 = new BitSet(new ulong[]{0x10014900UL,0x100105002002900UL});
		public static readonly BitSet _SkipSpace_in_assignmentExpressionNoIn2333 = new BitSet(new ulong[]{0x10014900UL,0x100105002002900UL});
		public static readonly BitSet _assignmentOperator_in_assignmentExpressionNoIn2338 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _SkipSpace_in_assignmentExpressionNoIn2340 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _assignmentExpressionNoIn_in_assignmentExpressionNoIn2345 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _set_in_assignmentOperator2383 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _callExpression_in_leftHandSideExpression2490 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _newExpression_in_leftHandSideExpression2498 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _memberExpression_in_newExpression2515 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _NEW_in_newExpression2523 = new BitSet(new ulong[]{0x8080020000080000UL,0x700000488000UL,0x84084UL});
		public static readonly BitSet _SkipSpace_in_newExpression2525 = new BitSet(new ulong[]{0x8080020000080000UL,0x700000488000UL,0x84084UL});
		public static readonly BitSet _newExpression_in_newExpression2529 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _memberExpressionLHS_in_memberExpression2549 = new BitSet(new ulong[]{0x2UL,0x100000000000UL,0x90UL});
		public static readonly BitSet _SkipSpace_in_memberExpression2566 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x90UL});
		public static readonly BitSet _memberExpressionSuffix_in_memberExpression2571 = new BitSet(new ulong[]{0x2UL,0x100000000000UL,0x90UL});
		public static readonly BitSet _primaryExpression_in_memberExpressionLHS2603 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _functionExpression_in_memberExpressionLHS2611 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _newObjectExpression_in_memberExpressionLHS2619 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _NEW_in_newObjectExpression2636 = new BitSet(new ulong[]{0x8080020000080000UL,0x700000488000UL,0x84084UL});
		public static readonly BitSet _SkipSpace_in_newObjectExpression2638 = new BitSet(new ulong[]{0x8080020000080000UL,0x700000488000UL,0x84084UL});
		public static readonly BitSet _memberExpression_in_newObjectExpression2643 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x4UL});
		public static readonly BitSet _SkipSpace_in_newObjectExpression2645 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x4UL});
		public static readonly BitSet _arguments_in_newObjectExpression2650 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _indexSuffix_in_memberExpressionSuffix2687 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _propertyReferenceSuffix_in_memberExpressionSuffix2695 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _methodCallExpression_in_callExpression2715 = new BitSet(new ulong[]{0x2UL,0x100000000000UL,0x94UL});
		public static readonly BitSet _SkipSpace_in_callExpression2732 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x94UL});
		public static readonly BitSet _callExpressionSuffix_in_callExpression2737 = new BitSet(new ulong[]{0x2UL,0x100000000000UL,0x94UL});
		public static readonly BitSet _memberExpression_in_methodCallExpression2773 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x4UL});
		public static readonly BitSet _SkipSpace_in_methodCallExpression2775 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x4UL});
		public static readonly BitSet _arguments_in_methodCallExpression2780 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _arguments_in_callExpressionSuffix2817 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _indexSuffix_in_callExpressionSuffix2825 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _propertyReferenceSuffix_in_callExpressionSuffix2833 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _130_in_arguments2850 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x8408CUL});
		public static readonly BitSet _SkipSpace_in_arguments2853 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _assignmentExpression_in_arguments2856 = new BitSet(new ulong[]{0x200000UL,0x100000000000UL,0x8UL});
		public static readonly BitSet _SkipSpace_in_arguments2859 = new BitSet(new ulong[]{0x200000UL,0x100000000000UL});
		public static readonly BitSet _COMMA_in_arguments2862 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _SkipSpace_in_arguments2864 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _assignmentExpression_in_arguments2867 = new BitSet(new ulong[]{0x200000UL,0x100000000000UL,0x8UL});
		public static readonly BitSet _SkipSpace_in_arguments2873 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x8UL});
		public static readonly BitSet _131_in_arguments2876 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _135_in_indexSuffix2910 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _SkipSpace_in_indexSuffix2912 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _expression_in_indexSuffix2917 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x100UL});
		public static readonly BitSet _SkipSpace_in_indexSuffix2919 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x100UL});
		public static readonly BitSet _136_in_indexSuffix2922 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _132_in_propertyReferenceSuffix2960 = new BitSet(new ulong[]{0x8000000000000000UL,0x100000000000UL});
		public static readonly BitSet _SkipSpace_in_propertyReferenceSuffix2962 = new BitSet(new ulong[]{0x8000000000000000UL,0x100000000000UL});
		public static readonly BitSet _Identifier_in_propertyReferenceSuffix2967 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _132_in_propertyReferenceSuffix2992 = new BitSet(new ulong[]{0x80000000000000UL,0x100000000000UL});
		public static readonly BitSet _SkipSpace_in_propertyReferenceSuffix2994 = new BitSet(new ulong[]{0x80000000000000UL,0x100000000000UL});
		public static readonly BitSet _importedNotation_in_propertyReferenceSuffix2999 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _logicalORExpression_in_conditionalExpression3035 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x40UL});
		public static readonly BitSet _SkipSpace_in_conditionalExpression3037 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x40UL});
		public static readonly BitSet _134_in_conditionalExpression3040 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _SkipSpace_in_conditionalExpression3042 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _assignmentExpression_in_conditionalExpression3047 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x20UL});
		public static readonly BitSet _SkipSpace_in_conditionalExpression3049 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x20UL});
		public static readonly BitSet _133_in_conditionalExpression3052 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _SkipSpace_in_conditionalExpression3054 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _assignmentExpression_in_conditionalExpression3059 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _logicalORExpression_in_conditionalExpression3090 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _logicalORExpressionNoIn_in_conditionalExpressionNoIn3109 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x40UL});
		public static readonly BitSet _SkipSpace_in_conditionalExpressionNoIn3111 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x40UL});
		public static readonly BitSet _134_in_conditionalExpressionNoIn3114 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _SkipSpace_in_conditionalExpressionNoIn3116 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _assignmentExpressionNoIn_in_conditionalExpressionNoIn3121 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x20UL});
		public static readonly BitSet _SkipSpace_in_conditionalExpressionNoIn3123 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x20UL});
		public static readonly BitSet _133_in_conditionalExpressionNoIn3126 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _SkipSpace_in_conditionalExpressionNoIn3128 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _assignmentExpressionNoIn_in_conditionalExpressionNoIn3133 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _logicalORExpressionNoIn_in_conditionalExpressionNoIn3164 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _logicalANDExpression_in_logicalORExpression3184 = new BitSet(new ulong[]{0x2UL,0x100000800000UL});
		public static readonly BitSet _SkipSpace_in_logicalORExpression3201 = new BitSet(new ulong[]{0x0UL,0x100000800000UL});
		public static readonly BitSet _OR_in_logicalORExpression3206 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _SkipSpace_in_logicalORExpression3208 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _logicalANDExpression_in_logicalORExpression3213 = new BitSet(new ulong[]{0x2UL,0x100000800000UL});
		public static readonly BitSet _logicalANDExpressionNoIn_in_logicalORExpressionNoIn3251 = new BitSet(new ulong[]{0x2UL,0x100000800000UL});
		public static readonly BitSet _SkipSpace_in_logicalORExpressionNoIn3268 = new BitSet(new ulong[]{0x0UL,0x100000800000UL});
		public static readonly BitSet _OR_in_logicalORExpressionNoIn3273 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _SkipSpace_in_logicalORExpressionNoIn3275 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _logicalANDExpressionNoIn_in_logicalORExpressionNoIn3280 = new BitSet(new ulong[]{0x2UL,0x100000800000UL});
		public static readonly BitSet _bitwiseORExpression_in_logicalANDExpression3318 = new BitSet(new ulong[]{0x22UL,0x100000000000UL});
		public static readonly BitSet _SkipSpace_in_logicalANDExpression3335 = new BitSet(new ulong[]{0x20UL,0x100000000000UL});
		public static readonly BitSet _AND_in_logicalANDExpression3340 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _SkipSpace_in_logicalANDExpression3342 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _bitwiseORExpression_in_logicalANDExpression3347 = new BitSet(new ulong[]{0x22UL,0x100000000000UL});
		public static readonly BitSet _bitwiseORExpressionNoIn_in_logicalANDExpressionNoIn3385 = new BitSet(new ulong[]{0x22UL,0x100000000000UL});
		public static readonly BitSet _SkipSpace_in_logicalANDExpressionNoIn3402 = new BitSet(new ulong[]{0x20UL,0x100000000000UL});
		public static readonly BitSet _AND_in_logicalANDExpressionNoIn3407 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _SkipSpace_in_logicalANDExpressionNoIn3409 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _bitwiseORExpressionNoIn_in_logicalANDExpressionNoIn3414 = new BitSet(new ulong[]{0x22UL,0x100000000000UL});
		public static readonly BitSet _bitwiseXORExpression_in_bitwiseORExpression3452 = new BitSet(new ulong[]{0x2002UL,0x100000000000UL});
		public static readonly BitSet _SkipSpace_in_bitwiseORExpression3469 = new BitSet(new ulong[]{0x2000UL,0x100000000000UL});
		public static readonly BitSet _BIT_OR_in_bitwiseORExpression3474 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _SkipSpace_in_bitwiseORExpression3476 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _bitwiseXORExpression_in_bitwiseORExpression3481 = new BitSet(new ulong[]{0x2002UL,0x100000000000UL});
		public static readonly BitSet _bitwiseXORExpressionNoIn_in_bitwiseORExpressionNoIn3519 = new BitSet(new ulong[]{0x2002UL,0x100000000000UL});
		public static readonly BitSet _SkipSpace_in_bitwiseORExpressionNoIn3536 = new BitSet(new ulong[]{0x2000UL,0x100000000000UL});
		public static readonly BitSet _BIT_OR_in_bitwiseORExpressionNoIn3541 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _SkipSpace_in_bitwiseORExpressionNoIn3543 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _bitwiseXORExpressionNoIn_in_bitwiseORExpressionNoIn3548 = new BitSet(new ulong[]{0x2002UL,0x100000000000UL});
		public static readonly BitSet _bitwiseANDExpression_in_bitwiseXORExpression3586 = new BitSet(new ulong[]{0x8002UL,0x100000000000UL});
		public static readonly BitSet _SkipSpace_in_bitwiseXORExpression3603 = new BitSet(new ulong[]{0x8000UL,0x100000000000UL});
		public static readonly BitSet _BIT_XOR_in_bitwiseXORExpression3608 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _SkipSpace_in_bitwiseXORExpression3610 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _bitwiseANDExpression_in_bitwiseXORExpression3615 = new BitSet(new ulong[]{0x8002UL,0x100000000000UL});
		public static readonly BitSet _bitwiseANDExpressionNoIn_in_bitwiseXORExpressionNoIn3653 = new BitSet(new ulong[]{0x8002UL,0x100000000000UL});
		public static readonly BitSet _SkipSpace_in_bitwiseXORExpressionNoIn3670 = new BitSet(new ulong[]{0x8000UL,0x100000000000UL});
		public static readonly BitSet _BIT_XOR_in_bitwiseXORExpressionNoIn3675 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _SkipSpace_in_bitwiseXORExpressionNoIn3677 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _bitwiseANDExpressionNoIn_in_bitwiseXORExpressionNoIn3682 = new BitSet(new ulong[]{0x8002UL,0x100000000000UL});
		public static readonly BitSet _equalityExpression_in_bitwiseANDExpression3720 = new BitSet(new ulong[]{0x402UL,0x100000000000UL});
		public static readonly BitSet _SkipSpace_in_bitwiseANDExpression3737 = new BitSet(new ulong[]{0x400UL,0x100000000000UL});
		public static readonly BitSet _BIT_AND_in_bitwiseANDExpression3742 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _SkipSpace_in_bitwiseANDExpression3744 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _equalityExpression_in_bitwiseANDExpression3749 = new BitSet(new ulong[]{0x402UL,0x100000000000UL});
		public static readonly BitSet _equalityExpressionNoIn_in_bitwiseANDExpressionNoIn3787 = new BitSet(new ulong[]{0x402UL,0x100000000000UL});
		public static readonly BitSet _SkipSpace_in_bitwiseANDExpressionNoIn3804 = new BitSet(new ulong[]{0x400UL,0x100000000000UL});
		public static readonly BitSet _BIT_AND_in_bitwiseANDExpressionNoIn3809 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _SkipSpace_in_bitwiseANDExpressionNoIn3811 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _equalityExpressionNoIn_in_bitwiseANDExpressionNoIn3816 = new BitSet(new ulong[]{0x402UL,0x100000000000UL});
		public static readonly BitSet _relationalExpression_in_equalityExpression3854 = new BitSet(new ulong[]{0x1000000002UL,0x100240004000UL});
		public static readonly BitSet _SkipSpace_in_equalityExpression3871 = new BitSet(new ulong[]{0x1000000000UL,0x100240004000UL});
		public static readonly BitSet _equalityOperator_in_equalityExpression3876 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _SkipSpace_in_equalityExpression3878 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _relationalExpression_in_equalityExpression3883 = new BitSet(new ulong[]{0x1000000002UL,0x100240004000UL});
		public static readonly BitSet _relationalExpressionNoIn_in_equalityExpressionNoIn3921 = new BitSet(new ulong[]{0x1000000002UL,0x100240004000UL});
		public static readonly BitSet _SkipSpace_in_equalityExpressionNoIn3938 = new BitSet(new ulong[]{0x1000000000UL,0x100240004000UL});
		public static readonly BitSet _equalityOperator_in_equalityExpressionNoIn3943 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _SkipSpace_in_equalityExpressionNoIn3945 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _relationalExpressionNoIn_in_equalityExpressionNoIn3950 = new BitSet(new ulong[]{0x1000000002UL,0x100240004000UL});
		public static readonly BitSet _set_in_equalityOperator3983 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _shiftExpression_in_relationalExpression4017 = new BitSet(new ulong[]{0x4101800000000002UL,0x10000000000CUL});
		public static readonly BitSet _SkipSpace_in_relationalExpression4034 = new BitSet(new ulong[]{0x4101800000000000UL,0x10000000000CUL});
		public static readonly BitSet _relationalOperator_in_relationalExpression4039 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _SkipSpace_in_relationalExpression4041 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _shiftExpression_in_relationalExpression4046 = new BitSet(new ulong[]{0x4101800000000002UL,0x10000000000CUL});
		public static readonly BitSet _shiftExpression_in_relationalExpressionNoIn4084 = new BitSet(new ulong[]{0x4001800000000002UL,0x10000000000CUL});
		public static readonly BitSet _SkipSpace_in_relationalExpressionNoIn4101 = new BitSet(new ulong[]{0x4001800000000000UL,0x10000000000CUL});
		public static readonly BitSet _relationalOperatorNoIn_in_relationalExpressionNoIn4106 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _SkipSpace_in_relationalExpressionNoIn4108 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _shiftExpression_in_relationalExpressionNoIn4113 = new BitSet(new ulong[]{0x4001800000000002UL,0x10000000000CUL});
		public static readonly BitSet _set_in_relationalOperator4146 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _set_in_relationalOperatorNoIn4183 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _additiveExpression_in_shiftExpression4221 = new BitSet(new ulong[]{0x2UL,0x80102800000000UL});
		public static readonly BitSet _SkipSpace_in_shiftExpression4238 = new BitSet(new ulong[]{0x0UL,0x80102800000000UL});
		public static readonly BitSet _shiftOperator_in_shiftExpression4243 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _SkipSpace_in_shiftExpression4245 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _additiveExpression_in_shiftExpression4250 = new BitSet(new ulong[]{0x2UL,0x80102800000000UL});
		public static readonly BitSet _set_in_shiftOperator4283 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _multiplicativeExpression_in_additiveExpression4313 = new BitSet(new ulong[]{0x2UL,0x100001000080UL});
		public static readonly BitSet _SkipSpace_in_additiveExpression4330 = new BitSet(new ulong[]{0x0UL,0x100001000080UL});
		public static readonly BitSet _additiveOperator_in_additiveExpression4335 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _SkipSpace_in_additiveExpression4337 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _multiplicativeExpression_in_additiveExpression4342 = new BitSet(new ulong[]{0x2UL,0x100001000080UL});
		public static readonly BitSet _set_in_additiveOperator4375 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _unaryExpression_in_multiplicativeExpression4401 = new BitSet(new ulong[]{0x8000002UL,0x100000001400UL});
		public static readonly BitSet _SkipSpace_in_multiplicativeExpression4418 = new BitSet(new ulong[]{0x8000000UL,0x100000001400UL});
		public static readonly BitSet _multiplicativeOperator_in_multiplicativeExpression4423 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _SkipSpace_in_multiplicativeExpression4425 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _unaryExpression_in_multiplicativeExpression4430 = new BitSet(new ulong[]{0x8000002UL,0x100000001400UL});
		public static readonly BitSet _set_in_multiplicativeOperator4463 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _postfixExpression_in_unaryExpression4490 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _prefixOperator_in_unaryExpression4500 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _SkipSpace_in_unaryExpression4502 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _unaryExpression_in_unaryExpression4505 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _set_in_prefixOperator4539 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _leftHandSideExpression_in_postfixExpression4594 = new BitSet(new ulong[]{0x0UL,0x4000200UL});
		public static readonly BitSet _PLUS_INC_in_postfixExpression4599 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _MINUS_INC_in_postfixExpression4605 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _leftHandSideExpression_in_postfixExpression4638 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _THIS_in_primaryExpression4655 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _Identifier_in_primaryExpression4663 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _importedNotation_in_primaryExpression4671 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _literal_in_primaryExpression4679 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _arrayLiteral_in_primaryExpression4687 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _objectLiteral_in_primaryExpression4695 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _130_in_primaryExpression4703 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _SkipSpace_in_primaryExpression4705 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _expression_in_primaryExpression4708 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x8UL});
		public static readonly BitSet _SkipSpace_in_primaryExpression4710 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x8UL});
		public static readonly BitSet _131_in_primaryExpression4713 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _135_in_arrayLiteral4743 = new BitSet(new ulong[]{0x8080020044281000UL,0x80107000054C8280UL,0x84184UL});
		public static readonly BitSet _SkipSpace_in_arrayLiteral4745 = new BitSet(new ulong[]{0x8080020044281000UL,0x80107000054C8280UL,0x84184UL});
		public static readonly BitSet _assignmentExpression_in_arrayLiteral4748 = new BitSet(new ulong[]{0x200000UL,0x100000000000UL,0x100UL});
		public static readonly BitSet _SkipSpace_in_arrayLiteral4752 = new BitSet(new ulong[]{0x200000UL,0x100000000000UL});
		public static readonly BitSet _COMMA_in_arrayLiteral4755 = new BitSet(new ulong[]{0x8080020044281000UL,0x80107000054C8280UL,0x84184UL});
		public static readonly BitSet _SkipSpace_in_arrayLiteral4758 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _assignmentExpression_in_arrayLiteral4761 = new BitSet(new ulong[]{0x200000UL,0x100000000000UL,0x100UL});
		public static readonly BitSet _SkipSpace_in_arrayLiteral4767 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x100UL});
		public static readonly BitSet _136_in_arrayLiteral4770 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _147_in_objectLiteral4805 = new BitSet(new ulong[]{0x8000000000000000UL,0x300000400000UL});
		public static readonly BitSet _SkipSpace_in_objectLiteral4807 = new BitSet(new ulong[]{0x8000000000000000UL,0x300000400000UL});
		public static readonly BitSet _propertyNameAndValue_in_objectLiteral4810 = new BitSet(new ulong[]{0x200000UL,0x100000000000UL,0x100000UL});
		public static readonly BitSet _SkipSpace_in_objectLiteral4813 = new BitSet(new ulong[]{0x200000UL,0x100000000000UL});
		public static readonly BitSet _COMMA_in_objectLiteral4816 = new BitSet(new ulong[]{0x8000000000000000UL,0x300000400000UL});
		public static readonly BitSet _SkipSpace_in_objectLiteral4818 = new BitSet(new ulong[]{0x8000000000000000UL,0x300000400000UL});
		public static readonly BitSet _propertyNameAndValue_in_objectLiteral4823 = new BitSet(new ulong[]{0x200000UL,0x100000000000UL,0x100000UL});
		public static readonly BitSet _SkipSpace_in_objectLiteral4827 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x100000UL});
		public static readonly BitSet _148_in_objectLiteral4830 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _147_in_objectLiteral4858 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x100000UL});
		public static readonly BitSet _SkipSpace_in_objectLiteral4860 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x100000UL});
		public static readonly BitSet _148_in_objectLiteral4863 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _propertyNameAndValue_in_propertyInitializers4886 = new BitSet(new ulong[]{0x200000UL,0x100000000000UL});
		public static readonly BitSet _SkipSpace_in_propertyInitializers4889 = new BitSet(new ulong[]{0x200000UL,0x100000000000UL});
		public static readonly BitSet _COMMA_in_propertyInitializers4892 = new BitSet(new ulong[]{0x8000000000000000UL,0x300000400000UL});
		public static readonly BitSet _propertyInitializers_in_propertyInitializers4894 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _propertyName_in_propertyNameAndValue4914 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x20UL});
		public static readonly BitSet _SkipSpace_in_propertyNameAndValue4916 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x20UL});
		public static readonly BitSet _133_in_propertyNameAndValue4919 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _SkipSpace_in_propertyNameAndValue4921 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _assignmentExpression_in_propertyNameAndValue4926 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _set_in_propertyName4953 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _set_in_literal4987 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _IMPORT_START_in_importedNotation5038 = new BitSet(new ulong[]{0x8002000000000000UL,0x0UL,0x80UL});
		public static readonly BitSet _importedTypeName_in_importedNotation5041 = new BitSet(new ulong[]{0x0UL,0x0UL,0x100000UL});
		public static readonly BitSet _148_in_importedNotation5043 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _IMPORT_START_in_importedNotation5052 = new BitSet(new ulong[]{0x8002000000000000UL,0x0UL,0x80UL});
		public static readonly BitSet _importedTypeName_in_importedNotation5056 = new BitSet(new ulong[]{0x0UL,0x20000000000000UL});
		public static readonly BitSet _TYPE_SEPERATOR_in_importedNotation5058 = new BitSet(new ulong[]{0x8000000000000000UL});
		public static readonly BitSet _Identifier_in_importedNotation5062 = new BitSet(new ulong[]{0x0UL,0x0UL,0x100000UL});
		public static readonly BitSet _148_in_importedNotation5064 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _IMPORT_START_in_importedNotation5092 = new BitSet(new ulong[]{0x8002000000000000UL,0x0UL,0x80UL});
		public static readonly BitSet _importedTypeName_in_importedNotation5096 = new BitSet(new ulong[]{0x0UL,0x20000000000000UL});
		public static readonly BitSet _TYPE_SEPERATOR_in_importedNotation5098 = new BitSet(new ulong[]{0x8000000000000000UL});
		public static readonly BitSet _Identifier_in_importedNotation5102 = new BitSet(new ulong[]{0x0UL,0x0UL,0x4UL});
		public static readonly BitSet _importedMethodArguments_in_importedNotation5106 = new BitSet(new ulong[]{0x0UL,0x0UL,0x100000UL});
		public static readonly BitSet _148_in_importedNotation5108 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _130_in_importedMethodArguments5148 = new BitSet(new ulong[]{0x8002000000000000UL,0x100000000000UL,0x88UL});
		public static readonly BitSet _SkipSpace_in_importedMethodArguments5151 = new BitSet(new ulong[]{0x8002000000000000UL,0x100000000000UL,0x80UL});
		public static readonly BitSet _importedTypeName_in_importedMethodArguments5154 = new BitSet(new ulong[]{0x200000UL,0x100000000000UL,0x8UL});
		public static readonly BitSet _SkipSpace_in_importedMethodArguments5157 = new BitSet(new ulong[]{0x200000UL,0x100000000000UL});
		public static readonly BitSet _COMMA_in_importedMethodArguments5160 = new BitSet(new ulong[]{0x8002000000000000UL,0x100000000000UL,0x80UL});
		public static readonly BitSet _SkipSpace_in_importedMethodArguments5162 = new BitSet(new ulong[]{0x8002000000000000UL,0x100000000000UL,0x80UL});
		public static readonly BitSet _importedTypeName_in_importedMethodArguments5165 = new BitSet(new ulong[]{0x200000UL,0x100000000000UL,0x8UL});
		public static readonly BitSet _SkipSpace_in_importedMethodArguments5171 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x8UL});
		public static readonly BitSet _131_in_importedMethodArguments5174 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _135_in_importedTypeName5208 = new BitSet(new ulong[]{0x8000000000000000UL});
		public static readonly BitSet _importedTypeNamePart_in_importedTypeName5212 = new BitSet(new ulong[]{0x0UL,0x0UL,0x100UL});
		public static readonly BitSet _136_in_importedTypeName5214 = new BitSet(new ulong[]{0x8000000000000000UL});
		public static readonly BitSet _importedTypeNamePart_in_importedTypeName5218 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _Identifier_in_importedTypeName5238 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _GenericSignature_in_importedTypeName5254 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _Identifier_in_importedAssemblyNamePart5279 = new BitSet(new ulong[]{0x2UL,0x0UL,0x10UL});
		public static readonly BitSet _132_in_importedAssemblyNamePart5282 = new BitSet(new ulong[]{0x8000000000000000UL});
		public static readonly BitSet _Identifier_in_importedAssemblyNamePart5284 = new BitSet(new ulong[]{0x2UL,0x0UL,0x10UL});
		public static readonly BitSet _Identifier_in_importedTypeNamePart5320 = new BitSet(new ulong[]{0x2UL,0x0UL,0x10UL});
		public static readonly BitSet _132_in_importedTypeNamePart5323 = new BitSet(new ulong[]{0x8000000000000000UL});
		public static readonly BitSet _Identifier_in_importedTypeNamePart5325 = new BitSet(new ulong[]{0x2UL,0x0UL,0x10UL});
		public static readonly BitSet _132_in_importedTypeNamePart5330 = new BitSet(new ulong[]{0x0UL,0x40000000000000UL});
		public static readonly BitSet _TypeNameIdentifier_in_importedTypeNamePart5332 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _functionDeclaration_in_synpred5_JavaScript421 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _SkipSpace_in_synpred9_JavaScript510 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _statementBlock_in_synpred21_JavaScript678 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _expressionStatement_in_synpred24_JavaScript702 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _labelledStatement_in_synpred31_JavaScript758 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _SkipSpace_in_synpred34_JavaScript801 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _SkipSpace_in_synpred60_JavaScript1200 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x1000UL});
		public static readonly BitSet _140_in_synpred60_JavaScript1203 = new BitSet(new ulong[]{0x80800200448C1000UL,0xC012F004854C8280UL,0xFE884UL});
		public static readonly BitSet _SkipSpace_in_synpred60_JavaScript1205 = new BitSet(new ulong[]{0x80800200448C1000UL,0xC012F004854C8280UL,0xFE884UL});
		public static readonly BitSet _statement_in_synpred60_JavaScript1210 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _forStatement_in_synpred63_JavaScript1269 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _SkipSpace_in_synpred73_JavaScript1427 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _SkipSpace_in_synpred76_JavaScript1444 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _SkipSpace_in_synpred79_JavaScript1461 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _SkipSpace_in_synpred120_JavaScript1967 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _SkipSpace_in_synpred123_JavaScript1997 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _conditionalExpression_in_synpred143_JavaScript2257 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _conditionalExpressionNoIn_in_synpred146_JavaScript2321 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _callExpression_in_synpred160_JavaScript2490 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _memberExpression_in_synpred161_JavaScript2515 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _logicalORExpression_in_synpred190_JavaScript3035 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x40UL});
		public static readonly BitSet _SkipSpace_in_synpred190_JavaScript3037 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x40UL});
		public static readonly BitSet _134_in_synpred190_JavaScript3040 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _SkipSpace_in_synpred190_JavaScript3042 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _assignmentExpression_in_synpred190_JavaScript3047 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x20UL});
		public static readonly BitSet _SkipSpace_in_synpred190_JavaScript3049 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x20UL});
		public static readonly BitSet _133_in_synpred190_JavaScript3052 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _SkipSpace_in_synpred190_JavaScript3054 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _assignmentExpression_in_synpred190_JavaScript3059 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _logicalORExpressionNoIn_in_synpred195_JavaScript3109 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x40UL});
		public static readonly BitSet _SkipSpace_in_synpred195_JavaScript3111 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x40UL});
		public static readonly BitSet _134_in_synpred195_JavaScript3114 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _SkipSpace_in_synpred195_JavaScript3116 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _assignmentExpressionNoIn_in_synpred195_JavaScript3121 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x20UL});
		public static readonly BitSet _SkipSpace_in_synpred195_JavaScript3123 = new BitSet(new ulong[]{0x0UL,0x100000000000UL,0x20UL});
		public static readonly BitSet _133_in_synpred195_JavaScript3126 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _SkipSpace_in_synpred195_JavaScript3128 = new BitSet(new ulong[]{0x8080020044081000UL,0x80107000054C8280UL,0x84084UL});
		public static readonly BitSet _assignmentExpressionNoIn_in_synpred195_JavaScript3133 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _leftHandSideExpression_in_synpred276_JavaScript4594 = new BitSet(new ulong[]{0x0UL,0x4000200UL});
		public static readonly BitSet _PLUS_INC_in_synpred276_JavaScript4599 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _MINUS_INC_in_synpred276_JavaScript4605 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _SkipSpace_in_synpred285_JavaScript4745 = new BitSet(new ulong[]{0x2UL});

	}
	#endregion Follow sets
}

} // namespace NScript.JSParser

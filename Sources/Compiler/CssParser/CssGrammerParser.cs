// $ANTLR 3.3.0.7239 CssGrammer.g3 2025-11-20 09:22:13

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

namespace CssParser
{
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "3.3.0.7239")]
[System.CLSCompliant(false)]
public partial class CssGrammerParser : Antlr.Runtime.Parser
{
	internal static readonly string[] tokenNames = new string[] {
		"<invalid>", "<EOR>", "<DOWN>", "<UP>", "A", "ALL", "ANGLE", "ASSIGN_EXPR", "ATTRIB", "ATTRIB_CONTAINS", "ATTRIB_CONTAINS_WORD", "ATTRIB_ENDS_WITH", "ATTRIB_EQUALS", "ATTRIB_STARTS_WITH", "ATTRIB_STARTS_WITH_WORD", "ATTRIB_VALUE", "B", "BEGINSWITH", "C", "CALC", "CDC", "CDO", "CHARSET_SYM", "CLASS", "COLON", "COLOR", "COMMA", "COMMENT", "D", "DIMENSION", "DOT", "DOUBLE_COLON", "DPSEUDO", "DPSEUDO_FUNC", "E", "EMS", "ENDS_WITH", "ESCAPE", "EXPR", "EXPRS", "EXS", "F", "FOLLOWS", "FONT_FACE", "FREQ", "FROM_SYM", "FUNCTION", "G", "GREATER", "H", "HASH", "HASVALUE", "HEXCHAR", "I", "ID", "IDENT", "IDENTIFIER", "IMPORT", "IMPORTANT_SYM", "IMPORT_SYM", "INCLUDES", "INCLUDES_WORD", "INVALID", "J", "K", "KEYFRAME", "KEYFRAMES", "KEYFRAMESELECTORS", "KEYFRAMES_SYM", "L", "LBRACE", "LBRACKET", "LENGTH", "LPAREN", "M", "MEDIA", "MEDIA_FEATURE", "MEDIA_QUERY", "MEDIA_SYM", "MEDIA_TYPE", "MINUS", "MULTIPLIER", "N", "NAME", "NEST", "NESTED", "NL", "NMCHAR", "NMSTART", "NONASCII", "NOT_SYM", "NUMARG", "NUMBER", "O", "OPEQ", "P", "PAGE_SYM", "PARENTOF", "PERCENTAGE", "PLUS", "PRECEDEDS", "PROPERTY", "PSEUDO", "PSEUDO_FUNC", "PSEUDO_FUNC_SELECTOR", "Q", "R", "RBRACE", "RBRACKET", "REM", "RGBA", "RPAREN", "RULE", "S", "SELECTOR", "SELECTORS", "SELECTOR_OP", "SEL_OP", "SEMI", "SIMPLE_SEL", "SOLIDUS", "STAR", "STARTS_WITH", "STARTS_WITH_WORD", "STRING", "STRING_VAL", "T", "TAG", "TIME", "TO_SYM", "U", "UNDER", "UNICODE", "UNITEXPRS", "UNIT_VAL", "URI", "URL", "URL_VAL", "V", "W", "WS", "X", "Y", "Z", "'<'", "'<='", "'>='", "'and'", "'calc'", "'color-stop'", "'rgb'", "'rgba'", "'~'"
	};
	public const int EOF=-1;
	public const int A=4;
	public const int ALL=5;
	public const int ANGLE=6;
	public const int ASSIGN_EXPR=7;
	public const int ATTRIB=8;
	public const int ATTRIB_CONTAINS=9;
	public const int ATTRIB_CONTAINS_WORD=10;
	public const int ATTRIB_ENDS_WITH=11;
	public const int ATTRIB_EQUALS=12;
	public const int ATTRIB_STARTS_WITH=13;
	public const int ATTRIB_STARTS_WITH_WORD=14;
	public const int ATTRIB_VALUE=15;
	public const int B=16;
	public const int BEGINSWITH=17;
	public const int C=18;
	public const int CALC=19;
	public const int CDC=20;
	public const int CDO=21;
	public const int CHARSET_SYM=22;
	public const int CLASS=23;
	public const int COLON=24;
	public const int COLOR=25;
	public const int COMMA=26;
	public const int COMMENT=27;
	public const int D=28;
	public const int DIMENSION=29;
	public const int DOT=30;
	public const int DOUBLE_COLON=31;
	public const int DPSEUDO=32;
	public const int DPSEUDO_FUNC=33;
	public const int E=34;
	public const int EMS=35;
	public const int ENDS_WITH=36;
	public const int ESCAPE=37;
	public const int EXPR=38;
	public const int EXPRS=39;
	public const int EXS=40;
	public const int F=41;
	public const int FOLLOWS=42;
	public const int FONT_FACE=43;
	public const int FREQ=44;
	public const int FROM_SYM=45;
	public const int FUNCTION=46;
	public const int G=47;
	public const int GREATER=48;
	public const int H=49;
	public const int HASH=50;
	public const int HASVALUE=51;
	public const int HEXCHAR=52;
	public const int I=53;
	public const int ID=54;
	public const int IDENT=55;
	public const int IDENTIFIER=56;
	public const int IMPORT=57;
	public const int IMPORTANT_SYM=58;
	public const int IMPORT_SYM=59;
	public const int INCLUDES=60;
	public const int INCLUDES_WORD=61;
	public const int INVALID=62;
	public const int J=63;
	public const int K=64;
	public const int KEYFRAME=65;
	public const int KEYFRAMES=66;
	public const int KEYFRAMESELECTORS=67;
	public const int KEYFRAMES_SYM=68;
	public const int L=69;
	public const int LBRACE=70;
	public const int LBRACKET=71;
	public const int LENGTH=72;
	public const int LPAREN=73;
	public const int M=74;
	public const int MEDIA=75;
	public const int MEDIA_FEATURE=76;
	public const int MEDIA_QUERY=77;
	public const int MEDIA_SYM=78;
	public const int MEDIA_TYPE=79;
	public const int MINUS=80;
	public const int MULTIPLIER=81;
	public const int N=82;
	public const int NAME=83;
	public const int NEST=84;
	public const int NESTED=85;
	public const int NL=86;
	public const int NMCHAR=87;
	public const int NMSTART=88;
	public const int NONASCII=89;
	public const int NOT_SYM=90;
	public const int NUMARG=91;
	public const int NUMBER=92;
	public const int O=93;
	public const int OPEQ=94;
	public const int P=95;
	public const int PAGE_SYM=96;
	public const int PARENTOF=97;
	public const int PERCENTAGE=98;
	public const int PLUS=99;
	public const int PRECEDEDS=100;
	public const int PROPERTY=101;
	public const int PSEUDO=102;
	public const int PSEUDO_FUNC=103;
	public const int PSEUDO_FUNC_SELECTOR=104;
	public const int Q=105;
	public const int R=106;
	public const int RBRACE=107;
	public const int RBRACKET=108;
	public const int REM=109;
	public const int RGBA=110;
	public const int RPAREN=111;
	public const int RULE=112;
	public const int S=113;
	public const int SELECTOR=114;
	public const int SELECTORS=115;
	public const int SELECTOR_OP=116;
	public const int SEL_OP=117;
	public const int SEMI=118;
	public const int SIMPLE_SEL=119;
	public const int SOLIDUS=120;
	public const int STAR=121;
	public const int STARTS_WITH=122;
	public const int STARTS_WITH_WORD=123;
	public const int STRING=124;
	public const int STRING_VAL=125;
	public const int T=126;
	public const int TAG=127;
	public const int TIME=128;
	public const int TO_SYM=129;
	public const int U=130;
	public const int UNDER=131;
	public const int UNICODE=132;
	public const int UNITEXPRS=133;
	public const int UNIT_VAL=134;
	public const int URI=135;
	public const int URL=136;
	public const int URL_VAL=137;
	public const int V=138;
	public const int W=139;
	public const int WS=140;
	public const int X=141;
	public const int Y=142;
	public const int Z=143;
	public const int T__144=144;
	public const int T__145=145;
	public const int T__146=146;
	public const int T__147=147;
	public const int T__148=148;
	public const int T__149=149;
	public const int T__150=150;
	public const int T__151=151;
	public const int T__152=152;

	// delegates
	// delegators

	#if ANTLR_DEBUG
		private static readonly bool[] decisionCanBacktrack =
			new bool[]
			{
				false, // invalid decision
				false, false, false, false, false, false, false, false, false, false, 
				false, true, true, false, false, false, false, false, false, false, 
				false, false, false, false, false, false, false, false, true, true, 
				false, false, false, false, false, false, false, true, false, false, 
				false, false, false, false, false, true, false, false, false, false, 
				false, false, false, false, false, false, false, false, false, false, 
				false, false, false
			};
	#else
		private static readonly bool[] decisionCanBacktrack = new bool[0];
	#endif
	public CssGrammerParser( ITokenStream input )
		: this( input, new RecognizerSharedState() )
	{
	}
	public CssGrammerParser(ITokenStream input, RecognizerSharedState state)
		: base(input, state)
	{
		this.state.ruleMemo = new System.Collections.Generic.Dictionary<int, int>[160+1];

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

	public override string[] TokenNames { get { return CssGrammerParser.tokenNames; } }
	public override string GrammarFileName { get { return "CssGrammer.g3"; } }


	partial void OnCreated();
	partial void EnterRule(string ruleName, int ruleIndex);
	partial void LeaveRule(string ruleName, int ruleIndex);

	#region Rules
	public class styleSheet_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_styleSheet();
	partial void Leave_styleSheet();

	// $ANTLR start "styleSheet"
	// CssGrammer.g3:102:1: public styleSheet : charSet ( imports )* bodylist EOF ;
	[GrammarRule("styleSheet")]
	public CssGrammerParser.styleSheet_return styleSheet()
	{
		Enter_styleSheet();
		EnterRule("styleSheet", 1);
		TraceIn("styleSheet", 1);
		CssGrammerParser.styleSheet_return retval = new CssGrammerParser.styleSheet_return();
		retval.Start = (CommonToken)input.LT(1);
		int styleSheet_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken EOF4=null;
		CssGrammerParser.charSet_return charSet1 = default(CssGrammerParser.charSet_return);
		CssGrammerParser.imports_return imports2 = default(CssGrammerParser.imports_return);
		CssGrammerParser.bodylist_return bodylist3 = default(CssGrammerParser.bodylist_return);

		CommonTree EOF4_tree=null;

		try { DebugEnterRule(GrammarFileName, "styleSheet");
		DebugLocation(102, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 1)) { return retval; }
			// CssGrammer.g3:103:5: ( charSet ( imports )* bodylist EOF )
			DebugEnterAlt(1);
			// CssGrammer.g3:103:9: charSet ( imports )* bodylist EOF
			{
			root_0 = (CommonTree)adaptor.Nil();

			DebugLocation(103, 9);
			PushFollow(Follow._charSet_in_styleSheet508);
			charSet1=charSet();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) adaptor.AddChild(root_0, charSet1.Tree);
			DebugLocation(104, 9);
			// CssGrammer.g3:104:9: ( imports )*
			try { DebugEnterSubRule(1);
			while (true)
			{
				int alt1=2;
				try { DebugEnterDecision(1, decisionCanBacktrack[1]);
				int LA1_0 = input.LA(1);

				if ((LA1_0==IMPORT_SYM))
				{
					alt1=1;
				}


				} finally { DebugExitDecision(1); }
				switch ( alt1 )
				{
				case 1:
					DebugEnterAlt(1);
					// CssGrammer.g3:104:9: imports
					{
					DebugLocation(104, 9);
					PushFollow(Follow._imports_in_styleSheet518);
					imports2=imports();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) adaptor.AddChild(root_0, imports2.Tree);

					}
					break;

				default:
					goto loop1;
				}
			}

			loop1:
				;

			} finally { DebugExitSubRule(1); }

			DebugLocation(105, 9);
			PushFollow(Follow._bodylist_in_styleSheet529);
			bodylist3=bodylist();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) adaptor.AddChild(root_0, bodylist3.Tree);
			DebugLocation(106, 6);
			EOF4=(CommonToken)Match(input,EOF,Follow._EOF_in_styleSheet536); if (state.failed) return retval;
			if ( state.backtracking==0 ) {
			EOF4_tree = (CommonTree)adaptor.Create(EOF4);
			adaptor.AddChild(root_0, EOF4_tree);
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
			TraceOut("styleSheet", 1);
			LeaveRule("styleSheet", 1);
			Leave_styleSheet();
			if (state.backtracking > 0) { Memoize(input, 1, styleSheet_StartIndex); }
		}
		DebugLocation(107, 4);
		} finally { DebugExitRule(GrammarFileName, "styleSheet"); }
		return retval;

	}
	// $ANTLR end "styleSheet"

	public class charSet_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_charSet();
	partial void Leave_charSet();

	// $ANTLR start "charSet"
	// CssGrammer.g3:112:1: charSet : ( CHARSET_SYM STRING SEMI |);
	[GrammarRule("charSet")]
	private CssGrammerParser.charSet_return charSet()
	{
		Enter_charSet();
		EnterRule("charSet", 2);
		TraceIn("charSet", 2);
		CssGrammerParser.charSet_return retval = new CssGrammerParser.charSet_return();
		retval.Start = (CommonToken)input.LT(1);
		int charSet_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken CHARSET_SYM5=null;
		CommonToken STRING6=null;
		CommonToken SEMI7=null;

		CommonTree CHARSET_SYM5_tree=null;
		CommonTree STRING6_tree=null;
		CommonTree SEMI7_tree=null;

		try { DebugEnterRule(GrammarFileName, "charSet");
		DebugLocation(112, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 2)) { return retval; }
			// CssGrammer.g3:113:5: ( CHARSET_SYM STRING SEMI |)
			int alt2=2;
			try { DebugEnterDecision(2, decisionCanBacktrack[2]);
			int LA2_0 = input.LA(1);

			if ((LA2_0==CHARSET_SYM))
			{
				alt2=1;
			}
			else if ((LA2_0==EOF||LA2_0==COLON||(LA2_0>=DOT && LA2_0<=DOUBLE_COLON)||LA2_0==FONT_FACE||LA2_0==HASH||LA2_0==IDENT||LA2_0==IMPORT_SYM||LA2_0==KEYFRAMES_SYM||LA2_0==LBRACKET||LA2_0==MEDIA_SYM||LA2_0==PAGE_SYM||LA2_0==STAR))
			{
				alt2=2;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return retval;}
				NoViableAltException nvae = new NoViableAltException("", 2, 0, input);

				DebugRecognitionException(nvae);
				throw nvae;
			}
			} finally { DebugExitDecision(2); }
			switch (alt2)
			{
			case 1:
				DebugEnterAlt(1);
				// CssGrammer.g3:113:9: CHARSET_SYM STRING SEMI
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(113, 9);
				CHARSET_SYM5=(CommonToken)Match(input,CHARSET_SYM,Follow._CHARSET_SYM_in_charSet562); if (state.failed) return retval;
				if ( state.backtracking==0 ) {
				CHARSET_SYM5_tree = (CommonTree)adaptor.Create(CHARSET_SYM5);
				adaptor.AddChild(root_0, CHARSET_SYM5_tree);
				}
				DebugLocation(113, 21);
				STRING6=(CommonToken)Match(input,STRING,Follow._STRING_in_charSet564); if (state.failed) return retval;
				if ( state.backtracking==0 ) {
				STRING6_tree = (CommonTree)adaptor.Create(STRING6);
				adaptor.AddChild(root_0, STRING6_tree);
				}
				DebugLocation(113, 28);
				SEMI7=(CommonToken)Match(input,SEMI,Follow._SEMI_in_charSet566); if (state.failed) return retval;
				if ( state.backtracking==0 ) {
				SEMI7_tree = (CommonTree)adaptor.Create(SEMI7);
				adaptor.AddChild(root_0, SEMI7_tree);
				}

				}
				break;
			case 2:
				DebugEnterAlt(2);
				// CssGrammer.g3:115:5: 
				{
				root_0 = (CommonTree)adaptor.Nil();

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
			TraceOut("charSet", 2);
			LeaveRule("charSet", 2);
			Leave_charSet();
			if (state.backtracking > 0) { Memoize(input, 2, charSet_StartIndex); }
		}
		DebugLocation(115, 4);
		} finally { DebugExitRule(GrammarFileName, "charSet"); }
		return retval;

	}
	// $ANTLR end "charSet"

	public class imports_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_imports();
	partial void Leave_imports();

	// $ANTLR start "imports"
	// CssGrammer.g3:120:1: imports : IMPORT_SYM ( STRING | URI ) ( medium ( COMMA medium )* )? SEMI ;
	[GrammarRule("imports")]
	private CssGrammerParser.imports_return imports()
	{
		Enter_imports();
		EnterRule("imports", 3);
		TraceIn("imports", 3);
		CssGrammerParser.imports_return retval = new CssGrammerParser.imports_return();
		retval.Start = (CommonToken)input.LT(1);
		int imports_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken IMPORT_SYM8=null;
		CommonToken set9=null;
		CommonToken COMMA11=null;
		CommonToken SEMI13=null;
		CssGrammerParser.medium_return medium10 = default(CssGrammerParser.medium_return);
		CssGrammerParser.medium_return medium12 = default(CssGrammerParser.medium_return);

		CommonTree IMPORT_SYM8_tree=null;
		CommonTree set9_tree=null;
		CommonTree COMMA11_tree=null;
		CommonTree SEMI13_tree=null;

		try { DebugEnterRule(GrammarFileName, "imports");
		DebugLocation(120, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 3)) { return retval; }
			// CssGrammer.g3:121:5: ( IMPORT_SYM ( STRING | URI ) ( medium ( COMMA medium )* )? SEMI )
			DebugEnterAlt(1);
			// CssGrammer.g3:121:9: IMPORT_SYM ( STRING | URI ) ( medium ( COMMA medium )* )? SEMI
			{
			root_0 = (CommonTree)adaptor.Nil();

			DebugLocation(121, 9);
			IMPORT_SYM8=(CommonToken)Match(input,IMPORT_SYM,Follow._IMPORT_SYM_in_imports594); if (state.failed) return retval;
			if ( state.backtracking==0 ) {
			IMPORT_SYM8_tree = (CommonTree)adaptor.Create(IMPORT_SYM8);
			adaptor.AddChild(root_0, IMPORT_SYM8_tree);
			}
			DebugLocation(121, 20);
			set9=(CommonToken)input.LT(1);
			if (input.LA(1)==STRING||input.LA(1)==URI)
			{
				input.Consume();
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, (CommonTree)adaptor.Create(set9));
				state.errorRecovery=false;state.failed=false;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return retval;}
				MismatchedSetException mse = new MismatchedSetException(null,input);
				DebugRecognitionException(mse);
				throw mse;
			}

			DebugLocation(121, 33);
			// CssGrammer.g3:121:33: ( medium ( COMMA medium )* )?
			int alt4=2;
			try { DebugEnterSubRule(4);
			try { DebugEnterDecision(4, decisionCanBacktrack[4]);
			int LA4_0 = input.LA(1);

			if ((LA4_0==IDENT))
			{
				alt4=1;
			}
			} finally { DebugExitDecision(4); }
			switch (alt4)
			{
			case 1:
				DebugEnterAlt(1);
				// CssGrammer.g3:121:34: medium ( COMMA medium )*
				{
				DebugLocation(121, 34);
				PushFollow(Follow._medium_in_imports603);
				medium10=medium();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, medium10.Tree);
				DebugLocation(121, 41);
				// CssGrammer.g3:121:41: ( COMMA medium )*
				try { DebugEnterSubRule(3);
				while (true)
				{
					int alt3=2;
					try { DebugEnterDecision(3, decisionCanBacktrack[3]);
					int LA3_0 = input.LA(1);

					if ((LA3_0==COMMA))
					{
						alt3=1;
					}


					} finally { DebugExitDecision(3); }
					switch ( alt3 )
					{
					case 1:
						DebugEnterAlt(1);
						// CssGrammer.g3:121:42: COMMA medium
						{
						DebugLocation(121, 42);
						COMMA11=(CommonToken)Match(input,COMMA,Follow._COMMA_in_imports606); if (state.failed) return retval;
						if ( state.backtracking==0 ) {
						COMMA11_tree = (CommonTree)adaptor.Create(COMMA11);
						adaptor.AddChild(root_0, COMMA11_tree);
						}
						DebugLocation(121, 48);
						PushFollow(Follow._medium_in_imports608);
						medium12=medium();
						PopFollow();
						if (state.failed) return retval;
						if ( state.backtracking == 0 ) adaptor.AddChild(root_0, medium12.Tree);

						}
						break;

					default:
						goto loop3;
					}
				}

				loop3:
					;

				} finally { DebugExitSubRule(3); }


				}
				break;

			}
			} finally { DebugExitSubRule(4); }

			DebugLocation(121, 59);
			SEMI13=(CommonToken)Match(input,SEMI,Follow._SEMI_in_imports614); if (state.failed) return retval;
			if ( state.backtracking==0 ) {
			SEMI13_tree = (CommonTree)adaptor.Create(SEMI13);
			adaptor.AddChild(root_0, SEMI13_tree);
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
			TraceOut("imports", 3);
			LeaveRule("imports", 3);
			Leave_imports();
			if (state.backtracking > 0) { Memoize(input, 3, imports_StartIndex); }
		}
		DebugLocation(122, 4);
		} finally { DebugExitRule(GrammarFileName, "imports"); }
		return retval;

	}
	// $ANTLR end "imports"

	public class media_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_media();
	partial void Leave_media();

	// $ANTLR start "media"
	// CssGrammer.g3:128:1: media : MEDIA_SYM mediaQuery ( COMMA mediaQuery )* LBRACE ( ruleSet )+ RBRACE -> ^( MEDIA ( mediaQuery )* ( ruleSet )+ ) ;
	[GrammarRule("media")]
	private CssGrammerParser.media_return media()
	{
		Enter_media();
		EnterRule("media", 4);
		TraceIn("media", 4);
		CssGrammerParser.media_return retval = new CssGrammerParser.media_return();
		retval.Start = (CommonToken)input.LT(1);
		int media_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken MEDIA_SYM14=null;
		CommonToken COMMA16=null;
		CommonToken LBRACE18=null;
		CommonToken RBRACE20=null;
		CssGrammerParser.mediaQuery_return mediaQuery15 = default(CssGrammerParser.mediaQuery_return);
		CssGrammerParser.mediaQuery_return mediaQuery17 = default(CssGrammerParser.mediaQuery_return);
		CssGrammerParser.ruleSet_return ruleSet19 = default(CssGrammerParser.ruleSet_return);

		CommonTree MEDIA_SYM14_tree=null;
		CommonTree COMMA16_tree=null;
		CommonTree LBRACE18_tree=null;
		CommonTree RBRACE20_tree=null;
		RewriteRuleITokenStream stream_MEDIA_SYM=new RewriteRuleITokenStream(adaptor,"token MEDIA_SYM");
		RewriteRuleITokenStream stream_COMMA=new RewriteRuleITokenStream(adaptor,"token COMMA");
		RewriteRuleITokenStream stream_LBRACE=new RewriteRuleITokenStream(adaptor,"token LBRACE");
		RewriteRuleITokenStream stream_RBRACE=new RewriteRuleITokenStream(adaptor,"token RBRACE");
		RewriteRuleSubtreeStream stream_mediaQuery=new RewriteRuleSubtreeStream(adaptor,"rule mediaQuery");
		RewriteRuleSubtreeStream stream_ruleSet=new RewriteRuleSubtreeStream(adaptor,"rule ruleSet");
		try { DebugEnterRule(GrammarFileName, "media");
		DebugLocation(128, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 4)) { return retval; }
			// CssGrammer.g3:129:5: ( MEDIA_SYM mediaQuery ( COMMA mediaQuery )* LBRACE ( ruleSet )+ RBRACE -> ^( MEDIA ( mediaQuery )* ( ruleSet )+ ) )
			DebugEnterAlt(1);
			// CssGrammer.g3:129:7: MEDIA_SYM mediaQuery ( COMMA mediaQuery )* LBRACE ( ruleSet )+ RBRACE
			{
			DebugLocation(129, 7);
			MEDIA_SYM14=(CommonToken)Match(input,MEDIA_SYM,Follow._MEDIA_SYM_in_media635); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_MEDIA_SYM.Add(MEDIA_SYM14);

			DebugLocation(129, 17);
			PushFollow(Follow._mediaQuery_in_media637);
			mediaQuery15=mediaQuery();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_mediaQuery.Add(mediaQuery15.Tree);
			DebugLocation(129, 28);
			// CssGrammer.g3:129:28: ( COMMA mediaQuery )*
			try { DebugEnterSubRule(5);
			while (true)
			{
				int alt5=2;
				try { DebugEnterDecision(5, decisionCanBacktrack[5]);
				int LA5_0 = input.LA(1);

				if ((LA5_0==COMMA))
				{
					alt5=1;
				}


				} finally { DebugExitDecision(5); }
				switch ( alt5 )
				{
				case 1:
					DebugEnterAlt(1);
					// CssGrammer.g3:129:29: COMMA mediaQuery
					{
					DebugLocation(129, 29);
					COMMA16=(CommonToken)Match(input,COMMA,Follow._COMMA_in_media640); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_COMMA.Add(COMMA16);

					DebugLocation(129, 35);
					PushFollow(Follow._mediaQuery_in_media642);
					mediaQuery17=mediaQuery();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_mediaQuery.Add(mediaQuery17.Tree);

					}
					break;

				default:
					goto loop5;
				}
			}

			loop5:
				;

			} finally { DebugExitSubRule(5); }

			DebugLocation(130, 9);
			LBRACE18=(CommonToken)Match(input,LBRACE,Follow._LBRACE_in_media654); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_LBRACE.Add(LBRACE18);

			DebugLocation(131, 13);
			// CssGrammer.g3:131:13: ( ruleSet )+
			int cnt6=0;
			try { DebugEnterSubRule(6);
			while (true)
			{
				int alt6=2;
				try { DebugEnterDecision(6, decisionCanBacktrack[6]);
				int LA6_0 = input.LA(1);

				if ((LA6_0==COLON||(LA6_0>=DOT && LA6_0<=DOUBLE_COLON)||LA6_0==FONT_FACE||LA6_0==HASH||LA6_0==IDENT||LA6_0==LBRACKET||LA6_0==STAR))
				{
					alt6=1;
				}


				} finally { DebugExitDecision(6); }
				switch (alt6)
				{
				case 1:
					DebugEnterAlt(1);
					// CssGrammer.g3:131:13: ruleSet
					{
					DebugLocation(131, 13);
					PushFollow(Follow._ruleSet_in_media668);
					ruleSet19=ruleSet();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_ruleSet.Add(ruleSet19.Tree);

					}
					break;

				default:
					if (cnt6 >= 1)
						goto loop6;

					if (state.backtracking>0) {state.failed=true; return retval;}
					EarlyExitException eee6 = new EarlyExitException( 6, input );
					DebugRecognitionException(eee6);
					throw eee6;
				}
				cnt6++;
			}
			loop6:
				;

			} finally { DebugExitSubRule(6); }

			DebugLocation(132, 9);
			RBRACE20=(CommonToken)Match(input,RBRACE,Follow._RBRACE_in_media679); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_RBRACE.Add(RBRACE20);



			{
			// AST REWRITE
			// elements: mediaQuery, ruleSet
			// token labels: 
			// rule labels: retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 133:9: -> ^( MEDIA ( mediaQuery )* ( ruleSet )+ )
			{
				DebugLocation(133, 12);
				// CssGrammer.g3:133:12: ^( MEDIA ( mediaQuery )* ( ruleSet )+ )
				{
				CommonTree root_1 = (CommonTree)adaptor.Nil();
				DebugLocation(133, 14);
				root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(MEDIA, "MEDIA"), root_1);

				DebugLocation(133, 20);
				// CssGrammer.g3:133:20: ( mediaQuery )*
				while ( stream_mediaQuery.HasNext )
				{
					DebugLocation(133, 20);
					adaptor.AddChild(root_1, stream_mediaQuery.NextTree());

				}
				stream_mediaQuery.Reset();
				DebugLocation(133, 32);
				if ( !(stream_ruleSet.HasNext) )
				{
					throw new RewriteEarlyExitException();
				}
				while ( stream_ruleSet.HasNext )
				{
					DebugLocation(133, 32);
					adaptor.AddChild(root_1, stream_ruleSet.NextTree());

				}
				stream_ruleSet.Reset();

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
			TraceOut("media", 4);
			LeaveRule("media", 4);
			Leave_media();
			if (state.backtracking > 0) { Memoize(input, 4, media_StartIndex); }
		}
		DebugLocation(134, 4);
		} finally { DebugExitRule(GrammarFileName, "media"); }
		return retval;

	}
	// $ANTLR end "media"

	public class mediaQuery_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_mediaQuery();
	partial void Leave_mediaQuery();

	// $ANTLR start "mediaQuery"
	// CssGrammer.g3:135:1: mediaQuery : ( (op1= NOT_SYM )? mediaFeature ( 'and' mediaFeature )* -> ^( MEDIA_QUERY ( $op1)? ( mediaFeature )* ) | (op1= NOT_SYM )? medium ( 'and' mediaFeature )* -> ^( MEDIA_QUERY ( $op1)? medium ( mediaFeature )* ) );
	[GrammarRule("mediaQuery")]
	private CssGrammerParser.mediaQuery_return mediaQuery()
	{
		Enter_mediaQuery();
		EnterRule("mediaQuery", 5);
		TraceIn("mediaQuery", 5);
		CssGrammerParser.mediaQuery_return retval = new CssGrammerParser.mediaQuery_return();
		retval.Start = (CommonToken)input.LT(1);
		int mediaQuery_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken op1=null;
		CommonToken string_literal22=null;
		CommonToken string_literal25=null;
		CssGrammerParser.mediaFeature_return mediaFeature21 = default(CssGrammerParser.mediaFeature_return);
		CssGrammerParser.mediaFeature_return mediaFeature23 = default(CssGrammerParser.mediaFeature_return);
		CssGrammerParser.medium_return medium24 = default(CssGrammerParser.medium_return);
		CssGrammerParser.mediaFeature_return mediaFeature26 = default(CssGrammerParser.mediaFeature_return);

		CommonTree op1_tree=null;
		CommonTree string_literal22_tree=null;
		CommonTree string_literal25_tree=null;
		RewriteRuleITokenStream stream_NOT_SYM=new RewriteRuleITokenStream(adaptor,"token NOT_SYM");
		RewriteRuleITokenStream stream_147=new RewriteRuleITokenStream(adaptor,"token 147");
		RewriteRuleSubtreeStream stream_mediaFeature=new RewriteRuleSubtreeStream(adaptor,"rule mediaFeature");
		RewriteRuleSubtreeStream stream_medium=new RewriteRuleSubtreeStream(adaptor,"rule medium");
		try { DebugEnterRule(GrammarFileName, "mediaQuery");
		DebugLocation(135, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 5)) { return retval; }
			// CssGrammer.g3:136:5: ( (op1= NOT_SYM )? mediaFeature ( 'and' mediaFeature )* -> ^( MEDIA_QUERY ( $op1)? ( mediaFeature )* ) | (op1= NOT_SYM )? medium ( 'and' mediaFeature )* -> ^( MEDIA_QUERY ( $op1)? medium ( mediaFeature )* ) )
			int alt11=2;
			try { DebugEnterDecision(11, decisionCanBacktrack[11]);
			switch (input.LA(1))
			{
			case NOT_SYM:
				{
				int LA11_1 = input.LA(2);

				if ((LA11_1==LPAREN))
				{
					alt11=1;
				}
				else if ((LA11_1==IDENT))
				{
					alt11=2;
				}
				else
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 11, 1, input);

					DebugRecognitionException(nvae);
					throw nvae;
				}
				}
				break;
			case LPAREN:
				{
				alt11=1;
				}
				break;
			case IDENT:
				{
				alt11=2;
				}
				break;
			default:
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 11, 0, input);

					DebugRecognitionException(nvae);
					throw nvae;
				}
			}

			} finally { DebugExitDecision(11); }
			switch (alt11)
			{
			case 1:
				DebugEnterAlt(1);
				// CssGrammer.g3:136:7: (op1= NOT_SYM )? mediaFeature ( 'and' mediaFeature )*
				{
				DebugLocation(136, 7);
				// CssGrammer.g3:136:7: (op1= NOT_SYM )?
				int alt7=2;
				try { DebugEnterSubRule(7);
				try { DebugEnterDecision(7, decisionCanBacktrack[7]);
				int LA7_0 = input.LA(1);

				if ((LA7_0==NOT_SYM))
				{
					alt7=1;
				}
				} finally { DebugExitDecision(7); }
				switch (alt7)
				{
				case 1:
					DebugEnterAlt(1);
					// CssGrammer.g3:136:8: op1= NOT_SYM
					{
					DebugLocation(136, 11);
					op1=(CommonToken)Match(input,NOT_SYM,Follow._NOT_SYM_in_mediaQuery718); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_NOT_SYM.Add(op1);


					}
					break;

				}
				} finally { DebugExitSubRule(7); }

				DebugLocation(136, 22);
				PushFollow(Follow._mediaFeature_in_mediaQuery722);
				mediaFeature21=mediaFeature();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_mediaFeature.Add(mediaFeature21.Tree);
				DebugLocation(136, 35);
				// CssGrammer.g3:136:35: ( 'and' mediaFeature )*
				try { DebugEnterSubRule(8);
				while (true)
				{
					int alt8=2;
					try { DebugEnterDecision(8, decisionCanBacktrack[8]);
					int LA8_0 = input.LA(1);

					if ((LA8_0==147))
					{
						alt8=1;
					}


					} finally { DebugExitDecision(8); }
					switch ( alt8 )
					{
					case 1:
						DebugEnterAlt(1);
						// CssGrammer.g3:136:37: 'and' mediaFeature
						{
						DebugLocation(136, 37);
						string_literal22=(CommonToken)Match(input,147,Follow._147_in_mediaQuery726); if (state.failed) return retval; 
						if ( state.backtracking == 0 ) stream_147.Add(string_literal22);

						DebugLocation(136, 43);
						PushFollow(Follow._mediaFeature_in_mediaQuery728);
						mediaFeature23=mediaFeature();
						PopFollow();
						if (state.failed) return retval;
						if ( state.backtracking == 0 ) stream_mediaFeature.Add(mediaFeature23.Tree);

						}
						break;

					default:
						goto loop8;
					}
				}

				loop8:
					;

				} finally { DebugExitSubRule(8); }



				{
				// AST REWRITE
				// elements: op1, mediaFeature
				// token labels: op1
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.Tree = root_0;
				RewriteRuleITokenStream stream_op1=new RewriteRuleITokenStream(adaptor,"token op1",op1);
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

				root_0 = (CommonTree)adaptor.Nil();
				// 137:9: -> ^( MEDIA_QUERY ( $op1)? ( mediaFeature )* )
				{
					DebugLocation(137, 12);
					// CssGrammer.g3:137:12: ^( MEDIA_QUERY ( $op1)? ( mediaFeature )* )
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(137, 14);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(MEDIA_QUERY, "MEDIA_QUERY"), root_1);

					DebugLocation(137, 27);
					// CssGrammer.g3:137:27: ( $op1)?
					if ( stream_op1.HasNext )
					{
						DebugLocation(137, 27);
						adaptor.AddChild(root_1, stream_op1.NextNode());

					}
					stream_op1.Reset();
					DebugLocation(137, 32);
					// CssGrammer.g3:137:32: ( mediaFeature )*
					while ( stream_mediaFeature.HasNext )
					{
						DebugLocation(137, 32);
						adaptor.AddChild(root_1, stream_mediaFeature.NextTree());

					}
					stream_mediaFeature.Reset();

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
				// CssGrammer.g3:138:7: (op1= NOT_SYM )? medium ( 'and' mediaFeature )*
				{
				DebugLocation(138, 7);
				// CssGrammer.g3:138:7: (op1= NOT_SYM )?
				int alt9=2;
				try { DebugEnterSubRule(9);
				try { DebugEnterDecision(9, decisionCanBacktrack[9]);
				int LA9_0 = input.LA(1);

				if ((LA9_0==NOT_SYM))
				{
					alt9=1;
				}
				} finally { DebugExitDecision(9); }
				switch (alt9)
				{
				case 1:
					DebugEnterAlt(1);
					// CssGrammer.g3:138:8: op1= NOT_SYM
					{
					DebugLocation(138, 11);
					op1=(CommonToken)Match(input,NOT_SYM,Follow._NOT_SYM_in_mediaQuery762); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_NOT_SYM.Add(op1);


					}
					break;

				}
				} finally { DebugExitSubRule(9); }

				DebugLocation(138, 22);
				PushFollow(Follow._medium_in_mediaQuery766);
				medium24=medium();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_medium.Add(medium24.Tree);
				DebugLocation(138, 29);
				// CssGrammer.g3:138:29: ( 'and' mediaFeature )*
				try { DebugEnterSubRule(10);
				while (true)
				{
					int alt10=2;
					try { DebugEnterDecision(10, decisionCanBacktrack[10]);
					int LA10_0 = input.LA(1);

					if ((LA10_0==147))
					{
						alt10=1;
					}


					} finally { DebugExitDecision(10); }
					switch ( alt10 )
					{
					case 1:
						DebugEnterAlt(1);
						// CssGrammer.g3:138:30: 'and' mediaFeature
						{
						DebugLocation(138, 30);
						string_literal25=(CommonToken)Match(input,147,Follow._147_in_mediaQuery769); if (state.failed) return retval; 
						if ( state.backtracking == 0 ) stream_147.Add(string_literal25);

						DebugLocation(138, 36);
						PushFollow(Follow._mediaFeature_in_mediaQuery771);
						mediaFeature26=mediaFeature();
						PopFollow();
						if (state.failed) return retval;
						if ( state.backtracking == 0 ) stream_mediaFeature.Add(mediaFeature26.Tree);

						}
						break;

					default:
						goto loop10;
					}
				}

				loop10:
					;

				} finally { DebugExitSubRule(10); }



				{
				// AST REWRITE
				// elements: op1, medium, mediaFeature
				// token labels: op1
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.Tree = root_0;
				RewriteRuleITokenStream stream_op1=new RewriteRuleITokenStream(adaptor,"token op1",op1);
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

				root_0 = (CommonTree)adaptor.Nil();
				// 139:9: -> ^( MEDIA_QUERY ( $op1)? medium ( mediaFeature )* )
				{
					DebugLocation(139, 12);
					// CssGrammer.g3:139:12: ^( MEDIA_QUERY ( $op1)? medium ( mediaFeature )* )
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(139, 14);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(MEDIA_QUERY, "MEDIA_QUERY"), root_1);

					DebugLocation(139, 27);
					// CssGrammer.g3:139:27: ( $op1)?
					if ( stream_op1.HasNext )
					{
						DebugLocation(139, 27);
						adaptor.AddChild(root_1, stream_op1.NextNode());

					}
					stream_op1.Reset();
					DebugLocation(139, 32);
					adaptor.AddChild(root_1, stream_medium.NextTree());
					DebugLocation(139, 39);
					// CssGrammer.g3:139:39: ( mediaFeature )*
					while ( stream_mediaFeature.HasNext )
					{
						DebugLocation(139, 39);
						adaptor.AddChild(root_1, stream_mediaFeature.NextTree());

					}
					stream_mediaFeature.Reset();

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
			TraceOut("mediaQuery", 5);
			LeaveRule("mediaQuery", 5);
			Leave_mediaQuery();
			if (state.backtracking > 0) { Memoize(input, 5, mediaQuery_StartIndex); }
		}
		DebugLocation(140, 4);
		} finally { DebugExitRule(GrammarFileName, "mediaQuery"); }
		return retval;

	}
	// $ANTLR end "mediaQuery"

	public class mediaFeature_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_mediaFeature();
	partial void Leave_mediaFeature();

	// $ANTLR start "mediaFeature"
	// CssGrammer.g3:144:1: mediaFeature : ( LPAREN property COLON term RPAREN -> ^( MEDIA_FEATURE property term ) | LPAREN property RPAREN -> ^( MEDIA_FEATURE property ) | rangeForm );
	[GrammarRule("mediaFeature")]
	private CssGrammerParser.mediaFeature_return mediaFeature()
	{
		Enter_mediaFeature();
		EnterRule("mediaFeature", 6);
		TraceIn("mediaFeature", 6);
		CssGrammerParser.mediaFeature_return retval = new CssGrammerParser.mediaFeature_return();
		retval.Start = (CommonToken)input.LT(1);
		int mediaFeature_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken LPAREN27=null;
		CommonToken COLON29=null;
		CommonToken RPAREN31=null;
		CommonToken LPAREN32=null;
		CommonToken RPAREN34=null;
		CssGrammerParser.property_return property28 = default(CssGrammerParser.property_return);
		CssGrammerParser.term_return term30 = default(CssGrammerParser.term_return);
		CssGrammerParser.property_return property33 = default(CssGrammerParser.property_return);
		CssGrammerParser.rangeForm_return rangeForm35 = default(CssGrammerParser.rangeForm_return);

		CommonTree LPAREN27_tree=null;
		CommonTree COLON29_tree=null;
		CommonTree RPAREN31_tree=null;
		CommonTree LPAREN32_tree=null;
		CommonTree RPAREN34_tree=null;
		RewriteRuleITokenStream stream_LPAREN=new RewriteRuleITokenStream(adaptor,"token LPAREN");
		RewriteRuleITokenStream stream_COLON=new RewriteRuleITokenStream(adaptor,"token COLON");
		RewriteRuleITokenStream stream_RPAREN=new RewriteRuleITokenStream(adaptor,"token RPAREN");
		RewriteRuleSubtreeStream stream_property=new RewriteRuleSubtreeStream(adaptor,"rule property");
		RewriteRuleSubtreeStream stream_term=new RewriteRuleSubtreeStream(adaptor,"rule term");
		try { DebugEnterRule(GrammarFileName, "mediaFeature");
		DebugLocation(144, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 6)) { return retval; }
			// CssGrammer.g3:145:5: ( LPAREN property COLON term RPAREN -> ^( MEDIA_FEATURE property term ) | LPAREN property RPAREN -> ^( MEDIA_FEATURE property ) | rangeForm )
			int alt12=3;
			try { DebugEnterDecision(12, decisionCanBacktrack[12]);
			int LA12_0 = input.LA(1);

			if ((LA12_0==LPAREN))
			{
				int LA12_1 = input.LA(2);

				if ((EvaluatePredicate(synpred13_CssGrammer_fragment)))
				{
					alt12=1;
				}
				else if ((EvaluatePredicate(synpred14_CssGrammer_fragment)))
				{
					alt12=2;
				}
				else if ((true))
				{
					alt12=3;
				}
				else
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 12, 1, input);

					DebugRecognitionException(nvae);
					throw nvae;
				}
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return retval;}
				NoViableAltException nvae = new NoViableAltException("", 12, 0, input);

				DebugRecognitionException(nvae);
				throw nvae;
			}
			} finally { DebugExitDecision(12); }
			switch (alt12)
			{
			case 1:
				DebugEnterAlt(1);
				// CssGrammer.g3:145:7: LPAREN property COLON term RPAREN
				{
				DebugLocation(145, 7);
				LPAREN27=(CommonToken)Match(input,LPAREN,Follow._LPAREN_in_mediaFeature815); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_LPAREN.Add(LPAREN27);

				DebugLocation(145, 14);
				PushFollow(Follow._property_in_mediaFeature817);
				property28=property();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_property.Add(property28.Tree);
				DebugLocation(145, 23);
				COLON29=(CommonToken)Match(input,COLON,Follow._COLON_in_mediaFeature819); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_COLON.Add(COLON29);

				DebugLocation(145, 29);
				PushFollow(Follow._term_in_mediaFeature821);
				term30=term();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_term.Add(term30.Tree);
				DebugLocation(145, 34);
				RPAREN31=(CommonToken)Match(input,RPAREN,Follow._RPAREN_in_mediaFeature823); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_RPAREN.Add(RPAREN31);



				{
				// AST REWRITE
				// elements: property, term
				// token labels: 
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.Tree = root_0;
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

				root_0 = (CommonTree)adaptor.Nil();
				// 146:9: -> ^( MEDIA_FEATURE property term )
				{
					DebugLocation(146, 12);
					// CssGrammer.g3:146:12: ^( MEDIA_FEATURE property term )
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(146, 14);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(MEDIA_FEATURE, "MEDIA_FEATURE"), root_1);

					DebugLocation(146, 28);
					adaptor.AddChild(root_1, stream_property.NextTree());
					DebugLocation(146, 37);
					adaptor.AddChild(root_1, stream_term.NextTree());

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
				// CssGrammer.g3:147:7: LPAREN property RPAREN
				{
				DebugLocation(147, 7);
				LPAREN32=(CommonToken)Match(input,LPAREN,Follow._LPAREN_in_mediaFeature849); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_LPAREN.Add(LPAREN32);

				DebugLocation(147, 14);
				PushFollow(Follow._property_in_mediaFeature851);
				property33=property();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_property.Add(property33.Tree);
				DebugLocation(147, 23);
				RPAREN34=(CommonToken)Match(input,RPAREN,Follow._RPAREN_in_mediaFeature853); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_RPAREN.Add(RPAREN34);



				{
				// AST REWRITE
				// elements: property
				// token labels: 
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.Tree = root_0;
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

				root_0 = (CommonTree)adaptor.Nil();
				// 148:9: -> ^( MEDIA_FEATURE property )
				{
					DebugLocation(148, 12);
					// CssGrammer.g3:148:12: ^( MEDIA_FEATURE property )
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(148, 14);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(MEDIA_FEATURE, "MEDIA_FEATURE"), root_1);

					DebugLocation(148, 28);
					adaptor.AddChild(root_1, stream_property.NextTree());

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
				// CssGrammer.g3:149:7: rangeForm
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(149, 7);
				PushFollow(Follow._rangeForm_in_mediaFeature877);
				rangeForm35=rangeForm();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, rangeForm35.Tree);

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
			TraceOut("mediaFeature", 6);
			LeaveRule("mediaFeature", 6);
			Leave_mediaFeature();
			if (state.backtracking > 0) { Memoize(input, 6, mediaFeature_StartIndex); }
		}
		DebugLocation(150, 4);
		} finally { DebugExitRule(GrammarFileName, "mediaFeature"); }
		return retval;

	}
	// $ANTLR end "mediaFeature"

	public class rangeForm_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_rangeForm();
	partial void Leave_rangeForm();

	// $ANTLR start "rangeForm"
	// CssGrammer.g3:151:1: rangeForm : ( LPAREN property comparisionOp term RPAREN -> ^( MEDIA_FEATURE property term comparisionOp ) | LPAREN t1= term r1= rightDirectionOp property r2= rightDirectionOp t2= term RPAREN -> ^( MEDIA_FEATURE property $t1 $r1 $t2 $r2) | LPAREN t1= term l1= leftDirectionOp property l2= leftDirectionOp t2= term RPAREN -> ^( MEDIA_FEATURE property $t1 $l1 $t2 $l2) );
	[GrammarRule("rangeForm")]
	private CssGrammerParser.rangeForm_return rangeForm()
	{
		Enter_rangeForm();
		EnterRule("rangeForm", 7);
		TraceIn("rangeForm", 7);
		CssGrammerParser.rangeForm_return retval = new CssGrammerParser.rangeForm_return();
		retval.Start = (CommonToken)input.LT(1);
		int rangeForm_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken LPAREN36=null;
		CommonToken RPAREN40=null;
		CommonToken LPAREN41=null;
		CommonToken RPAREN43=null;
		CommonToken LPAREN44=null;
		CommonToken RPAREN46=null;
		CssGrammerParser.term_return t1 = default(CssGrammerParser.term_return);
		CssGrammerParser.rightDirectionOp_return r1 = default(CssGrammerParser.rightDirectionOp_return);
		CssGrammerParser.rightDirectionOp_return r2 = default(CssGrammerParser.rightDirectionOp_return);
		CssGrammerParser.term_return t2 = default(CssGrammerParser.term_return);
		CssGrammerParser.leftDirectionOp_return l1 = default(CssGrammerParser.leftDirectionOp_return);
		CssGrammerParser.leftDirectionOp_return l2 = default(CssGrammerParser.leftDirectionOp_return);
		CssGrammerParser.property_return property37 = default(CssGrammerParser.property_return);
		CssGrammerParser.comparisionOp_return comparisionOp38 = default(CssGrammerParser.comparisionOp_return);
		CssGrammerParser.term_return term39 = default(CssGrammerParser.term_return);
		CssGrammerParser.property_return property42 = default(CssGrammerParser.property_return);
		CssGrammerParser.property_return property45 = default(CssGrammerParser.property_return);

		CommonTree LPAREN36_tree=null;
		CommonTree RPAREN40_tree=null;
		CommonTree LPAREN41_tree=null;
		CommonTree RPAREN43_tree=null;
		CommonTree LPAREN44_tree=null;
		CommonTree RPAREN46_tree=null;
		RewriteRuleITokenStream stream_LPAREN=new RewriteRuleITokenStream(adaptor,"token LPAREN");
		RewriteRuleITokenStream stream_RPAREN=new RewriteRuleITokenStream(adaptor,"token RPAREN");
		RewriteRuleSubtreeStream stream_property=new RewriteRuleSubtreeStream(adaptor,"rule property");
		RewriteRuleSubtreeStream stream_comparisionOp=new RewriteRuleSubtreeStream(adaptor,"rule comparisionOp");
		RewriteRuleSubtreeStream stream_term=new RewriteRuleSubtreeStream(adaptor,"rule term");
		RewriteRuleSubtreeStream stream_rightDirectionOp=new RewriteRuleSubtreeStream(adaptor,"rule rightDirectionOp");
		RewriteRuleSubtreeStream stream_leftDirectionOp=new RewriteRuleSubtreeStream(adaptor,"rule leftDirectionOp");
		try { DebugEnterRule(GrammarFileName, "rangeForm");
		DebugLocation(151, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 7)) { return retval; }
			// CssGrammer.g3:152:5: ( LPAREN property comparisionOp term RPAREN -> ^( MEDIA_FEATURE property term comparisionOp ) | LPAREN t1= term r1= rightDirectionOp property r2= rightDirectionOp t2= term RPAREN -> ^( MEDIA_FEATURE property $t1 $r1 $t2 $r2) | LPAREN t1= term l1= leftDirectionOp property l2= leftDirectionOp t2= term RPAREN -> ^( MEDIA_FEATURE property $t1 $l1 $t2 $l2) )
			int alt13=3;
			try { DebugEnterDecision(13, decisionCanBacktrack[13]);
			int LA13_0 = input.LA(1);

			if ((LA13_0==LPAREN))
			{
				int LA13_1 = input.LA(2);

				if ((EvaluatePredicate(synpred15_CssGrammer_fragment)))
				{
					alt13=1;
				}
				else if ((EvaluatePredicate(synpred16_CssGrammer_fragment)))
				{
					alt13=2;
				}
				else if ((true))
				{
					alt13=3;
				}
				else
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 13, 1, input);

					DebugRecognitionException(nvae);
					throw nvae;
				}
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return retval;}
				NoViableAltException nvae = new NoViableAltException("", 13, 0, input);

				DebugRecognitionException(nvae);
				throw nvae;
			}
			} finally { DebugExitDecision(13); }
			switch (alt13)
			{
			case 1:
				DebugEnterAlt(1);
				// CssGrammer.g3:152:7: LPAREN property comparisionOp term RPAREN
				{
				DebugLocation(152, 7);
				LPAREN36=(CommonToken)Match(input,LPAREN,Follow._LPAREN_in_rangeForm893); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_LPAREN.Add(LPAREN36);

				DebugLocation(152, 14);
				PushFollow(Follow._property_in_rangeForm895);
				property37=property();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_property.Add(property37.Tree);
				DebugLocation(152, 23);
				PushFollow(Follow._comparisionOp_in_rangeForm897);
				comparisionOp38=comparisionOp();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_comparisionOp.Add(comparisionOp38.Tree);
				DebugLocation(152, 37);
				PushFollow(Follow._term_in_rangeForm899);
				term39=term();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_term.Add(term39.Tree);
				DebugLocation(152, 42);
				RPAREN40=(CommonToken)Match(input,RPAREN,Follow._RPAREN_in_rangeForm901); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_RPAREN.Add(RPAREN40);



				{
				// AST REWRITE
				// elements: property, term, comparisionOp
				// token labels: 
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.Tree = root_0;
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

				root_0 = (CommonTree)adaptor.Nil();
				// 153:9: -> ^( MEDIA_FEATURE property term comparisionOp )
				{
					DebugLocation(153, 12);
					// CssGrammer.g3:153:12: ^( MEDIA_FEATURE property term comparisionOp )
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(153, 14);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(MEDIA_FEATURE, "MEDIA_FEATURE"), root_1);

					DebugLocation(153, 28);
					adaptor.AddChild(root_1, stream_property.NextTree());
					DebugLocation(153, 37);
					adaptor.AddChild(root_1, stream_term.NextTree());
					DebugLocation(153, 42);
					adaptor.AddChild(root_1, stream_comparisionOp.NextTree());

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
				// CssGrammer.g3:154:7: LPAREN t1= term r1= rightDirectionOp property r2= rightDirectionOp t2= term RPAREN
				{
				DebugLocation(154, 7);
				LPAREN41=(CommonToken)Match(input,LPAREN,Follow._LPAREN_in_rangeForm929); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_LPAREN.Add(LPAREN41);

				DebugLocation(154, 16);
				PushFollow(Follow._term_in_rangeForm933);
				t1=term();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_term.Add(t1.Tree);
				DebugLocation(154, 24);
				PushFollow(Follow._rightDirectionOp_in_rangeForm937);
				r1=rightDirectionOp();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_rightDirectionOp.Add(r1.Tree);
				DebugLocation(154, 42);
				PushFollow(Follow._property_in_rangeForm939);
				property42=property();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_property.Add(property42.Tree);
				DebugLocation(154, 53);
				PushFollow(Follow._rightDirectionOp_in_rangeForm943);
				r2=rightDirectionOp();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_rightDirectionOp.Add(r2.Tree);
				DebugLocation(154, 73);
				PushFollow(Follow._term_in_rangeForm947);
				t2=term();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_term.Add(t2.Tree);
				DebugLocation(154, 79);
				RPAREN43=(CommonToken)Match(input,RPAREN,Follow._RPAREN_in_rangeForm949); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_RPAREN.Add(RPAREN43);



				{
				// AST REWRITE
				// elements: property, t1, r1, t2, r2
				// token labels: 
				// rule labels: t1, r1, t2, r2, retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.Tree = root_0;
				RewriteRuleSubtreeStream stream_t1=new RewriteRuleSubtreeStream(adaptor,"rule t1",t1!=null?t1.Tree:null);
				RewriteRuleSubtreeStream stream_r1=new RewriteRuleSubtreeStream(adaptor,"rule r1",r1!=null?r1.Tree:null);
				RewriteRuleSubtreeStream stream_t2=new RewriteRuleSubtreeStream(adaptor,"rule t2",t2!=null?t2.Tree:null);
				RewriteRuleSubtreeStream stream_r2=new RewriteRuleSubtreeStream(adaptor,"rule r2",r2!=null?r2.Tree:null);
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

				root_0 = (CommonTree)adaptor.Nil();
				// 155:9: -> ^( MEDIA_FEATURE property $t1 $r1 $t2 $r2)
				{
					DebugLocation(155, 12);
					// CssGrammer.g3:155:12: ^( MEDIA_FEATURE property $t1 $r1 $t2 $r2)
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(155, 14);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(MEDIA_FEATURE, "MEDIA_FEATURE"), root_1);

					DebugLocation(155, 28);
					adaptor.AddChild(root_1, stream_property.NextTree());
					DebugLocation(155, 38);
					adaptor.AddChild(root_1, stream_t1.NextTree());
					DebugLocation(155, 42);
					adaptor.AddChild(root_1, stream_r1.NextTree());
					DebugLocation(155, 46);
					adaptor.AddChild(root_1, stream_t2.NextTree());
					DebugLocation(155, 50);
					adaptor.AddChild(root_1, stream_r2.NextTree());

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
				// CssGrammer.g3:156:7: LPAREN t1= term l1= leftDirectionOp property l2= leftDirectionOp t2= term RPAREN
				{
				DebugLocation(156, 7);
				LPAREN44=(CommonToken)Match(input,LPAREN,Follow._LPAREN_in_rangeForm985); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_LPAREN.Add(LPAREN44);

				DebugLocation(156, 16);
				PushFollow(Follow._term_in_rangeForm989);
				t1=term();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_term.Add(t1.Tree);
				DebugLocation(156, 24);
				PushFollow(Follow._leftDirectionOp_in_rangeForm993);
				l1=leftDirectionOp();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_leftDirectionOp.Add(l1.Tree);
				DebugLocation(156, 41);
				PushFollow(Follow._property_in_rangeForm995);
				property45=property();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_property.Add(property45.Tree);
				DebugLocation(156, 52);
				PushFollow(Follow._leftDirectionOp_in_rangeForm999);
				l2=leftDirectionOp();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_leftDirectionOp.Add(l2.Tree);
				DebugLocation(156, 71);
				PushFollow(Follow._term_in_rangeForm1003);
				t2=term();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_term.Add(t2.Tree);
				DebugLocation(156, 77);
				RPAREN46=(CommonToken)Match(input,RPAREN,Follow._RPAREN_in_rangeForm1005); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_RPAREN.Add(RPAREN46);



				{
				// AST REWRITE
				// elements: property, t1, l1, t2, l2
				// token labels: 
				// rule labels: t1, l1, t2, l2, retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.Tree = root_0;
				RewriteRuleSubtreeStream stream_t1=new RewriteRuleSubtreeStream(adaptor,"rule t1",t1!=null?t1.Tree:null);
				RewriteRuleSubtreeStream stream_l1=new RewriteRuleSubtreeStream(adaptor,"rule l1",l1!=null?l1.Tree:null);
				RewriteRuleSubtreeStream stream_t2=new RewriteRuleSubtreeStream(adaptor,"rule t2",t2!=null?t2.Tree:null);
				RewriteRuleSubtreeStream stream_l2=new RewriteRuleSubtreeStream(adaptor,"rule l2",l2!=null?l2.Tree:null);
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

				root_0 = (CommonTree)adaptor.Nil();
				// 157:9: -> ^( MEDIA_FEATURE property $t1 $l1 $t2 $l2)
				{
					DebugLocation(157, 12);
					// CssGrammer.g3:157:12: ^( MEDIA_FEATURE property $t1 $l1 $t2 $l2)
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(157, 14);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(MEDIA_FEATURE, "MEDIA_FEATURE"), root_1);

					DebugLocation(157, 28);
					adaptor.AddChild(root_1, stream_property.NextTree());
					DebugLocation(157, 38);
					adaptor.AddChild(root_1, stream_t1.NextTree());
					DebugLocation(157, 42);
					adaptor.AddChild(root_1, stream_l1.NextTree());
					DebugLocation(157, 46);
					adaptor.AddChild(root_1, stream_t2.NextTree());
					DebugLocation(157, 50);
					adaptor.AddChild(root_1, stream_l2.NextTree());

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
			TraceOut("rangeForm", 7);
			LeaveRule("rangeForm", 7);
			Leave_rangeForm();
			if (state.backtracking > 0) { Memoize(input, 7, rangeForm_StartIndex); }
		}
		DebugLocation(158, 4);
		} finally { DebugExitRule(GrammarFileName, "rangeForm"); }
		return retval;

	}
	// $ANTLR end "rangeForm"

	public class comparisionOp_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_comparisionOp();
	partial void Leave_comparisionOp();

	// $ANTLR start "comparisionOp"
	// CssGrammer.g3:160:1: comparisionOp : ( leftDirectionOp | rightDirectionOp );
	[GrammarRule("comparisionOp")]
	private CssGrammerParser.comparisionOp_return comparisionOp()
	{
		Enter_comparisionOp();
		EnterRule("comparisionOp", 8);
		TraceIn("comparisionOp", 8);
		CssGrammerParser.comparisionOp_return retval = new CssGrammerParser.comparisionOp_return();
		retval.Start = (CommonToken)input.LT(1);
		int comparisionOp_StartIndex = input.Index;
		CommonTree root_0 = null;

		CssGrammerParser.leftDirectionOp_return leftDirectionOp47 = default(CssGrammerParser.leftDirectionOp_return);
		CssGrammerParser.rightDirectionOp_return rightDirectionOp48 = default(CssGrammerParser.rightDirectionOp_return);


		try { DebugEnterRule(GrammarFileName, "comparisionOp");
		DebugLocation(160, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 8)) { return retval; }
			// CssGrammer.g3:161:5: ( leftDirectionOp | rightDirectionOp )
			int alt14=2;
			try { DebugEnterDecision(14, decisionCanBacktrack[14]);
			int LA14_0 = input.LA(1);

			if (((LA14_0>=144 && LA14_0<=145)))
			{
				alt14=1;
			}
			else if ((LA14_0==GREATER||LA14_0==146))
			{
				alt14=2;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return retval;}
				NoViableAltException nvae = new NoViableAltException("", 14, 0, input);

				DebugRecognitionException(nvae);
				throw nvae;
			}
			} finally { DebugExitDecision(14); }
			switch (alt14)
			{
			case 1:
				DebugEnterAlt(1);
				// CssGrammer.g3:161:7: leftDirectionOp
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(161, 7);
				PushFollow(Follow._leftDirectionOp_in_comparisionOp1050);
				leftDirectionOp47=leftDirectionOp();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, leftDirectionOp47.Tree);

				}
				break;
			case 2:
				DebugEnterAlt(2);
				// CssGrammer.g3:162:7: rightDirectionOp
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(162, 7);
				PushFollow(Follow._rightDirectionOp_in_comparisionOp1058);
				rightDirectionOp48=rightDirectionOp();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, rightDirectionOp48.Tree);

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
			TraceOut("comparisionOp", 8);
			LeaveRule("comparisionOp", 8);
			Leave_comparisionOp();
			if (state.backtracking > 0) { Memoize(input, 8, comparisionOp_StartIndex); }
		}
		DebugLocation(163, 4);
		} finally { DebugExitRule(GrammarFileName, "comparisionOp"); }
		return retval;

	}
	// $ANTLR end "comparisionOp"

	public class leftDirectionOp_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_leftDirectionOp();
	partial void Leave_leftDirectionOp();

	// $ANTLR start "leftDirectionOp"
	// CssGrammer.g3:165:1: leftDirectionOp : ( '<=' | '<' );
	[GrammarRule("leftDirectionOp")]
	private CssGrammerParser.leftDirectionOp_return leftDirectionOp()
	{
		Enter_leftDirectionOp();
		EnterRule("leftDirectionOp", 9);
		TraceIn("leftDirectionOp", 9);
		CssGrammerParser.leftDirectionOp_return retval = new CssGrammerParser.leftDirectionOp_return();
		retval.Start = (CommonToken)input.LT(1);
		int leftDirectionOp_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken set49=null;

		CommonTree set49_tree=null;

		try { DebugEnterRule(GrammarFileName, "leftDirectionOp");
		DebugLocation(165, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 9)) { return retval; }
			// CssGrammer.g3:166:5: ( '<=' | '<' )
			DebugEnterAlt(1);
			// CssGrammer.g3:
			{
			root_0 = (CommonTree)adaptor.Nil();

			DebugLocation(166, 5);
			set49=(CommonToken)input.LT(1);
			if ((input.LA(1)>=144 && input.LA(1)<=145))
			{
				input.Consume();
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, (CommonTree)adaptor.Create(set49));
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
			TraceOut("leftDirectionOp", 9);
			LeaveRule("leftDirectionOp", 9);
			Leave_leftDirectionOp();
			if (state.backtracking > 0) { Memoize(input, 9, leftDirectionOp_StartIndex); }
		}
		DebugLocation(168, 4);
		} finally { DebugExitRule(GrammarFileName, "leftDirectionOp"); }
		return retval;

	}
	// $ANTLR end "leftDirectionOp"

	public class rightDirectionOp_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_rightDirectionOp();
	partial void Leave_rightDirectionOp();

	// $ANTLR start "rightDirectionOp"
	// CssGrammer.g3:170:1: rightDirectionOp : ( '>=' | GREATER );
	[GrammarRule("rightDirectionOp")]
	private CssGrammerParser.rightDirectionOp_return rightDirectionOp()
	{
		Enter_rightDirectionOp();
		EnterRule("rightDirectionOp", 10);
		TraceIn("rightDirectionOp", 10);
		CssGrammerParser.rightDirectionOp_return retval = new CssGrammerParser.rightDirectionOp_return();
		retval.Start = (CommonToken)input.LT(1);
		int rightDirectionOp_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken set50=null;

		CommonTree set50_tree=null;

		try { DebugEnterRule(GrammarFileName, "rightDirectionOp");
		DebugLocation(170, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 10)) { return retval; }
			// CssGrammer.g3:171:5: ( '>=' | GREATER )
			DebugEnterAlt(1);
			// CssGrammer.g3:
			{
			root_0 = (CommonTree)adaptor.Nil();

			DebugLocation(171, 5);
			set50=(CommonToken)input.LT(1);
			if (input.LA(1)==GREATER||input.LA(1)==146)
			{
				input.Consume();
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, (CommonTree)adaptor.Create(set50));
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
			TraceOut("rightDirectionOp", 10);
			LeaveRule("rightDirectionOp", 10);
			Leave_rightDirectionOp();
			if (state.backtracking > 0) { Memoize(input, 10, rightDirectionOp_StartIndex); }
		}
		DebugLocation(173, 4);
		} finally { DebugExitRule(GrammarFileName, "rightDirectionOp"); }
		return retval;

	}
	// $ANTLR end "rightDirectionOp"

	public class medium_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_medium();
	partial void Leave_medium();

	// $ANTLR start "medium"
	// CssGrammer.g3:178:1: medium : IDENT ;
	[GrammarRule("medium")]
	private CssGrammerParser.medium_return medium()
	{
		Enter_medium();
		EnterRule("medium", 11);
		TraceIn("medium", 11);
		CssGrammerParser.medium_return retval = new CssGrammerParser.medium_return();
		retval.Start = (CommonToken)input.LT(1);
		int medium_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken IDENT51=null;

		CommonTree IDENT51_tree=null;

		try { DebugEnterRule(GrammarFileName, "medium");
		DebugLocation(178, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 11)) { return retval; }
			// CssGrammer.g3:179:5: ( IDENT )
			DebugEnterAlt(1);
			// CssGrammer.g3:179:7: IDENT
			{
			root_0 = (CommonTree)adaptor.Nil();

			DebugLocation(179, 7);
			IDENT51=(CommonToken)Match(input,IDENT,Follow._IDENT_in_medium1128); if (state.failed) return retval;
			if ( state.backtracking==0 ) {
			IDENT51_tree = (CommonTree)adaptor.Create(IDENT51);
			adaptor.AddChild(root_0, IDENT51_tree);
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
			TraceOut("medium", 11);
			LeaveRule("medium", 11);
			Leave_medium();
			if (state.backtracking > 0) { Memoize(input, 11, medium_StartIndex); }
		}
		DebugLocation(180, 4);
		} finally { DebugExitRule(GrammarFileName, "medium"); }
		return retval;

	}
	// $ANTLR end "medium"

	public class bodylist_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_bodylist();
	partial void Leave_bodylist();

	// $ANTLR start "bodylist"
	// CssGrammer.g3:183:1: bodylist : ( bodyset )* ;
	[GrammarRule("bodylist")]
	private CssGrammerParser.bodylist_return bodylist()
	{
		Enter_bodylist();
		EnterRule("bodylist", 12);
		TraceIn("bodylist", 12);
		CssGrammerParser.bodylist_return retval = new CssGrammerParser.bodylist_return();
		retval.Start = (CommonToken)input.LT(1);
		int bodylist_StartIndex = input.Index;
		CommonTree root_0 = null;

		CssGrammerParser.bodyset_return bodyset52 = default(CssGrammerParser.bodyset_return);


		try { DebugEnterRule(GrammarFileName, "bodylist");
		DebugLocation(183, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 12)) { return retval; }
			// CssGrammer.g3:184:5: ( ( bodyset )* )
			DebugEnterAlt(1);
			// CssGrammer.g3:184:7: ( bodyset )*
			{
			root_0 = (CommonTree)adaptor.Nil();

			DebugLocation(184, 7);
			// CssGrammer.g3:184:7: ( bodyset )*
			try { DebugEnterSubRule(15);
			while (true)
			{
				int alt15=2;
				try { DebugEnterDecision(15, decisionCanBacktrack[15]);
				int LA15_0 = input.LA(1);

				if ((LA15_0==COLON||(LA15_0>=DOT && LA15_0<=DOUBLE_COLON)||LA15_0==FONT_FACE||LA15_0==HASH||LA15_0==IDENT||LA15_0==KEYFRAMES_SYM||LA15_0==LBRACKET||LA15_0==MEDIA_SYM||LA15_0==PAGE_SYM||LA15_0==STAR))
				{
					alt15=1;
				}


				} finally { DebugExitDecision(15); }
				switch ( alt15 )
				{
				case 1:
					DebugEnterAlt(1);
					// CssGrammer.g3:184:7: bodyset
					{
					DebugLocation(184, 7);
					PushFollow(Follow._bodyset_in_bodylist1151);
					bodyset52=bodyset();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) adaptor.AddChild(root_0, bodyset52.Tree);

					}
					break;

				default:
					goto loop15;
				}
			}

			loop15:
				;

			} finally { DebugExitSubRule(15); }


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
			TraceOut("bodylist", 12);
			LeaveRule("bodylist", 12);
			Leave_bodylist();
			if (state.backtracking > 0) { Memoize(input, 12, bodylist_StartIndex); }
		}
		DebugLocation(185, 4);
		} finally { DebugExitRule(GrammarFileName, "bodylist"); }
		return retval;

	}
	// $ANTLR end "bodylist"

	public class bodyset_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_bodyset();
	partial void Leave_bodyset();

	// $ANTLR start "bodyset"
	// CssGrammer.g3:187:1: bodyset : ( ruleSet | media | page | keyframesRule );
	[GrammarRule("bodyset")]
	private CssGrammerParser.bodyset_return bodyset()
	{
		Enter_bodyset();
		EnterRule("bodyset", 13);
		TraceIn("bodyset", 13);
		CssGrammerParser.bodyset_return retval = new CssGrammerParser.bodyset_return();
		retval.Start = (CommonToken)input.LT(1);
		int bodyset_StartIndex = input.Index;
		CommonTree root_0 = null;

		CssGrammerParser.ruleSet_return ruleSet53 = default(CssGrammerParser.ruleSet_return);
		CssGrammerParser.media_return media54 = default(CssGrammerParser.media_return);
		CssGrammerParser.page_return page55 = default(CssGrammerParser.page_return);
		CssGrammerParser.keyframesRule_return keyframesRule56 = default(CssGrammerParser.keyframesRule_return);


		try { DebugEnterRule(GrammarFileName, "bodyset");
		DebugLocation(187, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 13)) { return retval; }
			// CssGrammer.g3:188:5: ( ruleSet | media | page | keyframesRule )
			int alt16=4;
			try { DebugEnterDecision(16, decisionCanBacktrack[16]);
			switch (input.LA(1))
			{
			case COLON:
			case DOT:
			case DOUBLE_COLON:
			case FONT_FACE:
			case HASH:
			case IDENT:
			case LBRACKET:
			case STAR:
				{
				alt16=1;
				}
				break;
			case MEDIA_SYM:
				{
				alt16=2;
				}
				break;
			case PAGE_SYM:
				{
				alt16=3;
				}
				break;
			case KEYFRAMES_SYM:
				{
				alt16=4;
				}
				break;
			default:
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 16, 0, input);

					DebugRecognitionException(nvae);
					throw nvae;
				}
			}

			} finally { DebugExitDecision(16); }
			switch (alt16)
			{
			case 1:
				DebugEnterAlt(1);
				// CssGrammer.g3:188:7: ruleSet
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(188, 7);
				PushFollow(Follow._ruleSet_in_bodyset1173);
				ruleSet53=ruleSet();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, ruleSet53.Tree);

				}
				break;
			case 2:
				DebugEnterAlt(2);
				// CssGrammer.g3:189:7: media
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(189, 7);
				PushFollow(Follow._media_in_bodyset1181);
				media54=media();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, media54.Tree);

				}
				break;
			case 3:
				DebugEnterAlt(3);
				// CssGrammer.g3:190:7: page
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(190, 7);
				PushFollow(Follow._page_in_bodyset1189);
				page55=page();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, page55.Tree);

				}
				break;
			case 4:
				DebugEnterAlt(4);
				// CssGrammer.g3:191:7: keyframesRule
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(191, 7);
				PushFollow(Follow._keyframesRule_in_bodyset1197);
				keyframesRule56=keyframesRule();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, keyframesRule56.Tree);

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
			TraceOut("bodyset", 13);
			LeaveRule("bodyset", 13);
			Leave_bodyset();
			if (state.backtracking > 0) { Memoize(input, 13, bodyset_StartIndex); }
		}
		DebugLocation(192, 4);
		} finally { DebugExitRule(GrammarFileName, "bodyset"); }
		return retval;

	}
	// $ANTLR end "bodyset"

	public class keyframesRule_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_keyframesRule();
	partial void Leave_keyframesRule();

	// $ANTLR start "keyframesRule"
	// CssGrammer.g3:194:1: keyframesRule : KEYFRAMES_SYM IDENT LBRACE ( keyframesBlock )* RBRACE -> ^( KEYFRAMES IDENT ( keyframesBlock )* ) ;
	[GrammarRule("keyframesRule")]
	private CssGrammerParser.keyframesRule_return keyframesRule()
	{
		Enter_keyframesRule();
		EnterRule("keyframesRule", 14);
		TraceIn("keyframesRule", 14);
		CssGrammerParser.keyframesRule_return retval = new CssGrammerParser.keyframesRule_return();
		retval.Start = (CommonToken)input.LT(1);
		int keyframesRule_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken KEYFRAMES_SYM57=null;
		CommonToken IDENT58=null;
		CommonToken LBRACE59=null;
		CommonToken RBRACE61=null;
		CssGrammerParser.keyframesBlock_return keyframesBlock60 = default(CssGrammerParser.keyframesBlock_return);

		CommonTree KEYFRAMES_SYM57_tree=null;
		CommonTree IDENT58_tree=null;
		CommonTree LBRACE59_tree=null;
		CommonTree RBRACE61_tree=null;
		RewriteRuleITokenStream stream_KEYFRAMES_SYM=new RewriteRuleITokenStream(adaptor,"token KEYFRAMES_SYM");
		RewriteRuleITokenStream stream_IDENT=new RewriteRuleITokenStream(adaptor,"token IDENT");
		RewriteRuleITokenStream stream_LBRACE=new RewriteRuleITokenStream(adaptor,"token LBRACE");
		RewriteRuleITokenStream stream_RBRACE=new RewriteRuleITokenStream(adaptor,"token RBRACE");
		RewriteRuleSubtreeStream stream_keyframesBlock=new RewriteRuleSubtreeStream(adaptor,"rule keyframesBlock");
		try { DebugEnterRule(GrammarFileName, "keyframesRule");
		DebugLocation(194, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 14)) { return retval; }
			// CssGrammer.g3:195:5: ( KEYFRAMES_SYM IDENT LBRACE ( keyframesBlock )* RBRACE -> ^( KEYFRAMES IDENT ( keyframesBlock )* ) )
			DebugEnterAlt(1);
			// CssGrammer.g3:195:7: KEYFRAMES_SYM IDENT LBRACE ( keyframesBlock )* RBRACE
			{
			DebugLocation(195, 7);
			KEYFRAMES_SYM57=(CommonToken)Match(input,KEYFRAMES_SYM,Follow._KEYFRAMES_SYM_in_keyframesRule1221); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_KEYFRAMES_SYM.Add(KEYFRAMES_SYM57);

			DebugLocation(195, 21);
			IDENT58=(CommonToken)Match(input,IDENT,Follow._IDENT_in_keyframesRule1223); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_IDENT.Add(IDENT58);

			DebugLocation(195, 27);
			LBRACE59=(CommonToken)Match(input,LBRACE,Follow._LBRACE_in_keyframesRule1225); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_LBRACE.Add(LBRACE59);

			DebugLocation(195, 34);
			// CssGrammer.g3:195:34: ( keyframesBlock )*
			try { DebugEnterSubRule(17);
			while (true)
			{
				int alt17=2;
				try { DebugEnterDecision(17, decisionCanBacktrack[17]);
				int LA17_0 = input.LA(1);

				if ((LA17_0==FROM_SYM||LA17_0==PERCENTAGE||LA17_0==TO_SYM))
				{
					alt17=1;
				}


				} finally { DebugExitDecision(17); }
				switch ( alt17 )
				{
				case 1:
					DebugEnterAlt(1);
					// CssGrammer.g3:195:34: keyframesBlock
					{
					DebugLocation(195, 34);
					PushFollow(Follow._keyframesBlock_in_keyframesRule1227);
					keyframesBlock60=keyframesBlock();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_keyframesBlock.Add(keyframesBlock60.Tree);

					}
					break;

				default:
					goto loop17;
				}
			}

			loop17:
				;

			} finally { DebugExitSubRule(17); }

			DebugLocation(195, 50);
			RBRACE61=(CommonToken)Match(input,RBRACE,Follow._RBRACE_in_keyframesRule1230); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_RBRACE.Add(RBRACE61);



			{
			// AST REWRITE
			// elements: IDENT, keyframesBlock
			// token labels: 
			// rule labels: retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 196:9: -> ^( KEYFRAMES IDENT ( keyframesBlock )* )
			{
				DebugLocation(196, 12);
				// CssGrammer.g3:196:12: ^( KEYFRAMES IDENT ( keyframesBlock )* )
				{
				CommonTree root_1 = (CommonTree)adaptor.Nil();
				DebugLocation(196, 14);
				root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(KEYFRAMES, "KEYFRAMES"), root_1);

				DebugLocation(196, 24);
				adaptor.AddChild(root_1, stream_IDENT.NextNode());
				DebugLocation(196, 30);
				// CssGrammer.g3:196:30: ( keyframesBlock )*
				while ( stream_keyframesBlock.HasNext )
				{
					DebugLocation(196, 30);
					adaptor.AddChild(root_1, stream_keyframesBlock.NextTree());

				}
				stream_keyframesBlock.Reset();

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
			TraceOut("keyframesRule", 14);
			LeaveRule("keyframesRule", 14);
			Leave_keyframesRule();
			if (state.backtracking > 0) { Memoize(input, 14, keyframesRule_StartIndex); }
		}
		DebugLocation(197, 4);
		} finally { DebugExitRule(GrammarFileName, "keyframesRule"); }
		return retval;

	}
	// $ANTLR end "keyframesRule"

	public class keyframesBlock_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_keyframesBlock();
	partial void Leave_keyframesBlock();

	// $ANTLR start "keyframesBlock"
	// CssGrammer.g3:199:1: keyframesBlock : keyframeSelectors LBRACE (decls= declarationSet )? RBRACE -> ^( KEYFRAME keyframeSelectors ( $decls)? ) ;
	[GrammarRule("keyframesBlock")]
	private CssGrammerParser.keyframesBlock_return keyframesBlock()
	{
		Enter_keyframesBlock();
		EnterRule("keyframesBlock", 15);
		TraceIn("keyframesBlock", 15);
		CssGrammerParser.keyframesBlock_return retval = new CssGrammerParser.keyframesBlock_return();
		retval.Start = (CommonToken)input.LT(1);
		int keyframesBlock_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken LBRACE63=null;
		CommonToken RBRACE64=null;
		CssGrammerParser.declarationSet_return decls = default(CssGrammerParser.declarationSet_return);
		CssGrammerParser.keyframeSelectors_return keyframeSelectors62 = default(CssGrammerParser.keyframeSelectors_return);

		CommonTree LBRACE63_tree=null;
		CommonTree RBRACE64_tree=null;
		RewriteRuleITokenStream stream_LBRACE=new RewriteRuleITokenStream(adaptor,"token LBRACE");
		RewriteRuleITokenStream stream_RBRACE=new RewriteRuleITokenStream(adaptor,"token RBRACE");
		RewriteRuleSubtreeStream stream_keyframeSelectors=new RewriteRuleSubtreeStream(adaptor,"rule keyframeSelectors");
		RewriteRuleSubtreeStream stream_declarationSet=new RewriteRuleSubtreeStream(adaptor,"rule declarationSet");
		try { DebugEnterRule(GrammarFileName, "keyframesBlock");
		DebugLocation(199, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 15)) { return retval; }
			// CssGrammer.g3:200:5: ( keyframeSelectors LBRACE (decls= declarationSet )? RBRACE -> ^( KEYFRAME keyframeSelectors ( $decls)? ) )
			DebugEnterAlt(1);
			// CssGrammer.g3:200:7: keyframeSelectors LBRACE (decls= declarationSet )? RBRACE
			{
			DebugLocation(200, 7);
			PushFollow(Follow._keyframeSelectors_in_keyframesBlock1266);
			keyframeSelectors62=keyframeSelectors();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_keyframeSelectors.Add(keyframeSelectors62.Tree);
			DebugLocation(200, 25);
			LBRACE63=(CommonToken)Match(input,LBRACE,Follow._LBRACE_in_keyframesBlock1268); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_LBRACE.Add(LBRACE63);

			DebugLocation(200, 32);
			// CssGrammer.g3:200:32: (decls= declarationSet )?
			int alt18=2;
			try { DebugEnterSubRule(18);
			try { DebugEnterDecision(18, decisionCanBacktrack[18]);
			int LA18_0 = input.LA(1);

			if ((LA18_0==IDENT))
			{
				alt18=1;
			}
			} finally { DebugExitDecision(18); }
			switch (alt18)
			{
			case 1:
				DebugEnterAlt(1);
				// CssGrammer.g3:200:33: decls= declarationSet
				{
				DebugLocation(200, 38);
				PushFollow(Follow._declarationSet_in_keyframesBlock1273);
				decls=declarationSet();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_declarationSet.Add(decls.Tree);

				}
				break;

			}
			} finally { DebugExitSubRule(18); }

			DebugLocation(200, 56);
			RBRACE64=(CommonToken)Match(input,RBRACE,Follow._RBRACE_in_keyframesBlock1277); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_RBRACE.Add(RBRACE64);



			{
			// AST REWRITE
			// elements: keyframeSelectors, decls
			// token labels: 
			// rule labels: decls, retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_decls=new RewriteRuleSubtreeStream(adaptor,"rule decls",decls!=null?decls.Tree:null);
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 201:9: -> ^( KEYFRAME keyframeSelectors ( $decls)? )
			{
				DebugLocation(201, 12);
				// CssGrammer.g3:201:12: ^( KEYFRAME keyframeSelectors ( $decls)? )
				{
				CommonTree root_1 = (CommonTree)adaptor.Nil();
				DebugLocation(201, 14);
				root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(KEYFRAME, "KEYFRAME"), root_1);

				DebugLocation(201, 23);
				adaptor.AddChild(root_1, stream_keyframeSelectors.NextTree());
				DebugLocation(201, 42);
				// CssGrammer.g3:201:42: ( $decls)?
				if ( stream_decls.HasNext )
				{
					DebugLocation(201, 42);
					adaptor.AddChild(root_1, stream_decls.NextTree());

				}
				stream_decls.Reset();

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
			TraceOut("keyframesBlock", 15);
			LeaveRule("keyframesBlock", 15);
			Leave_keyframesBlock();
			if (state.backtracking > 0) { Memoize(input, 15, keyframesBlock_StartIndex); }
		}
		DebugLocation(202, 4);
		} finally { DebugExitRule(GrammarFileName, "keyframesBlock"); }
		return retval;

	}
	// $ANTLR end "keyframesBlock"

	public class keyframeSelectors_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_keyframeSelectors();
	partial void Leave_keyframeSelectors();

	// $ANTLR start "keyframeSelectors"
	// CssGrammer.g3:204:1: keyframeSelectors : keyframeSelector ( COMMA keyframeSelector )* -> ^( KEYFRAMESELECTORS ( keyframeSelector )+ ) ;
	[GrammarRule("keyframeSelectors")]
	private CssGrammerParser.keyframeSelectors_return keyframeSelectors()
	{
		Enter_keyframeSelectors();
		EnterRule("keyframeSelectors", 16);
		TraceIn("keyframeSelectors", 16);
		CssGrammerParser.keyframeSelectors_return retval = new CssGrammerParser.keyframeSelectors_return();
		retval.Start = (CommonToken)input.LT(1);
		int keyframeSelectors_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken COMMA66=null;
		CssGrammerParser.keyframeSelector_return keyframeSelector65 = default(CssGrammerParser.keyframeSelector_return);
		CssGrammerParser.keyframeSelector_return keyframeSelector67 = default(CssGrammerParser.keyframeSelector_return);

		CommonTree COMMA66_tree=null;
		RewriteRuleITokenStream stream_COMMA=new RewriteRuleITokenStream(adaptor,"token COMMA");
		RewriteRuleSubtreeStream stream_keyframeSelector=new RewriteRuleSubtreeStream(adaptor,"rule keyframeSelector");
		try { DebugEnterRule(GrammarFileName, "keyframeSelectors");
		DebugLocation(204, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 16)) { return retval; }
			// CssGrammer.g3:205:5: ( keyframeSelector ( COMMA keyframeSelector )* -> ^( KEYFRAMESELECTORS ( keyframeSelector )+ ) )
			DebugEnterAlt(1);
			// CssGrammer.g3:205:7: keyframeSelector ( COMMA keyframeSelector )*
			{
			DebugLocation(205, 7);
			PushFollow(Follow._keyframeSelector_in_keyframeSelectors1314);
			keyframeSelector65=keyframeSelector();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_keyframeSelector.Add(keyframeSelector65.Tree);
			DebugLocation(205, 24);
			// CssGrammer.g3:205:24: ( COMMA keyframeSelector )*
			try { DebugEnterSubRule(19);
			while (true)
			{
				int alt19=2;
				try { DebugEnterDecision(19, decisionCanBacktrack[19]);
				int LA19_0 = input.LA(1);

				if ((LA19_0==COMMA))
				{
					alt19=1;
				}


				} finally { DebugExitDecision(19); }
				switch ( alt19 )
				{
				case 1:
					DebugEnterAlt(1);
					// CssGrammer.g3:205:25: COMMA keyframeSelector
					{
					DebugLocation(205, 25);
					COMMA66=(CommonToken)Match(input,COMMA,Follow._COMMA_in_keyframeSelectors1317); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_COMMA.Add(COMMA66);

					DebugLocation(205, 31);
					PushFollow(Follow._keyframeSelector_in_keyframeSelectors1319);
					keyframeSelector67=keyframeSelector();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_keyframeSelector.Add(keyframeSelector67.Tree);

					}
					break;

				default:
					goto loop19;
				}
			}

			loop19:
				;

			} finally { DebugExitSubRule(19); }



			{
			// AST REWRITE
			// elements: keyframeSelector
			// token labels: 
			// rule labels: retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 206:9: -> ^( KEYFRAMESELECTORS ( keyframeSelector )+ )
			{
				DebugLocation(206, 12);
				// CssGrammer.g3:206:12: ^( KEYFRAMESELECTORS ( keyframeSelector )+ )
				{
				CommonTree root_1 = (CommonTree)adaptor.Nil();
				DebugLocation(206, 14);
				root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(KEYFRAMESELECTORS, "KEYFRAMESELECTORS"), root_1);

				DebugLocation(206, 32);
				if ( !(stream_keyframeSelector.HasNext) )
				{
					throw new RewriteEarlyExitException();
				}
				while ( stream_keyframeSelector.HasNext )
				{
					DebugLocation(206, 32);
					adaptor.AddChild(root_1, stream_keyframeSelector.NextTree());

				}
				stream_keyframeSelector.Reset();

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
			TraceOut("keyframeSelectors", 16);
			LeaveRule("keyframeSelectors", 16);
			Leave_keyframeSelectors();
			if (state.backtracking > 0) { Memoize(input, 16, keyframeSelectors_StartIndex); }
		}
		DebugLocation(207, 4);
		} finally { DebugExitRule(GrammarFileName, "keyframeSelectors"); }
		return retval;

	}
	// $ANTLR end "keyframeSelectors"

	public class keyframeSelector_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_keyframeSelector();
	partial void Leave_keyframeSelector();

	// $ANTLR start "keyframeSelector"
	// CssGrammer.g3:209:1: keyframeSelector : ( FROM_SYM | TO_SYM | PERCENTAGE );
	[GrammarRule("keyframeSelector")]
	private CssGrammerParser.keyframeSelector_return keyframeSelector()
	{
		Enter_keyframeSelector();
		EnterRule("keyframeSelector", 17);
		TraceIn("keyframeSelector", 17);
		CssGrammerParser.keyframeSelector_return retval = new CssGrammerParser.keyframeSelector_return();
		retval.Start = (CommonToken)input.LT(1);
		int keyframeSelector_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken set68=null;

		CommonTree set68_tree=null;

		try { DebugEnterRule(GrammarFileName, "keyframeSelector");
		DebugLocation(209, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 17)) { return retval; }
			// CssGrammer.g3:210:5: ( FROM_SYM | TO_SYM | PERCENTAGE )
			DebugEnterAlt(1);
			// CssGrammer.g3:
			{
			root_0 = (CommonTree)adaptor.Nil();

			DebugLocation(210, 5);
			set68=(CommonToken)input.LT(1);
			if (input.LA(1)==FROM_SYM||input.LA(1)==PERCENTAGE||input.LA(1)==TO_SYM)
			{
				input.Consume();
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, (CommonTree)adaptor.Create(set68));
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
			TraceOut("keyframeSelector", 17);
			LeaveRule("keyframeSelector", 17);
			Leave_keyframeSelector();
			if (state.backtracking > 0) { Memoize(input, 17, keyframeSelector_StartIndex); }
		}
		DebugLocation(213, 4);
		} finally { DebugExitRule(GrammarFileName, "keyframeSelector"); }
		return retval;

	}
	// $ANTLR end "keyframeSelector"

	public class page_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_page();
	partial void Leave_page();

	// $ANTLR start "page"
	// CssGrammer.g3:215:1: page : PAGE_SYM ( pseudoPage )? LBRACE declaration SEMI ( declaration SEMI )* RBRACE ;
	[GrammarRule("page")]
	private CssGrammerParser.page_return page()
	{
		Enter_page();
		EnterRule("page", 18);
		TraceIn("page", 18);
		CssGrammerParser.page_return retval = new CssGrammerParser.page_return();
		retval.Start = (CommonToken)input.LT(1);
		int page_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken PAGE_SYM69=null;
		CommonToken LBRACE71=null;
		CommonToken SEMI73=null;
		CommonToken SEMI75=null;
		CommonToken RBRACE76=null;
		CssGrammerParser.pseudoPage_return pseudoPage70 = default(CssGrammerParser.pseudoPage_return);
		CssGrammerParser.declaration_return declaration72 = default(CssGrammerParser.declaration_return);
		CssGrammerParser.declaration_return declaration74 = default(CssGrammerParser.declaration_return);

		CommonTree PAGE_SYM69_tree=null;
		CommonTree LBRACE71_tree=null;
		CommonTree SEMI73_tree=null;
		CommonTree SEMI75_tree=null;
		CommonTree RBRACE76_tree=null;

		try { DebugEnterRule(GrammarFileName, "page");
		DebugLocation(215, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 18)) { return retval; }
			// CssGrammer.g3:216:5: ( PAGE_SYM ( pseudoPage )? LBRACE declaration SEMI ( declaration SEMI )* RBRACE )
			DebugEnterAlt(1);
			// CssGrammer.g3:216:7: PAGE_SYM ( pseudoPage )? LBRACE declaration SEMI ( declaration SEMI )* RBRACE
			{
			root_0 = (CommonTree)adaptor.Nil();

			DebugLocation(216, 7);
			PAGE_SYM69=(CommonToken)Match(input,PAGE_SYM,Follow._PAGE_SYM_in_page1388); if (state.failed) return retval;
			if ( state.backtracking==0 ) {
			PAGE_SYM69_tree = (CommonTree)adaptor.Create(PAGE_SYM69);
			adaptor.AddChild(root_0, PAGE_SYM69_tree);
			}
			DebugLocation(216, 16);
			// CssGrammer.g3:216:16: ( pseudoPage )?
			int alt20=2;
			try { DebugEnterSubRule(20);
			try { DebugEnterDecision(20, decisionCanBacktrack[20]);
			int LA20_0 = input.LA(1);

			if ((LA20_0==COLON))
			{
				alt20=1;
			}
			} finally { DebugExitDecision(20); }
			switch (alt20)
			{
			case 1:
				DebugEnterAlt(1);
				// CssGrammer.g3:216:16: pseudoPage
				{
				DebugLocation(216, 16);
				PushFollow(Follow._pseudoPage_in_page1390);
				pseudoPage70=pseudoPage();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, pseudoPage70.Tree);

				}
				break;

			}
			} finally { DebugExitSubRule(20); }

			DebugLocation(217, 9);
			LBRACE71=(CommonToken)Match(input,LBRACE,Follow._LBRACE_in_page1401); if (state.failed) return retval;
			if ( state.backtracking==0 ) {
			LBRACE71_tree = (CommonTree)adaptor.Create(LBRACE71);
			adaptor.AddChild(root_0, LBRACE71_tree);
			}
			DebugLocation(218, 13);
			PushFollow(Follow._declaration_in_page1415);
			declaration72=declaration();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) adaptor.AddChild(root_0, declaration72.Tree);
			DebugLocation(218, 25);
			SEMI73=(CommonToken)Match(input,SEMI,Follow._SEMI_in_page1417); if (state.failed) return retval;
			if ( state.backtracking==0 ) {
			SEMI73_tree = (CommonTree)adaptor.Create(SEMI73);
			adaptor.AddChild(root_0, SEMI73_tree);
			}
			DebugLocation(218, 30);
			// CssGrammer.g3:218:30: ( declaration SEMI )*
			try { DebugEnterSubRule(21);
			while (true)
			{
				int alt21=2;
				try { DebugEnterDecision(21, decisionCanBacktrack[21]);
				int LA21_0 = input.LA(1);

				if ((LA21_0==IDENT))
				{
					alt21=1;
				}


				} finally { DebugExitDecision(21); }
				switch ( alt21 )
				{
				case 1:
					DebugEnterAlt(1);
					// CssGrammer.g3:218:31: declaration SEMI
					{
					DebugLocation(218, 31);
					PushFollow(Follow._declaration_in_page1420);
					declaration74=declaration();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) adaptor.AddChild(root_0, declaration74.Tree);
					DebugLocation(218, 43);
					SEMI75=(CommonToken)Match(input,SEMI,Follow._SEMI_in_page1422); if (state.failed) return retval;
					if ( state.backtracking==0 ) {
					SEMI75_tree = (CommonTree)adaptor.Create(SEMI75);
					adaptor.AddChild(root_0, SEMI75_tree);
					}

					}
					break;

				default:
					goto loop21;
				}
			}

			loop21:
				;

			} finally { DebugExitSubRule(21); }

			DebugLocation(219, 9);
			RBRACE76=(CommonToken)Match(input,RBRACE,Follow._RBRACE_in_page1434); if (state.failed) return retval;
			if ( state.backtracking==0 ) {
			RBRACE76_tree = (CommonTree)adaptor.Create(RBRACE76);
			adaptor.AddChild(root_0, RBRACE76_tree);
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
			TraceOut("page", 18);
			LeaveRule("page", 18);
			Leave_page();
			if (state.backtracking > 0) { Memoize(input, 18, page_StartIndex); }
		}
		DebugLocation(220, 4);
		} finally { DebugExitRule(GrammarFileName, "page"); }
		return retval;

	}
	// $ANTLR end "page"

	public class pseudoPage_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_pseudoPage();
	partial void Leave_pseudoPage();

	// $ANTLR start "pseudoPage"
	// CssGrammer.g3:222:1: pseudoPage : COLON IDENT ;
	[GrammarRule("pseudoPage")]
	private CssGrammerParser.pseudoPage_return pseudoPage()
	{
		Enter_pseudoPage();
		EnterRule("pseudoPage", 19);
		TraceIn("pseudoPage", 19);
		CssGrammerParser.pseudoPage_return retval = new CssGrammerParser.pseudoPage_return();
		retval.Start = (CommonToken)input.LT(1);
		int pseudoPage_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken COLON77=null;
		CommonToken IDENT78=null;

		CommonTree COLON77_tree=null;
		CommonTree IDENT78_tree=null;

		try { DebugEnterRule(GrammarFileName, "pseudoPage");
		DebugLocation(222, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 19)) { return retval; }
			// CssGrammer.g3:223:5: ( COLON IDENT )
			DebugEnterAlt(1);
			// CssGrammer.g3:223:7: COLON IDENT
			{
			root_0 = (CommonTree)adaptor.Nil();

			DebugLocation(223, 7);
			COLON77=(CommonToken)Match(input,COLON,Follow._COLON_in_pseudoPage1455); if (state.failed) return retval;
			if ( state.backtracking==0 ) {
			COLON77_tree = (CommonTree)adaptor.Create(COLON77);
			adaptor.AddChild(root_0, COLON77_tree);
			}
			DebugLocation(223, 13);
			IDENT78=(CommonToken)Match(input,IDENT,Follow._IDENT_in_pseudoPage1457); if (state.failed) return retval;
			if ( state.backtracking==0 ) {
			IDENT78_tree = (CommonTree)adaptor.Create(IDENT78);
			adaptor.AddChild(root_0, IDENT78_tree);
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
			TraceOut("pseudoPage", 19);
			LeaveRule("pseudoPage", 19);
			Leave_pseudoPage();
			if (state.backtracking > 0) { Memoize(input, 19, pseudoPage_StartIndex); }
		}
		DebugLocation(224, 4);
		} finally { DebugExitRule(GrammarFileName, "pseudoPage"); }
		return retval;

	}
	// $ANTLR end "pseudoPage"

	public class op_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_op();
	partial void Leave_op();

	// $ANTLR start "op"
	// CssGrammer.g3:226:1: op : ( SOLIDUS | COMMA |);
	[GrammarRule("op")]
	private CssGrammerParser.op_return op()
	{
		Enter_op();
		EnterRule("op", 20);
		TraceIn("op", 20);
		CssGrammerParser.op_return retval = new CssGrammerParser.op_return();
		retval.Start = (CommonToken)input.LT(1);
		int op_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken SOLIDUS79=null;
		CommonToken COMMA80=null;

		CommonTree SOLIDUS79_tree=null;
		CommonTree COMMA80_tree=null;

		try { DebugEnterRule(GrammarFileName, "op");
		DebugLocation(226, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 20)) { return retval; }
			// CssGrammer.g3:227:5: ( SOLIDUS | COMMA |)
			int alt22=3;
			try { DebugEnterDecision(22, decisionCanBacktrack[22]);
			switch (input.LA(1))
			{
			case SOLIDUS:
				{
				alt22=1;
				}
				break;
			case COMMA:
				{
				alt22=2;
				}
				break;
			case EOF:
				{
				alt22=3;
				}
				break;
			default:
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 22, 0, input);

					DebugRecognitionException(nvae);
					throw nvae;
				}
			}

			} finally { DebugExitDecision(22); }
			switch (alt22)
			{
			case 1:
				DebugEnterAlt(1);
				// CssGrammer.g3:227:7: SOLIDUS
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(227, 7);
				SOLIDUS79=(CommonToken)Match(input,SOLIDUS,Follow._SOLIDUS_in_op1478); if (state.failed) return retval;
				if ( state.backtracking==0 ) {
				SOLIDUS79_tree = (CommonTree)adaptor.Create(SOLIDUS79);
				adaptor.AddChild(root_0, SOLIDUS79_tree);
				}

				}
				break;
			case 2:
				DebugEnterAlt(2);
				// CssGrammer.g3:228:7: COMMA
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(228, 7);
				COMMA80=(CommonToken)Match(input,COMMA,Follow._COMMA_in_op1486); if (state.failed) return retval;
				if ( state.backtracking==0 ) {
				COMMA80_tree = (CommonTree)adaptor.Create(COMMA80);
				adaptor.AddChild(root_0, COMMA80_tree);
				}

				}
				break;
			case 3:
				DebugEnterAlt(3);
				// CssGrammer.g3:230:5: 
				{
				root_0 = (CommonTree)adaptor.Nil();

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
			TraceOut("op", 20);
			LeaveRule("op", 20);
			Leave_op();
			if (state.backtracking > 0) { Memoize(input, 20, op_StartIndex); }
		}
		DebugLocation(230, 4);
		} finally { DebugExitRule(GrammarFileName, "op"); }
		return retval;

	}
	// $ANTLR end "op"

	public class combinator_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_combinator();
	partial void Leave_combinator();

	// $ANTLR start "combinator"
	// CssGrammer.g3:232:1: combinator : ( PLUS -> ^( PRECEDEDS ) | GREATER -> ^( PARENTOF ) | '~' -> ^( FOLLOWS ) | -> ^( UNDER ) );
	[GrammarRule("combinator")]
	private CssGrammerParser.combinator_return combinator()
	{
		Enter_combinator();
		EnterRule("combinator", 21);
		TraceIn("combinator", 21);
		CssGrammerParser.combinator_return retval = new CssGrammerParser.combinator_return();
		retval.Start = (CommonToken)input.LT(1);
		int combinator_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken PLUS81=null;
		CommonToken GREATER82=null;
		CommonToken char_literal83=null;

		CommonTree PLUS81_tree=null;
		CommonTree GREATER82_tree=null;
		CommonTree char_literal83_tree=null;
		RewriteRuleITokenStream stream_PLUS=new RewriteRuleITokenStream(adaptor,"token PLUS");
		RewriteRuleITokenStream stream_GREATER=new RewriteRuleITokenStream(adaptor,"token GREATER");
		RewriteRuleITokenStream stream_152=new RewriteRuleITokenStream(adaptor,"token 152");

		try { DebugEnterRule(GrammarFileName, "combinator");
		DebugLocation(232, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 21)) { return retval; }
			// CssGrammer.g3:233:5: ( PLUS -> ^( PRECEDEDS ) | GREATER -> ^( PARENTOF ) | '~' -> ^( FOLLOWS ) | -> ^( UNDER ) )
			int alt23=4;
			try { DebugEnterDecision(23, decisionCanBacktrack[23]);
			switch (input.LA(1))
			{
			case PLUS:
				{
				alt23=1;
				}
				break;
			case GREATER:
				{
				alt23=2;
				}
				break;
			case 152:
				{
				alt23=3;
				}
				break;
			case COLON:
			case DOT:
			case DOUBLE_COLON:
			case HASH:
			case IDENT:
			case LBRACKET:
			case STAR:
				{
				alt23=4;
				}
				break;
			default:
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 23, 0, input);

					DebugRecognitionException(nvae);
					throw nvae;
				}
			}

			} finally { DebugExitDecision(23); }
			switch (alt23)
			{
			case 1:
				DebugEnterAlt(1);
				// CssGrammer.g3:233:7: PLUS
				{
				DebugLocation(233, 7);
				PLUS81=(CommonToken)Match(input,PLUS,Follow._PLUS_in_combinator1513); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_PLUS.Add(PLUS81);



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
				// 233:12: -> ^( PRECEDEDS )
				{
					DebugLocation(233, 15);
					// CssGrammer.g3:233:15: ^( PRECEDEDS )
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(233, 17);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(PRECEDEDS, "PRECEDEDS"), root_1);

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
				// CssGrammer.g3:234:7: GREATER
				{
				DebugLocation(234, 7);
				GREATER82=(CommonToken)Match(input,GREATER,Follow._GREATER_in_combinator1527); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_GREATER.Add(GREATER82);



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
				// 234:15: -> ^( PARENTOF )
				{
					DebugLocation(234, 18);
					// CssGrammer.g3:234:18: ^( PARENTOF )
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(234, 20);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(PARENTOF, "PARENTOF"), root_1);

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
				// CssGrammer.g3:235:7: '~'
				{
				DebugLocation(235, 7);
				char_literal83=(CommonToken)Match(input,152,Follow._152_in_combinator1541); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_152.Add(char_literal83);



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
				// 235:11: -> ^( FOLLOWS )
				{
					DebugLocation(235, 14);
					// CssGrammer.g3:235:14: ^( FOLLOWS )
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(235, 16);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(FOLLOWS, "FOLLOWS"), root_1);

					adaptor.AddChild(root_0, root_1);
					}

				}

				retval.Tree = root_0;
				}
				}

				}
				break;
			case 4:
				DebugEnterAlt(4);
				// CssGrammer.g3:236:7: 
				{

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
				// 236:7: -> ^( UNDER )
				{
					DebugLocation(236, 10);
					// CssGrammer.g3:236:10: ^( UNDER )
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(236, 12);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(UNDER, "UNDER"), root_1);

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
			TraceOut("combinator", 21);
			LeaveRule("combinator", 21);
			Leave_combinator();
			if (state.backtracking > 0) { Memoize(input, 21, combinator_StartIndex); }
		}
		DebugLocation(237, 4);
		} finally { DebugExitRule(GrammarFileName, "combinator"); }
		return retval;

	}
	// $ANTLR end "combinator"

	public class unaryOperator_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_unaryOperator();
	partial void Leave_unaryOperator();

	// $ANTLR start "unaryOperator"
	// CssGrammer.g3:239:1: unaryOperator : ( MINUS | PLUS );
	[GrammarRule("unaryOperator")]
	private CssGrammerParser.unaryOperator_return unaryOperator()
	{
		Enter_unaryOperator();
		EnterRule("unaryOperator", 22);
		TraceIn("unaryOperator", 22);
		CssGrammerParser.unaryOperator_return retval = new CssGrammerParser.unaryOperator_return();
		retval.Start = (CommonToken)input.LT(1);
		int unaryOperator_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken set84=null;

		CommonTree set84_tree=null;

		try { DebugEnterRule(GrammarFileName, "unaryOperator");
		DebugLocation(239, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 22)) { return retval; }
			// CssGrammer.g3:240:5: ( MINUS | PLUS )
			DebugEnterAlt(1);
			// CssGrammer.g3:
			{
			root_0 = (CommonTree)adaptor.Nil();

			DebugLocation(240, 5);
			set84=(CommonToken)input.LT(1);
			if (input.LA(1)==MINUS||input.LA(1)==PLUS)
			{
				input.Consume();
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, (CommonTree)adaptor.Create(set84));
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
			TraceOut("unaryOperator", 22);
			LeaveRule("unaryOperator", 22);
			Leave_unaryOperator();
			if (state.backtracking > 0) { Memoize(input, 22, unaryOperator_StartIndex); }
		}
		DebugLocation(242, 4);
		} finally { DebugExitRule(GrammarFileName, "unaryOperator"); }
		return retval;

	}
	// $ANTLR end "unaryOperator"

	public class property_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_property();
	partial void Leave_property();

	// $ANTLR start "property"
	// CssGrammer.g3:244:1: property : IDENT ;
	[GrammarRule("property")]
	private CssGrammerParser.property_return property()
	{
		Enter_property();
		EnterRule("property", 23);
		TraceIn("property", 23);
		CssGrammerParser.property_return retval = new CssGrammerParser.property_return();
		retval.Start = (CommonToken)input.LT(1);
		int property_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken IDENT85=null;

		CommonTree IDENT85_tree=null;

		try { DebugEnterRule(GrammarFileName, "property");
		DebugLocation(244, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 23)) { return retval; }
			// CssGrammer.g3:245:5: ( IDENT )
			DebugEnterAlt(1);
			// CssGrammer.g3:245:7: IDENT
			{
			root_0 = (CommonTree)adaptor.Nil();

			DebugLocation(245, 7);
			IDENT85=(CommonToken)Match(input,IDENT,Follow._IDENT_in_property1611); if (state.failed) return retval;
			if ( state.backtracking==0 ) {
			IDENT85_tree = (CommonTree)adaptor.Create(IDENT85);
			adaptor.AddChild(root_0, IDENT85_tree);
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
			TraceOut("property", 23);
			LeaveRule("property", 23);
			Leave_property();
			if (state.backtracking > 0) { Memoize(input, 23, property_StartIndex); }
		}
		DebugLocation(246, 4);
		} finally { DebugExitRule(GrammarFileName, "property"); }
		return retval;

	}
	// $ANTLR end "property"

	public class ruleSet_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_ruleSet();
	partial void Leave_ruleSet();

	// $ANTLR start "ruleSet"
	// CssGrammer.g3:248:1: ruleSet : sel= selectors LBRACE (decls= declarationSet )? RBRACE -> ^( RULE $sel ( $decls)? ) ;
	[GrammarRule("ruleSet")]
	private CssGrammerParser.ruleSet_return ruleSet()
	{
		Enter_ruleSet();
		EnterRule("ruleSet", 24);
		TraceIn("ruleSet", 24);
		CssGrammerParser.ruleSet_return retval = new CssGrammerParser.ruleSet_return();
		retval.Start = (CommonToken)input.LT(1);
		int ruleSet_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken LBRACE86=null;
		CommonToken RBRACE87=null;
		CssGrammerParser.selectors_return sel = default(CssGrammerParser.selectors_return);
		CssGrammerParser.declarationSet_return decls = default(CssGrammerParser.declarationSet_return);

		CommonTree LBRACE86_tree=null;
		CommonTree RBRACE87_tree=null;
		RewriteRuleITokenStream stream_LBRACE=new RewriteRuleITokenStream(adaptor,"token LBRACE");
		RewriteRuleITokenStream stream_RBRACE=new RewriteRuleITokenStream(adaptor,"token RBRACE");
		RewriteRuleSubtreeStream stream_selectors=new RewriteRuleSubtreeStream(adaptor,"rule selectors");
		RewriteRuleSubtreeStream stream_declarationSet=new RewriteRuleSubtreeStream(adaptor,"rule declarationSet");
		try { DebugEnterRule(GrammarFileName, "ruleSet");
		DebugLocation(248, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 24)) { return retval; }
			// CssGrammer.g3:249:5: (sel= selectors LBRACE (decls= declarationSet )? RBRACE -> ^( RULE $sel ( $decls)? ) )
			DebugEnterAlt(1);
			// CssGrammer.g3:249:7: sel= selectors LBRACE (decls= declarationSet )? RBRACE
			{
			DebugLocation(249, 10);
			PushFollow(Follow._selectors_in_ruleSet1634);
			sel=selectors();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_selectors.Add(sel.Tree);
			DebugLocation(249, 21);
			LBRACE86=(CommonToken)Match(input,LBRACE,Follow._LBRACE_in_ruleSet1636); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_LBRACE.Add(LBRACE86);

			DebugLocation(249, 28);
			// CssGrammer.g3:249:28: (decls= declarationSet )?
			int alt24=2;
			try { DebugEnterSubRule(24);
			try { DebugEnterDecision(24, decisionCanBacktrack[24]);
			int LA24_0 = input.LA(1);

			if ((LA24_0==IDENT))
			{
				alt24=1;
			}
			} finally { DebugExitDecision(24); }
			switch (alt24)
			{
			case 1:
				DebugEnterAlt(1);
				// CssGrammer.g3:249:29: decls= declarationSet
				{
				DebugLocation(249, 34);
				PushFollow(Follow._declarationSet_in_ruleSet1641);
				decls=declarationSet();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_declarationSet.Add(decls.Tree);

				}
				break;

			}
			} finally { DebugExitSubRule(24); }

			DebugLocation(249, 52);
			RBRACE87=(CommonToken)Match(input,RBRACE,Follow._RBRACE_in_ruleSet1645); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_RBRACE.Add(RBRACE87);



			{
			// AST REWRITE
			// elements: sel, decls
			// token labels: 
			// rule labels: sel, decls, retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_sel=new RewriteRuleSubtreeStream(adaptor,"rule sel",sel!=null?sel.Tree:null);
			RewriteRuleSubtreeStream stream_decls=new RewriteRuleSubtreeStream(adaptor,"rule decls",decls!=null?decls.Tree:null);
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 250:9: -> ^( RULE $sel ( $decls)? )
			{
				DebugLocation(250, 12);
				// CssGrammer.g3:250:12: ^( RULE $sel ( $decls)? )
				{
				CommonTree root_1 = (CommonTree)adaptor.Nil();
				DebugLocation(250, 14);
				root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(RULE, "RULE"), root_1);

				DebugLocation(250, 20);
				adaptor.AddChild(root_1, stream_sel.NextTree());
				DebugLocation(250, 25);
				// CssGrammer.g3:250:25: ( $decls)?
				if ( stream_decls.HasNext )
				{
					DebugLocation(250, 25);
					adaptor.AddChild(root_1, stream_decls.NextTree());

				}
				stream_decls.Reset();

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
			TraceOut("ruleSet", 24);
			LeaveRule("ruleSet", 24);
			Leave_ruleSet();
			if (state.backtracking > 0) { Memoize(input, 24, ruleSet_StartIndex); }
		}
		DebugLocation(251, 4);
		} finally { DebugExitRule(GrammarFileName, "ruleSet"); }
		return retval;

	}
	// $ANTLR end "ruleSet"

	public class selectors_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_selectors();
	partial void Leave_selectors();

	// $ANTLR start "selectors"
	// CssGrammer.g3:253:1: selectors : ( selector ( COMMA selector )* -> ^( SELECTORS ( selector )+ ) | FONT_FACE );
	[GrammarRule("selectors")]
	private CssGrammerParser.selectors_return selectors()
	{
		Enter_selectors();
		EnterRule("selectors", 25);
		TraceIn("selectors", 25);
		CssGrammerParser.selectors_return retval = new CssGrammerParser.selectors_return();
		retval.Start = (CommonToken)input.LT(1);
		int selectors_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken COMMA89=null;
		CommonToken FONT_FACE91=null;
		CssGrammerParser.selector_return selector88 = default(CssGrammerParser.selector_return);
		CssGrammerParser.selector_return selector90 = default(CssGrammerParser.selector_return);

		CommonTree COMMA89_tree=null;
		CommonTree FONT_FACE91_tree=null;
		RewriteRuleITokenStream stream_COMMA=new RewriteRuleITokenStream(adaptor,"token COMMA");
		RewriteRuleSubtreeStream stream_selector=new RewriteRuleSubtreeStream(adaptor,"rule selector");
		try { DebugEnterRule(GrammarFileName, "selectors");
		DebugLocation(253, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 25)) { return retval; }
			// CssGrammer.g3:254:5: ( selector ( COMMA selector )* -> ^( SELECTORS ( selector )+ ) | FONT_FACE )
			int alt26=2;
			try { DebugEnterDecision(26, decisionCanBacktrack[26]);
			int LA26_0 = input.LA(1);

			if ((LA26_0==COLON||(LA26_0>=DOT && LA26_0<=DOUBLE_COLON)||LA26_0==HASH||LA26_0==IDENT||LA26_0==LBRACKET||LA26_0==STAR))
			{
				alt26=1;
			}
			else if ((LA26_0==FONT_FACE))
			{
				alt26=2;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return retval;}
				NoViableAltException nvae = new NoViableAltException("", 26, 0, input);

				DebugRecognitionException(nvae);
				throw nvae;
			}
			} finally { DebugExitDecision(26); }
			switch (alt26)
			{
			case 1:
				DebugEnterAlt(1);
				// CssGrammer.g3:254:7: selector ( COMMA selector )*
				{
				DebugLocation(254, 7);
				PushFollow(Follow._selector_in_selectors1683);
				selector88=selector();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_selector.Add(selector88.Tree);
				DebugLocation(254, 16);
				// CssGrammer.g3:254:16: ( COMMA selector )*
				try { DebugEnterSubRule(25);
				while (true)
				{
					int alt25=2;
					try { DebugEnterDecision(25, decisionCanBacktrack[25]);
					int LA25_0 = input.LA(1);

					if ((LA25_0==COMMA))
					{
						alt25=1;
					}


					} finally { DebugExitDecision(25); }
					switch ( alt25 )
					{
					case 1:
						DebugEnterAlt(1);
						// CssGrammer.g3:254:17: COMMA selector
						{
						DebugLocation(254, 17);
						COMMA89=(CommonToken)Match(input,COMMA,Follow._COMMA_in_selectors1686); if (state.failed) return retval; 
						if ( state.backtracking == 0 ) stream_COMMA.Add(COMMA89);

						DebugLocation(254, 23);
						PushFollow(Follow._selector_in_selectors1688);
						selector90=selector();
						PopFollow();
						if (state.failed) return retval;
						if ( state.backtracking == 0 ) stream_selector.Add(selector90.Tree);

						}
						break;

					default:
						goto loop25;
					}
				}

				loop25:
					;

				} finally { DebugExitSubRule(25); }



				{
				// AST REWRITE
				// elements: selector
				// token labels: 
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.Tree = root_0;
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

				root_0 = (CommonTree)adaptor.Nil();
				// 255:9: -> ^( SELECTORS ( selector )+ )
				{
					DebugLocation(255, 12);
					// CssGrammer.g3:255:12: ^( SELECTORS ( selector )+ )
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(255, 14);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(SELECTORS, "SELECTORS"), root_1);

					DebugLocation(255, 24);
					if ( !(stream_selector.HasNext) )
					{
						throw new RewriteEarlyExitException();
					}
					while ( stream_selector.HasNext )
					{
						DebugLocation(255, 24);
						adaptor.AddChild(root_1, stream_selector.NextTree());

					}
					stream_selector.Reset();

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
				// CssGrammer.g3:256:7: FONT_FACE
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(256, 7);
				FONT_FACE91=(CommonToken)Match(input,FONT_FACE,Follow._FONT_FACE_in_selectors1715); if (state.failed) return retval;
				if ( state.backtracking==0 ) {
				FONT_FACE91_tree = (CommonTree)adaptor.Create(FONT_FACE91);
				adaptor.AddChild(root_0, FONT_FACE91_tree);
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
			TraceOut("selectors", 25);
			LeaveRule("selectors", 25);
			Leave_selectors();
			if (state.backtracking > 0) { Memoize(input, 25, selectors_StartIndex); }
		}
		DebugLocation(257, 4);
		} finally { DebugExitRule(GrammarFileName, "selectors"); }
		return retval;

	}
	// $ANTLR end "selectors"

	public class declarationSet_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_declarationSet();
	partial void Leave_declarationSet();

	// $ANTLR start "declarationSet"
	// CssGrammer.g3:260:1: public declarationSet : declaration SEMI ( declaration SEMI )* -> ( declaration )+ ;
	[GrammarRule("declarationSet")]
	public CssGrammerParser.declarationSet_return declarationSet()
	{
		Enter_declarationSet();
		EnterRule("declarationSet", 26);
		TraceIn("declarationSet", 26);
		CssGrammerParser.declarationSet_return retval = new CssGrammerParser.declarationSet_return();
		retval.Start = (CommonToken)input.LT(1);
		int declarationSet_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken SEMI93=null;
		CommonToken SEMI95=null;
		CssGrammerParser.declaration_return declaration92 = default(CssGrammerParser.declaration_return);
		CssGrammerParser.declaration_return declaration94 = default(CssGrammerParser.declaration_return);

		CommonTree SEMI93_tree=null;
		CommonTree SEMI95_tree=null;
		RewriteRuleITokenStream stream_SEMI=new RewriteRuleITokenStream(adaptor,"token SEMI");
		RewriteRuleSubtreeStream stream_declaration=new RewriteRuleSubtreeStream(adaptor,"rule declaration");
		try { DebugEnterRule(GrammarFileName, "declarationSet");
		DebugLocation(260, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 26)) { return retval; }
			// CssGrammer.g3:261:5: ( declaration SEMI ( declaration SEMI )* -> ( declaration )+ )
			DebugEnterAlt(1);
			// CssGrammer.g3:261:7: declaration SEMI ( declaration SEMI )*
			{
			DebugLocation(261, 7);
			PushFollow(Follow._declaration_in_declarationSet1734);
			declaration92=declaration();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_declaration.Add(declaration92.Tree);
			DebugLocation(261, 19);
			SEMI93=(CommonToken)Match(input,SEMI,Follow._SEMI_in_declarationSet1736); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_SEMI.Add(SEMI93);

			DebugLocation(261, 24);
			// CssGrammer.g3:261:24: ( declaration SEMI )*
			try { DebugEnterSubRule(27);
			while (true)
			{
				int alt27=2;
				try { DebugEnterDecision(27, decisionCanBacktrack[27]);
				int LA27_0 = input.LA(1);

				if ((LA27_0==IDENT))
				{
					alt27=1;
				}


				} finally { DebugExitDecision(27); }
				switch ( alt27 )
				{
				case 1:
					DebugEnterAlt(1);
					// CssGrammer.g3:261:25: declaration SEMI
					{
					DebugLocation(261, 25);
					PushFollow(Follow._declaration_in_declarationSet1739);
					declaration94=declaration();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_declaration.Add(declaration94.Tree);
					DebugLocation(261, 37);
					SEMI95=(CommonToken)Match(input,SEMI,Follow._SEMI_in_declarationSet1741); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_SEMI.Add(SEMI95);


					}
					break;

				default:
					goto loop27;
				}
			}

			loop27:
				;

			} finally { DebugExitSubRule(27); }



			{
			// AST REWRITE
			// elements: declaration
			// token labels: 
			// rule labels: retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 261:44: -> ( declaration )+
			{
				DebugLocation(261, 47);
				if ( !(stream_declaration.HasNext) )
				{
					throw new RewriteEarlyExitException();
				}
				while ( stream_declaration.HasNext )
				{
					DebugLocation(261, 47);
					adaptor.AddChild(root_0, stream_declaration.NextTree());

				}
				stream_declaration.Reset();

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
			TraceOut("declarationSet", 26);
			LeaveRule("declarationSet", 26);
			Leave_declarationSet();
			if (state.backtracking > 0) { Memoize(input, 26, declarationSet_StartIndex); }
		}
		DebugLocation(262, 4);
		} finally { DebugExitRule(GrammarFileName, "declarationSet"); }
		return retval;

	}
	// $ANTLR end "declarationSet"

	public class selector_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_selector();
	partial void Leave_selector();

	// $ANTLR start "selector"
	// CssGrammer.g3:264:1: selector : simpleSelector ( moreSelectors )* -> ^( SELECTOR simpleSelector ( moreSelectors )* ) ;
	[GrammarRule("selector")]
	private CssGrammerParser.selector_return selector()
	{
		Enter_selector();
		EnterRule("selector", 27);
		TraceIn("selector", 27);
		CssGrammerParser.selector_return retval = new CssGrammerParser.selector_return();
		retval.Start = (CommonToken)input.LT(1);
		int selector_StartIndex = input.Index;
		CommonTree root_0 = null;

		CssGrammerParser.simpleSelector_return simpleSelector96 = default(CssGrammerParser.simpleSelector_return);
		CssGrammerParser.moreSelectors_return moreSelectors97 = default(CssGrammerParser.moreSelectors_return);

		RewriteRuleSubtreeStream stream_simpleSelector=new RewriteRuleSubtreeStream(adaptor,"rule simpleSelector");
		RewriteRuleSubtreeStream stream_moreSelectors=new RewriteRuleSubtreeStream(adaptor,"rule moreSelectors");
		try { DebugEnterRule(GrammarFileName, "selector");
		DebugLocation(264, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 27)) { return retval; }
			// CssGrammer.g3:265:5: ( simpleSelector ( moreSelectors )* -> ^( SELECTOR simpleSelector ( moreSelectors )* ) )
			DebugEnterAlt(1);
			// CssGrammer.g3:265:6: simpleSelector ( moreSelectors )*
			{
			DebugLocation(265, 6);
			PushFollow(Follow._simpleSelector_in_selector1768);
			simpleSelector96=simpleSelector();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_simpleSelector.Add(simpleSelector96.Tree);
			DebugLocation(265, 21);
			// CssGrammer.g3:265:21: ( moreSelectors )*
			try { DebugEnterSubRule(28);
			while (true)
			{
				int alt28=2;
				try { DebugEnterDecision(28, decisionCanBacktrack[28]);
				int LA28_0 = input.LA(1);

				if ((LA28_0==COLON||(LA28_0>=DOT && LA28_0<=DOUBLE_COLON)||LA28_0==GREATER||LA28_0==HASH||LA28_0==IDENT||LA28_0==LBRACKET||LA28_0==PLUS||LA28_0==STAR||LA28_0==152))
				{
					alt28=1;
				}


				} finally { DebugExitDecision(28); }
				switch ( alt28 )
				{
				case 1:
					DebugEnterAlt(1);
					// CssGrammer.g3:265:21: moreSelectors
					{
					DebugLocation(265, 21);
					PushFollow(Follow._moreSelectors_in_selector1770);
					moreSelectors97=moreSelectors();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_moreSelectors.Add(moreSelectors97.Tree);

					}
					break;

				default:
					goto loop28;
				}
			}

			loop28:
				;

			} finally { DebugExitSubRule(28); }



			{
			// AST REWRITE
			// elements: simpleSelector, moreSelectors
			// token labels: 
			// rule labels: retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 266:9: -> ^( SELECTOR simpleSelector ( moreSelectors )* )
			{
				DebugLocation(266, 12);
				// CssGrammer.g3:266:12: ^( SELECTOR simpleSelector ( moreSelectors )* )
				{
				CommonTree root_1 = (CommonTree)adaptor.Nil();
				DebugLocation(266, 14);
				root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(SELECTOR, "SELECTOR"), root_1);

				DebugLocation(266, 23);
				adaptor.AddChild(root_1, stream_simpleSelector.NextTree());
				DebugLocation(266, 38);
				// CssGrammer.g3:266:38: ( moreSelectors )*
				while ( stream_moreSelectors.HasNext )
				{
					DebugLocation(266, 38);
					adaptor.AddChild(root_1, stream_moreSelectors.NextTree());

				}
				stream_moreSelectors.Reset();

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
			TraceOut("selector", 27);
			LeaveRule("selector", 27);
			Leave_selector();
			if (state.backtracking > 0) { Memoize(input, 27, selector_StartIndex); }
		}
		DebugLocation(267, 4);
		} finally { DebugExitRule(GrammarFileName, "selector"); }
		return retval;

	}
	// $ANTLR end "selector"

	public class moreSelectors_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_moreSelectors();
	partial void Leave_moreSelectors();

	// $ANTLR start "moreSelectors"
	// CssGrammer.g3:269:1: moreSelectors : combinator simpleSelector -> ^( SELECTOR_OP combinator simpleSelector ) ;
	[GrammarRule("moreSelectors")]
	private CssGrammerParser.moreSelectors_return moreSelectors()
	{
		Enter_moreSelectors();
		EnterRule("moreSelectors", 28);
		TraceIn("moreSelectors", 28);
		CssGrammerParser.moreSelectors_return retval = new CssGrammerParser.moreSelectors_return();
		retval.Start = (CommonToken)input.LT(1);
		int moreSelectors_StartIndex = input.Index;
		CommonTree root_0 = null;

		CssGrammerParser.combinator_return combinator98 = default(CssGrammerParser.combinator_return);
		CssGrammerParser.simpleSelector_return simpleSelector99 = default(CssGrammerParser.simpleSelector_return);

		RewriteRuleSubtreeStream stream_combinator=new RewriteRuleSubtreeStream(adaptor,"rule combinator");
		RewriteRuleSubtreeStream stream_simpleSelector=new RewriteRuleSubtreeStream(adaptor,"rule simpleSelector");
		try { DebugEnterRule(GrammarFileName, "moreSelectors");
		DebugLocation(269, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 28)) { return retval; }
			// CssGrammer.g3:270:5: ( combinator simpleSelector -> ^( SELECTOR_OP combinator simpleSelector ) )
			DebugEnterAlt(1);
			// CssGrammer.g3:270:7: combinator simpleSelector
			{
			DebugLocation(270, 7);
			PushFollow(Follow._combinator_in_moreSelectors1807);
			combinator98=combinator();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_combinator.Add(combinator98.Tree);
			DebugLocation(270, 18);
			PushFollow(Follow._simpleSelector_in_moreSelectors1809);
			simpleSelector99=simpleSelector();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_simpleSelector.Add(simpleSelector99.Tree);


			{
			// AST REWRITE
			// elements: combinator, simpleSelector
			// token labels: 
			// rule labels: retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 271:9: -> ^( SELECTOR_OP combinator simpleSelector )
			{
				DebugLocation(271, 12);
				// CssGrammer.g3:271:12: ^( SELECTOR_OP combinator simpleSelector )
				{
				CommonTree root_1 = (CommonTree)adaptor.Nil();
				DebugLocation(271, 14);
				root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(SELECTOR_OP, "SELECTOR_OP"), root_1);

				DebugLocation(271, 26);
				adaptor.AddChild(root_1, stream_combinator.NextTree());
				DebugLocation(271, 37);
				adaptor.AddChild(root_1, stream_simpleSelector.NextTree());

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
			TraceOut("moreSelectors", 28);
			LeaveRule("moreSelectors", 28);
			Leave_moreSelectors();
			if (state.backtracking > 0) { Memoize(input, 28, moreSelectors_StartIndex); }
		}
		DebugLocation(272, 4);
		} finally { DebugExitRule(GrammarFileName, "moreSelectors"); }
		return retval;

	}
	// $ANTLR end "moreSelectors"

	public class simpleSelector_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_simpleSelector();
	partial void Leave_simpleSelector();

	// $ANTLR start "simpleSelector"
	// CssGrammer.g3:274:1: simpleSelector : ( elementName ( ( esPred )=> elementSubsequent )* -> ^( SIMPLE_SEL elementName ( elementSubsequent )* ) | ( ( esPred )=> elementSubsequent )+ -> ^( SIMPLE_SEL ( elementSubsequent )+ ) );
	[GrammarRule("simpleSelector")]
	private CssGrammerParser.simpleSelector_return simpleSelector()
	{
		Enter_simpleSelector();
		EnterRule("simpleSelector", 29);
		TraceIn("simpleSelector", 29);
		CssGrammerParser.simpleSelector_return retval = new CssGrammerParser.simpleSelector_return();
		retval.Start = (CommonToken)input.LT(1);
		int simpleSelector_StartIndex = input.Index;
		CommonTree root_0 = null;

		CssGrammerParser.elementName_return elementName100 = default(CssGrammerParser.elementName_return);
		CssGrammerParser.elementSubsequent_return elementSubsequent101 = default(CssGrammerParser.elementSubsequent_return);
		CssGrammerParser.elementSubsequent_return elementSubsequent102 = default(CssGrammerParser.elementSubsequent_return);

		RewriteRuleSubtreeStream stream_elementName=new RewriteRuleSubtreeStream(adaptor,"rule elementName");
		RewriteRuleSubtreeStream stream_elementSubsequent=new RewriteRuleSubtreeStream(adaptor,"rule elementSubsequent");
		try { DebugEnterRule(GrammarFileName, "simpleSelector");
		DebugLocation(274, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 29)) { return retval; }
			// CssGrammer.g3:275:5: ( elementName ( ( esPred )=> elementSubsequent )* -> ^( SIMPLE_SEL elementName ( elementSubsequent )* ) | ( ( esPred )=> elementSubsequent )+ -> ^( SIMPLE_SEL ( elementSubsequent )+ ) )
			int alt31=2;
			try { DebugEnterDecision(31, decisionCanBacktrack[31]);
			int LA31_0 = input.LA(1);

			if ((LA31_0==IDENT||LA31_0==STAR))
			{
				alt31=1;
			}
			else if ((LA31_0==COLON||(LA31_0>=DOT && LA31_0<=DOUBLE_COLON)||LA31_0==HASH||LA31_0==LBRACKET))
			{
				alt31=2;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return retval;}
				NoViableAltException nvae = new NoViableAltException("", 31, 0, input);

				DebugRecognitionException(nvae);
				throw nvae;
			}
			} finally { DebugExitDecision(31); }
			switch (alt31)
			{
			case 1:
				DebugEnterAlt(1);
				// CssGrammer.g3:275:7: elementName ( ( esPred )=> elementSubsequent )*
				{
				DebugLocation(275, 7);
				PushFollow(Follow._elementName_in_simpleSelector1844);
				elementName100=elementName();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_elementName.Add(elementName100.Tree);
				DebugLocation(275, 19);
				// CssGrammer.g3:275:19: ( ( esPred )=> elementSubsequent )*
				try { DebugEnterSubRule(29);
				while (true)
				{
					int alt29=2;
					try { DebugEnterDecision(29, decisionCanBacktrack[29]);
					try
					{
						alt29 = dfa29.Predict(input);
					}
					catch (NoViableAltException nvae)
					{
						DebugRecognitionException(nvae);
						throw;
					}
					} finally { DebugExitDecision(29); }
					switch ( alt29 )
					{
					case 1:
						DebugEnterAlt(1);
						// CssGrammer.g3:275:20: ( esPred )=> elementSubsequent
						{
						DebugLocation(275, 30);
						PushFollow(Follow._elementSubsequent_in_simpleSelector1851);
						elementSubsequent101=elementSubsequent();
						PopFollow();
						if (state.failed) return retval;
						if ( state.backtracking == 0 ) stream_elementSubsequent.Add(elementSubsequent101.Tree);

						}
						break;

					default:
						goto loop29;
					}
				}

				loop29:
					;

				} finally { DebugExitSubRule(29); }



				{
				// AST REWRITE
				// elements: elementName, elementSubsequent
				// token labels: 
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.Tree = root_0;
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

				root_0 = (CommonTree)adaptor.Nil();
				// 276:9: -> ^( SIMPLE_SEL elementName ( elementSubsequent )* )
				{
					DebugLocation(276, 12);
					// CssGrammer.g3:276:12: ^( SIMPLE_SEL elementName ( elementSubsequent )* )
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(276, 14);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(SIMPLE_SEL, "SIMPLE_SEL"), root_1);

					DebugLocation(276, 25);
					adaptor.AddChild(root_1, stream_elementName.NextTree());
					DebugLocation(276, 37);
					// CssGrammer.g3:276:37: ( elementSubsequent )*
					while ( stream_elementSubsequent.HasNext )
					{
						DebugLocation(276, 37);
						adaptor.AddChild(root_1, stream_elementSubsequent.NextTree());

					}
					stream_elementSubsequent.Reset();

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
				// CssGrammer.g3:277:7: ( ( esPred )=> elementSubsequent )+
				{
				DebugLocation(277, 7);
				// CssGrammer.g3:277:7: ( ( esPred )=> elementSubsequent )+
				int cnt30=0;
				try { DebugEnterSubRule(30);
				while (true)
				{
					int alt30=2;
					try { DebugEnterDecision(30, decisionCanBacktrack[30]);
					switch (input.LA(1))
					{
					case HASH:
						{
						int LA30_2 = input.LA(2);

						if ((EvaluatePredicate(synpred44_CssGrammer_fragment)))
						{
							alt30=1;
						}


						}
						break;
					case DOT:
						{
						int LA30_3 = input.LA(2);

						if ((EvaluatePredicate(synpred44_CssGrammer_fragment)))
						{
							alt30=1;
						}


						}
						break;
					case LBRACKET:
						{
						int LA30_4 = input.LA(2);

						if ((EvaluatePredicate(synpred44_CssGrammer_fragment)))
						{
							alt30=1;
						}


						}
						break;
					case COLON:
					case DOUBLE_COLON:
						{
						int LA30_5 = input.LA(2);

						if ((EvaluatePredicate(synpred44_CssGrammer_fragment)))
						{
							alt30=1;
						}


						}
						break;

					}

					} finally { DebugExitDecision(30); }
					switch (alt30)
					{
					case 1:
						DebugEnterAlt(1);
						// CssGrammer.g3:277:8: ( esPred )=> elementSubsequent
						{
						DebugLocation(277, 18);
						PushFollow(Follow._elementSubsequent_in_simpleSelector1885);
						elementSubsequent102=elementSubsequent();
						PopFollow();
						if (state.failed) return retval;
						if ( state.backtracking == 0 ) stream_elementSubsequent.Add(elementSubsequent102.Tree);

						}
						break;

					default:
						if (cnt30 >= 1)
							goto loop30;

						if (state.backtracking>0) {state.failed=true; return retval;}
						EarlyExitException eee30 = new EarlyExitException( 30, input );
						DebugRecognitionException(eee30);
						throw eee30;
					}
					cnt30++;
				}
				loop30:
					;

				} finally { DebugExitSubRule(30); }



				{
				// AST REWRITE
				// elements: elementSubsequent
				// token labels: 
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.Tree = root_0;
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

				root_0 = (CommonTree)adaptor.Nil();
				// 278:9: -> ^( SIMPLE_SEL ( elementSubsequent )+ )
				{
					DebugLocation(278, 12);
					// CssGrammer.g3:278:12: ^( SIMPLE_SEL ( elementSubsequent )+ )
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(278, 14);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(SIMPLE_SEL, "SIMPLE_SEL"), root_1);

					DebugLocation(278, 25);
					if ( !(stream_elementSubsequent.HasNext) )
					{
						throw new RewriteEarlyExitException();
					}
					while ( stream_elementSubsequent.HasNext )
					{
						DebugLocation(278, 25);
						adaptor.AddChild(root_1, stream_elementSubsequent.NextTree());

					}
					stream_elementSubsequent.Reset();

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
			TraceOut("simpleSelector", 29);
			LeaveRule("simpleSelector", 29);
			Leave_simpleSelector();
			if (state.backtracking > 0) { Memoize(input, 29, simpleSelector_StartIndex); }
		}
		DebugLocation(279, 4);
		} finally { DebugExitRule(GrammarFileName, "simpleSelector"); }
		return retval;

	}
	// $ANTLR end "simpleSelector"

	public class esPred_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_esPred();
	partial void Leave_esPred();

	// $ANTLR start "esPred"
	// CssGrammer.g3:281:1: esPred : ( HASH | DOT | LBRACKET | COLON | DOUBLE_COLON );
	[GrammarRule("esPred")]
	private CssGrammerParser.esPred_return esPred()
	{
		Enter_esPred();
		EnterRule("esPred", 30);
		TraceIn("esPred", 30);
		CssGrammerParser.esPred_return retval = new CssGrammerParser.esPred_return();
		retval.Start = (CommonToken)input.LT(1);
		int esPred_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken set103=null;

		CommonTree set103_tree=null;

		try { DebugEnterRule(GrammarFileName, "esPred");
		DebugLocation(281, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 30)) { return retval; }
			// CssGrammer.g3:282:5: ( HASH | DOT | LBRACKET | COLON | DOUBLE_COLON )
			DebugEnterAlt(1);
			// CssGrammer.g3:
			{
			root_0 = (CommonTree)adaptor.Nil();

			DebugLocation(282, 5);
			set103=(CommonToken)input.LT(1);
			if (input.LA(1)==COLON||(input.LA(1)>=DOT && input.LA(1)<=DOUBLE_COLON)||input.LA(1)==HASH||input.LA(1)==LBRACKET)
			{
				input.Consume();
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, (CommonTree)adaptor.Create(set103));
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
			TraceOut("esPred", 30);
			LeaveRule("esPred", 30);
			Leave_esPred();
			if (state.backtracking > 0) { Memoize(input, 30, esPred_StartIndex); }
		}
		DebugLocation(283, 4);
		} finally { DebugExitRule(GrammarFileName, "esPred"); }
		return retval;

	}
	// $ANTLR end "esPred"

	public class elementSubsequent_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_elementSubsequent();
	partial void Leave_elementSubsequent();

	// $ANTLR start "elementSubsequent"
	// CssGrammer.g3:285:1: elementSubsequent : ( HASH -> ^( ID HASH ) | cssClass | attrib | pseudo );
	[GrammarRule("elementSubsequent")]
	private CssGrammerParser.elementSubsequent_return elementSubsequent()
	{
		Enter_elementSubsequent();
		EnterRule("elementSubsequent", 31);
		TraceIn("elementSubsequent", 31);
		CssGrammerParser.elementSubsequent_return retval = new CssGrammerParser.elementSubsequent_return();
		retval.Start = (CommonToken)input.LT(1);
		int elementSubsequent_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken HASH104=null;
		CssGrammerParser.cssClass_return cssClass105 = default(CssGrammerParser.cssClass_return);
		CssGrammerParser.attrib_return attrib106 = default(CssGrammerParser.attrib_return);
		CssGrammerParser.pseudo_return pseudo107 = default(CssGrammerParser.pseudo_return);

		CommonTree HASH104_tree=null;
		RewriteRuleITokenStream stream_HASH=new RewriteRuleITokenStream(adaptor,"token HASH");

		try { DebugEnterRule(GrammarFileName, "elementSubsequent");
		DebugLocation(285, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 31)) { return retval; }
			// CssGrammer.g3:286:5: ( HASH -> ^( ID HASH ) | cssClass | attrib | pseudo )
			int alt32=4;
			try { DebugEnterDecision(32, decisionCanBacktrack[32]);
			switch (input.LA(1))
			{
			case HASH:
				{
				alt32=1;
				}
				break;
			case DOT:
				{
				alt32=2;
				}
				break;
			case LBRACKET:
				{
				alt32=3;
				}
				break;
			case COLON:
			case DOUBLE_COLON:
				{
				alt32=4;
				}
				break;
			default:
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 32, 0, input);

					DebugRecognitionException(nvae);
					throw nvae;
				}
			}

			} finally { DebugExitDecision(32); }
			switch (alt32)
			{
			case 1:
				DebugEnterAlt(1);
				// CssGrammer.g3:286:7: HASH
				{
				DebugLocation(286, 7);
				HASH104=(CommonToken)Match(input,HASH,Follow._HASH_in_elementSubsequent1962); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_HASH.Add(HASH104);



				{
				// AST REWRITE
				// elements: HASH
				// token labels: 
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.Tree = root_0;
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

				root_0 = (CommonTree)adaptor.Nil();
				// 286:12: -> ^( ID HASH )
				{
					DebugLocation(286, 15);
					// CssGrammer.g3:286:15: ^( ID HASH )
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(286, 17);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(ID, "ID"), root_1);

					DebugLocation(286, 20);
					adaptor.AddChild(root_1, stream_HASH.NextNode());

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
				// CssGrammer.g3:287:7: cssClass
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(287, 7);
				PushFollow(Follow._cssClass_in_elementSubsequent1978);
				cssClass105=cssClass();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, cssClass105.Tree);

				}
				break;
			case 3:
				DebugEnterAlt(3);
				// CssGrammer.g3:288:7: attrib
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(288, 7);
				PushFollow(Follow._attrib_in_elementSubsequent1986);
				attrib106=attrib();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, attrib106.Tree);

				}
				break;
			case 4:
				DebugEnterAlt(4);
				// CssGrammer.g3:289:7: pseudo
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(289, 7);
				PushFollow(Follow._pseudo_in_elementSubsequent1994);
				pseudo107=pseudo();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, pseudo107.Tree);

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
			TraceOut("elementSubsequent", 31);
			LeaveRule("elementSubsequent", 31);
			Leave_elementSubsequent();
			if (state.backtracking > 0) { Memoize(input, 31, elementSubsequent_StartIndex); }
		}
		DebugLocation(290, 4);
		} finally { DebugExitRule(GrammarFileName, "elementSubsequent"); }
		return retval;

	}
	// $ANTLR end "elementSubsequent"

	public class cssClass_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_cssClass();
	partial void Leave_cssClass();

	// $ANTLR start "cssClass"
	// CssGrammer.g3:292:1: cssClass : DOT IDENT -> ^( CLASS IDENT ) ;
	[GrammarRule("cssClass")]
	private CssGrammerParser.cssClass_return cssClass()
	{
		Enter_cssClass();
		EnterRule("cssClass", 32);
		TraceIn("cssClass", 32);
		CssGrammerParser.cssClass_return retval = new CssGrammerParser.cssClass_return();
		retval.Start = (CommonToken)input.LT(1);
		int cssClass_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken DOT108=null;
		CommonToken IDENT109=null;

		CommonTree DOT108_tree=null;
		CommonTree IDENT109_tree=null;
		RewriteRuleITokenStream stream_DOT=new RewriteRuleITokenStream(adaptor,"token DOT");
		RewriteRuleITokenStream stream_IDENT=new RewriteRuleITokenStream(adaptor,"token IDENT");

		try { DebugEnterRule(GrammarFileName, "cssClass");
		DebugLocation(292, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 32)) { return retval; }
			// CssGrammer.g3:293:5: ( DOT IDENT -> ^( CLASS IDENT ) )
			DebugEnterAlt(1);
			// CssGrammer.g3:293:7: DOT IDENT
			{
			DebugLocation(293, 7);
			DOT108=(CommonToken)Match(input,DOT,Follow._DOT_in_cssClass2015); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_DOT.Add(DOT108);

			DebugLocation(293, 11);
			IDENT109=(CommonToken)Match(input,IDENT,Follow._IDENT_in_cssClass2017); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_IDENT.Add(IDENT109);



			{
			// AST REWRITE
			// elements: IDENT
			// token labels: 
			// rule labels: retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 294:9: -> ^( CLASS IDENT )
			{
				DebugLocation(294, 12);
				// CssGrammer.g3:294:12: ^( CLASS IDENT )
				{
				CommonTree root_1 = (CommonTree)adaptor.Nil();
				DebugLocation(294, 14);
				root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(CLASS, "CLASS"), root_1);

				DebugLocation(294, 20);
				adaptor.AddChild(root_1, stream_IDENT.NextNode());

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
			TraceOut("cssClass", 32);
			LeaveRule("cssClass", 32);
			Leave_cssClass();
			if (state.backtracking > 0) { Memoize(input, 32, cssClass_StartIndex); }
		}
		DebugLocation(295, 4);
		} finally { DebugExitRule(GrammarFileName, "cssClass"); }
		return retval;

	}
	// $ANTLR end "cssClass"

	public class elementName_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_elementName();
	partial void Leave_elementName();

	// $ANTLR start "elementName"
	// CssGrammer.g3:297:1: elementName : ( IDENT -> ^( TAG IDENT ) | STAR -> ^( ALL STAR ) );
	[GrammarRule("elementName")]
	private CssGrammerParser.elementName_return elementName()
	{
		Enter_elementName();
		EnterRule("elementName", 33);
		TraceIn("elementName", 33);
		CssGrammerParser.elementName_return retval = new CssGrammerParser.elementName_return();
		retval.Start = (CommonToken)input.LT(1);
		int elementName_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken IDENT110=null;
		CommonToken STAR111=null;

		CommonTree IDENT110_tree=null;
		CommonTree STAR111_tree=null;
		RewriteRuleITokenStream stream_IDENT=new RewriteRuleITokenStream(adaptor,"token IDENT");
		RewriteRuleITokenStream stream_STAR=new RewriteRuleITokenStream(adaptor,"token STAR");

		try { DebugEnterRule(GrammarFileName, "elementName");
		DebugLocation(297, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 33)) { return retval; }
			// CssGrammer.g3:298:5: ( IDENT -> ^( TAG IDENT ) | STAR -> ^( ALL STAR ) )
			int alt33=2;
			try { DebugEnterDecision(33, decisionCanBacktrack[33]);
			int LA33_0 = input.LA(1);

			if ((LA33_0==IDENT))
			{
				alt33=1;
			}
			else if ((LA33_0==STAR))
			{
				alt33=2;
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return retval;}
				NoViableAltException nvae = new NoViableAltException("", 33, 0, input);

				DebugRecognitionException(nvae);
				throw nvae;
			}
			} finally { DebugExitDecision(33); }
			switch (alt33)
			{
			case 1:
				DebugEnterAlt(1);
				// CssGrammer.g3:298:7: IDENT
				{
				DebugLocation(298, 7);
				IDENT110=(CommonToken)Match(input,IDENT,Follow._IDENT_in_elementName2054); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_IDENT.Add(IDENT110);



				{
				// AST REWRITE
				// elements: IDENT
				// token labels: 
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.Tree = root_0;
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

				root_0 = (CommonTree)adaptor.Nil();
				// 298:13: -> ^( TAG IDENT )
				{
					DebugLocation(298, 16);
					// CssGrammer.g3:298:16: ^( TAG IDENT )
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(298, 18);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(TAG, "TAG"), root_1);

					DebugLocation(298, 22);
					adaptor.AddChild(root_1, stream_IDENT.NextNode());

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
				// CssGrammer.g3:299:7: STAR
				{
				DebugLocation(299, 7);
				STAR111=(CommonToken)Match(input,STAR,Follow._STAR_in_elementName2070); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_STAR.Add(STAR111);



				{
				// AST REWRITE
				// elements: STAR
				// token labels: 
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.Tree = root_0;
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

				root_0 = (CommonTree)adaptor.Nil();
				// 299:12: -> ^( ALL STAR )
				{
					DebugLocation(299, 15);
					// CssGrammer.g3:299:15: ^( ALL STAR )
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(299, 17);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(ALL, "ALL"), root_1);

					DebugLocation(299, 21);
					adaptor.AddChild(root_1, stream_STAR.NextNode());

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
			TraceOut("elementName", 33);
			LeaveRule("elementName", 33);
			Leave_elementName();
			if (state.backtracking > 0) { Memoize(input, 33, elementName_StartIndex); }
		}
		DebugLocation(300, 4);
		} finally { DebugExitRule(GrammarFileName, "elementName"); }
		return retval;

	}
	// $ANTLR end "elementName"

	public class attrib_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_attrib();
	partial void Leave_attrib();

	// $ANTLR start "attrib"
	// CssGrammer.g3:302:1: attrib : LBRACKET IDENT ( attribVal )? RBRACKET -> ^( ATTRIB IDENT ( attribVal )? ) ;
	[GrammarRule("attrib")]
	private CssGrammerParser.attrib_return attrib()
	{
		Enter_attrib();
		EnterRule("attrib", 34);
		TraceIn("attrib", 34);
		CssGrammerParser.attrib_return retval = new CssGrammerParser.attrib_return();
		retval.Start = (CommonToken)input.LT(1);
		int attrib_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken LBRACKET112=null;
		CommonToken IDENT113=null;
		CommonToken RBRACKET115=null;
		CssGrammerParser.attribVal_return attribVal114 = default(CssGrammerParser.attribVal_return);

		CommonTree LBRACKET112_tree=null;
		CommonTree IDENT113_tree=null;
		CommonTree RBRACKET115_tree=null;
		RewriteRuleITokenStream stream_LBRACKET=new RewriteRuleITokenStream(adaptor,"token LBRACKET");
		RewriteRuleITokenStream stream_IDENT=new RewriteRuleITokenStream(adaptor,"token IDENT");
		RewriteRuleITokenStream stream_RBRACKET=new RewriteRuleITokenStream(adaptor,"token RBRACKET");
		RewriteRuleSubtreeStream stream_attribVal=new RewriteRuleSubtreeStream(adaptor,"rule attribVal");
		try { DebugEnterRule(GrammarFileName, "attrib");
		DebugLocation(302, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 34)) { return retval; }
			// CssGrammer.g3:303:5: ( LBRACKET IDENT ( attribVal )? RBRACKET -> ^( ATTRIB IDENT ( attribVal )? ) )
			DebugEnterAlt(1);
			// CssGrammer.g3:303:7: LBRACKET IDENT ( attribVal )? RBRACKET
			{
			DebugLocation(303, 7);
			LBRACKET112=(CommonToken)Match(input,LBRACKET,Follow._LBRACKET_in_attrib2099); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_LBRACKET.Add(LBRACKET112);

			DebugLocation(303, 16);
			IDENT113=(CommonToken)Match(input,IDENT,Follow._IDENT_in_attrib2101); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_IDENT.Add(IDENT113);

			DebugLocation(303, 22);
			// CssGrammer.g3:303:22: ( attribVal )?
			int alt34=2;
			try { DebugEnterSubRule(34);
			try { DebugEnterDecision(34, decisionCanBacktrack[34]);
			int LA34_0 = input.LA(1);

			if ((LA34_0==ENDS_WITH||(LA34_0>=INCLUDES && LA34_0<=INCLUDES_WORD)||LA34_0==OPEQ||(LA34_0>=STARTS_WITH && LA34_0<=STARTS_WITH_WORD)))
			{
				alt34=1;
			}
			} finally { DebugExitDecision(34); }
			switch (alt34)
			{
			case 1:
				DebugEnterAlt(1);
				// CssGrammer.g3:303:22: attribVal
				{
				DebugLocation(303, 22);
				PushFollow(Follow._attribVal_in_attrib2103);
				attribVal114=attribVal();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_attribVal.Add(attribVal114.Tree);

				}
				break;

			}
			} finally { DebugExitSubRule(34); }

			DebugLocation(303, 34);
			RBRACKET115=(CommonToken)Match(input,RBRACKET,Follow._RBRACKET_in_attrib2107); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_RBRACKET.Add(RBRACKET115);



			{
			// AST REWRITE
			// elements: IDENT, attribVal
			// token labels: 
			// rule labels: retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 304:9: -> ^( ATTRIB IDENT ( attribVal )? )
			{
				DebugLocation(304, 12);
				// CssGrammer.g3:304:12: ^( ATTRIB IDENT ( attribVal )? )
				{
				CommonTree root_1 = (CommonTree)adaptor.Nil();
				DebugLocation(304, 14);
				root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(ATTRIB, "ATTRIB"), root_1);

				DebugLocation(304, 21);
				adaptor.AddChild(root_1, stream_IDENT.NextNode());
				DebugLocation(304, 27);
				// CssGrammer.g3:304:27: ( attribVal )?
				if ( stream_attribVal.HasNext )
				{
					DebugLocation(304, 27);
					adaptor.AddChild(root_1, stream_attribVal.NextTree());

				}
				stream_attribVal.Reset();

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
			TraceOut("attrib", 34);
			LeaveRule("attrib", 34);
			Leave_attrib();
			if (state.backtracking > 0) { Memoize(input, 34, attrib_StartIndex); }
		}
		DebugLocation(305, 4);
		} finally { DebugExitRule(GrammarFileName, "attrib"); }
		return retval;

	}
	// $ANTLR end "attrib"

	public class attribVal_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_attribVal();
	partial void Leave_attribVal();

	// $ANTLR start "attribVal"
	// CssGrammer.g3:307:1: attribVal : attribOp attribValue -> ^( ATTRIB_VALUE attribOp attribValue ) ;
	[GrammarRule("attribVal")]
	private CssGrammerParser.attribVal_return attribVal()
	{
		Enter_attribVal();
		EnterRule("attribVal", 35);
		TraceIn("attribVal", 35);
		CssGrammerParser.attribVal_return retval = new CssGrammerParser.attribVal_return();
		retval.Start = (CommonToken)input.LT(1);
		int attribVal_StartIndex = input.Index;
		CommonTree root_0 = null;

		CssGrammerParser.attribOp_return attribOp116 = default(CssGrammerParser.attribOp_return);
		CssGrammerParser.attribValue_return attribValue117 = default(CssGrammerParser.attribValue_return);

		RewriteRuleSubtreeStream stream_attribOp=new RewriteRuleSubtreeStream(adaptor,"rule attribOp");
		RewriteRuleSubtreeStream stream_attribValue=new RewriteRuleSubtreeStream(adaptor,"rule attribValue");
		try { DebugEnterRule(GrammarFileName, "attribVal");
		DebugLocation(307, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 35)) { return retval; }
			// CssGrammer.g3:308:5: ( attribOp attribValue -> ^( ATTRIB_VALUE attribOp attribValue ) )
			DebugEnterAlt(1);
			// CssGrammer.g3:308:7: attribOp attribValue
			{
			DebugLocation(308, 7);
			PushFollow(Follow._attribOp_in_attribVal2143);
			attribOp116=attribOp();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_attribOp.Add(attribOp116.Tree);
			DebugLocation(308, 16);
			PushFollow(Follow._attribValue_in_attribVal2145);
			attribValue117=attribValue();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_attribValue.Add(attribValue117.Tree);


			{
			// AST REWRITE
			// elements: attribOp, attribValue
			// token labels: 
			// rule labels: retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 309:9: -> ^( ATTRIB_VALUE attribOp attribValue )
			{
				DebugLocation(309, 12);
				// CssGrammer.g3:309:12: ^( ATTRIB_VALUE attribOp attribValue )
				{
				CommonTree root_1 = (CommonTree)adaptor.Nil();
				DebugLocation(309, 14);
				root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(ATTRIB_VALUE, "ATTRIB_VALUE"), root_1);

				DebugLocation(309, 27);
				adaptor.AddChild(root_1, stream_attribOp.NextTree());
				DebugLocation(309, 36);
				adaptor.AddChild(root_1, stream_attribValue.NextTree());

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
			TraceOut("attribVal", 35);
			LeaveRule("attribVal", 35);
			Leave_attribVal();
			if (state.backtracking > 0) { Memoize(input, 35, attribVal_StartIndex); }
		}
		DebugLocation(310, 4);
		} finally { DebugExitRule(GrammarFileName, "attribVal"); }
		return retval;

	}
	// $ANTLR end "attribVal"

	public class attribValue_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_attribValue();
	partial void Leave_attribValue();

	// $ANTLR start "attribValue"
	// CssGrammer.g3:312:1: attribValue : ( IDENT | STRING );
	[GrammarRule("attribValue")]
	private CssGrammerParser.attribValue_return attribValue()
	{
		Enter_attribValue();
		EnterRule("attribValue", 36);
		TraceIn("attribValue", 36);
		CssGrammerParser.attribValue_return retval = new CssGrammerParser.attribValue_return();
		retval.Start = (CommonToken)input.LT(1);
		int attribValue_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken set118=null;

		CommonTree set118_tree=null;

		try { DebugEnterRule(GrammarFileName, "attribValue");
		DebugLocation(312, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 36)) { return retval; }
			// CssGrammer.g3:313:5: ( IDENT | STRING )
			DebugEnterAlt(1);
			// CssGrammer.g3:
			{
			root_0 = (CommonTree)adaptor.Nil();

			DebugLocation(313, 5);
			set118=(CommonToken)input.LT(1);
			if (input.LA(1)==IDENT||input.LA(1)==STRING)
			{
				input.Consume();
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, (CommonTree)adaptor.Create(set118));
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
			TraceOut("attribValue", 36);
			LeaveRule("attribValue", 36);
			Leave_attribValue();
			if (state.backtracking > 0) { Memoize(input, 36, attribValue_StartIndex); }
		}
		DebugLocation(315, 4);
		} finally { DebugExitRule(GrammarFileName, "attribValue"); }
		return retval;

	}
	// $ANTLR end "attribValue"

	public class attribOp_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_attribOp();
	partial void Leave_attribOp();

	// $ANTLR start "attribOp"
	// CssGrammer.g3:317:1: attribOp : ( OPEQ -> ^( ATTRIB_EQUALS ) | INCLUDES_WORD -> ^( ATTRIB_CONTAINS_WORD ) | STARTS_WITH_WORD -> ^( ATTRIB_STARTS_WITH_WORD ) | INCLUDES -> ^( ATTRIB_CONTAINS ) | STARTS_WITH -> ^( ATTRIB_STARTS_WITH ) | ENDS_WITH -> ^( ATTRIB_ENDS_WITH ) );
	[GrammarRule("attribOp")]
	private CssGrammerParser.attribOp_return attribOp()
	{
		Enter_attribOp();
		EnterRule("attribOp", 37);
		TraceIn("attribOp", 37);
		CssGrammerParser.attribOp_return retval = new CssGrammerParser.attribOp_return();
		retval.Start = (CommonToken)input.LT(1);
		int attribOp_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken OPEQ119=null;
		CommonToken INCLUDES_WORD120=null;
		CommonToken STARTS_WITH_WORD121=null;
		CommonToken INCLUDES122=null;
		CommonToken STARTS_WITH123=null;
		CommonToken ENDS_WITH124=null;

		CommonTree OPEQ119_tree=null;
		CommonTree INCLUDES_WORD120_tree=null;
		CommonTree STARTS_WITH_WORD121_tree=null;
		CommonTree INCLUDES122_tree=null;
		CommonTree STARTS_WITH123_tree=null;
		CommonTree ENDS_WITH124_tree=null;
		RewriteRuleITokenStream stream_OPEQ=new RewriteRuleITokenStream(adaptor,"token OPEQ");
		RewriteRuleITokenStream stream_INCLUDES_WORD=new RewriteRuleITokenStream(adaptor,"token INCLUDES_WORD");
		RewriteRuleITokenStream stream_STARTS_WITH_WORD=new RewriteRuleITokenStream(adaptor,"token STARTS_WITH_WORD");
		RewriteRuleITokenStream stream_INCLUDES=new RewriteRuleITokenStream(adaptor,"token INCLUDES");
		RewriteRuleITokenStream stream_STARTS_WITH=new RewriteRuleITokenStream(adaptor,"token STARTS_WITH");
		RewriteRuleITokenStream stream_ENDS_WITH=new RewriteRuleITokenStream(adaptor,"token ENDS_WITH");

		try { DebugEnterRule(GrammarFileName, "attribOp");
		DebugLocation(317, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 37)) { return retval; }
			// CssGrammer.g3:318:5: ( OPEQ -> ^( ATTRIB_EQUALS ) | INCLUDES_WORD -> ^( ATTRIB_CONTAINS_WORD ) | STARTS_WITH_WORD -> ^( ATTRIB_STARTS_WITH_WORD ) | INCLUDES -> ^( ATTRIB_CONTAINS ) | STARTS_WITH -> ^( ATTRIB_STARTS_WITH ) | ENDS_WITH -> ^( ATTRIB_ENDS_WITH ) )
			int alt35=6;
			try { DebugEnterDecision(35, decisionCanBacktrack[35]);
			switch (input.LA(1))
			{
			case OPEQ:
				{
				alt35=1;
				}
				break;
			case INCLUDES_WORD:
				{
				alt35=2;
				}
				break;
			case STARTS_WITH_WORD:
				{
				alt35=3;
				}
				break;
			case INCLUDES:
				{
				alt35=4;
				}
				break;
			case STARTS_WITH:
				{
				alt35=5;
				}
				break;
			case ENDS_WITH:
				{
				alt35=6;
				}
				break;
			default:
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 35, 0, input);

					DebugRecognitionException(nvae);
					throw nvae;
				}
			}

			} finally { DebugExitDecision(35); }
			switch (alt35)
			{
			case 1:
				DebugEnterAlt(1);
				// CssGrammer.g3:318:7: OPEQ
				{
				DebugLocation(318, 7);
				OPEQ119=(CommonToken)Match(input,OPEQ,Follow._OPEQ_in_attribOp2205); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_OPEQ.Add(OPEQ119);



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
				// 318:12: -> ^( ATTRIB_EQUALS )
				{
					DebugLocation(318, 15);
					// CssGrammer.g3:318:15: ^( ATTRIB_EQUALS )
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(318, 17);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(ATTRIB_EQUALS, "ATTRIB_EQUALS"), root_1);

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
				// CssGrammer.g3:319:7: INCLUDES_WORD
				{
				DebugLocation(319, 7);
				INCLUDES_WORD120=(CommonToken)Match(input,INCLUDES_WORD,Follow._INCLUDES_WORD_in_attribOp2219); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_INCLUDES_WORD.Add(INCLUDES_WORD120);



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
				// 319:21: -> ^( ATTRIB_CONTAINS_WORD )
				{
					DebugLocation(319, 24);
					// CssGrammer.g3:319:24: ^( ATTRIB_CONTAINS_WORD )
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(319, 26);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(ATTRIB_CONTAINS_WORD, "ATTRIB_CONTAINS_WORD"), root_1);

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
				// CssGrammer.g3:320:7: STARTS_WITH_WORD
				{
				DebugLocation(320, 7);
				STARTS_WITH_WORD121=(CommonToken)Match(input,STARTS_WITH_WORD,Follow._STARTS_WITH_WORD_in_attribOp2233); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_STARTS_WITH_WORD.Add(STARTS_WITH_WORD121);



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
				// 320:24: -> ^( ATTRIB_STARTS_WITH_WORD )
				{
					DebugLocation(320, 27);
					// CssGrammer.g3:320:27: ^( ATTRIB_STARTS_WITH_WORD )
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(320, 29);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(ATTRIB_STARTS_WITH_WORD, "ATTRIB_STARTS_WITH_WORD"), root_1);

					adaptor.AddChild(root_0, root_1);
					}

				}

				retval.Tree = root_0;
				}
				}

				}
				break;
			case 4:
				DebugEnterAlt(4);
				// CssGrammer.g3:321:7: INCLUDES
				{
				DebugLocation(321, 7);
				INCLUDES122=(CommonToken)Match(input,INCLUDES,Follow._INCLUDES_in_attribOp2247); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_INCLUDES.Add(INCLUDES122);



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
				// 321:16: -> ^( ATTRIB_CONTAINS )
				{
					DebugLocation(321, 19);
					// CssGrammer.g3:321:19: ^( ATTRIB_CONTAINS )
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(321, 21);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(ATTRIB_CONTAINS, "ATTRIB_CONTAINS"), root_1);

					adaptor.AddChild(root_0, root_1);
					}

				}

				retval.Tree = root_0;
				}
				}

				}
				break;
			case 5:
				DebugEnterAlt(5);
				// CssGrammer.g3:322:7: STARTS_WITH
				{
				DebugLocation(322, 7);
				STARTS_WITH123=(CommonToken)Match(input,STARTS_WITH,Follow._STARTS_WITH_in_attribOp2261); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_STARTS_WITH.Add(STARTS_WITH123);



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
				// 322:19: -> ^( ATTRIB_STARTS_WITH )
				{
					DebugLocation(322, 22);
					// CssGrammer.g3:322:22: ^( ATTRIB_STARTS_WITH )
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(322, 24);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(ATTRIB_STARTS_WITH, "ATTRIB_STARTS_WITH"), root_1);

					adaptor.AddChild(root_0, root_1);
					}

				}

				retval.Tree = root_0;
				}
				}

				}
				break;
			case 6:
				DebugEnterAlt(6);
				// CssGrammer.g3:323:7: ENDS_WITH
				{
				DebugLocation(323, 7);
				ENDS_WITH124=(CommonToken)Match(input,ENDS_WITH,Follow._ENDS_WITH_in_attribOp2275); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_ENDS_WITH.Add(ENDS_WITH124);



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
				// 323:17: -> ^( ATTRIB_ENDS_WITH )
				{
					DebugLocation(323, 20);
					// CssGrammer.g3:323:20: ^( ATTRIB_ENDS_WITH )
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(323, 22);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(ATTRIB_ENDS_WITH, "ATTRIB_ENDS_WITH"), root_1);

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
			TraceOut("attribOp", 37);
			LeaveRule("attribOp", 37);
			Leave_attribOp();
			if (state.backtracking > 0) { Memoize(input, 37, attribOp_StartIndex); }
		}
		DebugLocation(324, 4);
		} finally { DebugExitRule(GrammarFileName, "attribOp"); }
		return retval;

	}
	// $ANTLR end "attribOp"

	public class pseudo_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_pseudo();
	partial void Leave_pseudo();

	// $ANTLR start "pseudo"
	// CssGrammer.g3:326:1: pseudo : ( pseudoStart identifier LPAREN ( pseudoFuncArgs )? RPAREN -> ^( PSEUDO_FUNC pseudoStart identifier ( pseudoFuncArgs )? ) | pseudoStart identifier LPAREN ( selector )? RPAREN -> ^( PSEUDO_FUNC pseudoStart identifier ( selector )? ) | pseudoStart identifier -> ^( PSEUDO pseudoStart identifier ) );
	[GrammarRule("pseudo")]
	private CssGrammerParser.pseudo_return pseudo()
	{
		Enter_pseudo();
		EnterRule("pseudo", 38);
		TraceIn("pseudo", 38);
		CssGrammerParser.pseudo_return retval = new CssGrammerParser.pseudo_return();
		retval.Start = (CommonToken)input.LT(1);
		int pseudo_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken LPAREN127=null;
		CommonToken RPAREN129=null;
		CommonToken LPAREN132=null;
		CommonToken RPAREN134=null;
		CssGrammerParser.pseudoStart_return pseudoStart125 = default(CssGrammerParser.pseudoStart_return);
		CssGrammerParser.identifier_return identifier126 = default(CssGrammerParser.identifier_return);
		CssGrammerParser.pseudoFuncArgs_return pseudoFuncArgs128 = default(CssGrammerParser.pseudoFuncArgs_return);
		CssGrammerParser.pseudoStart_return pseudoStart130 = default(CssGrammerParser.pseudoStart_return);
		CssGrammerParser.identifier_return identifier131 = default(CssGrammerParser.identifier_return);
		CssGrammerParser.selector_return selector133 = default(CssGrammerParser.selector_return);
		CssGrammerParser.pseudoStart_return pseudoStart135 = default(CssGrammerParser.pseudoStart_return);
		CssGrammerParser.identifier_return identifier136 = default(CssGrammerParser.identifier_return);

		CommonTree LPAREN127_tree=null;
		CommonTree RPAREN129_tree=null;
		CommonTree LPAREN132_tree=null;
		CommonTree RPAREN134_tree=null;
		RewriteRuleITokenStream stream_LPAREN=new RewriteRuleITokenStream(adaptor,"token LPAREN");
		RewriteRuleITokenStream stream_RPAREN=new RewriteRuleITokenStream(adaptor,"token RPAREN");
		RewriteRuleSubtreeStream stream_pseudoStart=new RewriteRuleSubtreeStream(adaptor,"rule pseudoStart");
		RewriteRuleSubtreeStream stream_identifier=new RewriteRuleSubtreeStream(adaptor,"rule identifier");
		RewriteRuleSubtreeStream stream_pseudoFuncArgs=new RewriteRuleSubtreeStream(adaptor,"rule pseudoFuncArgs");
		RewriteRuleSubtreeStream stream_selector=new RewriteRuleSubtreeStream(adaptor,"rule selector");
		try { DebugEnterRule(GrammarFileName, "pseudo");
		DebugLocation(326, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 38)) { return retval; }
			// CssGrammer.g3:327:5: ( pseudoStart identifier LPAREN ( pseudoFuncArgs )? RPAREN -> ^( PSEUDO_FUNC pseudoStart identifier ( pseudoFuncArgs )? ) | pseudoStart identifier LPAREN ( selector )? RPAREN -> ^( PSEUDO_FUNC pseudoStart identifier ( selector )? ) | pseudoStart identifier -> ^( PSEUDO pseudoStart identifier ) )
			int alt38=3;
			try { DebugEnterDecision(38, decisionCanBacktrack[38]);
			int LA38_0 = input.LA(1);

			if ((LA38_0==COLON||LA38_0==DOUBLE_COLON))
			{
				int LA38_1 = input.LA(2);

				if ((LA38_1==IDENT||LA38_1==NOT_SYM||LA38_1==TO_SYM))
				{
					int LA38_2 = input.LA(3);

					if ((LA38_2==LPAREN))
					{
						switch (input.LA(4))
						{
						case IDENT:
							{
							int LA38_5 = input.LA(5);

							if ((LA38_5==RPAREN))
							{
								int LA38_7 = input.LA(6);

								if ((EvaluatePredicate(synpred61_CssGrammer_fragment)))
								{
									alt38=1;
								}
								else if ((EvaluatePredicate(synpred63_CssGrammer_fragment)))
								{
									alt38=2;
								}
								else
								{
									if (state.backtracking>0) {state.failed=true; return retval;}
									NoViableAltException nvae = new NoViableAltException("", 38, 7, input);

									DebugRecognitionException(nvae);
									throw nvae;
								}
							}
							else if ((LA38_5==COLON||(LA38_5>=DOT && LA38_5<=DOUBLE_COLON)||LA38_5==GREATER||LA38_5==HASH||LA38_5==IDENT||LA38_5==LBRACKET||LA38_5==PLUS||LA38_5==STAR||LA38_5==152))
							{
								alt38=2;
							}
							else
							{
								if (state.backtracking>0) {state.failed=true; return retval;}
								NoViableAltException nvae = new NoViableAltException("", 38, 5, input);

								DebugRecognitionException(nvae);
								throw nvae;
							}
							}
							break;
						case MULTIPLIER:
						case NUMBER:
							{
							alt38=1;
							}
							break;
						case RPAREN:
							{
							int LA38_7 = input.LA(5);

							if ((EvaluatePredicate(synpred61_CssGrammer_fragment)))
							{
								alt38=1;
							}
							else if ((EvaluatePredicate(synpred63_CssGrammer_fragment)))
							{
								alt38=2;
							}
							else
							{
								if (state.backtracking>0) {state.failed=true; return retval;}
								NoViableAltException nvae = new NoViableAltException("", 38, 7, input);

								DebugRecognitionException(nvae);
								throw nvae;
							}
							}
							break;
						case COLON:
						case DOT:
						case DOUBLE_COLON:
						case HASH:
						case LBRACKET:
						case STAR:
							{
							alt38=2;
							}
							break;
						default:
							{
								if (state.backtracking>0) {state.failed=true; return retval;}
								NoViableAltException nvae = new NoViableAltException("", 38, 3, input);

								DebugRecognitionException(nvae);
								throw nvae;
							}
						}

					}
					else if ((LA38_2==EOF||LA38_2==COLON||LA38_2==COMMA||(LA38_2>=DOT && LA38_2<=DOUBLE_COLON)||LA38_2==GREATER||LA38_2==HASH||LA38_2==IDENT||(LA38_2>=LBRACE && LA38_2<=LBRACKET)||LA38_2==PLUS||LA38_2==RPAREN||LA38_2==STAR||LA38_2==152))
					{
						alt38=3;
					}
					else
					{
						if (state.backtracking>0) {state.failed=true; return retval;}
						NoViableAltException nvae = new NoViableAltException("", 38, 2, input);

						DebugRecognitionException(nvae);
						throw nvae;
					}
				}
				else
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 38, 1, input);

					DebugRecognitionException(nvae);
					throw nvae;
				}
			}
			else
			{
				if (state.backtracking>0) {state.failed=true; return retval;}
				NoViableAltException nvae = new NoViableAltException("", 38, 0, input);

				DebugRecognitionException(nvae);
				throw nvae;
			}
			} finally { DebugExitDecision(38); }
			switch (alt38)
			{
			case 1:
				DebugEnterAlt(1);
				// CssGrammer.g3:327:7: pseudoStart identifier LPAREN ( pseudoFuncArgs )? RPAREN
				{
				DebugLocation(327, 7);
				PushFollow(Follow._pseudoStart_in_pseudo2298);
				pseudoStart125=pseudoStart();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_pseudoStart.Add(pseudoStart125.Tree);
				DebugLocation(327, 19);
				PushFollow(Follow._identifier_in_pseudo2300);
				identifier126=identifier();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_identifier.Add(identifier126.Tree);
				DebugLocation(327, 30);
				LPAREN127=(CommonToken)Match(input,LPAREN,Follow._LPAREN_in_pseudo2302); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_LPAREN.Add(LPAREN127);

				DebugLocation(327, 37);
				// CssGrammer.g3:327:37: ( pseudoFuncArgs )?
				int alt36=2;
				try { DebugEnterSubRule(36);
				try { DebugEnterDecision(36, decisionCanBacktrack[36]);
				int LA36_0 = input.LA(1);

				if ((LA36_0==IDENT||LA36_0==MULTIPLIER||LA36_0==NUMBER))
				{
					alt36=1;
				}
				} finally { DebugExitDecision(36); }
				switch (alt36)
				{
				case 1:
					DebugEnterAlt(1);
					// CssGrammer.g3:327:38: pseudoFuncArgs
					{
					DebugLocation(327, 38);
					PushFollow(Follow._pseudoFuncArgs_in_pseudo2305);
					pseudoFuncArgs128=pseudoFuncArgs();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_pseudoFuncArgs.Add(pseudoFuncArgs128.Tree);

					}
					break;

				}
				} finally { DebugExitSubRule(36); }

				DebugLocation(327, 55);
				RPAREN129=(CommonToken)Match(input,RPAREN,Follow._RPAREN_in_pseudo2309); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_RPAREN.Add(RPAREN129);



				{
				// AST REWRITE
				// elements: pseudoStart, identifier, pseudoFuncArgs
				// token labels: 
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.Tree = root_0;
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

				root_0 = (CommonTree)adaptor.Nil();
				// 328:9: -> ^( PSEUDO_FUNC pseudoStart identifier ( pseudoFuncArgs )? )
				{
					DebugLocation(328, 12);
					// CssGrammer.g3:328:12: ^( PSEUDO_FUNC pseudoStart identifier ( pseudoFuncArgs )? )
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(328, 14);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(PSEUDO_FUNC, "PSEUDO_FUNC"), root_1);

					DebugLocation(328, 26);
					adaptor.AddChild(root_1, stream_pseudoStart.NextTree());
					DebugLocation(328, 38);
					adaptor.AddChild(root_1, stream_identifier.NextTree());
					DebugLocation(328, 49);
					// CssGrammer.g3:328:49: ( pseudoFuncArgs )?
					if ( stream_pseudoFuncArgs.HasNext )
					{
						DebugLocation(328, 49);
						adaptor.AddChild(root_1, stream_pseudoFuncArgs.NextTree());

					}
					stream_pseudoFuncArgs.Reset();

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
				// CssGrammer.g3:329:7: pseudoStart identifier LPAREN ( selector )? RPAREN
				{
				DebugLocation(329, 7);
				PushFollow(Follow._pseudoStart_in_pseudo2338);
				pseudoStart130=pseudoStart();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_pseudoStart.Add(pseudoStart130.Tree);
				DebugLocation(329, 19);
				PushFollow(Follow._identifier_in_pseudo2340);
				identifier131=identifier();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_identifier.Add(identifier131.Tree);
				DebugLocation(329, 30);
				LPAREN132=(CommonToken)Match(input,LPAREN,Follow._LPAREN_in_pseudo2342); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_LPAREN.Add(LPAREN132);

				DebugLocation(329, 37);
				// CssGrammer.g3:329:37: ( selector )?
				int alt37=2;
				try { DebugEnterSubRule(37);
				try { DebugEnterDecision(37, decisionCanBacktrack[37]);
				int LA37_0 = input.LA(1);

				if ((LA37_0==COLON||(LA37_0>=DOT && LA37_0<=DOUBLE_COLON)||LA37_0==HASH||LA37_0==IDENT||LA37_0==LBRACKET||LA37_0==STAR))
				{
					alt37=1;
				}
				} finally { DebugExitDecision(37); }
				switch (alt37)
				{
				case 1:
					DebugEnterAlt(1);
					// CssGrammer.g3:329:38: selector
					{
					DebugLocation(329, 38);
					PushFollow(Follow._selector_in_pseudo2345);
					selector133=selector();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_selector.Add(selector133.Tree);

					}
					break;

				}
				} finally { DebugExitSubRule(37); }

				DebugLocation(329, 49);
				RPAREN134=(CommonToken)Match(input,RPAREN,Follow._RPAREN_in_pseudo2349); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_RPAREN.Add(RPAREN134);



				{
				// AST REWRITE
				// elements: pseudoStart, identifier, selector
				// token labels: 
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.Tree = root_0;
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

				root_0 = (CommonTree)adaptor.Nil();
				// 330:9: -> ^( PSEUDO_FUNC pseudoStart identifier ( selector )? )
				{
					DebugLocation(330, 12);
					// CssGrammer.g3:330:12: ^( PSEUDO_FUNC pseudoStart identifier ( selector )? )
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(330, 14);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(PSEUDO_FUNC, "PSEUDO_FUNC"), root_1);

					DebugLocation(330, 26);
					adaptor.AddChild(root_1, stream_pseudoStart.NextTree());
					DebugLocation(330, 38);
					adaptor.AddChild(root_1, stream_identifier.NextTree());
					DebugLocation(330, 49);
					// CssGrammer.g3:330:49: ( selector )?
					if ( stream_selector.HasNext )
					{
						DebugLocation(330, 49);
						adaptor.AddChild(root_1, stream_selector.NextTree());

					}
					stream_selector.Reset();

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
				// CssGrammer.g3:331:7: pseudoStart identifier
				{
				DebugLocation(331, 7);
				PushFollow(Follow._pseudoStart_in_pseudo2378);
				pseudoStart135=pseudoStart();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_pseudoStart.Add(pseudoStart135.Tree);
				DebugLocation(331, 19);
				PushFollow(Follow._identifier_in_pseudo2380);
				identifier136=identifier();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_identifier.Add(identifier136.Tree);


				{
				// AST REWRITE
				// elements: pseudoStart, identifier
				// token labels: 
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.Tree = root_0;
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

				root_0 = (CommonTree)adaptor.Nil();
				// 332:9: -> ^( PSEUDO pseudoStart identifier )
				{
					DebugLocation(332, 12);
					// CssGrammer.g3:332:12: ^( PSEUDO pseudoStart identifier )
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(332, 14);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(PSEUDO, "PSEUDO"), root_1);

					DebugLocation(332, 21);
					adaptor.AddChild(root_1, stream_pseudoStart.NextTree());
					DebugLocation(332, 33);
					adaptor.AddChild(root_1, stream_identifier.NextTree());

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
			TraceOut("pseudo", 38);
			LeaveRule("pseudo", 38);
			Leave_pseudo();
			if (state.backtracking > 0) { Memoize(input, 38, pseudo_StartIndex); }
		}
		DebugLocation(333, 4);
		} finally { DebugExitRule(GrammarFileName, "pseudo"); }
		return retval;

	}
	// $ANTLR end "pseudo"

	public class pseudoStart_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_pseudoStart();
	partial void Leave_pseudoStart();

	// $ANTLR start "pseudoStart"
	// CssGrammer.g3:335:1: pseudoStart : ( COLON | DOUBLE_COLON );
	[GrammarRule("pseudoStart")]
	private CssGrammerParser.pseudoStart_return pseudoStart()
	{
		Enter_pseudoStart();
		EnterRule("pseudoStart", 39);
		TraceIn("pseudoStart", 39);
		CssGrammerParser.pseudoStart_return retval = new CssGrammerParser.pseudoStart_return();
		retval.Start = (CommonToken)input.LT(1);
		int pseudoStart_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken set137=null;

		CommonTree set137_tree=null;

		try { DebugEnterRule(GrammarFileName, "pseudoStart");
		DebugLocation(335, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 39)) { return retval; }
			// CssGrammer.g3:336:9: ( COLON | DOUBLE_COLON )
			DebugEnterAlt(1);
			// CssGrammer.g3:
			{
			root_0 = (CommonTree)adaptor.Nil();

			DebugLocation(336, 9);
			set137=(CommonToken)input.LT(1);
			if (input.LA(1)==COLON||input.LA(1)==DOUBLE_COLON)
			{
				input.Consume();
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, (CommonTree)adaptor.Create(set137));
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
			TraceOut("pseudoStart", 39);
			LeaveRule("pseudoStart", 39);
			Leave_pseudoStart();
			if (state.backtracking > 0) { Memoize(input, 39, pseudoStart_StartIndex); }
		}
		DebugLocation(338, 4);
		} finally { DebugExitRule(GrammarFileName, "pseudoStart"); }
		return retval;

	}
	// $ANTLR end "pseudoStart"

	public class pseudoFuncArgs_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_pseudoFuncArgs();
	partial void Leave_pseudoFuncArgs();

	// $ANTLR start "pseudoFuncArgs"
	// CssGrammer.g3:340:1: pseudoFuncArgs : ( IDENT | MULTIPLIER ( unaryOperator NUMBER )? | NUMBER );
	[GrammarRule("pseudoFuncArgs")]
	private CssGrammerParser.pseudoFuncArgs_return pseudoFuncArgs()
	{
		Enter_pseudoFuncArgs();
		EnterRule("pseudoFuncArgs", 40);
		TraceIn("pseudoFuncArgs", 40);
		CssGrammerParser.pseudoFuncArgs_return retval = new CssGrammerParser.pseudoFuncArgs_return();
		retval.Start = (CommonToken)input.LT(1);
		int pseudoFuncArgs_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken IDENT138=null;
		CommonToken MULTIPLIER139=null;
		CommonToken NUMBER141=null;
		CommonToken NUMBER142=null;
		CssGrammerParser.unaryOperator_return unaryOperator140 = default(CssGrammerParser.unaryOperator_return);

		CommonTree IDENT138_tree=null;
		CommonTree MULTIPLIER139_tree=null;
		CommonTree NUMBER141_tree=null;
		CommonTree NUMBER142_tree=null;

		try { DebugEnterRule(GrammarFileName, "pseudoFuncArgs");
		DebugLocation(340, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 40)) { return retval; }
			// CssGrammer.g3:341:5: ( IDENT | MULTIPLIER ( unaryOperator NUMBER )? | NUMBER )
			int alt40=3;
			try { DebugEnterDecision(40, decisionCanBacktrack[40]);
			switch (input.LA(1))
			{
			case IDENT:
				{
				alt40=1;
				}
				break;
			case MULTIPLIER:
				{
				alt40=2;
				}
				break;
			case NUMBER:
				{
				alt40=3;
				}
				break;
			default:
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 40, 0, input);

					DebugRecognitionException(nvae);
					throw nvae;
				}
			}

			} finally { DebugExitDecision(40); }
			switch (alt40)
			{
			case 1:
				DebugEnterAlt(1);
				// CssGrammer.g3:341:7: IDENT
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(341, 7);
				IDENT138=(CommonToken)Match(input,IDENT,Follow._IDENT_in_pseudoFuncArgs2448); if (state.failed) return retval;
				if ( state.backtracking==0 ) {
				IDENT138_tree = (CommonTree)adaptor.Create(IDENT138);
				adaptor.AddChild(root_0, IDENT138_tree);
				}

				}
				break;
			case 2:
				DebugEnterAlt(2);
				// CssGrammer.g3:342:7: MULTIPLIER ( unaryOperator NUMBER )?
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(342, 7);
				MULTIPLIER139=(CommonToken)Match(input,MULTIPLIER,Follow._MULTIPLIER_in_pseudoFuncArgs2456); if (state.failed) return retval;
				if ( state.backtracking==0 ) {
				MULTIPLIER139_tree = (CommonTree)adaptor.Create(MULTIPLIER139);
				adaptor.AddChild(root_0, MULTIPLIER139_tree);
				}
				DebugLocation(342, 18);
				// CssGrammer.g3:342:18: ( unaryOperator NUMBER )?
				int alt39=2;
				try { DebugEnterSubRule(39);
				try { DebugEnterDecision(39, decisionCanBacktrack[39]);
				int LA39_0 = input.LA(1);

				if ((LA39_0==MINUS||LA39_0==PLUS))
				{
					alt39=1;
				}
				} finally { DebugExitDecision(39); }
				switch (alt39)
				{
				case 1:
					DebugEnterAlt(1);
					// CssGrammer.g3:342:19: unaryOperator NUMBER
					{
					DebugLocation(342, 19);
					PushFollow(Follow._unaryOperator_in_pseudoFuncArgs2459);
					unaryOperator140=unaryOperator();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) adaptor.AddChild(root_0, unaryOperator140.Tree);
					DebugLocation(342, 33);
					NUMBER141=(CommonToken)Match(input,NUMBER,Follow._NUMBER_in_pseudoFuncArgs2461); if (state.failed) return retval;
					if ( state.backtracking==0 ) {
					NUMBER141_tree = (CommonTree)adaptor.Create(NUMBER141);
					adaptor.AddChild(root_0, NUMBER141_tree);
					}

					}
					break;

				}
				} finally { DebugExitSubRule(39); }


				}
				break;
			case 3:
				DebugEnterAlt(3);
				// CssGrammer.g3:343:7: NUMBER
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(343, 7);
				NUMBER142=(CommonToken)Match(input,NUMBER,Follow._NUMBER_in_pseudoFuncArgs2471); if (state.failed) return retval;
				if ( state.backtracking==0 ) {
				NUMBER142_tree = (CommonTree)adaptor.Create(NUMBER142);
				adaptor.AddChild(root_0, NUMBER142_tree);
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
			TraceOut("pseudoFuncArgs", 40);
			LeaveRule("pseudoFuncArgs", 40);
			Leave_pseudoFuncArgs();
			if (state.backtracking > 0) { Memoize(input, 40, pseudoFuncArgs_StartIndex); }
		}
		DebugLocation(344, 4);
		} finally { DebugExitRule(GrammarFileName, "pseudoFuncArgs"); }
		return retval;

	}
	// $ANTLR end "pseudoFuncArgs"

	public class declaration_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_declaration();
	partial void Leave_declaration();

	// $ANTLR start "declaration"
	// CssGrammer.g3:346:1: declaration : property COLON expressions ( prio )? -> ^( PROPERTY property expressions ( prio )? ) ;
	[GrammarRule("declaration")]
	private CssGrammerParser.declaration_return declaration()
	{
		Enter_declaration();
		EnterRule("declaration", 41);
		TraceIn("declaration", 41);
		CssGrammerParser.declaration_return retval = new CssGrammerParser.declaration_return();
		retval.Start = (CommonToken)input.LT(1);
		int declaration_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken COLON144=null;
		CssGrammerParser.property_return property143 = default(CssGrammerParser.property_return);
		CssGrammerParser.expressions_return expressions145 = default(CssGrammerParser.expressions_return);
		CssGrammerParser.prio_return prio146 = default(CssGrammerParser.prio_return);

		CommonTree COLON144_tree=null;
		RewriteRuleITokenStream stream_COLON=new RewriteRuleITokenStream(adaptor,"token COLON");
		RewriteRuleSubtreeStream stream_property=new RewriteRuleSubtreeStream(adaptor,"rule property");
		RewriteRuleSubtreeStream stream_expressions=new RewriteRuleSubtreeStream(adaptor,"rule expressions");
		RewriteRuleSubtreeStream stream_prio=new RewriteRuleSubtreeStream(adaptor,"rule prio");
		try { DebugEnterRule(GrammarFileName, "declaration");
		DebugLocation(346, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 41)) { return retval; }
			// CssGrammer.g3:347:5: ( property COLON expressions ( prio )? -> ^( PROPERTY property expressions ( prio )? ) )
			DebugEnterAlt(1);
			// CssGrammer.g3:347:7: property COLON expressions ( prio )?
			{
			DebugLocation(347, 7);
			PushFollow(Follow._property_in_declaration2488);
			property143=property();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_property.Add(property143.Tree);
			DebugLocation(347, 16);
			COLON144=(CommonToken)Match(input,COLON,Follow._COLON_in_declaration2490); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_COLON.Add(COLON144);

			DebugLocation(347, 22);
			PushFollow(Follow._expressions_in_declaration2492);
			expressions145=expressions();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_expressions.Add(expressions145.Tree);
			DebugLocation(347, 34);
			// CssGrammer.g3:347:34: ( prio )?
			int alt41=2;
			try { DebugEnterSubRule(41);
			try { DebugEnterDecision(41, decisionCanBacktrack[41]);
			int LA41_0 = input.LA(1);

			if ((LA41_0==IMPORTANT_SYM))
			{
				alt41=1;
			}
			} finally { DebugExitDecision(41); }
			switch (alt41)
			{
			case 1:
				DebugEnterAlt(1);
				// CssGrammer.g3:347:34: prio
				{
				DebugLocation(347, 34);
				PushFollow(Follow._prio_in_declaration2494);
				prio146=prio();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_prio.Add(prio146.Tree);

				}
				break;

			}
			} finally { DebugExitSubRule(41); }



			{
			// AST REWRITE
			// elements: property, expressions, prio
			// token labels: 
			// rule labels: retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 348:9: -> ^( PROPERTY property expressions ( prio )? )
			{
				DebugLocation(348, 12);
				// CssGrammer.g3:348:12: ^( PROPERTY property expressions ( prio )? )
				{
				CommonTree root_1 = (CommonTree)adaptor.Nil();
				DebugLocation(348, 14);
				root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(PROPERTY, "PROPERTY"), root_1);

				DebugLocation(348, 23);
				adaptor.AddChild(root_1, stream_property.NextTree());
				DebugLocation(348, 32);
				adaptor.AddChild(root_1, stream_expressions.NextTree());
				DebugLocation(348, 44);
				// CssGrammer.g3:348:44: ( prio )?
				if ( stream_prio.HasNext )
				{
					DebugLocation(348, 44);
					adaptor.AddChild(root_1, stream_prio.NextTree());

				}
				stream_prio.Reset();

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
			TraceOut("declaration", 41);
			LeaveRule("declaration", 41);
			Leave_declaration();
			if (state.backtracking > 0) { Memoize(input, 41, declaration_StartIndex); }
		}
		DebugLocation(349, 4);
		} finally { DebugExitRule(GrammarFileName, "declaration"); }
		return retval;

	}
	// $ANTLR end "declaration"

	public class prio_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_prio();
	partial void Leave_prio();

	// $ANTLR start "prio"
	// CssGrammer.g3:351:1: prio : IMPORTANT_SYM ;
	[GrammarRule("prio")]
	private CssGrammerParser.prio_return prio()
	{
		Enter_prio();
		EnterRule("prio", 42);
		TraceIn("prio", 42);
		CssGrammerParser.prio_return retval = new CssGrammerParser.prio_return();
		retval.Start = (CommonToken)input.LT(1);
		int prio_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken IMPORTANT_SYM147=null;

		CommonTree IMPORTANT_SYM147_tree=null;

		try { DebugEnterRule(GrammarFileName, "prio");
		DebugLocation(351, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 42)) { return retval; }
			// CssGrammer.g3:352:5: ( IMPORTANT_SYM )
			DebugEnterAlt(1);
			// CssGrammer.g3:352:7: IMPORTANT_SYM
			{
			root_0 = (CommonTree)adaptor.Nil();

			DebugLocation(352, 7);
			IMPORTANT_SYM147=(CommonToken)Match(input,IMPORTANT_SYM,Follow._IMPORTANT_SYM_in_prio2537); if (state.failed) return retval;
			if ( state.backtracking==0 ) {
			IMPORTANT_SYM147_tree = (CommonTree)adaptor.Create(IMPORTANT_SYM147);
			adaptor.AddChild(root_0, IMPORTANT_SYM147_tree);
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
			TraceOut("prio", 42);
			LeaveRule("prio", 42);
			Leave_prio();
			if (state.backtracking > 0) { Memoize(input, 42, prio_StartIndex); }
		}
		DebugLocation(353, 4);
		} finally { DebugExitRule(GrammarFileName, "prio"); }
		return retval;

	}
	// $ANTLR end "prio"

	public class expressions_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_expressions();
	partial void Leave_expressions();

	// $ANTLR start "expressions"
	// CssGrammer.g3:355:1: expressions : expr ( COMMA expr )* -> ^( EXPRS ( expr )+ ) ;
	[GrammarRule("expressions")]
	private CssGrammerParser.expressions_return expressions()
	{
		Enter_expressions();
		EnterRule("expressions", 43);
		TraceIn("expressions", 43);
		CssGrammerParser.expressions_return retval = new CssGrammerParser.expressions_return();
		retval.Start = (CommonToken)input.LT(1);
		int expressions_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken COMMA149=null;
		CssGrammerParser.expr_return expr148 = default(CssGrammerParser.expr_return);
		CssGrammerParser.expr_return expr150 = default(CssGrammerParser.expr_return);

		CommonTree COMMA149_tree=null;
		RewriteRuleITokenStream stream_COMMA=new RewriteRuleITokenStream(adaptor,"token COMMA");
		RewriteRuleSubtreeStream stream_expr=new RewriteRuleSubtreeStream(adaptor,"rule expr");
		try { DebugEnterRule(GrammarFileName, "expressions");
		DebugLocation(355, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 43)) { return retval; }
			// CssGrammer.g3:356:5: ( expr ( COMMA expr )* -> ^( EXPRS ( expr )+ ) )
			DebugEnterAlt(1);
			// CssGrammer.g3:356:7: expr ( COMMA expr )*
			{
			DebugLocation(356, 7);
			PushFollow(Follow._expr_in_expressions2554);
			expr148=expr();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_expr.Add(expr148.Tree);
			DebugLocation(356, 12);
			// CssGrammer.g3:356:12: ( COMMA expr )*
			try { DebugEnterSubRule(42);
			while (true)
			{
				int alt42=2;
				try { DebugEnterDecision(42, decisionCanBacktrack[42]);
				int LA42_0 = input.LA(1);

				if ((LA42_0==COMMA))
				{
					alt42=1;
				}


				} finally { DebugExitDecision(42); }
				switch ( alt42 )
				{
				case 1:
					DebugEnterAlt(1);
					// CssGrammer.g3:356:13: COMMA expr
					{
					DebugLocation(356, 13);
					COMMA149=(CommonToken)Match(input,COMMA,Follow._COMMA_in_expressions2557); if (state.failed) return retval; 
					if ( state.backtracking == 0 ) stream_COMMA.Add(COMMA149);

					DebugLocation(356, 19);
					PushFollow(Follow._expr_in_expressions2559);
					expr150=expr();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_expr.Add(expr150.Tree);

					}
					break;

				default:
					goto loop42;
				}
			}

			loop42:
				;

			} finally { DebugExitSubRule(42); }



			{
			// AST REWRITE
			// elements: expr
			// token labels: 
			// rule labels: retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 357:9: -> ^( EXPRS ( expr )+ )
			{
				DebugLocation(357, 12);
				// CssGrammer.g3:357:12: ^( EXPRS ( expr )+ )
				{
				CommonTree root_1 = (CommonTree)adaptor.Nil();
				DebugLocation(357, 14);
				root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(EXPRS, "EXPRS"), root_1);

				DebugLocation(357, 20);
				if ( !(stream_expr.HasNext) )
				{
					throw new RewriteEarlyExitException();
				}
				while ( stream_expr.HasNext )
				{
					DebugLocation(357, 20);
					adaptor.AddChild(root_1, stream_expr.NextTree());

				}
				stream_expr.Reset();

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
			TraceOut("expressions", 43);
			LeaveRule("expressions", 43);
			Leave_expressions();
			if (state.backtracking > 0) { Memoize(input, 43, expressions_StartIndex); }
		}
		DebugLocation(358, 4);
		} finally { DebugExitRule(GrammarFileName, "expressions"); }
		return retval;

	}
	// $ANTLR end "expressions"

	public class expr_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_expr();
	partial void Leave_expr();

	// $ANTLR start "expr"
	// CssGrammer.g3:360:1: expr : ( term )+ -> ^( EXPR ( term )+ ) ;
	[GrammarRule("expr")]
	private CssGrammerParser.expr_return expr()
	{
		Enter_expr();
		EnterRule("expr", 44);
		TraceIn("expr", 44);
		CssGrammerParser.expr_return retval = new CssGrammerParser.expr_return();
		retval.Start = (CommonToken)input.LT(1);
		int expr_StartIndex = input.Index;
		CommonTree root_0 = null;

		CssGrammerParser.term_return term151 = default(CssGrammerParser.term_return);

		RewriteRuleSubtreeStream stream_term=new RewriteRuleSubtreeStream(adaptor,"rule term");
		try { DebugEnterRule(GrammarFileName, "expr");
		DebugLocation(360, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 44)) { return retval; }
			// CssGrammer.g3:361:5: ( ( term )+ -> ^( EXPR ( term )+ ) )
			DebugEnterAlt(1);
			// CssGrammer.g3:361:7: ( term )+
			{
			DebugLocation(361, 7);
			// CssGrammer.g3:361:7: ( term )+
			int cnt43=0;
			try { DebugEnterSubRule(43);
			while (true)
			{
				int alt43=2;
				try { DebugEnterDecision(43, decisionCanBacktrack[43]);
				int LA43_0 = input.LA(1);

				if ((LA43_0==ANGLE||LA43_0==EMS||LA43_0==EXS||LA43_0==FREQ||LA43_0==HASH||LA43_0==IDENT||LA43_0==LENGTH||LA43_0==MINUS||LA43_0==NOT_SYM||LA43_0==NUMBER||(LA43_0>=PERCENTAGE && LA43_0<=PLUS)||LA43_0==REM||LA43_0==STRING||(LA43_0>=TIME && LA43_0<=TO_SYM)||LA43_0==URI||LA43_0==148||(LA43_0>=150 && LA43_0<=151)))
				{
					alt43=1;
				}


				} finally { DebugExitDecision(43); }
				switch (alt43)
				{
				case 1:
					DebugEnterAlt(1);
					// CssGrammer.g3:361:7: term
					{
					DebugLocation(361, 7);
					PushFollow(Follow._term_in_expr2599);
					term151=term();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_term.Add(term151.Tree);

					}
					break;

				default:
					if (cnt43 >= 1)
						goto loop43;

					if (state.backtracking>0) {state.failed=true; return retval;}
					EarlyExitException eee43 = new EarlyExitException( 43, input );
					DebugRecognitionException(eee43);
					throw eee43;
				}
				cnt43++;
			}
			loop43:
				;

			} finally { DebugExitSubRule(43); }



			{
			// AST REWRITE
			// elements: term
			// token labels: 
			// rule labels: retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 362:9: -> ^( EXPR ( term )+ )
			{
				DebugLocation(362, 12);
				// CssGrammer.g3:362:12: ^( EXPR ( term )+ )
				{
				CommonTree root_1 = (CommonTree)adaptor.Nil();
				DebugLocation(362, 14);
				root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(EXPR, "EXPR"), root_1);

				DebugLocation(362, 19);
				if ( !(stream_term.HasNext) )
				{
					throw new RewriteEarlyExitException();
				}
				while ( stream_term.HasNext )
				{
					DebugLocation(362, 19);
					adaptor.AddChild(root_1, stream_term.NextTree());

				}
				stream_term.Reset();

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
			TraceOut("expr", 44);
			LeaveRule("expr", 44);
			Leave_expr();
			if (state.backtracking > 0) { Memoize(input, 44, expr_StartIndex); }
		}
		DebugLocation(363, 4);
		} finally { DebugExitRule(GrammarFileName, "expr"); }
		return retval;

	}
	// $ANTLR end "expr"

	public class calcExpr_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_calcExpr();
	partial void Leave_calcExpr();

	// $ANTLR start "calcExpr"
	// CssGrammer.g3:365:1: calcExpr : 'calc' LPAREN unitTerm ( operators unitTerm )* RPAREN -> ^( CALC ( operators )* ( unitTerm )+ ) ;
	[GrammarRule("calcExpr")]
	private CssGrammerParser.calcExpr_return calcExpr()
	{
		Enter_calcExpr();
		EnterRule("calcExpr", 45);
		TraceIn("calcExpr", 45);
		CssGrammerParser.calcExpr_return retval = new CssGrammerParser.calcExpr_return();
		retval.Start = (CommonToken)input.LT(1);
		int calcExpr_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken string_literal152=null;
		CommonToken LPAREN153=null;
		CommonToken RPAREN157=null;
		CssGrammerParser.unitTerm_return unitTerm154 = default(CssGrammerParser.unitTerm_return);
		CssGrammerParser.operators_return operators155 = default(CssGrammerParser.operators_return);
		CssGrammerParser.unitTerm_return unitTerm156 = default(CssGrammerParser.unitTerm_return);

		CommonTree string_literal152_tree=null;
		CommonTree LPAREN153_tree=null;
		CommonTree RPAREN157_tree=null;
		RewriteRuleITokenStream stream_148=new RewriteRuleITokenStream(adaptor,"token 148");
		RewriteRuleITokenStream stream_LPAREN=new RewriteRuleITokenStream(adaptor,"token LPAREN");
		RewriteRuleITokenStream stream_RPAREN=new RewriteRuleITokenStream(adaptor,"token RPAREN");
		RewriteRuleSubtreeStream stream_unitTerm=new RewriteRuleSubtreeStream(adaptor,"rule unitTerm");
		RewriteRuleSubtreeStream stream_operators=new RewriteRuleSubtreeStream(adaptor,"rule operators");
		try { DebugEnterRule(GrammarFileName, "calcExpr");
		DebugLocation(365, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 45)) { return retval; }
			// CssGrammer.g3:366:5: ( 'calc' LPAREN unitTerm ( operators unitTerm )* RPAREN -> ^( CALC ( operators )* ( unitTerm )+ ) )
			DebugEnterAlt(1);
			// CssGrammer.g3:366:7: 'calc' LPAREN unitTerm ( operators unitTerm )* RPAREN
			{
			DebugLocation(366, 7);
			string_literal152=(CommonToken)Match(input,148,Follow._148_in_calcExpr2634); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_148.Add(string_literal152);

			DebugLocation(366, 14);
			LPAREN153=(CommonToken)Match(input,LPAREN,Follow._LPAREN_in_calcExpr2636); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_LPAREN.Add(LPAREN153);

			DebugLocation(366, 21);
			PushFollow(Follow._unitTerm_in_calcExpr2638);
			unitTerm154=unitTerm();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) stream_unitTerm.Add(unitTerm154.Tree);
			DebugLocation(366, 30);
			// CssGrammer.g3:366:30: ( operators unitTerm )*
			try { DebugEnterSubRule(44);
			while (true)
			{
				int alt44=2;
				try { DebugEnterDecision(44, decisionCanBacktrack[44]);
				int LA44_0 = input.LA(1);

				if ((LA44_0==MINUS||LA44_0==PLUS||(LA44_0>=SOLIDUS && LA44_0<=STAR)))
				{
					alt44=1;
				}


				} finally { DebugExitDecision(44); }
				switch ( alt44 )
				{
				case 1:
					DebugEnterAlt(1);
					// CssGrammer.g3:366:31: operators unitTerm
					{
					DebugLocation(366, 31);
					PushFollow(Follow._operators_in_calcExpr2641);
					operators155=operators();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_operators.Add(operators155.Tree);
					DebugLocation(366, 41);
					PushFollow(Follow._unitTerm_in_calcExpr2643);
					unitTerm156=unitTerm();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_unitTerm.Add(unitTerm156.Tree);

					}
					break;

				default:
					goto loop44;
				}
			}

			loop44:
				;

			} finally { DebugExitSubRule(44); }

			DebugLocation(366, 52);
			RPAREN157=(CommonToken)Match(input,RPAREN,Follow._RPAREN_in_calcExpr2647); if (state.failed) return retval; 
			if ( state.backtracking == 0 ) stream_RPAREN.Add(RPAREN157);



			{
			// AST REWRITE
			// elements: operators, unitTerm
			// token labels: 
			// rule labels: retval
			// token list labels: 
			// rule list labels: 
			// wildcard labels: 
			if ( state.backtracking == 0 ) {
			retval.Tree = root_0;
			RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

			root_0 = (CommonTree)adaptor.Nil();
			// 366:59: -> ^( CALC ( operators )* ( unitTerm )+ )
			{
				DebugLocation(366, 62);
				// CssGrammer.g3:366:62: ^( CALC ( operators )* ( unitTerm )+ )
				{
				CommonTree root_1 = (CommonTree)adaptor.Nil();
				DebugLocation(366, 64);
				root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(CALC, "CALC"), root_1);

				DebugLocation(366, 69);
				// CssGrammer.g3:366:69: ( operators )*
				while ( stream_operators.HasNext )
				{
					DebugLocation(366, 69);
					adaptor.AddChild(root_1, stream_operators.NextTree());

				}
				stream_operators.Reset();
				DebugLocation(366, 80);
				if ( !(stream_unitTerm.HasNext) )
				{
					throw new RewriteEarlyExitException();
				}
				while ( stream_unitTerm.HasNext )
				{
					DebugLocation(366, 80);
					adaptor.AddChild(root_1, stream_unitTerm.NextTree());

				}
				stream_unitTerm.Reset();

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
			TraceOut("calcExpr", 45);
			LeaveRule("calcExpr", 45);
			Leave_calcExpr();
			if (state.backtracking > 0) { Memoize(input, 45, calcExpr_StartIndex); }
		}
		DebugLocation(367, 4);
		} finally { DebugExitRule(GrammarFileName, "calcExpr"); }
		return retval;

	}
	// $ANTLR end "calcExpr"

	public class operators_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_operators();
	partial void Leave_operators();

	// $ANTLR start "operators"
	// CssGrammer.g3:369:1: operators : ( PLUS | MINUS | STAR | SOLIDUS );
	[GrammarRule("operators")]
	private CssGrammerParser.operators_return operators()
	{
		Enter_operators();
		EnterRule("operators", 46);
		TraceIn("operators", 46);
		CssGrammerParser.operators_return retval = new CssGrammerParser.operators_return();
		retval.Start = (CommonToken)input.LT(1);
		int operators_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken set158=null;

		CommonTree set158_tree=null;

		try { DebugEnterRule(GrammarFileName, "operators");
		DebugLocation(369, 13);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 46)) { return retval; }
			// CssGrammer.g3:370:5: ( PLUS | MINUS | STAR | SOLIDUS )
			DebugEnterAlt(1);
			// CssGrammer.g3:
			{
			root_0 = (CommonTree)adaptor.Nil();

			DebugLocation(370, 5);
			set158=(CommonToken)input.LT(1);
			if (input.LA(1)==MINUS||input.LA(1)==PLUS||(input.LA(1)>=SOLIDUS && input.LA(1)<=STAR))
			{
				input.Consume();
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, (CommonTree)adaptor.Create(set158));
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
			TraceOut("operators", 46);
			LeaveRule("operators", 46);
			Leave_operators();
			if (state.backtracking > 0) { Memoize(input, 46, operators_StartIndex); }
		}
		DebugLocation(373, 13);
		} finally { DebugExitRule(GrammarFileName, "operators"); }
		return retval;

	}
	// $ANTLR end "operators"

	public class term_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_term();
	partial void Leave_term();

	// $ANTLR start "term"
	// CssGrammer.g3:375:1: term : ( identifier -> ^( IDENTIFIER identifier ) | STRING -> ^( STRING_VAL STRING ) | color | calcExpr | URI -> ^( URL_VAL URI ) | (oper= unaryOperator )? val= unitTerm -> ^( UNIT_VAL $val ( $oper)? ) | identifier LPAREN funcArgs RPAREN -> ^( FUNCTION identifier funcArgs ) | legacyFilterMethod LPAREN funcArgs RPAREN -> ^( FUNCTION legacyFilterMethod funcArgs ) | unitExpr );
	[GrammarRule("term")]
	private CssGrammerParser.term_return term()
	{
		Enter_term();
		EnterRule("term", 47);
		TraceIn("term", 47);
		CssGrammerParser.term_return retval = new CssGrammerParser.term_return();
		retval.Start = (CommonToken)input.LT(1);
		int term_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken STRING160=null;
		CommonToken URI163=null;
		CommonToken LPAREN165=null;
		CommonToken RPAREN167=null;
		CommonToken LPAREN169=null;
		CommonToken RPAREN171=null;
		CssGrammerParser.unaryOperator_return oper = default(CssGrammerParser.unaryOperator_return);
		CssGrammerParser.unitTerm_return val = default(CssGrammerParser.unitTerm_return);
		CssGrammerParser.identifier_return identifier159 = default(CssGrammerParser.identifier_return);
		CssGrammerParser.color_return color161 = default(CssGrammerParser.color_return);
		CssGrammerParser.calcExpr_return calcExpr162 = default(CssGrammerParser.calcExpr_return);
		CssGrammerParser.identifier_return identifier164 = default(CssGrammerParser.identifier_return);
		CssGrammerParser.funcArgs_return funcArgs166 = default(CssGrammerParser.funcArgs_return);
		CssGrammerParser.legacyFilterMethod_return legacyFilterMethod168 = default(CssGrammerParser.legacyFilterMethod_return);
		CssGrammerParser.funcArgs_return funcArgs170 = default(CssGrammerParser.funcArgs_return);
		CssGrammerParser.unitExpr_return unitExpr172 = default(CssGrammerParser.unitExpr_return);

		CommonTree STRING160_tree=null;
		CommonTree URI163_tree=null;
		CommonTree LPAREN165_tree=null;
		CommonTree RPAREN167_tree=null;
		CommonTree LPAREN169_tree=null;
		CommonTree RPAREN171_tree=null;
		RewriteRuleITokenStream stream_STRING=new RewriteRuleITokenStream(adaptor,"token STRING");
		RewriteRuleITokenStream stream_URI=new RewriteRuleITokenStream(adaptor,"token URI");
		RewriteRuleITokenStream stream_LPAREN=new RewriteRuleITokenStream(adaptor,"token LPAREN");
		RewriteRuleITokenStream stream_RPAREN=new RewriteRuleITokenStream(adaptor,"token RPAREN");
		RewriteRuleSubtreeStream stream_identifier=new RewriteRuleSubtreeStream(adaptor,"rule identifier");
		RewriteRuleSubtreeStream stream_unaryOperator=new RewriteRuleSubtreeStream(adaptor,"rule unaryOperator");
		RewriteRuleSubtreeStream stream_unitTerm=new RewriteRuleSubtreeStream(adaptor,"rule unitTerm");
		RewriteRuleSubtreeStream stream_funcArgs=new RewriteRuleSubtreeStream(adaptor,"rule funcArgs");
		RewriteRuleSubtreeStream stream_legacyFilterMethod=new RewriteRuleSubtreeStream(adaptor,"rule legacyFilterMethod");
		try { DebugEnterRule(GrammarFileName, "term");
		DebugLocation(375, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 47)) { return retval; }
			// CssGrammer.g3:376:5: ( identifier -> ^( IDENTIFIER identifier ) | STRING -> ^( STRING_VAL STRING ) | color | calcExpr | URI -> ^( URL_VAL URI ) | (oper= unaryOperator )? val= unitTerm -> ^( UNIT_VAL $val ( $oper)? ) | identifier LPAREN funcArgs RPAREN -> ^( FUNCTION identifier funcArgs ) | legacyFilterMethod LPAREN funcArgs RPAREN -> ^( FUNCTION legacyFilterMethod funcArgs ) | unitExpr )
			int alt46=9;
			try { DebugEnterDecision(46, decisionCanBacktrack[46]);
			try
			{
				alt46 = dfa46.Predict(input);
			}
			catch (NoViableAltException nvae)
			{
				DebugRecognitionException(nvae);
				throw;
			}
			} finally { DebugExitDecision(46); }
			switch (alt46)
			{
			case 1:
				DebugEnterAlt(1);
				// CssGrammer.g3:376:7: identifier
				{
				DebugLocation(376, 7);
				PushFollow(Follow._identifier_in_term2712);
				identifier159=identifier();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_identifier.Add(identifier159.Tree);


				{
				// AST REWRITE
				// elements: identifier
				// token labels: 
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.Tree = root_0;
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

				root_0 = (CommonTree)adaptor.Nil();
				// 376:18: -> ^( IDENTIFIER identifier )
				{
					DebugLocation(376, 21);
					// CssGrammer.g3:376:21: ^( IDENTIFIER identifier )
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(376, 23);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(IDENTIFIER, "IDENTIFIER"), root_1);

					DebugLocation(376, 34);
					adaptor.AddChild(root_1, stream_identifier.NextTree());

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
				// CssGrammer.g3:377:7: STRING
				{
				DebugLocation(377, 7);
				STRING160=(CommonToken)Match(input,STRING,Follow._STRING_in_term2728); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_STRING.Add(STRING160);



				{
				// AST REWRITE
				// elements: STRING
				// token labels: 
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.Tree = root_0;
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

				root_0 = (CommonTree)adaptor.Nil();
				// 377:14: -> ^( STRING_VAL STRING )
				{
					DebugLocation(377, 17);
					// CssGrammer.g3:377:17: ^( STRING_VAL STRING )
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(377, 19);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(STRING_VAL, "STRING_VAL"), root_1);

					DebugLocation(377, 30);
					adaptor.AddChild(root_1, stream_STRING.NextNode());

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
				// CssGrammer.g3:378:7: color
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(378, 7);
				PushFollow(Follow._color_in_term2744);
				color161=color();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, color161.Tree);

				}
				break;
			case 4:
				DebugEnterAlt(4);
				// CssGrammer.g3:379:7: calcExpr
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(379, 7);
				PushFollow(Follow._calcExpr_in_term2752);
				calcExpr162=calcExpr();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, calcExpr162.Tree);

				}
				break;
			case 5:
				DebugEnterAlt(5);
				// CssGrammer.g3:380:7: URI
				{
				DebugLocation(380, 7);
				URI163=(CommonToken)Match(input,URI,Follow._URI_in_term2760); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_URI.Add(URI163);



				{
				// AST REWRITE
				// elements: URI
				// token labels: 
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.Tree = root_0;
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

				root_0 = (CommonTree)adaptor.Nil();
				// 380:11: -> ^( URL_VAL URI )
				{
					DebugLocation(380, 14);
					// CssGrammer.g3:380:14: ^( URL_VAL URI )
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(380, 16);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(URL_VAL, "URL_VAL"), root_1);

					DebugLocation(380, 24);
					adaptor.AddChild(root_1, stream_URI.NextNode());

					adaptor.AddChild(root_0, root_1);
					}

				}

				retval.Tree = root_0;
				}
				}

				}
				break;
			case 6:
				DebugEnterAlt(6);
				// CssGrammer.g3:381:7: (oper= unaryOperator )? val= unitTerm
				{
				DebugLocation(381, 7);
				// CssGrammer.g3:381:7: (oper= unaryOperator )?
				int alt45=2;
				try { DebugEnterSubRule(45);
				try { DebugEnterDecision(45, decisionCanBacktrack[45]);
				int LA45_0 = input.LA(1);

				if ((LA45_0==MINUS||LA45_0==PLUS))
				{
					alt45=1;
				}
				} finally { DebugExitDecision(45); }
				switch (alt45)
				{
				case 1:
					DebugEnterAlt(1);
					// CssGrammer.g3:381:8: oper= unaryOperator
					{
					DebugLocation(381, 12);
					PushFollow(Follow._unaryOperator_in_term2779);
					oper=unaryOperator();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_unaryOperator.Add(oper.Tree);

					}
					break;

				}
				} finally { DebugExitSubRule(45); }

				DebugLocation(381, 32);
				PushFollow(Follow._unitTerm_in_term2785);
				val=unitTerm();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_unitTerm.Add(val.Tree);


				{
				// AST REWRITE
				// elements: val, oper
				// token labels: 
				// rule labels: val, oper, retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.Tree = root_0;
				RewriteRuleSubtreeStream stream_val=new RewriteRuleSubtreeStream(adaptor,"rule val",val!=null?val.Tree:null);
				RewriteRuleSubtreeStream stream_oper=new RewriteRuleSubtreeStream(adaptor,"rule oper",oper!=null?oper.Tree:null);
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

				root_0 = (CommonTree)adaptor.Nil();
				// 381:42: -> ^( UNIT_VAL $val ( $oper)? )
				{
					DebugLocation(381, 45);
					// CssGrammer.g3:381:45: ^( UNIT_VAL $val ( $oper)? )
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(381, 47);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(UNIT_VAL, "UNIT_VAL"), root_1);

					DebugLocation(381, 57);
					adaptor.AddChild(root_1, stream_val.NextTree());
					DebugLocation(381, 62);
					// CssGrammer.g3:381:62: ( $oper)?
					if ( stream_oper.HasNext )
					{
						DebugLocation(381, 62);
						adaptor.AddChild(root_1, stream_oper.NextTree());

					}
					stream_oper.Reset();

					adaptor.AddChild(root_0, root_1);
					}

				}

				retval.Tree = root_0;
				}
				}

				}
				break;
			case 7:
				DebugEnterAlt(7);
				// CssGrammer.g3:382:7: identifier LPAREN funcArgs RPAREN
				{
				DebugLocation(382, 7);
				PushFollow(Follow._identifier_in_term2806);
				identifier164=identifier();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_identifier.Add(identifier164.Tree);
				DebugLocation(382, 18);
				LPAREN165=(CommonToken)Match(input,LPAREN,Follow._LPAREN_in_term2808); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_LPAREN.Add(LPAREN165);

				DebugLocation(382, 25);
				PushFollow(Follow._funcArgs_in_term2810);
				funcArgs166=funcArgs();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_funcArgs.Add(funcArgs166.Tree);
				DebugLocation(382, 34);
				RPAREN167=(CommonToken)Match(input,RPAREN,Follow._RPAREN_in_term2812); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_RPAREN.Add(RPAREN167);



				{
				// AST REWRITE
				// elements: identifier, funcArgs
				// token labels: 
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.Tree = root_0;
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

				root_0 = (CommonTree)adaptor.Nil();
				// 383:9: -> ^( FUNCTION identifier funcArgs )
				{
					DebugLocation(383, 12);
					// CssGrammer.g3:383:12: ^( FUNCTION identifier funcArgs )
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(383, 14);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(FUNCTION, "FUNCTION"), root_1);

					DebugLocation(383, 23);
					adaptor.AddChild(root_1, stream_identifier.NextTree());
					DebugLocation(383, 34);
					adaptor.AddChild(root_1, stream_funcArgs.NextTree());

					adaptor.AddChild(root_0, root_1);
					}

				}

				retval.Tree = root_0;
				}
				}

				}
				break;
			case 8:
				DebugEnterAlt(8);
				// CssGrammer.g3:384:7: legacyFilterMethod LPAREN funcArgs RPAREN
				{
				DebugLocation(384, 7);
				PushFollow(Follow._legacyFilterMethod_in_term2838);
				legacyFilterMethod168=legacyFilterMethod();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_legacyFilterMethod.Add(legacyFilterMethod168.Tree);
				DebugLocation(384, 26);
				LPAREN169=(CommonToken)Match(input,LPAREN,Follow._LPAREN_in_term2840); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_LPAREN.Add(LPAREN169);

				DebugLocation(384, 33);
				PushFollow(Follow._funcArgs_in_term2842);
				funcArgs170=funcArgs();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_funcArgs.Add(funcArgs170.Tree);
				DebugLocation(384, 42);
				RPAREN171=(CommonToken)Match(input,RPAREN,Follow._RPAREN_in_term2844); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_RPAREN.Add(RPAREN171);



				{
				// AST REWRITE
				// elements: legacyFilterMethod, funcArgs
				// token labels: 
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.Tree = root_0;
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

				root_0 = (CommonTree)adaptor.Nil();
				// 385:9: -> ^( FUNCTION legacyFilterMethod funcArgs )
				{
					DebugLocation(385, 12);
					// CssGrammer.g3:385:12: ^( FUNCTION legacyFilterMethod funcArgs )
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(385, 14);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(FUNCTION, "FUNCTION"), root_1);

					DebugLocation(385, 23);
					adaptor.AddChild(root_1, stream_legacyFilterMethod.NextTree());
					DebugLocation(385, 42);
					adaptor.AddChild(root_1, stream_funcArgs.NextTree());

					adaptor.AddChild(root_0, root_1);
					}

				}

				retval.Tree = root_0;
				}
				}

				}
				break;
			case 9:
				DebugEnterAlt(9);
				// CssGrammer.g3:386:7: unitExpr
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(386, 7);
				PushFollow(Follow._unitExpr_in_term2870);
				unitExpr172=unitExpr();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, unitExpr172.Tree);

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
			TraceOut("term", 47);
			LeaveRule("term", 47);
			Leave_term();
			if (state.backtracking > 0) { Memoize(input, 47, term_StartIndex); }
		}
		DebugLocation(387, 4);
		} finally { DebugExitRule(GrammarFileName, "term"); }
		return retval;

	}
	// $ANTLR end "term"

	public class legacyFilterMethod_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_legacyFilterMethod();
	partial void Leave_legacyFilterMethod();

	// $ANTLR start "legacyFilterMethod"
	// CssGrammer.g3:389:1: legacyFilterMethod : identifier COLON identifier ( DOT identifier )* ;
	[GrammarRule("legacyFilterMethod")]
	private CssGrammerParser.legacyFilterMethod_return legacyFilterMethod()
	{
		Enter_legacyFilterMethod();
		EnterRule("legacyFilterMethod", 48);
		TraceIn("legacyFilterMethod", 48);
		CssGrammerParser.legacyFilterMethod_return retval = new CssGrammerParser.legacyFilterMethod_return();
		retval.Start = (CommonToken)input.LT(1);
		int legacyFilterMethod_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken COLON174=null;
		CommonToken DOT176=null;
		CssGrammerParser.identifier_return identifier173 = default(CssGrammerParser.identifier_return);
		CssGrammerParser.identifier_return identifier175 = default(CssGrammerParser.identifier_return);
		CssGrammerParser.identifier_return identifier177 = default(CssGrammerParser.identifier_return);

		CommonTree COLON174_tree=null;
		CommonTree DOT176_tree=null;

		try { DebugEnterRule(GrammarFileName, "legacyFilterMethod");
		DebugLocation(389, 47);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 48)) { return retval; }
			// CssGrammer.g3:390:3: ( identifier COLON identifier ( DOT identifier )* )
			DebugEnterAlt(1);
			// CssGrammer.g3:390:3: identifier COLON identifier ( DOT identifier )*
			{
			root_0 = (CommonTree)adaptor.Nil();

			DebugLocation(390, 3);
			PushFollow(Follow._identifier_in_legacyFilterMethod2883);
			identifier173=identifier();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) adaptor.AddChild(root_0, identifier173.Tree);
			DebugLocation(390, 14);
			COLON174=(CommonToken)Match(input,COLON,Follow._COLON_in_legacyFilterMethod2885); if (state.failed) return retval;
			if ( state.backtracking==0 ) {
			COLON174_tree = (CommonTree)adaptor.Create(COLON174);
			adaptor.AddChild(root_0, COLON174_tree);
			}
			DebugLocation(390, 20);
			PushFollow(Follow._identifier_in_legacyFilterMethod2887);
			identifier175=identifier();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) adaptor.AddChild(root_0, identifier175.Tree);
			DebugLocation(390, 31);
			// CssGrammer.g3:390:31: ( DOT identifier )*
			try { DebugEnterSubRule(47);
			while (true)
			{
				int alt47=2;
				try { DebugEnterDecision(47, decisionCanBacktrack[47]);
				int LA47_0 = input.LA(1);

				if ((LA47_0==DOT))
				{
					alt47=1;
				}


				} finally { DebugExitDecision(47); }
				switch ( alt47 )
				{
				case 1:
					DebugEnterAlt(1);
					// CssGrammer.g3:390:32: DOT identifier
					{
					DebugLocation(390, 32);
					DOT176=(CommonToken)Match(input,DOT,Follow._DOT_in_legacyFilterMethod2890); if (state.failed) return retval;
					if ( state.backtracking==0 ) {
					DOT176_tree = (CommonTree)adaptor.Create(DOT176);
					adaptor.AddChild(root_0, DOT176_tree);
					}
					DebugLocation(390, 36);
					PushFollow(Follow._identifier_in_legacyFilterMethod2892);
					identifier177=identifier();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) adaptor.AddChild(root_0, identifier177.Tree);

					}
					break;

				default:
					goto loop47;
				}
			}

			loop47:
				;

			} finally { DebugExitSubRule(47); }


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
			TraceOut("legacyFilterMethod", 48);
			LeaveRule("legacyFilterMethod", 48);
			Leave_legacyFilterMethod();
			if (state.backtracking > 0) { Memoize(input, 48, legacyFilterMethod_StartIndex); }
		}
		DebugLocation(390, 47);
		} finally { DebugExitRule(GrammarFileName, "legacyFilterMethod"); }
		return retval;

	}
	// $ANTLR end "legacyFilterMethod"

	public class unitExpr_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_unitExpr();
	partial void Leave_unitExpr();

	// $ANTLR start "unitExpr"
	// CssGrammer.g3:392:1: unitExpr : ( identifier -> ^( IDENTIFIER identifier ) | STRING -> ^( STRING_VAL STRING ) | color | calcExpr | URI -> ^( URL_VAL URI ) | (oper= unaryOperator )? val= unitTerm -> ^( UNIT_VAL $val ( $oper)? ) );
	[GrammarRule("unitExpr")]
	private CssGrammerParser.unitExpr_return unitExpr()
	{
		Enter_unitExpr();
		EnterRule("unitExpr", 49);
		TraceIn("unitExpr", 49);
		CssGrammerParser.unitExpr_return retval = new CssGrammerParser.unitExpr_return();
		retval.Start = (CommonToken)input.LT(1);
		int unitExpr_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken STRING179=null;
		CommonToken URI182=null;
		CssGrammerParser.unaryOperator_return oper = default(CssGrammerParser.unaryOperator_return);
		CssGrammerParser.unitTerm_return val = default(CssGrammerParser.unitTerm_return);
		CssGrammerParser.identifier_return identifier178 = default(CssGrammerParser.identifier_return);
		CssGrammerParser.color_return color180 = default(CssGrammerParser.color_return);
		CssGrammerParser.calcExpr_return calcExpr181 = default(CssGrammerParser.calcExpr_return);

		CommonTree STRING179_tree=null;
		CommonTree URI182_tree=null;
		RewriteRuleITokenStream stream_STRING=new RewriteRuleITokenStream(adaptor,"token STRING");
		RewriteRuleITokenStream stream_URI=new RewriteRuleITokenStream(adaptor,"token URI");
		RewriteRuleSubtreeStream stream_identifier=new RewriteRuleSubtreeStream(adaptor,"rule identifier");
		RewriteRuleSubtreeStream stream_unaryOperator=new RewriteRuleSubtreeStream(adaptor,"rule unaryOperator");
		RewriteRuleSubtreeStream stream_unitTerm=new RewriteRuleSubtreeStream(adaptor,"rule unitTerm");
		try { DebugEnterRule(GrammarFileName, "unitExpr");
		DebugLocation(392, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 49)) { return retval; }
			// CssGrammer.g3:393:5: ( identifier -> ^( IDENTIFIER identifier ) | STRING -> ^( STRING_VAL STRING ) | color | calcExpr | URI -> ^( URL_VAL URI ) | (oper= unaryOperator )? val= unitTerm -> ^( UNIT_VAL $val ( $oper)? ) )
			int alt49=6;
			try { DebugEnterDecision(49, decisionCanBacktrack[49]);
			switch (input.LA(1))
			{
			case IDENT:
			case NOT_SYM:
			case TO_SYM:
				{
				alt49=1;
				}
				break;
			case STRING:
				{
				alt49=2;
				}
				break;
			case HASH:
			case 150:
			case 151:
				{
				alt49=3;
				}
				break;
			case 148:
				{
				alt49=4;
				}
				break;
			case URI:
				{
				alt49=5;
				}
				break;
			case ANGLE:
			case EMS:
			case EXS:
			case FREQ:
			case LENGTH:
			case MINUS:
			case NUMBER:
			case PERCENTAGE:
			case PLUS:
			case REM:
			case TIME:
				{
				alt49=6;
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
				// CssGrammer.g3:393:7: identifier
				{
				DebugLocation(393, 7);
				PushFollow(Follow._identifier_in_unitExpr2906);
				identifier178=identifier();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_identifier.Add(identifier178.Tree);


				{
				// AST REWRITE
				// elements: identifier
				// token labels: 
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.Tree = root_0;
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

				root_0 = (CommonTree)adaptor.Nil();
				// 393:18: -> ^( IDENTIFIER identifier )
				{
					DebugLocation(393, 21);
					// CssGrammer.g3:393:21: ^( IDENTIFIER identifier )
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(393, 23);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(IDENTIFIER, "IDENTIFIER"), root_1);

					DebugLocation(393, 34);
					adaptor.AddChild(root_1, stream_identifier.NextTree());

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
				// CssGrammer.g3:394:7: STRING
				{
				DebugLocation(394, 7);
				STRING179=(CommonToken)Match(input,STRING,Follow._STRING_in_unitExpr2922); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_STRING.Add(STRING179);



				{
				// AST REWRITE
				// elements: STRING
				// token labels: 
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.Tree = root_0;
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

				root_0 = (CommonTree)adaptor.Nil();
				// 394:14: -> ^( STRING_VAL STRING )
				{
					DebugLocation(394, 17);
					// CssGrammer.g3:394:17: ^( STRING_VAL STRING )
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(394, 19);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(STRING_VAL, "STRING_VAL"), root_1);

					DebugLocation(394, 30);
					adaptor.AddChild(root_1, stream_STRING.NextNode());

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
				// CssGrammer.g3:395:7: color
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(395, 7);
				PushFollow(Follow._color_in_unitExpr2938);
				color180=color();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, color180.Tree);

				}
				break;
			case 4:
				DebugEnterAlt(4);
				// CssGrammer.g3:396:7: calcExpr
				{
				root_0 = (CommonTree)adaptor.Nil();

				DebugLocation(396, 7);
				PushFollow(Follow._calcExpr_in_unitExpr2946);
				calcExpr181=calcExpr();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, calcExpr181.Tree);

				}
				break;
			case 5:
				DebugEnterAlt(5);
				// CssGrammer.g3:397:7: URI
				{
				DebugLocation(397, 7);
				URI182=(CommonToken)Match(input,URI,Follow._URI_in_unitExpr2954); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_URI.Add(URI182);



				{
				// AST REWRITE
				// elements: URI
				// token labels: 
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.Tree = root_0;
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

				root_0 = (CommonTree)adaptor.Nil();
				// 397:11: -> ^( URL_VAL URI )
				{
					DebugLocation(397, 14);
					// CssGrammer.g3:397:14: ^( URL_VAL URI )
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(397, 16);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(URL_VAL, "URL_VAL"), root_1);

					DebugLocation(397, 24);
					adaptor.AddChild(root_1, stream_URI.NextNode());

					adaptor.AddChild(root_0, root_1);
					}

				}

				retval.Tree = root_0;
				}
				}

				}
				break;
			case 6:
				DebugEnterAlt(6);
				// CssGrammer.g3:398:7: (oper= unaryOperator )? val= unitTerm
				{
				DebugLocation(398, 7);
				// CssGrammer.g3:398:7: (oper= unaryOperator )?
				int alt48=2;
				try { DebugEnterSubRule(48);
				try { DebugEnterDecision(48, decisionCanBacktrack[48]);
				int LA48_0 = input.LA(1);

				if ((LA48_0==MINUS||LA48_0==PLUS))
				{
					alt48=1;
				}
				} finally { DebugExitDecision(48); }
				switch (alt48)
				{
				case 1:
					DebugEnterAlt(1);
					// CssGrammer.g3:398:8: oper= unaryOperator
					{
					DebugLocation(398, 12);
					PushFollow(Follow._unaryOperator_in_unitExpr2973);
					oper=unaryOperator();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) stream_unaryOperator.Add(oper.Tree);

					}
					break;

				}
				} finally { DebugExitSubRule(48); }

				DebugLocation(398, 32);
				PushFollow(Follow._unitTerm_in_unitExpr2979);
				val=unitTerm();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_unitTerm.Add(val.Tree);


				{
				// AST REWRITE
				// elements: val, oper
				// token labels: 
				// rule labels: val, oper, retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.Tree = root_0;
				RewriteRuleSubtreeStream stream_val=new RewriteRuleSubtreeStream(adaptor,"rule val",val!=null?val.Tree:null);
				RewriteRuleSubtreeStream stream_oper=new RewriteRuleSubtreeStream(adaptor,"rule oper",oper!=null?oper.Tree:null);
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

				root_0 = (CommonTree)adaptor.Nil();
				// 398:42: -> ^( UNIT_VAL $val ( $oper)? )
				{
					DebugLocation(398, 45);
					// CssGrammer.g3:398:45: ^( UNIT_VAL $val ( $oper)? )
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(398, 47);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(UNIT_VAL, "UNIT_VAL"), root_1);

					DebugLocation(398, 57);
					adaptor.AddChild(root_1, stream_val.NextTree());
					DebugLocation(398, 62);
					// CssGrammer.g3:398:62: ( $oper)?
					if ( stream_oper.HasNext )
					{
						DebugLocation(398, 62);
						adaptor.AddChild(root_1, stream_oper.NextTree());

					}
					stream_oper.Reset();

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
			TraceOut("unitExpr", 49);
			LeaveRule("unitExpr", 49);
			Leave_unitExpr();
			if (state.backtracking > 0) { Memoize(input, 49, unitExpr_StartIndex); }
		}
		DebugLocation(399, 4);
		} finally { DebugExitRule(GrammarFileName, "unitExpr"); }
		return retval;

	}
	// $ANTLR end "unitExpr"

	public class funcArg_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_funcArg();
	partial void Leave_funcArg();

	// $ANTLR start "funcArg"
	// CssGrammer.g3:401:1: funcArg : ( ( unitExpr )+ -> ^( UNITEXPRS ( unitExpr )+ ) | identifier '=' funcArg -> ^( ASSIGN_EXPR identifier funcArg ) |id= 'color-stop' LPAREN funcArgs RPAREN -> ^( FUNCTION $id funcArgs ) );
	[GrammarRule("funcArg")]
	private CssGrammerParser.funcArg_return funcArg()
	{
		Enter_funcArg();
		EnterRule("funcArg", 50);
		TraceIn("funcArg", 50);
		CssGrammerParser.funcArg_return retval = new CssGrammerParser.funcArg_return();
		retval.Start = (CommonToken)input.LT(1);
		int funcArg_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken id=null;
		CommonToken char_literal185=null;
		CommonToken LPAREN187=null;
		CommonToken RPAREN189=null;
		CssGrammerParser.unitExpr_return unitExpr183 = default(CssGrammerParser.unitExpr_return);
		CssGrammerParser.identifier_return identifier184 = default(CssGrammerParser.identifier_return);
		CssGrammerParser.funcArg_return funcArg186 = default(CssGrammerParser.funcArg_return);
		CssGrammerParser.funcArgs_return funcArgs188 = default(CssGrammerParser.funcArgs_return);

		CommonTree id_tree=null;
		CommonTree char_literal185_tree=null;
		CommonTree LPAREN187_tree=null;
		CommonTree RPAREN189_tree=null;
		RewriteRuleITokenStream stream_OPEQ=new RewriteRuleITokenStream(adaptor,"token OPEQ");
		RewriteRuleITokenStream stream_149=new RewriteRuleITokenStream(adaptor,"token 149");
		RewriteRuleITokenStream stream_LPAREN=new RewriteRuleITokenStream(adaptor,"token LPAREN");
		RewriteRuleITokenStream stream_RPAREN=new RewriteRuleITokenStream(adaptor,"token RPAREN");
		RewriteRuleSubtreeStream stream_unitExpr=new RewriteRuleSubtreeStream(adaptor,"rule unitExpr");
		RewriteRuleSubtreeStream stream_identifier=new RewriteRuleSubtreeStream(adaptor,"rule identifier");
		RewriteRuleSubtreeStream stream_funcArg=new RewriteRuleSubtreeStream(adaptor,"rule funcArg");
		RewriteRuleSubtreeStream stream_funcArgs=new RewriteRuleSubtreeStream(adaptor,"rule funcArgs");
		try { DebugEnterRule(GrammarFileName, "funcArg");
		DebugLocation(401, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 50)) { return retval; }
			// CssGrammer.g3:402:5: ( ( unitExpr )+ -> ^( UNITEXPRS ( unitExpr )+ ) | identifier '=' funcArg -> ^( ASSIGN_EXPR identifier funcArg ) |id= 'color-stop' LPAREN funcArgs RPAREN -> ^( FUNCTION $id funcArgs ) )
			int alt51=3;
			try { DebugEnterDecision(51, decisionCanBacktrack[51]);
			switch (input.LA(1))
			{
			case IDENT:
			case NOT_SYM:
			case TO_SYM:
				{
				int LA51_1 = input.LA(2);

				if ((LA51_1==EOF||LA51_1==ANGLE||LA51_1==COMMA||LA51_1==EMS||LA51_1==EXS||LA51_1==FREQ||LA51_1==HASH||LA51_1==IDENT||LA51_1==LENGTH||LA51_1==MINUS||LA51_1==NOT_SYM||LA51_1==NUMBER||(LA51_1>=PERCENTAGE && LA51_1<=PLUS)||LA51_1==REM||LA51_1==RPAREN||LA51_1==STRING||(LA51_1>=TIME && LA51_1<=TO_SYM)||LA51_1==URI||LA51_1==148||(LA51_1>=150 && LA51_1<=151)))
				{
					alt51=1;
				}
				else if ((LA51_1==OPEQ))
				{
					alt51=2;
				}
				else
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 51, 1, input);

					DebugRecognitionException(nvae);
					throw nvae;
				}
				}
				break;
			case ANGLE:
			case EMS:
			case EXS:
			case FREQ:
			case HASH:
			case LENGTH:
			case MINUS:
			case NUMBER:
			case PERCENTAGE:
			case PLUS:
			case REM:
			case STRING:
			case TIME:
			case URI:
			case 148:
			case 150:
			case 151:
				{
				alt51=1;
				}
				break;
			case 149:
				{
				alt51=3;
				}
				break;
			default:
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 51, 0, input);

					DebugRecognitionException(nvae);
					throw nvae;
				}
			}

			} finally { DebugExitDecision(51); }
			switch (alt51)
			{
			case 1:
				DebugEnterAlt(1);
				// CssGrammer.g3:402:7: ( unitExpr )+
				{
				DebugLocation(402, 7);
				// CssGrammer.g3:402:7: ( unitExpr )+
				int cnt50=0;
				try { DebugEnterSubRule(50);
				while (true)
				{
					int alt50=2;
					try { DebugEnterDecision(50, decisionCanBacktrack[50]);
					int LA50_0 = input.LA(1);

					if ((LA50_0==ANGLE||LA50_0==EMS||LA50_0==EXS||LA50_0==FREQ||LA50_0==HASH||LA50_0==IDENT||LA50_0==LENGTH||LA50_0==MINUS||LA50_0==NOT_SYM||LA50_0==NUMBER||(LA50_0>=PERCENTAGE && LA50_0<=PLUS)||LA50_0==REM||LA50_0==STRING||(LA50_0>=TIME && LA50_0<=TO_SYM)||LA50_0==URI||LA50_0==148||(LA50_0>=150 && LA50_0<=151)))
					{
						alt50=1;
					}


					} finally { DebugExitDecision(50); }
					switch (alt50)
					{
					case 1:
						DebugEnterAlt(1);
						// CssGrammer.g3:402:7: unitExpr
						{
						DebugLocation(402, 7);
						PushFollow(Follow._unitExpr_in_funcArg3013);
						unitExpr183=unitExpr();
						PopFollow();
						if (state.failed) return retval;
						if ( state.backtracking == 0 ) stream_unitExpr.Add(unitExpr183.Tree);

						}
						break;

					default:
						if (cnt50 >= 1)
							goto loop50;

						if (state.backtracking>0) {state.failed=true; return retval;}
						EarlyExitException eee50 = new EarlyExitException( 50, input );
						DebugRecognitionException(eee50);
						throw eee50;
					}
					cnt50++;
				}
				loop50:
					;

				} finally { DebugExitSubRule(50); }



				{
				// AST REWRITE
				// elements: unitExpr
				// token labels: 
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.Tree = root_0;
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

				root_0 = (CommonTree)adaptor.Nil();
				// 402:17: -> ^( UNITEXPRS ( unitExpr )+ )
				{
					DebugLocation(402, 20);
					// CssGrammer.g3:402:20: ^( UNITEXPRS ( unitExpr )+ )
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(402, 22);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(UNITEXPRS, "UNITEXPRS"), root_1);

					DebugLocation(402, 32);
					if ( !(stream_unitExpr.HasNext) )
					{
						throw new RewriteEarlyExitException();
					}
					while ( stream_unitExpr.HasNext )
					{
						DebugLocation(402, 32);
						adaptor.AddChild(root_1, stream_unitExpr.NextTree());

					}
					stream_unitExpr.Reset();

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
				// CssGrammer.g3:403:7: identifier '=' funcArg
				{
				DebugLocation(403, 7);
				PushFollow(Follow._identifier_in_funcArg3031);
				identifier184=identifier();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_identifier.Add(identifier184.Tree);
				DebugLocation(403, 18);
				char_literal185=(CommonToken)Match(input,OPEQ,Follow._OPEQ_in_funcArg3033); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_OPEQ.Add(char_literal185);

				DebugLocation(403, 22);
				PushFollow(Follow._funcArg_in_funcArg3035);
				funcArg186=funcArg();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_funcArg.Add(funcArg186.Tree);


				{
				// AST REWRITE
				// elements: identifier, funcArg
				// token labels: 
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.Tree = root_0;
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

				root_0 = (CommonTree)adaptor.Nil();
				// 403:30: -> ^( ASSIGN_EXPR identifier funcArg )
				{
					DebugLocation(403, 33);
					// CssGrammer.g3:403:33: ^( ASSIGN_EXPR identifier funcArg )
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(403, 35);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(ASSIGN_EXPR, "ASSIGN_EXPR"), root_1);

					DebugLocation(403, 47);
					adaptor.AddChild(root_1, stream_identifier.NextTree());
					DebugLocation(403, 58);
					adaptor.AddChild(root_1, stream_funcArg.NextTree());

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
				// CssGrammer.g3:404:7: id= 'color-stop' LPAREN funcArgs RPAREN
				{
				DebugLocation(404, 9);
				id=(CommonToken)Match(input,149,Follow._149_in_funcArg3055); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_149.Add(id);

				DebugLocation(404, 23);
				LPAREN187=(CommonToken)Match(input,LPAREN,Follow._LPAREN_in_funcArg3057); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_LPAREN.Add(LPAREN187);

				DebugLocation(404, 30);
				PushFollow(Follow._funcArgs_in_funcArg3059);
				funcArgs188=funcArgs();
				PopFollow();
				if (state.failed) return retval;
				if ( state.backtracking == 0 ) stream_funcArgs.Add(funcArgs188.Tree);
				DebugLocation(404, 39);
				RPAREN189=(CommonToken)Match(input,RPAREN,Follow._RPAREN_in_funcArg3061); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_RPAREN.Add(RPAREN189);



				{
				// AST REWRITE
				// elements: id, funcArgs
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
				// 404:46: -> ^( FUNCTION $id funcArgs )
				{
					DebugLocation(404, 49);
					// CssGrammer.g3:404:49: ^( FUNCTION $id funcArgs )
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(404, 51);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(FUNCTION, "FUNCTION"), root_1);

					DebugLocation(404, 61);
					adaptor.AddChild(root_1, stream_id.NextNode());
					DebugLocation(404, 64);
					adaptor.AddChild(root_1, stream_funcArgs.NextTree());

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
			TraceOut("funcArg", 50);
			LeaveRule("funcArg", 50);
			Leave_funcArg();
			if (state.backtracking > 0) { Memoize(input, 50, funcArg_StartIndex); }
		}
		DebugLocation(405, 4);
		} finally { DebugExitRule(GrammarFileName, "funcArg"); }
		return retval;

	}
	// $ANTLR end "funcArg"

	public class funcArgs_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_funcArgs();
	partial void Leave_funcArgs();

	// $ANTLR start "funcArgs"
	// CssGrammer.g3:407:1: funcArgs : funcArg ( COMMA funcArg )* ;
	[GrammarRule("funcArgs")]
	private CssGrammerParser.funcArgs_return funcArgs()
	{
		Enter_funcArgs();
		EnterRule("funcArgs", 51);
		TraceIn("funcArgs", 51);
		CssGrammerParser.funcArgs_return retval = new CssGrammerParser.funcArgs_return();
		retval.Start = (CommonToken)input.LT(1);
		int funcArgs_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken COMMA191=null;
		CssGrammerParser.funcArg_return funcArg190 = default(CssGrammerParser.funcArg_return);
		CssGrammerParser.funcArg_return funcArg192 = default(CssGrammerParser.funcArg_return);

		CommonTree COMMA191_tree=null;

		try { DebugEnterRule(GrammarFileName, "funcArgs");
		DebugLocation(407, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 51)) { return retval; }
			// CssGrammer.g3:408:5: ( funcArg ( COMMA funcArg )* )
			DebugEnterAlt(1);
			// CssGrammer.g3:408:7: funcArg ( COMMA funcArg )*
			{
			root_0 = (CommonTree)adaptor.Nil();

			DebugLocation(408, 7);
			PushFollow(Follow._funcArg_in_funcArgs3089);
			funcArg190=funcArg();
			PopFollow();
			if (state.failed) return retval;
			if ( state.backtracking == 0 ) adaptor.AddChild(root_0, funcArg190.Tree);
			DebugLocation(408, 15);
			// CssGrammer.g3:408:15: ( COMMA funcArg )*
			try { DebugEnterSubRule(52);
			while (true)
			{
				int alt52=2;
				try { DebugEnterDecision(52, decisionCanBacktrack[52]);
				int LA52_0 = input.LA(1);

				if ((LA52_0==COMMA))
				{
					alt52=1;
				}


				} finally { DebugExitDecision(52); }
				switch ( alt52 )
				{
				case 1:
					DebugEnterAlt(1);
					// CssGrammer.g3:408:16: COMMA funcArg
					{
					DebugLocation(408, 16);
					COMMA191=(CommonToken)Match(input,COMMA,Follow._COMMA_in_funcArgs3092); if (state.failed) return retval;
					if ( state.backtracking==0 ) {
					COMMA191_tree = (CommonTree)adaptor.Create(COMMA191);
					adaptor.AddChild(root_0, COMMA191_tree);
					}
					DebugLocation(408, 22);
					PushFollow(Follow._funcArg_in_funcArgs3094);
					funcArg192=funcArg();
					PopFollow();
					if (state.failed) return retval;
					if ( state.backtracking == 0 ) adaptor.AddChild(root_0, funcArg192.Tree);

					}
					break;

				default:
					goto loop52;
				}
			}

			loop52:
				;

			} finally { DebugExitSubRule(52); }


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
			TraceOut("funcArgs", 51);
			LeaveRule("funcArgs", 51);
			Leave_funcArgs();
			if (state.backtracking > 0) { Memoize(input, 51, funcArgs_StartIndex); }
		}
		DebugLocation(409, 4);
		} finally { DebugExitRule(GrammarFileName, "funcArgs"); }
		return retval;

	}
	// $ANTLR end "funcArgs"

	public class unitTerm_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_unitTerm();
	partial void Leave_unitTerm();

	// $ANTLR start "unitTerm"
	// CssGrammer.g3:411:1: unitTerm : ( PERCENTAGE | LENGTH | REM | EMS | EXS | ANGLE | TIME | FREQ | NUMBER );
	[GrammarRule("unitTerm")]
	private CssGrammerParser.unitTerm_return unitTerm()
	{
		Enter_unitTerm();
		EnterRule("unitTerm", 52);
		TraceIn("unitTerm", 52);
		CssGrammerParser.unitTerm_return retval = new CssGrammerParser.unitTerm_return();
		retval.Start = (CommonToken)input.LT(1);
		int unitTerm_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken set193=null;

		CommonTree set193_tree=null;

		try { DebugEnterRule(GrammarFileName, "unitTerm");
		DebugLocation(411, 12);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 52)) { return retval; }
			// CssGrammer.g3:412:5: ( PERCENTAGE | LENGTH | REM | EMS | EXS | ANGLE | TIME | FREQ | NUMBER )
			DebugEnterAlt(1);
			// CssGrammer.g3:
			{
			root_0 = (CommonTree)adaptor.Nil();

			DebugLocation(412, 5);
			set193=(CommonToken)input.LT(1);
			if (input.LA(1)==ANGLE||input.LA(1)==EMS||input.LA(1)==EXS||input.LA(1)==FREQ||input.LA(1)==LENGTH||input.LA(1)==NUMBER||input.LA(1)==PERCENTAGE||input.LA(1)==REM||input.LA(1)==TIME)
			{
				input.Consume();
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, (CommonTree)adaptor.Create(set193));
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
			TraceOut("unitTerm", 52);
			LeaveRule("unitTerm", 52);
			Leave_unitTerm();
			if (state.backtracking > 0) { Memoize(input, 52, unitTerm_StartIndex); }
		}
		DebugLocation(420, 12);
		} finally { DebugExitRule(GrammarFileName, "unitTerm"); }
		return retval;

	}
	// $ANTLR end "unitTerm"

	public class identifier_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_identifier();
	partial void Leave_identifier();

	// $ANTLR start "identifier"
	// CssGrammer.g3:422:1: identifier : ( IDENT | NOT_SYM | TO_SYM );
	[GrammarRule("identifier")]
	private CssGrammerParser.identifier_return identifier()
	{
		Enter_identifier();
		EnterRule("identifier", 53);
		TraceIn("identifier", 53);
		CssGrammerParser.identifier_return retval = new CssGrammerParser.identifier_return();
		retval.Start = (CommonToken)input.LT(1);
		int identifier_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken set194=null;

		CommonTree set194_tree=null;

		try { DebugEnterRule(GrammarFileName, "identifier");
		DebugLocation(422, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 53)) { return retval; }
			// CssGrammer.g3:423:5: ( IDENT | NOT_SYM | TO_SYM )
			DebugEnterAlt(1);
			// CssGrammer.g3:
			{
			root_0 = (CommonTree)adaptor.Nil();

			DebugLocation(423, 5);
			set194=(CommonToken)input.LT(1);
			if (input.LA(1)==IDENT||input.LA(1)==NOT_SYM||input.LA(1)==TO_SYM)
			{
				input.Consume();
				if ( state.backtracking == 0 ) adaptor.AddChild(root_0, (CommonTree)adaptor.Create(set194));
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
			TraceOut("identifier", 53);
			LeaveRule("identifier", 53);
			Leave_identifier();
			if (state.backtracking > 0) { Memoize(input, 53, identifier_StartIndex); }
		}
		DebugLocation(426, 4);
		} finally { DebugExitRule(GrammarFileName, "identifier"); }
		return retval;

	}
	// $ANTLR end "identifier"

	public class color_return : ParserRuleReturnScope<CommonToken>, IAstRuleReturnScope<CommonTree>
	{
		private CommonTree _tree;
		public CommonTree Tree { get { return _tree; } set { _tree = value; } }
	}

	partial void Enter_color();
	partial void Leave_color();

	// $ANTLR start "color"
	// CssGrammer.g3:428:1: color : ( 'rgba' LPAREN n1= NUMBER COMMA n2= NUMBER COMMA n3= NUMBER COMMA n4= NUMBER RPAREN -> ^( RGBA $n1 $n2 $n3 $n4) | 'rgb' LPAREN n1= NUMBER COMMA n2= NUMBER COMMA n3= NUMBER RPAREN -> ^( RGBA $n1 $n2 $n3) |cl= HASH -> ^( COLOR $cl) );
	[GrammarRule("color")]
	private CssGrammerParser.color_return color()
	{
		Enter_color();
		EnterRule("color", 54);
		TraceIn("color", 54);
		CssGrammerParser.color_return retval = new CssGrammerParser.color_return();
		retval.Start = (CommonToken)input.LT(1);
		int color_StartIndex = input.Index;
		CommonTree root_0 = null;

		CommonToken n1=null;
		CommonToken n2=null;
		CommonToken n3=null;
		CommonToken n4=null;
		CommonToken cl=null;
		CommonToken string_literal195=null;
		CommonToken LPAREN196=null;
		CommonToken COMMA197=null;
		CommonToken COMMA198=null;
		CommonToken COMMA199=null;
		CommonToken RPAREN200=null;
		CommonToken string_literal201=null;
		CommonToken LPAREN202=null;
		CommonToken COMMA203=null;
		CommonToken COMMA204=null;
		CommonToken RPAREN205=null;

		CommonTree n1_tree=null;
		CommonTree n2_tree=null;
		CommonTree n3_tree=null;
		CommonTree n4_tree=null;
		CommonTree cl_tree=null;
		CommonTree string_literal195_tree=null;
		CommonTree LPAREN196_tree=null;
		CommonTree COMMA197_tree=null;
		CommonTree COMMA198_tree=null;
		CommonTree COMMA199_tree=null;
		CommonTree RPAREN200_tree=null;
		CommonTree string_literal201_tree=null;
		CommonTree LPAREN202_tree=null;
		CommonTree COMMA203_tree=null;
		CommonTree COMMA204_tree=null;
		CommonTree RPAREN205_tree=null;
		RewriteRuleITokenStream stream_151=new RewriteRuleITokenStream(adaptor,"token 151");
		RewriteRuleITokenStream stream_LPAREN=new RewriteRuleITokenStream(adaptor,"token LPAREN");
		RewriteRuleITokenStream stream_NUMBER=new RewriteRuleITokenStream(adaptor,"token NUMBER");
		RewriteRuleITokenStream stream_COMMA=new RewriteRuleITokenStream(adaptor,"token COMMA");
		RewriteRuleITokenStream stream_RPAREN=new RewriteRuleITokenStream(adaptor,"token RPAREN");
		RewriteRuleITokenStream stream_150=new RewriteRuleITokenStream(adaptor,"token 150");
		RewriteRuleITokenStream stream_HASH=new RewriteRuleITokenStream(adaptor,"token HASH");

		try { DebugEnterRule(GrammarFileName, "color");
		DebugLocation(428, 4);
		try
		{
			if (state.backtracking > 0 && AlreadyParsedRule(input, 54)) { return retval; }
			// CssGrammer.g3:429:9: ( 'rgba' LPAREN n1= NUMBER COMMA n2= NUMBER COMMA n3= NUMBER COMMA n4= NUMBER RPAREN -> ^( RGBA $n1 $n2 $n3 $n4) | 'rgb' LPAREN n1= NUMBER COMMA n2= NUMBER COMMA n3= NUMBER RPAREN -> ^( RGBA $n1 $n2 $n3) |cl= HASH -> ^( COLOR $cl) )
			int alt53=3;
			try { DebugEnterDecision(53, decisionCanBacktrack[53]);
			switch (input.LA(1))
			{
			case 151:
				{
				alt53=1;
				}
				break;
			case 150:
				{
				alt53=2;
				}
				break;
			case HASH:
				{
				alt53=3;
				}
				break;
			default:
				{
					if (state.backtracking>0) {state.failed=true; return retval;}
					NoViableAltException nvae = new NoViableAltException("", 53, 0, input);

					DebugRecognitionException(nvae);
					throw nvae;
				}
			}

			} finally { DebugExitDecision(53); }
			switch (alt53)
			{
			case 1:
				DebugEnterAlt(1);
				// CssGrammer.g3:429:11: 'rgba' LPAREN n1= NUMBER COMMA n2= NUMBER COMMA n3= NUMBER COMMA n4= NUMBER RPAREN
				{
				DebugLocation(429, 11);
				string_literal195=(CommonToken)Match(input,151,Follow._151_in_color3227); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_151.Add(string_literal195);

				DebugLocation(429, 18);
				LPAREN196=(CommonToken)Match(input,LPAREN,Follow._LPAREN_in_color3229); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_LPAREN.Add(LPAREN196);

				DebugLocation(429, 27);
				n1=(CommonToken)Match(input,NUMBER,Follow._NUMBER_in_color3233); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_NUMBER.Add(n1);

				DebugLocation(429, 35);
				COMMA197=(CommonToken)Match(input,COMMA,Follow._COMMA_in_color3235); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_COMMA.Add(COMMA197);

				DebugLocation(429, 43);
				n2=(CommonToken)Match(input,NUMBER,Follow._NUMBER_in_color3239); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_NUMBER.Add(n2);

				DebugLocation(429, 51);
				COMMA198=(CommonToken)Match(input,COMMA,Follow._COMMA_in_color3241); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_COMMA.Add(COMMA198);

				DebugLocation(429, 59);
				n3=(CommonToken)Match(input,NUMBER,Follow._NUMBER_in_color3245); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_NUMBER.Add(n3);

				DebugLocation(429, 67);
				COMMA199=(CommonToken)Match(input,COMMA,Follow._COMMA_in_color3247); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_COMMA.Add(COMMA199);

				DebugLocation(429, 75);
				n4=(CommonToken)Match(input,NUMBER,Follow._NUMBER_in_color3251); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_NUMBER.Add(n4);

				DebugLocation(429, 83);
				RPAREN200=(CommonToken)Match(input,RPAREN,Follow._RPAREN_in_color3253); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_RPAREN.Add(RPAREN200);



				{
				// AST REWRITE
				// elements: n1, n2, n3, n4
				// token labels: n1, n2, n3, n4
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.Tree = root_0;
				RewriteRuleITokenStream stream_n1=new RewriteRuleITokenStream(adaptor,"token n1",n1);
				RewriteRuleITokenStream stream_n2=new RewriteRuleITokenStream(adaptor,"token n2",n2);
				RewriteRuleITokenStream stream_n3=new RewriteRuleITokenStream(adaptor,"token n3",n3);
				RewriteRuleITokenStream stream_n4=new RewriteRuleITokenStream(adaptor,"token n4",n4);
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

				root_0 = (CommonTree)adaptor.Nil();
				// 430:13: -> ^( RGBA $n1 $n2 $n3 $n4)
				{
					DebugLocation(430, 16);
					// CssGrammer.g3:430:16: ^( RGBA $n1 $n2 $n3 $n4)
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(430, 18);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(RGBA, "RGBA"), root_1);

					DebugLocation(430, 24);
					adaptor.AddChild(root_1, stream_n1.NextNode());
					DebugLocation(430, 28);
					adaptor.AddChild(root_1, stream_n2.NextNode());
					DebugLocation(430, 32);
					adaptor.AddChild(root_1, stream_n3.NextNode());
					DebugLocation(430, 36);
					adaptor.AddChild(root_1, stream_n4.NextNode());

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
				// CssGrammer.g3:431:11: 'rgb' LPAREN n1= NUMBER COMMA n2= NUMBER COMMA n3= NUMBER RPAREN
				{
				DebugLocation(431, 11);
				string_literal201=(CommonToken)Match(input,150,Follow._150_in_color3295); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_150.Add(string_literal201);

				DebugLocation(431, 17);
				LPAREN202=(CommonToken)Match(input,LPAREN,Follow._LPAREN_in_color3297); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_LPAREN.Add(LPAREN202);

				DebugLocation(431, 26);
				n1=(CommonToken)Match(input,NUMBER,Follow._NUMBER_in_color3301); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_NUMBER.Add(n1);

				DebugLocation(431, 34);
				COMMA203=(CommonToken)Match(input,COMMA,Follow._COMMA_in_color3303); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_COMMA.Add(COMMA203);

				DebugLocation(431, 42);
				n2=(CommonToken)Match(input,NUMBER,Follow._NUMBER_in_color3307); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_NUMBER.Add(n2);

				DebugLocation(431, 50);
				COMMA204=(CommonToken)Match(input,COMMA,Follow._COMMA_in_color3309); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_COMMA.Add(COMMA204);

				DebugLocation(431, 58);
				n3=(CommonToken)Match(input,NUMBER,Follow._NUMBER_in_color3313); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_NUMBER.Add(n3);

				DebugLocation(431, 66);
				RPAREN205=(CommonToken)Match(input,RPAREN,Follow._RPAREN_in_color3315); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_RPAREN.Add(RPAREN205);



				{
				// AST REWRITE
				// elements: n1, n2, n3
				// token labels: n1, n2, n3
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.Tree = root_0;
				RewriteRuleITokenStream stream_n1=new RewriteRuleITokenStream(adaptor,"token n1",n1);
				RewriteRuleITokenStream stream_n2=new RewriteRuleITokenStream(adaptor,"token n2",n2);
				RewriteRuleITokenStream stream_n3=new RewriteRuleITokenStream(adaptor,"token n3",n3);
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

				root_0 = (CommonTree)adaptor.Nil();
				// 432:13: -> ^( RGBA $n1 $n2 $n3)
				{
					DebugLocation(432, 16);
					// CssGrammer.g3:432:16: ^( RGBA $n1 $n2 $n3)
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(432, 18);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(RGBA, "RGBA"), root_1);

					DebugLocation(432, 24);
					adaptor.AddChild(root_1, stream_n1.NextNode());
					DebugLocation(432, 28);
					adaptor.AddChild(root_1, stream_n2.NextNode());
					DebugLocation(432, 32);
					adaptor.AddChild(root_1, stream_n3.NextNode());

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
				// CssGrammer.g3:433:11: cl= HASH
				{
				DebugLocation(433, 13);
				cl=(CommonToken)Match(input,HASH,Follow._HASH_in_color3356); if (state.failed) return retval; 
				if ( state.backtracking == 0 ) stream_HASH.Add(cl);



				{
				// AST REWRITE
				// elements: cl
				// token labels: cl
				// rule labels: retval
				// token list labels: 
				// rule list labels: 
				// wildcard labels: 
				if ( state.backtracking == 0 ) {
				retval.Tree = root_0;
				RewriteRuleITokenStream stream_cl=new RewriteRuleITokenStream(adaptor,"token cl",cl);
				RewriteRuleSubtreeStream stream_retval=new RewriteRuleSubtreeStream(adaptor,"rule retval",retval!=null?retval.Tree:null);

				root_0 = (CommonTree)adaptor.Nil();
				// 433:19: -> ^( COLOR $cl)
				{
					DebugLocation(433, 22);
					// CssGrammer.g3:433:22: ^( COLOR $cl)
					{
					CommonTree root_1 = (CommonTree)adaptor.Nil();
					DebugLocation(433, 24);
					root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(COLOR, "COLOR"), root_1);

					DebugLocation(433, 31);
					adaptor.AddChild(root_1, stream_cl.NextNode());

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
			TraceOut("color", 54);
			LeaveRule("color", 54);
			Leave_color();
			if (state.backtracking > 0) { Memoize(input, 54, color_StartIndex); }
		}
		DebugLocation(434, 4);
		} finally { DebugExitRule(GrammarFileName, "color"); }
		return retval;

	}
	// $ANTLR end "color"

	partial void Enter_synpred13_CssGrammer_fragment();
	partial void Leave_synpred13_CssGrammer_fragment();

	// $ANTLR start synpred13_CssGrammer
	public void synpred13_CssGrammer_fragment()
	{
		Enter_synpred13_CssGrammer_fragment();
		EnterRule("synpred13_CssGrammer_fragment", 67);
		TraceIn("synpred13_CssGrammer_fragment", 67);
		try
		{
			// CssGrammer.g3:145:7: ( LPAREN property COLON term RPAREN )
			DebugEnterAlt(1);
			// CssGrammer.g3:145:7: LPAREN property COLON term RPAREN
			{
			DebugLocation(145, 7);
			Match(input,LPAREN,Follow._LPAREN_in_synpred13_CssGrammer815); if (state.failed) return;
			DebugLocation(145, 14);
			PushFollow(Follow._property_in_synpred13_CssGrammer817);
			property();
			PopFollow();
			if (state.failed) return;
			DebugLocation(145, 23);
			Match(input,COLON,Follow._COLON_in_synpred13_CssGrammer819); if (state.failed) return;
			DebugLocation(145, 29);
			PushFollow(Follow._term_in_synpred13_CssGrammer821);
			term();
			PopFollow();
			if (state.failed) return;
			DebugLocation(145, 34);
			Match(input,RPAREN,Follow._RPAREN_in_synpred13_CssGrammer823); if (state.failed) return;

			}

		}
		finally
		{
			TraceOut("synpred13_CssGrammer_fragment", 67);
			LeaveRule("synpred13_CssGrammer_fragment", 67);
			Leave_synpred13_CssGrammer_fragment();
		}
	}
	// $ANTLR end synpred13_CssGrammer

	partial void Enter_synpred14_CssGrammer_fragment();
	partial void Leave_synpred14_CssGrammer_fragment();

	// $ANTLR start synpred14_CssGrammer
	public void synpred14_CssGrammer_fragment()
	{
		Enter_synpred14_CssGrammer_fragment();
		EnterRule("synpred14_CssGrammer_fragment", 68);
		TraceIn("synpred14_CssGrammer_fragment", 68);
		try
		{
			// CssGrammer.g3:147:7: ( LPAREN property RPAREN )
			DebugEnterAlt(1);
			// CssGrammer.g3:147:7: LPAREN property RPAREN
			{
			DebugLocation(147, 7);
			Match(input,LPAREN,Follow._LPAREN_in_synpred14_CssGrammer849); if (state.failed) return;
			DebugLocation(147, 14);
			PushFollow(Follow._property_in_synpred14_CssGrammer851);
			property();
			PopFollow();
			if (state.failed) return;
			DebugLocation(147, 23);
			Match(input,RPAREN,Follow._RPAREN_in_synpred14_CssGrammer853); if (state.failed) return;

			}

		}
		finally
		{
			TraceOut("synpred14_CssGrammer_fragment", 68);
			LeaveRule("synpred14_CssGrammer_fragment", 68);
			Leave_synpred14_CssGrammer_fragment();
		}
	}
	// $ANTLR end synpred14_CssGrammer

	partial void Enter_synpred15_CssGrammer_fragment();
	partial void Leave_synpred15_CssGrammer_fragment();

	// $ANTLR start synpred15_CssGrammer
	public void synpred15_CssGrammer_fragment()
	{
		Enter_synpred15_CssGrammer_fragment();
		EnterRule("synpred15_CssGrammer_fragment", 69);
		TraceIn("synpred15_CssGrammer_fragment", 69);
		try
		{
			// CssGrammer.g3:152:7: ( LPAREN property comparisionOp term RPAREN )
			DebugEnterAlt(1);
			// CssGrammer.g3:152:7: LPAREN property comparisionOp term RPAREN
			{
			DebugLocation(152, 7);
			Match(input,LPAREN,Follow._LPAREN_in_synpred15_CssGrammer893); if (state.failed) return;
			DebugLocation(152, 14);
			PushFollow(Follow._property_in_synpred15_CssGrammer895);
			property();
			PopFollow();
			if (state.failed) return;
			DebugLocation(152, 23);
			PushFollow(Follow._comparisionOp_in_synpred15_CssGrammer897);
			comparisionOp();
			PopFollow();
			if (state.failed) return;
			DebugLocation(152, 37);
			PushFollow(Follow._term_in_synpred15_CssGrammer899);
			term();
			PopFollow();
			if (state.failed) return;
			DebugLocation(152, 42);
			Match(input,RPAREN,Follow._RPAREN_in_synpred15_CssGrammer901); if (state.failed) return;

			}

		}
		finally
		{
			TraceOut("synpred15_CssGrammer_fragment", 69);
			LeaveRule("synpred15_CssGrammer_fragment", 69);
			Leave_synpred15_CssGrammer_fragment();
		}
	}
	// $ANTLR end synpred15_CssGrammer

	partial void Enter_synpred16_CssGrammer_fragment();
	partial void Leave_synpred16_CssGrammer_fragment();

	// $ANTLR start synpred16_CssGrammer
	public void synpred16_CssGrammer_fragment()
	{
		CssGrammerParser.term_return t1 = default(CssGrammerParser.term_return);
		CssGrammerParser.rightDirectionOp_return r1 = default(CssGrammerParser.rightDirectionOp_return);
		CssGrammerParser.rightDirectionOp_return r2 = default(CssGrammerParser.rightDirectionOp_return);
		CssGrammerParser.term_return t2 = default(CssGrammerParser.term_return);

		Enter_synpred16_CssGrammer_fragment();
		EnterRule("synpred16_CssGrammer_fragment", 70);
		TraceIn("synpred16_CssGrammer_fragment", 70);
		try
		{
			// CssGrammer.g3:154:7: ( LPAREN t1= term r1= rightDirectionOp property r2= rightDirectionOp t2= term RPAREN )
			DebugEnterAlt(1);
			// CssGrammer.g3:154:7: LPAREN t1= term r1= rightDirectionOp property r2= rightDirectionOp t2= term RPAREN
			{
			DebugLocation(154, 7);
			Match(input,LPAREN,Follow._LPAREN_in_synpred16_CssGrammer929); if (state.failed) return;
			DebugLocation(154, 16);
			PushFollow(Follow._term_in_synpred16_CssGrammer933);
			t1=term();
			PopFollow();
			if (state.failed) return;
			DebugLocation(154, 24);
			PushFollow(Follow._rightDirectionOp_in_synpred16_CssGrammer937);
			r1=rightDirectionOp();
			PopFollow();
			if (state.failed) return;
			DebugLocation(154, 42);
			PushFollow(Follow._property_in_synpred16_CssGrammer939);
			property();
			PopFollow();
			if (state.failed) return;
			DebugLocation(154, 53);
			PushFollow(Follow._rightDirectionOp_in_synpred16_CssGrammer943);
			r2=rightDirectionOp();
			PopFollow();
			if (state.failed) return;
			DebugLocation(154, 73);
			PushFollow(Follow._term_in_synpred16_CssGrammer947);
			t2=term();
			PopFollow();
			if (state.failed) return;
			DebugLocation(154, 79);
			Match(input,RPAREN,Follow._RPAREN_in_synpred16_CssGrammer949); if (state.failed) return;

			}

		}
		finally
		{
			TraceOut("synpred16_CssGrammer_fragment", 70);
			LeaveRule("synpred16_CssGrammer_fragment", 70);
			Leave_synpred16_CssGrammer_fragment();
		}
	}
	// $ANTLR end synpred16_CssGrammer

	partial void Enter_synpred42_CssGrammer_fragment();
	partial void Leave_synpred42_CssGrammer_fragment();

	// $ANTLR start synpred42_CssGrammer
	public void synpred42_CssGrammer_fragment()
	{
		Enter_synpred42_CssGrammer_fragment();
		EnterRule("synpred42_CssGrammer_fragment", 96);
		TraceIn("synpred42_CssGrammer_fragment", 96);
		try
		{
			// CssGrammer.g3:275:20: ( esPred )
			DebugEnterAlt(1);
			// CssGrammer.g3:275:21: esPred
			{
			DebugLocation(275, 21);
			PushFollow(Follow._esPred_in_synpred42_CssGrammer1848);
			esPred();
			PopFollow();
			if (state.failed) return;

			}

		}
		finally
		{
			TraceOut("synpred42_CssGrammer_fragment", 96);
			LeaveRule("synpred42_CssGrammer_fragment", 96);
			Leave_synpred42_CssGrammer_fragment();
		}
	}
	// $ANTLR end synpred42_CssGrammer

	partial void Enter_synpred44_CssGrammer_fragment();
	partial void Leave_synpred44_CssGrammer_fragment();

	// $ANTLR start synpred44_CssGrammer
	public void synpred44_CssGrammer_fragment()
	{
		Enter_synpred44_CssGrammer_fragment();
		EnterRule("synpred44_CssGrammer_fragment", 98);
		TraceIn("synpred44_CssGrammer_fragment", 98);
		try
		{
			// CssGrammer.g3:277:8: ( esPred )
			DebugEnterAlt(1);
			// CssGrammer.g3:277:9: esPred
			{
			DebugLocation(277, 9);
			PushFollow(Follow._esPred_in_synpred44_CssGrammer1882);
			esPred();
			PopFollow();
			if (state.failed) return;

			}

		}
		finally
		{
			TraceOut("synpred44_CssGrammer_fragment", 98);
			LeaveRule("synpred44_CssGrammer_fragment", 98);
			Leave_synpred44_CssGrammer_fragment();
		}
	}
	// $ANTLR end synpred44_CssGrammer

	partial void Enter_synpred61_CssGrammer_fragment();
	partial void Leave_synpred61_CssGrammer_fragment();

	// $ANTLR start synpred61_CssGrammer
	public void synpred61_CssGrammer_fragment()
	{
		Enter_synpred61_CssGrammer_fragment();
		EnterRule("synpred61_CssGrammer_fragment", 115);
		TraceIn("synpred61_CssGrammer_fragment", 115);
		try
		{
			// CssGrammer.g3:327:7: ( pseudoStart identifier LPAREN ( pseudoFuncArgs )? RPAREN )
			DebugEnterAlt(1);
			// CssGrammer.g3:327:7: pseudoStart identifier LPAREN ( pseudoFuncArgs )? RPAREN
			{
			DebugLocation(327, 7);
			PushFollow(Follow._pseudoStart_in_synpred61_CssGrammer2298);
			pseudoStart();
			PopFollow();
			if (state.failed) return;
			DebugLocation(327, 19);
			PushFollow(Follow._identifier_in_synpred61_CssGrammer2300);
			identifier();
			PopFollow();
			if (state.failed) return;
			DebugLocation(327, 30);
			Match(input,LPAREN,Follow._LPAREN_in_synpred61_CssGrammer2302); if (state.failed) return;
			DebugLocation(327, 37);
			// CssGrammer.g3:327:37: ( pseudoFuncArgs )?
			int alt59=2;
			try { DebugEnterSubRule(59);
			try { DebugEnterDecision(59, decisionCanBacktrack[59]);
			int LA59_0 = input.LA(1);

			if ((LA59_0==IDENT||LA59_0==MULTIPLIER||LA59_0==NUMBER))
			{
				alt59=1;
			}
			} finally { DebugExitDecision(59); }
			switch (alt59)
			{
			case 1:
				DebugEnterAlt(1);
				// CssGrammer.g3:327:38: pseudoFuncArgs
				{
				DebugLocation(327, 38);
				PushFollow(Follow._pseudoFuncArgs_in_synpred61_CssGrammer2305);
				pseudoFuncArgs();
				PopFollow();
				if (state.failed) return;

				}
				break;

			}
			} finally { DebugExitSubRule(59); }

			DebugLocation(327, 55);
			Match(input,RPAREN,Follow._RPAREN_in_synpred61_CssGrammer2309); if (state.failed) return;

			}

		}
		finally
		{
			TraceOut("synpred61_CssGrammer_fragment", 115);
			LeaveRule("synpred61_CssGrammer_fragment", 115);
			Leave_synpred61_CssGrammer_fragment();
		}
	}
	// $ANTLR end synpred61_CssGrammer

	partial void Enter_synpred63_CssGrammer_fragment();
	partial void Leave_synpred63_CssGrammer_fragment();

	// $ANTLR start synpred63_CssGrammer
	public void synpred63_CssGrammer_fragment()
	{
		Enter_synpred63_CssGrammer_fragment();
		EnterRule("synpred63_CssGrammer_fragment", 117);
		TraceIn("synpred63_CssGrammer_fragment", 117);
		try
		{
			// CssGrammer.g3:329:7: ( pseudoStart identifier LPAREN ( selector )? RPAREN )
			DebugEnterAlt(1);
			// CssGrammer.g3:329:7: pseudoStart identifier LPAREN ( selector )? RPAREN
			{
			DebugLocation(329, 7);
			PushFollow(Follow._pseudoStart_in_synpred63_CssGrammer2338);
			pseudoStart();
			PopFollow();
			if (state.failed) return;
			DebugLocation(329, 19);
			PushFollow(Follow._identifier_in_synpred63_CssGrammer2340);
			identifier();
			PopFollow();
			if (state.failed) return;
			DebugLocation(329, 30);
			Match(input,LPAREN,Follow._LPAREN_in_synpred63_CssGrammer2342); if (state.failed) return;
			DebugLocation(329, 37);
			// CssGrammer.g3:329:37: ( selector )?
			int alt60=2;
			try { DebugEnterSubRule(60);
			try { DebugEnterDecision(60, decisionCanBacktrack[60]);
			int LA60_0 = input.LA(1);

			if ((LA60_0==COLON||(LA60_0>=DOT && LA60_0<=DOUBLE_COLON)||LA60_0==HASH||LA60_0==IDENT||LA60_0==LBRACKET||LA60_0==STAR))
			{
				alt60=1;
			}
			} finally { DebugExitDecision(60); }
			switch (alt60)
			{
			case 1:
				DebugEnterAlt(1);
				// CssGrammer.g3:329:38: selector
				{
				DebugLocation(329, 38);
				PushFollow(Follow._selector_in_synpred63_CssGrammer2345);
				selector();
				PopFollow();
				if (state.failed) return;

				}
				break;

			}
			} finally { DebugExitSubRule(60); }

			DebugLocation(329, 49);
			Match(input,RPAREN,Follow._RPAREN_in_synpred63_CssGrammer2349); if (state.failed) return;

			}

		}
		finally
		{
			TraceOut("synpred63_CssGrammer_fragment", 117);
			LeaveRule("synpred63_CssGrammer_fragment", 117);
			Leave_synpred63_CssGrammer_fragment();
		}
	}
	// $ANTLR end synpred63_CssGrammer

	partial void Enter_synpred75_CssGrammer_fragment();
	partial void Leave_synpred75_CssGrammer_fragment();

	// $ANTLR start synpred75_CssGrammer
	public void synpred75_CssGrammer_fragment()
	{
		Enter_synpred75_CssGrammer_fragment();
		EnterRule("synpred75_CssGrammer_fragment", 129);
		TraceIn("synpred75_CssGrammer_fragment", 129);
		try
		{
			// CssGrammer.g3:376:7: ( identifier )
			DebugEnterAlt(1);
			// CssGrammer.g3:376:7: identifier
			{
			DebugLocation(376, 7);
			PushFollow(Follow._identifier_in_synpred75_CssGrammer2712);
			identifier();
			PopFollow();
			if (state.failed) return;

			}

		}
		finally
		{
			TraceOut("synpred75_CssGrammer_fragment", 129);
			LeaveRule("synpred75_CssGrammer_fragment", 129);
			Leave_synpred75_CssGrammer_fragment();
		}
	}
	// $ANTLR end synpred75_CssGrammer

	partial void Enter_synpred76_CssGrammer_fragment();
	partial void Leave_synpred76_CssGrammer_fragment();

	// $ANTLR start synpred76_CssGrammer
	public void synpred76_CssGrammer_fragment()
	{
		Enter_synpred76_CssGrammer_fragment();
		EnterRule("synpred76_CssGrammer_fragment", 130);
		TraceIn("synpred76_CssGrammer_fragment", 130);
		try
		{
			// CssGrammer.g3:377:7: ( STRING )
			DebugEnterAlt(1);
			// CssGrammer.g3:377:7: STRING
			{
			DebugLocation(377, 7);
			Match(input,STRING,Follow._STRING_in_synpred76_CssGrammer2728); if (state.failed) return;

			}

		}
		finally
		{
			TraceOut("synpred76_CssGrammer_fragment", 130);
			LeaveRule("synpred76_CssGrammer_fragment", 130);
			Leave_synpred76_CssGrammer_fragment();
		}
	}
	// $ANTLR end synpred76_CssGrammer

	partial void Enter_synpred77_CssGrammer_fragment();
	partial void Leave_synpred77_CssGrammer_fragment();

	// $ANTLR start synpred77_CssGrammer
	public void synpred77_CssGrammer_fragment()
	{
		Enter_synpred77_CssGrammer_fragment();
		EnterRule("synpred77_CssGrammer_fragment", 131);
		TraceIn("synpred77_CssGrammer_fragment", 131);
		try
		{
			// CssGrammer.g3:378:7: ( color )
			DebugEnterAlt(1);
			// CssGrammer.g3:378:7: color
			{
			DebugLocation(378, 7);
			PushFollow(Follow._color_in_synpred77_CssGrammer2744);
			color();
			PopFollow();
			if (state.failed) return;

			}

		}
		finally
		{
			TraceOut("synpred77_CssGrammer_fragment", 131);
			LeaveRule("synpred77_CssGrammer_fragment", 131);
			Leave_synpred77_CssGrammer_fragment();
		}
	}
	// $ANTLR end synpred77_CssGrammer

	partial void Enter_synpred78_CssGrammer_fragment();
	partial void Leave_synpred78_CssGrammer_fragment();

	// $ANTLR start synpred78_CssGrammer
	public void synpred78_CssGrammer_fragment()
	{
		Enter_synpred78_CssGrammer_fragment();
		EnterRule("synpred78_CssGrammer_fragment", 132);
		TraceIn("synpred78_CssGrammer_fragment", 132);
		try
		{
			// CssGrammer.g3:379:7: ( calcExpr )
			DebugEnterAlt(1);
			// CssGrammer.g3:379:7: calcExpr
			{
			DebugLocation(379, 7);
			PushFollow(Follow._calcExpr_in_synpred78_CssGrammer2752);
			calcExpr();
			PopFollow();
			if (state.failed) return;

			}

		}
		finally
		{
			TraceOut("synpred78_CssGrammer_fragment", 132);
			LeaveRule("synpred78_CssGrammer_fragment", 132);
			Leave_synpred78_CssGrammer_fragment();
		}
	}
	// $ANTLR end synpred78_CssGrammer

	partial void Enter_synpred79_CssGrammer_fragment();
	partial void Leave_synpred79_CssGrammer_fragment();

	// $ANTLR start synpred79_CssGrammer
	public void synpred79_CssGrammer_fragment()
	{
		Enter_synpred79_CssGrammer_fragment();
		EnterRule("synpred79_CssGrammer_fragment", 133);
		TraceIn("synpred79_CssGrammer_fragment", 133);
		try
		{
			// CssGrammer.g3:380:7: ( URI )
			DebugEnterAlt(1);
			// CssGrammer.g3:380:7: URI
			{
			DebugLocation(380, 7);
			Match(input,URI,Follow._URI_in_synpred79_CssGrammer2760); if (state.failed) return;

			}

		}
		finally
		{
			TraceOut("synpred79_CssGrammer_fragment", 133);
			LeaveRule("synpred79_CssGrammer_fragment", 133);
			Leave_synpred79_CssGrammer_fragment();
		}
	}
	// $ANTLR end synpred79_CssGrammer

	partial void Enter_synpred81_CssGrammer_fragment();
	partial void Leave_synpred81_CssGrammer_fragment();

	// $ANTLR start synpred81_CssGrammer
	public void synpred81_CssGrammer_fragment()
	{
		CssGrammerParser.unaryOperator_return oper = default(CssGrammerParser.unaryOperator_return);
		CssGrammerParser.unitTerm_return val = default(CssGrammerParser.unitTerm_return);

		Enter_synpred81_CssGrammer_fragment();
		EnterRule("synpred81_CssGrammer_fragment", 135);
		TraceIn("synpred81_CssGrammer_fragment", 135);
		try
		{
			// CssGrammer.g3:381:7: ( (oper= unaryOperator )? val= unitTerm )
			DebugEnterAlt(1);
			// CssGrammer.g3:381:7: (oper= unaryOperator )? val= unitTerm
			{
			DebugLocation(381, 7);
			// CssGrammer.g3:381:7: (oper= unaryOperator )?
			int alt62=2;
			try { DebugEnterSubRule(62);
			try { DebugEnterDecision(62, decisionCanBacktrack[62]);
			int LA62_0 = input.LA(1);

			if ((LA62_0==MINUS||LA62_0==PLUS))
			{
				alt62=1;
			}
			} finally { DebugExitDecision(62); }
			switch (alt62)
			{
			case 1:
				DebugEnterAlt(1);
				// CssGrammer.g3:381:8: oper= unaryOperator
				{
				DebugLocation(381, 12);
				PushFollow(Follow._unaryOperator_in_synpred81_CssGrammer2779);
				oper=unaryOperator();
				PopFollow();
				if (state.failed) return;

				}
				break;

			}
			} finally { DebugExitSubRule(62); }

			DebugLocation(381, 32);
			PushFollow(Follow._unitTerm_in_synpred81_CssGrammer2785);
			val=unitTerm();
			PopFollow();
			if (state.failed) return;

			}

		}
		finally
		{
			TraceOut("synpred81_CssGrammer_fragment", 135);
			LeaveRule("synpred81_CssGrammer_fragment", 135);
			Leave_synpred81_CssGrammer_fragment();
		}
	}
	// $ANTLR end synpred81_CssGrammer
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
	DFA29 dfa29;
	DFA46 dfa46;

	protected override void InitDFAs()
	{
		base.InitDFAs();
		dfa29 = new DFA29( this, SpecialStateTransition29 );
		dfa46 = new DFA46( this, SpecialStateTransition46 );
	}

	private class DFA29 : DFA
	{
		private const string DFA29_eotS =
			"\x12\xFFFF";
		private const string DFA29_eofS =
			"\x1\x1\x11\xFFFF";
		private const string DFA29_minS =
			"\x1\x18\x1\xFFFF\x1\x0\x3\x37\x1\xFFFF\x1\x0\x1\x24\x1\x0\x6\x37\x1\x0"+
			"\x1\x6C";
		private const string DFA29_maxS =
			"\x1\x98\x1\xFFFF\x1\x0\x2\x37\x1\x81\x1\xFFFF\x1\x0\x1\x7B\x1\x0\x6\x7C"+
			"\x1\x0\x1\x6C";
		private const string DFA29_acceptS =
			"\x1\xFFFF\x1\x2\x4\xFFFF\x1\x1\xB\xFFFF";
		private const string DFA29_specialS =
			"\x2\xFFFF\x1\x0\x4\xFFFF\x1\x1\x1\xFFFF\x1\x2\x6\xFFFF\x1\x3\x1\xFFFF}>";
		private static readonly string[] DFA29_transitionS =
			{
				"\x1\x5\x1\xFFFF\x1\x1\x3\xFFFF\x1\x3\x1\x5\x10\xFFFF\x1\x1\x1\xFFFF"+
				"\x1\x2\x4\xFFFF\x1\x1\xE\xFFFF\x1\x1\x1\x4\x1B\xFFFF\x1\x1\xB\xFFFF"+
				"\x1\x1\x9\xFFFF\x1\x1\x1E\xFFFF\x1\x1",
				"",
				"\x1\xFFFF",
				"\x1\x7",
				"\x1\x8",
				"\x1\x9\x22\xFFFF\x1\x9\x26\xFFFF\x1\x9",
				"",
				"\x1\xFFFF",
				"\x1\xF\x17\xFFFF\x1\xD\x1\xB\x20\xFFFF\x1\xA\xD\xFFFF\x1\x10\xD\xFFFF"+
				"\x1\xE\x1\xC",
				"\x1\xFFFF",
				"\x1\x11\x44\xFFFF\x1\x11",
				"\x1\x11\x44\xFFFF\x1\x11",
				"\x1\x11\x44\xFFFF\x1\x11",
				"\x1\x11\x44\xFFFF\x1\x11",
				"\x1\x11\x44\xFFFF\x1\x11",
				"\x1\x11\x44\xFFFF\x1\x11",
				"\x1\xFFFF",
				"\x1\x10"
			};

		private static readonly short[] DFA29_eot = DFA.UnpackEncodedString(DFA29_eotS);
		private static readonly short[] DFA29_eof = DFA.UnpackEncodedString(DFA29_eofS);
		private static readonly char[] DFA29_min = DFA.UnpackEncodedStringToUnsignedChars(DFA29_minS);
		private static readonly char[] DFA29_max = DFA.UnpackEncodedStringToUnsignedChars(DFA29_maxS);
		private static readonly short[] DFA29_accept = DFA.UnpackEncodedString(DFA29_acceptS);
		private static readonly short[] DFA29_special = DFA.UnpackEncodedString(DFA29_specialS);
		private static readonly short[][] DFA29_transition;

		static DFA29()
		{
			int numStates = DFA29_transitionS.Length;
			DFA29_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA29_transition[i] = DFA.UnpackEncodedString(DFA29_transitionS[i]);
			}
		}

		public DFA29( BaseRecognizer recognizer, SpecialStateTransitionHandler specialStateTransition )
			: base(specialStateTransition)
		{
			this.recognizer = recognizer;
			this.decisionNumber = 29;
			this.eot = DFA29_eot;
			this.eof = DFA29_eof;
			this.min = DFA29_min;
			this.max = DFA29_max;
			this.accept = DFA29_accept;
			this.special = DFA29_special;
			this.transition = DFA29_transition;
		}

		public override string Description { get { return "()* loopback of 275:19: ( ( esPred )=> elementSubsequent )*"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private int SpecialStateTransition29(DFA dfa, int s, IIntStream _input)
	{
		ITokenStream input = (ITokenStream)_input;
		int _s = s;
		switch (s)
		{
			case 0:
				int LA29_2 = input.LA(1);


				int index29_2 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred42_CssGrammer_fragment)) ) {s = 6;}

				else if ( (true) ) {s = 1;}


				input.Seek(index29_2);
				if ( s>=0 ) return s;
				break;
			case 1:
				int LA29_7 = input.LA(1);


				int index29_7 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred42_CssGrammer_fragment)) ) {s = 6;}

				else if ( (true) ) {s = 1;}


				input.Seek(index29_7);
				if ( s>=0 ) return s;
				break;
			case 2:
				int LA29_9 = input.LA(1);


				int index29_9 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred42_CssGrammer_fragment)) ) {s = 6;}

				else if ( (true) ) {s = 1;}


				input.Seek(index29_9);
				if ( s>=0 ) return s;
				break;
			case 3:
				int LA29_16 = input.LA(1);


				int index29_16 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred42_CssGrammer_fragment)) ) {s = 6;}

				else if ( (true) ) {s = 1;}


				input.Seek(index29_16);
				if ( s>=0 ) return s;
				break;
		}
		if (state.backtracking > 0) {state.failed=true; return -1;}
		NoViableAltException nvae = new NoViableAltException(dfa.Description, 29, _s, input);
		dfa.Error(nvae);
		throw nvae;
	}
	private class DFA46 : DFA
	{
		private const string DFA46_eotS =
			"\x28\xFFFF";
		private const string DFA46_eofS =
			"\x28\xFFFF";
		private const string DFA46_minS =
			"\x1\x6\x1\x18\x1\x0\x2\x49\x1\x0\x1\x49\x1\x0\x1\x6\x1\x0\x5\xFFFF\x2"+
			"\x5C\x1\xFFFF\x1\x6\x2\xFFFF\x2\x1A\x1\x50\x2\x5C\x1\x6\x1\x0\x2\x1A"+
			"\x1\x50\x1\xFFFF\x2\x5C\x1\x1A\x1\x6F\x1\x5C\x1\x0\x1\x6F\x1\x0";
		private const string DFA46_maxS =
			"\x1\x97\x1\x49\x1\x0\x2\x49\x1\x0\x1\x49\x1\x0\x1\x80\x1\x0\x5\xFFFF"+
			"\x2\x5C\x1\xFFFF\x1\x80\x2\xFFFF\x2\x1A\x1\x79\x2\x5C\x1\x80\x1\x0\x2"+
			"\x1A\x1\x79\x1\xFFFF\x2\x5C\x1\x1A\x1\x6F\x1\x5C\x1\x0\x1\x6F\x1\x0";
		private const string DFA46_acceptS =
			"\xA\xFFFF\x1\x7\x1\x8\x1\x1\x1\x9\x1\x2\x2\xFFFF\x1\x3\x1\xFFFF\x1\x5"+
			"\x1\x6\xA\xFFFF\x1\x4\x8\xFFFF";
		private const string DFA46_specialS =
			"\x1\xFFFF\x1\x0\x1\x1\x2\xFFFF\x1\x2\x1\xFFFF\x1\x3\x1\xFFFF\x1\x4\x11"+
			"\xFFFF\x1\x5\x9\xFFFF\x1\x6\x1\xFFFF\x1\x7}>";
		private static readonly string[] DFA46_transitionS =
			{
				"\x1\x9\x1C\xFFFF\x1\x9\x4\xFFFF\x1\x9\x3\xFFFF\x1\x9\x5\xFFFF\x1\x5"+
				"\x4\xFFFF\x1\x1\x10\xFFFF\x1\x9\x7\xFFFF\x1\x8\x9\xFFFF\x1\x1\x1\xFFFF"+
				"\x1\x9\x5\xFFFF\x1\x9\x1\x8\x9\xFFFF\x1\x9\xE\xFFFF\x1\x2\x3\xFFFF\x1"+
				"\x9\x1\x1\x5\xFFFF\x1\x7\xC\xFFFF\x1\x6\x1\xFFFF\x1\x4\x1\x3",
				"\x1\xB\x30\xFFFF\x1\xA",
				"\x1\xFFFF",
				"\x1\xF",
				"\x1\x10",
				"\x1\xFFFF",
				"\x1\x12",
				"\x1\xFFFF",
				"\x1\x9\x1C\xFFFF\x1\x9\x4\xFFFF\x1\x9\x3\xFFFF\x1\x9\x1B\xFFFF\x1\x9"+
				"\x13\xFFFF\x1\x9\x5\xFFFF\x1\x9\xA\xFFFF\x1\x9\x12\xFFFF\x1\x9",
				"\x1\xFFFF",
				"",
				"",
				"",
				"",
				"",
				"\x1\x15",
				"\x1\x16",
				"",
				"\x1\x17\x1C\xFFFF\x1\x17\x4\xFFFF\x1\x17\x3\xFFFF\x1\x17\x1B\xFFFF\x1"+
				"\x17\x13\xFFFF\x1\x17\x5\xFFFF\x1\x17\xA\xFFFF\x1\x17\x12\xFFFF\x1\x17",
				"",
				"",
				"\x1\x18",
				"\x1\x19",
				"\x1\x1A\x12\xFFFF\x1\x1A\xB\xFFFF\x1\x1B\x8\xFFFF\x2\x1A",
				"\x1\x1C",
				"\x1\x1D",
				"\x1\x1E\x1C\xFFFF\x1\x1E\x4\xFFFF\x1\x1E\x3\xFFFF\x1\x1E\x1B\xFFFF\x1"+
				"\x1E\x13\xFFFF\x1\x1E\x5\xFFFF\x1\x1E\xA\xFFFF\x1\x1E\x12\xFFFF\x1\x1E",
				"\x1\xFFFF",
				"\x1\x20",
				"\x1\x21",
				"\x1\x1A\x12\xFFFF\x1\x1A\xB\xFFFF\x1\x1B\x8\xFFFF\x2\x1A",
				"",
				"\x1\x22",
				"\x1\x23",
				"\x1\x24",
				"\x1\x25",
				"\x1\x26",
				"\x1\xFFFF",
				"\x1\x27",
				"\x1\xFFFF"
			};

		private static readonly short[] DFA46_eot = DFA.UnpackEncodedString(DFA46_eotS);
		private static readonly short[] DFA46_eof = DFA.UnpackEncodedString(DFA46_eofS);
		private static readonly char[] DFA46_min = DFA.UnpackEncodedStringToUnsignedChars(DFA46_minS);
		private static readonly char[] DFA46_max = DFA.UnpackEncodedStringToUnsignedChars(DFA46_maxS);
		private static readonly short[] DFA46_accept = DFA.UnpackEncodedString(DFA46_acceptS);
		private static readonly short[] DFA46_special = DFA.UnpackEncodedString(DFA46_specialS);
		private static readonly short[][] DFA46_transition;

		static DFA46()
		{
			int numStates = DFA46_transitionS.Length;
			DFA46_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA46_transition[i] = DFA.UnpackEncodedString(DFA46_transitionS[i]);
			}
		}

		public DFA46( BaseRecognizer recognizer, SpecialStateTransitionHandler specialStateTransition )
			: base(specialStateTransition)
		{
			this.recognizer = recognizer;
			this.decisionNumber = 46;
			this.eot = DFA46_eot;
			this.eof = DFA46_eof;
			this.min = DFA46_min;
			this.max = DFA46_max;
			this.accept = DFA46_accept;
			this.special = DFA46_special;
			this.transition = DFA46_transition;
		}

		public override string Description { get { return "375:1: term : ( identifier -> ^( IDENTIFIER identifier ) | STRING -> ^( STRING_VAL STRING ) | color | calcExpr | URI -> ^( URL_VAL URI ) | (oper= unaryOperator )? val= unitTerm -> ^( UNIT_VAL $val ( $oper)? ) | identifier LPAREN funcArgs RPAREN -> ^( FUNCTION identifier funcArgs ) | legacyFilterMethod LPAREN funcArgs RPAREN -> ^( FUNCTION legacyFilterMethod funcArgs ) | unitExpr );"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private int SpecialStateTransition46(DFA dfa, int s, IIntStream _input)
	{
		ITokenStream input = (ITokenStream)_input;
		int _s = s;
		switch (s)
		{
			case 0:
				int LA46_1 = input.LA(1);


				int index46_1 = input.Index;
				input.Rewind();
				s = -1;
				if ( (LA46_1==LPAREN) ) {s = 10;}

				else if ( (LA46_1==COLON) ) {s = 11;}

				else if ( (EvaluatePredicate(synpred75_CssGrammer_fragment)) ) {s = 12;}

				else if ( (true) ) {s = 13;}


				input.Seek(index46_1);
				if ( s>=0 ) return s;
				break;
			case 1:
				int LA46_2 = input.LA(1);


				int index46_2 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred76_CssGrammer_fragment)) ) {s = 14;}

				else if ( (true) ) {s = 13;}


				input.Seek(index46_2);
				if ( s>=0 ) return s;
				break;
			case 2:
				int LA46_5 = input.LA(1);


				int index46_5 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred77_CssGrammer_fragment)) ) {s = 17;}

				else if ( (true) ) {s = 13;}


				input.Seek(index46_5);
				if ( s>=0 ) return s;
				break;
			case 3:
				int LA46_7 = input.LA(1);


				int index46_7 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred79_CssGrammer_fragment)) ) {s = 19;}

				else if ( (true) ) {s = 13;}


				input.Seek(index46_7);
				if ( s>=0 ) return s;
				break;
			case 4:
				int LA46_9 = input.LA(1);


				int index46_9 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred81_CssGrammer_fragment)) ) {s = 20;}

				else if ( (true) ) {s = 13;}


				input.Seek(index46_9);
				if ( s>=0 ) return s;
				break;
			case 5:
				int LA46_27 = input.LA(1);


				int index46_27 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred78_CssGrammer_fragment)) ) {s = 31;}

				else if ( (true) ) {s = 13;}


				input.Seek(index46_27);
				if ( s>=0 ) return s;
				break;
			case 6:
				int LA46_37 = input.LA(1);


				int index46_37 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred77_CssGrammer_fragment)) ) {s = 17;}

				else if ( (true) ) {s = 13;}


				input.Seek(index46_37);
				if ( s>=0 ) return s;
				break;
			case 7:
				int LA46_39 = input.LA(1);


				int index46_39 = input.Index;
				input.Rewind();
				s = -1;
				if ( (EvaluatePredicate(synpred77_CssGrammer_fragment)) ) {s = 17;}

				else if ( (true) ) {s = 13;}


				input.Seek(index46_39);
				if ( s>=0 ) return s;
				break;
		}
		if (state.backtracking > 0) {state.failed=true; return -1;}
		NoViableAltException nvae = new NoViableAltException(dfa.Description, 46, _s, input);
		dfa.Error(nvae);
		throw nvae;
	}

	#endregion DFA

	#region Follow sets
	private static class Follow
	{
		public static readonly BitSet _charSet_in_styleSheet508 = new BitSet(new ulong[]{0x8840800C1000000UL,0x200000100004090UL});
		public static readonly BitSet _imports_in_styleSheet518 = new BitSet(new ulong[]{0x8840800C1000000UL,0x200000100004090UL});
		public static readonly BitSet _bodylist_in_styleSheet529 = new BitSet(new ulong[]{0x0UL});
		public static readonly BitSet _EOF_in_styleSheet536 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _CHARSET_SYM_in_charSet562 = new BitSet(new ulong[]{0x0UL,0x1000000000000000UL});
		public static readonly BitSet _STRING_in_charSet564 = new BitSet(new ulong[]{0x0UL,0x40000000000000UL});
		public static readonly BitSet _SEMI_in_charSet566 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _IMPORT_SYM_in_imports594 = new BitSet(new ulong[]{0x0UL,0x1000000000000000UL,0x80UL});
		public static readonly BitSet _set_in_imports596 = new BitSet(new ulong[]{0x80000000000000UL,0x40000000000000UL});
		public static readonly BitSet _medium_in_imports603 = new BitSet(new ulong[]{0x4000000UL,0x40000000000000UL});
		public static readonly BitSet _COMMA_in_imports606 = new BitSet(new ulong[]{0x80000000000000UL});
		public static readonly BitSet _medium_in_imports608 = new BitSet(new ulong[]{0x4000000UL,0x40000000000000UL});
		public static readonly BitSet _SEMI_in_imports614 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _MEDIA_SYM_in_media635 = new BitSet(new ulong[]{0x80000000000000UL,0x4000200UL});
		public static readonly BitSet _mediaQuery_in_media637 = new BitSet(new ulong[]{0x4000000UL,0x40UL});
		public static readonly BitSet _COMMA_in_media640 = new BitSet(new ulong[]{0x80000000000000UL,0x4000200UL});
		public static readonly BitSet _mediaQuery_in_media642 = new BitSet(new ulong[]{0x4000000UL,0x40UL});
		public static readonly BitSet _LBRACE_in_media654 = new BitSet(new ulong[]{0x840800C1000000UL,0x200000000000080UL});
		public static readonly BitSet _ruleSet_in_media668 = new BitSet(new ulong[]{0x840800C1000000UL,0x200080000000080UL});
		public static readonly BitSet _RBRACE_in_media679 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _NOT_SYM_in_mediaQuery718 = new BitSet(new ulong[]{0x0UL,0x4000200UL});
		public static readonly BitSet _mediaFeature_in_mediaQuery722 = new BitSet(new ulong[]{0x2UL,0x0UL,0x80000UL});
		public static readonly BitSet _147_in_mediaQuery726 = new BitSet(new ulong[]{0x0UL,0x4000200UL});
		public static readonly BitSet _mediaFeature_in_mediaQuery728 = new BitSet(new ulong[]{0x2UL,0x0UL,0x80000UL});
		public static readonly BitSet _NOT_SYM_in_mediaQuery762 = new BitSet(new ulong[]{0x80000000000000UL});
		public static readonly BitSet _medium_in_mediaQuery766 = new BitSet(new ulong[]{0x2UL,0x0UL,0x80000UL});
		public static readonly BitSet _147_in_mediaQuery769 = new BitSet(new ulong[]{0x0UL,0x4000200UL});
		public static readonly BitSet _mediaFeature_in_mediaQuery771 = new BitSet(new ulong[]{0x2UL,0x0UL,0x80000UL});
		public static readonly BitSet _LPAREN_in_mediaFeature815 = new BitSet(new ulong[]{0x80000000000000UL});
		public static readonly BitSet _property_in_mediaFeature817 = new BitSet(new ulong[]{0x1000000UL});
		public static readonly BitSet _COLON_in_mediaFeature819 = new BitSet(new ulong[]{0x84110800000040UL,0x1000200C14010100UL,0xD00083UL});
		public static readonly BitSet _term_in_mediaFeature821 = new BitSet(new ulong[]{0x0UL,0x800000000000UL});
		public static readonly BitSet _RPAREN_in_mediaFeature823 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _LPAREN_in_mediaFeature849 = new BitSet(new ulong[]{0x80000000000000UL});
		public static readonly BitSet _property_in_mediaFeature851 = new BitSet(new ulong[]{0x0UL,0x800000000000UL});
		public static readonly BitSet _RPAREN_in_mediaFeature853 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _rangeForm_in_mediaFeature877 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _LPAREN_in_rangeForm893 = new BitSet(new ulong[]{0x80000000000000UL});
		public static readonly BitSet _property_in_rangeForm895 = new BitSet(new ulong[]{0x1000000000000UL,0x0UL,0x70000UL});
		public static readonly BitSet _comparisionOp_in_rangeForm897 = new BitSet(new ulong[]{0x84110800000040UL,0x1000200C14010100UL,0xD00083UL});
		public static readonly BitSet _term_in_rangeForm899 = new BitSet(new ulong[]{0x0UL,0x800000000000UL});
		public static readonly BitSet _RPAREN_in_rangeForm901 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _LPAREN_in_rangeForm929 = new BitSet(new ulong[]{0x84110800000040UL,0x1000200C14010100UL,0xD00083UL});
		public static readonly BitSet _term_in_rangeForm933 = new BitSet(new ulong[]{0x1000000000000UL,0x0UL,0x70000UL});
		public static readonly BitSet _rightDirectionOp_in_rangeForm937 = new BitSet(new ulong[]{0x80000000000000UL});
		public static readonly BitSet _property_in_rangeForm939 = new BitSet(new ulong[]{0x1000000000000UL,0x0UL,0x70000UL});
		public static readonly BitSet _rightDirectionOp_in_rangeForm943 = new BitSet(new ulong[]{0x84110800000040UL,0x1000200C14010100UL,0xD00083UL});
		public static readonly BitSet _term_in_rangeForm947 = new BitSet(new ulong[]{0x0UL,0x800000000000UL});
		public static readonly BitSet _RPAREN_in_rangeForm949 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _LPAREN_in_rangeForm985 = new BitSet(new ulong[]{0x84110800000040UL,0x1000200C14010100UL,0xD00083UL});
		public static readonly BitSet _term_in_rangeForm989 = new BitSet(new ulong[]{0x0UL,0x0UL,0x30000UL});
		public static readonly BitSet _leftDirectionOp_in_rangeForm993 = new BitSet(new ulong[]{0x80000000000000UL});
		public static readonly BitSet _property_in_rangeForm995 = new BitSet(new ulong[]{0x0UL,0x0UL,0x30000UL});
		public static readonly BitSet _leftDirectionOp_in_rangeForm999 = new BitSet(new ulong[]{0x84110800000040UL,0x1000200C14010100UL,0xD00083UL});
		public static readonly BitSet _term_in_rangeForm1003 = new BitSet(new ulong[]{0x0UL,0x800000000000UL});
		public static readonly BitSet _RPAREN_in_rangeForm1005 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _leftDirectionOp_in_comparisionOp1050 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _rightDirectionOp_in_comparisionOp1058 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _set_in_leftDirectionOp1073 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _set_in_rightDirectionOp1098 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _IDENT_in_medium1128 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _bodyset_in_bodylist1151 = new BitSet(new ulong[]{0x840800C1000002UL,0x200000100004090UL});
		public static readonly BitSet _ruleSet_in_bodyset1173 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _media_in_bodyset1181 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _page_in_bodyset1189 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _keyframesRule_in_bodyset1197 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _KEYFRAMES_SYM_in_keyframesRule1221 = new BitSet(new ulong[]{0x80000000000000UL});
		public static readonly BitSet _IDENT_in_keyframesRule1223 = new BitSet(new ulong[]{0x0UL,0x40UL});
		public static readonly BitSet _LBRACE_in_keyframesRule1225 = new BitSet(new ulong[]{0x200000000000UL,0x80400000000UL,0x2UL});
		public static readonly BitSet _keyframesBlock_in_keyframesRule1227 = new BitSet(new ulong[]{0x200000000000UL,0x80400000000UL,0x2UL});
		public static readonly BitSet _RBRACE_in_keyframesRule1230 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _keyframeSelectors_in_keyframesBlock1266 = new BitSet(new ulong[]{0x0UL,0x40UL});
		public static readonly BitSet _LBRACE_in_keyframesBlock1268 = new BitSet(new ulong[]{0x80000000000000UL,0x80000000000UL});
		public static readonly BitSet _declarationSet_in_keyframesBlock1273 = new BitSet(new ulong[]{0x0UL,0x80000000000UL});
		public static readonly BitSet _RBRACE_in_keyframesBlock1277 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _keyframeSelector_in_keyframeSelectors1314 = new BitSet(new ulong[]{0x4000002UL});
		public static readonly BitSet _COMMA_in_keyframeSelectors1317 = new BitSet(new ulong[]{0x200000000000UL,0x400000000UL,0x2UL});
		public static readonly BitSet _keyframeSelector_in_keyframeSelectors1319 = new BitSet(new ulong[]{0x4000002UL});
		public static readonly BitSet _set_in_keyframeSelector1353 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _PAGE_SYM_in_page1388 = new BitSet(new ulong[]{0x1000000UL,0x40UL});
		public static readonly BitSet _pseudoPage_in_page1390 = new BitSet(new ulong[]{0x0UL,0x40UL});
		public static readonly BitSet _LBRACE_in_page1401 = new BitSet(new ulong[]{0x80000000000000UL});
		public static readonly BitSet _declaration_in_page1415 = new BitSet(new ulong[]{0x0UL,0x40000000000000UL});
		public static readonly BitSet _SEMI_in_page1417 = new BitSet(new ulong[]{0x80000000000000UL,0x80000000000UL});
		public static readonly BitSet _declaration_in_page1420 = new BitSet(new ulong[]{0x0UL,0x40000000000000UL});
		public static readonly BitSet _SEMI_in_page1422 = new BitSet(new ulong[]{0x80000000000000UL,0x80000000000UL});
		public static readonly BitSet _RBRACE_in_page1434 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _COLON_in_pseudoPage1455 = new BitSet(new ulong[]{0x80000000000000UL});
		public static readonly BitSet _IDENT_in_pseudoPage1457 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _SOLIDUS_in_op1478 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _COMMA_in_op1486 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _PLUS_in_combinator1513 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _GREATER_in_combinator1527 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _152_in_combinator1541 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _set_in_unaryOperator1578 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _IDENT_in_property1611 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _selectors_in_ruleSet1634 = new BitSet(new ulong[]{0x0UL,0x40UL});
		public static readonly BitSet _LBRACE_in_ruleSet1636 = new BitSet(new ulong[]{0x80000000000000UL,0x80000000000UL});
		public static readonly BitSet _declarationSet_in_ruleSet1641 = new BitSet(new ulong[]{0x0UL,0x80000000000UL});
		public static readonly BitSet _RBRACE_in_ruleSet1645 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _selector_in_selectors1683 = new BitSet(new ulong[]{0x4000002UL});
		public static readonly BitSet _COMMA_in_selectors1686 = new BitSet(new ulong[]{0x840000C1000000UL,0x200000000000080UL});
		public static readonly BitSet _selector_in_selectors1688 = new BitSet(new ulong[]{0x4000002UL});
		public static readonly BitSet _FONT_FACE_in_selectors1715 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _declaration_in_declarationSet1734 = new BitSet(new ulong[]{0x0UL,0x40000000000000UL});
		public static readonly BitSet _SEMI_in_declarationSet1736 = new BitSet(new ulong[]{0x80000000000002UL});
		public static readonly BitSet _declaration_in_declarationSet1739 = new BitSet(new ulong[]{0x0UL,0x40000000000000UL});
		public static readonly BitSet _SEMI_in_declarationSet1741 = new BitSet(new ulong[]{0x80000000000002UL});
		public static readonly BitSet _simpleSelector_in_selector1768 = new BitSet(new ulong[]{0x850000C1000002UL,0x200000800000080UL,0x1000000UL});
		public static readonly BitSet _moreSelectors_in_selector1770 = new BitSet(new ulong[]{0x850000C1000002UL,0x200000800000080UL,0x1000000UL});
		public static readonly BitSet _combinator_in_moreSelectors1807 = new BitSet(new ulong[]{0x840000C1000000UL,0x200000000000080UL});
		public static readonly BitSet _simpleSelector_in_moreSelectors1809 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _elementName_in_simpleSelector1844 = new BitSet(new ulong[]{0x840000C1000002UL,0x200000000000080UL});
		public static readonly BitSet _elementSubsequent_in_simpleSelector1851 = new BitSet(new ulong[]{0x840000C1000002UL,0x200000000000080UL});
		public static readonly BitSet _elementSubsequent_in_simpleSelector1885 = new BitSet(new ulong[]{0x840000C1000002UL,0x200000000000080UL});
		public static readonly BitSet _set_in_esPred1923 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _HASH_in_elementSubsequent1962 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _cssClass_in_elementSubsequent1978 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _attrib_in_elementSubsequent1986 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _pseudo_in_elementSubsequent1994 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _DOT_in_cssClass2015 = new BitSet(new ulong[]{0x80000000000000UL});
		public static readonly BitSet _IDENT_in_cssClass2017 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _IDENT_in_elementName2054 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _STAR_in_elementName2070 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _LBRACKET_in_attrib2099 = new BitSet(new ulong[]{0x80000000000000UL});
		public static readonly BitSet _IDENT_in_attrib2101 = new BitSet(new ulong[]{0x3000001000000000UL,0xC00100040000000UL});
		public static readonly BitSet _attribVal_in_attrib2103 = new BitSet(new ulong[]{0x0UL,0x100000000000UL});
		public static readonly BitSet _RBRACKET_in_attrib2107 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _attribOp_in_attribVal2143 = new BitSet(new ulong[]{0x80000000000000UL,0x1000000000000000UL});
		public static readonly BitSet _attribValue_in_attribVal2145 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _set_in_attribValue2178 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _OPEQ_in_attribOp2205 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _INCLUDES_WORD_in_attribOp2219 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _STARTS_WITH_WORD_in_attribOp2233 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _INCLUDES_in_attribOp2247 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _STARTS_WITH_in_attribOp2261 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _ENDS_WITH_in_attribOp2275 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _pseudoStart_in_pseudo2298 = new BitSet(new ulong[]{0x80000000000000UL,0x4000000UL,0x2UL});
		public static readonly BitSet _identifier_in_pseudo2300 = new BitSet(new ulong[]{0x0UL,0x200UL});
		public static readonly BitSet _LPAREN_in_pseudo2302 = new BitSet(new ulong[]{0x80000000000000UL,0x800010020000UL});
		public static readonly BitSet _pseudoFuncArgs_in_pseudo2305 = new BitSet(new ulong[]{0x0UL,0x800000000000UL});
		public static readonly BitSet _RPAREN_in_pseudo2309 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _pseudoStart_in_pseudo2338 = new BitSet(new ulong[]{0x80000000000000UL,0x4000000UL,0x2UL});
		public static readonly BitSet _identifier_in_pseudo2340 = new BitSet(new ulong[]{0x0UL,0x200UL});
		public static readonly BitSet _LPAREN_in_pseudo2342 = new BitSet(new ulong[]{0x840000C1000000UL,0x200800000000080UL});
		public static readonly BitSet _selector_in_pseudo2345 = new BitSet(new ulong[]{0x0UL,0x800000000000UL});
		public static readonly BitSet _RPAREN_in_pseudo2349 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _pseudoStart_in_pseudo2378 = new BitSet(new ulong[]{0x80000000000000UL,0x4000000UL,0x2UL});
		public static readonly BitSet _identifier_in_pseudo2380 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _set_in_pseudoStart2417 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _IDENT_in_pseudoFuncArgs2448 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _MULTIPLIER_in_pseudoFuncArgs2456 = new BitSet(new ulong[]{0x2UL,0x800010000UL});
		public static readonly BitSet _unaryOperator_in_pseudoFuncArgs2459 = new BitSet(new ulong[]{0x0UL,0x10000000UL});
		public static readonly BitSet _NUMBER_in_pseudoFuncArgs2461 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _NUMBER_in_pseudoFuncArgs2471 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _property_in_declaration2488 = new BitSet(new ulong[]{0x1000000UL});
		public static readonly BitSet _COLON_in_declaration2490 = new BitSet(new ulong[]{0x84110800000040UL,0x1000200C14010100UL,0xD00083UL});
		public static readonly BitSet _expressions_in_declaration2492 = new BitSet(new ulong[]{0x400000000000002UL});
		public static readonly BitSet _prio_in_declaration2494 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _IMPORTANT_SYM_in_prio2537 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _expr_in_expressions2554 = new BitSet(new ulong[]{0x4000002UL});
		public static readonly BitSet _COMMA_in_expressions2557 = new BitSet(new ulong[]{0x84110800000040UL,0x1000200C14010100UL,0xD00083UL});
		public static readonly BitSet _expr_in_expressions2559 = new BitSet(new ulong[]{0x4000002UL});
		public static readonly BitSet _term_in_expr2599 = new BitSet(new ulong[]{0x84110800000042UL,0x1000200C14010100UL,0xD00083UL});
		public static readonly BitSet _148_in_calcExpr2634 = new BitSet(new ulong[]{0x0UL,0x200UL});
		public static readonly BitSet _LPAREN_in_calcExpr2636 = new BitSet(new ulong[]{0x110800000040UL,0x200C10010100UL,0x1UL});
		public static readonly BitSet _unitTerm_in_calcExpr2638 = new BitSet(new ulong[]{0x0UL,0x300800800010000UL});
		public static readonly BitSet _operators_in_calcExpr2641 = new BitSet(new ulong[]{0x110800000040UL,0x200C10010100UL,0x1UL});
		public static readonly BitSet _unitTerm_in_calcExpr2643 = new BitSet(new ulong[]{0x0UL,0x300800800010000UL});
		public static readonly BitSet _RPAREN_in_calcExpr2647 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _set_in_operators2674 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _identifier_in_term2712 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _STRING_in_term2728 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _color_in_term2744 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _calcExpr_in_term2752 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _URI_in_term2760 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _unaryOperator_in_term2779 = new BitSet(new ulong[]{0x110800000040UL,0x200C10010100UL,0x1UL});
		public static readonly BitSet _unitTerm_in_term2785 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _identifier_in_term2806 = new BitSet(new ulong[]{0x0UL,0x200UL});
		public static readonly BitSet _LPAREN_in_term2808 = new BitSet(new ulong[]{0x84110800000040UL,0x1000200C14010100UL,0xF00083UL});
		public static readonly BitSet _funcArgs_in_term2810 = new BitSet(new ulong[]{0x0UL,0x800000000000UL});
		public static readonly BitSet _RPAREN_in_term2812 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _legacyFilterMethod_in_term2838 = new BitSet(new ulong[]{0x0UL,0x200UL});
		public static readonly BitSet _LPAREN_in_term2840 = new BitSet(new ulong[]{0x84110800000040UL,0x1000200C14010100UL,0xF00083UL});
		public static readonly BitSet _funcArgs_in_term2842 = new BitSet(new ulong[]{0x0UL,0x800000000000UL});
		public static readonly BitSet _RPAREN_in_term2844 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _unitExpr_in_term2870 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _identifier_in_legacyFilterMethod2883 = new BitSet(new ulong[]{0x1000000UL});
		public static readonly BitSet _COLON_in_legacyFilterMethod2885 = new BitSet(new ulong[]{0x80000000000000UL,0x4000000UL,0x2UL});
		public static readonly BitSet _identifier_in_legacyFilterMethod2887 = new BitSet(new ulong[]{0x40000002UL});
		public static readonly BitSet _DOT_in_legacyFilterMethod2890 = new BitSet(new ulong[]{0x80000000000000UL,0x4000000UL,0x2UL});
		public static readonly BitSet _identifier_in_legacyFilterMethod2892 = new BitSet(new ulong[]{0x40000002UL});
		public static readonly BitSet _identifier_in_unitExpr2906 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _STRING_in_unitExpr2922 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _color_in_unitExpr2938 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _calcExpr_in_unitExpr2946 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _URI_in_unitExpr2954 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _unaryOperator_in_unitExpr2973 = new BitSet(new ulong[]{0x110800000040UL,0x200C10010100UL,0x1UL});
		public static readonly BitSet _unitTerm_in_unitExpr2979 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _unitExpr_in_funcArg3013 = new BitSet(new ulong[]{0x84110800000042UL,0x1000200C14010100UL,0xD00083UL});
		public static readonly BitSet _identifier_in_funcArg3031 = new BitSet(new ulong[]{0x0UL,0x40000000UL});
		public static readonly BitSet _OPEQ_in_funcArg3033 = new BitSet(new ulong[]{0x84110800000040UL,0x1000200C14010100UL,0xF00083UL});
		public static readonly BitSet _funcArg_in_funcArg3035 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _149_in_funcArg3055 = new BitSet(new ulong[]{0x0UL,0x200UL});
		public static readonly BitSet _LPAREN_in_funcArg3057 = new BitSet(new ulong[]{0x84110800000040UL,0x1000200C14010100UL,0xF00083UL});
		public static readonly BitSet _funcArgs_in_funcArg3059 = new BitSet(new ulong[]{0x0UL,0x800000000000UL});
		public static readonly BitSet _RPAREN_in_funcArg3061 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _funcArg_in_funcArgs3089 = new BitSet(new ulong[]{0x4000002UL});
		public static readonly BitSet _COMMA_in_funcArgs3092 = new BitSet(new ulong[]{0x84110800000040UL,0x1000200C14010100UL,0xF00083UL});
		public static readonly BitSet _funcArg_in_funcArgs3094 = new BitSet(new ulong[]{0x4000002UL});
		public static readonly BitSet _set_in_unitTerm3111 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _set_in_identifier3188 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _151_in_color3227 = new BitSet(new ulong[]{0x0UL,0x200UL});
		public static readonly BitSet _LPAREN_in_color3229 = new BitSet(new ulong[]{0x0UL,0x10000000UL});
		public static readonly BitSet _NUMBER_in_color3233 = new BitSet(new ulong[]{0x4000000UL});
		public static readonly BitSet _COMMA_in_color3235 = new BitSet(new ulong[]{0x0UL,0x10000000UL});
		public static readonly BitSet _NUMBER_in_color3239 = new BitSet(new ulong[]{0x4000000UL});
		public static readonly BitSet _COMMA_in_color3241 = new BitSet(new ulong[]{0x0UL,0x10000000UL});
		public static readonly BitSet _NUMBER_in_color3245 = new BitSet(new ulong[]{0x4000000UL});
		public static readonly BitSet _COMMA_in_color3247 = new BitSet(new ulong[]{0x0UL,0x10000000UL});
		public static readonly BitSet _NUMBER_in_color3251 = new BitSet(new ulong[]{0x0UL,0x800000000000UL});
		public static readonly BitSet _RPAREN_in_color3253 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _150_in_color3295 = new BitSet(new ulong[]{0x0UL,0x200UL});
		public static readonly BitSet _LPAREN_in_color3297 = new BitSet(new ulong[]{0x0UL,0x10000000UL});
		public static readonly BitSet _NUMBER_in_color3301 = new BitSet(new ulong[]{0x4000000UL});
		public static readonly BitSet _COMMA_in_color3303 = new BitSet(new ulong[]{0x0UL,0x10000000UL});
		public static readonly BitSet _NUMBER_in_color3307 = new BitSet(new ulong[]{0x4000000UL});
		public static readonly BitSet _COMMA_in_color3309 = new BitSet(new ulong[]{0x0UL,0x10000000UL});
		public static readonly BitSet _NUMBER_in_color3313 = new BitSet(new ulong[]{0x0UL,0x800000000000UL});
		public static readonly BitSet _RPAREN_in_color3315 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _HASH_in_color3356 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _LPAREN_in_synpred13_CssGrammer815 = new BitSet(new ulong[]{0x80000000000000UL});
		public static readonly BitSet _property_in_synpred13_CssGrammer817 = new BitSet(new ulong[]{0x1000000UL});
		public static readonly BitSet _COLON_in_synpred13_CssGrammer819 = new BitSet(new ulong[]{0x84110800000040UL,0x1000200C14010100UL,0xD00083UL});
		public static readonly BitSet _term_in_synpred13_CssGrammer821 = new BitSet(new ulong[]{0x0UL,0x800000000000UL});
		public static readonly BitSet _RPAREN_in_synpred13_CssGrammer823 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _LPAREN_in_synpred14_CssGrammer849 = new BitSet(new ulong[]{0x80000000000000UL});
		public static readonly BitSet _property_in_synpred14_CssGrammer851 = new BitSet(new ulong[]{0x0UL,0x800000000000UL});
		public static readonly BitSet _RPAREN_in_synpred14_CssGrammer853 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _LPAREN_in_synpred15_CssGrammer893 = new BitSet(new ulong[]{0x80000000000000UL});
		public static readonly BitSet _property_in_synpred15_CssGrammer895 = new BitSet(new ulong[]{0x1000000000000UL,0x0UL,0x70000UL});
		public static readonly BitSet _comparisionOp_in_synpred15_CssGrammer897 = new BitSet(new ulong[]{0x84110800000040UL,0x1000200C14010100UL,0xD00083UL});
		public static readonly BitSet _term_in_synpred15_CssGrammer899 = new BitSet(new ulong[]{0x0UL,0x800000000000UL});
		public static readonly BitSet _RPAREN_in_synpred15_CssGrammer901 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _LPAREN_in_synpred16_CssGrammer929 = new BitSet(new ulong[]{0x84110800000040UL,0x1000200C14010100UL,0xD00083UL});
		public static readonly BitSet _term_in_synpred16_CssGrammer933 = new BitSet(new ulong[]{0x1000000000000UL,0x0UL,0x70000UL});
		public static readonly BitSet _rightDirectionOp_in_synpred16_CssGrammer937 = new BitSet(new ulong[]{0x80000000000000UL});
		public static readonly BitSet _property_in_synpred16_CssGrammer939 = new BitSet(new ulong[]{0x1000000000000UL,0x0UL,0x70000UL});
		public static readonly BitSet _rightDirectionOp_in_synpred16_CssGrammer943 = new BitSet(new ulong[]{0x84110800000040UL,0x1000200C14010100UL,0xD00083UL});
		public static readonly BitSet _term_in_synpred16_CssGrammer947 = new BitSet(new ulong[]{0x0UL,0x800000000000UL});
		public static readonly BitSet _RPAREN_in_synpred16_CssGrammer949 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _esPred_in_synpred42_CssGrammer1848 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _esPred_in_synpred44_CssGrammer1882 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _pseudoStart_in_synpred61_CssGrammer2298 = new BitSet(new ulong[]{0x80000000000000UL,0x4000000UL,0x2UL});
		public static readonly BitSet _identifier_in_synpred61_CssGrammer2300 = new BitSet(new ulong[]{0x0UL,0x200UL});
		public static readonly BitSet _LPAREN_in_synpred61_CssGrammer2302 = new BitSet(new ulong[]{0x80000000000000UL,0x800010020000UL});
		public static readonly BitSet _pseudoFuncArgs_in_synpred61_CssGrammer2305 = new BitSet(new ulong[]{0x0UL,0x800000000000UL});
		public static readonly BitSet _RPAREN_in_synpred61_CssGrammer2309 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _pseudoStart_in_synpred63_CssGrammer2338 = new BitSet(new ulong[]{0x80000000000000UL,0x4000000UL,0x2UL});
		public static readonly BitSet _identifier_in_synpred63_CssGrammer2340 = new BitSet(new ulong[]{0x0UL,0x200UL});
		public static readonly BitSet _LPAREN_in_synpred63_CssGrammer2342 = new BitSet(new ulong[]{0x840000C1000000UL,0x200800000000080UL});
		public static readonly BitSet _selector_in_synpred63_CssGrammer2345 = new BitSet(new ulong[]{0x0UL,0x800000000000UL});
		public static readonly BitSet _RPAREN_in_synpred63_CssGrammer2349 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _identifier_in_synpred75_CssGrammer2712 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _STRING_in_synpred76_CssGrammer2728 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _color_in_synpred77_CssGrammer2744 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _calcExpr_in_synpred78_CssGrammer2752 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _URI_in_synpred79_CssGrammer2760 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _unaryOperator_in_synpred81_CssGrammer2779 = new BitSet(new ulong[]{0x110800000040UL,0x200C10010100UL,0x1UL});
		public static readonly BitSet _unitTerm_in_synpred81_CssGrammer2785 = new BitSet(new ulong[]{0x2UL});

	}
	#endregion Follow sets
}

} // namespace CssParser

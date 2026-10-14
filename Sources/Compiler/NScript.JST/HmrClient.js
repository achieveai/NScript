// The live patch client baked into each dev bundle (HmrClient.cs). The bake trims each line,
// drops full-line // comments and joins the rest with spaces, so every statement ends in ';'
// or '}' and no line ends in a trailing // comment. hmr-client.test.mjs runs this file.
(function(){
if(typeof window==='undefined'||window.__nsHmrBoot)return;
var W=window,P='[nscript-hmr] ',all=W.__nsHmr||(W.__nsHmr={});
function log(m){console.info(P+m);}
function args(s){var a=s.split('|'),g=(a[4]||':0').split(':');return {file:a[0],id:a[1],url:a[2],token:a[3],gen:g[0],seq:+g[1]};}
function boot(text){var re=/__nsHmrBoot\("([^"]*)"\)/g,m,r=null;while((m=re.exec(text)))r=m[1];return r;}
W.__nsHmrBoot=function(s){
var b=args(s),file=b.file,store=null,key='nscriptHmr:'+file;
var st=all[file]={gen:b.gen,seq:b.seq,baseSeq:b.seq,applied:[],notes:[],note:null,lastReload:null,state:'disconnected'};
var cs=document.currentScript,src=cs&&cs.src;
if(!src){var list=document.getElementsByTagName('script');for(var i=0;i<list.length;i++){var u=list[i].src||'';if(u.split('?')[0].split('/').pop()===file)src=u;}}
try{store=W.sessionStorage;var lr=store.getItem(key+':reason');if(lr){store.removeItem(key+':reason');st.lastReload=lr;log('reloaded: '+lr);}}catch(e){}
// The bundle's closure threw while it loaded: a patch cannot finish its start, so the next build reloads.
var failed=W.__nsHmrFailed&&W.__nsHmrFailed[file];if(failed){st.failed=failed;log('load failed: '+failed+'; the next build reloads');}
var off=false;try{off=W.localStorage.getItem('nscriptHmr')==='off';}catch(e){}
if(off){st.state='off';log('off (localStorage)');return;}
// again: the check a waiting page repeats, as [reason, gen]; null when the page is in step.
var ws=null,delay=1000,timer=null,stopped=false,opened=false,again=null,capped=false;
function stop(state,m){stopped=true;if(state)st.state=state;if(m)log(m);if(timer){clearTimeout(timer);timer=null;}if(ws){try{ws.close();}catch(e){}ws=null;}}
function reload(reason){
var now=Date.now(),times=[];
try{times=JSON.parse(store.getItem('nscriptHmr:reloads')||'[]').filter(function(t){return now-t<60000;});}catch(e){}
// The cap refuses a reload, not patches: the socket stays, and a reload after the 60 s goes through.
if(times.length>=2){st.state='reload-only';again=null;if(!capped){capped=true;log('reload-only: 2 reloads in 60 s; live patches still apply');}return;}
capped=false;times.push(now);
try{store.setItem('nscriptHmr:reloads',JSON.stringify(times));store.setItem(key+':reason',reason);}catch(e){}
log('reloading: '+reason);stop(null,null);W.location.reload();
}
// With a socket, the next step checks the script again (a lagging copy updates with no
// socket event); without one, it connects. One timer, backoff 1 s to 30 s, paused while hidden.
function next(){if(stopped)return;if(!ws){connect();return;}if(again)check(again[0],again[1]);}
function retry(){
if(stopped||timer)return;
var d=delay;delay=Math.min(delay*2,30000);
timer=setTimeout(function(){timer=null;if(!document.hidden){next();return;}document.addEventListener('visibilitychange',function v(){if(!document.hidden){document.removeEventListener('visibilitychange',v);next();}});},d);
}
// The state says whether the page has a socket; waitMsg says what it waits for.
function wait(m){if(m!==st.waitMsg){st.waitMsg=m;if(m)log(m);}st.state=ws?'waiting':'disconnected';retry();}
function check(reason,gen){
if(stopped)return;
again=[reason,gen];
if(!src){wait('self-check failed: no script src');return;}
W.fetch(src,{cache:'no-cache'}).then(function(r){
if(r.status!==200)throw new Error(String(r.status));
var ct=r.headers.get('content-type')||'';
return r.text().then(function(t){if(/html/i.test(ct)||/^\s*</.test(t))throw new Error('html');return t;});
}).then(function(t){
if(stopped)return;
if(t.indexOf('__nsPatch')<0){reload(reason||'plain bundle');return;}
var s=boot(t);if(s===null)throw new Error('no boot call');
var c=args(s);
// A script at or behind the page's own seq (a lagging copy) holds no newer code: wait.
if((c.gen===st.gen&&c.seq<=st.seq)||(gen&&c.gen!==gen)){wait(gen&&gen!==st.gen?'waiting for bundle '+gen:(ws||!b.url?null:'cannot reach the build service; retrying'));return;}
reload(reason||('bundle '+c.gen+':'+c.seq));
}).catch(function(e){if(!stopped)wait('self-check failed: '+(e&&e.message||e));});
}
// A notice says why the page's code is not current (a red build); an empty one ends it.
function notice(n){if(n){if(n!==st.note){st.note=n;log(n);}}else if(st.note){st.note=null;log('build ok');}}
function receive(m){
if(stopped)return;
if(typeof m.note==='string'){notice(m.note);return;}
if(m.gen!==st.gen){check(m.reload||'new build',m.gen);return;}
if(m.reload){check(m.reload,m.gen);return;}
var rs=m.records||[];
for(var i=0;i<rs.length;i++){
var r=rs[i];
if(r.seq<=st.seq)continue;
if(r.seq>st.seq+1){check('gap',m.gen);return;}
if(r.reload||st.failed){check(r.reload||'load failed',m.gen);return;}
try{W.__nsPatch[file](r.js);}catch(e){log('live patch failed: '+(e&&e.message||e));check('live patch failed',m.gen);return;}
st.seq=r.seq;st.applied.push(r.seq);st.note=null;
var ns=r.notes||[];for(var j=0;j<ns.length;j++){st.notes.push(ns[j]);log('note: '+ns[j]+' keeps old code until it runs again');}
log('applied '+st.gen+':'+r.seq+' ('+r.methods+(r.methods===1?' method)':' methods)'));
}
// The page is in step with the socket's gen: a wait ends and its backoff restarts.
if(again){again=null;delay=1000;st.state='connected';st.waitMsg=null;}
}
function connect(){
if(stopped)return;
if(!b.url){check(null,null);return;}
opened=false;
try{ws=new WebSocket(b.url+'?t='+encodeURIComponent(b.token)+'&id='+b.id+'&file='+encodeURIComponent(file)+'&at='+st.gen+':'+st.seq);}catch(e){stop('off','socket failed: '+(e&&e.message||e));return;}
ws.onopen=function(){opened=true;delay=1000;st.state='connected';st.waitMsg=null;log('connected '+file+' at '+st.gen+':'+st.seq);};
ws.onmessage=function(ev){var m;try{m=JSON.parse(ev.data);}catch(e){return;}receive(m);};
ws.onclose=function(){ws=null;if(stopped)return;if(opened){st.state='disconnected';log('disconnected; reconnecting in '+Math.round(delay/1000)+'s');retry();}else{check(null,null);}};
}
document.addEventListener('securitypolicyviolation',function(e){var o=b.url&&b.url.split('/nscript-hmr')[0];if(o&&String(e.blockedURI||'').indexOf(o)===0)stop('off','blocked by CSP connect-src');});
connect();
};
})();

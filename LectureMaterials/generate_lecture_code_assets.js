const fs = require('fs');
const path = require('path');
const crypto = require('crypto');

const root = path.resolve(__dirname, '..');
const runtimeRoot = path.join(root, 'Assets', 'HUSTEnemyAILab', 'Scripts', 'Runtime');
const extractDir = path.join(__dirname, 'CodeExtracts');
const imageDir = path.join(__dirname, 'CodeImages');
const indexPath = path.join(__dirname, 'Lecture_Code_Index.md');
const playwrightPath = 'C:/Users/UC4-/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules/playwright';
const browserCandidates = [
  'C:/Program Files/Google/Chrome/Application/chrome.exe',
  'C:/Program Files (x86)/Microsoft/Edge/Application/msedge.exe'
];

const stages = {
  Prototype: { label: 'PROTOTYPE', color: '#48C774' },
  Intermediate: { label: 'INTERMEDIATE FSM', color: '#F2B84B' },
  Production: { label: 'PRODUCTION', color: '#4DA3FF' }
};

const excerpts = [
  ['P1','Prototype','Prototype/PrototypeSlime.cs',32,55,[40,41,52,53,54],'Complete fixed-step patrol loop.','Rigidbody movement, boundary normalization, direction state, sprite orientation.'],
  ['P2','Prototype','Prototype/PrototypeSlime.cs',40,54,[43,44,45,47,48,49,52,53,54],'Direction reversal at both patrol boundaries.','Threshold checks, mutually exclusive reversal, direction-to-velocity mapping.'],
  ['I1','Intermediate','Intermediate/FsmSlime.cs',6,6,[6],'Introduce the state vocabulary.','Enum-based finite state representation.'],
  ['I2','Intermediate','Intermediate/FsmSlime.cs',55,74,[58,59,60,61,62,68,69,70,71,72],'Frame update and physics update dispatch.','State dispatch and separation of decision timing from physics movement.'],
  ['I3','Intermediate','Intermediate/FsmSlime.cs',76,90,[78,83,84,85,89],'Patrol-to-Chase and Chase-to-Patrol/Attack decisions.','Guard conditions and directed FSM transitions.'],
  ['I4','Intermediate','Intermediate/FsmSlime.cs',92,122,[94,96,100,102,106,108,110,111,112,117,119,120],'Attack exits and timed wind-up, impact, and cooldown.','Temporal state within a state, early returns, and attack lifecycle.'],
  ['I4a','Intermediate','Intermediate/FsmSlime.cs',92,104,[94,96,100,102],'Exit conditions from Attack.','Transition priority and guard clauses.'],
  ['I4b','Intermediate','Intermediate/FsmSlime.cs',106,122,[106,108,110,111,112,117,119,120],'Attack wind-up, impact, and cooldown.','Time-driven lifecycle and deferred effects.'],
  ['I5','Intermediate','Intermediate/FsmSlime.cs',124,131,[126,127,128,129,130],'Centralized transition side effects.','Transition gateway, cancellation, entry action, and presentation feedback.'],
  ['I6','Intermediate','Intermediate/FsmSlime.cs',133,146,[141,142,143,144,145],'Patrol behavior inside the FSM version.','Reusable movement behavior under state control.'],
  ['I7','Intermediate','Intermediate/FsmSlime.cs',148,167,[150,152,155,160,161,166],'Chase movement and shared movement helpers.','Target steering, helper extraction, and defensive stopping.'],
  ['I8','Intermediate','Intermediate/FsmSlime.cs',169,180,[169,170,171,173,174,178,179],'Detection, attack-range checks, and damage application.','Boolean queries, spatial tests, and interface-based damage.'],
  ['R1','Production','Production/EnemyBrain.cs',6,12,[8,9,10,12],'Introduce the production coordinator and collaborators.','Composition and orchestration.'],
  ['R2','Production','Production/EnemyBrain.cs',15,41,[17,18,19,22,25,26,29,30,31,38,39,40],'Brain coordination across sensing, combat, decisions, and movement.','Thin coordinator, state dispatch, and Update/FixedUpdate responsibility split.'],
  ['R2a','Production','Production/EnemyBrain.cs',15,34,[17,18,19,22,25,26,29,30,31],'Sensing/combat ticks and state decisions.','Orchestration and collaborator delegation.'],
  ['R2b','Production','Production/EnemyBrain.cs',36,41,[38,39,40],'State-to-movement dispatch.','Physics-loop delegation.'],
  ['R3','Production','Production/EnemyBrain.cs',43,50,[45,46,47,48,49],'Transition cleanup and entry behavior.','Centralized transition side effects and attack cancellation.'],
  ['R4','Production','Production/EnemySensor.cs',5,23,[12,14,15,16,17,18,21,22,23],'Sensor ownership of target acquisition and range queries.','Single responsibility, physics query, and encapsulated perception API.'],
  ['R5','Production','Production/EnemyMovement.cs',18,39,[20,21,22,23,24,25,30,31,34,37,38],'Movement ownership for patrol, chase, stop, and facing.','Single responsibility, target steering, and a shared movement primitive.'],
  ['R6','Production','Production/EnemyCombat.cs',6,10,[6,8,9],'Introduce safe versus intentionally unsafe validation modes.','Explicit policy selection for a teaching experiment.'],
  ['R7','Production','Production/EnemyCombat.cs',24,31,[26,27,28,29],'Attack admission and wind-up setup.','Preconditions, target snapshot, and deferred impact.'],
  ['R8','Production','Production/EnemyCombat.cs',33,45,[35,36,37,38,39,40,41,42,43,44],'Impact-time revalidation, damage, and cooldown.','Attack lifecycle, stale-target defense, and interface-based damage.'],
  ['R9','Production','Production/EnemyCombat.cs',47,52,[49,50,51],'Safe attack cancellation.','Lifecycle cleanup and policy-dependent behavior.'],
  ['R10','Production','Production/EnemyHealth.cs',8,27,[15,16,17,20,22,24,25,26],'Health state, damage handling, and death dispatch.','Interface implementation, invariant clamping, and event-driven lifecycle.'],
  ['R11','Production','Production/EnemyHealth.cs',37,44,[39,40,41,42,43],'Death side effects.','Idempotence, component shutdown, and event notification.'],
  ['R12','Production','Production/EnemyConfig.cs',5,16,[5,6,8,9,10,11,12,13,14,15,16],'ScriptableObject and serialized tuning surface.','Data-driven configuration, reusable assets, and constrained inspector values.'],
  ['R13','Production','Production/EnemyConfig.cs',18,30,[21,22,23,24,25,26,27,28,29],'Validated runtime/test configuration.','Boundary validation and configuration invariants.']
].map(([id,stage,source,start,end,highlights,purpose,concept]) => ({id,stage,source,start,end,highlights,purpose,concept}));

function sha256(value) {
  return crypto.createHash('sha256').update(value).digest('hex');
}

function splitPhysicalLines(text) {
  const lines = [];
  const pattern = /.*?(?:\r\n|\n|\r|$)/g;
  let match;
  while ((match = pattern.exec(text)) !== null) {
    if (match[0] === '') break;
    const ending = match[0].match(/(?:\r\n|\n|\r)$/)?.[0] || '';
    lines.push({ content: match[0].slice(0, match[0].length - ending.length), ending, full: match[0] });
  }
  return lines;
}

function esc(value) {
  return value.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;');
}

const keywords = new Set(('abstract as base bool break byte case catch char checked class const continue decimal default delegate do double else enum event explicit extern false finally fixed float for foreach goto if implicit in int interface internal is lock long namespace new null object operator out override params private protected public readonly record ref return sbyte sealed short sizeof stackalloc static string struct switch this throw true try typeof uint ulong unchecked unsafe ushort using virtual void volatile while yield async await get set init value var').split(' '));
const knownTypes = new Set(('Action Behaviour Collider2D Color GameObject Gizmos IDamageable IEnumerator LayerMask Mathf MonoBehaviour Rigidbody2D ScriptableObject SerializeField SpriteRenderer Time Tooltip Transform Vector2 Vector3 WaitForSeconds').split(' '));

function tokenizeLine(line) {
  const tokenPattern = /(\/\/.*$|\/\*.*?\*\/|\$?@?"(?:""|\\.|[^"\\])*"|'(?:\\.|[^'\\])+'|\b\d+(?:\.\d+)?f?\b|\b[A-Za-z_][A-Za-z0-9_]*\b|\[[A-Za-z_][^\]]*\])/g;
  let out = '';
  let cursor = 0;
  for (const match of line.matchAll(tokenPattern)) {
    out += esc(line.slice(cursor, match.index));
    const raw = match[0];
    let cls = '';
    if (raw.startsWith('//') || raw.startsWith('/*')) cls = 'comment';
    else if (raw.startsWith('"') || raw.startsWith('@"') || raw.startsWith('$"') || raw.startsWith('$@"') || raw.startsWith('@$"') || raw.startsWith("'")) cls = 'string';
    else if (/^\d/.test(raw)) cls = 'number';
    else if (raw.startsWith('[')) cls = 'attribute';
    else if (keywords.has(raw)) cls = 'keyword';
    else if (knownTypes.has(raw) || /^[A-Z][A-Za-z0-9_]*$/.test(raw)) cls = 'type';
    out += cls ? `<span class="${cls}">${esc(raw)}</span>` : esc(raw);
    cursor = match.index + raw.length;
  }
  out += esc(line.slice(cursor));
  return out;
}

function fontSizeFor(lines, longestLine) {
  if (lines <= 10) return 38;
  if (lines <= 18) return Math.min(34, Math.max(24, Math.floor(2180 / Math.max(1, longestLine) / 0.62)));
  if (lines <= 24) return Math.min(30, Math.max(24, Math.floor(2180 / Math.max(1, longestLine) / 0.62)));
  if (lines <= 28) return Math.min(28, Math.max(24, Math.floor(2180 / Math.max(1, longestLine) / 0.62)));
  return Math.min(26, Math.max(24, Math.floor(2180 / Math.max(1, longestLine) / 0.62)));
}

function buildHtml(item, displayLines) {
  const stage = stages[item.stage];
  const longestLine = Math.max(...displayLines.map(line => line.length));
  const fontSize = Math.min(fontSizeFor(displayLines.length, longestLine),
    Math.max(24, Math.floor(2180 / Math.max(1, longestLine) / 0.62)));
  const rows = displayLines.map((text, offset) => {
    const number = item.start + offset;
    const selected = item.highlights.includes(number) ? ' selected' : '';
    return `<div class="code-row${selected}" data-line="${number}"><span class="line-number">${number}</span><code>${tokenizeLine(text)}</code></div>`;
  }).join('');
  return `<!doctype html><html><head><meta charset="utf-8"><style>
    * { box-sizing: border-box; }
    html, body { margin: 0; width: 2560px; height: 1440px; overflow: hidden; background: #090D14; }
    body { font-family: "Cascadia Mono", "Cascadia Code", Consolas, monospace; color: #D8DEE9; }
    .frame { width: 2560px; height: 1440px; padding: 60px 72px 64px; background: radial-gradient(circle at 88% 0%, #152033 0, #0C111B 34%, #090D14 72%); }
    .card { width: 100%; height: 100%; border: 1px solid #273246; border-radius: 24px; overflow: hidden; background: #0F1623; box-shadow: 0 24px 70px rgba(0,0,0,.38); }
    .header { height: 126px; display: flex; align-items: center; gap: 28px; padding: 0 42px; border-bottom: 1px solid #273246; background: #121B2A; border-top: 8px solid ${stage.color}; }
    .stage { flex: none; padding: 11px 18px; border: 2px solid ${stage.color}; border-radius: 999px; color: ${stage.color}; font: 700 22px/1 "Cascadia Mono"; letter-spacing: .08em; }
    .title { min-width: 0; }
    .filename { color: #F3F6FA; font: 700 34px/1.2 "Cascadia Mono"; white-space: nowrap; }
    .range { margin-top: 8px; color: #8D9AAF; font: 21px/1.2 "Cascadia Mono"; }
    .code { padding: 30px 34px 32px 22px; font-size: ${fontSize}px; line-height: 1.38; white-space: pre; }
    .code-row { display: flex; min-height: 1.38em; border-left: 5px solid transparent; border-radius: 7px; }
    .code-row.selected { background: rgba(255,255,255,.065); border-left-color: ${stage.color}; }
    .line-number { flex: 0 0 92px; padding-right: 25px; text-align: right; color: #536176; user-select: none; }
    code { display: block; flex: none; color: #D8DEE9; font: inherit; }
    .keyword { color: #C792EA; } .type { color: #82AAFF; } .string { color: #C3E88D; }
    .number { color: #F78C6C; } .comment { color: #6B7A90; font-style: italic; } .attribute { color: #FFCB6B; }
  </style></head><body><div class="frame"><div class="card"><header class="header"><div class="stage">${stage.label}</div><div class="title"><div class="filename">${esc(path.basename(item.source))}</div><div class="range">${esc(item.source)} · lines ${item.start}–${item.end}</div></div></header><main class="code">${rows}</main></div></div></body></html>`;
}

function pngDimensions(buffer) {
  if (buffer.length < 24 || buffer.toString('ascii', 1, 4) !== 'PNG') throw new Error('Not a PNG');
  return { width: buffer.readUInt32BE(16), height: buffer.readUInt32BE(20) };
}

async function main() {
  fs.mkdirSync(extractDir, { recursive: true });
  fs.mkdirSync(imageDir, { recursive: true });
  const { chromium } = require(playwrightPath);
  const executablePath = browserCandidates.find(fs.existsSync);
  if (!executablePath) throw new Error('Chrome or Edge executable not found.');
  const browser = await chromium.launch({ executablePath, headless: true });
  const page = await browser.newPage({ viewport: { width: 2560, height: 1440 }, deviceScaleFactor: 1 });
  const sourceCache = new Map();
  const results = [];

  try {
    for (const item of excerpts) {
      const sourcePath = path.join(runtimeRoot, ...item.source.split('/'));
      if (!sourceCache.has(sourcePath)) {
        const sourceBuffer = fs.readFileSync(sourcePath);
        const sourceText = sourceBuffer.toString('utf8').replace(/^\uFEFF/, '');
        sourceCache.set(sourcePath, { sourceBuffer, sourceText, lines: splitPhysicalLines(sourceText), hash: sha256(sourceBuffer) });
      }
      const source = sourceCache.get(sourcePath);
      if (item.start < 1 || item.end > source.lines.length || item.end < item.start) throw new Error(`Invalid range for ${item.id}`);
      const selected = source.lines.slice(item.start - 1, item.end);
      const excerptText = selected.map(line => line.full).join('');
      const displayLines = selected.map(line => line.content);
      const extractName = `${item.id}_${path.basename(item.source, '.cs')}_L${String(item.start).padStart(3,'0')}-L${String(item.end).padStart(3,'0')}.cs`;
      const imageName = extractName.replace(/\.cs$/, '.png');
      const extractPath = path.join(extractDir, extractName);
      const imagePath = path.join(imageDir, imageName);
      fs.writeFileSync(extractPath, excerptText, 'utf8');
      const extractedBuffer = fs.readFileSync(extractPath);
      const sourceSliceBuffer = Buffer.from(excerptText, 'utf8');
      const byteExact = extractedBuffer.equals(sourceSliceBuffer);
      if (!byteExact) throw new Error(`Extract validation failed for ${item.id}`);

      await page.setContent(buildHtml(item, displayLines), { waitUntil: 'load' });
      await page.evaluate(() => document.fonts.ready);
      const domValidation = await page.evaluate(({expected, start}) => {
        const rows = [...document.querySelectorAll('.code-row')];
        return {
          rowCount: rows.length,
          textMatches: rows.every((row, i) => row.querySelector('code').textContent === expected[i] && Number(row.dataset.line) === start + i),
          clipped: document.querySelector('.code').scrollHeight > document.querySelector('.card').clientHeight - document.querySelector('.header').clientHeight ||
                   document.querySelector('.code').scrollWidth > document.querySelector('.card').clientWidth
        };
      }, { expected: displayLines, start: item.start });
      if (domValidation.rowCount !== displayLines.length || !domValidation.textMatches) throw new Error(`DOM source validation failed for ${item.id}`);
      if (domValidation.clipped) throw new Error(`Rendered content clipped for ${item.id}`);
      await page.screenshot({ path: imagePath, type: 'png' });
      const imageBuffer = fs.readFileSync(imagePath);
      const dimensions = pngDimensions(imageBuffer);
      if (dimensions.width !== 2560 || dimensions.height !== 1440) throw new Error(`Unexpected dimensions for ${item.id}`);
      results.push({ ...item, extractName, imageName, sourceHash: source.hash, excerptHash: sha256(extractedBuffer), byteExact, domValidation, dimensions });
      process.stdout.write(`${item.id}: validated and rendered\n`);
    }
  } finally {
    await browser.close();
  }

  const uniqueSources = [...new Map(results.map(r => [r.source, r.sourceHash])).entries()];
  const hashTable = uniqueSources.map(([source, hash]) => `| \`${source}\` | \`${hash}\` |`).join('\n');
  const resultTable = results.map(r => `| ${r.id} | ${r.stage} | \`${r.source}\` | ${r.start}-${r.end} | \`${r.extractName}\` | \`${r.imageName}\` | ${r.purpose} | ${r.highlights.join(', ')} | PASS — exact bytes, ${r.domValidation.rowCount} lines, ${r.dimensions.width}×${r.dimensions.height} | \`${r.excerptHash}\` |`).join('\n');
  const generated = `## Generated asset manifest\n\nGenerated from the approved ranges. SHA-256 values are recorded in hexadecimal.\n\n### Source file hashes\n\n| Source file | SHA-256 |\n|---|---|\n${hashTable}\n\n### Extract and image validation\n\n| ID | Stage | Source file | Range | Extracted filename | Rendered image filename | Teaching purpose | Highlighted source lines | Validation | Excerpt SHA-256 |\n|---|---|---|---:|---|---|---|---|---|---|\n${resultTable}\n\n### Final verification\n\n- Expected extracts: ${results.length}; present and validated: ${results.length}.\n- Expected PNG images: ${results.length}; present and validated: ${results.length}.\n- Every image is 2560×1440 pixels.\n- Every rendered row was compared with its source line after removing presentation markup.\n- No image reported horizontal or vertical clipping.\n- I4 and R2 have split alternatives (I4a/I4b and R2a/R2b) for maximum classroom readability; the broad versions are also included.\n`;
  const originalIndex = fs.readFileSync(indexPath, 'utf8');
  const marker = '## Generated asset manifest';
  const baseIndex = originalIndex.includes(marker) ? originalIndex.slice(0, originalIndex.indexOf(marker)).trimEnd() + '\n\n' : originalIndex.trimEnd() + '\n\n';
  fs.writeFileSync(indexPath, baseIndex + generated, 'utf8');
  fs.writeFileSync(path.join(__dirname, 'generation_report.json'), JSON.stringify({ generatedAt: new Date().toISOString(), executablePath, count: results.length, results }, null, 2) + '\n', 'utf8');
  process.stdout.write(`COMPLETE: ${results.length} extracts and ${results.length} images\n`);
}

main().catch(error => { console.error(error.stack || error); process.exitCode = 1; });

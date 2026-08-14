# neyrd

Low-latency screen streaming over raw TCP — built out of frustration with sluggish macOS → Linux RDP performance.

*neyrd* loosely translates to "Not Yet Another Remote Desktop". You can call it _nerd_ though.

## Usage

```
neyrd --receiver-ip <receiver-ip-address> --adapter <X11|ScreenCaptureKit>
```

## How it works

neyrd has two components:

- **Emitter** — runs on the host machine. Captures frames using native platform APIs, encodes them, and streams over TCP port 22222.
- **Receiver** — runs on the client machine. Receives, decodes, and renders frames in real time.

The connection is bi-directional, enabling input events (pointer, keyboard) to be sent back to the host.

## Requirements

- TCP port 22222 open on both machines
- A reasonably modern CPU — encoding/decoding 30 FPS is compute-intensive and performance will degrade under heavy load

## Platform support

| Role | Platform | Capture backend |
|---|---|---|
| Emitter | macOS | ScreenCaptureKit |
| Emitter | Linux | X11 |
| Receiver | macOS / Linux | Avalonia UI |

## Target performance

- **30 FPS** stable stream
- **~20–50ms** end-to-end latency on LAN

| Stage | Budget |
|---|---|
| Capture → encode | ~5–10ms |
| Network transit | ~5–20ms |
| Decode → display | ~5–10ms |

## What neyrd is not

Not an RDP client. No RDP protocol involved — neyrd uses its own framing over a plain TCP socket.
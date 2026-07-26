import { Link } from "@tanstack/react-router";
import { Github, Twitter, Linkedin } from "lucide-react";

export function Footer() {
  return (
    <footer className="border-t border-border bg-card/40">
      <div className="mx-auto max-w-7xl px-4 py-12 sm:px-6">
        <div className="grid gap-8 md:grid-cols-4">
          <div>
            <Link to="/" className="flex items-center gap-2">
              <div className="flex h-8 w-8 items-center justify-center rounded-lg bg-primary text-primary-foreground font-bold">W</div>
              <span className="text-lg font-bold">Workly</span>
            </Link>
            <p className="mt-3 text-sm text-muted-foreground">The premier marketplace for hiring vetted freelance talent.</p>
          </div>
          <div>
            <h4 className="text-sm font-semibold">Marketplace</h4>
            <ul className="mt-3 space-y-2 text-sm text-muted-foreground">
              <li><Link to="/jobs" className="hover:text-foreground">Browse jobs</Link></li>
              <li><Link to="/register" className="hover:text-foreground">Become a freelancer</Link></li>
              <li><Link to="/register" className="hover:text-foreground">Hire talent</Link></li>
            </ul>
          </div>
          <div>
            <h4 className="text-sm font-semibold">Company</h4>
            <ul className="mt-3 space-y-2 text-sm text-muted-foreground">
              <li><Link to="/about" className="hover:text-foreground">About</Link></li>
              <li><Link to="/contact" className="hover:text-foreground">Contact</Link></li>
              <li><Link to="/faq" className="hover:text-foreground">FAQ</Link></li>
            </ul>
          </div>
          <div>
            <h4 className="text-sm font-semibold">Legal</h4>
            <ul className="mt-3 space-y-2 text-sm text-muted-foreground">
              <li><Link to="/privacy" className="hover:text-foreground">Privacy</Link></li>
              <li><Link to="/terms" className="hover:text-foreground">Terms</Link></li>
            </ul>
          </div>
        </div>
        <div className="mt-8 flex flex-col items-center justify-between gap-4 border-t border-border pt-6 sm:flex-row">
          <p className="text-xs text-muted-foreground">© {new Date().getFullYear()} Workly. All rights reserved.</p>
          <div className="flex gap-3 text-muted-foreground">
            <a href="#" aria-label="Twitter" className="hover:text-foreground"><Twitter size={16} /></a>
            <a href="#" aria-label="GitHub" className="hover:text-foreground"><Github size={16} /></a>
            <a href="#" aria-label="LinkedIn" className="hover:text-foreground"><Linkedin size={16} /></a>
          </div>
        </div>
      </div>
    </footer>
  );
}

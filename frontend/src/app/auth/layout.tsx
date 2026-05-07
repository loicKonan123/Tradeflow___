export default function AuthLayout({ children }: { children: React.ReactNode }) {
  return (
    <div className="trading-bg min-h-screen flex flex-col pt-14">
      <div className="flex-1 flex items-center justify-center px-8 py-16">
        {children}
      </div>
    </div>
  );
}

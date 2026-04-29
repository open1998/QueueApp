namespace QueueApp;

partial class Form1
{
    private System.ComponentModel.IContainer components = null;
    private Microsoft.AspNetCore.Components.WebView.WindowsForms.BlazorWebView blazorWebView1;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.blazorWebView1 = new Microsoft.AspNetCore.Components.WebView.WindowsForms.BlazorWebView();
        this.SuspendLayout();
        // 
        // blazorWebView1
        // 
        this.blazorWebView1.Dock = System.Windows.Forms.DockStyle.Fill;
        this.blazorWebView1.Location = new System.Drawing.Point(0, 0);
        this.blazorWebView1.Name = "blazorWebView1";
        this.blazorWebView1.Size = new System.Drawing.Size(800, 450);
        this.blazorWebView1.TabIndex = 0;
        this.blazorWebView1.Text = "blazorWebView1";
        // 
        // Form1
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(1000, 700);
        this.Controls.Add(this.blazorWebView1);
        this.Name = "Form1";
        this.Text = "Queue Management System - Modern UI";
        this.ResumeLayout(false);
    }
}

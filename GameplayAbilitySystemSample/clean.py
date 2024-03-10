import os
import stat
import glob

def rmdir(top):
    os.chdir("./") # 切换到目录
    if os.path.exists(top): 
        for root, dirs, files in os.walk(top, topdown=False):
            for name in files:
                filename = os.path.join(root, name)
                os.chmod(filename, stat.S_IWUSR)
                os.remove(filename)
            for name in dirs:
                os.rmdir(os.path.join(root, name))
        print("dir  deleted: " + top)
        os.rmdir(top)
    else:
        print("dir not exists: " + top)

def rmfile(fileExt):
    os.chdir("./") # 切换到目录
    for file in glob.glob("*.%(fileExtension)s" % {'fileExtension' : fileExt}):     
        if not os.access(file, os.W_OK):
            os.chmod(file, stat.S_IWUSR)
        print("file deleted: " + file)
        os.remove(file)


rmdir(".vs")
rmdir("obj")
rmdir("Logs")
rmdir("Temp")

rmfile("csproj")
rmfile("sln")
rmfile("txt")
rmfile("user")
package org.hertsig.voxeltycoon.bigmines

import com.google.gson.GsonBuilder
import com.google.gson.reflect.TypeToken
import java.nio.file.Path
import kotlin.io.path.*

fun main() = Generator.run()

object Generator {
    private const val factor = 20
    private val gson = GsonBuilder()
        .serializeNulls()
        .setPrettyPrinting()
        .create()
    private val base = Path("C:/Program Files (x86)/Steam/steamapps/common/VoxelTycoon/Content/base")
    private val repository = Path("..")
    private val release = Path("C:/Program Files (x86)/Steam/steamapps/common/VoxelTycoon/Content/BigMines")
    private val map = object : TypeToken<Map<String, *>>() {}
    private val uris = mutableMapOf<String, String>()

    fun run() {
        generateMines()
        generateLocalization()
        release()
    }

    private fun release() {
        release.toFile().deleteRecursively()
        release.createDirectories()
        release("localization")
        release("mines")
        release("researches")
        release("mod.json")
        release("preview.png")
    }

    private fun release(name: String, source: Path = repository, target: Path = release) {
        val sourceFile = source.resolve(name)
        val targetFile = target.createDirectories().resolve(name)
        if (sourceFile.isDirectory()) {
            sourceFile.list { release(it.fileName.name, sourceFile, targetFile.createDirectories()) }
        } else if (sourceFile.isRegularFile()) {
            sourceFile.copyTo(targetFile, true)
        }
    }

    private fun generateMines() {
        base.resolve("mines").list { folder ->
            copyMine(folder, repository.resolve("mines/${folder.fileName}").createDirectories())
        }
    }

    private fun generateLocalization() {
        val localizationFolder = repository.resolve("localization").createDirectories()
        base.resolve("localization").list("en.strings.json") {
            transformJsonFile(it, localizationFolder.resolve("en.strings.json")) { data ->
                val resources = uris.entries.associate { (source, target) ->
                    "$target#DisplayName" to "Super ${(data["$source#DisplayName"] as String).lowercase()}"
                }.toMutableMap()
                resources["BigMines/mining_3.research#DisplayName"] = "Mining III"
                resources
            }
        }
    }

    private fun copyMine(source: Path, target: Path) {
        source.list("*.mine") { file ->
            transformJsonFile(file, target.resolve("big_${file.fileName}")) { data ->
                data["OutputInterval"] = data["OutputInterval"] as Double / factor
                data["Spacing"] = 20.0
                data["Price"] = data["Price"] as Double * factor
                data["ResearchUri"] = "BigMines/mining_3.research"
                data
            }
        }
        source.list("*.*storages") { file ->
            transformJsonFile(file, target.resolve("big_${file.fileName}")) { data ->
                val originalUri = data["TargetUri"]
                val uri = (originalUri as String).replace("base/", "BigMines/big_")
                uris[originalUri] = uri
                data["TargetUri"] = uri
                data["MaxWeight"] = data["MaxWeight"] as Double * factor
                data["UnloadingTimeMultiplier"] = 1f / factor
                data
            }
        }
    }

    private fun transformJsonFile(file: Path, target: Path, transform: (MutableMap<String, Any>) -> Map<String, Any>) =
        writeJson(target, transform(readJson(file)))

    private fun writeJson(target: Path, data: Any) =
        target.bufferedWriter().use { writer -> gson.toJson(data, writer) }

    private fun readJson(file: Path): MutableMap<String, Any> =
        file.bufferedReader().use { r -> gson.fromJson(r, map.type) }

    private fun Path.list(glob: String = "*", action: (Path) -> Unit) = listDirectoryEntries(glob).forEach(action)
}

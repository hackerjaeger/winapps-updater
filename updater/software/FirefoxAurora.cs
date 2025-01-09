﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017 - 2025  Dirk Stolle

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using updater.data;
using updater.versions;

namespace updater.software
{
    /// <summary>
    /// Firefox Developer Edition (i.e. aurora channel)
    /// </summary>
    public class FirefoxAurora : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for FirefoxAurora class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(FirefoxAurora).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox Aurora
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=San Francisco, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2027, 6, 18, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "135.0b2";


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox Developer Edition software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public FirefoxAurora(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = knownChecksums32Bit()[langCode];
            checksum64Bit = knownChecksums64Bit()[langCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/135.0b2/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "129f81731e1b097d1683af9c1a5870a1ce18394a99de3380d54233c80ca8c90b6ece0d51c3e72077d51bec428342a3ea8198e2417dae4e09c3a3c0805c8d3dde" },
                { "af", "08d23b2fd69e027f350d1121d54517b0a7a44fe4d6a3088f72e323afee993ed531c0137efce6c83493d421b25b69b7f06f4db8ad487ffbbbd330cfbbfc0926ed" },
                { "an", "f2cc3f839fbffb2547a6f3deade4b7c16585a86c2d80ca894ebe8ec6359f0fc597828ee0f089e3d8cc118c4fa636dc9b0dd120070e37a55ef58d5761c0f52d03" },
                { "ar", "5af3b0e9065f90b92ba15a114a2f7a04bdab1ee6a60edff1eb1bdbb8cdc897704989219f6258df616aaa6967644b75d303d07079d7c2c787902121e801ae78ea" },
                { "ast", "1f78eeb5adb919b4e6fe88b42e022466cfa8711251da7b813b63d9d7a919c569e2c007e559c6f4c9b3e43e5ff4e708e160c428b9a273b9b2a3eb899555d447e6" },
                { "az", "2f6b9e5ec4013af480a126b2ec836c67982e9b5852cb240759d162fe1e39d956d73141c934164bb5d900b5dc7f6d0adc1c2c03a6447d79f51b5e2cc29046485e" },
                { "be", "48d9f51f8dce9b64848226739ebbc007045b1f94b6573c7cd5af1d247cf791b180b6061db23e0f61963d5f1b171b86ecaa0e0b331911c1734163b21445ea56bd" },
                { "bg", "e195b02a6220b4cdeaf7eca9397e1e899be46bd66dcd065a3e5f9fb2296e9408a6810acc4e7a6f4ebda2048ea952edc62a8f7525da679bb29b1c43ed415680ac" },
                { "bn", "0e33ddf70cf8d354d1c289bb58253cc513d5d8e2d53ed56a4a576866c5ba8b9b4f698adb21c38ac6f1742674f911b3193e4dac27bb7381edc03b914ed186d9ff" },
                { "br", "ae2973546907f741a43ba993ad47372742eabc4e1206615c4b1315f5db1cb6b67be36d7e8eabe17ae82182db2806f9b227ddd7fc93e30a30ab9d0c9c92d039a9" },
                { "bs", "e69890c9c11e56688bd5a338491fbaf10ee0e4d78980af5e4b2dd7cc2a0981ab35cb49de9d0a164d8c6b7b1303e5a0777792cb4f8efae89923b522511d3a4906" },
                { "ca", "ad9247ebe95f961b09156e26d11129736d98a791499cb57a101118a69e5e1beadc5221946de9b28c6ca5eb33b4af0ede1eb46b472a8fea287ec3b44171906325" },
                { "cak", "3d2542599580a536301f3d95f50b6d55cdad981ad7d3055ab157cdf078782c3b40b17f6da011504d2026cfe623a49ff47c59e5dd919733a90daf73bec5c2508b" },
                { "cs", "930650b3b0b01d5da538604fb06a396633b84b0ed2b18be435cac43d1861b849845fdd87d834ddf4e2bdcac9affef3834f88a0a3e2531791b41611ba6e905842" },
                { "cy", "6b5285fd34b032c742f7a22ebb114c5997f224148659c3929901b3d568f0ff49148cf151ba6f9b05b750e3547dc5d1124c5d756944b183dac7baedfd0e0cc964" },
                { "da", "d15a31e5ec2bcc511becf612eb492c7cb66310316b034afcc1aac20ff36ad6d9223bc18eaea0d764cdbc00ac51fa84e0696dc0198d6a9f423b48d0598ca7466f" },
                { "de", "950c53a26a7b1ef1bb09b8efa1b9608229d2a8f03d729797a208cc63638a3e2d520f90c11da8da2ef0d7a1b1d57ea1b442e86f62e7ad66c7cd7fde9a25b7994c" },
                { "dsb", "e2c0b28d47ade2c6e1c6e4261dca562fbc697f073db65614a383086d7f701358a4650d931af506b80da0371a162dd7f7b8d540d62a670920b7f794f9d69d2c33" },
                { "el", "51b5326a2643e0f7276038efa6e4f5464221ff16380103b5976c7dd35604c5fafca4a680559c71e35e697f6049218b548d357e81fe47064a3071a5e84f9e8b39" },
                { "en-CA", "2d54a710a742027047fd6bb7dc6e466de3b71dbc4a454af5f6472a7fa5ce637aaa5ba0c972bb5c8e25c5942a8f3454313aa512a1ec65471fde839b1fa60d1aae" },
                { "en-GB", "3c39487bda16330289d0c653fe75426b962a5e3cd25aa8584e0693ffbab01fe151c9b7f1f72ff08e754743cdf3fc804c05c617a90c23281df66c7e56d11b74df" },
                { "en-US", "c3b34ca8f3c3eeda9705933d30dbe82d978f68ea4375478141804169b656778749b0297a71453b447ffa237180730c76e6a8306b97857c74704888006e217cb8" },
                { "eo", "c4d5d284fcb4e50dd21b393ca4cde3b764698b0a07958535f544cd1a1ccda14d4038a9c26abc7f07e639da7524a18368e7e37817a314f7a6fbf51b7e07b20a74" },
                { "es-AR", "947bf93b8186eda1370716a01f588a12b87425b5515922f2b3a547fe48e3bd95b00a854af21a91c8fc7bc1d4b2a465898f9ab5455cb96480ab9eab2aaae690a0" },
                { "es-CL", "5dd09b28a050fda3b1b95d96f240122f7e723129c3813e06844818b5a1bff368b02d96072b7c6d3f2943d1a41203121163f287c09c723af26165b726f96c1206" },
                { "es-ES", "8e45a5b41f0cc9485a05599c5549122920e415f3f97aa4e2dcb7c9116f73c3c86dd45db3338855b1a5e081ffed065084cbefd17a4ef1f987675826cfbc0176b1" },
                { "es-MX", "6deab31e273d44a2f1e8b671dd053bb9507ea56f38d779a82be8f68ade152133ee0cc3890d4cd820faba78d06afec10603dd30f59a0629ec11f2ee386df29b2d" },
                { "et", "620d65ea55372d69386a5c99f546cc110af5f2ca74d3d7848eebe2a8aa636e710cd44c0c612210d5c13fbe1f6ffc6d9a7f2241e93777ba1bad1a99956b754a22" },
                { "eu", "6677bf87e6c509a369e67b4298f4ce4bfbdd86b6d4c858089caf4c05dfc8d31ff020b3bc8fa71f692c1c146dedcef274737eb111a17a380c87d763d77d8c5053" },
                { "fa", "5b3d135c55179eb71f7934c144f44835e0d8599f61866378f9d791adb44c35cb700332f406282f34fdd289c251180dceba80596a0b1374a47c211673611c2128" },
                { "ff", "ad8c671bcd8ab1d3b09479b17fcba3f77d02330f828911bfc28ef78628484a545415a10a21eb993427cfd1ea40356193250c4ccc2e8338810b6e2c29c0fd9848" },
                { "fi", "64f799971a78d61073c9b526cfbf2d93b4434339c81cdd5ff284b7992c28f63d0b729b46a03c84aa9cabebc1f5cf39b6c88f7700edffe214d757a18ace5caa5c" },
                { "fr", "fe5cbcaf3d10130ce26e71d3a04bb54a38a6e3432df28434f588e2e8c5f816ff02ea2fe6e15bc66d812fa3b13dc5c0786b5ed36bc8cc484be8ce511f307e8d14" },
                { "fur", "4bc2666dc03f205cd8f74c12e7e53da46b7972b2624ec2daaa19b44485327316b309f3d8725291b0246d5f4e2f5c9a990edb4947db379901a800c4c0e6630ddc" },
                { "fy-NL", "8439fdd1a5c56a22161ff7929f0f40a197f13460e9cbe6e5f82b98f24e5d1c32cbf73785336cd4dc0ff266941d6e8747c4566b54256655b00a4b9fb49f57c7c1" },
                { "ga-IE", "a74e7a96d424265618c904e7a9926df2bb5baf0af59873b9c97f3d87760a2b1b6e044e899eadeccf6c33aae568f4e474a5125e64e7f96fe8fc9df5bb5ed44e5a" },
                { "gd", "9da4a338e0b54541f144bf802079af14260cc74d378879ab7e566cf89a48275750e50a6c4b6022527782a45599fda8f7b1ae09bfb2479b9065299cbdb2e16bce" },
                { "gl", "e1cd029d58e8cf0429aa6e26c52159734fedcefe42d362bfc0fb55609c7ad9725100eccb69596c78052526d6b5a8cc97aff129a058a422ab65e08ff14a14040c" },
                { "gn", "7b628aec0711e29e484032ff2d2c1788dd2c483b51ce8a67bbcc26bed828922ee045ddc621ea4e9c44dbeefb6e8c2369e9645aa38c6d0036d4fb2a46027d02cd" },
                { "gu-IN", "60a9926590fdd9ebea8f6228f3bbfd472bb5dcfa05d8ffe080d0cb788274c40e7281d99e4d74d69bc2000049255930c8bb5eefb37fb5de6791340206d76fc41f" },
                { "he", "cd7f0012add478e51ec80b6e73197e612930c2c16028e4b9f816dcb0efc70ea061a037ec0c378c31dd4a54608794a00bc9fd0a52c10f9ec37ee30ad1c214c175" },
                { "hi-IN", "01f06a1b9097e54d637b9e620601a40710d659851064f27fa403f566930604f7d3905135293073832e3461435e7d3e955dc7c6adc19584aeed4f12784e2818bd" },
                { "hr", "a35c3f3c843ba6b3134b46d596fb6327c95d1ecd1cbb0643ce49e52a47ee88e363882bb22b67b7f775fa4ffb617feaea70488eeac2770615c8f47ead0cfdc49d" },
                { "hsb", "a42a26747c403f7c559596726406e81f9381f17a879c43b7db63b495586c8a6fb9f5298c1886c04da1ea4a15cac2d1fab419fb784d9170bb36785a594c87611b" },
                { "hu", "8a5ee0c751fd69de7cf5a5a62c9d40ceedefc101c65b07667fbbc9bbb20d54a3089f33ea785cf2e76f2192b3d98a41eefe1ca5049d225d510adeef750c8a6f81" },
                { "hy-AM", "fa0769e883b651c2053d0e8de17263bda0a712a6c5c09de1c2ed08807925a47a7471a225db4165efb597e08ce07a065be2a8401a1bfe088306af07ad4be0fdc7" },
                { "ia", "f7a019f32052dfe0351b829f1d33186546d25af523c3426846b33a339ffe26ee3325d8c76dadfcabdf2dbfd6b09fdcb0bb02d8908747fdda3b786f610ef84333" },
                { "id", "f30d7ab389d4095105a4b484792be20f2a432eb78ac2e8740461ce0dd1412560a79b373fde756743b1b367752ec1fe3dc62e9b3db5a68708df563dfaa8ecc55b" },
                { "is", "04c20faacb31c5fa962e42ecfe29cdf9fabc9bfc53aa31b67637d673edf57ac135bb3aafa7d5af5202a46813519bffb41f8304482072fa98530af3082e87740c" },
                { "it", "629103faba1dcca35ef00e7c053b7c989b3a086ffba07f5e4017a3f8846cd83d3508bd1f12e5c47b25d23d5d5fe4e88f3c083f81805122061ca795ff0ba019bd" },
                { "ja", "778967fd06bff9ccc24e09d9562d3a463395e4074ea761643af90d20c17c340843c2806405a3a9380ef0b10577a8b728145a5e02703a2a8068de0ee7f7a497b5" },
                { "ka", "efad8d269d1e6c3099b6d404daaeeb8a2290d80d327fcb1c14aea512cea1b1624d06ee23459798e0c9643f0774dfc559bfd7615b90de86294ed332efce0b5978" },
                { "kab", "9cc9cb89f63abea354cdf165a89f700c15b403fa96f572c48dddf7a5bfb49d4c5c473c5779015175cd11d3eaba9246258db4541f0f6ce11ca88d08f6d04b99e6" },
                { "kk", "456458f210ca710060fa492bae1b18e468826b4246396beb2f5ed20f947d886b6fa7ea16f19aa8d7bdcc3b62805b9cdf6e65fee860a5be9a06c2bd175d851144" },
                { "km", "7122df16dee108853e165bfcdacd59696128aade019149d0d886dfda086630beb29a2aa68f7e6d6529e8170ae4b888411a2db7822076cb985a7681e13fe187a2" },
                { "kn", "03ed486e22dd6d1281a804771cf29047319fc8e260b61b6bc1ee471e5a5dd15a0036392a0f23f9718396b82de55bbebaaf1427def977832e761283dc72950e8c" },
                { "ko", "568672254f41071680a468fc7c8dbd194296ae8b4039c746593ff77cb32c55ad078ab03ac4258c0487c2312437c9a84fe8af72e82bece5fb6513b21781b3691d" },
                { "lij", "b398aadc484ac437f77bc57b9d3ea83381de0ec2512cb9c3f75e65ef8cdf7d17a0aa245551d089a8d2f2b3426dbdc796bc9a5dc0b8806a40e2d2630932ae0176" },
                { "lt", "3cf5d3e04dd73344a30e1c1beff7169a415b7ff773d1642a9bf58de9a6cac31dcc3def6572c9597067e9cc83da64a4d0bedd5e98dd6bfef5b5f29503c89c48fb" },
                { "lv", "2986be1f921c876942fdeb34cc5365e9bc39effebe4e034a89009f212ba22b9afbe2751274e9e975a0d57696d88f72a4be0bd8afae04afbf559522132a2b5713" },
                { "mk", "a9eab37aec89784119d0cc398d1790dde8ea53206a7f686d55813c770f1405f40bd16f2f62d9ba82c9ca5f8e53a81d3fdea592fa125eb42d766834a1b8fbbf00" },
                { "mr", "e030884bbded4523831f7c2e536378fdbdc8019357b1d0870d539b43c26e954ed6cc445937e7560ccd4ba52e30f397734ba118e10016fbb3a600535707c36e7a" },
                { "ms", "474e70dfa425b1c8b58bce2e26554d44c5cbdc47f102743c99973f419d51ad9b31636d456a7cac4bf06f2bcbde0ca022036bacde2519c7ca9541237bf352b701" },
                { "my", "b48513aea3490870319bde1f817677441740d84cfc2086b11116f2afb41dde427e33becd5b94ce0323ee75a2ce9c63847dbad2b7408330d0de3cea7e1901cd5a" },
                { "nb-NO", "ec5f932bcb3b051a6065126327252475aa94de7747de76a93aaaa4f4ad9e0d9f3d4bf27e901b547bc38ce972043052539f28ed7b5e096f2eddbfd13adf66fca6" },
                { "ne-NP", "f7ff03f6b472f29d961a8ac3e88af2c7c5c5dd733d58998a7b9c1094d863b305ee31516dc7a7298109e78c66650636ae619d6439aa2a0783eb0e405954fb3185" },
                { "nl", "1f83fc210ef7c84249ae897f62984aa0b21037ee3207ce157db7c9e909438fc0c6b0f5b6ddd93546204c2176b92330776479bf49eeb42eeca42ada012752055e" },
                { "nn-NO", "7fc5ead170309d0c780802618befcd7c52f588db51a70d187ef0425bec4f9e2c008254a135ca43144ad96a9d2bef72d1c0d3fca6da2150d89c415400f3fc3577" },
                { "oc", "d1c43fec78a7fefb3d9edd8425ef2dbc0e47b2366263529fddb2a7d4fce9afc093053e8c34088570211191f0a6d379f10a2b2f4ccf190c846f9b98d2edc9d5c0" },
                { "pa-IN", "9d88650a75ffb5e0257abac9bb1bbb3d1ac0bbf9dbf7bea113958a7dd7864458965f667a62ce22af810400e5ba55d1450099248ff31ceb98ba3622c2aca6d213" },
                { "pl", "29f4db8fa8cbaa9fb411a8cc498bbdb2ad46ab9b4c3dd3f416bea0e14cd0d9184aa4c8f97638d0bf3e91e256fdff97b307b06a9bac12623d55a228145c884728" },
                { "pt-BR", "bcfd595625ec82364a3b53f4ee4fb2ddb2aa3f934ae5ba6f09e3d05afaeb3c1169fdb20a31c6c969a7dcb555b12070ec25cf7d69c3f9a3b793942e646bdfde15" },
                { "pt-PT", "18ab86130e3f17fea20a61d876a5c70c546ceaecf0013d36954851fbaa89613821413e5faa0508104050525589c6f540890db87cbeba2ff0677f52b29a178973" },
                { "rm", "7b29347382b4ea570a258992657f3ed94bc09f56a4df73503f54c2f45594f13112ce7e7e42e1cea007eeb08755241e80692c8babd970f5ec86b4bf54957f0164" },
                { "ro", "1ac36a8249dfba694ef3f6aa5ef69fbbf9d0f476f696c297e61627a041e275696543e225aa92da39b6ab58c5ab8e24514bdfa083b688675cff6e8797dceec8fb" },
                { "ru", "3dfd0319f29990aceaad0f5f78979ed6b6c5d6ac986cf99981221970bed5858028b9f3f636de443fc3a1ca7518278af1146c75edf1a367b90e602b5f508367b6" },
                { "sat", "baa8cdb74f1280a09b4d9f147fa699d71652dbc39b2866c8bf000663621ac4205c048e29f854485bf4404b5e57165db7d8d2ed81294ecfb87b35d4653f8cf2f6" },
                { "sc", "f3e1ecbe2df69630a47405769b973ab3be417c01e1512d47187e02ea162d11416080ba9f26517ca979323b7167a57f17c0afcf9b84bbcbd404ae0f8b7be551f9" },
                { "sco", "327fa5b8c4db18be33bb8179dfe628420d6a7d784c2b298d3b62883531384d2d5af85879f12d2a60d9e501de788858f52eaff0309b4131abd0ffd19e66d80cbb" },
                { "si", "c27e047f76e5e7d5630977be9a4063db01029cd708668803e98e0dce3e61de314512d6d40a57d38604b12672505a6b7073366289f65174266e7671e94c5dba02" },
                { "sk", "fd2e7f4a8037bbcb1e309227e18a6331f05d499a01c9c69c3f6242237ebaedba36f3cef1a646aaeb6f29230c88eb3f908b23eb1485bbfa6ec3d5187d5abae7ac" },
                { "skr", "34a7547da9d062a0bdaa9893fa2ee18285a890b9e2f474da1f78ba251b5630a5704080cc6e2e02f2fd258de10f40fe805522fcde59129d53c3806de340bee65f" },
                { "sl", "5f1441456714e7d1a103ecfee0bc39a0f3a9d7733b18c8f30778d1cd245756b9e79048b7567d7c23e5ab62a61a49f49dfe2ab35a082f99d98236dad74de9547d" },
                { "son", "9c633b165594b2acd7aae23fea994fb7bee9ba5e0ffa50571482be26433a1f9efe6c200837621f08208fc4b5e27cbba468fb220c4c59c3448f89a5ae70386c0d" },
                { "sq", "0e8b01842552ca0919206e6c2fa0a7b04e0fdefc41ce8a359fb79ff8ce904ef88dd6b39a2b04041fa51f72fe6b543ac1f37e5c8c203d5330db67cfcfef23601e" },
                { "sr", "086f156b44a11e6546e886891b80cd5453ccefee4f17f30310187f252c5688fc6996255528461a4ba0d4d9fb8276a638a71f1626aa7adabeea27d6549250cf30" },
                { "sv-SE", "21020480aa763e9341eee5294ffbccfeca575dcbbaee22a778abb6d9b686388e9287bd0fac6e467a2082fdc4cd8c5346738a167b16ec9a694faf049b490f1bad" },
                { "szl", "77e769476f553c02ad8ed05e18cf356df964f7895941dd2ed38a3bf67f5606830ea9006c5590c2b1934ba1c4acdda1bc2b04be3c9a50b8fd0b3b7dd1603f0ec2" },
                { "ta", "f767daca434184f9a02ae3361bb44cd6b980329d829d6228136005c211a38f74365dfe22b4389838e624d1beba6bbd91bba19345cd81816396e3aa1fa5692afe" },
                { "te", "02f9891a3c90bf7332f9d80407703af8aaaa898a14b9790da81439d11b987ba878ae67fa856e1b237bf9625c512151b83eaf40c9842ad9b3986d23155010b48a" },
                { "tg", "443711e95ee902420528ee34509be8f32ac5903bc76e9541ea40d3aee638695ecb0a6e781e8e7ca4792705bd451f8ac17a78d0dce4fd2175814539a1b2d0421c" },
                { "th", "d844bcb6d37bf9d8a0c70d0c1134d7a11244734d8be7dcb40e5dd05b4ce23e3b0b16ed98ab35b0b49e75a67650eb84cff0c7f594514246708999e8dc6d4c3e6d" },
                { "tl", "07012af2acf77ff49476c76161ccfde9b56a87073c605e65e9e10a1daa7bc465ff4350a98e928daf9e1b574aac4b0938acec68c47a8e3d9c7f1864d0751cc4c8" },
                { "tr", "5179e0df81880e719c31433e903d07b5535899b2a6e6165c595d5749353949adcced0c46cee2f8762f818b1a4b101ae4617a6d27d44cfb68015cba7635ff0d4c" },
                { "trs", "79c6d7f665b3c02b9bb4af52594d4b91d2a5f9166d7ec35fcd71ee6044e666889bf2ae874998b9109d26c0872fdb2e4f84c200c358451237e5d5f054b2a0e0c3" },
                { "uk", "69e16aefc7ed7d8f52e6ecd6d6d0eac0a1444407f997e24339044eb6b365dd0d237719aa2a4129d6edff97a93075c9596f46f40c7266123f05c17d06f3c3f92c" },
                { "ur", "d5f8e78e553edf0333cb533fb56604d8262a7aa97625b2a32ce8ed844467b02452a9f5965887ec99b43c0c09de4db07270f1445c54cdd35b2c86aa627a2a5644" },
                { "uz", "4750f95fb5527ceb61a598d9139565d3810895a14c792ecaccf873371475ee9ed51acc9b7c5b1e3f502b76ea59a3b3ccde36115c265ec1e825440c1106e43e19" },
                { "vi", "8a29f6a59874b867eec9b0ec7d657d16a7ece6ab622c6cbdc3ceff68c4c9dcebc27b30b95545ac25432497794d1cab7b66a66e45391d10ea2b656490da9edff6" },
                { "xh", "e4dc87183d6fe0f276e330f3bb139eba0ceed0f3ad9d2deed4600cad358b3023a53140c34d50d37da953b67ba70bcc2054c3b70f15fabbc73d01b9610b4e4ff0" },
                { "zh-CN", "90c6b3e4a9422ee70cf2b32ecc04d81cb1705b445c862dd04b8305318703781edcd8089721e322d181d14bdf22b362150586a5f2b76e27bfd244e93129fa3ccb" },
                { "zh-TW", "0f8a4d6fdd37f99f13d70da4102c81a17f33087493e524bf6831f6025274976a5cc17bd033cbb916a9f1b98bbf354206e8ace51e615f6cd317fd9db88413865f" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/135.0b2/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "d8caab9be9ccfe862915c3f2d4b4ffa81916c06360c7b34d0269348cbebce5d2cb360422a482074ad7d374b4d7425478d3da77ac427cdee102bc09cffec948f4" },
                { "af", "9e8f41c75a697103a74a5c76c95711e4f7b7ce0335169f936e50b4ca1e2906313c9e6b402c182143583f2c754c90717a56fbe345dd15ea8e351b6a1f2edbfac5" },
                { "an", "60f479b6b6c59a0b2788332873936dedf37e066a3ca78dc94e8784f5a24f94c9987303f808f1b95254038bf3a561b1decdca260d0ff73fe2e0af37419c172fd1" },
                { "ar", "0405998938d4f46af3338c678e8479ae66edfae1b1010d3c1093a5fcfe508dd6542ea30b1e06433357ed284383c5b78f92feeb635758d8373e7826f8a87ce77e" },
                { "ast", "5cc9ea09d7fd63c515769d085c1373d50fe2a3b50b71585f45f0f86ca523ef8141e1b410991bbe7c3de0f3661b0d1f3fca60aab88872330359de1f118cac6b2a" },
                { "az", "e04aa187b34a3e298a80bbbd6540ad82bbb3febbe22f4559d2e89286bea15a4ee1cf19d0a4224fe0e7c97e96ef358a59be5a9260f13d184e6b455702f3e4c5bc" },
                { "be", "4a2f27ae2815d5cc1a3913b26d1514a335e3b88c6302c672746616cf48100c72510132719551a09f46fed24ff9420b976c04eb471e72c05f278ef72a476007bc" },
                { "bg", "91e7feaa1fb4775e9bd1fe7f76d9a3228e051a6277a9959f1c3f0f5b23031b8c9fca60cab08b592e29588f3736d4987b76d3efef1dafe4c3e8ee7a3696a44345" },
                { "bn", "f2a11391abcbd77b9ea266aca506aad5c8f0eadbacd55e25c5b7d09b0454fc887e396a65bad8bf5b9671149aa409041ae2276961c6db1481fb644cfab5de5f49" },
                { "br", "f49db5a683973ad89041fc66236e5f6e5d9bd2dcd847742b6e5d64d717df1138924c6be69313c045339aaf8185279b46a8dcc01f7bf66f09b4b10e4b5d012871" },
                { "bs", "f927d41452dd7b6782b5295492d8f10dd2152f93173e1985bd04937c386b03b0e4207aeddfee136a4aabd4f8bbca8c974e8f0d5c863758b39c250a6bbc8bb29d" },
                { "ca", "e56a78417fcb585c7e1a4acaa4f387d949859316c84fcfd024298843ee8bde48891ca51fc468c77f555dfd4ccd59e980ddc8c5b21e17bcf735743428dc291bdc" },
                { "cak", "6660da7538b682155eccefaf077f4275f8ce0c31cf252a56ea5f454c408f8664d76a970537ee3896f9040d1e1ee3e8f1e0e294b5b188eb4c59b1f95644dffcde" },
                { "cs", "ccc03d806c1dc534378d3b1cd523c06109ba63fa390c94bdc8ef4ebbbf492f02315350e2881533577fd637498c9e1aede62288c9d5aa7723a11fb2a006f056d4" },
                { "cy", "1d76d247112671f1f34ff4f2b82bbb44be4c32a7dd9bd2e6a686bbf648cb011f9c70bf5e3aabd615d36c49f4923f531a4811e47d26fda0a53d729dd40aa18804" },
                { "da", "6792707670c1498f6c818efd3a34b2ec39ebfa57363f1f02dc3edbfe815aa269a20ac0cb8dea82e728b3ee666884bd600f966a2b266e7c796432ac800da57826" },
                { "de", "2f10ae4190cd8b2afe2687f93b3693a0f7791ea5ed2ab477f29f734f2084b4545cb167ebbe14cc4cb136f67d12197839e21fe15ca54ac646421a7c565d8baae8" },
                { "dsb", "3051a0db7b38a1f2da3edeabb8506f0fa5693d5420ca084b9c6249f9f89362fcffecd390738088aac6a2a7249de96e28b808e722255aa40a1d5b8a3f1b8c74a7" },
                { "el", "dcadeae9c8e23d30010ec9fd6b037939037c0a1f9f83867ec9217e339b1db0531b9e3c49e6cdc424bb77e82a60fe4535ba0e1c92bd5328fe481fa692eb559c83" },
                { "en-CA", "63f406cfc3ebc7bd3d579cf1e885ecef1e57509ff78d83b8e2db2610c4852c26ac1e8d9a11ce98e3067b25015c221f459266329d6f05c8c8e4833847b0b907ed" },
                { "en-GB", "50362d8bea2159227f28b31d211990701daa4992c7d46f2071de5ac13f027f0ea753b34ac976677eee644df8c08b13feb8d54233978b7f2d2202eb65631aa6b8" },
                { "en-US", "67b9c2357d0f3f4381d2ca45acdbbf49dc93ccaee87eae2bd7a58882521d44ec68bc872465c24c333088dcf2ccd93c83042a82ea20f7378cb94f5d961a3beb1d" },
                { "eo", "7b7d492638985a45c40e79f8f137e33adb07384ae98a7042d7c127899280711b1dc57657af3c8b4fb7454db9d4107fb2b342ca0f9c632a980e7e7fa5d3f11685" },
                { "es-AR", "3078a05bc0ba02ed543a7863f5dc612cfca68c81d9fa24e7e207777f57ba40c5921b06f6b4518f5642a8f0350fba75b19ed72d933c5ab2ba78fe092d84df0e78" },
                { "es-CL", "2b5828deaa07defa64462a0808d943202c93b2b40136500cb76d2a61cdacf90ac5085a29e41ea850caaaf3bdf2c554861446f9a07db3f4335c9b113077092eca" },
                { "es-ES", "6646bf881efb0682bdae1c79638444b39978edd9becc8694d1da8bdd1ddfbdca09211eac6dbd88fa0b86da9bc01db91151504fc4bdccac2070c4abc275a8dd43" },
                { "es-MX", "88ee2ee40ac8c14e272fd61e5b6d2e2339a8fa8490e4ee9500d8bc7a8e75440b2b3cb51f30f1fa29235c99e4d5711c0fee1be63de3999c1a0eb013f78025281e" },
                { "et", "9836b98d30bd8617c3741229470aac99e02a1506f5c3683b04eb6d9420089aaccb03e83311c7e085b11fd2827a2d6d0659ea3ac538319df12bc2d4d968b83d64" },
                { "eu", "01fc96e3f7ab60231106aa4d7352f120146f0c90639882ce7fca3e95a75d3789d845c4771c4030b7279dff3c79652d072620631279fac2d287bee14733efc42c" },
                { "fa", "1717e7f12d3af96efdf5d923fb55bc3321ca97617b8d8cb991e800ef67de442636b8330dbaedaefea6f2f9a45780de8bfa6058638b9c635be1f56b53d2bcf931" },
                { "ff", "023acdb12c5e4f43ced021d6ca7473ed9002a4a6aaef38e9ce90ce1264f62aa2ffdcb17a91ddecaa9b526744a90be4a770a3c189c749541571a4f071fd59e24e" },
                { "fi", "237981c5a499cfc9a845c4bdf1a2385652ce2fa0bab2e278c2d0a09c029d871c47e24fcfb88901a829708b3204e2dbf84850004b87615f4efea187c1b0aee912" },
                { "fr", "a1e60f9bb8c4d5d0f2989faf3e558db42cd689c300d0579292b1a537ff659ff021fdfc5e9067768a332ed56846e848442a895778c17eb1425deaadac61b6c7d9" },
                { "fur", "73bd358ae83a0460bfcc4c0e904b8c8efc3cfb335ba91d6601cc7b23184d1ae472c09241365ff7d707084c2915f02102aed4975ba8804535d6d40d2135910af6" },
                { "fy-NL", "8a4d4c1421a13c1d960ef02f85221229a45f3383cec5cf2f25fa076af7354376a125fbd9cd9b760f9c2757bb84e35f75fd2329c92481213f7ad1926d1fdd079e" },
                { "ga-IE", "4963029395efe96e9453b31e905f13b07c9f6fb6f3e50cfc199946a57dfa17da937299e97a0241d260313113aacb199162f9d27d93783704901ae4dfeabf4f95" },
                { "gd", "adb90deabeada549dc18c99c762582b17387a5bd77ee3899afe0eba5937a1d9d88b35c67bfaee82729a4f297e7f1c7089f51edb1f64ea8591b4a040b916a770a" },
                { "gl", "02b335af5057d1fc8e43da8377a82b00e9c43eb56f7e0fb2328383ebc671cd9247dfc66f1563c95376c72b6f78f8dc82854303ebc3fa9bb6812dee9a99ef403a" },
                { "gn", "09294f9d58e9a97de9b02f921dbf4de4872142bef8112b108b864cd7c6e6ee1e077a8d45b923a3df04c3eabd5ed25fe79fae5837c94e31f3e354b5b189e58bce" },
                { "gu-IN", "d980c41b94954ca40004242236016e5537fa69f738e8967c29c05cfb582a1c0039459f9f2a1bd346ef7586db938d542818a7c850406e53121c7459df6285532c" },
                { "he", "23e9973e3bdeb8d0c0564707e14a2ea0d576b6d2fac5f4085e2b14990f35e1c5a68dcbb7594405b3345483c3f3dc2176bc4d551e9904546c7f4b918370b2c26f" },
                { "hi-IN", "6f5d95ed5caedf0c2ab7e3e6062b16063f0a4a208439593d0f3918db6ee8e83ed6630cf0440e7b9210dcabb97521fa9c031c23f67b5a12c25f29528891a03d0f" },
                { "hr", "b75116069c4401142c2ac7db56534b2b37511894432ea4166870df3f4ae4577e690b7f861689c0946d0dd053987cec669edf6bcadf15a1f2c6196e7b0c2b0333" },
                { "hsb", "a14b1cecfef9e38d1fe75bfa1a395ff03f51153fa4a051282de216768e5d00bc3826094b1b895bf7b980fb783b701a0a1e934c8a5378662d428bb42a0839a73e" },
                { "hu", "95e1107e0e164cd67663c2c5c6adbc6851f4697ba1be66d4e80bc8659040bc2e979036862afea912fb0b2f638dd37337db3a1ab68a86b73a4663bd37b00da377" },
                { "hy-AM", "bc3681fd4245b11066694b1c619bb3c1ecdb0c14ac589816802318eafa64d5ac0e4c51f2ee525f37bb81803233a6b08941513f192b8fb8003c9ab6747b749cde" },
                { "ia", "13f222923d1927fba7675ba363bda71b0ca46230c4857d9a0a1b20211d36aa826355a2c8c2d50c9ba227a7ede534e5bbdd45d6216a84bd027f7cbeb58d3e5617" },
                { "id", "a001e784a6bf7eb25e15f5cd1d18131822f59d319f4c9ea9e4032e4a61c2ad7786b814f35bb12e4f0792352ea2c5722866fe0b14f9c1be2e617e109b670a07a1" },
                { "is", "2f39c0167d12064d6426f0569b72b84e229f27c2e3d7334e5b30844a523ff56e416bc37c722e60f7514a55af697a8beaba3c81577ac43a4af80dcf7a3aa8982a" },
                { "it", "d039d006be18d6abcbd03575ab462042f2bb02f432ac39a0813874ebb84cae5396bfd3824dd879075d866841c01e795fe89cf4b2366e8e27667768289137cc80" },
                { "ja", "d669863dfad9728bd3301e344cde35fdd658acc258ca9c03789399be0aecc034180fedb56a57631fcdcac3a93deb354ce6f1d902d09c9e22ccf10de0b242f3da" },
                { "ka", "d3ce8bb4a78ca2b80642ff383986cd0756cf208c2eebef515248e1324d95fb52176ad47ae23ae3f05486479bce1bf91811679f84dedd08ae164bf1494c5c5e62" },
                { "kab", "6dd766633880226f59450199687e5cb109ebcb17b80ac072371463c11993708b8e3c360eaba4227c4a4d0fbdecd957c66cd6b3257c4a7ace2287f1aca7699b78" },
                { "kk", "785130cf4195a470a91d7e10848b4809eee242d665ccae921e14c3ac1f15ffffe1658006f49e5b365321ab002bfc1922750b44fbe62d8ab09086744e8b786002" },
                { "km", "5da48f0fc423da62386e1da525f813b9a305795abc4ec7cb17b9a53c4e11505958a4c4d4f5f2b924e06550652248081221d80d198589fb7d8eae82f877e6103f" },
                { "kn", "a9ef50a2e24d9f6ed3f9f7498720d42dbda37b48212acc565be0786c95924c7b4519690e0257020d0683d59ed021176a9bf0a0d7b357cc3b734eab3248009c34" },
                { "ko", "9e68f2e634b535d17b5f60fcb3137c55569dae417932e5580c3070555fb28d3415272d0eb8304ee32c0f523277b14a0eccea9a9bad11a6c1f0e01e1fdde7a522" },
                { "lij", "39270ae6af348f38cb976ee6677b39ee17fdb01b58f8cb28263a780d6ffaa3fa7628315ae284b6b1918bed0b147532c9b419128881b472d706ab42d37c903a23" },
                { "lt", "f41e28d0d87f9ffb81b7c4dbd0619912df61868f14d2e45053301e0776926566b885c655da6d84b69c23c81f1692b27510dbc040eb45217a1a134992445eaeca" },
                { "lv", "627b9bc666a5527f72633bd46d35c65bea087b8695d52578c2a66426ad92db7af08d760536e6aad315cfc345f6cc48e29072b2e22ca8cbd52e6356467df283fb" },
                { "mk", "e9b8f5fc194eab5a838f76aed53d07056a50e2857f8a30098da53aa70ea82fa18af5ddf44950e9fcaf3aa46a241734c852587e2b28ec417f3fac01f85382b943" },
                { "mr", "6b68102e20aeff8d10ac285c6323c3f38e59cb610e8dba7f23f8b6cf2311d45261f8f89f1aceb8e2326e3b87d2259f9584bfc5fb5be14071955cf3f35e0a0b1c" },
                { "ms", "fdcc33e40b66504127d6c7820e196e162fdcbbc86a5158bdbd541bbc15270e47a966fedafe01c720712da05ec6b00f923d2f944c11537db20712e74ce30eb627" },
                { "my", "30ecdfd16f851dbf7e2fecd52bca291f8a5c1fcb5c527aecdf8c3002587110dc6dde2a5b262598f16490f3380d1d63cae96353bff62781fdc533386aba5b812d" },
                { "nb-NO", "df88a52bc6fd0a352292d68767604abc2dfafbee86b11342dc6f97b1a1823e3c7425ab38dc07ecc8d22c53adffaa15c823b3453f68773ecd42963baeadf1ea2f" },
                { "ne-NP", "283fd2f50efc6b9d641edfd9e2c2cebb72fb43ced6a97c384cead215ec3abf54b75854e60369773f94c70f4b0716f6fe907ebeaccea82c20a871fce176602c97" },
                { "nl", "cc79ad44b6e23ede7c7ba58d347818fcb20f9dcbe63e19920aa2814cc6db086f684fee5d4d6b81e576bc7d5764c920f884f368cbbac176ecc9336738e27dc739" },
                { "nn-NO", "3d99b34dab03089cfcaad9a8aebf4fe4d078819eaf65973a7b6098a8a4086c8f433972b09d37c29fa8b8ef2fef1fc7209211442b5fe56875e57e6753224d2ae7" },
                { "oc", "30726cd205cbdd9f8b70c58eda362efc9aece590c44f04a58eec3e99ace51663ee14dd3a94dd61bf6cbdb3ed6f1fedf1c1e0f5d57676cc3a2c3f9f253a75e776" },
                { "pa-IN", "01c1c39cd814b0d46cc593bb6b9f028277f355ca5809ef756fa55458baec23d3a85de9c24dfecab0aaf96769959319ea91a7c8d78706c8c211f07e0dd576474b" },
                { "pl", "32565ecdf15c0148462b6b84db92127e485baa0f22301442e002fe717c21e097955c1172289d974b1b7894652d0c62172b877dcd335eb873193ec7df65a6cc18" },
                { "pt-BR", "6d1ce5a5b4fea457b51e633ddde958b4e46e4f4fbac425a16deb4a694b42538ad215032e9920425c16af70841d1016e6570c5cd7b2c6cce70303e496d78e8427" },
                { "pt-PT", "27c243b49910ccc61f82d75bee82076b143b9798e62da0ecf62fa2dffa0e9f0d295a7a93b0e3641bb665e052e0dcb5a6548dd8610a0c42b2383880f940874316" },
                { "rm", "21b72607316c543fb7a5dce6fa3c78a9f06eb9b1c38bfe88022c053e21d42d31bc52638da84d98fdac58273136bdff59ad73ff14d0c7af12ad884104c8c7ae8a" },
                { "ro", "f9306011c6c4fcfbb80368fd940fda2910dd10e59467883a23f2701b3d62fc6990c25ec8244034debf3abca4860bba766a776ac953b7047371144d65bee897e5" },
                { "ru", "bb28d11d7b7468326508e6b65bca91be90964396f856c6b5da839d783dc57a98bb0950b55ee56dee378e4c3a7d53a7be991e3f79356847106ec2e676a879d4bd" },
                { "sat", "a55be1ef0418162acedce0784370f67f433930442ab33adf5a4ba35dea0c84249ad70f1fdd2c7d6df7ebbf03d2d0f755149750e346164f3bd154fa5209053d83" },
                { "sc", "5efcddc10ff53ca0cce9b536e9b2722897f18459e2e5df0806e5ca88270923f98dba996fa28a1e6eeedcdb41cd631a5993d4ccc039305b0ace605fba105537d5" },
                { "sco", "8352e21ac63b68844cd06f81c1bc7bf99167316d041f76bbd8463475adf4d6afd9aa96c35734db415f26d7e610f3d0651888554c4268ef92b296edad73615ec2" },
                { "si", "53d45ce3389485ec0da9c74808f83e221b50b9f7d56aa5b9703eb5902ff3c8a4c8a457e53b87a6a043d69a457eb07e4efc8433df30e9c51cddb6217e6480238f" },
                { "sk", "2e0629840527519a5d3bba387d2d94981f03624acd949584c15da8101d58de9665bbff499ba6baad4ac1abea75ba413c5b973e2e560b3a5984d1934149a5cf76" },
                { "skr", "aed3cee343a9d28cf1eb2296d29dbe62676e1ad7f6faa719f37b8f6c3da3a666a0f37b0a23d55e06084b253af5e05f096899df3cddf945c4bf8c575b77f23ed2" },
                { "sl", "5eca75c75113a0960245b192a90033c6253d1d7aa210c6af59242953ed8952bf00ecf2f2c0c9c298389d5b340fb87aaf8a08c9a72199c83ccef9a861383a2ed2" },
                { "son", "9c03f2d084cd3a517dcab6ea86ae8b4d25864519b5ea074486223a69e0872313951f83b3d78024228ac141bc9989a7cd6ed010b4e5e8bcce8a8475653742841a" },
                { "sq", "f8aef14637e42d1f89d30799af7419d405fd82248beee20735d560dd598f455d49b64eeac1a450cb3343e54f642645b8b213b8ee75d079d37c5aa953e3c73a5f" },
                { "sr", "ab0d4ff355cc130d297954880a09c0189d2d53b8609d27521a235a39c2012f942116c3854e0e126b59d382c94704fcaa3861fd1d1bf9db6f5c9bd26d3bb03fb3" },
                { "sv-SE", "79bcc303eac2848a4feee952153cedd19afbdb87305f3b08a166ea809b02bfaedd2dcee32bae3896b303a354dbf62e71f97d1dcb94d9128a24a9ec8c8ebd61f9" },
                { "szl", "d90457397a4113ddc75d761cf3b0abdf981bc386c1438872bf047db54521b74778942041c60926ed2f446c8925dead4cd705119b996c27391660546c5267597e" },
                { "ta", "c23d389c340e6c5ff4738991933f36479f63ee02cd49225144da85bfeda734baa12b227ee5fe3e84b4c11f312a76edf877f6337b1ce1a05fc3ba29c54870631c" },
                { "te", "038bf6e93f2a45b8fd0b22bac2decfb6fde21156cf7c680bd2f7b590042238c6058724955323aeaaba309845925036868700f4e68d6c15bec54f60585d6b367e" },
                { "tg", "7a89605d7809082ce619391cf1580e6169321086265a33a3280425c079e3d7072a990c2ad9b1a3ce229f218920bd09309e2a793b5e092dad5574bc0f83c81ca2" },
                { "th", "3285c4498ae0f63b8e8bece318c91065991507541f4d5f5eb5f9e33a9ad8a87fa22f7aa17ea4cac70fd9bad9cb0489543eb7249d11b88bf5a65192a2516be09f" },
                { "tl", "cf487c971eeedda213450a44f6c814bc52726d36d2546589f79bd018682f26e5ef5b88cbe0b06a3a7a029f8a356487b685e4f6095ea47ce5fe5344a3fe425895" },
                { "tr", "04d33ef58a76717a8adc9bd9e1cb75902acbe99f4bcb3abc43b00b7ea247f911bd410bf6da2dd0a1715a8cbcad8f603affc8c6528c82494426acd131a71edaa1" },
                { "trs", "279f55f31df0600337c67d375c35b387cd31bdcde7a2b04bc13189a74c266c4262fa3fae8d06924f8913a2f8aa2710bb68fd4545ab68319e6e4a82760d51fedb" },
                { "uk", "9ecd05f8c34912a0b29c4d9a3737ffccdbe174330c0523b02dc1b8f4e51ab19b556063971ccb2d0603c18adfaf623dd3389433dc6730240800c8f16f49052aec" },
                { "ur", "1ba9231b2dbd0158756717d6c73707113398fb1d0e613db83ffb5875affe9631fa85663c693bb2d75ee2705d9e38fc580444784c6b36f073b297978f170c1193" },
                { "uz", "01bfbf6a11a296f1b09da838b2bd99e709384babbf906ac118044f89a5e268a153f1bfe1fd27e9a2d89b86e3412ed059b910c000035bfb5f43d02053fdf6d6c7" },
                { "vi", "ea554e0ae2a124b8eea4a6a876674c91b976cb11256061f1e6c037a2951cab62fbc2bc693b1abec61130cba487c8f552416648808bb0aadf76a3b5bcbf7cd923" },
                { "xh", "315dc685d2659b6d24bd8ad3b8894dd571961e01749275650cffb19dc54e62cf0e4d9e5de4a05026347fa70af0ab574bdf1e443324d03820441ceded7c705e95" },
                { "zh-CN", "d78f7040add7415dab48b7e2d17c4ffd9501565fce5b03bfdcaf570270cea40672775b3d4a7fe1710d0405473b0908fafd74a2ea2f8cd9cf808016e0a997bf42" },
                { "zh-TW", "90018a15a247fcd84d23215ec0205a75ec8e4f2ed10853fef59c8a4de9a83fa7ac5bb24ff3b4574401143daaf6f5c9425d6d0116f26aba4383d86d26beee447d" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            return knownChecksums32Bit().Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Firefox Developer Edition (" + languageCode + ")",
                currentVersion,
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32-bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64-bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win64/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma")
                );
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return ["firefox-aurora", "firefox-aurora-" + languageCode.ToLower()];
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox Developer Edition.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public static string determineNewestVersion()
        {
            string url = "https://ftp.mozilla.org/pub/devedition/releases/";

            string htmlContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                htmlContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox Developer Edition version: " + ex.Message);
                return null;
            }

            // HTML source contains something like "<a href="/pub/devedition/releases/54.0b11/">54.0b11/</a>"
            // for every version. We just collect them all and look for the newest version.
            var versions = new List<QuartetAurora>();
            var regEx = new Regex("<a href=\"/pub/devedition/releases/([0-9]+\\.[0-9]+[a-z][0-9]+)/\">([0-9]+\\.[0-9]+[a-z][0-9]+)/</a>");
            MatchCollection matches = regEx.Matches(htmlContent);
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    versions.Add(new QuartetAurora(match.Groups[1].Value));
                }
            } // foreach
            versions.Sort();
            if (versions.Count > 0)
            {
                return versions[^1].full();
            }
            else
                return null;
        }


        /// <summary>
        /// Tries to get the checksums of the newer version.
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32-bit and 64-bit (in that order), if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/devedition/releases/60.0b9/SHA512SUMS
             * Common lines look like
             * "7d2caf5e18....2aa76f2  win64/en-GB/Firefox Setup 60.0b9.exe"
             */

            logger.Debug("Determining newest checksums of Firefox Developer Edition (" + languageCode + ")...");
            string sha512SumsContent;
            if (!string.IsNullOrWhiteSpace(checksumsText) && (newerVersion == currentVersion))
            {
                // Use text from earlier request.
                sha512SumsContent = checksumsText;
            }
            else
            {
                // Get file content from Mozilla server.
                string url = "https://ftp.mozilla.org/pub/devedition/releases/" + newerVersion + "/SHA512SUMS";
                var client = HttpClientProvider.Provide();
                try
                {
                    var task = client.GetStringAsync(url);
                    task.Wait();
                    sha512SumsContent = task.Result;
                    if (newerVersion == currentVersion)
                    {
                        checksumsText = sha512SumsContent;
                    }
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer"
                        + " version of Firefox Developer Edition (" + languageCode + "): " + ex.Message);
                    return null;
                }
            } // else
            if (newerVersion == currentVersion)
            {
                if (cs64 == null || cs32 == null)
                {
                    fillChecksumDictionaries();
                }
                if (cs64 != null && cs32 != null
                    && cs32.TryGetValue(languageCode, out string hash32)
                    && cs64.TryGetValue(languageCode, out string hash64))
                {
                    return [hash32, hash64];
                }
            }
            var sums = new List<string>(2);
            foreach (var bits in new string[] { "32", "64" })
            {
                // look for line with the correct data
                var reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
                    + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
                Match matchChecksum = reChecksum.Match(sha512SumsContent);
                if (!matchChecksum.Success)
                    return null;
                // checksum is the first 128 characters of the match
                sums.Add(matchChecksum.Value[..128]);
            } // foreach
            // return list as array
            return [.. sums];
        }


        /// <summary>
        /// Takes the plain text from the checksum file (if already present) and extracts checksums from that file into a dictionary.
        /// </summary>
        private static void fillChecksumDictionaries()
        {
            if (!string.IsNullOrWhiteSpace(checksumsText))
            {
                if ((null == cs32) || (cs32.Count == 0))
                {
                    // look for lines with language code and version for 32-bit
                    var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = [];
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value[..128]);
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64-bit
                    var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = [];
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value[..128]);
                    }
                }
            }
        }


        /// <summary>
        /// Determines whether the method searchForNewer() is implemented.
        /// </summary>
        /// <returns>Returns true, if searchForNewer() is implemented for that
        /// class. Returns false, if not. Calling searchForNewer() may throw an
        /// exception in the later case.</returns>
        public override bool implementsSearchForNewer()
        {
            return true;
        }


        /// <summary>
        /// Looks for newer versions of the software than the currently known version.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the information
        /// that was retrieved from the net.</returns>
        public override AvailableSoftware searchForNewer()
        {
            logger.Info("Searching for newer version of Firefox Developer Edition (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            // If versions match, we can return the current information.
            var currentInfo = knownInfo();
            if (newerVersion == currentInfo.newestVersion)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if ((null == newerChecksums) || (newerChecksums.Length != 2)
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
                // fallback to known information
                return null;
            // replace all stuff
            string oldVersion = currentInfo.newestVersion;
            currentInfo.newestVersion = newerVersion;
            currentInfo.install32Bit.downloadUrl = currentInfo.install32Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install32Bit.checksum = newerChecksums[0];
            currentInfo.install64Bit.downloadUrl = currentInfo.install64Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install64Bit.checksum = newerChecksums[1];
            return currentInfo;
        }


        /// <summary>
        /// Lists names of processes that might block an update, e.g. because
        /// the application cannot be updated while it is running.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a list of process names that block the upgrade.</returns>
        public override List<string> blockerProcesses(DetectedSoftware detected)
        {
            return [];
        }


        /// <summary>
        /// language code for the Firefox Developer Edition version
        /// </summary>
        private readonly string languageCode;


        /// <summary>
        /// checksum for the 32-bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64-bit installer
        /// </summary>
        private readonly string checksum64Bit;


        /// <summary>
        /// static variable that contains the text from the checksums file
        /// </summary>
        private static string checksumsText = null;

        /// <summary>
        /// dictionary of known checksums for 32-bit versions (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs32 = null;

        /// <summary>
        /// dictionary of known checksums for 64-bit version (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs64 = null;
    } // class
} // namespace

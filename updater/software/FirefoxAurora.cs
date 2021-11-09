﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021  Dirk Stolle

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
using System.Net;
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
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new DateTime(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "95.0b4";

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
                throw new ArgumentNullException("langCode", "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains<string>(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
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
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/95.0b4/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "0774d77e7abcca103f527e0bca1d9bf59ce8b66eb12c566dc506e2aa9a2143c7ada9f318b600e042682ee1f9713eccda0d5555a61d8e0ba90f856dee8b84c4e7" },
                { "af", "3bb2cb0116faeee00b3e305451018616518024915c89b57218d5a3d2013df656cb3ec87210d98722f59137e5627480bf492faaf8653e41011b3ebc4c6775e6aa" },
                { "an", "62b5c87af8ef6f606d103ab72b3b4dafa50e6a5a4f555fa173e911d9ba88099bc1b90a406f1ffc145946c288338ed12e49c328ba51bc99ba93b1bc271d085d5d" },
                { "ar", "0c8724ebc904eed2dee6f027444dc3451d963793ef0f6a651b3ee1ee91f3a349f877a267fdaeccd85c8d24ec414f375b1b65bdf8e4e1aba32379f73555d8ca5c" },
                { "ast", "17aac239b614484a67acd23f56d725391cf5d40452cf80faee21ca8d69dec8606644be0c20bd85c3522e7b5d5b21de675dce8715ab472e20157f1d5846d69ca2" },
                { "az", "1302b428a19e05c51d0dac6c7fc994a5cecc370b2a0f9e68827f0cca95b532ba1fa5ea50c15900f4133aeadd7c15d02815d3cfbe6d96b712af96553732c6047c" },
                { "be", "ae9c4268d78763e2c718da6e6128930ce50052bc1e8745eeff8a4a5ddcea4e9e4826471cbb19ba13bf8a5042b5cafa5d51ec5c6441ec51f0578889a9db0a6be4" },
                { "bg", "4d085d77540909de6c66f1bad91bf7dd969c6cc408812d9873b56239a2f0bb73c6503e207c2ba518c4b067aaaa29f61655547877cb348056fd269c6beb33e53f" },
                { "bn", "97f63c49e3ec6e5e1d86d4ccaea907699da467d493c7db32927a2ddb88bd9b35651f28ea669cfe23c158985a01a000c2d9a5c1c30112908e7f0de423318ebd3c" },
                { "br", "8270dc801e5713421f2fd6a8bee13d1bf3f5beb2ff70c5b631961bac688919c57040b15dd0a6c87a8560bbf18e58e39a2f54ebe0bd530ce0f984df6ae33e2347" },
                { "bs", "106e11cb11a6a14e150177d1e6507803ed8021e7a6252164a53245969f08d641c18962cba423bc72ffce8742bd0093d9106c99f3bc565858845bc54a86fc25d1" },
                { "ca", "e7fb3c80cc9433dda8c738d1789da61c368ce144febde1638635cd5a34b3176f50904ee412ef51fd5d23a17ce00db9516ee85bbe77232bd266cbd3bc0a45f219" },
                { "cak", "1c1af5ebbfbd98a4d2b62e0eb3f6e565959b4f69d74d7c482d5ddee264ed6131c282b2bfbb496c309e866350d359e98735f8d17e22295d91360020574f0a5a69" },
                { "cs", "6413b266c4e833b9bd0f4269763b4091d7ddef5012da41e859d9143952fe51780002ef128744b2f12b3d195114e212041a69288c5c342e7c53d4acb86f2dc927" },
                { "cy", "cf7212528735bf825cde77e74dc2ec7b48c9e11fd589c42641bb3b85f12e6c416392f1edcfce4dca30ccf15523ab97d053243298eb5a4c6d7351cd5bdd748b31" },
                { "da", "76b945736fff9356bf144fafbc18e5e6b9de5d626c0ff6d3df00ea95cf132bf20ae54465750bb0c0176a8c04b3a43715a6cac2dee4cdf380187001f7bef0418c" },
                { "de", "6f99eb15643b7199dcdf8fdb590af58ea0f8d21f02bcce148854316d50ee754ecfb51771d5d742342c1159e9ce819a56f9d0eea261d84716d2c305e2f7e9073c" },
                { "dsb", "9bad40b94d5f05565dee14f24b93b4eca0ed25c30633df90d193f1ae244d61652667715a4ddac44e2ffde19ddd13c4412fa221ff65d8a275e10ee80f9afe2f85" },
                { "el", "54672a0541441466eede8c2bf1451b32682a689d2eef6bb379b8f4a825cbcdebb01862447bcf54d8382c73f59f340ee32803fd947c982b980a08517a504b45e7" },
                { "en-CA", "91ad72b30b54e2de24c4af6e92c4430d8fd5786a57cb5e72ad07f91d0d985725faf9cf8b94bc559e84cb0132cfc709407ab876185ae53d78791ee48674f638b3" },
                { "en-GB", "6b43623a93dca4d4c85e0c9860e6b11ece0b2202542f9128174eb0ff753acf73ec2ec7c03be3c75e9c088dfc8396cebb6f7a38e51f4123b877e575f0ae0615fe" },
                { "en-US", "ef2bf8c6f44d660531f994798579caaa013b338793d69463d27b135f70636dc550832f2d4c9dfb36ef924676bfb109ba62eeda894a2f0a60d044f8819e8e1f56" },
                { "eo", "304b37a29a9d46da9a6759fbc3d1d19365b9bbe3e58cb1dc2f6aa80809e498603f3c6b38cae792ccfa814be161b3adfe92a43a27fd0286a07cce390bb22ca446" },
                { "es-AR", "3a8b8b73a2103a3a3ab538ff4f7bac19c92017a919f0455de360cafa1c1d19c01b47b239891ae1c0fc97e7b6e1b3e2bfc9342b57b07e5d13e9385e32359d4d48" },
                { "es-CL", "5865318e02856b324aa67862442c37bb752489633ddce4873279461f2abf9d0b70407619ac6e009b7fef10fc3d1266b8c52e34ec53757b7380bf5d6d002c7ff9" },
                { "es-ES", "6d591dd308d675912f1339e5c4fbe3101933d9ae7e2f454009a008ea37eb7d5990617b0edc676fbb4442511bdab894fab1d817f765c241fbb6fb34a48b996369" },
                { "es-MX", "6144c8758be31bed479bf5c769bfbbbb8f05cfb518d416d2714fd301732e7e2a34cdeea261744eff724cde05ea64e832696a8f49953bbb295397298f11a809b0" },
                { "et", "8b5421239fde88c43dae90093facb3249155a729b6778409416675a9a3fe3f388f330c17067441fba81862a9031d01a738083ce754b453b7532d2bfbfd935311" },
                { "eu", "39fb60a831c8b5623fbd004be6f2a2d4f57d5af19b991ce2c0600ef93057e9c4065f9ff7d5d2f1193f706dbb9a04e9b16397e8be5259358ae6664834a6ad5e16" },
                { "fa", "03e1a08a2250eb9aa32462912163e5caf0f70f8644b64d44c924a5b620b3d139e300e35df2b393651490ec3e563ac26d5f6d88b4e48255526fe62a55940030a0" },
                { "ff", "93dd76e036a8b8500b4f187a365154e7985891e50b8372fa287853f37021868a7337d000881823dac44b08e6fbae2dec1c924944596237d5d236c05c6592c914" },
                { "fi", "a01a4174406805ccda70e1f1d1980b854950608add2269f6ea371d797d2b3b8772eecbaf442cb10774b000c1c941f96a61f07fbb7d5daa82e130dd7a514d3b30" },
                { "fr", "bbda8a3505e5fe330e52d8f64a04a1a0c360715916c2638f08796dd7d3d6a9f7cee08b75327c27a6ef21521bd0fe1f1edd0f9761539cee801656acefc28f35ae" },
                { "fy-NL", "ee0372c79aa37de9681f169c5445792ea95bc901aa57d3bca1a1cf2d454795010a43ef01aa57f3c76737ef2c0bdab646759ed05efc38d4f8106aeab1467b91ab" },
                { "ga-IE", "18f7ad44106d2a2e065430269b1b3b8afec0996d5c6745b869decdab3d24160aaf12d3be78513ad53114b039ac6d9b31652025c981bde0fe94117f91dee61374" },
                { "gd", "2cc4b076e4cea638458ad26633ee476c44bfdf73234c393ee9daa4233d06dd71d0b497a61b1ef813bdf1a49801049adea6bcc3e57029406463347f9cfc39d578" },
                { "gl", "c4e7d484445e96b683343a6b41aa2afdefa272be8d35720905245798c1f9accb849a88e16ba11a7663a0a46a884c0b06ccc34ba99c6234e15d6ab6e5a715f95e" },
                { "gn", "6cdf39c6875cb48c15155f2533d539847e798cf7c09feff4c7cb617bc3b079ab7f63eab2d7107f43679c9376e1fc0c4600a82382cd660dafdc8af78414b8fef5" },
                { "gu-IN", "b36e9aaa975505ef86cad781e22258cb15eb47192b3951daad1176e7e836de92a0d3639e28c7771e47bea7876102be4a7de801e061fdf6245c36af4a83d551f7" },
                { "he", "904824b67033f9974e4765023d98b4d5d581ff332e497f78773f0752ca922269e0b8d35906c5ca136e1ec81b1e228efc064f671976e18a2883cceb04db7865b1" },
                { "hi-IN", "53b7d0b8529b055067de3ed57138e27d181c1b6aedb8a555466251c7bb878df451ef456f15cbb2fefe2df1ea3737ce56af39c83e77c1c8e536eda491d1921905" },
                { "hr", "d6038b252ca82f0ebe9ee935992ae7592e6fabf799f87d27234af8eae34b1dbeee599ebe21de8d485e798881a8e37a27dccac5c18385f8ddb417367ba90b42cf" },
                { "hsb", "66486c8cb34838f655c3058380a7270ec33f436e5c9dc24feb78a05bb55dca56d7aef82d61ff4eb05bccc55b593b43d33464e1ed32361d26f2768060ee9856ba" },
                { "hu", "71dee71fe81c778c898c09c9ef7eb1b5c2a3e50f9791907ab9c490cf2c6e95b42394217b03a4dd97bd9b2af77b0c9757a2f9418509cf132965be437fd06fbc89" },
                { "hy-AM", "2d1bd76df2e75f2485d81a885602e0f68f7cc3384d0ce18aa89d12cc0b6bc27dd8311cb0f35740b4a4ce442eca4c282850e6f4871e2c0520528e2432e0d7e081" },
                { "ia", "f59081265eb9fd71398daf9b5e07f4520e87ee9e76e66f4abc424ff72b62358a8d3b4cee185d322fed22776413cd74e5c0918a47e21c5f44a241a76b26090ee3" },
                { "id", "ab69916064999d29826995eb18b9fefd8bcaf898ea5bc58e6f2946e30ab056f5a5990b3e5facd78bbc9d13ac2b4b1a55de13445145f04cf4d55cd161109b443a" },
                { "is", "69760b017b002eb79f7599072f97fa2833935444554b084c098b668d9b34ae2e9f1111e75c2d48e6fd3f69f69fbb9d61726ff47068e5cc4b206b01ef03840a51" },
                { "it", "54873526ac446d1d896aa835326ce6cd662dee794160c2aaa550763969dbffb1eed0ee93fac0d3bb01009a46c374621fb5e33fc55dcb12e1b18827199127766c" },
                { "ja", "bd116d50e51575ee7f8061ee7c7a2ea59de6e95432df2d39861aecf0195409032d9a6aefbd84f99d4cdcaf2902c6ec5e2d42a04214de542ecd6e7f29d2a9ed62" },
                { "ka", "fda6564cf7a07663d2eca014d3662fc1f752efdad8d3c10b899410f8889a411deaa00877c3aafd7844263369bcc4f316e9f4fd358d1e3e4c8e57329c42af82ab" },
                { "kab", "c8654b99d5181193089dcfa4a9b70a51348f66b905baee4084668532807819801a9fd2675a5811f61ecb511fbeb6f0f46340896b305f5af4d469da5fbd370a6a" },
                { "kk", "f786994d9aa3ba517e86191cbcab11c1fdbbad9955b48a85b459e10de1511f2a84c886bf714a896df0998f123c150a79fbc4189ff785087049409e2eaa27eb2d" },
                { "km", "11cbde0a1a7b8168b23b5e2e5974ddc8d749dad1736bb9e0811570ad6e951b7cb1d4029feea3c619982b40412489d7fca0b2625d13649be1960e7b02d20451e3" },
                { "kn", "ca7a1365096dd89eae1e4b523143b94cfe56c579b942b55edf050de372032c2d3389fa9187938cee56d0f9fe828667ad119f1a4c308b810283cc1de2f387216b" },
                { "ko", "85c6b960d0f52efa2a7fc9a365b7f2b1b5781030adb50ec9e8822e962966939c6807f24bea294dc6813f3d008e5f2eb580d46784e540aef105dbaab02b23d669" },
                { "lij", "624b07f8c42e78ddff261bff06afb9e6f124996d31954e4d4bc77d1dccad95cc9f86f08596c74f59b92b7fe7292c23a18355010648ce35e548b061af4c342327" },
                { "lt", "af5e63c9f026c66bcca3344e88da9c3063ec02180999799d8c5a6c6032dce5ce952ce17d24c7dba57d38618f8042bb87ea9fcc642deae5070ba7e9d60fff287f" },
                { "lv", "6965962fb763d235d4de2fedb64c5c25441409471acc6aeaebd3926cbde89f361a2ffc7de996305fb9c0edfabb1efd9b11fcce2ce66b53852fdfdbb96f83eab1" },
                { "mk", "25d9935e11da8f29237593bd88eefb141105a9f7b0210922cd0718a7c8436c0ba64f81513b37cd29c68fdcc2d670185891d203797f098835bdc0b08c4febdf60" },
                { "mr", "1b35728c33614243b07b2da5e0411c1169599fa75804efd942978d38179525cde557b56a992497cf00008e54e52697515b7a19a870f769b989cbc4092cba1f6f" },
                { "ms", "87918680134e0ab405f228426380b39da2401760916953be7cab19c8eb2e7ee216ce12a8610a9b09edfd1f7c7cda5395c3b87ffece5ea2b0e50499a9540369bc" },
                { "my", "16fbefc1b0a8355d19bf475f0b6bd1a5bf702e3a69ceb4b898b2a518e94e2da642d4d428b2b090cc8aa55d4e1d2b2e684d46d54154451a60786dd9e837298c96" },
                { "nb-NO", "fdaa4e952939aef0d75cbed5acf5e60af92b484325482956f501439ab9d30347ee8b3d8954b508f83f21db7dc1d94dac2d23be74079e3f92c47fbbb6129a98f8" },
                { "ne-NP", "a5af9c9a7ff821ebe0a6bdd7fdab687f03fb94bf4fafaea965257bf46b56cd3914c11614498cb8a7831e2a7ab278f5728910ebb783b5fbff709854c47f5580c8" },
                { "nl", "fd87491d8a2634d1e6f7d033986978f5b666fd7df2cad6598c12331e1ad9d522ef77c42ff277c1e4a77f0fa26b37babe0e39faa78d6918c55fd9a1b46dfbf6e4" },
                { "nn-NO", "3e2ccb56b2ca9eecbec365248aee6d14e046bbfa216b05ba35ac86f9c05757d19e1323b1d496714449b82266c7a3e7aa52698fbec3548c43be83020c439b75a7" },
                { "oc", "574a4bc1d263c77f4135432255d634cc6122a0699a23549302e10e9731fffbb5806bfb19782077936491723dd9a8cc0729671bcc05d176a7b876d77e6e24aeda" },
                { "pa-IN", "10429e002f24f0e2f3a78f4505718892aefa028fabe8f694d452523141eedb8e1a5981ef3e3e18ac83235086225703c375240d09ee71121139e08a001eafb206" },
                { "pl", "f64159ae737c847c9b5c68461734373323b2a51bf50ce26d30a32bac48be695108bc1d5d8d681fdca2345de006423ed7a0b600968ed92ec7373680333d7b79b1" },
                { "pt-BR", "5cee599d686352ec5bb356f6ced18ddd62c4490f53a86ec83a82fc4958b0729059bd1aea6458e5eb7f4019526b56d4e322a6abe9223963c741e889ea4dbfd060" },
                { "pt-PT", "bc86cb4bc71be59a2c00a7d9c53e814bbfcc4a6f8721a67be75e854c280ac381a2440da9a18082c5c23983a0f74f5bbda2bf3f3a478440e09afdf4199219e558" },
                { "rm", "b9821958353034d8af0f6946d29008d0daa82fdfd67c9d4fdc5167476f69de2b34eae85a98d29aabc61f4a977248f7420830ef95c5c06011a8e73a5eaba602a6" },
                { "ro", "a4f39ef13240d60de59a81ca4c4e0ff4fd8541525478a7fe89b02f55e95b9714bdbce8940fa68b35d76250d218f7b2109a6e5c3342a56f6537f96886c032e38b" },
                { "ru", "7ac1b47b2481e34b5b9aaf9f035ae039d8ebaec05ccce6a426a1caa7a73d22399109fb9b5fbd9e4ae5115d6ee883e739f29f04fa5830a3bba314e72c23b048b3" },
                { "sco", "1ad25139b1ff1ef98d4788fb4956f4c9e88fc7e5085fbd4e9fc0c8a58234628ca3563f18138a3e73ba0de76657246d98969c8f2e8cac627bf292652ce1d3a030" },
                { "si", "00ac7a4fdcf8259c94b145483d0ce1f6f00d42dabd0e21f4767001675d1bc66b5712294d51cac3c5e98de22a6cd81a2d99196652f549db1234dea49f65f64f97" },
                { "sk", "c1224199b48e34b767a1497c81549c9213d0d6b1d9cf24a5c3a1eee0aaee316a96ed2bfe9a2317258b37db90f30f66f1607834c81c8592d78461b522a1726857" },
                { "sl", "3547d20daddb657579f5b0550c84635c790e2b9e92a1b82823630a0e6a439fc0296ab52a83ee60b427e8a348477ec70ed137a1c76744841503a7019c78272af7" },
                { "son", "f10fe033ba6dc4899f86f74964da9636941093f11cd08c3b76565de705f75dca8c53aac5ec89fc1b13b394846e020013a552fadc188c5311bdb8b2382789f271" },
                { "sq", "6e614364955169528a7a588075ac5bf09c07809124ab0d777a1e90a193e3c00afb972a6f08500c88e3290d8bbc9ead0adead04ac500557e2681733e0a3899cb4" },
                { "sr", "441d56e977ee8561479bd620bedf2a83146a19bdf4456df21f86cbea68bd6043b610cd71db50ae7d12fa8b11dee7546ad4ff9bb29edcbf3889d2811c00e1e1c4" },
                { "sv-SE", "4df3354d4df796db8defaef4e74560b7ef28f7c37046be3f5f38fb5d33b47c9a4bec9c744696ac4a3b94992a0c983d4352c15331f7ecec68d84a5bb98e713d57" },
                { "szl", "77ff776f00c8358c90a0a803397937b949bc18e9dca528844071710c2eb1e1c14674308f7b34d7e7e8fa41775c095781b14ba9a18d2dfc13b33f199a8d211bbe" },
                { "ta", "0acdd892cf4c105f7dcedc8b36ab738bd42f71d063c873e6a528d032b3acf898fc267d5af683fddb94d30f55e97a1c136ad9439154f2abde00f447f3ee7a23e3" },
                { "te", "1703dce368db93c948171e473c207109073ed6eae52a7ab2b2c4fa908e50294078369fd45b0a1c94c04e6a7a9f9f9b0e2e0831a7300f53e015e9fc777c91cc38" },
                { "th", "45a1fa1a2b4243cab31c5d771309445bae366fa38eb982ef6e69b33cfbd5b758fa738715c9391ed5070598392517ecc721c79f4b7b09caaacfef2ee455e49d92" },
                { "tl", "7cd53844b7126a69425359254512971d2f9c008334132360cad97f9bf3952ea3a951fa2d64900072a4e76104bbeac180fae23fed20f2b50936ba8b2619cc0882" },
                { "tr", "2ac7e2915b840878b6773d56a69e35b1751eb20608c2d7ae4ac77d1120c60ea6fd0dbd3597f5a68a0992c40fd1865e1ab8f8a4694dafb2700515b27f1cdb2021" },
                { "trs", "5524b07637ab7b62a09f5c5c6e9e091515e2cccdfeadcdeebdd46119c136e73e876c32eb441d6b1b18ce6e128730045c70b21c9a16f900183484e60206faac00" },
                { "uk", "698daf278d027fbec64082ac58bfdd914f033d5eaf3ed52789d43d2f14d6ef657a285f5e4fa74c4e8b47896e8c7c19b17ee0fbd51fe125f86fda9e0c5c4fc045" },
                { "ur", "64cf4cdb87c81325de54bd4f03bfbeecfd7273981f02f7e21ddbacd8df928f4fa581f08634832f847e0fd55b2cbd14da8d748644b08c9bfac4289ac7cb8d25a7" },
                { "uz", "628e1ff8f82fa2fc469f4a76645589532e8250e7b7471bee7be730c512440deed42f5ea011070720ee7be4fe37a55fe023d876ab65c91fbf761c54d745a6ac22" },
                { "vi", "5beb373db5bf48cb5c052738c5cfbed94882067b0aca4654186143f5e23b1c2c980fdb79fee313df86744c69d7ac20523880e612fdcada4b5a982ac0e94831a9" },
                { "xh", "604965c773d9d9623948c5ce3d1c2aaaede07bfe6d3cd1b5b4c5969cf3476b6eacd9782fdea8810e0da5ad0972d437ad65251400a191740199ea82fa271c2067" },
                { "zh-CN", "f32a12b1ac54f48765d4fdb28c6023d001feb4932ee89c696a30793ab0c878062260d911a00aeda3200249768ec9ee8249897e695673d24bb6f21e93b39653a6" },
                { "zh-TW", "72c04146ef5dcfab66275e6cb5c2adc88e7dab24b34203fa6c7bc4d54f0100225917ad1fca14ecb61702094687fbae388edf127b1266c15a3dbf56453da94579" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/95.0b4/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "b6793c3dc00a8a1f65ef653afa8800d343fef51745866e600d36c54f8776d5cc0974bd2a196b7150b5293dd7673df83ed2fd91776ef82fe4bd0214e7ddea4c86" },
                { "af", "64146654f26f4511abc3ed6e617b362b84bfaab44b84b415573c069b572d672de89bb5b9415b2d3dfe1a3a5069cf41292394fda4b4f0e92028d602b7103a42b0" },
                { "an", "253cd8c51440aeb04ec4e9f9968cc904a50a771a46ed2fde86761409260f53e95e13650472643c3fd9cbb4241de3396c6e7c49dc7736c8d830c030566a3392f2" },
                { "ar", "dc53f85ced8d5b6f096224621eb4bd8af8388e47e5472f656f2aca02484ccd74c840b3087f5821cf4a9b3235726b46ca440fb478ae064073c5368ce873b89da2" },
                { "ast", "4f869f3f959b0dcd7d5a4d77f1a1b6760e37a48b8060d72983943d9532abca449fff4e491355ad98621067358b0117bd2b315c052fcb9bf8b6419fbf4c2de660" },
                { "az", "71f50b3e32b19d5ab289e0b0d247c48a1bbc4ae25a0352134821b77343f502633f2672444a7c32960b28b061f0ed50261316b2d969c5eee9091ec630819f3344" },
                { "be", "74a20dc44876e45ba46e2208c556cda5fe9fac77c289bacdeaa0651a7fe90ff073f89d1fdaa6fb1bfaeb55550c76fd996881d75b8bd9052f0fb558dea18b1cf4" },
                { "bg", "aefd507c7fc125b4b9689e965b147c87129bd2ee38cbaeea0dbac4d6b6d52cdd77060bf1f73c901504aa217b00328d99d683f457423242961c2bde2f1f835ef5" },
                { "bn", "bd04df336bc807ac4f97e764a9035997aab3ff011b24f925d79e223b6338c1342bb307209c2620b1460473362ac92474151abc30100f806b6b27dbcc639fcdca" },
                { "br", "72cdad931693d787d550120926a8a1fc12717c59142f03312821b7ccd05404b60b0bffa388f432652fac38db0aefdd60c3fb57cb412ad1880442b3cad7684f23" },
                { "bs", "2222552412f2b3f2931650a9c8d4c5c277f03f753ebd0d83e5ff931c997525f3cd0cd45fb025ccacc2c77aa0ba77cb0cf97a3f2055404206bef595fc14189569" },
                { "ca", "537ec5c79e5df21cd425fae924d3e026e3f44145c318dd627faab6933d22672beda2e5602b8e8570c95314b7dc8933752176c0e678aeee181a5847d9268d7958" },
                { "cak", "cba18650f52d55ef8f2ef51268ccacbfa99e6416465984f33fc1bc5203afc0c0e1c587277a891b6289084de5bc303a6548a2ae3adb92c24f6c021a4c730f5995" },
                { "cs", "36117b4872f9ea2b851534433270ffe14a5697290e61392f145defe918805ddca25dab79dbc08e57617bc157f270ecd41877a0da7084b387d67d50818ab97ac9" },
                { "cy", "62f8c75e8175a89a986d7668a96784886639578477dcd5ec967f2ec67678c681b3d69eb98854e9de149a635899067ac8b87697ca28a6423b6f906eda5e906232" },
                { "da", "7e1fa34be74b61ec58f4b293d768ef77174a49e9357e25be44f2049a7fe02870c6ba855bc59e284a7b387514b8703548cfca94efcf9938107dd751f5cc457a2d" },
                { "de", "839e5929b37c37d4f9f44d8ee593a1eaf7294631f95909cfb334a51f03366eab8b378d0da4ec49611357ccee43db1a411de57ee36e46962fef96b6374a710d2e" },
                { "dsb", "469d8d0e70e39dd62bbc323b164799204e6344a058e6d03e9df0ab67990a1229de0989744a00aad9458775661f53f8445e0ff9f94762005bd8c91a57c379e911" },
                { "el", "b35690cecf8ff516592f47ae7e3f6390464167182000f099c8eca9132014343eefb69185d1e2cd92d8015d969b58fbc873c21dc7d1928936d88b2b656217d921" },
                { "en-CA", "e3a97babf96baffd73772e061c212a3b5ff8c3854aaaf05645f4896df80a67b23337c165aceb34910aa35bb19f7e12ad5d1fc77baf35fa0e324833200e9104ac" },
                { "en-GB", "4a8b95549f28faf5f781b66a12149e80c0b4168ad09db42fb3e4b7055de826996e44ff00e54ea57149e3fb6fa7c70941eab97ba1c942ad40327e6c4661324deb" },
                { "en-US", "20c34f65238ffdf0fcdb89695472f07a6e5e2de5fe2fc5e7209f72be5ecd47b6fef21cb1b23bd33d8fe2d4bf530a6f1c9f24605b50ab6a445424daa84a643219" },
                { "eo", "72d8c036fb045bf793ef497c2ccec81f7a8a00e79c456f04e81371c475a5d23d50e80f7aac062f391f1d37f7fa7e18885b51866943110d9e9f42f784b15907b8" },
                { "es-AR", "78855a44c0b8fb38151b9f0a8de0f48def7ccfe6c98ad07936a51933b5259d9b729bb269eb369ac5c3c18daab5aa74f155c433711338aba302dc2ba654578218" },
                { "es-CL", "b1ab95c4b62a61f6f9cb3b7345556395340163068cc94fcf0eeb392599e75699184b89246e3cee0b159a404c5c4694d7f5ae1b881944af6a839ab9755b0d7c68" },
                { "es-ES", "e3ac8dd4b41d7a26cc35c4f80a755c18e3c6c2ee86578d726b2db7ec4c9ff2f4acd2f6898a635575e78e3eef37dc89f7f62df16d38324de1de8a1621776ae364" },
                { "es-MX", "f02d4f505aefb718dda995e0e15b242f7b127626c7e32c3ecacd7ad743dde695764b2f8e36c1d8b7708363ca24bdffbcec4f23fed06a505db7c2001eb074ae4d" },
                { "et", "424dd10f27440dd3cf6292b25e94aaeccd9a3e193c55a46045c1fd89b595c6c6a1466524f419beaeeeda90ec6e5c2b3f419cbf6f4ae2f7c4f263caac3fc551b7" },
                { "eu", "10d784ec217b1631c998536ecb2a6defbc385b4fe593cfc421946e3fe5f84f7cf46259049da2dda564a9fbdf85023a4d5edad4da894384d642144cb2450a2440" },
                { "fa", "78d497e37a2f7ed8f573488c19a305f243bd23fcea411e7e21f32a595e70b6efa2cae0f63c8aaaae3aece6dc4963c10b80accf50e90af100f86910cdbe04ecbd" },
                { "ff", "82fd60604fd256fe1c1ec101ef311f89e8f2292e26578a64e071f02d7a79f494bcd75b583f28e7bb5f8a523d9453f4b2b99f6b466d234a6d2779110a83b36677" },
                { "fi", "18d05194c883031e7e809f395dfab91fa4afd740f3538b00bb0577606052820a74010abf1fb3f40b702890cb696c95027dc108df789da5d85cdb14f36ab7553f" },
                { "fr", "d3f388d257f65e714ce7b4ada9778d35be1f733ee33225fdfed5e534e2686063a971edebccb214a4847097caf1529c95e37db76167a2f9bd921a766e2af375a4" },
                { "fy-NL", "0a6bd19f2546d4aaf34cbc1eddf137e5d50fa470de0c4a34bc6cf2717dc20287307140f6648762ba32b53e811712f4536218b349fe5acd8c197eddbea349491e" },
                { "ga-IE", "313c787e1b9668dc5d1b5acbb68c11a41a8b9d8934c4bd5e5a372ec1455473f00840366096afc50283caaaeaa06268dca0833ab692eed1a6a3db0426aa79f41f" },
                { "gd", "5fb05c9d0e4fbcd1e11f1d73cca1040d73151b975f44fc9c429e492d66dfd61286502e23dfad0eb626ccd289936faee28eb72448f92280dcaaa6f6ed1258ba9a" },
                { "gl", "c000932c555caf462d0b57c618590529895fd2cd7a57eebf85a4272224b0e7fea8d8aeb08000ea32595f3224dc3ef70c25b9149644c7bc124411600f5c27bc31" },
                { "gn", "cfca1d9ed7a3415a64dbf200673caef90e56b9006f5722af802812410ca9e4f60cb0b08d2f0cd0ab790c6f2bb14c6dbafc5ee73e10116e5708d570625bfad634" },
                { "gu-IN", "89505102d83ade31656dae0c5fb0d9e2ed44043205631b87451888eff58ab3f5de69f8db6f6034685e33ce1348f616682d42b0176f855f06c53db7b17d020048" },
                { "he", "294b71d6a609eaa157d55c93d879659d5c16fec0d1465946b475fa0e42b3781e9220cdd1bfa8119e55f9f91b064d536d36afeb31e09cefcb648dc5a947031d61" },
                { "hi-IN", "a8ded434ca4a158577858670d3fcd5d439b9a14a6611bc60b93cc9727c63100275180aef827d1e34c93bf694869d3edba5d8e5bbe03425705dcfc4f31051d3d4" },
                { "hr", "b4b8674950a7c2a9edba5f1553372b3864b80c27287d17e98516573860bf6b9c814224d297fe6768f5e579fd00b4849e451db03a4e521957b3be055ad0e2a8dc" },
                { "hsb", "67875ea0df5cb4c88bbd6e3f2a27b461a1ef992067fac7870670e9cf5e2d209190dde400933cd0e95fac5ab561d31b9b43afaf58142ed3ad7de0425f3793d04d" },
                { "hu", "8d9a21698ceddd329b56f46b92274bcae5a5740c24af490a25c63edfbf97664e9658ed9fbd7377b040792bcf4b627645399f219212d38bbdc6bcce8e7f79a703" },
                { "hy-AM", "5afcc7ffc5af130a4333df686dd0eccf8fc541dffc0d2bebd088b2832fb49bc7bb98849e968261b5a46c43c576b6653a3783256fd208c2712a473e7ea8b6bf8c" },
                { "ia", "636f69cfd0f0f0e51aa9ab6e4130716abaa6874f744ccf120b3650e2dba5d41ff71fd9b6f07e09dd4530431d04105b92993b65b03cfc68339cacf027e804382f" },
                { "id", "4f3f4bc9d4f46fad2dcfa5df91b239bd95780166288ee87c75abe653dc6af6e21c03ead0fef8bf645a0cb3407d7a89178026a9e4bfe6ddab6577eefb47091a33" },
                { "is", "2108934329da8f88781a02796516affbd627290a6d68622fb8d94f212429f409a2c7d1c7f8e89d636abec9ec4c4ab42858fa06f0d93cd29d5b979aa67d3a73bf" },
                { "it", "a1a285bef0739d5c69e0f04ae22c4316131012eebacc6ed630b52191adee3a4ef5dc23b221b70bc869ed2d63d80fd362d332e50e8d5098136d58ceb3a3e3e47a" },
                { "ja", "6bdb1218749ac5be6ba88ab27e980cdf1c12825626bf5537daed1feb74d909f67e146a01c85e21d66e6b1d9b4590e21bc7a2e24c635e36b7dc923e33d947b0c6" },
                { "ka", "0b2f3c02b2dde230089407e6cdcc1555f553dac8086e6ca88ef4ba7b3e31413d7699473c1f46a40d85e5e15b43f5e5f026cb207eba75dd1c5e7cc0011b950d2e" },
                { "kab", "0caded90a0e0c82f4c3523d1d1f367b01e970a966fb2f91c22032c2e11dc878c0530a5597651df459a8f8894b5b781372e60beb0719c0ce1be4c5250298f4997" },
                { "kk", "35487707404d2222050f4f912f17696170e45171a87e2c5c17d6b1a36441e432cd2e3ec74a6a979be625e6ac3ce06bed139406be6a3836b9ecf4453ef3fdd62f" },
                { "km", "abc9df62de383f33a9ff117d4122f1df82a1d49683fb04d1a87b4b6dfb23a723ccc417112ae042e6300f895c6feb6ce7b986e72fcd8a70244720a36f23420235" },
                { "kn", "58e8924f079cb0e54c5301d86caa7c224614b2bbdac45038c66c72b37a839767f9251244e062092f39ab233d4b95a240e1009f9f50ce307ee6106ff2095ceaf7" },
                { "ko", "6db6d546142f32738e70ea4d007dcca4ccf1d17b3f06344ccebb0d998660830d98c812f25b40bf79f1839f6a98c9af93c3871ff0b5be1fc34f374ef4781e9303" },
                { "lij", "5509234e01222ac9c1b88a498888ff9101f4bdeb6ca96314a5a2c4c3805cfb21931bc604aea9c2e66c40b9f67f59b5510e9b734cd1d8e06619f8bf005678e226" },
                { "lt", "24dec49a6ee43874127260de6b86ca432871ed93d3a4e65343f2b7ef20367091d7bda1ffef8ecb4688845950c2400bbd838e8be38ad882ededd6fe65fe6b54f3" },
                { "lv", "df3a1d595b578de77f3c5a4b06b281edcd12f2e089f25426e7d2cf835f2f0f8260f2e0a9adf49b2600f28113e335d562251d989cb651ea7570eef2f811d6e6df" },
                { "mk", "caf1851b2e9680895f48c254eae65a53744cad376dccd633ce0560ad0b1098eb46ff7898b10e00fbe00a269587246d8959cad3c6d8a1121446fecdb794abb2ce" },
                { "mr", "8ab4d9a02b016320e098b71d185928f270bcd566dd69be3015d039fde16ceb95922c68d5b31a1a7f5752f57c9949e5fad42ab103a9422c0e03086448acd9088f" },
                { "ms", "e2a33a87558b7ce39a7b541fa7ea739f95105bbb2cc18bd107dd5d4c36ac285353341db55edeaefc508420ea642a6e28bfce0cc45acd667c69c222657549ef8b" },
                { "my", "02bc40142672dd4772f99d4ce4caeaa19be305996ddeb21e0f7700060531cf5f68ee9913148995a3733f4a344f2f4ceb98ed72700c36c24ea471e61317025399" },
                { "nb-NO", "d60abe28c53c6edc235a429a9e30ba342cd9f9751622b869fc69b04ad52e3cee45a6652f946417c223785ad8e2ccb98635dff19970cfa64bed533a0207ea91b8" },
                { "ne-NP", "a0867c36b3a96a974005d59f4aae284706d76630a362f7c2be5f9a63463dd5dc39dc1927149ae5148954ce0a9e00a19c04e0d0accb0d5e079cb6fa089cd93fcc" },
                { "nl", "fb5a2f8881b0fa7d9c48f46cdcf89a10c1d9c11b95dbf75974eca81b45bad8c120a8bd6dade70c2a2ff3baccd0a9aba4c8a65eeeb7fcfa99f1ed01786fd713c3" },
                { "nn-NO", "36570f833016dcd0e7025b8170666ed4cebbf8f24b08a0232abcce6ee1746fd43b0ad0d007cec34e5399dff764c657311daa87be6edda7cce15a1a2e39063c2d" },
                { "oc", "a4e2a4b362a94e56254e5dae2c8d664dd0a3c5791ce76d1a71152d0acb6cdad1ea6e02de06d6ee60e2f38bdf90f6cbb0038a0a1f7ffbb4ea2dedf181a2d881c2" },
                { "pa-IN", "c19b4a8282ba72aad3b202f1838db9d3b709dfffe65119f5ae9d852ea9b841e4a935db1188052d0215609cfe7941454fc373b4bc88c892fe02d4390bb0834e00" },
                { "pl", "c7ba02a8b330f72e007641791f39d8ed7d63d9bc545737a91785c04d2d5ff0bcdbd6f5fc959c3209ad1e0a3491c219d8c941eeff05b362a37fa02a3b50df0932" },
                { "pt-BR", "66bd8b8f048f4a2b9c5e5d2a03614e68349a32dd97d65a72d0c85ec55a2b5e315c6c5df9a2217d135ea6adf655c9b6c78ed608aca7eacce9aa49902cddcb9474" },
                { "pt-PT", "85e2d028a17d61363b1530d3a1f129a96eb60fd51e71cb3a6cffdbdaba1fdc6933cf0565559f4a13636d92fdc08284e546ee214cf564294b99ef53d476613e9d" },
                { "rm", "dc65be51c78d5977dd0e875162f21989f6c3821404cf2a9423344fb7ddf1929366662d9d12b382c909c92f080f2ef18d0ffd780ae8e1ff8d06b7404871a40697" },
                { "ro", "74e642eb7746f1f03f9548509e9f40c0159c8a161160c34c60e5790d849ec0d810703fe8065540fc44587500cba00275938b5bfe8bb1b35f186f6af8d404e1db" },
                { "ru", "baa8644fd9d1c5f8c2cf2f16950a11db943e07028bce128449e91c787ea3fb445d5b9e1817b3311a6fc5871f1dcd7dbdd505273307dde2864d96467bee79fb52" },
                { "sco", "a6ce686e049deb49ede7ee1dae0d3810f540458c0acb7b784240eea735530b8bd23037eedcb74ff4a64636f9d30d8abf1cc31c6cfca5cae219dbdd4081de8de1" },
                { "si", "6f2efb650b67d18eeb12e5e0ac91809685592abf811ebec0f746e679e30ad4db34c217edf634dc84f08c82032a666ac9a69c5748576624a3dbfed349ccb44adc" },
                { "sk", "59c42e8d2b6c225d1a0b480fb312388c7966688bb5107b3f8dcdefaec1f3fffa783ebc26adbdc46d9ba8ffc89c3801c2d35fc1f55a308beda03734f4ae18f1fa" },
                { "sl", "a2c6a259cbf56aabee8a8fde4157b8ba068f26aa3edd4772a5654399aa740aac9b9910ed296e013c982afea65dfaf63613a9f0efd7670b68c61076d16ebc34af" },
                { "son", "3370e3d8296c756a067ca0d7cb28b4826b498cac32edb92df81968c562c831942e36da642641e1d92ac25bb0cafa9fbc1d4a6f96015572bd49ccae21d412c5fa" },
                { "sq", "5e276cfdc5b8240495762849a629c0850518da20daf236fe089815be6d4085473de86a33fe72e98db474a9139dfeca2a05eaaff49a2ae1ff2fcd560d9cad7fd7" },
                { "sr", "8dab162a647b2a0e211428fa1528c081a56317a8966ff905af6420719733b787e946fbc995d446cad1395bc243f7cc7ef4d074d1a355734101b64ebddd430487" },
                { "sv-SE", "409d86cabe2d3e9cfb68881b93d48f4d147c647a3e7d7f4a0e18a34d465a081747fe2b831ede2f649458450f9a74268f51c3658a0562b413efd75a075316b8d5" },
                { "szl", "c537ba3e3252b8df0f706bf4595998d9491b90a4dbac6eeda5e1a3e47c130c7f00604594d8be8771045265a14e8c6d2194e1446ba7b017a0e344c1a476f12ac4" },
                { "ta", "5430cc22995d7c86a1927faec0edcfd9eb25c7544a3e27483dc2fcba817feceb58be9a47099337a214d0b1e9422a976d1379b1a608c51c535a09ce18ddf4ac43" },
                { "te", "593ecfde332602ad5c37551e36444301a1e02870e2f75a23beb6beb1912c6642e9901732e65f71167a1ee8a55d2374545307b500147d28c1586741f51798e476" },
                { "th", "a9106e9cd605042cb336be5b116d6f7c4c4ffb6ae994a242b54434c9b7af3e50779ca3121eaf5b5cc608edb7e8fe4fd6479bb76a8553dc842439b5ac32844105" },
                { "tl", "5e4032f34b27815cb3df308baa3d276728c0551367896d014eb9b514635ae81fdc87b886cfc8f79e24a6223c83c37ddadc8641498354d5a94078991c5498a604" },
                { "tr", "c0a94b2dcab6706df052a4a4c896e5b80b69a41e63bda834c8899c596527b28b48556963e20e8ad4f744d30aa88fb50736c316942302f6b428fbe387fc875601" },
                { "trs", "06868ed76506dfd71460d246911937c1d5fcd59b044828ab85c50af06fe682f732322c92f57b6dbf5ed608bef831f2ef8322687c6ebb6da7cc0a85baf81c4b83" },
                { "uk", "ff02b5129bbb0f2f441f24c0a2f57fd03b5efadc14a6ece57460f835dc3ba0e4a5f44d024678b4cd0187384c46bed056c90d5ca18ae8ecf37947282289a1de43" },
                { "ur", "a47dd793c8c997201075bf8852b567422544d34e97eaaa9892fad4195b5565cac9663ecbff56170459628a89f3a6537ee18483654c45cc17cc1abb5369806032" },
                { "uz", "ab40640bd07a737add7caf03331c0e458d4cd841a2783965731cd13ac5d8ec9abcbfe8dcb0e42a16b4de2c4ec775a256c119b6421c5ab68ccf579a5eba45f0e6" },
                { "vi", "363b1120fe94bc7a27eaa24866dd8cf49451b0048f6e31e8357f2ee023b56500d59160ef607d4ba83be019be1251cf62724c0a2174b94431884cc992a27bb367" },
                { "xh", "72ee3a68638bf175474661da68cbe19c42a001a9a2672d091ae9aa63e55e5f0bef67e7e6dfb005cd8989af6d4af396f2b0fe04de3c68002c9cee67e05fda1334" },
                { "zh-CN", "d2505ae4c60ea4d5e8684b34368a891544b115139e4f13c70f421ed1817b98ae372254d4bc94d65de1de1f64ecebc57d35c512b53f7832e2be6f6b7d81fca2f2" },
                { "zh-TW", "2a7b32b187f288c48007537ea5146ee012443e45ff3bcbcdadb40fdf474a83212cd6e8543feef917c0fa6ee21214d386cc83e7d22a9c1254518f0004411a60b8" }
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
                // 32 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
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
            return new string[] { "firefox-aurora", "firefox-aurora-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox Developer Edition.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://ftp.mozilla.org/pub/devedition/releases/";

            string htmlContent = null;
            using (var client = new WebClient())
            {
                try
                {
                    htmlContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Error while looking for newer Firefox Developer Edition version: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } // using

            // HTML source contains something like "<a href="/pub/devedition/releases/54.0b11/">54.0b11/</a>"
            // for every version. We just collect them all and look for the newest version.
            List<QuartetAurora> versions = new List<QuartetAurora>();
            Regex regEx = new Regex("<a href=\"/pub/devedition/releases/([0-9]+\\.[0-9]+[a-z][0-9]+)/\">([0-9]+\\.[0-9]+[a-z][0-9]+)/</a>");
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
                return versions[versions.Count - 1].full();
            }
            else
                return null;
        }


        /// <summary>
        /// Tries to get the checksums of the newer version.
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32 bit and 64 bit (in that order), if successful.
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
            string sha512SumsContent = null;
            if (!string.IsNullOrWhiteSpace(checksumsText) && (newerVersion == currentVersion))
            {
                // Use text from earlier request.
                sha512SumsContent = checksumsText;
            }
            else
            {
                // Get file content from Mozilla server.
                string url = "https://ftp.mozilla.org/pub/devedition/releases/" + newerVersion + "/SHA512SUMS";
                using (var client = new WebClient())
                {
                    try
                    {
                        sha512SumsContent = client.DownloadString(url);
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
                    client.Dispose();
                } // using
            } // else
            if (newerVersion == currentVersion)
            {
                if (cs64 == null || cs32 == null)
                {
                    fillChecksumDictionaries();
                }
                if (cs64 != null && cs32 != null && cs32.ContainsKey(languageCode) && cs64.ContainsKey(languageCode))
                {
                    return new string[2] { cs32[languageCode], cs64[languageCode] };
                }
            }
            var sums = new List<string>();
            foreach (var bits in new string[] { "32", "64" })
            {
                // look for line with the correct data
                Regex reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
                    + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
                Match matchChecksum = reChecksum.Match(sha512SumsContent);
                if (!matchChecksum.Success)
                    return null;
                // checksum is the first 128 characters of the match
                sums.Add(matchChecksum.Value.Substring(0, 128));
            } // foreach
            // return list as array
            return sums.ToArray();
        }


        /// <summary>
        /// Takes the plain text from the checksum file (if already present) and extracts checksums from that file into a dictionary.
        /// </summary>
        private void fillChecksumDictionaries()
        {
            if (!string.IsNullOrWhiteSpace(checksumsText))
            {
                if ((null == cs32) || (cs32.Count == 0))
                {
                    // look for lines with language code and version for 32 bit
                    Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value.Substring(0, 128));
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64 bit
                    Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value.Substring(0, 128));
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether or not the method searchForNewer() is implemented.
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
            return new List<string>();
        }


        /// <summary>
        /// language code for the Firefox Developer Edition version
        /// </summary>
        private readonly string languageCode;


        /// <summary>
        /// checksum for the 32 bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private readonly string checksum64Bit;


        /// <summary>
        /// static variable that contains the text from the checksums file
        /// </summary>
        private static string checksumsText = null;

        /// <summary>
        /// dictionary of known checksums for 32 bit versions (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs32 = null;

        /// <summary>
        /// dictionary of known checksums for 64 bit version (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs64 = null;
    } // class
} // namespace

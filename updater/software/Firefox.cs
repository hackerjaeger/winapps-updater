﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022, 2023, 2024  Dirk Stolle

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
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Firefox, release channel
    /// </summary>
    public class Firefox : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for Firefox class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Firefox).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=San Francisco, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2027, 6, 18, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Firefox(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            if (!d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/132.0.1/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "431bd3f9e988e9a997b75ca8390b9f366d9b3cae4938689fa40d30bbb802582928e9513837e8a2de35d73a4e9126f7bf2247ab1cb6ab0131574899434ef98efc" },
                { "af", "7b9fd1ef6a6cc4a305814b68c4dd8ab9c106bb3390a205b1e7adca5ce105cfaa012e38e632c3eb7ef944b530b538b325ad5a2f72173e36714b954329d6072ca4" },
                { "an", "668e504a58a7133e1632b768cb86398bc4c0415f2e3a3d11ea174031f1dae051063aa066f18d2adcd8db4e77d83e24d28a9e4c668600097ea1f78296d261b4ca" },
                { "ar", "a401981d17178668fe7096a9a06406c1bde742360d2f09216cf5905822d4873a5b1f06fda1a59c428599660227f5eb1bcc5a4184e823ddaf1687c9d3914de677" },
                { "ast", "f941a41d3ca86e5aee0ec4e2e212084b23a0bc80c26a02b96b0d7addcf31639ecd3cd00798b8e1a348b334f779d91e6b31f8fa4723795b698043984bf93b2a37" },
                { "az", "1e8f81cd7991424be75fbc09f6f41b65e0a49bd60c8f6c4eb9a84db3b837ba96935aebc07e3ab828294c55c6e567a1d27000f611b7f81e14ebcb12b2b06f4f32" },
                { "be", "96493c60cf15081888f2dab78650d5f107fa3c43a7a205409a0859e3c5d89e5be30a04178485d027aee9b93978201a8cc18ac601b0d0015eb219fa9984f3b838" },
                { "bg", "23dafc98a885edca7c8982c7a3e765d5514845469a7e0a58ed0c043672519693757bc8d94ea038d21cad68a4f8aa5c747843cf86f16263795aab3ef09d98cceb" },
                { "bn", "d22743fa548b7c79bd9d35185347ac24eb432ced4e1f26c31e3bd590392711cdeb7dae5e4499c357ce6d45530faa1f44ad76eadb9c8ec9c6f0ef1b7675291ec0" },
                { "br", "4c71f3edf5555dfc06e232d23f4fa2b7c36fde43e874299fbaefb28e611197ba270687d9b27306da4d1b9c631425d7fb34a4421cce6d8fd842dc0e15a053d5f6" },
                { "bs", "8ba3a2041082ee37da45b75aa3b2e3459911db8e3ce84f278aef11db121a21643acd70658b6297049c688a0de6ec61c9bf8c43473c5606256b7e985f26b10033" },
                { "ca", "c751aa8ebaabfe15e82f47e6b1ec23657d99ca999c1d826a49f4d515a92e1aa4459ea3eb23092b8a579ab4242ac49071f49ae5a100f34ecb041001287663ea43" },
                { "cak", "fca7ab6571da3ae88a1ab9027ae64e1c1e8312868607738a6242df9898842caacc85354a77ce6c5c30d426590acbc9cd768788aa5cb33fea96495a3298182b7f" },
                { "cs", "77fd88b7a9bd3bc60209b47b5e2a5e295d8e24611120c894bd0a4b8b95a403fa2be6aaf12df5d8dea35067e9c316d6b8bea3bc4fbcd7df8d7be004e362aece48" },
                { "cy", "b32de2b47a146ba6a02c144a9f2e7fa49c2d68cf274830f0b70694614b03db939a0eab54d7acbed2a01526307ed6ae414f43e16254bdc1ea3bdf63d893586714" },
                { "da", "4e6ae767cd4d8954d9c73749b14d509868246b7ab0b448ff04253cf461d1a66b836b4551135a094d3093f143179f19918b97bdfcc8a402de2e01503b5c0bb77e" },
                { "de", "1277b3dd138c91a9dd243c49a6ed3c51d3e1b5bd7735e63bf9a902d04c784603562c8b731af77af06b1ab7ef459c9918ae4c9a580585866037d27b62a1f5a42b" },
                { "dsb", "826898a855877b669ff89620229aa51d8dd27287b095b5eaee538385de0e88e6753bf238c4cbecd1c38bba0e6463a52899c7e056afab1fee4b01135dc7948375" },
                { "el", "4a385dffa5ed5bf24036ccb614ae838fd22c0692d90f071a02c1a8f7c7ecc5a4ce9fa3b6d633f6b316eb8279fb8ccd1e8d5e71d5cd9a9ec598574284714c2b8e" },
                { "en-CA", "3403e0722b0dfd1c803d343d9c55f92918d8b0516bf73cbd1595985e49230a68ef8bb7b9faa8f9aab8be3c128e0e2fc33ddbc3e3518d40d1f84abab0312b1899" },
                { "en-GB", "ace0cd688fa4d3bff4153368de0afee58c9106f6e971766420a00d0afc447a0bce18d859a7025fd9c995037d28b822e5f50229e4106cb22526c100309e856107" },
                { "en-US", "5dd8ea4047c7a08ca3c941b86b6525795a99a045e4d2388adf3a6a0eead617d6eefeea057152a24e08462e71ccbfc4893bed32fdfeff3688ac6bafde557d6c40" },
                { "eo", "50dd80105d4328b8ea4530d15b0a9702a2058b75819fc3f9813c8ab3b794f127616da8460289fbe24d0a6c6bde24b1b17e00bd15f63f09d455d2c37ba7ad460b" },
                { "es-AR", "136e88be014f401aefc40764a5b4f68ab1f2a5d61f723a98134509f452075daa1626a97e1cda96e5b03488ac6ec71a2778dce070ef553edb35e6c32e4248e884" },
                { "es-CL", "b4125af4108ce6a7ae4ac29b591989c789554c1b7a73b9fedf69dfbf62f7f20b23479c4bc68f4ef86769d443b8ec36a19852b4e8806e9b10257afa18147a6878" },
                { "es-ES", "b1b2ef349a4cad6f23b37f1473cad301a05586c3ac046e0485f4d11d49f30740b4cac6e15e0ac6afe4154359e93401ffaf0fc4062533595c05c9cb27dd74d761" },
                { "es-MX", "69d8c0911234e83c2810124241479223c66727f2cadfd87759a1d2c2645711177c6bf0829717a1a33a0cf5e3095ab3998032bf7ae314cfdfc07330fba1028480" },
                { "et", "e1af46b17d115a63ee5fba8260e10fcbf668b7331ce6c18fe9666bf694ea94e11ecddbeab6bd99dc3292e653f2ad85d3e9ebe089db67d80c18254c37bf3ed495" },
                { "eu", "6c31a9b209a6f80b4f93363607a4dd4be5402e3f15cfe10f654f778aeeebd4465c4aa09e90073ce070796d58ee9d2f6b6e7a9fe078f44d2d68d19f6bdaf73ac8" },
                { "fa", "8723b82129e0b87c0fdea6ebc3d6b87aca7ac5b46ee1c3b0d595e9064a0d17768ffb9da059abaaf565f3456f620b08f84915b93ba74166af1df52d058882e93c" },
                { "ff", "c8b299c04977d66932ff556564518f57110d8422c09361011dd039e7b53ca7abd3e4e15ed16b6da532fc0054772cae95b5e60a02796390424a21ee395484e8a3" },
                { "fi", "21d2865547416217342fcf133e425301feeba958fb4c2c3fc9cc0df03e4e87b4cff28e155821e46aa247a882707df566836cf6b6e43350f9aff8f58dc6a84b7d" },
                { "fr", "f3fa3687f36592e71debd338e3c0e7b6d5826125bb84d26ba2c7244be16e260259096ff9e2a1605d6b60566fb78f00627585beff7df542e11158a5a02184c155" },
                { "fur", "237ee01042c959368f428b3eeed81eb70da07e9fb26343983027819a6484d392603ed3e618ec355b1fb8954c4a5c6aecfb6cd6b7829ec74e5647f3d073c67714" },
                { "fy-NL", "a1be791cefa1e84825e8cc216a4eb69d7118303b342e441ebfc516c811a78f3c48dab2a8b4a0616a857625dd86f20dba9e7097c150da440edc0c49d1243ac44e" },
                { "ga-IE", "dac619c7a66a1f06c4871e75aeaa9c3c653568d36e6b6ab00e6f8d3d26f39d84e200e3a575ed193ee1d43afe7b34aca4515743fdfdc857b013215f482e9ae5c7" },
                { "gd", "6a828aad6d6487409d241b45864d05636cf073c58ee4d30f75e8e8ea5eeb3aebf5085461dc1a82cd8d299210e54b15c8ed6184413b74a40d41d5507560992348" },
                { "gl", "c403c4efd09ee2db7e08d8765ca723af4780166f3519ca938ba085de437944cdc7eb282d2ff4fc5989524137fdf4b83dcb06b1776c3f1d54c7c7ddd311e956c2" },
                { "gn", "b5b9b307e6a50acfbb38cb953d5330cf10fd4493fb39fdea28c8300de5ffc005e92bee8adfa574533eeb888db4c76e26d2d607a6d872daaef10a874f36d7ff06" },
                { "gu-IN", "6704e795a1d5b5da851259329bdffa93772ca41d80f26da2fc9f7c4d742404bfb832244cdded30b8f2808c35a02a5f5efe3361c4be58b57d89ab469b55a28f68" },
                { "he", "7188b2e6a132640013099af9e8ce8d224ff2d702cac04992701bdce3b4bb303f911ed93fb582f3108ee9774c4e2d456b067fdd7cbeb8b0d6b9e321f4bbdb417d" },
                { "hi-IN", "d2fab69aa8e2096a6c7b49f40fbfb97e7c1c28e3bda4db1c7900beb52a9036592797733f687bad735d2450492a58542ae7bcd3d3fdfa1d66b0a342df4e2734e5" },
                { "hr", "65038e1f522efba10c223e51a4912fdcedfbbca2194e703f7c124d3179b362f4ad3af6b16df06434607f95f1464f0dd5b3ec09c03a493dcd394e0dee9cda50a4" },
                { "hsb", "6f8d1f0f9b8fbb667246a3376b4ecd149638070f580f8a3177371d827c4d8a16a8df0b825e0830d512740992f839f89bcf6f77a0ae66366842490770f37d0bef" },
                { "hu", "6daab6489a390c34c0ae68d159bff5466e1f73d7b28f9786e171dba30c4071b7eec1b171f9594f7bf88530b88c6fbb7db7187ebe0e50bdbefc8505be74ecf9c5" },
                { "hy-AM", "22ad9e534a43ebd6900b92748bdd089e828df774512d87d701cc5c7c6587dc6be3e38aae85f80b125f678b863dc643d39c37872c69ad1f6a319747e85833caf0" },
                { "ia", "e95f18b766e3ab61df17b9f01337d3901414aa578a49def357f45949a6c14ce11c860f699bb740bbb77da380664d350ac1846318a0ffe1254274bb27000acb8f" },
                { "id", "a5fad8860d1f24069cd376aac93ad0d41955720c9222b1529875588a727a9e39f4d0b891c0b8240ee1256f29004f0b5104195cf0ff96e68c85c420483fad1c1f" },
                { "is", "09040f8733e53faaad71a59da82b78e73f9e908d039b31431e7372494994f62248fcaa4f35980c7fce0e2a9e891995b413f6cee66296c2d1ef6527327a4678c9" },
                { "it", "d143aeb6a49438884e65bcf2cff4c0b5907550b832979b6a6967c5c32d999776712be893db166d7bb7bb676bd32b03eb3734b93b9b2fe1f878e6f881ed4b054c" },
                { "ja", "93b27c9fcad0abf8eab8b7fdb3d469c486d1f9f3e2be6e5fb1441ae5c74c3621717cd5c5756823c5c6afdf7009dcd5e7a5fdc7a8f8d1551982571727e8de118f" },
                { "ka", "ea0d789c7ccff7b5bb92b828c122789aedc638d0cfb9bf1c5486e8bd5b40ebbe578f309e58eae581db80932f29d5fbedcbcf4f17bcb23b214d21bb813d5093f6" },
                { "kab", "00ac9c7faf9ab45276b71d3f1035349400e414edb80a2fe40e9157bb37d809fb74770f6adb667a29fa65b623cd9470a400c8c375f17572c3bd5edb4fcf56df32" },
                { "kk", "638468df33326734e5d90a52039ae30dc503875166b570e391fd9439170e109aff152432037446adce8c3db1b6c2a3ae5b0f24f8683c04c00f0fb1680eacf78c" },
                { "km", "0ab2f5112129ce09bbc775aecc8c9f885a1ec0142bd10c1e744d142dec3cb14a9a63ac5f6ef2517385182d8db75733885f4fc26acf1efc9a7fbb15f2cac706ff" },
                { "kn", "8b60704ed572a3e30d8c4e2b825f336e65bd0fdc2ca048cfc2ac923ba822e6430fa7da6ee4f8a0cdf5bb334ee947c7ff3ae9afcc17325fad0cd34d1b1c11aede" },
                { "ko", "72c96dab9ea3e0e23c147e92b9133b4fa0ed0c7f77fa31515819c3acba4e002811ad066a918492985dd628f7a39cac590b689e4b13eea04fb5c8fda81ab38cdc" },
                { "lij", "0492a60d52f44335b938a56208f5486abd13427a4e04ae23089f922adfd85ac58a31209a1cdbccbb916efdae6f97f5ea6585240144d48b4c796385a0c772bbe3" },
                { "lt", "a03cb0282ab52c5a803ae113a876185634cf11a5a7c0725bc91f0f9f16b830fafbd2245c80376d6f21a9925e38def63c8e94889ac926fabfddbf88fca8db9f38" },
                { "lv", "04f03560346420a679126eb0bfaa4a01cac9d151ffbcea50fed58f7784633c68004ea7d79796e87f0aff9c9b5dceb930df1e14bfcf22ea2d5d003344cef84c6a" },
                { "mk", "7af6358a616171391994d0a2d2eae3051f158548f3099f2066bf5b02a97cb1745242a9e5242a5dc099ad47b362df93f7b3b1aaa4194e3646dbab9844b2b3a9d6" },
                { "mr", "cb2ef049b6879cb97c5b74c61b93cfb389309fa9252ebf6ff996b32b8ddf7a8945faf7f49719e3ce1e0059d36b2d6deb649b07a7bc4e5ea3aed5cc2bcd5097f6" },
                { "ms", "fc7830382ee40f0fa4c9d8e065047c49d3a0aac95636582c772d40c243c2f0bdaa6283a0d1c2671983b16ec7c7f467cb74b530e2c8a1c423fec92d14fea028a4" },
                { "my", "2d06a9366c38482c0305794b080562eedfc71cbe58ebf596478c73462d61e1dd5f22712ce17c143fbc4ad930221ee2b455494bef339cd5874f9e6bd55f5b5405" },
                { "nb-NO", "9cd6ec229af4d9609180d6755006012a2e14533f81a7ac351a3a672e85c0a8b57c914d7cb2a795d09439c0f0863866cd78e4364a86e52da18ca379f43c8d8d63" },
                { "ne-NP", "fb0b7628ff6e95ae545e282295c141a4ae4b78f44a12d56a90ddae82de89cc878bfc6f4e20c39397e73a5784104c0dec91dd3431f1f1a058f69250abf4ea40de" },
                { "nl", "9ca8aacfcdbe127baaae1d768f750f55a8e24e6d63dc1351d735e28263882514b07f1b1142e8d1a104077bfe0cc9f9d56ad0d0fea2072ca525aa4a1310e637ff" },
                { "nn-NO", "9c2ce7f6b6ce586dd4d6deb28ac10f5c0fc83fba839c2f179e39de77b44d36721fcde4f29e9aef6dac937f429e30dd87f03ce509659283ebbeef03e5ecf09b94" },
                { "oc", "4a9e6253b0cdaa76ca3c89bacc5d9a1d526839f8b2f995ec4e7a5cf1ff88aaac8227c54b6dd6edf4cbecdad0e0d34462222b7eb8ac0c8688b2a3cdd7f8894512" },
                { "pa-IN", "5ed4c8969f8dab8e84533fd22d930b3eba4269f6b31f2a1e9c563c096c96c3f4ee6efb1b552195968218c78a09aba01656173eb81e449d662faa14dbff8a6c62" },
                { "pl", "63c6f0e90cc79d4d31778c6606219e5168ea2db63dcad567e05154fb68fc84f88b63ba22bdefb419b4b85c22821aad2547706f8a10d4d10ef78a09172044901e" },
                { "pt-BR", "7287c7a0052609cb24f5476b66852940c44a5f1715900906da319b4554f8c30a02000c595338c0f92d388985182072b51419ea0c6d9f28b8ab2990a86dc9df02" },
                { "pt-PT", "555c9d747db5f34d19eaa9a98342f088f93cd98c5580a7344a71ff6d4935dc6693bf3c1a040c88ad9f400a40ef68a1c034789f2f2db2462f3d608ff532f143be" },
                { "rm", "9aaf7b8525ce17a18ab0a56e6b34bb6305f7b9e8547fe07205da838220752e79fbe0285b4533aa9f735c454f7dba56dda1259f8108b471d1196dd5ec24f053ac" },
                { "ro", "ed4a821a414baace2ed14381438948218c93a3578df769ad98f8a05fb7431e73de0dfed8172df9e90defe13d4ce565cf27ee14424084ea3dcc0f1b5164cf2a9c" },
                { "ru", "dda0d872addd9e7f836b5dcd1c8e2816e15679002e2c9eb94d2cc917e49b96a63654893d43a27c4812293f7efc4b81eae200fa11ca810bfe5faa4d9befc03591" },
                { "sat", "2fa8244ddeb7bdbadef438e51dc4a4238af2188efdcacc72bf52832f3a5a42d6f1c3f431b723cda2b6cf9da4f8eb690a4f535797a20ab1938499b35e0ccd0d95" },
                { "sc", "f6c9488f6baffe89a0625385dda21ff96df6f13bfd5cd8ebdb6d7ac795db6d81f4ee386023d8eeaf59d0753588e8076250e7b6d0676b78f5c094754616ab991c" },
                { "sco", "859af71248f6cb93e1371c9e162414f95370eef582a0582bc212b1a24ded1d5ca8921badda61dca1cb0d21ed617a365d7fc98a177d428c76cd4c5c0b3bd69fe5" },
                { "si", "110ee0f3efa3550a5e144a46019242ba94d74dcb7c25a4619645f6d117c3d06914786c5da4e3eb389fbd81be5d1d0d2f1ba50fd1f461f1e7d6346e92084be325" },
                { "sk", "b4bd1746edd8f6a3b0a55e8a1965c56c0039d8191f1ec930a533074e707e4ee109ed850bee9b9dacf1196c51b1c7ce58f48e6196e32ddc803b6f4785153e7e56" },
                { "skr", "3912df98969d59cf1e51e2cb06470664a69915bf2d3098caf27410a3e8f2ff5340cac87a6a2402f839132ba182a7a200efcd9de279430e759ad2fdc66f2e0364" },
                { "sl", "6764f72e2e0c2d60a997a4f7ed965b54073965610072a08370849c952a50b1c001d5938e6d3b60dd91f6e2a03af86391c0f966c37cd02853a5a4c4d52b2d557d" },
                { "son", "2a835a8ddb8e5821fa5fa982ffbfc021e64c0cba30a8a543051be71bac41ebe476e89a5372f4ff4ef9e8c6c4bcb7bd5d256267c452223ee3d91e6b901242c668" },
                { "sq", "b94b120a28b23b4c58b33e1f602d7935624312f8ff8e34c9d9e8c88be5c2d516bb3d9cdeaa9cd908fd72d6d12c2d29cae2c36ca61dd3fb2f26f25b18313c39a1" },
                { "sr", "e4cc976fa78b4c5fb645c20871164e262f4320e04744fc73f579b84222b300d94a59872f154c0fd166a33597c219d45effa9c206fff659e890b579f9f92aaded" },
                { "sv-SE", "2437f9c6b0b2f03a7dff643382bb1c490871d81ffe7af2cc454fa9f7922d13e9f83f859e705cbcf4c4bff2fdfa1c389fc81b4613babb19ccd71f3d619229ba7a" },
                { "szl", "096299fa5f532cdbd0792c0cb654d4fecb16eeaed838d3dc95be7dd40c0568fa7276d804da175fb2644f2a03970030fb5be97319f6a50487a70764a0b633ab2d" },
                { "ta", "83826848cd1bb61d92e7a12195faa1d2985753296534b1c774247e5e231ff2ac33cb8225bf8bb86e08cca8a11947a72c03b0514f2a7cab484dc324c5ca5c8025" },
                { "te", "e054d7520007713bdac89742bd79f7d5b10d9040cca8707e3c237df3c52abe952908f8d3eb9552241761f86d36dc93afea91250a632a179e7e38b8dd2a3689ca" },
                { "tg", "968ebe04f0cd62a3f51b4f1376fcfb688ee3d4941c0eed160070403750a3f8469202467d761ad4f23a231fc6a5e8f909d0cc1ae29181b768e590770e9d9db580" },
                { "th", "eddf44123bf17a3cda12e08a555aadda406bfcb1f1bd7897899e075423a5a96b9ea379ebfcd0a7e37ae6681a71f20b63489376cf7e3fc23a2133f1b857fb08e8" },
                { "tl", "70f0c08f1511fc205e32aa43e495ac6356eb5c343bf5842d56550036fa50665ae8f5480ecdbac9ab42236fe4c20b95ed33219c7493678f3db958d1babb0029a0" },
                { "tr", "7146704d303403efb88547b680b69fe79ed2c8fd227cc8541f0f56fc0805ee88a9b27d03e247e636f023e8b48dffdde0d45d63c3d6640addbdf4f550af07792c" },
                { "trs", "a8172bc5640823a25415e68c1dafe6fe11c19a9542dd8693761a98c0d4fa75a45858412fa62a328f3246a132958077cb37fb141aa6c66adfbec97a9da13f0007" },
                { "uk", "00d694e884aca271bf728363d7532c327cfc4b7d7cabcc7610849bc91fa7c3dd4d9223052b79648bcb02d27630e53b792fe2ad740617d7aeaa2f9e757fa2ba18" },
                { "ur", "31b76b51c1ced482603574b12121ebe338eb42e88c7f78d5f463208d8f4e66166124ae416ba447d42201d6b0e7a8cd2bd328d8c2b3ee86de850e569aeffb8ee1" },
                { "uz", "3aa0ef0c9b8e2779139cd61f29ab556688fbaffb51bee4af4d66c00e238ff1f85fa98bde0ce8b7069c8f27ef8a0938ea54ccc70aa603de1ee3d9ac8706b5e753" },
                { "vi", "54ccc8bf1be17f9381e7f54bd8d63e7a39e12e9816e717e3980bce49f948fcf16f5d516752c8e3349528d3c527cb505e932d52e316f8b55aa5422d0f0fa6416a" },
                { "xh", "f60e63ed353c12cfabf82e8f69db203a6c93058f09e4583361b1761e5bb402b144440dc3c10b7bd2678ebb11c44a0a463e47c6d3f013e981b8acad64f1bcdd05" },
                { "zh-CN", "c0fb812eb60bde8e456a09c44b51e6e5ab081041ffeedcad3cf37a1b505f5c444a624d53250805e044cad21b8da065e007b2ba09b2aa07f4ef12fe6efce7983b" },
                { "zh-TW", "d4c4a9972cff8c8e554e74f0c29b2fa7e8fd6dac0247adebc8e89791d03d1202509f3a232c0353dca487ed8a7a561abd122d1ca0906f3c307ea49e9330a8b8de" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/132.0.1/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "e2ead90c29a842a402e1b86c0a436dc18e448392a3ec557d344ae6b4b7da96a794bd9ac56267164ed050a9d44796248bc5afcffeba76e737d21f7fcc8236f2fd" },
                { "af", "eb196a2fc0e5544a707123fa3581ae01f1a27d678b978a6bf69efee537d21533771e3bc579a6e93140eb55646dae78937730719e0aeaa7836447397ac99ec62d" },
                { "an", "5135b0d2e2b4efedfd1ed4d722363f801e53d4f770db588a49e832ea72f2e67492c44460cdd4a241cc506aa96ea54d43865619d39940348648b3d1044b3ac683" },
                { "ar", "2e3abba60c9004cbce597989188f8ace630796ab6ed38bb4c17a59b5fa97d60d19fbef36a6f351613c19aa6dac3f5945b56ea768a7e1a69939a6cbe642b9215f" },
                { "ast", "71e5cf07e749beb67ffd9f6937ade22457d7a55c8ae4200e3035cd353b2884e1790bd29f62aada7c14036480058beb986e6ee72d989482f04c2ab9ee50e34787" },
                { "az", "8aa5107bfb524c5eb1c5a4de52e639dc07908cd47ba61ebe2a5a5f96aef4df2d7f22a62cc55e486ed01b61461dbf55eaebf91c4d15dbfefbd5f581d594ab8f9b" },
                { "be", "5a0428d7164a2c12bba823c5ad9fb7b105ab3d89859cc30b1d9ce2cc391be8412a5ca2d4862bf011815b559e2b1bfcc563a328b558c00f3f0c304aee542905cb" },
                { "bg", "31385f167c34a68098945409791a350cc825278ea508ce86e3458ceb7aef2862ea693b838dbdffa3fce86b6dfa4e73ae23f8bbffcfa30bbdbce78958cbe2c217" },
                { "bn", "9d3e1fefddc87cae80db5638860bb70285dacadce4947b86c47a46f6e1cbb9b5e6f9d4499d7cdee255b27419069359385f7f5dc3e2f321dd101df9575ca771fb" },
                { "br", "e9d7ef2170e53147bc8908e11fbff3ef3a1450c3360909435016d2096f111485f6d320d318bc5db502320cb1f4b77d3963724953abe106a896cd24199c4cab4e" },
                { "bs", "3fe687426ff225c113e7fd0996e4b2b499a0bbb954ff3821c77a8ba098b89e31f069ae6c78d301320f131c381784bd641b5e6d77ae97cad39e770810b15718de" },
                { "ca", "4c0528ec7cb2201ae042cebc5a67789449f2239e5b2a4357e312a507e6a351f319911cc735dee175094f287c576f10dd2d62d40402b2c1d9b03d1d15ca592ded" },
                { "cak", "d6f6f16d9b80554b700831366e9e63c45365f7eafc76440a18b7e34f5e6c06b5bea07108bed9f91a8c75dcb41212c69ad6f4727f44f03133e74b0e43e76df3cb" },
                { "cs", "805ef04ca0f1d1cfc627d053e819395d16746b24de357f51439d2b5961b44056411b8c1cdca5bfd3bb41c5d0f16e73d8e0cfc72656afacfcf550bcd95151d573" },
                { "cy", "178297c3e84ca8a4ccf2088572306058b3a408f7e3d6f3ef100cc6b89b46fa24c3b55b347fc7c1490aa074f133d024dd116b75cda3222c42d24efc5aa653670a" },
                { "da", "a067cd5cc7e14871ade2cdd4e93c1deef413c3a1ac556173c023cb6a73f722958ed9d1329dca1014630a47f4aef968fc07934b7e3bd2c6b0975d1a29eccf346d" },
                { "de", "178f4df5dbc75e82b246e91b606beed438b1a30e6b8f86c8b86b5dcb3436e2483929aa76c1e7e073ea6e945b5624a3b6e6c59fcfba26176c7d9133ae4ef9d310" },
                { "dsb", "8cc380bf90210c2a971c4913e35786479cc22b4625267387dc77d0056f25e8323bef43e18653af731bfc6e0395e28a391104b23672019c31fb0e65e0322165ee" },
                { "el", "5a6fac3e1876f5327b601c628f6e64e97c266c763ec15bf2845496907e141e2b4378d28434194b204364a5f9442fb016008028853a747d22439f934f54e88403" },
                { "en-CA", "b793afec26a817fa3bb3fcef2ea32ccce2a64600a9458d9720fc037ee60d93bdc2d1867ebb1e9f087ee9adc28b6fc966debf7af5a9274f16832327018c99e6f6" },
                { "en-GB", "77d112785163431166b58d424918864ff03f81083c67fcb6f2518a1fbe09c209a5c2bcc3d381322d520b79606bbc0790cc54ed0903ebc015d9f8a68db6b36c1a" },
                { "en-US", "b38a189872b575facbe0a5b9d587482d22bed651188cbbac98902a527dcb7f439afc4d0f887b9214e6a82fa427d4a3d217ad7e2bf92519f3f36a67f0206be88e" },
                { "eo", "18729f27769b770ae4b5c503451a99e7664f52b5199f17a77b2f28081754c0c3c20d012c0ca59a7cfc77131680b84e6762ce074dfea08805a07672fc8479984a" },
                { "es-AR", "e057abef031e64394fc4c02e4fc104ec876836cb3835f24e2a933bdfc7a14b4bc74e9bb539b890b7faed538e255e31a6f0ce6487543cd7b35d747c3327d1955f" },
                { "es-CL", "67b42661a871aac2a8917c80f251b71562fef9b9e8c25df95a4b9ac5ac7694c36dbd0b9e67022b164bf020ca1dde95b9da51f58c73bad5bfc3e01e1c7ef402ef" },
                { "es-ES", "8c09bbc5879f72cbcd839ddefdea21083380f59b8c9744d3c4dd29118e44860987d7d73d9773ea2e758bcc817a5687d00bf1ab758f6b700b49ae34c307287871" },
                { "es-MX", "f60554b3d059510c538b145103c34f4305158bd64f77eb8cc90a2e67cf5505b02f5b735cba0490b3408fc1156a2a93a734a61404e949b9fe84740c18492d37b2" },
                { "et", "943ce83c3fcc98604266c491a093265679ea53e3b4defa9802f3a57231ff1842ad59651901c0b127a6d7e3985f10e60298aba7f3b40cd4dab5550df2f0e7dab1" },
                { "eu", "d0b0e2ebdd958aac7047148a1f19daa994022c642a2d1188f045caa1ae524b0e5509e9405810e210b3ae0e520a612912218ce70ccf4bf233615e1d3fee2f43d6" },
                { "fa", "d4718c8f865e2847881561c7ae585ee795a251d1f09bc2821d95dee98de5c54db38b6b1cfe14306cc854498d10acea812173f55bba44848e67298e77b8379bc7" },
                { "ff", "3d029ad3e6f0ec02ae9118a982fb7aabbf61584b600bdb71af43e79f5f99b3231b9d9d13a724cc94ee61cc8ce9653dd9ea5198718d47dce1a7e32916824dee6b" },
                { "fi", "df73710de42c9821d0653e86719f9c9837f77fe9fe1125cdf620257575522ab6ea56d0c28a05a32a380cbce967f77a52839ad602e47f29cbcf05a678af605cf3" },
                { "fr", "955203ae4cba52ea5f9b3b8f06c2c1e8ea086b5aabce52f6cf1d5a3960d088da282d42523a2ccf52ea077944778ea60de4f8d86c10bda8ebf69ff042eb1e7492" },
                { "fur", "5b974f0b6649ca8576bc1b42e599413663dba46de07cebd3d812e8ba63108bb6a0be518a49c947e10dfa5d5c6a8e0b16aa720b047a64a940b44031007dff0124" },
                { "fy-NL", "b7982b3e883150e777b9b37482c5602ef51b549b17ca2a3e37f8c4feade093072fa5404b7c167c02f60f7129e86923d3ecc9ab6a22006a5d9dfc700d72a668fd" },
                { "ga-IE", "a0e10aeb480b2bbed05b4a6b01b3078743c42f5d0af3680bd461d633c273c60df4f21fb9902ce6a6c65ebf128a099bca7084cd36206173f9a2410185b63e2ae8" },
                { "gd", "6030338df45e94939eacc7804e348bede9eaed8d49de1fafa7f7f16fd9233db9625c648c10278ebdfca97ae5b151b4a9d2fc7444ef7289c426848043115b3965" },
                { "gl", "26b299998e8b5df716772cbca742fbe1861b3bd9f39013d3db97a0cf53fdd2c85c7a9e35b75de69f6e1e84f53dcae76c2700049106d90b5d41c46be505955490" },
                { "gn", "72ab1c35b85eb325b197644e30db8063c9002879af97570a11bf48d3613a4a37c54df625a2d38ec9b593ad795bd20c312a31b8615bd2ea90bf9fefdbfa6a21b3" },
                { "gu-IN", "3fab9e5657b563c4fe413c6a921ccb38c3ddfabfe002c7c1a28fcb7a558b7ad53048776527d07d232ce8c938cfc8a4bb5b1e5e7003439ebde388ab15bd5bb999" },
                { "he", "c36a212c40dc9de259f40f47f8299c295b3d6219997a87d30beeb6c9892bdce54e5acf985666f9952162158abe2efe2ccb3fc9c2e256efbed0625b6658c535a8" },
                { "hi-IN", "35a0c733c00d6083e853d8a67e5943a7f503e3f5603903031bb6c73e9034a25fa7bb9d282e55e5c9ce4a3876c5f61d914368c0fd79f01276cb513a34cfbf05e4" },
                { "hr", "9643b654ab816ef50b7435400a9956dbf392e7d35c69397e3fffd564fd17be7214c2a9544c45a4b556fa8877d66ef60bf1e15cdbcbaf845976eb4e64c80ce088" },
                { "hsb", "b74141fb786f35962a9ad404dfc10bc7bcb2de23e5e55ec725c1674dc79e594fe6b41bea8bc5dae86492cdb2690e3e22099e1a80cde5b23ef7d283e6e15149a7" },
                { "hu", "706116bd469d7ea4e21d9527da8fd9ddda520920b367be8a46a536ba01d4b3ba0767bfb62022c8a822c65581ccfb47587c29246f6fb37099a5b114d3b5a8b6f2" },
                { "hy-AM", "28653bca054840abc904c1372468b011e02a6837be33b916b98f632e35b7c8fb29ec643054a94017cbc74be4b01c1c990a2457d139f790bcca944adfb56d3bc1" },
                { "ia", "cabb6f208d4c93bd1c9d9e62212e1434f26d69e94614f8c2ad1a697486ab38a143eff4dceee63f6b3115d90151cc9a756d2270d515d7ae9e54baeadf4425d62c" },
                { "id", "acd9536dce9b49ec7e3065cbeeb3e011edf92c291624dca5be9bdbe7e9deb4e8c976af29342125d2ea8f9b72833eb1a5dca587acc6343e01abe1d4011f9bf42b" },
                { "is", "a940e6623593cbff8c00e88ccab057f55ecaed0c4c489455c7aa877165ac6b5964e3007157f45c2bc72631247ca2ecb5b586a30b46540bd55c5e991f4804ca3a" },
                { "it", "e737960fed1212b66d6a2e8253ab8f3edc9636e41ff12249a09346b1ed582f49271ebaf48dfe9d2081512f4b6484b2e512e28904cf903b3b619ace67a8ba24c9" },
                { "ja", "f0dbff76a16dae3a0c5c074d54ae86b134206f0c58b78ccda1fc858e5ef73cbec387bcedb3c769e31fe97031d675dfe203e67ff0df27661b68b54fcaf2ea7b7c" },
                { "ka", "b500f4db5c4ff4883cb990858e5b75b195872cdce92c68c44b29094fd88b4de6ef54bf62b1894494ff656780cad1e6fc1254634b81980b58dd9203a15095a69d" },
                { "kab", "5cd1d0104edfc31e6e9a71b763fb06fb71389aa77fda02b82a4fe9b81249c30f6e1aa7ca1ea2d12bf92322b5aecd276e1891879cb450183b725b633c605aed6f" },
                { "kk", "d01f36207dfc667e5f9afe8797d7eaffbdd80f2d19b8841c48d4c2d38ac6a7a654558be4ac38033b0c5216ec12855bc011de7ca6e3f24b64b94a959dfbf88a86" },
                { "km", "66ff35998619a62ccc8bf75e7a6f4e81e50e16ca1c9fbe5a5d1a353ac2b9e2ac2392a06bfc60a7d6510acd40c6e7a3023bc8c6af9baf208685785711b623d1be" },
                { "kn", "b9330ee8f6c14649366e1d1cc2b0739f206f1c141002662eb83d68298d2a877cca3f40d1688c88dc670c152d011a078277cec446a062b60ea9e65e2d75e6e909" },
                { "ko", "2c73bf44b6190797c9fb4c1658dba8fb063fe6a63cb03ab5084eb28b59e19be0c1b0037178ec7c5b328442660dbf673a2e8ea334142143c0287be556f7fa96ce" },
                { "lij", "f3f2064430eaeccc065e816f83d7d332bb31036248465cf57a88584e52bc54d23672776ce971f26f26f4cb3747bf4d102bc437f30bf7402275421da361311a82" },
                { "lt", "3c55492ef403f92fc87330fbaabbd5b9817dd547cd83625d162e64cd61269a50f5c8541a4dbd57e004910c718828a1f336a06f320f47fdb6d3d7e23b492b2e5d" },
                { "lv", "a7bf55f1bcc9f4314aa357dda5a13cceebe258d85656a49c85b935b7a5edb5d66d52fe589dbfc23eacf8666e11f9a7ed34ea904b44d744409a82cefc76e749b6" },
                { "mk", "0fac3c0adfaa299ba04591a9e817341a34bdccc43a570a71ade395fcf03532988f6aec47125feb326d86ca15488f75ccff75a15d07a65e144b1a778c6c625795" },
                { "mr", "2ce21c58fe3d72666819cb1beb91203f205dc67dabf3df9b696a4cd13e1f58f1f0b9276e5e1c61ed99dc1cb3531bae785b5e6a88dee6f095cb77ce930c684ff7" },
                { "ms", "68b83a34d9c1ea0078f5da36e311d9de20e7c4dcd17fb299f02afe9a5ec326b47c372c7f607dc1e4392b1fef1d088f28e3bb954069197a7a375230e7b5e4c019" },
                { "my", "e669ed7c11cc966ed6bda341b5d8942ec1a87c2990b6703d638c29916923d293994233e69ed235a2f3c79e91040109ccf870d7552fb8021c66b687c1e8ebc277" },
                { "nb-NO", "bd07cf0db618c651cb4a7945d36507421be399f78cbb5e59c9d7211dce25aa203c5eff93227560f07a43c1286f8e6c0a261393f4dc7dd63619e1a419a9d417a7" },
                { "ne-NP", "39700ebdf5c4b5368d8dfb630c053ea5e60e80eab99caa919f2d8e9b1a64bd134ad4a70ff8c227e35badcdbc0e8d13d7e0e0de08888f47348370c895f4036773" },
                { "nl", "7d96554837fc8598c9054ce5fbb17b3c8a124b5db8d801971c91313c7fccb00f869d6c2060b8b1a2183c4bacc14dea658e333846d4d6aea4f68f14766db72f69" },
                { "nn-NO", "eb082628a15ed12b59fb8816a7ce093132042325518c48a8002dc58b70d12905962f234da38f010593cd073c4219b33bf4c5a0c2ce3ccc816629592a7c4c3735" },
                { "oc", "30a29a8cbd2807feeaad914bcf1f551ba17b95ea0664963d2f4a98f8ee5d55d84bd65d578fce59095bb4a7cf0750800490b69ec377f83ab6543d2c475985d91d" },
                { "pa-IN", "d25af8ded23ccc95a90e9f721703c59b50a72738be89cafe5ac6e3f7db7c39744ab3085f87892dd11d07f04d6e6038da08c17edd8943d950aca7fff602898949" },
                { "pl", "1ea71cf19f607605c982e5d1ac1985bfa2a4e7e8f99ef534a8589f90d3a76a71e54e2079a2040db6ab3f210bd2835fee9bb976ebdab96af6f35596a7690b35f9" },
                { "pt-BR", "195f6957ce1116bf16506fb624f5a1ee5afa62234bbd484ac0d2f3a03a4c98dfcc3747b1121ba8a7a62b57800a5e660626010824698ecc80f9adf3ab53f81eb9" },
                { "pt-PT", "c74675d5694ac8724d83dc9b20f23acf76341d286feedac02b0859dd7fc65400d4d47be2452c132a5606bac3682656d39122aea673852a4bd028b790678cd559" },
                { "rm", "ba0ab224c4b424988931b5841f0f41ac84e684851e6525a15df5fa74e28a5a100e71854ff7af58375542a0f4a052e671e91dcdd229f0f8892db076c8ecd3b7c5" },
                { "ro", "3341397294eb141847d68b252c5d5809b53335ba388047d7eea12b39d34210b257760461bda66817727551746c1a124cc00f2e4386fa5afb188fef7451862c34" },
                { "ru", "022d325b14338eed47bda45625e5e3d605e1b7b71d86a819ad8146bc288d5a49f97650904ad87b2e9b17eb01ccbcf076ebc44a1ebcf49b617887e30fa646ea19" },
                { "sat", "2202b7c0a76f2ba151b9360aa80760f8fb7715c9e62f36463465d935c00ba518885643407fbd5161b3cad893bbb440b8e3532707193284c9c9ed6169f00bdf3c" },
                { "sc", "6a9c456e16ff31cc34ff5ed25912ba99cb276c2f60dd64553bcf70524e569eb1ea8e9d339aa7403d2fb7e740ed37ba3b2c2323befc3bf2ea63741e468cf761dd" },
                { "sco", "fa1b17da491cb99584b34dc7d713d1d3c12aedb5e6b2c5e0a76ab10ca2e164ecf71a8f0e7484b8641f9082c5fe1e2ac036ade59fe4e6eabbac595af3c11cf546" },
                { "si", "98e81883f8ef403b75e5f32373b1593c6adb94dce9ae58b60fd8da6e88c20992ae966c6465c742a03afa469ce6b1c148a5eee686b1377b37f6b792e59b78e7f8" },
                { "sk", "1c5bc20e4c64f316e6f65073a5d3ab94311283f07b33b59a36761cec5a39c590d6eeeaab8698b11d8fc2bc0f2a6853f211973f99475dd59f39e5fc3350b7cc5b" },
                { "skr", "6d3ac5693288994bf54f0c2553ab255dc5f9f863514d906e90ea3ff91a384e8810075bf1220064d75514327030835c8cc983cc7e9318c6e6a9501d66bc39e28e" },
                { "sl", "bb61b31f90db428405ade904c619e6af913b8459d032ebf7167fd004255fc0f8e0491f4784286a35bb44987ae031ed6e9f0886cae121c81f89c3cf7c652a51cf" },
                { "son", "b28a460cc2d6eecd57f379665fca90e735a768236e579923b48f37c74a91d3942ba3eabe80b03da426e9619af30cbe51d5bf1f520121b0feb50fe4c16f2873e7" },
                { "sq", "c9719db37ae58edbf3c418022bcc9c3427cdf57dfb557170843e5665c288d40718d9ff92c4edf7a0e3bd93abaca020f3405a713741fa96f5c06e0ef4aa453bcd" },
                { "sr", "942f26951fb91558017a559002b63be73c6195cfad633415217043bbc3ea3d7becd4458d3f92cd55c161f25e6ad1fc79fabea90d0665a4cbaf323cd97818ebe9" },
                { "sv-SE", "4d955066a256c2cd51e60405fce882f7e6ce423ae3610072f32fed2349a89a99dd04aea6ac0c477e1d8842039c690ccf5b19777de1610b1da99698ab92d78a54" },
                { "szl", "3cf1a35809442fb1cde597287fb20daa6796399593cd5775e0311e308f30351e5715c3916ae82cf7b994927cd1568a76a91edcfae9665144f7b99db0dc99e60c" },
                { "ta", "0ffae3718d3b9f71400f208dcbad0189fb96cf8ed81ee492630edbf5163a86fd7be7b9c8ae8f7f3a62c088fb4db7c4e918f0ffd710da335b0a7fe08e87056a26" },
                { "te", "c017f543b3328c244b8dd186df75a8d9750b64dbd0f0ca1a0d0697182ed8c623e02ad36676607a9cde6186093e1ace0177ad4ce02e67f084b9e22b099e24f31e" },
                { "tg", "f065b194ecda81327fdc89d8bb6c75aa59ce7f44c7d1db0e83157af0e9659fe311c9dcee2ffcb71c75013b6035a94e8e51615da3900b0f71cdec789395e7b244" },
                { "th", "225408ee1bae5f1de6424eebb72cb9dd49fe6e907b2f27c23084a13bfa9b76e740e2d6bb62f2124c043e65c5ea8bff6da2ced1b2bd1b2beba49c21e748de608f" },
                { "tl", "1e8fbda2abd4153c4cf2f60384494cb1273eb8c637c8519c044878b7092d380fc642d04456443ff161ef93f0eee17f20afc367d450b98b644a0cfe6605f687a5" },
                { "tr", "9d468d20862fe153d20e7c29325a12e3d4dd32e5e22547288e380dd0ee79cbd14f572e050c51940397477f931ddc6c37cd180fe82e4c614d3642956c35c8fe8a" },
                { "trs", "be5fa63fcc5aabcdfdf7ea5f8528e3c8e4805a7d8f688f6f0562e038b89d0eac70825b057c39472054009a0b99c641a315bc7d1d4dd05219dc2a77b1de9a0f36" },
                { "uk", "791c39e13175f224666126e694b52216a56cb53697966baa696862511e5c64b0622447771fbb8685b0263104dca04df11a548ca575aff606e1149a71b356d9c6" },
                { "ur", "4ccb82133f1815bcad39a5e888b2df9bea397fee01b350969523042f3c5f84ad64b9d47ee1f5ec5d5085dacd87bc4a1e83839ab1ac58a0d85f84c66c4bbac95f" },
                { "uz", "2b03df6d0eba4f49e78006c51a1014ebcc04bd0ab3cb2fceef888be72caf6beeebd48a5d892cf28f8a59313e2cbd871d14a7e3f73bc639badf377afc0eec913d" },
                { "vi", "87873423ed0a50578b7d819819e1f1dd06c918c83134843dc46cada9150e6c789a77e8bbb73f15490d534503759cebcc5fc4e49039a73f615ffce5aa3053e52a" },
                { "xh", "0878435609d1ca8746935567e3440d7d651a9218156e441309c0296e80443b9e7b664cc319412679e731b9d4c172810026304f4f205898dcc56c2021a009ab88" },
                { "zh-CN", "bf02ed72e7a5cd8a7e4171409bf25ad09e0a8434eea8ecd352256e63ee7d96727c4fa1ef9390577a1b813d756fafb7ef69b83519f13c25f77fd2ed1136e27615" },
                { "zh-TW", "f12165ea8bde3bd49d53ac2c29702e311fccecc74adde817713b7d26a957ac22f06e8b4ef0c7c889daeaed59f69089a22a7d5c3245a2c67fb50d9e96b34ea86a" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            var d = knownChecksums32Bit();
            return d.Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            const string knownVersion = "132.0.1";
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32-bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64-bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
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
            return ["firefox", "firefox-" + languageCode.ToLower()];
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=firefox-latest&os=win&lang=" + languageCode;
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };
            var client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            try
            {
                var task = client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
                task.Wait();
                var response = task.Result;
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers.Location?.ToString();
                response = null;
                client = null;
                var reVersion = new Regex("[0-9]{2,3}\\.[0-9](\\.[0-9])?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;

                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox version: " + ex.Message);
                return null;
            }
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
             * https://ftp.mozilla.org/pub/firefox/releases/51.0.1/SHA512SUMS
             * Common lines look like
             * "02324d3a...9e53  win64/en-GB/Firefox Setup 51.0.1.exe"
             */

            string url = "https://ftp.mozilla.org/pub/firefox/releases/" + newerVersion + "/SHA512SUMS";
            string sha512SumsContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                sha512SumsContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Exception occurred while checking for newer version of Firefox: " + ex.Message);
                return null;
            }

            // look for line with the correct language code and version for 32-bit
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64-bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return [matchChecksum32Bit.Value[..128], matchChecksum64Bit.Value[..128]];
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
            logger.Info("Searching for newer version of Firefox...");
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
                // failure occurred
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
        /// language code for the Firefox ESR version
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
    } // class
} // namespace

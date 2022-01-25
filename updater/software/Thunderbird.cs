﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022  Dirk Stolle

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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Manages updates for Thunderbird.
    /// </summary>
    public class Thunderbird : AbstractSoftware
    {
        /// <summary>
        /// NLog.Logger for Thunderbird class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Thunderbird).FullName);

        
        /// <summary>
        /// publisher of the signed binaries
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// certificate expiration date
        /// </summary>
        private static readonly DateTime certificateExpiration = new DateTime(2024, 6, 20, 0, 0, 0, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Thunderbird software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Thunderbird(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException("langCode", "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode) || !d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 32 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/91.5.1/SHA512SUMS
            return new Dictionary<string, string>(65)
            {
                { "af", "76632ce3251284fc0809a7da286013746450b11d1ecd0c58e6b3aa3db65df995db6fe32c4da112c11c99742d48271577fc551d60e05a1376d280a33bd9f9f7c3" },
                { "ar", "9344de3df4d817d85ff972b8f56473cb683352e764f7ab408e975bf3aa4448b6b0c3f518041ef3f46ebda4f7d14198efd70836a444b8adf1c3981b306c34cd86" },
                { "ast", "6a72aff3844cc8d75040855f62fa7fa591bbdd0428965f395fc2c49c82b7a489ddd429cde4ce84a1b6bc827a7eba997b1607eb0616b4fba8bfba0935f36c9491" },
                { "be", "4076b1fce00074bfdf18e7a6f81394bca732a8a0f9e23fcf0755359e34b2f2a29bc96f0437eb443d768a4b65a0e6cf84f2036038f246b484c7b8d59b4aac0a5b" },
                { "bg", "f10bfad3d2eed1d66bcd58f279ab93e39063a701927687d2dca80255cf8b648cb4afddc179ad9ee6d7a886f3002300bf621c83cd455cacb12a323351f19a9dfa" },
                { "br", "8ea11bb5b54df8c7936993b092c310b9215c9e3d8a306a19bcc0327f0fad1f29fa86ee0ef405223d5223cdc974f31e3cb2b864873cc7fb354e965b095aee0ead" },
                { "ca", "d244beed48fa3fcbb912755c76ff5331670591b48da68359437490e398b740e8f687ac01fb90bddb5412bc1bcc73eda559daca4909fae15e3a29ba9718c60667" },
                { "cak", "e0263e0108e3a2fd4c1416106b053102dfb20bbc41fe4741429d9efa655fcecc61a5abd58da708a2a8da4942b76f7f4a793a6b4ce54da034ce3b376b2324de8f" },
                { "cs", "b5b0471839923d6f779786a99567d7f3e089ccd82abe82e3082b309a2ceed20979c8c4a7c7862d8fb619ccedb123eedb6dfe0839526a50338b147b7b95e6a1aa" },
                { "cy", "45762dc5dbddc6ed451f86ddb36ea7358c282ea07dbd6e51db14d7a0fa002417c484f308462d70983112fe0d57494bc7b941a82fd15f77141dacff8dceb9da31" },
                { "da", "ad0326710c0eb21ee9b0fd5c06f7770489cdb34212b137c39c7e0a4c2b10180e1642cefc6b735981f71d2a4c8e893538085269739b8b13f01184fbfa5312713c" },
                { "de", "58ec59821dfcba0ae0175add821c7d357ef50f1d9517332076b96bd94437ffc98820e494e592481cbcdbc4b2c509bec01f6ea499fcb4dd83b66b314187e018fd" },
                { "dsb", "c77978f495c603005e727d60daa12fcb9658d425fe670fd20ac1157a8b3f52420c91907a51f897c23108acacae04e169819d0e048eba8b82517a2b143b35b3de" },
                { "el", "26ca7e36eefd25d4f8efe7e3d12c79b8156b3bd2774c72da85d65fd82807a684abe975e9ebe1984525728466750586d3dcc0e1a65e91bc90019acd0a9e903dd0" },
                { "en-CA", "96fb92f3ebd89182146ac03a8873a10fa4fd8402e7c230ed7336a7366dead79f04cc61ecd2ee7aa53e0ad5439f5e2b105b500e3d11dd1d5122adee70098ed282" },
                { "en-GB", "73b6f2d0f2c02a5893499328b069618cac36287a17a91fe0a2043ff760265af76873cd798c01a96c1ae6c6155fe9b25196cc2ef659bf2850daa4a41bd543575a" },
                { "en-US", "3ea7edb4c5cdec20f28047ef07f3b861026602a34a1207b7bc214fe2c64b3790a33e76ffaad4530332c41d02c0bf6d8b67ba7b9e3c9b52754771063adafe5b51" },
                { "es-AR", "c5ccff5a932abb3a532f1c02f741d7086ced5885570c939272095910ed98095fb28a5e8ea3c30db78014acd0dff0662f26555089926c568e92da71c7053d639f" },
                { "es-ES", "3916fa9c41bac730c334bac60144eba6809d026bd1f795101b6850cbd28c49e981df3c15cc50e73de91db31297ad66300666900f70f7a3016ca395f6be365cef" },
                { "et", "bb3040fb1ae5cbef817eb56153a819f426e73dc46f4b585ed5758fe5c4f8dadd5ed857ffd67b332f2b58bc433e055f7791c172768f865a580ff6a201891799f6" },
                { "eu", "eb3a89dca966c65759e6f3f2de74469e64368172bef29cfc8ed8384583d9e29baf7712e80ebbab2978b17a97c4dbab210dedf17b2475b51a82bcfe475cfece7a" },
                { "fi", "737f43e86b983a44725bd6df9a14d8235885dc2e6ccc184dc76d935af177e82380ff88cbdb7ea4180012ff094952253a26bc7c05bd515b63fdc580da0b3bdf06" },
                { "fr", "7d9382d1ba682f59aed5748a98521112d5d2733a9821886c77182e074ce07eac92c5d86b64451aaf5e428b9f0ef920fbf9b696c82b60ddc97b902127fb648c26" },
                { "fy-NL", "f6589198fc1b542d2ec6e95a294e61312b90bb7ea1eaead12de14703a596f9a766c60b8da075bc454a079b0411ffedbb16e76fcc4070060dc56784e988a9ff8b" },
                { "ga-IE", "e1310d9215811a404d1396a2b11145f662f54ecd0a442c9d266aeaccec1aa0b98a4d8a3d83ce4070d67b38c44fd44a94f6322dae57c0c56466f3bf6722ee64e1" },
                { "gd", "2e9c2d6ce9cc72d970ed41f4f802a085136ff5580de3db620da2ea16ec1de864c4c2a1ccbe10deff726e6fff5e51bbb4b6a4534d394b2aa316ee2af27a68c5d2" },
                { "gl", "b54184384c61c32f2aee825470ca9de23d6b98652fbcfb50226abd2eb38cc34390c181dbca3af698d112c012d75711b2affcecbbeb1308b6cbc4f89b7650fa07" },
                { "he", "5da0a115045e331ec07bfa9f2dd61d2e6dee087fb2d1d6ad5b193acd64a383163eed919dbc98a5387ba1aeb41e636d19b2569fd30d4e81ba7477ee813f4766e5" },
                { "hr", "d35d448cba047cc7da18b810eb5384f302ce4491f849fdeeef2c91cfd016bce37ebeba69f60dc453b768bfd066e44343cd9247045a75a8cb1f36912de1ff9811" },
                { "hsb", "3ce7f6024939d20bbf06fdd22fd24e77fc087c4cac0c322adc15e28fa224716798652036045a21eed907b33e08eb01ec680623e4122d9273030f0e5e3bc8f92b" },
                { "hu", "ca8d5d75e67dd27819db876b947ce3971ca163e8f0d8b2963cd864792f49c8c3e2d3c669f4eba7063162733c7d98b9b81fa982fb032a053a102517cf7ff981a3" },
                { "hy-AM", "9ff032ff00198048cfaf3f24c25d7e26e8c75382a1e253a27da28df979563eab8fc4553d782cdf3c13e942536dcdd95bdf7e099091e3ed0c38b28875c7baf7ec" },
                { "id", "673689bb78b82e1f857391c043d8c5ae6e66b80d9563f386f42c8ba525a3b4941df96245999daca571e98fd18006eefb1f47281ac8bb0752966090eebabb0858" },
                { "is", "6a96bb505a5e4afd4259bd31ed891f777d0cc4959680172f4288dfea4e50485e52c102f24eaead9c37abce82980890437b929645b27af4bf8b6602dc4a398ef9" },
                { "it", "d8383ff2bfc83625c62e0187cc05d072cdc356daafc3e72ea64ec7bda8ffe235543e12ea8261a2b94e441bff9180888acdb90e323248ccc04a124e183f7a236b" },
                { "ja", "06ea3119781f4a38bfdaf517b36cf44278ceb1d8b17b9e5a91e1c415314bf7edfa38a4e01bf8cf09ef2b479d29d626778fe86522e3011dde4e1b0c5d6a4d8430" },
                { "ka", "0d34ac1b8d352f249b848b1b6e07e48954a045341811a00fe752949503434700a553d6981e8f2d62ec98fd8a5fa90e304be588173f47187eb48ca92be04afd1d" },
                { "kab", "55b6b49552e528063c6878a48736e63f1b0df83fc0902a637e6343365d7a08a0a8f710fb7ae7fd0fd26d15128bced8f51cb47ed61ea96b28be5ba524963fb704" },
                { "kk", "f276436c4650d9ccfc8c2e056c78e5042c203422a2d3d9ebd73348d36b5f4ba52da9e615185b89659ca8440f48ce57e610a96577f8efe96c158a82d9b3c1d433" },
                { "ko", "5a1d3bc3f51c02807f78e7a2ecf3aec9392b81302561fbebc3e14d5bc47f85aca3952ac7030a9054d27dd984873a60036f6fe96881106f5bb7e126eea806a7fa" },
                { "lt", "420ce4a43eb39e4abe208ae23fbe55a9c1493141cf770959ab4096e2b10874f5f2ae239fb0fef90474180894f3e91f05d0c033d1562f843a828140553f4d0f1c" },
                { "lv", "ba5a30b982ab12177ab65e82c916463f587bcfb232a4dc3cbd4074c3cda0a3fb5102d99f8ea47b1480f5ff21e1b547f0aae10b64f0ad83144a34057db8bbf053" },
                { "ms", "32aae5b0aa75a296dde1bc1486aa84d44120952cab7d9fa20fc667b1e68c1f8ca7f6b8882bde9a0cc8a323009a517d63970039196e3e2ffc0cca460a79dd9e7c" },
                { "nb-NO", "acb6127c2017e7941c0e6ab0369fe4d2a9612e290f5f55f426ee4f615c354b7237bac8a856d8a59d7f205795c7bc6aa6a8abc002d1aec979929cfdb5239991e8" },
                { "nl", "6637e20e2d2d46706ca3d7fa15efab225b4712b68360980d6139f2d468143acf20eedc954a4978294cce01b3969e67008e522a26e8ac1a2be1dc0763572377ec" },
                { "nn-NO", "1720997df5822209b5f1ed6af89cf67e29805c2e319f3a3682c5b64a5328ad4df7e31f80f4206b1290a8d76126b9747119a9ba064950accd2b97c373c9242f9f" },
                { "pa-IN", "37764cf4548f13f456ca64b054dd68bc37a192f53e6bae28678e9ee3dbd53493bb34641786ac0db95585f3dcfa0a71f0f38ea8862492c72ffc6269f7155a931f" },
                { "pl", "ef068f81a9fade498919a0302053f777bbffb6c1cd9db60ead866fea838b0a29456498ba82215a3f1ee41e69098e9495bd9c10224d48f706a6a2e1a8f3ad4fbf" },
                { "pt-BR", "68e795535775888474541d3324ccf6ba8ddb19e70625009daa939a3cd85804ba1a1461069f7d628c8160ff57de396c46146ec403ce2a1108e5311fa0599f6f1b" },
                { "pt-PT", "5a113fb0b3c7f24e2aceb8cfca3e704b14479fb40a32ff1f79e163cc4ac5be06069c12bcce391fbae67085deae75c319f6ef0d1b60ca97002c62372812b83897" },
                { "rm", "7381c6278559c6e334be04b2eae8bbadca2eae6759dc3838ce8d7dc7b62eaaae5ca5fa006f21a7af28f373c89f58744d8057893f65bb7ae6dd33a26fc513828f" },
                { "ro", "1811dbd090c4aa9fa82e6f44c3525fd074746a615ba15acf80c6442737c663e8b8ffe84e07e998173e80a6ceb99011bb12a60ae54a6955a90d166e503abeea32" },
                { "ru", "680cdf70be45e569d6926f1c4f95121a35fdfe90d7cf99eb57600298c2576d4149717c2c29dad2c0d7baa9c06ef8932c7376b98c5f8003e01164578c04c6c9bc" },
                { "sk", "b78f3fa57c1f98f8ecb791f65d2f41a8f2510f521ac30cce1516c70be9a7efea564f3f075f3049237c68a5ce634c2d2c198b0865573116d8640a4b4108977ca5" },
                { "sl", "9bcd556af15cfb295b92b242bc18dec1366d92cda4b3d17419b9b380d9b9fe106e935d2df39a175c9b61e5d1b78fcbd87a9421d4bef3e05ccccacb415fc76193" },
                { "sq", "676d604357430f84d75a107971ab86e69e9d62f86a85c1ff228c5bb7e59da981214c791f200ea4c38e7120c00243203e7a068a9c83adc5b15da45e087b4391ca" },
                { "sr", "6e4f0e46afe0522613d60e3025e7deb3fe4c2de2a2ac86c84b1f34b8bde442b759cf1f3719d4f52c68ebe5ad2682d2fe68f6e69b91b3133238e95c91a700d16d" },
                { "sv-SE", "485ca1689ab1e47f8ad0251ab6b52b6e4c78e4f8d5b5d91e5918e660f6399219246b6cea5b7abeb09aa6228ce98a873fda1d77704367f3a7d7be6b8ab82e62a0" },
                { "th", "f649fc9ff089d296b55152223b099c328d68532b61a64f1249bb6efd1af4cd027680e8e907af7934209c76f8e5811243c5bfe50250d06f5b008f3b293b0e25d9" },
                { "tr", "d236873fb74f25892c00b1c3588d8d6651c3218625af0271ce74f947d118f583e68250d74357a3e379c8bc6f7930ac7fcf6f5b38ef79dbc7ded67ef1c87d2e9f" },
                { "uk", "ab868f29311be86c7b4b25678be370d2622fb7a36fc4ff6118e7f8d1b9ff440102db212b8bda7df0d7de970f09b47838b266ae97914d3efc661c43bf6c6021b5" },
                { "uz", "abb64522e194a08361d8044489c13e42cd36ca4d7d62aceddf0e8ed313b3cf3454c087a3f61e0c7d3b4be1c2c2636d37c2b7b3bd6e4570e6dd4054e84d8b1886" },
                { "vi", "742da45b1dbf79af9b81038e89b9713c7677345d69a10708652082867a1d9cd02962495653a851fe9224b4e3eddce03ea27d090c0683c0f377ec5968281c3d92" },
                { "zh-CN", "b24d3d3846536a551113a3c1dae9dff8aee5ce98490c38189cd7e13a0a8250c11380d1bbf13d6fee9ad04b5f93291d57c313551d178a5284afaed4e66b1f0618" },
                { "zh-TW", "26462ea9ae8cd9fe8605799f6546fc4c0cf8ff1551c200a2741dfd7e57950f9246aa9f530aaacbda5842f0271df65cec7520fedcacbf994987268cd90a910cde" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/91.5.1/SHA512SUMS
            return new Dictionary<string, string>(65)
            {
                { "af", "6236b27e6c0c709f0dbebfb04ebefdbfd111e1c89badccac6b13491e2983f0d5f86ce73d61e40afeee8e531dea4537c72a6c9c4df7cfa35b38d2e5f13785285e" },
                { "ar", "02ce24f3af95560be3346b6ab8305db8bb09f78bd380b68a4cd7b177b00e10bb72315d4710c36d341035d8127bde5cd200954a5c12e200a863a6ca8dfe6d5c4e" },
                { "ast", "58874a003f1bfc6ab32a8a6c9b06f300f6f15adcbea49ff6f06d6bd38d6789b7e65efeafde1e4ca2bfe763d8b730938952eb1ce8affa19991692f97b7f7349f7" },
                { "be", "f3d7a0f026723cea51b55d081cc7351e5651b35e1f28dace05c558b9a2e2378dcd1bd88ceac4938756563247b60b38c930aab11bf1d5cc6096574296ee9e33dc" },
                { "bg", "ba7f4fe955d7857cd40205ed0adf87ae845a1a0be4ba0cad7ea30609a93135685d7a7ee1738f56ea63e8cedca8bd6d353339d5fac0473be4e68d78dc2d56fcda" },
                { "br", "8be94267a70eb260f6b2e56131a5d23c3be9a8636684c77d78c58369ac2d8259fb7650a1595142b93e4a2df7ab36f05cefcd92650801e99c905b3fd402f019d1" },
                { "ca", "e7a4d3ba74af9f21f1aefaaac40dccdefaa3146b166b23593b4918d380851a0c01a9e7954424639aff74cd055a3b7fcf3fcad4cb9e64deba4b997a1e84dee1fc" },
                { "cak", "9798f1d8499b43643a3fd72620aacc1fdfda2acdd12713df9b2deeb8c1878e0cb419963eba4d93f4b50d20a9628763e643cf76770f58ec0d98b6111167e29a95" },
                { "cs", "8c142ae9893a66dd4432d85af0d983e1fe74a0e8ebe54858c3e7968554c8083be1792ad15ba3e2c7020090645efd27502ac6c2029b17409256641aed121089fe" },
                { "cy", "0600dac7cb060c2482f89fb5cc995d55689c20afd2cd28969332a311672470dc48645b73974e2632414991e5aad6c1a726253fd81100321c404d21c6c7bf5041" },
                { "da", "faa6fec32c75744be936bfee5d71db07eedd515294f51086995689dedf603edf3ba22da8c647f05a2035e13fe4d3d9a2bfe21d82a1ad96d0e68161d52db4c761" },
                { "de", "992379c9a17d7dc857793a826e25a2b8dbd4aac2fb3cfc585975dcf27fd63d2cb41eb6f599573083fdb3869aa361b63cc0bd0ebcdc5c7461c093b15f9cf14935" },
                { "dsb", "c1dc6cacce90383dbc20eeca8c7e9642678cc8e69638b35e2eab253ed6c3bb43be3d3c2c8383871b71df414b693dd804720e7e0ad438d0ec2e3de8979f6e29c7" },
                { "el", "972c866a84ea4e1b37c22a2440c13639663dfa385b4027d10656e06afa5035da50af0bec924c22c1d62b40a508acb81e6da904693e2fe8e183db4ded611d06b1" },
                { "en-CA", "543eaa150d945fd7f11ad274a14f6b2a6108a0da89e0868d95fd2c98c6f2cbbd0d727b35a4cfd3a86be9edd29cb1716b47eaa0ae73f0fc53df52f88ad2715b67" },
                { "en-GB", "29f15c74c2c7e297ded55f2764ca0a209a4dcb9bba695a3784e83921f082dde4d922e7d2e0ee5d26c455ace484e886ea1d4355b2c05c109122586b10f85bceef" },
                { "en-US", "81784184b7a60cda7cddedaf4c43578e69963c39fbf1f5c6fe23e20a91e68227e1e032381d205377a389226ca1365961bf5c4857d21b5c253fa635a4df8748a1" },
                { "es-AR", "1ef76317af5261ed05cab7b8bc48a0116d6e2b702becd90070ca4137b1268879eb0d528ad4ab0788825638255c7fd11dc5f99e53bcfb105753a946019880898f" },
                { "es-ES", "320af5f1f9be6d9da14ab1e57e544c12a0c5a4e746c90b5088a55077eb3145f9211bc8173ca32a24c53a1c857b3e0af016df62ab24791b5c94ea4d6a6918e0d7" },
                { "et", "8562755a839b6214d874b5a5174af8df3b10a4f21dbcd8c2a395498cdc0cde0950fd8f1c6c3659f3856ab6e96f4a4016c3e185815447c072ae083325c35bb47a" },
                { "eu", "965e7c1f55d3e50f7bb4911ef0949928f23035a6929f23acde98fcbdbcb78017ad21254bc67c5060a8f632bf05b4e0c9ee9883be7cf8b3ff14b5e3436b706008" },
                { "fi", "c6d7a382708e567e8a48e02ac084f0a3288a9ec82cbf3fb13c192fbb75e6b30def87566cbfe9f5c768deefe1e4f62f2acc85c78a071c3e6f5b97d70842122b54" },
                { "fr", "40b9323fcb4b6801b75d45db12973d6b114d926ac2f85aacdf4514547831c9294acd19c70723a70a2f19a6dab7e07ed1d347d094ffd8d6880cf55400b92c01b7" },
                { "fy-NL", "6c7bc9ebaed1aeac9e7b83efe06fe2351ae2899095156018012413ef67b3a58cba09de76fd0946c54415c43678eb60f7419ef2f7d52e126b42cd8f244b9d5f69" },
                { "ga-IE", "cb37ccd2f5317b8af403824a6be1dea6a12d1fce26e568a432e765cfacba9f33fead4aa8342c9306e747129f9fd63fbe3991658931e6efeade7bce078a71c4f3" },
                { "gd", "201ebbdb65d1d7512f297102d6713631c650a7e737b50ef9cc8bbf3d25bbeaf1ccb8d43fc0612c045457b237513e4796522bbc0b586d08ce71febbfa8d134939" },
                { "gl", "135313db15a3f26ca3e9ac1af0c872f401e5eacd5e68c307a8100b0b5680f155799fe09b8734f01f39cfd879a55126b9f47b16bf51614810336e9b758d32cd84" },
                { "he", "0c171f09dbbb09b6e9d36bbf9bbce0c586c3185a3c1d1588c431c8fd20ec6c12f0d8479ad4333f358e70227e8c915c1386cc3af708c7c6bd51a036df80a1c400" },
                { "hr", "6ecd828d18554110a7e73230edbf9602e26f6efea8741ed5111b490913f7f2b0558359bb5ab0c3ec2bc130c76a677dea097088b5b488068d769a5cec88847d2e" },
                { "hsb", "cc99419345bcc0eda14463750a3f96e2f1c859e4f1c59d9361707746957f5ed0437e3f92636a4c05aacae5a6c20d7888394a6a72430fef74becccc6243a8e4ea" },
                { "hu", "1e26c7abd6735861e1c85f3a62135761cd32aafb19b601127ca16f2351c094c3519af14950dc34d14a52982d8659ecfa1d59ab6a79e9b7cca1aa9c0a5e02576d" },
                { "hy-AM", "f7cf7a98d7eebf9b510ed29f4040985a24ab43873a6bc6b8a09f0c6007c8ec8d19226c86e694cf6082a2f0854b1e7a3aaabced659d57623345493b00add7b006" },
                { "id", "5e1d1ef483c3ab2dfd7fa163f26aea3718e7a590417a965d325d2eedfba8a7f31798735027147feaa43962af077c5e23a8cde7e5c4ecdf268e04370f4f98a998" },
                { "is", "792a1946a994c27febdcb9f20ee431f584f4a8a58955006d9058cfe2160956eb08de804ddf90be86e2320833f7e7aea4ae8928df325cabe51bd3fd3851ea2226" },
                { "it", "cf84dce1ba6d4314926ac858978a154ab95edba856811a6602df69217a26db183a88eace64b65966b8d6a6d91f84ec48434957283399c8d1f1dbed1dedb3ebe6" },
                { "ja", "c312748a4279f2520703e2a23bd68c90e0cb3657958cb47df2365c1c62b25130a82b50ecb7b02e1a77370cc91fbc3480c04b9f2cca403e968d230ad4df2c93af" },
                { "ka", "a9b621321f21a5d723dc63ef853195731d864a6e2020bdc8d022755989c17e6927b5313e4eaf55257b1afb8ca7247845eb54f483118afac8faaf08366a362ddc" },
                { "kab", "8e918d31700718b03721794c99c22ecca2469a2d9042acf77fe3354d037343785ff1496010ac77be982266b3674fc457e966a5dedc66988304fc8ce608d18bbd" },
                { "kk", "53b19f9839629e9f23af79b22dfbf505ef57601321a54072a1aef0e0bedc9604fa98e8efaf63cb07b550b5f71091ea0f334fb11fb35b007a15ec69a7145b10a8" },
                { "ko", "0cfd5a53f8efe77adc5b1914ee5c226c8a4e6b7296321891efad9f20859aaa8750bf3bd16cadb16f8e503ab926798841f90bf26af87c447e5add8a41fe75b3df" },
                { "lt", "df2a681d4f5d890ec3e29d62957b0615669b3ea31f9b1032fcfd8042b7577579657cbc2fbf022810242e1956cabeaef9dbba77a7bdc6bc6428e6925ea1951379" },
                { "lv", "b7c2a3042fcdb479bd58dca55a240af8b5ce6e39440f0dcdfbbae6afb56e204600806daf80dc3be83b7bc2f506481403b28450b769e57708025760c96f781425" },
                { "ms", "fab06455c078071a922bb7e21137459333a68193960eb942f824d571e470e03cc05bb52a51fee51eb54bbc671a9a323b6928b30414197cfd5a5a06256a4b89ec" },
                { "nb-NO", "622584b0efbfe7bc3a95a874c8f89d2564a7c853ea947011fcd013c1381bb238e7337a4c60ed6e90e822f6afc474f161d473cff4636ef91732af4858bae1cf27" },
                { "nl", "bdaa51dbe96fef152d1ac42598fdfcfe70000a1507a7877a6a6b66f431a4dd6cdf5a3e78ce03ab6d73d5d28bfe9e5ede8b9638fac22aa4b9558003e39c0d8595" },
                { "nn-NO", "cf8b1f06691611ecda2f8fbfab1525dcd2782936a60989c76b5d77db0c30bc0b1d59fd9e865afbd540a5cea3aa76a2ff470b1e8a4590e75cd9bc8c83d2a8189e" },
                { "pa-IN", "f68739210e86b69c3812810c18989d20b32475daea48aed8b9b64f3de797b365d93e56292df29023e9e7fb29b371d7ca2803fd0041bed224cf0abf309e6fe177" },
                { "pl", "d0a5ca1fc25bd113f5448fabd16a27a9955bda49ac7e9be4c7a90088fe1b7817c46d1095b6144c96db8f46e6325e1f8a5534c36a23b52d831a615b6d1f795f51" },
                { "pt-BR", "910ec2a4ea41245cfd1b59000163bfbf273a1ac1113d69c2798c20279d43996ba779bc7bbef76742d3647cd2b9a8a23c28de13f44f9fe0104cf8e41016fbf653" },
                { "pt-PT", "6208884da710050655d4c798b716431351d3b4390d578e24d6cc1b3a485c4fac06602bccc5b4c266e90a6da54b6dee3be626cbbac04b22412e6f7be6bed828fb" },
                { "rm", "c3a48180c70c43fa6664fc53537110ac51af995b25a5344fc20fdfa3ddc71bc8155599f50dfc5513c05e050c1cf732f96465111960d5733edf5c01a467a871da" },
                { "ro", "2e03884ab622e5dc3dec42840a7a48bb154cf66a8ad6c8c4cb0c890654c92ded3f156841f8ed4106e895e173ff4c3da90d882012c3c3a8a1f34dd5b3d11d4a6c" },
                { "ru", "daefe3374d2843c66611ad22fb57125d1969acb7f66d6f7075bf179205008b7306e1533ff89a82cd87242d2271f8e4b67759ff1cabee84a3bb99a509a09cfeca" },
                { "sk", "22e71de82b5717a26e3bbcb958e995fdb570cbf41ee40e78f0a870542d771292eae4a1e4572ffff1b6073a7526953a7f5a79b4ae9583925b0b13afdd94641884" },
                { "sl", "d878a46cb75e7367b2dfdad4191db0fc84b10580bb548ce60533b85fe004ee08d98afc8ed2de32a50a9565eeea671f7546df9add2e4cb4d0dcb69b09a2e056d5" },
                { "sq", "15b11001f7768e6872e9f0d228d38cd9fdd92c7f8291225bdea31c09ca28ce48d2ff3bb47712c59e294ee8a297644524b1c7f5a62737070c5fcf8f665c3e2d98" },
                { "sr", "28e3dc0d214dd4fd7bafbe9d0e8383a830fdafe04479cc962ad7f94b27373dc56f2c252f51b70590fe3917242f744cf1b25d892da683d00f4ad7258d9bfb7919" },
                { "sv-SE", "fae62f3eae2543e59e86093a91913ddbbd40a63b1f85ea5f9c69799fe786a0578ef6c4140bf5c5a75fe882f48fca33f3fceec9e5afeb0100e112a0486d46d829" },
                { "th", "0748d172772b995c7facaaeac5bfaf2dcf18600484dc9cecced84b74932f9ac236d9c319cfe2ba164efaef1b49c368d14300751d144ef570a7beb03248d70a70" },
                { "tr", "d8079f1391cdc4f30326b8eff7c6459a3e4be84fa23b7918f342ea954146790f1351d3400f47904ac6d56c365f986b3d8fa3e1662a44ed6889de4743ae69f12e" },
                { "uk", "59ad5576b47720086e4c7d6fe6110fd1623b49d8e74019962f85fcc92bc356e7c7104fd605c080b3abe6b3e9a17dccc03c8174b6764361ed03c56bd0dc11d406" },
                { "uz", "ec38578d1641dc6fff6ce19bce58c6f45215e5393234e391a4876bec3e17d6f6da1f8f95f50c7d878f1bed5ca8982a711376c83f060f3331f0b75efb3b79a620" },
                { "vi", "5f324a45948995c62853661ec797b92ed2167f014dd0f59ac7b8648ce1b7c40400342b25230f3b257bb5f5efc2c3ce3ada549f8bcbb6a5519c2dd7d444fafdd4" },
                { "zh-CN", "3bbd3261e93ceb380c581537a1f3296c83b17946149f42d2c91bf8359398e134daac390b754705dba62cfb04bbf052e94237747dea07d93e63136a393478ec6d" },
                { "zh-TW", "2d93b2f0ef5809fad90a77fc808626c0a86735da12184ea7832833b4f2494f6dec0a2b264ba7812bf202b168a29f841a62b725c7fabf2046aca003f6fd2bb316" }
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
            var signature = new Signature(publisherX509, certificateExpiration);
            const string version = "91.5.1";
            return new AvailableSoftware("Mozilla Thunderbird (" + languageCode + ")",
                version,
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win32/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win64/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma"));
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "thunderbird-" + languageCode.ToLower(), "thunderbird" };
        }


        /// <summary>
        /// Tries to find the newest version number of Thunderbird.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=thunderbird-latest&os=win&lang=" + languageCode;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Head;
            request.AllowAutoRedirect = false;
            request.Timeout = 30000; // 30_000 ms / 30 seconds
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers[HttpResponseHeader.Location];
                request = null;
                response = null;
                Regex reVersion = new Regex("[0-9]+\\.[0-9]+(\\.[0-9]+)?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;
                
                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Thunderbird version: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// Tries to get the checksum of the newer version.
        /// </summary>
        /// <returns>Returns a string containing the checksum, if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/thunderbird/releases/78.7.1/SHA512SUMS
             * Common lines look like
             * "69d11924...7eff  win32/en-GB/Thunderbird Setup 45.7.1.exe"
             * for the 32 bit installer, and like
             * "1428e70c...fb3c  win64/en-GB/Thunderbird Setup 78.7.1.exe"
             * for the 64 bit installer.
             */

            string url = "https://ftp.mozilla.org/pub/thunderbird/releases/" + newerVersion + "/SHA512SUMS";
            string sha512SumsContent = null;
            using (var client = new WebClient())
            {
                try
                {
                    sha512SumsContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer version of Thunderbird: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } // using
            // look for line with the correct language code and version
            Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // Checksums are the first 128 characters of each match.
            return new string[2] {
                matchChecksum32Bit.Value.Substring(0, 128),
                matchChecksum64Bit.Value.Substring(0, 128)
            };
        }


        /// <summary>
        /// Indicates whether or not the method searchForNewer() is implemented.
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
            logger.Info("Searching for newer version of Thunderbird (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            var currentInfo = knownInfo();
            var newTriple = new versions.Triple(newerVersion);
            var currentTriple = new versions.Triple(currentInfo.newestVersion);
            if (newerVersion == currentInfo.newestVersion || newTriple < currentTriple)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if (null == newerChecksums || newerChecksums.Length != 2
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
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
            return new List<string>(1)
            {
                "thunderbird"
            };
        }


        /// <summary>
        /// Determines whether or not a separate process must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns true, if a separate process returned by
        /// preUpdateProcess() needs to run in preparation of the update.
        /// Returns false, if not. Calling preUpdateProcess() may throw an
        /// exception in the later case.</returns>
        public override bool needsPreUpdateProcess(DetectedSoftware detected)
        {
            return true;
        }


        /// <summary>
        /// Returns a process that must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a Process ready to start that should be run before
        /// the update. May return null or may throw, if needsPreUpdateProcess()
        /// returned false.</returns>
        public override List<Process> preUpdateProcess(DetectedSoftware detected)
        {
            if (string.IsNullOrWhiteSpace(detected.installPath))
                return null;
            var processes = new List<Process>();
            // Uninstall previous version to avoid having two Thunderbird entries in control panel.
            var proc = new Process();
            proc.StartInfo.FileName = Path.Combine(detected.installPath, "uninstall", "helper.exe");
            proc.StartInfo.Arguments = "/SILENT";
            processes.Add(proc);
            return processes;
        }


        /// <summary>
        /// language code for the Thunderbird version
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

    } // class
} // namespace

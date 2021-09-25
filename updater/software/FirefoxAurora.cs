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
        private const string currentVersion = "93.0b9";

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
            // https://ftp.mozilla.org/pub/devedition/releases/93.0b9/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "13d5ca5c1f168492f5772932260c237f8d18f29cf6128ee286439ca2dfca4ae31b56fa29eaefe204b235c507149ddb2eaa0a2736a0ab105dd685a9fcb4dbce18" },
                { "af", "e4fefa0e1da26d2ca43761cd3d3628ae298d1784499b8890712b63938e1cfd3e40bf188824b511544c94811c81fc796b3414ebec7b05609d24019823fa88ae71" },
                { "an", "a921de8582b4dba2308e74650e908129f6023a2775f372e219602b95e3318edd791244b03e45457b311de256077180e952215cdfbed963430a76c7c7ecedc2a8" },
                { "ar", "0fc1ddb580f2b56babc3ca277de399d8cb6a662bd15462cf29af8b8ff0f1ad361864db9e3ce4b5ce70fbbd974e14864e8b1e7ceaf5a22506620eb53c3aa6d4ea" },
                { "ast", "ed226cdf735859beae45d371cd8c6034361b323b89344ddcfdaecd926e141ac121460d3cedb23d8962d3b74a38962742ce24e80f14447992705dc3d88c12e02d" },
                { "az", "b864aaea344004551eced059d04c789286adcb17c4d6d5a684e539560287933ecd316002d2ca1ba808444dd64a2c53531e29d9af6798ec8b5229fe5fee144349" },
                { "be", "6a62fa707b7e69693f129d125f522818416a43b62ce8fe0848215f73f0aa14397f5a5a6069dec0c674a16b1196549e778b49e180d2b6febc9dcb932bd457ed73" },
                { "bg", "682da470f70c1419d96780d420783a3f51761e1ea606b30f7d4e830d263a888b055f1da40ed06a20c99d7edb1dad287be70c1db2a9d38f521da5ab4623e8df8e" },
                { "bn", "df13bfed937db37cec1de120cbc50dfb3f4a7f32e42a6d8cdfe3a2d37f2a05597a8e4eb234a03fce4ad7c082e0e1c33830bc2bdc3e831ce15ee2ec1bb3439d25" },
                { "br", "c2409a634bb943d172cd495bd84fcabffc7c50ff69e59b40a410e75215d2daf0260fa0067d15bb070a905fdc5dc97f5eb78851d0cb79acb9d81fa8cb3771fe95" },
                { "bs", "68ee8e4d9c4a75ec999aad612bdbff3edc023c83c11a027dc717a08b5ea20f19aea76d8440770568814a0513600e0577a85c4edf0db4aa9fe73ffa555a49e055" },
                { "ca", "13ba79c9579019851bcb6a421c46f1db9535ca960cdcea6eff68cd40cfb1f8a48e4f0938af82c67ca6a4e3d9a6071950b9ae40aa05c2c4ccfcb9acc878595fb8" },
                { "cak", "2f24c34383b829fc29f2d4fd245c1bbdf852cfc131975d7f65fb8d115ef8fed8b8eae21cd21f2ef114cc9297c7bec062a8ecb5bf44eb83267ed41493594ca794" },
                { "cs", "51766e95c138b55b97a2dedc83ff06f4b082cb3d3227d091e6bab39892398881f890fb40f27820e04e85e1bf871776170578ef706919508ae95173fc98aa2d51" },
                { "cy", "a2b3a003a364b13d92d985199df98a7212b936fd82d29adee59e74ac18472a4869ae1beb0e8f15090497211765d97329c92943199678294ed5fd228b3661b9cd" },
                { "da", "49b5ae05391a50952ca4630cfd67c38325b363892a70b8a7fe8e2cd89fd801a85f3a6366f983b12320aa2594d994cca558bc08509faeb2fcb1890d5681f5a98f" },
                { "de", "1253355316c20ca290bdc371a5010436614cda8eb1d2a13a0c95a2b9b331e698104405ebe8326a557675303fffae601bd2d4e0212443eadb2b6f3a1faa526104" },
                { "dsb", "7551d4481d59e55816149c7369fbc6249ed5edd4928c24afcc3858e0e6a649b281badb1c257a45f4c6fbe345bc67f14e4b2dd189b658217dd803ad618a20ea33" },
                { "el", "35f253d0c1a8005ed022a1cefa7fe0f8ea5aa3c1b1db3e1e594ddcbfb7fbb68f0d26c40de05e7fce50b4e0ea25c1a8ea04047f550efdfd849efefc49978f915c" },
                { "en-CA", "f8c08c8ac3dea52f91b2dc630e22eda85bd599ea59728ffecc0e47ca3abf2ece946b308918972141ebbc53a191cc000aa1299e3d75ff131c676ae361a451ce3c" },
                { "en-GB", "61e2988bc2be0e41bd414333e3f3549989155e7d690a63e7106deaa4848c06fe04dfbd4e258d93df8c92c2cf92adcad03a7b1ebf71211639c5d57c8574f27690" },
                { "en-US", "27f5543cea1e0c14e3618af2e4322f185ec373a973b7459253f37e2af91cb55bcd67b94db92fca022883915e58317462858eed03f737deb585416304f920c076" },
                { "eo", "8cb3b5d2acfde4112a4ee3c3b74a47c05afed8f18e5db11edcd9d59cc6def2d9f2872190a3ef6f41f9336fb518f527b8f3545aab7b0ae2a3430c7783d98b05b0" },
                { "es-AR", "04854f05658b0fba0e91db58e29d4cbdb2fd95b1ffc63b1aec467a35b6f325077957e333dbab73d3da4ddf00da4d91378dcd7825ce4b975fd1632439692ce5f1" },
                { "es-CL", "68a9b586f6dfc6a357a247bb6e68db7bb03702178a0c1c0d44c9af585f3b8b5601c44903a0a2045c98bc29e9d55cd9d841a2c08c0a91c5e609eb4dc2a599ab69" },
                { "es-ES", "fdbe9fdda4e5bcfa810ace4a4fac88e59d8d5c48f4f63542b8ea318e1361514eafb4319513c7ee1bd5aef3a49c131fb0597a3588828d4a7e778426a0b8518dcd" },
                { "es-MX", "a497a257adef857f51a28e43fade612ca1d3f171ad9069aa0c182fbb6380f35ed23eb283654aa1fe7fdbbedcca5d5fccd128e68b43fa1a6184a3af0c75bc9546" },
                { "et", "18fcd1ba4a1046f43a20796e8a128444b69053f96eefabee79c412077921c4a8753510137cc78e62fb44552edfa231f383be0ad76a3bb511814adac8e484167d" },
                { "eu", "691e5d401fd211c77fce264da7827ddc9b631e9aea7acf584835a9738bd006bdf6b7d92be1fa621de8ca2555e1933fec5aacae7c379a2427785d3f45e14dd927" },
                { "fa", "4a5a9ce809c655871c69bae94557ca460a892e2cf1520ea04d7f750e99f04d6920798bff306b136877ec0c272b8ca01d89172792e2be84774bd8f4e53b927520" },
                { "ff", "35bdb8eb4cb563389d74a4a8dc24fa7ca364e982869e1b54a5f0d640e6175d85fe7980e9e2eb17506447f2c9f21dfec7781614187fbaff81cad51ee957155c8d" },
                { "fi", "82331b3f601fea389da0f27fe0deb47613b2d0b47737eb695e7a69a8c9be1c48f5eec55d2b93586a53046f4f98b3bfb003a9b88c214ac82221cda9ef1c971c6a" },
                { "fr", "fb0b94fc18f2dd51d1980e493781fcd472f2a56a22cc425ef8f52f826a9d030052d3d147493d0bf27d71b46faa5ad0fbc25da877c008119131acfefc26d11724" },
                { "fy-NL", "1744c7521575f802d3cce01769b7def8a4200a6b157258bbf422526051c7e5676e7f7dcb6909a71272d60515fea4777f0ee1b71c2589af1f435daf45c98a6333" },
                { "ga-IE", "a32ccfdfd151dba378f92c539a770912c94090466b58252de2e90ae2662afa00f8d29fb4d968c49df89b36a2a7a56e20ce8827d296b1bc3f3a0a7accf9089fc4" },
                { "gd", "d37dff97353f00269e135e94ed1f1d266aa1291c3a21160244a43cf1701d350520edb251655d324ef0139a71e25b6302c16f7e8e9a7667eb955fdef5855ef98a" },
                { "gl", "b94059045fb2a0cb71341d73e09047a69eb2d3477b4c4700addc37a0c3fd0376bfa85569bf07f7d0a1a57209c5cd84859ce69a74e606c203e2576e97d58fe593" },
                { "gn", "022003a9258f69ceb2a6122e13e881ec257bc8e73382755954b1f3ea28612864cdd8a17b07a28a64985c87266d00fe3945ddd3bd2c3966f59288aa3df1e453c6" },
                { "gu-IN", "10289919a2905153687be5ab033dff4b1534ee1995744efe6e37876cbac3a4f7f4ba5aa9ddb159eaa4bf4bb1ea83d3d3f6ac4441d66aa643ccf55c78c243ecd8" },
                { "he", "1cd2e31b939999472f240b6f2a4c754c836739e5edbebbe170b83fc779b2ae83ed48ddcb6cf21f6dbdfcd58e8ae764b8b96ff92bbc954fbe3cf77412b1b728ed" },
                { "hi-IN", "9e5df9b7903f778b5688b8758927489de80aa6b007bfc82260bc58fed7724ae766f03ddf0d8db83a591ad50585e126218879e471e5aebfa2d5843f1b9de125ac" },
                { "hr", "13452d4fd86af9d67843333af360af3e94a556c3e8aebd3149b6558dc1aef98c27bd66b62a6d1d1d8c20e6f91a1b3461e98e947ac084b84bd8099d95c1cf17ef" },
                { "hsb", "c7efb59464ce2479ba8857f9882461e991fe99f19967778a8ade8512d33b4dab2f705e470538731f5d4f35960ac38548461df07814515f532dfe74c4dca81d4b" },
                { "hu", "e80830084bf331663f84ed33875d63c850258ce5f4bf9b9f1ebe2a8f62abdd0e27097bbbbd498f9589b650f4cc3c526cacb1749b6058cbaf2bd21507ff2f64a5" },
                { "hy-AM", "49f034396c6ec2ada217aec5d8bb8762b40c91bb7c1712375ffef2d70f4f8c3ac19220979efa27d10c3e8d37e7f0669e1a68fbfbda2eeb08f61c7cc64bed637f" },
                { "ia", "b088d0290fdaea84f05604db624f180995fad1f00ebac862f4ece576aec88bb6a14b49ebee62404c0c435aed924d34837cb4df047f8f8eb3ab9f17a3f5dcd42d" },
                { "id", "c57a0e67f2f5e8fe45c31c6415b0ea1c0e3f8780e6de46ac556a77e61263db6457632ab3ce17546beabd2d6af80fad48fb1311130ebc5b5a800b877a03727eb5" },
                { "is", "4e52c45e58de4aadce831d674ff63ca4cf2bbdef728b5ff75f28fbaed3eee6a8aff164b4d0b6dbab7ab39917b32df756ad00aadb56a5ec4ab2b6cbe45d6a6975" },
                { "it", "6abc5b2f121ddb97410f1fcde7ab7c4088a4e3eb305ede521f4a32b205706cfc981027b4b5e377892ac96b577dba0ad96986f53bf53fd2ea8a4f0c47c33db61d" },
                { "ja", "78aa1a2eed358180b13c7b117be9f5a2c3ac548d448b49a579f23ef92218d204f014906732aa3b18ea0ca64705669b55ad2e40c85b900cc63da6256247c410a3" },
                { "ka", "fba62a20aa75ea1bcad2e42d7ef8ddac3528c33de55fedc423f5b02286a02ccf659025a17e020cdab780f0434cfff6fd332dbf1741fab0642ad4f3f5aed76a7e" },
                { "kab", "ff1e49075b9113b7e1fb435c8cd6b0ff1179595a13214b805b5120540bbbef107f750be396c4fd530bd0f7ad77306b37a7a26c2f4c4750d91e0b3191f55212af" },
                { "kk", "0945c9b0983fede7d436fd9332090a4ec5050e175db9cd0404c080b52ca738375e10066ba5008c453cd446f79792e8a16afdf95c6effbb441e1afe331f4a5500" },
                { "km", "d989881db659139bccd4b41d68bff377e3a99559f924ef2f0b34ab55a0c5daaeed7c977f0afe8a7871a626d59c936df112cd993e474019a579961a8fbdf2d0d8" },
                { "kn", "5cd246c194d39c9c713580fe0e8bdaee20637a1a6680f90eb1d7a0ded31923847db16cc24cd975066b91500acc1858226dc6c2eb9c418529a233f65cf8c4eaff" },
                { "ko", "48d25a7c00f9c7f25f72e8fb2c376f402c119e23dcc529c66fc38d2d2c351d665c5ab5e0b19fc2a5953ef04adb101a5a63e7e777f60a6ce2c3790b0963a23242" },
                { "lij", "09e817663d9f72f9be23f0f212e64d4e320388eb9b4f6f56b269c55c2c281130cb2d935a29b21292c4b78d564bdf2da0ef18d120f80a15cea9692f599060977c" },
                { "lt", "eea209cac67e959ee1de7d07f6ed64aeac1a5dfe51c4ce2d44f69c6b3dc9eb4fdd3a56d28400e5a597605400189dbe2f5a287977464bb73e02d3edc94d89f30e" },
                { "lv", "fee3cddbdebd1dd939ee69cfb47c88523378483229218953ddd1bab27ff3d42e99d055db4d77b3b6e4814b2590db2c6216c764e8788a0398b6e019c523bca182" },
                { "mk", "6d7fffb6f759b158d4f9acf0e196c7760ef1c40a20084410b690cfa2994879488875f66448ba3cde405a8d663cfebda9573a16f02b7652e6d31b226694fb0a40" },
                { "mr", "db94c022775813b5af6c3c14d29c773122aee230328bcbf4b299490287cc15163ffd4160eaf285c5802002a288623bd92ee7ec19af4a2ac41e536a2519f3fbbe" },
                { "ms", "5b97f2d335b2c607dae84289e2d4b7360c88e14cec9b0fb30298c42572ab7503e5d17601984aec4c3d9f05b32d3dd1a21843df9fd0831bfcc0150522ee398413" },
                { "my", "326e3005a819e3ac13d3bbd8be0205493b30d29a912fbc7b7be373747bd1095534604a8fa0b1c21be90711553fbcc8998bc54f8ca35fc2493c6fede2d6196744" },
                { "nb-NO", "6364ea2a20ec20765619d32b361826acc3e7ebf6908c0bac7cd8e3ccec571296c072985b1eb683e1accec804ed34d481ae02d61c429a7cbaefdd2a51d681b034" },
                { "ne-NP", "70dbf3e5563c8e23e1da504790ed5477699ee934697093d2e03cb03f2b4ff41bbde6b45ba0bd53a82b2f5a8baaab87e25bd0f6d55ad4d310c2e1c0bd5b884ce6" },
                { "nl", "9ff98491f8b6f77ae19e849376dd95f64031f83d5f5a4ab412fa6db65bda5a4a740d39233e1de439d00fa8744b61f05ab6c80a8176feeb3e43737d3eb6b26c12" },
                { "nn-NO", "25269a17b77f1f6e7171cb0abed6c3c50ddc14af25d727e9d81cd43664d613d9c68b4d099e3be461cb98b97ee3a77562843fe9e9ef8083ec5502732f9dd3b89c" },
                { "oc", "4e850e4b9c9e00061aa5678a3f96dc4277ad6a46934569c64b2056e7ff511d54870788cda770c1fa1b42c3d8e3615b1aca9595b81f3b9d01458ee399e5b19875" },
                { "pa-IN", "aa0a6ae470c54cfc891c9a6ee6794ab72018e971f8d8e729fb905b45bdf1e0cb373addb0c60e5aa255785d09dd79353659478c15885f219c5cac650e62632f66" },
                { "pl", "666daca37da7d0209e6238ba0be35fb848f83b68948fbd08346fc93196e7da31ef1ca89c678254b13dc5984a6470d6a9dd1fbd3735861b905da0a94a76a6e9a2" },
                { "pt-BR", "568ae1c288c6412bb065a32b2f3d7c2f413a383a6308706e6620f79f428c8508e00645153e8c5b22279223b8ad78642680097a3b2e46395ad22cb77a5910f21a" },
                { "pt-PT", "2e6d7f4a7de8bef48f1e1c31c3ad79f088947a1c880b38ff5389ca91bb3cd0b6ca90c39f9d0311e9aae71b4b596501d3ef9ff760971381654b08c52deaf668bc" },
                { "rm", "0051e09e26c436b25689309504b7cd27971523f47db6780e9a1c8c31b835eaae491bcfd3f7e842222ef35bf911d894239064be9da53d5b833daae4685d656554" },
                { "ro", "732b8ac5c1679eb2dc6f0f09d0c69c016ba928390c6ee9aefab49fcc9dd4c485da0fdaafde7c7cf4bec0c2e7f948da2afc3bfddf423c87afcb212355221a946c" },
                { "ru", "60e59c75ce8254fedfe9d13f214d3ea251ce2526853f9721878d1347231ac01c55cf1b4134464d744bdd6290a47a1d1ff4aadcdc8c1e651e39a072584246492d" },
                { "sco", "9e4fda7d96049b24657a9663884ad189f625b8a39c3af0b9c1d7ace6b5ae79bd04e27ab928aabd97a3cff5d83c28b3c079936218cd726e866b1fd41b2a6b82bb" },
                { "si", "c749bafee10a5b20e6a201ff2487c2aca38de86b314cd25f7bac63043701b1c0544e83434cea56494fd7a7e1030253dbc5b2b09ac7c4b5e7c88a7ed916cffe22" },
                { "sk", "792b6042fc09fced5afd6685c780b5bf74776a13233aa65dd9dea773081792bdf963fe3b1902082fe21a9ef3610a1471cb11a7dfbec33d1497e5988f5787d508" },
                { "sl", "71997f2645a8483932edf615218373aa13c9655ce14ef26587ed471f1efab3434451abe8f4c49e52c2d7f7b96b1732b6b537e3737be381ca3b9901566bd63647" },
                { "son", "7428c9e454df9744fe727c10e314d924561e06669192eeb0c4e6a620f42c5bfc6fedb6171d24be9c93abb5ef10390e0023340db37d5d1ed38ae93b70ba40cf60" },
                { "sq", "6e92aeae66970fb616e81586f6f5685b94832b1ae7e78f2136bd0f008362ab66ec889461059ca742cd3034ba6ec12d25ba6aa021d887b6983807c3956ff7b0d8" },
                { "sr", "fe6240b0ac71c59b838661acf753be95f36bb1d66a247d3c3ec264d7bb815d35956ecb2786d9291318e855bf9db2b0b1927c40202188ce3b4734ffa9a9d4fe27" },
                { "sv-SE", "d538542fbbe3c7785a8a1660b0112eaf95ef48276f7211052df5e307a473ec0dd5f148122d3a22ebaca231d7a85b232196c2318b7a358d3f42556308f3e81f2d" },
                { "szl", "5173d8f9af31ec29582f857c53f52e84525b6913a39a3e9f4dee351bfd6122263cae873bedad7a6570bbc87cf1bcb0bd601492870ab57cc44181b690b8eb0b44" },
                { "ta", "9886e96101de9219fb29615a19105886bd56277328fe1b80e4c8a9b58ff43f5e4ff9dbd9e7e0c1b364d0e7e37693b333ee13b40fd14c45e21779171c33f008e4" },
                { "te", "a4a33a72e3df0753cdb9d5c9ef80563edc31772419037d787eba5e4d596e25f4a07a91e59be63a9a311bb6670d5680fc52888edaf98236677b6e931a63537c71" },
                { "th", "de1839055e372878f29e03b59379ccdff3d9da16424e8a98d0d06ddf0b29f464389421c55a4c1a92a320f8f3cb7ecf8cf3789c2b9c7a677df96b586f36f96195" },
                { "tl", "3b98defe5555cc342f90d41456821eac6fbdfd9bcba068a9ecc6c2f2241b61ecdccde122b1de5cdf02fb6ef685256cfd5ff067c12b6544b3f1287f42ae438389" },
                { "tr", "e6fcd46f6164ede1bb574459947b9a9ad9da1429c0afcb6d93d2aa0ba7cf0a9061990d969d7ec6b114b5e29566fbfc06d95bf144daab0f16af213207ffa22051" },
                { "trs", "0ade431233dd029202b7789901456322ce63ad3e4dd6e5958c9b610200955dbc2f550bc939b23fb349f81bb276488b0547d29509478c9adeb7204bce2a949722" },
                { "uk", "153503d4b62b095c2af11a7eb6a9f0e99da4921eb33255e5f4b8bd951cce4477aa50909e43b828c1e249bc9bb459959451c253295d57be75e1042e5a9287af41" },
                { "ur", "a0087554ddd9a1d9540c39b9007003592988cec9bf9113ad045728e41dbeebf4c9e680f9eabd278f6435500e9c63a43306995995580949b1c5e4e514fb45d6cc" },
                { "uz", "d68cbdede7898d6bc453d854321e7693ad64d3333e41d77c6056115d57087165995148fa06ae070896bdb21f88812492ec3794681f7f5c20ceb691d7b57326a7" },
                { "vi", "695b74bc75f09e9e561e6c1fd5ca70be73e8114616ec8f96d8a52c940845358a5020bce9bfce7e61b35e46291c75602604e3e5ec10e7083eb2d3f35bb499b01c" },
                { "xh", "3c3f51e769c21a5dfcde24f90513d7efe0a80c931070652c9f14cac9c03f33e3a064da7ac0950ea67c4c2cb7b0f8ecd04a367dcd3cae73fb3b72583debdd8c8d" },
                { "zh-CN", "cfc60b0042fd0e7cb611612338bf6e5eabe3dbcb4088110c23d2abacca8145bf99876fc97bca4f47d8bbca95a1b324e6c40921124659ea3cbefb9f2e5fdd1855" },
                { "zh-TW", "d1f7576466d1a9e915cc59adda7c80d863f822e7b72ba8524b0ab469414aa94051758b6ce7908e966558a36e0aa6edbcb0d6f5e296d8db45127beac739ad7d85" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/93.0b9/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "98f588a3861169684fe012ec3be09fa39fb6e17ece0586181a82e068657f44e57d01f5f3d2c5b258736f9591c79cb68161814dbea9d63cc64176ec036513a18a" },
                { "af", "cebf8507ccc652d955c66d77685de2546584e4fbfa221dbf0ee6b40762fd45db8c4e6650530b972cc2819280222fa08c5170016e97cd97519ed1b1cdf7b4aaff" },
                { "an", "93e04d2a12555efff9a0f56d824938ab27ae0f404fb8a503f41b919b2bec48ba68a8a2d9620a16c4620d24ffa841f4f444f4d382d24da0e51cccf7963e3da7fc" },
                { "ar", "c31d0e9ee011065de40002a853e843ad1ceff191247e5be1925bb1b5a8c04ee8b51241ae8252ba77e38f9369c9e294c8c1124e4e2a9b96559c1e507b32afccbf" },
                { "ast", "3933faf2a729fd512ba95632875324665e73903b5ecbd0a8b28ad3adec6a0ab6a536d62ea06a2a2de1a0521a31bc78fdcd0f24225ebb4a82eb0ddb810baf24cf" },
                { "az", "3c7b2f33d09d4620e24455e568e7402673728d750a9e4ee9e013947ac229facb6bd5d0462646792b9478981de6e80c7cb40bc9eb0bbd3cc5997aa77939192782" },
                { "be", "3c88bc01b9d337db772fad017928c9d3a058217b3cfa77c5aad0c5c1e0d609d2c429840133b9a373b438bf3f1e6a022919b8e20c4a317562c95a1fd0eb248788" },
                { "bg", "4530404386d47192affb61519205b65f64840d442a73de7ddb87985ccb81468ddcdc611047d92cca0ff3a67f41ae7892496c6029b43c3a98b2b5cb40a8a94347" },
                { "bn", "836c02b749578d152fd21a320ab5cfaec5d2eee38c923abc945cea3a3e9079099013538f82f5b28f2ab673fa772712272cac3bf73b031bd78b15ee68aee5f68f" },
                { "br", "347f6b23f565bea0684f001f81c0595578522f222bcb7afed716c15b63f45d7f5b5b9b22699223c0c5cc85c1b4f88663d636938a330e7d6e251b81cb5920f307" },
                { "bs", "01f49b9a26b41265327dbdbf62a0968fd716d8919101b162765b00d6d3872ebdf2918870092decc689412cbda689f6fb50cb23d64a76f8be851d8b1014d9b371" },
                { "ca", "57ba8b235d4b06e63fc56d5dcf442cf809fdf3b42fc61242c68879ccb049fc10b357082f23f63228625f60c84c15d3eb46763759cf909320def1fc97ec3bfaf6" },
                { "cak", "303691ff80dd7a7bcf8ee409a62e532d5140ec454887c454536280ee740f50cb34ec22ef9074d1d4ea565793d9e5e00e84c8d05c539904057b4f3673e1d3e1bc" },
                { "cs", "c99ba16a9b6c5d2f2d12e73ab5953e0ae2b388c8338fb5d7ac8c1bd293ae9500e5ead783553333b7dc1a1b38c6442e45420c7117690c793f1efbae23179886a4" },
                { "cy", "a9e519c7c9ddf8561512a70bdcb8d698da7540553547d865ff1278f954652620e205fdbb2b9ecb5844aa2198ebfab001bc3989c6de8d7c54c546f6e1f653aea5" },
                { "da", "246251297a48d5860a970156ba79f617c96b5fc67ac0f75396ab5e6a6653b9bfbdcdc446fcd2cb9c069096ea1084480aed6410b28fca4aa8115116b6d7794e56" },
                { "de", "3004402d1f29e3634dde4294c1c1836b426ea4287f23c40dba17a0d99c4ec7cb3e216643f67774e9eaad502694c4ff0341aee538b274ad6e262c5d3795589e20" },
                { "dsb", "d79ef6474d4c6a414c781747e6316bb70c52c0b95a55ef9c1c90dc9cd2df0b77139ddae0008897ce874ea624fa5409c2118ccac902050490039d68c8514ee291" },
                { "el", "c88cea401df7f19eaf74cd775d862c549b33f1aa3a1406f1f0833810c0b4d516cd52bfe4b173c9975c2ebbe205d522339ebe09aef96869f8a16416c448a02d90" },
                { "en-CA", "8aeff22cc721e7b6cce3a919d6a30ebef59535bf7a1c059ee76b8d29caa34aa51c209b8a5c9f4a1ed95fde38299f27ef3fca5cc3cae1432679dda0a490e673b2" },
                { "en-GB", "36133e2c05333fd22195fa98af61a84a7adbafdee705d7381eb5cbcf310cdfe7868bd32e80d1f14101c05aa6ce94b19174a6e90d8134b35ba245c18c74ceee26" },
                { "en-US", "d6763be460996af2cfb86fa1fbb8eee2a852719a923de9e25bd40ea52b01d97d99f310eb714cccae12afd97e87907260d9f60c3f57ef888276f4936a18e4ed6f" },
                { "eo", "d3a2420ff39d4aec632acfa8e588feeb120259c20018cc9db97f0db6cafe93980bfd4c01db9971b057cf47da83a7834272402a7958b68a030c8410d44ad75a70" },
                { "es-AR", "26552437a46ee0176a4a0e9576bf4aa8fd76d875cdc9ff754f82e3fe7181d71a09993943fa2102581aa019b8f41554cd0552b2ac9f98b0c0348c3ac443e8a08e" },
                { "es-CL", "7102b9ee4e841b77f7b54eeaf04af7331e7c403df61e471b23b4b0cd8de9ccd4fe6e68576957c1fb07bdb02514949fea7d435ee037cef8b643705e08364d3c9a" },
                { "es-ES", "e5c44350cac066aa5f528ec598178894d3e153660fd423ea1dee421b40c4db3c0181408b58b2b01069b82bf0a46dbbaf469cbbef6239b3ee40c5e9b9599cf9b0" },
                { "es-MX", "fcdc4eb39ae61268425ee6634e822891f9e656ba3ae10378c2c3de9afd5237e964669ab3e2d1d93f65f6eeba18ff4a6bd7b2fbea42fb046b64006661f4cdb299" },
                { "et", "1701c5e03872202d60aaa7e85a02e1f92c5b403a00a8295920cf744140fd416e45fad227a6c3d638003c25a346320d332219774dc851a7463e5f635aef2e8098" },
                { "eu", "4b44ea02a8d44b264f61fbbb1be936e155dbe840525ecc5a2c6225ae83e667c537d88f2ec20d3e653ff594fc5a06611698f0a49139f89397d0c5f49b3efc1865" },
                { "fa", "a7980b4109d243c238370daa56bdb0192e2ab2301ccb3086d744eef988c5424347e2d9f3a511faf21819a5c4f0aa8b66ffc5c5491ff875eef3cbd132c608a542" },
                { "ff", "5cc5bf60bae777c5f959ea7d5927041de9c242160cbfecd4b17caedc491e37115d6c1037086a640aea005be778f55b76531c029ac40bb0b67d9a6a0512653ac3" },
                { "fi", "0f04d5cb91996bf0bf20fd35ba1113ed498bf5d546281fdf3b26813a06e69c5b7b86ea22efbc0f088bd73e6652a7104785a4d3a4091cd06089d185b4fd3a87b8" },
                { "fr", "d77538e47aee032fafd0ea46a99e88148685dda1b99beee6d8ee61bed138adb808df1eeda51dfb55952b888ca98971fd265044b5655fe5083c1747f6e3923099" },
                { "fy-NL", "2cfb7ef8ecd4ec143fbe065c1f08ea5de4c7c65515184d71731a4a9dfb387792d93334fb80d77c1d723198ff45d3ae589d735b01b65d90a549cc195c1a74ecd3" },
                { "ga-IE", "39a35ea679679c18096bdf2a62034d756ef23e8fa69ed1fb08ee554381047f188622bf3e7329e2fba5f3c44f313dca692361b8177501061b81fb72ee8416c443" },
                { "gd", "d17f54353b9387fbaa7320763976d4881e9fab8661a0a701f641c7554038f491cb8b1ad1768bab4a54123239b27b4c1bab56c12bd19fbbba33c0378aebf63c85" },
                { "gl", "6204400a31cca1397094830f07f0ee5cd216eb0e2848e0422175af36349c30147fda3e341b8f4fa6cea7e185ace23fb0e0e306695bfc66981a69ff2954a37c73" },
                { "gn", "c8f2a0854791a1e89628081c60ab66682a3cfcc48a2954d55b3bddbb55e7cae7bfffa9d2f07715e188ae7d906f092a9dc8a2eb15f886fcd838156c216dd89992" },
                { "gu-IN", "f60cef7a6f43b5b049a106cc9c64d8fc230b26303d51f89a6166109af943438fafdd4d3397bcd7d5bdc7c1b45c6cb7b8cecb426f2900f38f93650163f903b6dd" },
                { "he", "783705200af05f99bb6317fdcd919f85101cb65a943838b9d09b0b886f7433b666177a896f3321e2de5193710c0da3acd27bc23f9d64d7e07392155bf3348672" },
                { "hi-IN", "7a764050c107ed893da5132383d978addeac0d6eac6eda5d95a105856e3d33dda97704b3eef6ebdcb5e8a2856a2275feac0c1ab3a8904947a7eb2c20caa1deb0" },
                { "hr", "4f91f2d3eef81aec62469b21ae4c9db791ef113322a46fca13deb98dc80fc4b877ec911d902f6648714752cc3e49c3dd442af8e0299106734e8605d22ae01aba" },
                { "hsb", "9175c0c15364b1f2f9a5891ba00965089a1962ac57ae78528e1e69ecdfcb41020a1538f78d80477aa0880c97c9d43c75181e2a35b8dd7ec9eb6fc393ea11d34c" },
                { "hu", "c2da413b1bdbaba1a643e259f7198677bd1c3e71bb55ae3cf57e04bda9d6cbf2387db1b015c4b2a767975e449c59c0b613d6eda22addf4039e507a42bfa697f7" },
                { "hy-AM", "3265ec87579a1ee5ce8ece7464ab24c655edbe54fa305d062b079340a037542a6b1252519e82f3518a2f54f1fe63f138c3cc8c0c7bebbd11f7f04a8e289edc7f" },
                { "ia", "fc048c6948e9bf0f4d68cc4ec0d99fd35a9be7011412e42d3649d5f1b0dff8f6d30b872f35913b926a8de82a25d9434c36cfa7a7e623eb820ba79e31d29386b0" },
                { "id", "dc03a3d04d363f276020c7fdcdebbeafb29aead5409a7063cfa2c2b809896c7a27a78d03232fb11d35f6fcc9030bc2bccfdd2495e5eee94094d8e0139ca9bb06" },
                { "is", "061583984395bc16aee9aecddded13fdd7d0ad50f3f0356dcc76cb773c2c8f087eedcf3b58ac553509723b498a12ce14ef39bbc36f6a7ed86596255fffa6c5c7" },
                { "it", "3f953d1280ba81f65f6b1ca8d6814e887c37fa5b78e6f468c8c68e19336596fd5d874be8cfd8485cf1ad44317a7d8c967b2513c87b714631257f6b44d6095660" },
                { "ja", "f7e6365afceb3b5525d6c731ce17ea8e38367f4faf32d385cf1c52871293f1513caad39250fe30d0b1521d9729aa795ed6d076b5bc96debe84d80e2c3147372d" },
                { "ka", "ed8bc982afeb41b187f8a92fa6029445635c037bf00a9c3e88484d282f91177d508f5cf8159ea5f0d045b0e00d0f0395543be7a3a1ede6b7647eef6eda4dd5a2" },
                { "kab", "07de630ca7254d83b7d7c068faa0103deb9a2fe9aeea1f535f3c7bb390406feb4a401b6db2dab42cc3f68dc101f75e2369051e8b4b0dc614db7cac7b025c2fac" },
                { "kk", "abc3b4c6eb08f1b2b8acc601b1c93d1dada6a963477bea4344dd31f4a950c663fa23d9ac836313e9078c9dc13276c8361a1f5880586c499bf96c9e03bc38f4d0" },
                { "km", "aba8ab41d407f2624d68d4909ed08e273dc8fcadf6359817f5bd8021bd67004e7da1b8069af2562f0bf9e4c8ea09ee1b35dd18c65a38cce47a9078649c7bc709" },
                { "kn", "425a0a98494a9e58ebb4e6f4ca73d4d8fa4e17aecb1a9e1e3911edf3b4ca94301ee61e79b5e0217eb4a962a5a76b91da1663397df28f698e3303b1b1dcc2b85c" },
                { "ko", "a69d151663b278366994059fc5b34cd3f1fedd83f955d5424e2910fda9c1d26f8e6621da184fb288eb76ee2008174c2fdb911ba291d06e0ca9d1ab0930fb8d94" },
                { "lij", "2565d50318dc71e71e058e437277e41e6f067b35cd1aaa62cb487a990faacf2532910062eaad763feb541497f829435c6c1925a2d3268348e31540a529877816" },
                { "lt", "ca94105ba11e40885984a2d2bf739bf7a0fa6e7408146886d3deb0d0795bb8cb41dc1a86054acbdbb128a2be96d963db5c434a8c8499174eaff679898fd25cf2" },
                { "lv", "02fb90bda1c9ca09d1a168132c13fd87547580057ffdb358e68bbde98286e94f9365ef83578ad4ded25667d65209e03c0cff4bad54148cab44ab0b04141ea8ab" },
                { "mk", "e55a758467046f0fa8f50855837075c5d08f85631d39616f330c20b8298a23bbb67671d97b1ef4f9dafca7eb633849581012600953c475f7fa485cc18b9c144a" },
                { "mr", "b3e7a63104206d8b7d72ae902dc10cf029d0361d6e7d84c444f8be7263cbd4e751489e0e73deff808e18f05ec8bdad5e0d5bb0208621d326817a14b42998ef08" },
                { "ms", "9feca62b034183e43f643cb524637687c7da241b80e209771bcd3c58caa72f7a14f5eb784721465ce1bf840e267ecb3eaa21953d23f100803fde375f2df5a272" },
                { "my", "eea0ce786865f8a76b4eb2e94ca07c35dcb2c15b9a5016ac7f137092fcb68427fe64458c23e2770b9d204fd68a1511030b29be6a9a85d3bc42d1556387210b15" },
                { "nb-NO", "dae5e267db97b2fe9782fefeea3d269c94d47bff6e52ae9fcf9385c53e6c16d1a8f52d5b553600b8c6e175737dd4ec32ff6af51789d9cd899a324425201d7345" },
                { "ne-NP", "3c8be0f34ab62f75d2520281d867bdfbe9e66cac86f8e8c8ddc9fcbe4e8e4b71151b593f20ae38366758f8db3bf78a030fe6b656b243815a6b20d3c67363b8c0" },
                { "nl", "c7559ef1c89d6606d1695ad1f3f0a7f9bed4312e0494469bd52da610a6a679cb9745c1c73d187ca3b2f5fba0e50793c82d9cb80fcc3e3d59f866328b293e9894" },
                { "nn-NO", "ec099af8206c6ce9fa6980a5dd8b976185c8f54df81300f1ff51b9ee7167d221593bad2acb3d00e1478b49dd15b3cb2c8cbf0e0235e0039e50191669897bfee0" },
                { "oc", "037d494651071ea8c6d86923b9058b4220b70317745dfe97b95ae1c125e4e6f1e10dec530f44928454f6ceb0be93ecfde92fd434a376589818389d46222e4af8" },
                { "pa-IN", "ce25ee4e4312152d1036e5be6e6eaa368fe2394d9e5b183faa1ad71222433769d8de91e359ef3ff96e5cfd330ce04b355bdd6020decaeb5183860ba0027d11cd" },
                { "pl", "aa9556fc1bf7b5a39d786a3067aef765f9d625d8e9f478b5c53a0414c662e2aa0ff6b606ee793cc59c23169a87b8a7567cdc8a16abf131d03240096513a4c565" },
                { "pt-BR", "8f23ccd7346df7772ed559ce5064f32ccb9b7a0c66d2e07665a0d7ce8d7eb66879011aaa9e17ed4d5e0e3e4282397fd8f77b8b6e5755c9921743c7eaa5fb0111" },
                { "pt-PT", "586fe718e742ff9f905400fc0e827595e1051cf9d3cf2e2836a44beaa028bbdf4153c9684e2bd0e744edd33053561bd73cebb9670097bb3c53f6ba76e6275b37" },
                { "rm", "d76f78f3216c6393cd8b5d54821177d20e176757d3569545930debf62d74dac8591e20954874530e6e6522fde5ee6da7bdf644d0a840405dcd691b4be88c46c5" },
                { "ro", "27468f3c394795f010bf8cf3ab3c6b953cfad894cf76e4745f0d8c32f5109971c27e9cda668786a631d03ec79925ee69739efdf8d7f9638d1a5cc32576c7c10c" },
                { "ru", "43d26ea769256f23a68f4d15401b063416f0ef088e95ac84b0f9fdede8b452080a9baeecb3d4bfe04e9895efd68c238e56db77c99c4435b1cc9f1ca0ae033720" },
                { "sco", "0000cdb9f00e60316bb32779a8441f052e1ea04c814e75c7ade9eb0574c27966ee9672fe72dd687d7f6063ca43bff381a8e7e704ac2488f251f6e597e3d71383" },
                { "si", "6cc340119085006158d285054924c151b576616af5ef29aa21492b7e655de85378e2f75f6475873ac3fd457def4c238523968d39ce693003bccaa28fea95de69" },
                { "sk", "f3d0430d9431118f1fe16b3113e9ab17b6511b309ca0816afe06517d1b80bb41a970dbce9545cc835cec5e54a3da1c4db37c7a02c4c3186a3676edf0b06ecf48" },
                { "sl", "21c9abc059abdd7374f642095725f864c504768c50f5132b4f0e70213cbc5f5de32c4d188f2c9a7c9021d40d719a01ad3a243d1823e668afdecec1c4cccf0eeb" },
                { "son", "d5fb47013d325b3f9ce4c885122fd8ad8331f2fad7adc8e7ec0ee976a63c61876ddc92ecd70c43753041220b89b245439a077a2f48242faa116b0c0caf773c58" },
                { "sq", "d1d74a1369b9524d6baf4a858881731c2d410184574bb224404593eeb0adf85bf853a171bc165b69fa6494643bdf8fa23de564541841f8666079435607b95904" },
                { "sr", "af55126407a579559f4205b4af00cbd049c1a2d5208b618007e9ff56a640b6dded893aa14378c8a8b52cba688de4d9f3582d558c44e79b00726663c9f80b83e6" },
                { "sv-SE", "9eba1b46a182fc9b7add9a4d0eaa3b09a08b40c88ab36ad2c265c4b51bc5d9073c7b5e70acc0fece0a7d7a65abbc074f7344bddc19cf692b0034dea59c7c7fac" },
                { "szl", "610bb455e32c904feb65d573b5e9d077941380c9e6299edc577234e5af466bf21d43e3889873bbd6eda44c767786ff78e84599b43f375e37b61fb0f4b6794a9b" },
                { "ta", "adff92481ae0d7a8dd1b2c7874fb32721edfd41d1c8f5068186ffe53ef33cc85d9c20c601dc6826559380734665a6fde0081131e102531a681023c20a1fdefe5" },
                { "te", "2e84463fc1d1f62963c244a4bdb9effeb432ff4d3f37be334b024ec77a3a5efb09ab3d03a5feb6f7350bd9d3a02194a49cbd6f5fcee3819a1f1038492c5dee60" },
                { "th", "1648003cc12be6a4cf2be34c0695acbfc108612c2bce9e91a07d369180f9c58a04307a66ffb5e5eaec023389f978d66e13dbb3b4200934c09238255b43882e25" },
                { "tl", "c0defa1ea640b6d98743fb43d4620e2790f241a09f43b72155ec90e09db4666e109c9f3b1f617eb3c36d660a071d458a0ced1cef75f15e7ea7b7eb57f010e7d8" },
                { "tr", "f83b34631ab86e9a346368c6dce1ab9c2dd0c5f96ced5cdba7726e3961ec44dd51f19c808bba55313237a89d7c1c29358d4008c52c6d5ec2cc740e0f7e027d7c" },
                { "trs", "0017e823794e0d71d547fc6d44707929cb533de26b1e21ec5ff4b85f6def94592c0d00628adf4ccd994992acb91d597c3138b5504c12bd1427eab36e61d6135d" },
                { "uk", "a49e66203e289406ed5def9c2e326a2ce4d87fb58edb6dc35d6e791ae29d981e17c128c7eef967a69c325e36d2f430d5b2b95b62704eb983cfc5efb37c402e6e" },
                { "ur", "de979590941f8ff538b88beb5ad6a69998307d4ff48824d8ae00f41dbb1364ba16245e8173cc4ff3c2815e014ebb700a8a501d6dc9cb81794218044bd0aacf41" },
                { "uz", "c4f3ce146ecdc16bc361f5b57a93f2c8cb9e0958dd6dc1d8e932840ad55a7ffbd55b32ec5020aee54a4729d71dcd24159e9a9f20f449d7a777a1ffd7cde91d8e" },
                { "vi", "6acf948145d97fae28eb0b0c11d526636ba4a055de8835d46683446276613f062d8183dbc8ef1cd320a8bdbee7daea08b71b114d888a9a553a753140a86c34f2" },
                { "xh", "99cff998fc5110f9dab940e377bbeb02dfde5354e61a046d4bd14c301763a001865691e272f6f9ff8d3946443c5a6907b9304e3cd4e80ac1771853fc4942803b" },
                { "zh-CN", "0157dca456a0f9d465d3833d86b9ee46eea13fadab78c22a67be6d848cb653af8b5b3415a6e36077cb56aaf3f8c2a39213456f1e7fc0d9b8f25f45a11903c763" },
                { "zh-TW", "95d3764313ac8c3de8a5affe241fcf09ae5fdc5bd984d9443f05316b919ccd8df82f95b9bc2b1db3063d96c2f15d4a6af1c016fff1062cb111b6269982cf6e4b" }
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
